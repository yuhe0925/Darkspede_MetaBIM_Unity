using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MetaBIM;
using UnityEngine.UI;
using System;
using System.Linq;

public class Page_ShareLink : MonoBehaviour
{
    public static Page_ShareLink Instance;
    public PanelChange Panel;
    public List<GameObject> DisableObject;


    [Header("Data Buffer")]
    public bool IsLoadingData = false;
    public int LoadProjectStatus = 0;
    public Workspace SelectedWorkspace;


    [Header("UI Element")]
    public TextMeshProUGUI Text_LoadingMessage;
    public RectTransform Rect_LoadingBar;
    public float MaxLoadingBarWidth = 500;




    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        Panel.OnOpenAction = OnOpenAction;
        Panel.OnCloseAction = OnCloseAction;
    }



    public void OnOpenAction()
    {
        IsLoadingData = true;
        LoadProjectStatus = 0;

        // reset LoadingBar
        Rect_LoadingBar.sizeDelta = new Vector2(10, Rect_LoadingBar.sizeDelta.y);
        Text_LoadingMessage.text = "";

        if (AppController.Instance.CurrentShareLinkPackage.shared)
        {
            OnRenderProjectModels(AppController.Instance.CurrentShareLinkPackage);
        }


        foreach (var item in DisableObject)
        {
            item.SetActive(false);
        }
    }


    public void OnCloseAction()
    {
        IsLoadingData = false;
    }


    
    public void OnRenderProjectModels(ShareLinkPackage _sharelink)
    {
        Debug.Log("OnRenderProjectModels");
        SelectedWorkspace = _sharelink.workspace;
        _sharelink.CompleteCallback = OnModelLoadCompleteCallback;
        _sharelink.LoadingProgress = OnLoadingProgress;

        ProjectModelHandler.Instance.AddNewModelToHandle(_sharelink.workspaceGuid, _sharelink.version, _sharelink.project);
        ProjectModelHandler.Instance.OnModelDownload(_sharelink.version.attachedProject, _sharelink.version.guid, null);
    }

    public void OnLoadingProgress(float _progress)
    {
        float p = _progress * MaxLoadingBarWidth;

        if(p > MaxLoadingBarWidth)
        {
            p = MaxLoadingBarWidth;
        }

        if(_progress < 0.5f)
        {
            Text_LoadingMessage.text = StringBuffer.ShareLink_Message_Downloading.S;
        }
        else
        {
            Text_LoadingMessage.text = StringBuffer.ShareLink_Message_Processing.S;
        }
        
        Rect_LoadingBar.sizeDelta = new Vector2(p, Rect_LoadingBar.sizeDelta.y);     
    }

    public void OnModelLoadCompleteCallback(bool _result)
    {
        foreach (ModelVersion mv in ProjectModelHandler.Instance.modelVersions)
        {
            mv.EnableModel();
        }
        
        Page_BIMViewer.Instance.SelectedVersion = AppController.Instance.CurrentShareLinkPackage.version;
        Page_BIMViewer.Instance.OnLoadBimObjectsIntoSceneFromhandler(true);
        AppController.Instance.SetPage(AppController.PageIndex.bimview);
    }





    #region Utility


    #endregion


    #region MISC




    #endregion
}
