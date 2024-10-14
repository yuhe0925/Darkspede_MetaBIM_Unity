using Linefy;
using Linefy.Serialization;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//[ExecuteInEditMode]
public class ZoneLevelPlane : MonoBehaviour
{
    public PolygonalMesh Box;
    public Lines WireFrame;
    public bool isDrawing = true;

    [Header("Bound / Mesh Data")]

    public Vector3[] CornerArray;
    public Polygon[] Polygon;
    public Vector2[] uvs;
    public Color[] colors;

    public List<Transform> LockedCorners;
    public float Height;
    public BimLevel level;

    [Header("Linefy Properity")]
    public SerializationData_PolygonalMeshProperties polygonalMeshProperties = new SerializationData_PolygonalMeshProperties();
    public SerializationData_Lines wireframePropertyes = new SerializationData_Lines(2, Color.black, 1);

    void Start()
    {
    
    }


    void Update()
    {
        if (isDrawing)
        {
            SetItem();

            Box.LoadSerializationData(polygonalMeshProperties);
            Box.positionEdgesWireframe = WireFrame;
            WireFrame.LoadSerializationData(wireframePropertyes);
            Box.Draw(transform.localToWorldMatrix);
            WireFrame.Draw(transform.localToWorldMatrix);
        }
    }

    public void SetItem()
    {
        // create box
        CornerArray = GetCornerPositionOfBound().ToArray();
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

        if (level != null)
        {
            if (level.LevelCurrentHeight == 0)
            {
                Height = 0;
            }
            else
            {
                Height = level.LevelCurrentHeight / 1000;  // unit is in mm
            }
        }

    }


    public void SetPlane(List<Transform> _corners, float _height, BimLevel _level)
    {
        LockedCorners = _corners;
        Height = _height;
        level = _level;

        isDrawing = true;
    }


    private List<Vector3> GetCornerPositionOfBound()
    {
        List<Vector3> corners = new List<Vector3>();

        Vector3 boundPoint1 = LockedCorners[0].position;
        Vector3 boundPoint2 = LockedCorners[1].position;
        Vector3 boundPoint3 = LockedCorners[2].position;
        Vector3 boundPoint4 = LockedCorners[3].position;

        boundPoint1.y = Height;
        boundPoint2.y = Height;
        boundPoint3.y = Height;
        boundPoint4.y = Height;


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
