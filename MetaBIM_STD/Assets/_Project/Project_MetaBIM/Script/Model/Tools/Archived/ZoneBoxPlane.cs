using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class ZoneBoxPlane : MonoBehaviour
{

    [Header("Zone Box")]
    public int ZoneBoxIndex;
    public int PlaneIndex;  // of the box;
    public ZoneBox ZoneBox;
    public Transform[] PolyCorners;  // to track
    public ZoneBoxGrab[] GrabPoints;  // to track
    public List<Transform> TopCorners;  // to track
    public int[] cornerIndex;

    [Header("Buffer")]
    public MeshFilter MeshFilter;
    public MeshCollider Collider;
    public Vector3[] Corner;
    public int[] triangle;
    public bool IsHovering;
    public Vector3 Normal;
    public Vector3 position;
    
    [Header("Lock")]
    public bool Selected;
    public int LockIndex = 0;
    public List<Transform> LockedCorners;  // to track


    // BUFFER
    private Mesh Mesh;

    // Start is called before the first frame update
    void Start()
    {
        Mesh = MeshFilter.mesh;
        GrabPoints = new ZoneBoxGrab[PolyCorners.Length];

        for (int i = 0; i < PolyCorners.Length; i++)
        {
            PolyCorners[i] = ZoneBox.Corners[ZoneBox.Polygon[PlaneIndex].corners[i].position];
            GrabPoints[i]  = PolyCorners[i].gameObject.GetComponent<ZoneBoxGrab>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < PolyCorners.Length; i++)
        {
            Corner[i] = PolyCorners[i].position;
        }

        Mesh.vertices = Corner;
        Mesh.triangles = triangle;
        //Mesh.RecalculateBounds();
        Collider.sharedMesh = Mesh;
        
        Corner = Mesh.vertices;

        Normal = Utility.GetNormalofTriangle(Corner[0], Corner[1], Corner[2]);
        GetPlaneCenter(Corner[0], Corner[1], Corner[2]);

        // get my corners to follow locked coners

        // this is a hardcoded process, do something smart latyer
        // TODO

        if (ZoneBox.IsPanelLocked && LockedCorners.Count == 4)
        {
            GrabPoints[2].isLocked = true;
            GrabPoints[3].isLocked = true;
            GrabPoints[2].MovePoint(LockedCorners[2].position);
            GrabPoints[3].MovePoint(LockedCorners[3].position);
        }

    }


    
    

    public void OnMouseHover()
    {
        if (!IsHovering)
        {
            IsHovering = true;
            ZoneBox.SetBoxSelectPlaneColor(PlaneIndex, ZoneBox.Action.OnMouseEnter);
        }
    }

    public void OnMouseLeave()
    {
        if (IsHovering)
        {
            IsHovering = false;
            ZoneBox.SetBoxSelectPlaneColor(PlaneIndex, ZoneBox.Action.OnMouseExit);
        }
    }

    public void OnMouseSelect()
    {

        ZoneBox.SetBoxSelectPlaneColor(PlaneIndex, ZoneBox.Action.OnMouseSelect);
    }
    
    public void OnMouseUnselected()
    {

        ZoneBox.SetBoxSelectPlaneColor(PlaneIndex, ZoneBox.Action.OnMouseUnselect);
    }


    public void GetPlaneCenter(Vector3 _a, Vector3 _b, Vector3 _c)
    {
        position = (_a + _b + _c) / 3;
    }
}
