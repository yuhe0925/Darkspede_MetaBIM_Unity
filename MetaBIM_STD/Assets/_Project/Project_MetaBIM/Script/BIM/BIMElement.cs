using MetaBIM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Linefy;
using System;
using Unity.VisualScripting;
using System.Linq;
using System.Xml.Linq;
using IfcToolkit.IfcSpec;

/// <summary>
/// This class is not stored in DB, only action as birdge for the viewer
/// </summary>
[Serializable]
public class BIMElement : MonoBehaviour
{
    [Header("Linked Object")]
    public BimObject BimObject;
    public int SelectedVersion;

    [Header("Data Buffer")]
    public MeshFilter Filter;
    public MeshRenderer Renderer;
    public MeshCollider Collider;
    public Material[] OriginalMaterials; // sometimes missing
    public bool IsObjectTransparent = false;

    public Vector3[] vertices = new Vector3[0];
    public int[] triangles = new int[0];

    [Header("Border Edges")]
    public List<BimFace> Faces = new List<BimFace>();
    public int BorderEdgeCount;
    public Color LineColor = Color.black;
    public float LineWidth = 1f;
    public Lines BorderLines;
    public List<Line> Lines = new List<Line>();
    public bool isDrawBorder;


    [Header("Zone Information")]
    public List<GameObject> SplitedObjects = new List<GameObject>();
    public bool isShowZoneObject;
    public bool isSplitedProcessed;   // this is not a globel status, this only check if the object is processed
    public int NumberOfZone;


    [Header("Status Information")]
    public bool IsElementHide = false;
    public bool IsIsolated = false;
    public bool IsSelected = false;
    public bool IsSubElement = false;
    public bool IsAssigned = false;   // been assigned to custom data tree



    [Header("Linked Object")]
    public StructureNode LinkedNodeItem;


    public Vector3 InScenePosition;
    
    public BimObjectRecord GetRecord(int _versionID)
    {
        if (_versionID >= BimObject.records.Count)
        {
            return null;
        }

        return BimObject.records[_versionID];
    }

    
    public BimObjectRecord GetCurrentRecord()
    {
        return BimObject.records[BimObject.versionID];
    }


    #region Event

    void Start()
    {
        isDrawBorder = false;
        SetBorderLine();
    }

    public void SetBorderLine()
    {
        if (BorderLines == null && Lines.Count > PageStatus.MIN_MODEL_EDGE_LINE && Lines.Count < PageStatus.MAX_MODEL_EDGE_LINE)
        {
            BorderLines = new Lines("", Lines.ToArray(), false, 1f);
        }
    }


    void Update()
    { 
        if (BorderLines == null)
        {
            return;
        }

        if (IsElementHide || IsIsolated)
        {
            return;
        }
        
        if (PageStatus.IsDrawingModelEdge && Lines.Count > 4)
        {
            BorderLines.Draw();
            return;
        }

        if (isDrawBorder && Lines.Count > 4)
        {
            BorderLines.Draw();
        }
    }

    /*

    public void OnMouseEnter()
    {
        if (MouseInputUIBlocker.BlockedByUI)
        {
            return;
        }

        ProjectModelHandler.Instance.CurrentHoveringObject = gameObject;
        Page_BIMViewer.Instance.HoverOnMeshObject(gameObject);
    }

    
    public void OnMouseExit()
    {
        if (MouseInputUIBlocker.BlockedByUI)
        {
            return;
        }

    }



    // Left clicked
    public void OnMouseDown()
    {
        if (MouseInputUIBlocker.BlockedByUI)
        {
            return;
        }


    }

    // right clicked
    public void OnMouseUp()
    {
        if (MouseInputUIBlocker.BlockedByUI)
        {
            return;
        }

        Page_BIMViewer.Instance.SelectMeshObject(gameObject);
    }

    */
    
    #endregion


    #region Object Interaction


    // a full retor
    public void RestoreObject()
    {
        if (Collider != null)
        {
            Collider.enabled = true;
        }
     
        if (Renderer != null)
        {
            Renderer.enabled = true;

            if(OriginalMaterials != null)
            {
                if (OriginalMaterials.Length > 0)
                {
                    Renderer.materials = OriginalMaterials;
                }
            }

            //isDrawBorder = true;
        }

        isDrawBorder = false;

        IsElementHide = false;
        IsIsolated = false;
        IsSelected = false;
        
        SetSplitedObjects();
    }

    public void RestoreObjectFromIsolate()
    {
        IsIsolated = false;
        
        if (Collider != null)
        {
            Collider.enabled = true;
        }

        if (Renderer != null)
        {
            Renderer.enabled = true;

            if (OriginalMaterials != null)
            {
                if (OriginalMaterials.Length > 0)
                {
                    Renderer.materials = OriginalMaterials;
                }
            }

            //isDrawBorder = true;
        }

        isDrawBorder = false;
        IsSelected = false;

        SetSplitedObjects();
    }

 
    public void SetToHoverMode()
    {
        if (Renderer != null)
        {
            List<Material> mats = new List<Material>();

            for(int i = 0;i < OriginalMaterials.Length; i++)
            {
                mats.Add(ResourceHolder.Instance.BIM_OBJECT_ONHOVER);
            }

            Renderer.materials = mats.ToArray();
            isDrawBorder = true;
        }
    }

    public void SetToSelectedMode()
    {
        if (Renderer != null)
        {
            // set material to selected
            List<Material> mats = new List<Material>();

            for (int i = 0; i < OriginalMaterials.Length; i++)
            {
                mats.Add(ResourceHolder.Instance.BIM_OBJECT_SELECTION);
            }

            Renderer.materials = mats.ToArray();
            isDrawBorder = true;
        }
    }

    // not interactable
    public void SetToHideMode()
    {
        if (Collider != null)
        {
            Collider.enabled = false;
        }

        if (Renderer != null)
        {
            Renderer.enabled = false;
        }

        isDrawBorder = false;
        IsElementHide = true;

        SetSplitedObjects();
    }


    // the element is set to isolation mode when those elements that is not selected to isolation
    // not interactable when a element is in fade from isolation
    public void SetToIsolatedMode()
    {
        if (Collider != null)
        {
            Collider.enabled = false;
        }

        if (Renderer != null)
        {
            List<Material> mats = new List<Material>();


            for (int i = 0; i < OriginalMaterials.Length; i++)
            {
                mats.Add(ResourceHolder.Instance.BIM_OBJECT_ISOLATION);
            }

            Renderer.materials = mats.ToArray();
        }

        isDrawBorder = false;
        IsIsolated = true;
    }


    #endregion


    public void ClearSplitedObjects()
    {
        foreach (GameObject ob in SplitedObjects)
        {
            Destroy(ob);
        }

        SplitedObjects.Clear();
    }

    public void SetSplitedObjects()
    {

        if (SplitedObjects.Count > 0)
        {
            //Debug.Log("Set Split Object Display to: " + ProjectConfiguration.Instance.IsShowSplitedObjects);
            // disable this object
            Renderer.enabled = !ProjectConfiguration.Instance.IsShowSplitedObjects;
            Collider.enabled = !ProjectConfiguration.Instance.IsShowSplitedObjects;


            // enable splited objects
            foreach (GameObject ob in SplitedObjects)
            {
                ob.SetActive(ProjectConfiguration.Instance.IsShowSplitedObjects);
               
            }
        }
    }



    public void SetSplitedObjectsInSplitingMode(bool _isEditing = true)
    {

        if (SplitedObjects.Count > 0)
        {
            Debug.Log("SetSplitedObjectsInSplitingMode: " + _isEditing);
            // disable this object
            Renderer.enabled = !_isEditing;
            Collider.enabled = !_isEditing;


            // enable splited objects
            foreach (GameObject ob in SplitedObjects)
            {
                ob.SetActive(_isEditing);
            }
        }
    }



    public void SetLineConfig(Color lineColor, float lineWidth)
    {
        LineColor = lineColor;
        LineWidth = lineWidth;
    }

    public void SetSetBorderLine(Color lineColor, float lineWidth)
    {
        for (int i = 0; i < BorderLines.count; i++)
        {
            BorderLines.SetColor(i, lineColor);
            BorderLines.SetWidth(i, lineWidth);
        }
    }


    #region Extra
    public void AddToFace(Vector3 _p1, Vector3 _p2, Vector3 _p3)
    {
        Plane plane = new Plane(_p1, _p2, _p3);
        Vector3 narmal = plane.normal;

        BimFace face = GetFace(narmal);

        if (face == null)
        {
            face = new BimFace(narmal);
            Faces.Add(face);
        }

        face.AddTriangle(new BimTriangle(_p1, _p2, _p3));
    }

    public BimFace GetFace(Vector3 _normal)
    {
        foreach (var face in Faces)
        {
            if (isSame(face.Normal, _normal))
            {
                return face;
            }
        }

        return null;
    }

    private bool isSame(Vector3 a, Vector3 b)
    {
        float diff = Vector3.Distance(a, b);

        if (Mathf.Abs(diff) < 0.001)
        {
            return true;
        }
        return false;
    }
    
    #endregion

    #region Generate mode from data
    public void OnGenerateBimGameObject()
    {
        // Save the item in the root of the model object
        BimObjectRecord SelectedRecord = BimObject.GerRecord(SelectedVersion);

        if (SelectedRecord == null)
        {
            return;
        }

        // Set object name
        gameObject.name = SelectedRecord.objectName;

        // Set transform, maybe need local position?
        transform.position = Vector3D.FromVecter3D(SelectedRecord.transformPosition);
        transform.localScale = Vector3D.FromVecter3D(SelectedRecord.transformScale);
        transform.rotation = Quaternion.Euler(Vector3D.FromVecter3D(SelectedRecord.transformRotation));

        if (SelectedRecord.ifcGeometryType == BIM_GEOMETRY_TYPE.Geometry)
        {
            Mesh mesh = new Mesh();

            // convert mesh
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();

            foreach (Vector3D item in SelectedRecord.geometryVertices)
            {
                vertices.Add(Vector3D.FromVecter3D(item));
            }

            foreach (Vector3D item in SelectedRecord.geometryNormals)
            {
                normals.Add(Vector3D.FromVecter3D(item));
            }

            foreach (Vector2D item in SelectedRecord.geometryUV)
            {
                uv.Add(Vector2D.FromVecter2D(item));
            }

            mesh.SetVertices(vertices);
            mesh.SetIndices(SelectedRecord.geometryIndics, MeshTopology.Triangles, 0, false);

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            //mesh.Optimize();
            mesh.name = BimObject.elementID;

            Filter.mesh = mesh;

            if (SelectedRecord.geometryTriangles.Length > 2)
            {
                Collider.sharedMesh = mesh;
            }

        }

    }

    public void ApplyMeshToData()
    {
        // Save the item in the root of the model object
        BimObjectRecord SelectedRecord = BimObject.GerRecord(SelectedVersion);

        if (SelectedRecord == null)
        {
            return;
        }

        SelectedRecord.transformPosition = Vector3D.FromVecter3(transform.position);
        SelectedRecord.transformScale = Vector3D.FromVecter3(transform.localScale);
        SelectedRecord.transformRotation = Vector3D.FromVecter3(transform.rotation.eulerAngles);

        // TODO

    }

    public void ApplyMaterials()
    {
        /*
        ColorItem colorItem = ResourceHolder.Instance.SearchMatItem(MaterialName.ToLower());
        OriginalMaterialColor = colorItem.Color;
        OriginalMaterial = colorItem.Material;

        Renderer.material = OriginalMaterial;

        // save to buffer
        ObjectMaterials = Renderer.materials;
        */

    }


    // add more conditions, may be more to convertor processor ?
    public string GetMaterialName()
    {
        string ifcMatname = "default";
        ifcMatname = BimObject.records[BimObject.versionID].ifcProperties.Find("Material");

        if (ifcMatname != null)
        {
            return ifcMatname;
        }

        ifcMatname = BimObject.records[BimObject.versionID].ifcAttribute.Find("Material");

        if (ifcMatname != null)
        {
            return ifcMatname;
        }

        ifcMatname = BimObject.records[BimObject.versionID].ifcTypes.Find("Material");

        if (ifcMatname != null)
        {
            return ifcMatname;
        }

        ifcMatname = BimObject.records[BimObject.versionID].objectName;
        return ifcMatname;
    }


    // covert data to BIMObject, maybe after editing and spliting operation
    public void ConvertFromBimGameObject()
    {

    }


    public static void CopyItem(BIMElement _target, BIMElement _source)
    {

    }
    #endregion
}





