using Linefy;
using Linefy.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MetaBIM;


[Serializable]
public class BoundBox : MonoBehaviour
{

    
    public PolygonalMesh Box;
    public Lines WireFrame;
    public bool isDrawing = false;

    public Vector3[] CornerArray;
    public Polygon[] Polygon;
    public Vector2[] uvs;

    public int[] triangles;

    public float BoxVolume;
    public float BoxArea;
    public float BoxExtentScale = 1.00f;

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



    void Start()
    {
        for (int i = 0; i < MeshColors.Length; i++)
        {
            MeshColors[i] = colors[1];
        }

        WireFrame = new Lines(12);
        Box = new PolygonalMesh(CornerArray, uvs, MeshColors, Polygon);
        wireframePropertyes.colorMultiplier = colors[0];

        Box.LoadSerializationData(polygonalMeshProperties);
        WireFrame.LoadSerializationData(wireframePropertyes);
    }




    void Update()
    {
        if (isDrawing)
        {
            Box.LoadSerializationData(polygonalMeshProperties);
            Box.positionEdgesWireframe = WireFrame;
            Box.Draw(transform.localToWorldMatrix);
            //Box.Draw();

            WireFrame.LoadSerializationData(wireframePropertyes);
            wireframePropertyes.colorMultiplier = colors[0];
            WireFrame.Draw(transform.localToWorldMatrix);
            //WireFrame.Draw();

        }



    }


    
    #region Box Init

    // TODO, init a box with rotation
    public void InitBox(Bounds _targetBound, Vector3 _offset, Vector3 _rotation)
    {
        SavedBound = _targetBound;

        Offset = _offset;


        // this is make the section box a litt bigger than the target bound, to avoide z-index fighting
        //SavedBound.size = SavedBound.size * BoxExtentScale;
        //Debug.Log("SavedBound: " + SavedBound.size);
        SavedBound.extents = SavedBound.extents  + new Vector3(0.1f, 0.1f, 0.1f);
        //Debug.Log("SavedBound: " + SavedBound.size);

        // create box
        CornerArray = GetCornerPositionOfBound(SavedBound).ToArray();
        Polygon = GetPolygonsOfBoundCorners(CornerArray).ToArray();

        for (int i = 0; i < 6; i++)
        {
            MeshColors[i] = colors[1];
        }
    
        Box = new PolygonalMesh(CornerArray, uvs, MeshColors, Polygon);


        // update box
        for (int i = 0; i < CornerArray.Length; i++)
        {
            Box.SetPosition(i, CornerArray[i]);
        }

        // setposition of box
        transform.position = Offset;

        // this may not be correct to rotato the bound box to fix the target bound
        transform.rotation = Quaternion.Euler(_rotation);  
        
        isDrawing = true;
    }



    public void SetColor(Color _boxColor)
    {
        for (int i = 0; i < 6; i++)
        {
            MeshColors[i] = _boxColor;
            Box = new PolygonalMesh(CornerArray, uvs, MeshColors, Polygon);
        }
    }
    
    public void DisableBox()
    {
        isDrawing = false;
        
    }

    #endregion




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
            //Corners[i].position = corners[i];
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







}
