using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCenterBound : MonoBehaviour
{
    public Bounds combinedBounds = new Bounds();
    public Vector3 PivitPoint;


    public void FindBound()
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer render in renderers)
        {
            combinedBounds.Encapsulate(render.bounds);
        }

        PivitPoint = gameObject.transform.position;
    }
}
