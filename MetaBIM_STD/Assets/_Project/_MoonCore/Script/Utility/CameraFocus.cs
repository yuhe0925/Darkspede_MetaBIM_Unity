using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{

    public static CameraFocus Instance;

    public Transform CameraTransform;
    public Transform Target;


    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }


    public void OnFocus(Transform _target)
    {
        if(Target.gameObject.GetComponent<Renderer>() != null)
        {
            CameraTransform.LookAt(_target);

        }
    }
}
