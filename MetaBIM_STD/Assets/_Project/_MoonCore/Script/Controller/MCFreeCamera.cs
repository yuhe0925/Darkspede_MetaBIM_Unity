using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Camera))]
public class MCFreeCamera : MonoBehaviour
{
    public CameraOperationMode mode = CameraOperationMode.orbit;

    [Header("Components")]
    public Camera CurrentCamera;
    public Camera TargetCamera;
    public enum CameraOperationMode
    {
        orbit,    // object mode, orbiting ,pening, zooming
        fpv,      // FPS mode, free walking, flying
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (MouseInputUIBlocker.BlockedByUI)
        {
            return;
        }

        if(mode == CameraOperationMode.orbit)
        {
            CameraOrbitMode();
        }
        else if (mode == CameraOperationMode.fpv)
        {
            CameraFpvMode();
        }
    }


    private void CameraOrbitMode()
    {

    }


    private void CameraFpvMode()
    {

    }


}
