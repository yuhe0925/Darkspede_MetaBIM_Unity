using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class BimEdge 
{
    public Vector3 p1;
    public Vector3 p2;

    public BimEdge(Vector3 p1, Vector3 p2)
    {
        this.p1 = p1;
        this.p2 = p2;
    }

    public bool Equals(BimEdge other)
    {
      
        if (isSame(p1, other.p1) && isSame(p2, other.p2))
        {
            return true;
        }
        else if (isSame(p1, other.p2) && isSame(p2, other.p1))
        {
            return true;
        }

        return false;
        //Debug.Log(a + " | " + b + " = false");
    }
    

    public bool isSame(Vector3 a, Vector3 b)
    {
        float diff  = Vector3.Distance(a,b);
                  
        if (Mathf.Abs(diff) < 0.001)
        {
            return true;
        }
        return false;
    }
}
