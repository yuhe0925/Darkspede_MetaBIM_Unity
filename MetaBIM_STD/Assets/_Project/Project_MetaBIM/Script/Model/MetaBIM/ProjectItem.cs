using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectItem : MonoBehaviour
{
   

    public string BuildingItemName;
    public Transform CameraTransform;

    public OnlineMapsMarker3D Marker_Building;
    public OnlineMapsMarker3D Marker_Icon;

    


    private void Update()
    {
        // swith between Marker building and Marker Icon by the distance between camera and the object

        if (Vector3.Distance(CameraTransform.position, transform.position) < ProjectConfiguration.Instance.GIS_MAX_ICON_DISTANCE)
        {
            Marker_Building.enabled = true;
            Marker_Icon.enabled = false;
        }
        else
        {
            Marker_Building.enabled = false;
            Marker_Icon.enabled = true;
        }
    }
}
