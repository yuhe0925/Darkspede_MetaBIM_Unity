using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelsHierarchy : MonoBehaviour
{
    static Canvas MainCanvas;

    private void Start()
    {
        MainCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    public static void SetAsTopPanelNoBlur(Transform panel)
    {
        panel.SetAsLastSibling();
    }

    public static void SetAsTopPanel(Transform panel)
    {
        panel.SetAsLastSibling();
    }

    public static void HideBlocker()
    {
    }

    public static float GetCanvasScaler()
    {
        return MainCanvas.scaleFactor;
    }
}
