using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEditor;

public class Openiframe : MonoBehaviour
{
    // ===============================================================================================================================
    // Calling external (JS) functions out side of Unity build
    // ===============================================================================================================================

    [DllImport("__Internal")]
    private static extern void OpenBimFrameByUrl(string url);

    [DllImport("__Internal")]
    private static extern void OpenBimFrame();

    [DllImport("__Internal")]
    private static extern void CloseBimFrame();

    [DllImport("__Internal")]
    private static extern void ResizeBimFrame(string height, string width);


    [DllImport("__Internal")]
    private static extern void OpenUploaderFrame(string item, string user);

    [DllImport("__Internal")]
    private static extern void CloseUploaderFrame();

    [DllImport("__Internal")]
    private static extern void OpenBimUploaderFrame(string _attachedProject, string _projectName, string _attachedTaskGroup, string _taskGroupName, string _attachedUser, string _actionTarget);

    [DllImport("__Internal")]
    private static extern void CloseBimUploaderFrame();

    [DllImport("__Internal")]
    private static extern void OpenXMLUploaderFrame(string item, string user);

    [DllImport("__Internal")]
    private static extern void CloseXMLUploaderFrame();



    // ===============================================================================================================================
    // This methods are to be call by Unity Script
    // ===============================================================================================================================
    public static void OnOpenBimFrameByUrl(string _url)
    {
        Debug.Log("UNITY: OnOpenBimFrameByUrl: " + _url);
#if !UNITY_EDITOR
        OpenBimFrameByUrl(_url);
#endif
    }

    public static void OnOpenBimFrame()
    {
        Debug.Log("UNITY: OnOpenBimFrame");
#if !UNITY_EDITOR
        OpenBimFrame();
#endif
    }

    public static void OnCloseBimFrame()
    {
        Debug.Log("UNITY: OnCloseBimFrame");
#if !UNITY_EDITOR
        CloseBimFrame();
#endif
    }

    public static void OnResizeBimFrame(int _height, int _width)
    {
        string h = _height + "px";
        string w = _width + "px";
        Debug.Log("UNITY OnResizeBimFrame: " + h + " | " + w);
#if !UNITY_EDITOR
        ResizeBimFrame(h,w);
#endif
    }

    public static void OnOpenUploaderFrame(string _item, string _user)
    {
        string item = _item;
        string user = _user;
        Debug.Log("UNITY: OpenUploaderFrame " + item + " | " + user);
#if !UNITY_EDITOR
        OpenUploaderFrame(item,user);
#endif
    }

    public static void OnCloseUploaderFrame()
    {
        Debug.Log("UNITY: OnCloseUploaderFrame ");
#if !UNITY_EDITOR
        CloseUploaderFrame();
#endif
    }

    public static void OnOpenBimUploaderFrame(string _attachedProject, string _projectName, string _attachedTaskGroup, string _taskGroupName, string _attachedUser,string _actionTarget)
    {
        Debug.Log("UNITY: OnOpenBimUploaderFrame " + _attachedProject);
#if !UNITY_EDITOR
        OpenBimUploaderFrame(_attachedProject, _projectName, _attachedTaskGroup,_taskGroupName,_attachedUser,_actionTarget ) ;
#endif
    }

    public static void OnCloseBimUploaderFrame()
    {
        Debug.Log("UNITY: OnCloseBimUploaderFrame ");
#if !UNITY_EDITOR
        CloseUploaderFrame();
#endif
    }

    public static void OnOpenXMLUploaderFrame(string _item, string _user)
    {
        string item = _item;
        string user = _user;
        Debug.Log("UNITY: OpenXMLUploaderFrame " + item + " | " + user);
#if !UNITY_EDITOR
        OpenUploaderFrame(item,user);
#endif
    }


    public static void OnCloseXMLUploaderFrame()
    {
        Debug.Log("UNITY: CloseXMLUploaderFrame ");
#if !UNITY_EDITOR
        CloseUploaderFrame();
#endif
    }





    void Start()
    {
        OnCloseBimFrame();
    }
}
