from annotated_types import Len
from black import target_version_option_callback
from core import dataset, mongodb,IFCOffscreenRenderer
from core.logger import Logger 
from typing import Optional, Tuple, Callable, Dict
from PIL import Image
import json
import ifcopenshell
import ifcopenshell.geom
import re
import tempfile
import requests
import os
import math
import numpy as np
import trimesh
import pyrender


'''
Converter class for handling IFC conversion.
'''

processing_workspace = None
processing_project  = None
processing_version = None


class converter:

    
    def print_progress(done, total):
        if total:
            pct = (done / total) * 100
            Logger.info(f"\rDownloading... {pct:.1f}% ({done}/{total} bytes)")
        else:
            Logger.info(f"\rDownloading... {done} bytes")
        

    def _disposition_filename(content_disposition: str) -> Optional[str]:
        """
        Extract filename from a Content-Disposition header, if present.
        Handles RFC5987 (filename*=) and plain filename=.
        """
        if not content_disposition:
            return None

        # filename*="UTF-8''some%20name.ifc"
        m = re.search(r"filename\*\s*=\s*([^']*)'[^']*'([^;]+)", content_disposition, flags=re.I)
        if m:
            # the second group is the URL-encoded filename portion
            from urllib.parse import unquote
            return unquote(m.group(2).strip().strip('"'))

        # filename="some name.ifc"
        m = re.search(r'filename\s*=\s*"([^"]+)"', content_disposition, flags=re.I)
        if m:
            return m.group(1).strip()

        # filename=plain.ifc
        m = re.search(r'filename\s*=\s*([^;]+)', content_disposition, flags=re.I)
        if m:
            return m.group(1).strip().strip('"')

        return None

    """
    Download an IFC file from a URL and optionally open it with IfcOpenShell.

    Returns:
        (local_path, ifc_model_or_None)

    Args:
        url: http(s) or pre-signed URL to the IFC file.
        dest_dir: folder to save the file. Defaults to the system temp dir.
        filename: force a name (e.g., "model.ifc"); if None, infer from headers or URL.
        headers: optional HTTP headers (e.g., {"Authorization": "Bearer <token>"}).
        timeout: per-request timeout in seconds.
        verify_tls: set False to allow self-signed certs (not recommended).
        size_limit_mb: if set, abort if file exceeds this size.
        open_with_ifcopenshell: open and return a model if IfcOpenShell is installed.
        progress: optional callback called with (downloaded_bytes, total_bytes or None).
    """
    def load_ifc_from_url(
            url: str,
            dest_dir: Optional[str] = None,
            filename: Optional[str] = None,
            headers: Optional[Dict[str, str]] = None,
            timeout: int = 30,
            verify_tls: bool = True,
            size_limit_mb: Optional[int] = None,
            open_with_ifcopenshell: bool = True,
            progress: Optional[Callable[[int, Optional[int]], None]] = None,  # (downloaded_bytes, total_bytes)
        ) -> Tuple[str, Optional[object]]:

        dest_dir = dest_dir or tempfile.gettempdir()
        os.makedirs(dest_dir, exist_ok=True)

        with requests.get(url, stream=True, headers=headers or {}, timeout=timeout, verify=verify_tls) as r:
            r.raise_for_status()

            # Determine filename
            if not filename:
                cd = r.headers.get("Content-Disposition", "")
                inferred = converter._disposition_filename(cd)
                if not inferred:
                    # fallback to URL path segment
                    from urllib.parse import urlparse
                    path = urlparse(url).path
                    inferred = os.path.basename(path) or "download.ifc"
                filename = inferred

            # Ensure .ifc extension if none/unknown
            if not os.path.splitext(filename)[1]:
                filename += ".ifc"
            elif not filename.lower().endswith((".ifc", ".ifczip", ".ifcxml")):
                # keep original but you may enforce .ifc if you want strictness
                pass

            local_path = os.path.join(dest_dir, filename)

            # Check content length (if provided)
            total = r.headers.get("Content-Length")
            total_int = int(total) if total and total.isdigit() else None
            if size_limit_mb and total_int and (total_int > size_limit_mb * 1024 * 1024):
                raise ValueError(f"Remote file is larger than the size limit ({size_limit_mb} MB).")

            # Stream to disk
            downloaded = 0
            chunk_size = 1024 * 1024  # 1 MB
            with open(local_path, "wb") as f:
                for chunk in r.iter_content(chunk_size=chunk_size):
                    if not chunk:
                        continue
                    f.write(chunk)
                    downloaded += len(chunk)
                    if progress:
                        progress(downloaded, total_int)

            # Final size validation
            if size_limit_mb:
                final_size = os.path.getsize(local_path)
                if final_size > size_limit_mb * 1024 * 1024:
                    try:
                        os.remove(local_path)
                    except Exception:
                        pass
                    raise ValueError(f"Downloaded file exceeds size limit ({size_limit_mb} MB).")

        # Optional: open with IfcOpenShell
        model = None
        if open_with_ifcopenshell and ifcopenshell is not None:
            # Supports .ifc, .ifcxml; .ifczip may require extraction beforehand
            if local_path.lower().endswith(".ifczip"):
                # Simple .ifczip extractor (optional)
                import zipfile
                with zipfile.ZipFile(local_path, "r") as zf:
                    # Extract the first IFC-like entry
                    candidates = [n for n in zf.namelist() if n.lower().endswith((".ifc", ".ifcxml"))]
                    if not candidates:
                        raise ValueError("No IFC file found inside IFCZIP.")
                    inner_name = candidates[0]
                    extract_dir = tempfile.mkdtemp(prefix="ifczip_")
                    extracted_path = zf.extract(inner_name, path=extract_dir)
                    model = ifcopenshell.open(extracted_path)
            else:
                model = ifcopenshell.open(local_path)

        return local_path, model

    
    def _ifc_to_trimesh(model) -> trimesh.Trimesh:
        import ifcopenshell, ifcopenshell.geom
        s = ifcopenshell.geom.settings()
        s.set(s.WELD_VERTICES, True)
        s.set(s.USE_WORLD_COORDS, True)
        it = ifcopenshell.geom.iterator(s, model); it.initialize()
        meshes = []
        while True:
            shape = it.get()
            if shape is None: break
            g = shape.geometry
            if len(g.verts) and len(g.faces):
                verts = np.asarray(g.verts, dtype=float).reshape(-1, 3)
                faces = np.asarray(g.faces, dtype=np.int64).reshape(-1, 3)
                meshes.append(trimesh.Trimesh(vertices=verts, faces=faces, process=False))
            if not it.next(): break
        if not meshes:
            return trimesh.Trimesh(vertices=np.zeros((0,3)), faces=np.zeros((0,3),dtype=np.int64), process=False)
        return trimesh.util.concatenate(meshes)


    """
    Convenience: download IFC, open with IfcOpenShell, convert to mesh, return ready renderer.
    """
    def load_and_prepare_renderer_from_url(ifc_url: str, dest_dir: str = r".\download") -> Tuple[str, object, IFCOffscreenRenderer]:

        path, model = converter.load_ifc_from_url(
            ifc_url,
            dest_dir=dest_dir,
            timeout=60,
            size_limit_mb=500,
            open_with_ifcopenshell=True,
            progress=converter.print_progress
        )
        if not model:
            raise RuntimeError("Failed to open IFC model.")
        renderer = converter.build_renderer_from_ifc_model(model)
        return path, model, renderer

    """
    Convert IFC -> trimesh and prepare an offscreen renderer ready for snapshots.
    """
    def build_renderer_from_ifc_model(model) -> IFCOffscreenRenderer:
        mesh = converter._ifc_to_trimesh(model)
        Logger.info(f"TriMesh: verts={len(mesh.vertices)}, faces={len(mesh.faces)}")
        return IFCOffscreenRenderer.IFCOffscreenRenderer(mesh, background=(255, 255, 255, 0), fov_y_deg=50.0, light_intensity=2400.0)



    '''
    Main conversion process 
    '''
    def convertion_process():
        ## Load target model version
        mongo = mongodb.MongoHelper(collection="workspace")
        workspace_item = mongo.get_item_by_guid(dataset.argument["workspace"])


        ## Get target model version
        if not workspace_item:
            processing_workspace = workspace_item
            Logger.error("Failed to fetch workspace item.")
            return


        project_item = workspace_item.get("projects", dataset.argument["project"])
        if not project_item:
            Logger.error("Failed to fetch project item.")
            return

        for pro in project_item:
            version_item = pro.get("versions", dataset.argument.get("version"))
            if version_item and len(version_item) == 1:
                processing_project = pro
                processing_version = version_item[0]
                break
        
        if not processing_version:
            Logger.error("Failed to fetch version item.")
            return

        filename = processing_version.get("originalFileName")
        ifc_path = dataset.configuration.get("domain") + processing_version.get("sourceFile")

        Logger.info("=== Get IFC File Path ===")
        Logger.info(f"In Project   : {processing_project.get("projectName")}")
        Logger.info(f"IFC File Name: {filename}")
        Logger.info(f"IFC File Path: {ifc_path}")

        ## Download and load IFC file

        path, model = converter.load_ifc_from_url(
            ifc_path,
            dest_dir=r".\download",
            timeout=60,
            size_limit_mb=500,  # optional safety, TODO, add to config
            open_with_ifcopenshell=True,
            progress=converter.print_progress
            )

        Logger.info("Saved to: " + path)

        if not model:
            Logger.error("Not model loaded, Stop process")
            return

        # Example: list all products count
        products = model.by_type("IfcProduct")
        Logger.info(f"Product Elements: {len(products)}")


        renderer = converter.build_renderer_from_ifc_model(model)



     