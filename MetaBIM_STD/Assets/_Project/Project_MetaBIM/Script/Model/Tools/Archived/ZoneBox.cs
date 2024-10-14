using Linefy;
using Linefy.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MetaBIM;

[ExecuteInEditMode]
[Serializable]
public class ZoneBox : MonoBehaviour
{
    public int BoxIndex;
    public string ZoneName = "New Zone";
    public string ZoneType = "space";
    public string ZoneCategory = "linked";
    public int level = 0;

    public List<Transform> Corners;
    public List<Transform> GrabPoints;
    public List<Transform> ActiveGrabPoints;
    public List<Vector3> GrabPointDirections;
    public List<SDFPoint> SDFPoints;
    public List<ZoneBoxGrab> GrabPointItems;
    public List<ZoneBoxPlane> SelectablePlane;
    public ZoneBoxController Controller;
    
    public PolygonalMesh Box;
    public Lines WireFrame;
    public bool isDrawing = true;
    public int SelectedPlaneIndex = -1;  // -1  = no plane selected

    public Vector3[] CornerArray;
    public Polygon[] Polygon;
    public Vector2[] uvs;

    private Mesh BoxMesh;
    public MeshCollider BoxCollider;
    public int[] triangles;

    public float BoxVolume;
    public float BoxArea;

    /// <summary>
    ///  0 = wire color
    ///  1 = mesh original color
    ///  2 = mesh select color
    ///  3 = plane select color;
    /// </summary>
    public Color[] colors;

    public Color[] MeshColors;


    public Vector3 Offset;

    public Bounds SavedBound;
    public Bounds CurrentBound;

    [Header("Linefy Properity")]
    public SerializationData_PolygonalMeshProperties polygonalMeshProperties = new SerializationData_PolygonalMeshProperties();
    public SerializationData_Lines wireframePropertyes = new SerializationData_Lines(2, Color.black, 1);


    [Header("Operation Planes")]
    public List<int> VerticalPlanes = new List<int> { 0, 2, 5, 6 };
    public List<Vector3> VerticalPlaneDirections;


    [Header("Status")]
    public bool IsPanelLocked = false;  // get lock by other plane
    public bool IsPanelLockTo = false;  // one of my plane is locked to another
    public bool IsMenuUpdating = false;

    [Header("Level Planes")]

    public List<ZoneLevelPlane> levelPlanes = new List<ZoneLevelPlane>();

    void Start()
    {
        BoxMesh = new Mesh();
        
        if (Controller == null)
        {
            Controller = ZoneBoxController.Instance;
        }


        SelectedPlaneIndex = -1;

        for (int i = 0; i < MeshColors.Length; i++)
        {
            MeshColors[i] = colors[1];
        }

        WireFrame = new Lines(12);
        Box = new PolygonalMesh(CornerArray, uvs, MeshColors, Polygon);
        wireframePropertyes.colorMultiplier = colors[0];

        foreach(ZoneBoxPlane plane in SelectablePlane)
        {
            plane.ZoneBoxIndex = BoxIndex;
        }

        Box.LoadSerializationData(polygonalMeshProperties);
        WireFrame.LoadSerializationData(wireframePropertyes);


        //triangles = { 0 ,1,2};
    }


    private void Update()
    {
        if (isDrawing)
        {
            GetCornerPosition();

            if (IsMenuUpdating)
            {
                for (int i = 0; i < 6; i++)
                {
                    MeshColors[i] = colors[2];
                    Box = new PolygonalMesh(CornerArray, uvs, MeshColors, Polygon);
                }
            }
            
            Box.LoadSerializationData(polygonalMeshProperties);
            Box.positionEdgesWireframe = WireFrame;
            //Box.Draw(transform.localToWorldMatrix);
            Box.Draw();
            
            WireFrame.LoadSerializationData(wireframePropertyes);
            wireframePropertyes.colorMultiplier = colors[0];
            //WireFrame.Draw(transform.localToWorldMatrix);
            WireFrame.Draw();
            
            // this maybe calculation haevy
            BoxMesh.vertices = CornerArray;
            BoxMesh.triangles = triangles;
            BoxMesh.RecalculateNormals();
            
            BoxCollider.sharedMesh = BoxMesh;
        }


        
    }


    public Mesh GetBoxMesh()
    {
        return BoxMesh;
    }

    
    #region Data Operation

    public VersionZone OnConvertToZoneUpdate(VersionZone _versionzone)
    {
        VersionZone saveZone;
        
        if (_versionzone == null)
        {
            saveZone = new VersionZone();
        }else
        {
            saveZone = _versionzone;

        }
        
        saveZone.zoneIndex = BoxIndex;
        saveZone.zoneName = ZoneName;
        
        saveZone.cornerArray.Clear();
        
        foreach (var point in CornerArray)
        {
            Vector3D v = Vector3D.FromVecter3(point);
            saveZone.cornerArray.Add(v);
        }

        return saveZone;
    }

    public void OnConvertFromZoneUpdate(VersionZone _versionzone)
    {
        BoxIndex = _versionzone.zoneIndex;
        ZoneName = _versionzone.zoneName;

        List<Vector3> points = new List<Vector3>();
        
        foreach (var point in _versionzone.cornerArray)
        {
            Vector3 v = new Vector3(point.x, point.y, point.z);
            points.Add(v);
        }

        CornerArray = points.ToArray();
    }



    #endregion

    #region Box Init

    private Transform Center;

    public void OnInitBox(Bounds _targetBound, Vector3 _offset, Transform _center)
    {
        //Debug.Log("OnInitBox" + _targetBound.center);
        Center = _center;
        Center.position = _targetBound.center;
        SavedBound = _targetBound;
        Offset = _offset;

        // create box
        // this is make the section box a litt bigger than the target bound
        SavedBound.size = SavedBound.size * 1.01f;

        CornerArray = GetCornerPositionOfBound(SavedBound).ToArray();
        Polygon = GetPolygonsOfBoundCorners(CornerArray).ToArray();
        Box = new PolygonalMesh(CornerArray, uvs, MeshColors, Polygon);


        // update box
        for (int i = 0; i < CornerArray.Length; i++)
        {
            Box.SetPosition(i, CornerArray[i]);
        }

        // setposition of box
        transform.position = Offset;

        isDrawing = true;

        
        OnEnableEditing(true);


    }

    public void OnResetId(int _boxID)
    {
        BoxIndex = _boxID;

        foreach (var plane in SelectablePlane)
        {
            plane.ZoneBoxIndex = BoxIndex;
        }

    }

    public void LockToPlane()
    {
        // this put here may not 
        if (BoxIndex > 0)
        {
            OnAdjustBoxToLastOne(ZoneBoxController.Instance.GetLockedPlane(BoxIndex));
        }
    }
        
    
    private void OnAdjustBoxToLastOne(ZoneBoxPlane _connectPlane)
    {
        // connect my plane to _connectedPlane;

        ZoneBoxPlane myplane = SelectablePlane[_connectPlane.LockIndex];
        
        if (IsPanelLocked)
        {
            Debug.Log("OnAdjustBoxToLastOne: Connect Plane = " + _connectPlane.PlaneIndex);
            
            Debug.Log("Get Locked Panel: Plane = " + _connectPlane.PlaneIndex);
           
            myplane.LockedCorners = _connectPlane.PolyCorners.ToList();
        }

    }


    public void UnlockPanel()
    {
        IsPanelLocked = false;
    }


    #endregion






    public void OnEnableEditing(bool _Enable = true)
    {
        if (Controller == null)
        {
            Controller = ZoneBoxController.Instance;
        }
        
        foreach (var point in GrabPoints)
        {
            point.gameObject.GetComponent<MeshRenderer>().enabled = _Enable;
            point.gameObject.GetComponent<BoxCollider>().enabled = _Enable;
        }

        foreach (var plane in SelectablePlane)
        {
            plane.gameObject.GetComponent<MeshCollider>().enabled = _Enable;
        }
            
        foreach (var point in GrabPointItems)
        {
            point.cameraNav = Controller.CameraNav;
        }

        foreach (SDFPoint point in SDFPoints)
        {
            point.SDF_Target = Controller.SD_Target;
        }
    }
    

    public void SetBoxColor(Color _plane, Color _wire)
    {

    }

    public void SetBoxSelectPlaneColor(int _index, Action _action)
    {
        //Debug.Log("SetBoxSelectPlaneColor: " + _index + " " + _action);

        // Disable top and botton face for selection
        // MARK
        if (SelectablePlane[_index].Normal.y != 0)
        {
            return;
        }

        switch (_action)
        {

            case Action.OnMouseEnter:
                if (SelectedPlaneIndex != _index)
                {
                    MeshColors[_index] = colors[2];
                    Box = new PolygonalMesh(CornerArray, uvs, MeshColors, Polygon);
                }
                break;
            case Action.OnMouseExit:
                if (SelectedPlaneIndex != _index)
                {
                    MeshColors[_index] = colors[1];
                    Box = new PolygonalMesh(CornerArray, uvs, MeshColors, Polygon);
                }
                break;
            case Action.OnMouseSelect:
                if (SelectedPlaneIndex != _index && !SelectablePlane[_index].Selected)
                {
                    if (SelectedPlaneIndex > -1)
                    {
                        MeshColors[SelectedPlaneIndex] = colors[1];
                        SelectablePlane[SelectedPlaneIndex].Selected = false;
                    }

                    MeshColors[_index] = colors[3];
                    Box = new PolygonalMesh(CornerArray, uvs, MeshColors, Polygon);
                    SelectedPlaneIndex = _index;
                    SelectablePlane[_index].Selected = true;
                }
                break;
            case Action.OnMouseUnselect:
                if (SelectedPlaneIndex > -1)
                {
                    MeshColors[_index] = colors[1];
                    Box = new PolygonalMesh(CornerArray, uvs, MeshColors, Polygon);
                    SelectedPlaneIndex = -1;
                    SelectablePlane[_index].Selected = true;
                }
                break;
            default:
                break;
        }
    }


    public void SetBoxHighLightColor(bool _isOrigin)
    {
        if (_isOrigin)
        {
            // rest 

        }
        else
        {
            // highlight

        }
    }

    public void OnDeselect()
    {
        isDrawing = false;
    }


    public void GetCornerPosition()
    {
        for (int i = 0; i < CornerArray.Length; i++)
        {
            CornerArray[i] = Corners[i].position;
            Box.SetPosition(i, CornerArray[i]);
        }
    }


    public enum Action
    {
        OnMouseEnter,
        OnMouseExit,
        OnMouseSelect,
        OnMouseUnselect
    }

    

    public void OnUpdateDragPoint(int _index)
    {
        // update bound
        if (Corners[0].position.x > Corners[1].position.x)
        {
            CurrentBound.max = Corners[0].position;
            CurrentBound.min = Corners[1].position;
        }
        else
        {
            CurrentBound.max = Corners[1].position;
            CurrentBound.min = Corners[0].position;
        }

        CurrentBound.center = (CurrentBound.max + CurrentBound.min) / 2f;
        CurrentBound.size = CurrentBound.max - CurrentBound.min;
        CurrentBound.extents = CurrentBound.size / 2f;
    }


    private List<Vector3> GetCornerPositionOfBound(Bounds _bounds)
    {
        List<Vector3> corners = new List<Vector3>();

        Vector3 boundPoint1 = _bounds.min; // -1, -1, -1
        Vector3 boundPoint2 = _bounds.max; // 1, 1, 1

        Vector3 boundPoint3 = new Vector3(boundPoint1.x, boundPoint1.y, boundPoint2.z);// -1, -1, 1
        Vector3 boundPoint4 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint1.z);// -1, 1, -1
        Vector3 boundPoint5 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint1.z);// 1, -1, -1
        Vector3 boundPoint6 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint2.z);// -1, 1, 1
        Vector3 boundPoint7 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint2.z);// 1, -1, 1
        Vector3 boundPoint8 = new Vector3(boundPoint2.x, boundPoint2.y, boundPoint1.z);// 1, 1, -1

        corners.Add(boundPoint1);  // 0
        corners.Add(boundPoint2);  // 0
        corners.Add(boundPoint3);  // 0
        corners.Add(boundPoint4);  // 0

        corners.Add(boundPoint5);  // 0
        corners.Add(boundPoint6);  // 0
        corners.Add(boundPoint7);  // 0
        corners.Add(boundPoint8);  // 0

        for (int i = 0; i < CornerArray.Length; i++)
        {
            Corners[i].position = corners[i];
        }

        CurrentBound = _bounds;
        return corners;
    }


    private List<Polygon> GetPolygonsOfBoundCorners(Vector3[] _corners)
    {
        // corner order is in anticlockwise face forward
        List<Polygon> polygons = new List<Polygon>();
        // p0, back
        polygons.Add(new Polygon(0, 0,
            new PolygonCorner(0, 0, 0),
            new PolygonCorner(4, 1, 0),
            new PolygonCorner(7, 2, 0),
            new PolygonCorner(3, 3, 0)));
        //p1, top
        polygons.Add(new Polygon(1, 0,
            new PolygonCorner(3, 0, 1),
            new PolygonCorner(7, 1, 1),
            new PolygonCorner(1, 2, 1),
            new PolygonCorner(5, 3, 1)));
        //p2, front
        polygons.Add(new Polygon(2, 0,
            new PolygonCorner(2, 0, 2),
            new PolygonCorner(6, 1, 2),
            new PolygonCorner(1, 2, 2),
            new PolygonCorner(5, 3, 2)));
        //p3, bottom
        polygons.Add(new Polygon(3, 0,
            new PolygonCorner(0, 0, 3),
            new PolygonCorner(4, 1, 3),
            new PolygonCorner(6, 2, 3),
            new PolygonCorner(2, 3, 3)));
        //p4, right
        polygons.Add(new Polygon(4, 0,
            new PolygonCorner(4, 0, 4),
            new PolygonCorner(6, 1, 4),
            new PolygonCorner(1, 2, 4),
            new PolygonCorner(7, 3, 4)));
        //p5, left
        polygons.Add(new Polygon(5, 0,
            new PolygonCorner(0, 0, 5),
            new PolygonCorner(2, 1, 5),
            new PolygonCorner(5, 2, 5),
            new PolygonCorner(3, 3, 5)));


        return polygons;
    }




    public void SetLevels(List<BimLevel> _levels, GameObject _planePrefabs, Transform _parent)
    {
        Debug.Log("SetLevels");

        if (levelPlanes.Count > 0)
        {
            foreach (var item in levelPlanes)
            {
                Destroy(item.gameObject);
            }
        }

        levelPlanes.Clear();

        for (int i = _levels.Count -1 ;i >= 0; i--)
        {
            GameObject planeItem = Instantiate(_planePrefabs, _parent);
            ZoneLevelPlane plane = planeItem.GetComponent<ZoneLevelPlane>();

            float height = _levels[i].LevelCurrentHeight / 1000;

            plane.SetPlane(ActiveGrabPoints, height, _levels[i]);


            levelPlanes.Add(plane);
        }
    }


    public void StopLevel()
    {
    
        Debug.Log("StopLevel");
        foreach (var item in levelPlanes)
        {
            item.isDrawing = false;
        }
    }



    
}
