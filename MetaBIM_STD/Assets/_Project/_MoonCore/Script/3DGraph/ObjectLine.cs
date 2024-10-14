using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLine : MonoBehaviour
{
    public LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void DrawLineTo(Transform _transform)
    {
        if (lineRenderer != null)
        {
            lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
        }
    }
}
