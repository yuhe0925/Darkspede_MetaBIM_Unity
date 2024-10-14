using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter))]
public class WireframeDrawer : MonoBehaviour
{
    public Color lineColor = Color.green;
    public bool IsDrawing = false;

    private Material lineMaterial;
    private Mesh mesh;
    private Dictionary<string, EdgeData> edges = new Dictionary<string, EdgeData>();

    struct EdgeData
    {
        public Vector3 normal;
        public List<Vector3> vertices;
    }

    void Start()
    {
        // Initialize the material
        lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        lineMaterial.SetInt("_ZWrite", 0);

        mesh = GetComponent<MeshFilter>().mesh;
        CalculateUniqueEdges();
    }

    void CalculateUniqueEdges()
    {
        var vertices = mesh.vertices;
        var triangles = mesh.triangles;
        var normals = mesh.normals;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 faceNormal = (normals[triangles[i]] + normals[triangles[i + 1]] + normals[triangles[i + 2]]) / 3;

            AddEdge(triangles[i], triangles[i + 1], faceNormal, vertices);
            AddEdge(triangles[i + 1], triangles[i + 2], faceNormal, vertices);
            AddEdge(triangles[i + 2], triangles[i], faceNormal, vertices);
        }
    }

    void AddEdge(int index1, int index2, Vector3 normal, Vector3[] vertices)
    {
        string edgeKey = index1 < index2 ? $"{index1}-{index2}" : $"{index2}-{index1}";

        if (edges.TryGetValue(edgeKey, out EdgeData data))
        {
            // Only keep the edge if normals are not equal, implying it's a boundary edge
            if (Vector3.Dot(data.normal, normal) < 0.9f)  // Use a tolerance to account for floating-point imprecision
            {
                edges[edgeKey] = new EdgeData { normal = normal, vertices = new List<Vector3> { vertices[index1], vertices[index2] } };
            }
            else
            {
                // Remove the edge if normals are similar, as it's an internal edge
                edges.Remove(edgeKey);
            }
        }
        else
        {
            edges.Add(edgeKey, new EdgeData { normal = normal, vertices = new List<Vector3> { vertices[index1], vertices[index2] } });
        }
    }

    void OnRenderObject()
    {
        if (!IsDrawing) return;
        if (lineMaterial == null) return;

        lineMaterial.SetPass(0);
        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Color(lineColor);

        foreach (var edge in edges.Values)
        {
            GL.Vertex(edge.vertices[0]);
            GL.Vertex(edge.vertices[1]);
        }

        GL.End();
        GL.PopMatrix();
    }

    void OnDestroy()
    {
        if (lineMaterial != null)
        {
            DestroyImmediate(lineMaterial);
        }
    }


    public void SetEnable(bool enable)
    {
        IsDrawing = enable;
    }



}
