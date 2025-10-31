import os
import platform
import math
import numpy as np
from typing import Optional, Tuple, List, Dict
from PIL import Image

try:
    import pyrender
    _PYRENDER_AVAILABLE = True
except Exception as _e:
    _PYRENDER_AVAILABLE = False




class IFCOffscreenRenderer:
    """
    Minimal utility to render an IFC model (via trimesh) using pyrender offscreen,
    with orbit camera control and image export.
    """
    def __init__(
        self,
        mesh,
        background: Optional[Tuple[int, int, int, int]] = (255, 255, 255, 0),
        fov_y_deg: float = 50.0,
        light_intensity: float = 2400.0,
        add_floor: bool = False,
    ):
        if not _PYRENDER_AVAILABLE:
            raise RuntimeError("pyrender is not available. Please `pip install pyrender PyOpenGL` "
                               "and ensure an offscreen backend is available (EGL/OSMesa).")

        # Try to help headless setups: prefer EGL if available.
        os.environ.setdefault("PYOPENGL_PLATFORM", "egl")

        self.mesh_trimesh = mesh
        self.scene = pyrender.Scene(bg_color=background if background else None)
        self.fov_y_deg = fov_y_deg
        self.background = background

        # ---- Add geometry
        render_mesh = pyrender.Mesh.from_trimesh(self.mesh_trimesh, smooth=False)
        self.mesh_node = self.scene.add(render_mesh)

        # ---- Bounds & target
        self._compute_bounds()
        self.target = self.centroid.copy()

        # ---- Camera
        self.camera = pyrender.PerspectiveCamera(yfov=np.deg2rad(self.fov_y_deg))
        cam_pose = np.eye(4)
        self.cam_node = self.scene.add(self.camera, pose=cam_pose)

        # ---- Lights: one directional + some fill
        key = pyrender.DirectionalLight(intensity=light_intensity)
        fill = pyrender.DirectionalLight(intensity=light_intensity * 0.4)
        rim = pyrender.DirectionalLight(intensity=light_intensity * 0.3)

        self.key_node  = self.scene.add(key,  pose=np.eye(4))
        self.fill_node = self.scene.add(fill, pose=np.eye(4))
        self.rim_node  = self.scene.add(rim,  pose=np.eye(4))

        if add_floor:
            self._add_floor()

        # ---- Renderer (single persistent context)
        self.renderer = None
        self._viewport = (800, 600)  # initialized; will be resized per snapshot
        self._ensure_renderer(*self._viewport)

        # Default to an isometric-ish view
        self.set_camera_spherical(azimuth_deg=45.0, elevation_deg=30.0, distance_scale=1.6)

    # ---------- internals ----------
    def _ensure_renderer(self, width: int, height: int):
        if (self.renderer is None) or (self._viewport != (width, height)):
            if self.renderer is not None:
                self.renderer.delete()
            self.renderer = pyrender.OffscreenRenderer(viewport_width=width, viewport_height=height)
            self._viewport = (width, height)

    def _compute_bounds(self):
        if len(self.mesh_trimesh.vertices) == 0:
            self.centroid = np.array([0.0, 0.0, 0.0])
            self.extents = np.array([1.0, 1.0, 1.0])
            self.radius = 1.0
            return
        bbox = self.mesh_trimesh.bounds  # (min, max)
        mins, maxs = bbox
        self.centroid = (mins + maxs) * 0.5
        self.extents = (maxs - mins)
        self.radius = np.linalg.norm(self.extents) * 0.5

    def _look_at(self, eye: np.ndarray, target: np.ndarray, up: np.ndarray = np.array([0, 0, 1.0])):
        """
        Build a 4x4 camera pose matrix looking from eye -> target with given up.
        Note: pyrender expects OpenGL convention; camera looks along -Z in its local frame.
        """
        f = (target - eye)
        f = f / (np.linalg.norm(f) + 1e-9)
        u = up / (np.linalg.norm(up) + 1e-9)
        s = np.cross(f, u)
        s = s / (np.linalg.norm(s) + 1e-9)
        u2 = np.cross(s, f)

        m = np.eye(4)
        # Camera space: X to right, Y up, Z back
        m[0, :3] = s
        m[1, :3] = u2
        m[2, :3] = -f
        m[:3, 3] = eye
        return m

    def _add_floor(self):
        # Add a thin quad under the model for grounding (optional)
        z = self.centroid[2] - self.extents[2] * 0.5 - 0.01
        size = float(max(self.extents.max(), self.radius * 4.0))
        half = size * 0.5
        verts = np.array([
            [-half, -half, z],
            [ half, -half, z],
            [ half,  half, z],
            [-half,  half, z],
        ])
        faces = np.array([[0, 1, 2], [0, 2, 3]])
        import trimesh
        tmesh = trimesh.Trimesh(vertices=verts, faces=faces, process=False)
        floor_mesh = pyrender.Mesh.from_trimesh(tmesh, smooth=False)
        self.scene.add(floor_mesh)

    # ---------- public API ----------
    def set_camera_spherical(
        self,
        azimuth_deg: float,
        elevation_deg: float,
        distance_scale: float = 1.5,
        up_axis: str = "z"
    ):
        """
        Orbit camera around the model:
          - azimuth_deg: rotation around up-axis (0 = +X, 90 = +Y)
          - elevation_deg: angle above the horizon (0 = level, 90 = top-down)
          - distance_scale: multiplier for auto distance based on model size
          - up_axis: 'z' (IFC typical) or 'y'
        """
        up = np.array([0, 0, 1.0]) if up_axis.lower() == "z" else np.array([0, 1.0, 0])

        # Compute a reasonable distance from FOV and model size
        # Ensure full model fits in view; use the largest extent as proxy.
        extent_max = float(max(self.extents.max(), 1e-3))
        fov = np.deg2rad(self.fov_y_deg)
        # Distance to fit height: d = (0.5*extent) / tan(fov/2)
        d_fit = (0.5 * extent_max) / math.tan(max(1e-3, fov * 0.5))
        distance = max(self.radius * 1.1, d_fit) * distance_scale

        az = math.radians(azimuth_deg)
        el = math.radians(elevation_deg)

        # Spherical around target (X-right, Y-forward, Z-up)
        x = self.target[0] + distance * math.cos(el) * math.cos(az)
        y = self.target[1] + distance * math.cos(el) * math.sin(az)
        z = self.target[2] + distance * math.sin(el)
        eye = np.array([x, y, z])

        cam_pose = self._look_at(eye, self.target, up)
        self.scene.set_pose(self.cam_node, pose=cam_pose)

        # Move lights to roughly match a classic 3-point setup around the same eye pos
        def pose_from_dir(az_off, el_off, dist_mul):
            daz = math.radians(azimuth_deg + az_off)
            delv = math.radians(elevation_deg + el_off)
            r = distance * dist_mul
            lx = self.target[0] + r * math.cos(delv) * math.cos(daz)
            ly = self.target[1] + r * math.cos(delv) * math.sin(daz)
            lz = self.target[2] + r * math.sin(delv)
            return self._look_at(np.array([lx, ly, lz]), self.target)

        self.scene.set_pose(self.key_node,  pose=pose_from_dir( 20,  20, 1.0))
        self.scene.set_pose(self.fill_node, pose=pose_from_dir(-60,   5, 1.2))
        self.scene.set_pose(self.rim_node,  pose=pose_from_dir(140,  45, 1.4))

    def snapshot(
        self,
        out_path: str,
        width: int = 1600,
        height: int = 1200,
        super_sample: int = 1,   # 1=no SSAA, 2=2x, 4=4x
        tone_map: bool = True
    ):
        """
        Render and save an image. If super_sample>1, renders larger and downsamples for sharp edges.
        """
        if super_sample < 1:
            super_sample = 1
        W = int(width * super_sample)
        H = int(height * super_sample)

        self._ensure_renderer(W, H)
        color, _ = self.renderer.render(self.scene)

        img = Image.fromarray(color)
        if super_sample > 1:
            img = img.resize((width, height), resample=Image.BICUBIC)

        if tone_map:
            # Light adaptive tweak (very light touch)
            img = self._simple_tone_map(img)

        os.makedirs(os.path.dirname(os.path.abspath(out_path)) or ".", exist_ok=True)
        img.save(out_path)

    def snapshot_views(
        self,
        out_dir: str,
        basename: str,
        views: List[Dict],
        width: int = 1600,
        height: int = 1200,
        super_sample: int = 2
    ):
        """
        Batch render multiple views.
        views: list of dicts with keys:
           { 'name': 'iso', 'az': 45, 'el': 30, 'scale': 1.6 }
        """
        os.makedirs(out_dir, exist_ok=True)
        for v in views:
            az = v.get("az", 45.0)
            el = v.get("el", 30.0)
            sc = v.get("scale", 1.6)
            nm = v.get("name", f"az{int(az)}_el{int(el)}")
            self.set_camera_spherical(azimuth_deg=az, elevation_deg=el, distance_scale=sc)
            self.snapshot(os.path.join(out_dir, f"{basename}_{nm}.png"), width=width, height=height, super_sample=super_sample)

    def _simple_tone_map(self, img: Image.Image) -> Image.Image:
        # Subtle S-curve contrast to avoid flat renders
        arr = np.asarray(img).astype(np.float32) / 255.0
        gamma = 1.0
        arr = np.clip(arr, 0.0, 1.0) ** gamma
        # gentle contrast curve
        arr = 0.5 + (arr - 0.5) * 1.05
        arr = np.clip(arr, 0.0, 1.0)
        return Image.fromarray((arr * 255.0 + 0.5).astype(np.uint8))

    def dispose(self):
        if self.renderer is not None:
            self.renderer.delete()
            self.renderer = None
