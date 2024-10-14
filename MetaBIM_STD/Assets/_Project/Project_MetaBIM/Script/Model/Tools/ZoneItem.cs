using Linefy;
using Linefy.Serialization;
using MetaBIM;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



[ExecuteInEditMode]
public class ZoneItem : MonoBehaviour
{
    public ElementZone Item;
    public int Index; // the index of ui


    [Header("Zone Box Configuration")]
    public List<Transform> CornersTrans;
    public List<Transform> GrabPoints;
    public List<Vector3> GrabPointDirections;
    public List<SDFPoint> SDFPoints;
    public List<ZoneBoxGrab> GrabPointItems;
    public Transform Center;
    public ZoneItemLabel ZoneLabel;
    public UIBlock_BimViewer_ZoneManagement_ZoneBoxItem UIBlock;
    

    [Header("Linefy Properity")]
    public PolygonalMesh Box;
    public Lines WireFrame;
    public SerializationData_PolygonalMeshProperties polygonalMeshProperties = new SerializationData_PolygonalMeshProperties();
    public SerializationData_Lines wireframePropertyes = new SerializationData_Lines(2, Color.black, 1);

    [Header("Zone Box Propertys")]
    public Vector3[] CornerArray;
    public Polygon[] Polygon;
    public Vector2[] uvs;
    public Color[] colors;
    public Color[] MeshColors;

    private Mesh BoxMesh;
    public MeshCollider BoxCollider;
    public int[] triangles;


    [Header("Status")]
    public float BoxVolume;
    public Bounds ItemBound;
    public Bounds CurrentBound; // this is the one to use

    public bool isDrawing = true;
    public bool IsVisiable = true;  // not save in database
    public bool IsMenuUpdating = false;
    public bool IsEditing = false;


    [Header("Buffer")]
    public List<GameObject> selectedElements;


    void OnEnable()
    {
        JustInit();
    }

    void Start()
    {
        JustInit();

    }




    private void OnDestroy()
    {
        Destroy(ZoneLabel.gameObject);
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
                    MeshColors[i] = colors[1];
                    Box = new PolygonalMesh(CornerArray, uvs, MeshColors, Polygon);
                }

                wireframePropertyes.colorMultiplier = colors[0];
                WireFrame.LoadSerializationData(wireframePropertyes);
                Box.LoadSerializationData(polygonalMeshProperties);
                Box.positionEdgesWireframe = WireFrame;
            }

            WireFrame.Draw();
            Box.Draw();
        }
    }


    public void JustInit()
    {
        BoxMesh = new Mesh();

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


    public void GetCornerPosition()
    {
        for (int i = 0; i < CornerArray.Length; i++)
        {
            CornerArray[i] = CornersTrans[i].position;
            Box.SetPosition(i, CornerArray[i]);
        }
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
            CornersTrans[i].position = corners[i];
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





    // updated when drag the grab point
    public void UpdateFaceCenter(int _index)
    {
        Vector3 centerPoint = Vector3.zero;

        foreach(var grab in GrabPoints)
        {
            centerPoint = centerPoint + grab.position;

            var item = grab.GetComponent<ZoneBoxGrab>();
            if(item.ThisIndex != _index)
            {
                item.isInit = false;
            }
        }

        centerPoint = centerPoint / 8;

        //Debug.Log("Update Center: " + centerPoint);

        // this maybe calculation haevy
        BoxMesh.SetVertices(new List<Vector3>(CornerArray));
        BoxMesh.SetTriangles(triangles, 0);
        BoxMesh.RecalculateNormals();

        BoxCollider.sharedMesh = BoxMesh;

        CurrentBound = BoxMesh.bounds;

        //Center.position = centerPoint;
        Center.position = CurrentBound.center;
        // update changed data
        Item.zoneCenter = Vector3D.FromVecter3(CurrentBound.center);
        Item.zoneExtense = Vector3D.FromVecter3(CurrentBound.extents);


    }



    public void StartEditing()
    {
        IsEditing = true;
        foreach (var item in GrabPoints)
        {
            item.gameObject.SetActive(true); 
        }
    }


    public void EndEditing()
    {
        IsEditing = false;
        foreach (var item in GrabPoints)
        {
            item.gameObject.SetActive(false);
        }
    }


    // UI 

    public void OnSetVisiable(bool _isVisiable) 
    {
        IsVisiable = _isVisiable;

        // do something to set visiable of zone box in scene

        if (IsVisiable)
        {
            isDrawing = true;
            ZoneLabel.gameObject.SetActive(true);
        }
        else
        {
            isDrawing = false;
            ZoneLabel.gameObject.SetActive(false);
        }
    } 


    public void SetZoneBoxColor(Color _color)
    {
        for (int i = 0; i < 6; i++)
        {
            MeshColors[i] = _color;
            Box = new PolygonalMesh(CornerArray, uvs, MeshColors, Polygon);

            wireframePropertyes.colorMultiplier = colors[0];
            WireFrame.LoadSerializationData(wireframePropertyes);
            Box.LoadSerializationData(polygonalMeshProperties);
            Box.positionEdgesWireframe = WireFrame;
        }
    }



    public void SetZoneBoxVisable()
    {
        isDrawing = true;
    }


    public void SetZoneBoxUnvisable()
    {
        isDrawing = false;
        ZoneLabel.gameObject.SetActive(false);
    }





    public void GetElementInBoxBound()
    {

    }


    public void OnIsolateElments()
    {

    }


    public void ApplyBoxDataToZone()
    {
        //Item.zoneCenter = Vector3D.FromVecter3D(CurrentBound.center);   
    }

    // read from saved zone
    public void InitBoxFromItem()
    {
        // get 8 corners from bound
        //selectedElements = new List<GameObject>();

        List<Vector3> newCorner = new List<Vector3>();
        var center = Vector3D.FromVecter3D(Item.zoneCenter);

        var extents = Vector3D.FromVecter3D(Item.zoneExtense)
            // this is make the section box a litt bigger than the target bound
            + new Vector3(0.001f, 0.001f, 0.001f);

        Debug.Log("InitBoxFromItem: " + Item.zoneCenter);


        newCorner.Add(new Vector3(center.x - extents.x, center.y - extents.y, center.z - extents.z));  // 1
        newCorner.Add(new Vector3(center.x + extents.x, center.y + extents.y, center.z + extents.z));  // 2
        newCorner.Add(new Vector3(center.x - extents.x, center.y - extents.y, center.z + extents.z));  // 3
        newCorner.Add(new Vector3(center.x - extents.x, center.y + extents.y, center.z - extents.z));  // 4
        newCorner.Add(new Vector3(center.x + extents.x, center.y - extents.y, center.z - extents.z));  // 5
        newCorner.Add(new Vector3(center.x - extents.x, center.y + extents.y, center.z + extents.z));  // 6
        newCorner.Add(new Vector3(center.x + extents.x, center.y - extents.y, center.z + extents.z));  // 7
        newCorner.Add(new Vector3(center.x + extents.x, center.y + extents.y, center.z - extents.z));  // 8


        for(int i = 0; i < CornersTrans.Count; i++)
        {
            CornersTrans[i].position = newCorner[i];
        }

        if (ZoneLabel == null)
        {
            ZoneLabel = Instantiate(
                ResourceHolder.Instance.GetPrefabItem("FloatLabel_ZoneItem"),
                ZoneManagement.Instance.ZoneLabelParent)
                .GetComponent<ZoneItemLabel>();
            ZoneLabel.target = Center;
            ZoneLabel.gameObject.SetActive(false);
        }

        CurrentBound = new Bounds(center, extents);


        UpdateFaceCenter(-1);
   
        colors[1] = new Color(Item.zoneColor[0], Item.zoneColor[1], Item.zoneColor[2], Item.zoneColor[3]);
        SetZoneBoxColor(colors[1]);
        isDrawing = true;
        ZoneLabel.Text_ZoneName.text = Item.zoneName;
        ZoneLabel.LinkedObject = this;
    }

    // read from new zone / new selection
    public void InitBoxFromSelectedElementBound(Bounds _targetBound)
    {
        //Debug.Log("InitBoxFromSelectedElementBound" + _targetBound.center);
        Center.position = _targetBound.center;
        CurrentBound = _targetBound;


        if (ZoneLabel == null)
        {
            ZoneLabel = Instantiate(
                ResourceHolder.Instance.GetPrefabItem("FloatLabel_ZoneItem"),
                ZoneManagement.Instance.ZoneLabelParent)
                .GetComponent<ZoneItemLabel>();
            ZoneLabel.target = Center;
            ZoneLabel.gameObject.SetActive(false);
        }

        // create box
        // this is make the section box a litt bigger than the target bound
        CurrentBound.size = CurrentBound.size + new Vector3(0.001f, 0.001f, 0.001f);

        CornerArray = GetCornerPositionOfBound(CurrentBound).ToArray();
        Polygon = GetPolygonsOfBoundCorners(CornerArray).ToArray();
        Box = new PolygonalMesh(CornerArray, uvs, MeshColors, Polygon);


        // update box
        for (int i = 0; i < CornerArray.Length; i++)
        {
            Box.SetPosition(i, CornerArray[i]);
        }


        UpdateFaceCenter(-1);

        colors[1] = new Color(Item.zoneColor[0], Item.zoneColor[1], Item.zoneColor[2], Item.zoneColor[3]);
        SetZoneBoxColor(colors[1]);
        isDrawing = true;
        ZoneLabel.Text_ZoneName.text = Item.zoneName;
        ZoneLabel.LinkedObject = this;
    }

    public void InitBoxFromSelectedElement()
    {
        //Debug.Log("InitBoxFromSelectedElement");
    

        int index = 0;

        foreach (var item in selectedElements)
        {
            var element = item.GetComponent<BIMElement>();

            if (index == 0)
            {
                CurrentBound = item.GetComponent<BIMElement>().Collider.bounds;
            }
            else
            {
                CurrentBound.Encapsulate(item.GetComponent<BIMElement>().Collider.bounds);
            }

            index++;

        }

        if (ZoneLabel == null)
        {
            ZoneLabel = Instantiate(
                ResourceHolder.Instance.GetPrefabItem("FloatLabel_ZoneItem"),
                ZoneManagement.Instance.ZoneLabelParent)
                .GetComponent<ZoneItemLabel>();
            ZoneLabel.target = Center;
            ZoneLabel.gameObject.SetActive(false);
        }

        // create box
        // this is make the section box a litt bigger than the target bound
        CurrentBound.size = CurrentBound.size + new Vector3(0.001f, 0.001f, 0.001f);

        CornerArray = GetCornerPositionOfBound(CurrentBound).ToArray();
        Polygon = GetPolygonsOfBoundCorners(CornerArray).ToArray();
        Box = new PolygonalMesh(CornerArray, uvs, MeshColors, Polygon);


        // update box
        for (int i = 0; i < CornerArray.Length; i++)
        {
            Box.SetPosition(i, CornerArray[i]);
        }


        UpdateFaceCenter(-1);

        colors[1] = new Color(Item.zoneColor[0], Item.zoneColor[1], Item.zoneColor[2], Item.zoneColor[3]);
        SetZoneBoxColor(colors[1]);
        isDrawing = true;
        ZoneLabel.Text_ZoneName.text = Item.zoneName;
        ZoneLabel.LinkedObject = this;
    }


    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter: ");
    }

}
