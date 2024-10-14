using Linefy;
using Linefy.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CAD_Render_Line : MonoBehaviour
{
    [Header("Property")]
    public Lines Cad_Line;
    public Color LineColor;
    public float LineWidth;

    [Header("Status")]
    public bool isDrawing = false;

    // Start is called before the first frame update
    void Start()
    {
        Cad_Line = new Lines(1);
    }

    private void Update()
    {
        if (isDrawing)
        {
            Cad_Line.Draw(transform.localToWorldMatrix);
        }
    }



    public void SetEntity(Vector3 _start, Vector3 _end)
    {
        // consistent color and width
        Cad_Line[0] = new Line(_start, _end, LineColor, LineColor, LineWidth, LineWidth);
        isDrawing = true;
    }


    public void StopDraw()
    {
        isDrawing = false;
    }

}
