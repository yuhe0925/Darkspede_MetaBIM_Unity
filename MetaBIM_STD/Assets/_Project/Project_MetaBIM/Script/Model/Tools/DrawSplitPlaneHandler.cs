using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Linefy;
using Linefy.Serialization;
using System;
using UnityEngine.Events;

public class DrawSplitPlaneHandler : MonoBehaviour
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


    [Header("Handler Status")]

    public bool isDrawingReady = false;
    public UnityEvent OnDrawCompleteRedirect;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDrawing)
        {
            //Box.LoadSerializationData(polygonalMeshProperties);
            //Box.positionEdgesWireframe = WireFrame;
            //WireFrame.LoadSerializationData(wireframePropertyes);
            //Box.Draw(transform.localToWorldMatrix);
            //WireFrame.Draw(transform.localToWorldMatrix);
        }
    }





    public void OnMouseDown()
    {
        if (isDrawingReady)
        {
            // set the first point
        }
    }

    public void OnMouseDrag()
    {
        
    }


    public void OnMouseUp()
    {
        // see if the codition is read
        
    }



    public void InitDrawPlane()
    {
        isDrawingReady = false;
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
