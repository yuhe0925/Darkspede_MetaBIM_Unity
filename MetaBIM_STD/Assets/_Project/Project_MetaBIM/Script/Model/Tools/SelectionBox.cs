using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Linefy;
using Linefy.Serialization;
using IfcToolkit.IfcSpec;
using System;

[Serializable]
public class SelectionBox : MonoBehaviour
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

    [Header("Linefy Properity")]
    public SerializationData_PolygonalMeshProperties polygonalMeshProperties = new SerializationData_PolygonalMeshProperties();
    public SerializationData_Lines wireframePropertyes = new SerializationData_Lines(2, Color.black, 1);

    void Start()
    {
        WireFrame = new Lines(12);
        Polygon = GetPolygonsOfBoundCorners(CornerArray).ToArray();
        Box = new PolygonalMesh(CornerArray, uvs, colors, Polygon);
        OnDeselect();
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

    /// <summary>
    ///  Selection box is always a cube, with 8 corner points, 6 polygons, 12 lines
    /// </summary>
    public void OnSelectBoxShow(Bounds _targetBound, Vector3 _offset)
    {
        Debug.Log("OnSelectBoxShow£º" + _targetBound.center);
        
        Offset = _offset;

        // create box
        CornerArray = GetCornerPositionOfBound(_targetBound).ToArray();
        //Polygon = GetPolygonsOfBoundCorners(CornerArray).ToArray();

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



    private List<Vector3> GetCornerPositionOfBound(Bounds _bounds)
    {
        List<Vector3> corners = new List<Vector3>();

        Vector3 boundPoint1 = _bounds.min;
        Vector3 boundPoint2 = _bounds.max;
        Vector3 boundPoint3 = new Vector3(boundPoint1.x, boundPoint1.y, boundPoint2.z);
        Vector3 boundPoint4 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint1.z);
        Vector3 boundPoint5 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint1.z);
        Vector3 boundPoint6 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint2.z);
        Vector3 boundPoint7 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint2.z);
        Vector3 boundPoint8 = new Vector3(boundPoint2.x, boundPoint2.y, boundPoint1.z);

        corners.Add(boundPoint1);
        corners.Add(boundPoint2);
        corners.Add(boundPoint3);
        corners.Add(boundPoint4);

        corners.Add(boundPoint5);
        corners.Add(boundPoint6);
        corners.Add(boundPoint7);
        corners.Add(boundPoint8);

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
