using MetaBIM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectConfiguration : MonoBehaviour
{

    public static ProjectConfiguration Instance;

    // ===============================================

    [Header("Project Configuration")]
    public string ProjectName;
    public bool IfLoadingExample;
    public LocationType DefaultLanguage;
    public AppController.PageIndex DefaultMainPage;


    [Header("Page Status")]
#if (UNITY_WEBGL && !UNITY_EDITOR)
    public int MIN_MODEL_EDGE_LINE = 4;
    public  int MAX_MODEL_EDGE_LINE = 20;
#else
    public int MIN_MODEL_EDGE_LINE = 4;
    public int MAX_MODEL_EDGE_LINE = 400;
#endif
      

    [Header("Viewer Config")]
    [Header("================================")]
    // show search matched result in element list
    public bool IsDisplaySearchResult = false;

    // show selected element in element list
    public bool IsDisplaySelectedElement = false;



    public bool IsDrawingModelEdge = false;
    public bool IsInInsolateMode = false;

    public bool IsShowSplitedObjects = true;



    [Header("Data Config")]
    [Header("================================")]
    public bool IsExportHideElement = true;


    [Header("System")]
    public int APP_TARGET_FRAME_RATE = 60;
    public int MAX_IENUMERATOR_RATE = 8;
    public int GIS_MAX_ICON_DISTANCE = 1000;  // max distance between camera and object to switch between building and icon


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        // Check Page Status
        PageStatus.IsDrawingModelEdge = IsDrawingModelEdge;
        PageStatus.MIN_MODEL_EDGE_LINE = MIN_MODEL_EDGE_LINE;
        PageStatus.MAX_MODEL_EDGE_LINE = MAX_MODEL_EDGE_LINE;

    }



    public enum LocationType
    {
        EN,
        ZH,
        System,
    }
}
