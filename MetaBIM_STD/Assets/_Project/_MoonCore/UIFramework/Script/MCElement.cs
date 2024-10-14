using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class MCElement : MonoBehaviour
{

    public int Depth;


    void Awake()
    {
        if (Depth == 0)
        {
            GetObjectDepth(transform);
        }
    }



    private void GetObjectDepth(Transform _transform)
    {
        if(_transform.parent != null)
        {
            Depth++;
            GetObjectDepth(_transform.parent);
        }
    }

}
