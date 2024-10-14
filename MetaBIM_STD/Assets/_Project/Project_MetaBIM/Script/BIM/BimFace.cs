using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

[Serializable]
public class BimFace
{
    public Vector3 Normal;
    public double distance;
    
    public List<BimTriangle> Triangles;
    public List<BimEdge> Edges;
    public List<BimEdge> BorderEdges;

    public BimFace(Vector4 _Normal)
    {
        Normal = _Normal;
        Triangles = new List<BimTriangle>();
        Edges = new List<BimEdge>();
        BorderEdges = new List<BimEdge>();
    }


    public void RemoveSharedEdge()
    {
        BorderEdges = new List<BimEdge>(Edges);

        for (int i = BorderEdges.Count - 1; i > 0; i--)
        {
            for (int n = i - 1; n >= 0; n--)
            {
                if (BorderEdges[i].Equals(BorderEdges[n]))
                {
                    // shared edge so remove both
                    BorderEdges.RemoveAt(i);
                    BorderEdges.RemoveAt(n);
                    i--;
                    break;
                }
            }
        }

    }




    public void AddTriangle(BimTriangle _bimTriangle)
    {
        Triangles.Add(_bimTriangle);
        BimEdge v1 = new BimEdge(_bimTriangle.P1, _bimTriangle.P2);
        BimEdge v2 = new BimEdge(_bimTriangle.P2, _bimTriangle.P3);
        BimEdge v3 = new BimEdge(_bimTriangle.P3, _bimTriangle.P1);
        Edges.Add(v1);
        Edges.Add(v2);
        Edges.Add(v3);

 
    }


    public (bool ,bool , bool) GetEdge(BimEdge _e1, BimEdge _e2, BimEdge _e3)
    {
        bool r1 = false, r2=false, r3=false;

        foreach(var e in Edges)
        {
            if(_e1.p1 == e.p1 && _e1.p2 == e.p2)
            {
                r1 = true;
            }
            else if (_e1.p1 == e.p2 && _e1.p2 == e.p1)
            {
                r1 = true;
            }

            if (_e2.p1 == e.p1 && _e2.p2 == e.p2)
            {
                r2 = true;
            }
            else if (_e1.p1 == e.p2 && _e2.p2 == e.p1)
            {
                r2 = true;
            }

            if (_e3.p1 == e.p1 && _e3.p2 == e.p2)
            {
                r3 = true;
            }
            else if (_e3.p1 == e.p2 && _e3.p2 == e.p1)
            {
                r3 = true;
            }
        }
        
        return (r1, r2, r3);
    }


    public void ProcessBorder()
    {
        for(int i = 0; i < Triangles.Count; i++)
        {
            BimTriangle triangle = Triangles[i];

            
        }
    }

}
