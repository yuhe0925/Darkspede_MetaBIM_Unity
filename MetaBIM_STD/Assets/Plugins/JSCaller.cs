using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEditor;
using System;
using System.Net.NetworkInformation;

public class JSCaller : MonoBehaviour
{
    // ===============================================================================================================================
    // Calling external (JS) functions out side of Unity build
    // ===============================================================================================================================

    [DllImport("__Internal")]
    private static extern void HtmlToast(string msg);

    [DllImport("__Internal")]
    private static extern void EnablePointEvent();

    [DllImport("__Internal")]
    private static extern void DisablePointEvent();


    [DllImport("__Internal")]
    private static extern void UploadExternalFile(); 
    
    


    
    void Start()
    {
        //DisablePointEvent();
    }


    public static void OnHtmlToast(string _msg)
    {
        Debug.Log("UNITY: HtmlToast: " + _msg);
#if !UNITY_EDITOR
        HtmlToast(_msg);
#endif
    }

    public static void OnEnablePointEvent()
    {
        //Debug.Log("UNITY: EnablePointEvent: ");
#if !UNITY_EDITOR
        EnablePointEvent();
#endif
    }

    public static void OnDisablePointEvent()
    {
        //Debug.Log("UNITY: DisablePointEvent: ");
#if !UNITY_EDITOR
        DisablePointEvent();
#endif
    }




    /*       Uploading Actions: UploadModelVersion       */

    [DllImport("__Internal")]
    private static extern void External_UploadModelVersion(string _workspaceGuid, string _projectGuid, string _profileGuid, string _fileType, string _convertionAction);

    public static void RequestUploadModelVersion(string _workspaceGuid, string _projectGuid, string _profileGuid, string _fileType, string _convertionAction)
    {
        Debug.Log("UNITY.UploadModelVersion: " + _projectGuid);
#if !UNITY_EDITOR
        External_UploadModelVersion( _workspaceGuid,  _projectGuid,  _profileGuid, _fileType, _convertionAction);
#endif
    }


    [DllImport("__Internal")]
    private static extern void External_UploadFile(string _workspaceGuid, string _projectGuid, string _profileGuid, string _fileType);

    public static void RequestUploadFile(string _workspaceGuid, string _projectGuid, string _profileGuid, string _fileType)
    {
        Debug.Log("UNITY.RequestUploadFile: " + _projectGuid);
#if !UNITY_EDITOR
        External_UploadFile( _workspaceGuid,  _projectGuid,  _profileGuid, _fileType);
#endif
    }



    /*       Uploading Actions: UploadUserIcon       */


    /*       Uploading Actions: UploadSnapshot      */


    /*       Uploading Actions: Upload something      */



    /*       On Request Redirect URL                       */


    [DllImport("__Internal")]
    private static extern void External_OnRequestRedirectURL(string _msg = "");

    public static void OnRequestRedirectURL(string _msg = "")
    {
        Debug.Log("UNITY: OnRequestRedirectURL: " + _msg);
#if !UNITY_EDITOR
        External_OnRequestRedirectURL(_msg);
#endif
    }
}
