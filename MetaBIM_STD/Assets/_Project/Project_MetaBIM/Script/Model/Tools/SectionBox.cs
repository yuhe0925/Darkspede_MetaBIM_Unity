using Linefy;
using Linefy.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class SectionBox : MonoBehaviour
{

    public List<Transform> Corners;
    public List<Transform> GrabPoints; 
    public List<Vector3> GrabPointDirections;
    public List<SectionBoxGrab> GrabPointItems;

    public PolygonalMesh Box;
    public Lines WireFrame;
    public bool isDrawing = true;


    public Vector3[] CornerArray;
    public Polygon[] Polygon;
    public Vector2[] uvs;
    public Color[] colors;

    public Vector3 Offset;

    public Bounds SavedBound;
    public Bounds CurrentBound;

    [Header("Linefy Properity")]
    public SerializationData_PolygonalMeshProperties polygonalMeshProperties = new SerializationData_PolygonalMeshProperties();
    public SerializationData_Lines wireframePropertyes = new SerializationData_Lines(2, Color.black, 1);



    [Header("Operation Planes")]
    public List<int> VerticalPlanes = new List<int> { 0, 2, 5, 6 };
    public List<Vector3> VerticalPlaneDirections;

    void Start()
    {
        WireFrame = new Lines(12);
        Box = new PolygonalMesh(CornerArray, uvs, colors, Polygon);
    }


    private void Update()
    {
        if (isDrawing)
        {
            GetCornerPosition();
            Box.LoadSerializationData(polygonalMeshProperties);
            Box.positionEdgesWireframe = WireFrame;
            WireFrame.LoadSerializationData(wireframePropertyes);
            Box.Draw(transform.localToWorldMatrix);
            WireFrame.Draw(transform.localToWorldMatrix);
        }
    }

    public void OnDeselect()
    {
        isDrawing = false;
    }

    /// <summary>
    ///  Selection box is always a cube, with 8 corner points, 6 polygons, 12 lines
    /// </summary>
    public void OnInitSectionBox(Bounds _targetBound, Vector3 _offset)
    {
        Debug.Log("OnInitSectionBox£º" + _targetBound.center);
        SavedBound = _targetBound;
        Offset = _offset;
        // create box

        // this is make the section box a litt bigger than the target bound
        SavedBound.size = SavedBound.size * 1.01f;

        CornerArray = GetCornerPositionOfBound(SavedBound).ToArray();
        Polygon = GetPolygonsOfBoundCorners(CornerArray).ToArray();
        Box = new PolygonalMesh(CornerArray, uvs, colors, Polygon);


        // update box
        for (int i = 0; i < CornerArray.Length; i++)
        {
            Box.SetPosition(i, CornerArray[i]);
        }

        for (int i = 0; i < colors.Length; i++)
        {
            Box.SetColor(i, colors[i]);
        }

        // setposition of box
        transform.position = Offset;

        isDrawing = true;
    }

    public void Reset()
    {
        
    }


    public void GetCornerPosition()
    {
        for(int i = 0;i < CornerArray.Length; i++)
        {
            CornerArray[i] = Corners[i].position;
            Box.SetPosition(i, CornerArray[i]);
        }
    }


    public void OnUpdateDragPoint(int _index)
    {
        for (int i = 0; i < GrabPoints.Count; i++)
        {
            if(i == _index)
            {
                continue;
            }
            else
            {
                Vector3 centerpoint = (GrabPointItems[i].RaletedPoints[0].position + GrabPointItems[i].RaletedPoints[2].position) / 2f;
                GrabPoints[i].position = centerpoint;
            }
        }

        // update bound
        if(Corners[0].position.x > Corners[1].position.x)
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

        Vector3 boundPoint1 = _bounds.min;
        Vector3 boundPoint2 = _bounds.max;
        Vector3 boundPoint3 = new Vector3(boundPoint1.x, boundPoint1.y, boundPoint2.z);// -1, -1, 1
        Vector3 boundPoint4 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint1.z);// -1, 1, -1
        Vector3 boundPoint5 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint1.z);// 1, -1, -1
        Vector3 boundPoint6 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint2.z);// -1, 1, 1
        Vector3 boundPoint7 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint2.z);// 1, -1, 1
        Vector3 boundPoint8 = new Vector3(boundPoint2.x, boundPoint2.y, boundPoint1.z);// 1, 1, -1

        corners.Add(boundPoint1);
        corners.Add(boundPoint2);
        corners.Add(boundPoint3);
        corners.Add(boundPoint4);

        corners.Add(boundPoint5);
        corners.Add(boundPoint6);
        corners.Add(boundPoint7);
        corners.Add(boundPoint8);

        for (int i = 0; i < CornerArray.Length; i++)
        {
            Corners[i].position = corners[i];
        }


        GrabPoints[0].position = new Vector3(_bounds.center.x, _bounds.center.y, _bounds.center.z - _bounds.extents.z );
        GrabPoints[1].position = new Vector3(_bounds.center.x, _bounds.center.y + _bounds.extents.y, _bounds.center.z );
        GrabPoints[2].position = new Vector3(_bounds.center.x, _bounds.center.y, _bounds.center.z + _bounds.extents.z);
        GrabPoints[3].position = new Vector3(_bounds.center.x, _bounds.center.y - _bounds.extents.y, _bounds.center.z);
        GrabPoints[4].position = new Vector3(_bounds.center.x + _bounds.extents.x, _bounds.center.y, _bounds.center.z);
        GrabPoints[5].position = new Vector3(_bounds.center.x - _bounds.extents.x, _bounds.center.y, _bounds.center.z);


        CurrentBound = _bounds;
        return corners;
    }


    private List<Polygon> GetPolygonsOfBoundCorners(Vector3[] _corners)
    {
        List<Polygon> polygons = new List<Polygon>();

        polygons.Add(new Polygon(0, 0,
            new PolygonCorner(0, 0, 0),
            new PolygonCorner(4, 1, 0),
            new PolygonCorner(7, 2, 0),
            new PolygonCorner(3, 3, 0)));

        

        polygons.Add(new Polygon(1, 0,
            new PolygonCorner(7, 0, 0),
            new PolygonCorner(3, 1, 0),
            new PolygonCorner(5, 2, 0),
            new PolygonCorner(1, 3, 0)));

        polygons.Add(new Polygon(2, 0,
            new PolygonCorner(5, 0, 0),
            new PolygonCorner(1, 1, 0),
            new PolygonCorner(6, 2, 0),
            new PolygonCorner(2, 3, 0)));

        polygons.Add(new Polygon(3, 0,
            new PolygonCorner(6, 0, 0),
            new PolygonCorner(2, 1, 0),
            new PolygonCorner(0, 2, 0),
            new PolygonCorner(4, 3, 0)));

        polygons.Add(new Polygon(4, 0,
            new PolygonCorner(4, 0, 0),
            new PolygonCorner(6, 1, 0),
            new PolygonCorner(1, 2, 0),
            new PolygonCorner(7, 3, 0)));

        polygons.Add(new Polygon(5, 0,
            new PolygonCorner(0, 0, 0),
            new PolygonCorner(2, 1, 0),
            new PolygonCorner(5, 2, 0),
            new PolygonCorner(3, 3, 0)));


        return polygons;
    }
}
