using MetaBIM;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[ExecuteInEditMode]
public class ZoneBoxGrab : MonoBehaviour
{
    public FreeCameraNav cameraNav;
    public ZoneItem ZoneItem;
    public List<Transform> RaletedPoints;
    public bool MoveOn_X;
    public bool MoveOn_Y;
    public bool MoveOn_Z;

    public Vector3 offset;
    public int ThisIndex;
    
    public Vector3 newPosition;

    public bool isLocked;


    public bool isInit = true;


    public void Start()
    {
        if(cameraNav == null)
        {
            cameraNav = Camera.main.GetComponent<FreeCameraNav>();
        }
    }


    public void Update()
    {
        if (!isInit)
        {
            OnPointInit();
            isInit = true;
        }
    }

    public void OnPointInit()
    {
        var centerPoint = Vector3.zero;

        foreach (var point in RaletedPoints)
        {
            centerPoint += point.position;
        }

        centerPoint = centerPoint/RaletedPoints.Count;

        this.transform.position = centerPoint;
    }

    void OnMouseDown()
    {
        if (MouseInputUIBlocker.BlockedByUI)
        {
            return;
        }

        cameraNav.enabled = false;
        offset = transform.position - MouseWorldPosition();
    }


    void OnMouseDrag()
    {
        if (MouseInputUIBlocker.BlockedByUI)
        {
            return;
        }

        // not moving if locked
        if(isLocked)
        {
            return; 
        }
        
        newPosition = MouseWorldPosition();
        float interval = ZoneManagement.Instance.DragInterval;

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            newPosition = new Vector3(
                ((int)(newPosition.x / interval)) * interval,
                ((int)(newPosition.y / interval)) * interval,
                ((int)(newPosition.z / interval)) * interval);
        }

        ZoneManagement.Instance.IsZoneDragging = true;

        MovePoint(newPosition);

        ZoneItem.UpdateFaceCenter(ThisIndex);


    }
    

    void OnMouseExit()
    {

    }

    void OnMouseUp()
    {
        if (MouseInputUIBlocker.BlockedByUI)
        {
            return;
        }

        cameraNav.enabled = true;
        ZoneManagement.Instance.IsZoneDragging = false;
    }

    public void MovePoint(Vector3 _newPosition)
    {
        if (MoveOn_X)
        {
            transform.position = new Vector3(_newPosition.x, transform.position.y, transform.position.z);
        }

        if (MoveOn_Y)
        {
            transform.position = new Vector3(transform.position.x, _newPosition.y, transform.position.z);
        }

        if (MoveOn_Z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, _newPosition.z);
        }


        foreach (Transform point in RaletedPoints)
        {
            if (MoveOn_X)
            {
                point.position = new Vector3(transform.position.x, point.position.y, point.position.z);
            }

            if (MoveOn_Y)
            {
                point.position = new Vector3(point.position.x, transform.position.y, point.position.z);
            }

            if (MoveOn_Z)
            {
                point.position = new Vector3(point.position.x, point.position.y, transform.position.z);
            }
        }
    }




    Vector3 MouseWorldPosition()
    {
        var mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mouseScreenPos);
    }
}
