using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageStatus 
{
    // CONFIG
    public static bool IsDrawingModelEdge = false;
    public static int MIN_MODEL_EDGE_LINE = 4;
    public static int MAX_MODEL_EDGE_LINE = 200;
    public static float SCROLL_TO_ITEM_TIME = 0.1f;


    // STATE
    public static bool IsShowZoneSectionBox = false;
    public static bool IsShowZoneLevel= false;
    public static bool IsPanelDragging = false;

    public static void OnReset()
    {
        IsDrawingModelEdge = false;
        IsShowZoneSectionBox = false;
    }
}
