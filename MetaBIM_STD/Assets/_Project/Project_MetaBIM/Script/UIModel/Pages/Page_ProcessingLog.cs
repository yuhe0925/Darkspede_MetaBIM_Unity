using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MetaBIM;
using System;
using UnityEditor;

public class Page_ProcessingLog : MonoBehaviour
{

    public static Page_ProcessingLog Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;    
        }

        MainPanel.OnOpenAction = OnPanelOpen;
        MainPanel.OnCloseAction = OnPanelClose;
    }

    public PanelChange MainPanel;

    public void OnPanelOpen()
    {
        //LogItems.Clear();
        //ProcessingLogAdapter.SetItems(LogItems);
    }


    public void OnPanelClose()
    {
        //LogItems.Clear();
        //ProcessingLogAdapter.SetItems(LogItems);
    }


    public void Open()
    {
        MainPanel.OnPanelOpen();
    }


    public void Close()
    {
        MainPanel.OnPanelClose();
    }

    [Header("Data Buffer")]
    public ProcessingLogAdapter ProcessingLogAdapter;   
    public List<ProcessingLogItem> LogItems;
    public RectTransform ProgressBar;

    public Action<string> OnPasueAction;
    public Action<string> OnStopAction;

    public int ProgressMax = 100;

    public void AddLog(string _message)
    {
        ProcessingLogItem logItem = new ProcessingLogItem();
        logItem.Message = _message;
        LogItems.Add(logItem);
        ProcessingLogAdapter.SetItems(LogItems);
        Debug.Log("Count:           " + LogItems.Count);
        Debug.Log("AddLog Index:    " + logItem.Index);

        if(LogItems.Count > ProjectConfiguration.Instance.MAX_IENUMERATOR_RATE)
            ProcessingLogAdapter.ScrollTo(LogItems.Count-1, 1, 1);


    }

    public void OnProcess_Start(Action<string> _OnPasueAction, Action<string> _OnStopAction, int _max)
    {
        OnPasueAction = _OnPasueAction;
        OnStopAction = _OnStopAction;
        ProgressMax= _max;
        ProgressBar.localScale = new Vector3(0, 1, 1);
        MainPanel.OnPanelOpen();

        LogItems.Clear();

        ProcessingLogAdapter.SetItems(LogItems);
    }

    public void OnProcess(string _message, int _progress)
    {
        float progress = (float)_progress / (float)ProgressMax;  
        

        if (progress > 1)
            progress = 1;
        if (progress < 0)
            progress = 0;

        ProgressBar.localScale = new Vector3(progress, 1, 1);

        AddLog(_message);

        if(_progress >= ProgressMax-1)
        {
            OnProcess_Complete();
        }
    }

    public void OnProcess_Complete()
    {
        AddLog("Process Complete");
        ProgressBar.localScale = new Vector3(1, 1, 1);
    }

    public void OnProcess_Clear()
    {
        LogItems.Clear();
        ProgressBar.localScale = new Vector3(0, 1, 1);
        ProcessingLogAdapter.SetItems(LogItems);
    }

    public void OnProcess_Pasue()
    {
        OnPasueAction("");
    }

    public void OnProcess_Stop()
    {
        ProgressBar.localScale = new Vector3(0, 1, 1);
        OnStopAction("");
    }
}



   
