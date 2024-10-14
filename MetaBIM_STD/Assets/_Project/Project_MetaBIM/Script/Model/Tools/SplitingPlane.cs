using MetaBIM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Linefy;
using Linefy.Serialization;
using System;


public class SplitingPlane : MonoBehaviour
{
    public SplitPlane Item;
    

    // the plane mode, vertical is for level
    public bool IsPlaneVertical = true;
    public bool IsEnableInEditor = true;


    [Header("Interaction")]
    public List<Transform> CornerGrabs;
    public Transform CenterGrab;
    

    [Header("Render Parameters")]
    public PolygonalMesh Plane;
    public Lines WireFrame;
    
    public SerializationData_PolygonalMeshProperties polygonalMeshProperties = new SerializationData_PolygonalMeshProperties();
    public SerializationData_Lines wireframePropertyes = new SerializationData_Lines(2, Color.black, 1);


    public Vector3[] CornerArray;
    public Polygon[] Polygon;
    public Vector2[] uvs;
    
    public Color[] colors;
    public Color[] MeshColors;

    public Vector3 Offset;

    public Vector3 PlaneDirection;
    public Vector3 PlaneCenterPoint;

    [Header("Status")]
    public bool isDrawing;
    public bool isDragable = false;

    void Start()
    {

    }



    public void OnPlaneInitByHandlerHorizental(Bounds _bound, bool _isX)
    {
        //Debug.Log("OnPlaneInitByHandlerHorizental: " + _isX);

        IsPlaneVertical = false;

        List<Vector3> _corners = new List<Vector3>();

        Vector3 center = _bound.center;
        Vector3 extents = _bound.extents;
        CenterGrab.gameObject.SetActive(true);
        CenterGrab.GetComponent<SplitingPlanePointGrab>().isLocked = false;


        //CenterGrab.position = center;

        if (_isX) // is X
        {
            // Top points
            _corners.Add(new Vector3(center.x, center.y + extents.y, center.z + extents.z));
            _corners.Add(new Vector3(center.x, center.y + extents.y, center.z - extents.z));

            // bottom points
            _corners.Add(new Vector3(center.x, center.y - extents.y, center.z - extents.z));
            _corners.Add(new Vector3(center.x, center.y - extents.y, center.z + extents.z));

            //CenterGrab.gameObject.GetComponent<SplitingPlanePointGrab>().MoveOn_X = true;
        }
        else  // is Z
        {
            // Top points
            _corners.Add(new Vector3(center.x - extents.x, center.y + extents.y, center.z));
            _corners.Add(new Vector3(center.x + extents.x, center.y + extents.y, center.z));

            // bottom points
            _corners.Add(new Vector3(center.x + extents.x, center.y - extents.y, center.z));
            _corners.Add(new Vector3(center.x - extents.x, center.y - extents.y, center.z));

        }


        ActiveDrag(false);
        // set drag point transform

        foreach (var grab in CornerGrabs)
        {
            grab.transform.position = _corners[CornerGrabs.IndexOf(grab)];
        }

        OnUpdataCenter();
        SavePlane();
        isDrawing = true;
    }


    public void OnPlaneInitByHandlerVertical(Bounds _bound, float _planeHeight)
    {
        //Debug.Log("OnPlaneInitByHandlerHorizental: " + _planeHeight);


        IsPlaneVertical = true;

        List<Vector3> _corners = new List<Vector3>();

        Vector3 center = _bound.center;
        Vector3 extents = _bound.extents;
        CenterGrab.gameObject.SetActive(true);
        CenterGrab.GetComponent<SplitingPlanePointGrab>().isLocked = false;

        // set center
        CenterGrab.position = new Vector3(center.x, _planeHeight, center.z);


        Vector3 topFrontRight = center + new Vector3(extents.x, extents.y, extents.z);
        Vector3 topFrontLeft = center + new Vector3(-extents.x, extents.y, extents.z);
        Vector3 topBackRight = center + new Vector3(extents.x, extents.y, -extents.z);
        Vector3 topBackLeft = center + new Vector3(-extents.x, extents.y, -extents.z);

        _corners.Add(new Vector3(topFrontRight.x, _planeHeight, topFrontRight.z));
        _corners.Add(new Vector3(topFrontLeft.x, _planeHeight, topFrontLeft.z));
        _corners.Add(new Vector3(topBackLeft.x, _planeHeight, topBackLeft.z));
        _corners.Add(new Vector3(topBackRight.x, _planeHeight, topBackRight.z));


        ActiveDrag(true);

        // set drag point transform
        foreach (var grab in CornerGrabs)
        {
            grab.transform.position = _corners[CornerGrabs.IndexOf(grab)];
        }

        OnUpdataCenter();
        SavePlane();
        isDrawing = true;
    }


    public void OnPlaneInitByObject(SplitPlane _item)
    {
        Item = _item;
        IsPlaneVertical = Item.planeType == SplitPlane.PlaneType.vertical;
        CenterGrab.gameObject.SetActive(true);
        CenterGrab.GetComponent<SplitingPlanePointGrab>().isLocked = false;

        foreach (var grab in CornerGrabs)
        {
            grab.transform.position = Vector3D.FromVecter3D( _item.planeCorners[CornerGrabs.IndexOf(grab)]);
        }
        CenterGrab.position = Vector3D.FromVecter3D(Item.planePosition);
        PlaneDirection = Vector3D.FromVecter3D(Item.planeDirection);

        ActiveDrag(IsPlaneVertical);


        isDrawing = _item.isApplied;
    }


    void Update()
    {
        if (IsEnableInEditor)
        {
            //Init();
        }
        else
        {

        }



        UpdatePlane();
    }


    public void UpdatePlane()
    {
        if (isDrawing)
        {
            // set position of box
            Init();

            // drawing
            Plane.LoadSerializationData(polygonalMeshProperties);
            Plane.positionEdgesWireframe = WireFrame;
            //Box.Draw(transform.localToWorldMatrix);
            Plane.Draw();

            WireFrame.LoadSerializationData(wireframePropertyes);
            wireframePropertyes.colorMultiplier = colors[0];
            //WireFrame.Draw(transform.localToWorldMatrix);
            WireFrame.Draw();

            GetPlaneDirection();

        }
    }



    // setting plane via object
    public void SetPlane(SplitPlane _item)
    {
        Item = _item;
        List<Vector3> newCorners = new List<Vector3>();

        foreach(var point in Item.planeCorners)
        {
            newCorners.Add(new Vector3(point.x, point.y, point.z));
        }

        Vector3 _rotation = new Vector3(0, Item.planeRotation, 0);

        InitPlane(newCorners, Vector3.zero, _rotation);
    }

    public void Init()
    {
        List<Vector3> _corners = new List<Vector3>();

        foreach (var c in CornerGrabs)
        {
            _corners.Add(c.position);
        }


        for (int i = 0; i < MeshColors.Length; i++)
        {
            MeshColors[i] = colors[1];
        }

        InitPlane(_corners, Vector3.zero, Vector3.zero);

 
        //Plane = new PolygonalMesh(CornerArray, uvs, MeshColors, Polygon);
        //wireframePropertyes.colorMultiplier = colors[0];

        //Plane.LoadSerializationData(polygonalMeshProperties);
        // WireFrame.LoadSerializationData(wireframePropertyes);
    }


    public void InitPlane(List<Vector3> _corners, Vector3 _offset, Vector3 _rotation)
    {

        Offset = _offset;
        CornerArray = _corners.ToArray();
        Polygon = GetPolygonsOfBoundCorners(CornerArray).ToArray();

        for (int i = 0; i < MeshColors.Length; i++)
        {
            MeshColors[i] = colors[1];
        }

        Plane = new PolygonalMesh(CornerArray, uvs, MeshColors, Polygon);

        // update box
        for (int i = 0; i < CornerArray.Length; i++)
        {
            Plane.SetPosition(i, CornerArray[i]);
        }

        WireFrame = new Lines(4);

    }


    public void SavePlane()
    {
        Item.planeCorners.Clear();

        for (int i= 0; i < CornerGrabs.Count; i++)
        {
            Item.planeCorners.Add(Vector3D.FromVecter3(CornerGrabs[i].position));
        }

        Item.planePosition = Vector3D.FromVecter3(CenterGrab.position);
        Item.planeDirection = Vector3D.FromVecter3(PlaneDirection);

    }

    public void ActiveDrag(bool _isVertical)
    {
        //CenterGrab.gameObject.SetActive(_isVertical);

        CenterGrab.gameObject.SetActive(true);
        CenterGrab.GetComponent<SplitingPlanePointGrab>().isLocked = false;

        if (!_isVertical)
        {
            int index = 0;
            foreach (var item in CornerGrabs)
            {
                item.gameObject.SetActive(true);

                if (index == 0 || index == 1)
                {
                    item.gameObject.GetComponent<SplitingPlanePointGrab>().isLocked = false;
                }
                else
                {
                    item.gameObject.GetComponent<SplitingPlanePointGrab>().isLocked = true;
                }


                index++;

            }

            CornerGrabs[0].GetComponent<SplitingPlanePointGrab>().isLocked = false;
            CornerGrabs[1].GetComponent<SplitingPlanePointGrab>().isLocked = false;


            SplitingPlanePointGrab cent = CenterGrab.gameObject.GetComponent<SplitingPlanePointGrab>();
            cent.MoveOn_X = true;
            cent.MoveOn_Y = false;
            cent.MoveOn_Z = true;

        }
        else
        {
            foreach (var item in CornerGrabs)
            {
                item.gameObject.SetActive(false);
                item.gameObject.GetComponent<SplitingPlanePointGrab>().isLocked = true;
            }


            SplitingPlanePointGrab cent = CenterGrab.gameObject.GetComponent<SplitingPlanePointGrab>();
            cent.MoveOn_X = false;
            cent.MoveOn_Y = true;
            cent.MoveOn_Z = false;
        }

        //OnUpdataCenter();  // wrong place, but it works

        isDragable = true;

    }



    public void DeactiveDrag()
    {
        CenterGrab.GetComponent<SplitingPlanePointGrab>().isLocked = true;
        CenterGrab.gameObject.SetActive(false);

        foreach (var item in CornerGrabs)
        {
            item.gameObject.SetActive(false);
        }

        isDragable = false;
    }

    public void OnUpdateCorner(int _index, Vector3 _position)
    {
        CornerArray[_index] = _position;
        Polygon = GetPolygonsOfBoundCorners(CornerArray).ToArray();
        Plane = new PolygonalMesh(CornerArray, uvs, MeshColors, Polygon);


        // update box
        for (int i = 0; i < CornerArray.Length; i++)
        {
            Plane.SetPosition(i, CornerArray[i]);
        }
    }


    public void OnUpdataCenter()
    {
        if (!IsPlaneVertical)
        {
            //PlaneCenterPoint = (CornerGrabs[0].position + CornerGrabs[1].position + CornerGrabs[2].position + CornerGrabs[3].position) / 4;
            //CenterGrab.position = PlaneCenterPoint;
        }

        PlaneCenterPoint = (CornerGrabs[0].position + CornerGrabs[1].position + CornerGrabs[2].position + CornerGrabs[3].position) / 4;
        CenterGrab.position = PlaneCenterPoint;
    }



    public void UpdatePlaneByObject()
    {
        
    }






    private List<Polygon> GetPolygonsOfBoundCorners(Vector3[] _corners)
    {
        // corner order is in anticlockwise face forward
        List<Polygon> polygons = new List<Polygon>();
        //p1, top
        polygons.Add(new Polygon(1, 0,
            new PolygonCorner(0, 0, 1),
            new PolygonCorner(1, 1, 1),
            new PolygonCorner(2, 2, 1),
            new PolygonCorner(3, 3, 1)));


        return polygons;
    }


    public void OnPointSelected(SplitingPlanePointGrab _grabPoint)
    {

    }


    public Vector3 GetPlaneDirection()
    {
        PlaneCenterPoint = CenterGrab.position;

        Vector3 vectorAB = CornerGrabs[1].position - CornerGrabs[0].position;
        Vector3 vectorAC = CornerGrabs[2].position - CornerGrabs[1].position;
        PlaneDirection = Vector3.Cross(vectorAB, vectorAC).normalized;
        
        return PlaneDirection;
    }
}
