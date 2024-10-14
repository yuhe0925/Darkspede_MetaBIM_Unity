using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionBoxGrab : MonoBehaviour
{
    public FreeCameraNav cameraNav;
    public List<Transform> RaletedPoints;
    public bool MoveOn_X;
    public bool MoveOn_Y;
    public bool MoveOn_Z;

    public Vector3 offset;
    public SectionBox SectionBox;
    public int ThisIndex;

    void OnMouseDown()
    {
        cameraNav.enabled = false;
        offset = transform.position - MouseWorldPosition();
    }


    void OnMouseDrag()
    {
        Vector3 newPosition = MouseWorldPosition();

        if (MoveOn_X)
        {
            transform.position = new Vector3(newPosition.x, transform.position.y, transform.position.z);
        }

        if (MoveOn_Y)
        {
            transform.position = new Vector3(transform.position.x, newPosition.y, transform.position.z);
        }

        if (MoveOn_Z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, newPosition.z);
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

        SectionBox.OnUpdateDragPoint(ThisIndex);
    }

    void OnMouseUp()
    {
        cameraNav.enabled = true;
    }

    Vector3 MouseWorldPosition()
    {
        var mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mouseScreenPos);
    }
}
