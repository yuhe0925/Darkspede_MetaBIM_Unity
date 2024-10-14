using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Linefy;


[Serializable]
[ExecuteInEditMode]
public class BimMesh : MonoBehaviour
{
    public List<BimFace> Faces;
    public int BorderEdgeCount;

    public bool isDrawBorder;

    public Color LineColor = Color.black;
    public float LineWidth = 1f;

    public float ProcessedTime;

    // field
    private Lines BorderLiens;

    public BimMesh()
    {
        Faces = new List<BimFace>();
        BorderLiens = null;
        isDrawBorder = false;
    }


    void Update()
    {
        if (isDrawBorder)
        {
            BorderLiens.Draw();
        }
    }



    public void OnMouseEnter()
    {
        if (BorderLiens.count >= 4)
        {
            //isDrawBorder = true;
        }
    }

    public void OnMouseExit()
    {
        //isDrawBorder = false;
    }


    public void ProcessObjectBorder()
    {
        Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        Matrix4x4 localToWorld = gameObject.transform.localToWorldMatrix;

        // Get the vertices, triangles, and normals of the mesh
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Iterate over each triangle in the mesh
        for (int i = 0; i < triangles.Length - 2; i += 3)
        {
            // Get the three vertices that make up the triangle
            Vector3 vertex1 = transform.TransformPoint(vertices[triangles[i]]);
            Vector3 vertex2 = transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 vertex3 = transform.TransformPoint(vertices[triangles[i + 2]]);

            AddToFace(vertex1, vertex2, vertex3);
        }

        foreach (var face in Faces)
        {
            face.RemoveSharedEdge();
            BorderEdgeCount += face.BorderEdges.Count;
        }

        CreateBorderLineBatch(LineColor, LineWidth);

        isDrawBorder = true;
    }




    public void CreateBorderLineBatch(Color lineColor, float lineWidth)
    {
        BorderLiens = new Lines(BorderEdgeCount);

        int EdgeCount = 0;

        foreach (var face in Faces)
        {
            for (int i = 0; i < face.BorderEdges.Count; i++)
            {
                //Debug.DrawLine(face.BorderEdges[i].p1, face.BorderEdges[i].p2, Color.red, 5f);
                BorderLiens[EdgeCount] = new Line(face.BorderEdges[i].p1, face.BorderEdges[i].p2, lineColor, lineColor, lineWidth, lineWidth);
                EdgeCount++;
            }
        }

        if (BorderLiens.count >= 4)
        {
            isDrawBorder = true;
        }
    }


    public void SetSetBorderLine(Color lineColor, float lineWidth)
    {
        for (int i = 0; i < BorderLiens.count; i++)
        {
            BorderLiens.SetColor(i, lineColor);
            BorderLiens.SetWidth(i, lineWidth);
        }
    }

    public void AddToFace(Vector3 _p1, Vector3 _p2, Vector3 _p3)
    {
        Vector4 PlaneCoefficients = CalculatePlaneCoefficients(_p1, _p2, _p3);

        Plane plane = new Plane(_p1, _p2, _p3);
        Vector3 narmal = plane.normal;


        BimFace face = GetFace(narmal);

        if (face == null)
        {
            face = new BimFace(narmal);
            Faces.Add(face);
        }

        face.AddTriangle(new BimTriangle(_p1, _p2, _p3));
    }



    private Vector4 CalculatePlaneCoefficients(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        Vector3 normal = Vector3.Cross(vertex2 - vertex1, vertex3 - vertex1).normalized;
        float d = -Vector3.Dot(normal, vertex1);
        return new Vector4(normal.x, normal.y, normal.z, d);
    }


    public BimFace GetFace(Vector3 _normal)
    {
        foreach (var face in Faces)
        {
            if (isSame(face.Normal, _normal))
            {
                return face;
            }
        }

        return null;
    }


    private bool isSame(Vector3 a, Vector3 b)
    {
        float diff = Vector3.Distance(a, b);

        if (Mathf.Abs(diff) < 0.001)
        {
            return true;
        }
        return false;
    }


}
