using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SplitingPlanePointGrab : MonoBehaviour
{
    public FreeCameraNav cameraNav;
    public SplitingPlane splitingPlane;
    public List<Transform> RaletedPoints;
    public bool MoveOn_X;
    public bool MoveOn_Y;
    public bool MoveOn_Z;

    public Vector3 offset;
    public int ThisIndex;

    public Vector3 newPosition;
    public Vector3 distanceMoved;
    public Vector3 lastPosition;
    public bool isLocked;
    public int interval = 1;
    public bool isCenterGrab;
    public bool isFollowMovement;


    private void Start()
    {
        cameraNav = Camera.main.GetComponent<FreeCameraNav>();
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
        if (isLocked)
        {
            return;
        }

        newPosition = MouseWorldPosition();

        lastPosition=  transform.position;

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            newPosition = new Vector3(
                ((int)(newPosition.x / interval)) * interval,
                ((int)(newPosition.y / interval)) * interval,
                ((int)(newPosition.z / interval)) * interval);
        }


        distanceMoved = newPosition - transform.position;

        MovePoint(newPosition);

 


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

        distanceMoved = transform.position - lastPosition;


        // update center
        if (!isCenterGrab)
        {
            splitingPlane.OnUpdataCenter();

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
        else
        {
            foreach (Transform point in RaletedPoints)
            {
                point.position += distanceMoved;
            }
        }
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

        splitingPlane.SavePlane();

    }


    Vector3 MouseWorldPosition()
    {
        var mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mouseScreenPos);
    }
}
