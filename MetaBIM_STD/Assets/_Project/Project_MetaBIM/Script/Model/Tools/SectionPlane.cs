using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Linefy;
using Linefy.Serialization;
using System;

public class SectionPlane : MonoBehaviour
{
    public PolygonalMesh Box;
    public Lines WireFrame;
    public bool isDrawing = true;

    [Header("Bound / Mesh Data")]

    public Vector3[] CornerArray;
    public Polygon[] Polygon;
    public Vector2[] uvs;
    public Color[] colors;

    public Vector3 Offset;
    public Bounds TargetBound;
    
    [Header("Linefy Properity")]
    public SerializationData_PolygonalMeshProperties polygonalMeshProperties = new SerializationData_PolygonalMeshProperties();
    public SerializationData_Lines wireframePropertyes = new SerializationData_Lines(2, Color.black, 1);

    void Start()
    {

    }


    private void Update()
    {
        if (isDrawing)
        {
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

    public void SetItem(Bounds _targetBound, Vector3 _offset)
    {
        Offset = _offset;
        // create box
        CornerArray = GetCornerPositionOfBound(_targetBound).ToArray();
        //Polygon = GetPolygonsOfBoundCorners(CornerArray).ToArray();
        WireFrame = new Lines(4);
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
        
        isDrawing = false;
    }



    private List<Vector3> GetCornerPositionOfBound(Bounds _bounds)
    {
        List<Vector3> corners = new List<Vector3>();

        TargetBound = _bounds;

        Vector3 boundPoint1 = new Vector3(_bounds.center.x - _bounds.extents.x, 0, _bounds.center.z - _bounds.extents.z);
        Vector3 boundPoint2 = new Vector3(_bounds.center.x + _bounds.extents.x, 0, _bounds.center.z - _bounds.extents.z);
        Vector3 boundPoint3 = new Vector3(_bounds.center.x + _bounds.extents.x, 0, _bounds.center.z + _bounds.extents.z);
        Vector3 boundPoint4 = new Vector3(_bounds.center.x - _bounds.extents.x, 0, _bounds.center.z + _bounds.extents.z);


        corners.Add(boundPoint1);
        corners.Add(boundPoint2);
        corners.Add(boundPoint3);
        corners.Add(boundPoint4);

        return corners;
    }


    private List<Polygon> GetPolygonsOfBoundCorners(Vector3[] _corners)
    {
        List<Polygon> polygons = new List<Polygon>();

        polygons.Add(new Polygon(0, 0,
            new PolygonCorner(0, 0, 0),
            new PolygonCorner(1, 1, 0),
            new PolygonCorner(2, 2, 0),
            new PolygonCorner(3, 3, 0)));


        return polygons;
    }
}
