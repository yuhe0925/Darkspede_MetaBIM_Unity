using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using MetaBIM;
using JetBrains.Annotations;

public class OnJavascriptCallBack : MonoBehaviour
{
    public static OnJavascriptCallBack Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public static Action<bool, string> UploadCallback;




    public void UploadPostImageComplete_Callback(string result)
    {
        Debug.Log("OnUploadPostImageCompleteCallback: " + result);
        if (result != "false")
        {
            //string url = Darkspede.Sycamore.Config.Resouce_PostImage + "" + result;
            //Darkspede.Sycamore.Page_NewPost.Instance.UploadImageCallback(true, result);
        }
        else
        {
            //Darkspede.Sycamore.Page_NewPost.Instance.UploadImageCallback(false, result);
        }

    }

    public void UploadPostImageEditorComplete_Callback(string result)
    {
        Debug.Log("OnUploadPostImageEditorCompleteCallback: " + result);
        if (result != "false")
        {
            //string url = Darkspede.Sycamore.Config.Resouce_PostImage + "" + result;
            //Darkspede.Sycamore.Page_PostManage.Instance.UploadImageCallback(true, result);
        }
        else
        {
            //Darkspede.Sycamore.Page_PostManage.Instance.UploadImageCallback(false, result);
        }
    }

    public void UploadUserImageComplete_Callback(string result)
    {
        Debug.Log("OnUploadUserImageCompleteCallback: " + result);

        if (result != "false")
        {
            //string url = Darkspede.Sycamore.Config.Resouce_UserImage + "" + result;
            //Darkspede.Sycamore.Page_UserProfile.Instance.OnUploadUserIconCallback(true, result);
        }
        else
        {
            //Darkspede.Sycamore.Page_UserProfile.Instance.OnUploadUserIconCallback(false, result);
        }
    }


    //======================================================================================================
    //================ Model Operation =====================================================================
    //======================================================================================================

    public void UploadExternalFileComplete_Callback(string result)
    {
        Debug.Log("OnUploadExternalFileCompleteCallback: " + result);

        /*
        DataProxyResponse_Document DataProxyResponse_Document = DataProxyResponse_Document.FromJson(result);

        if (DataProxyResponse_Document.success == "true")
        {
            // Handle uploaded Files
            switch (DataProxyResponse_Document.package.documentType)
            {
                case "usericon":
                    Page_UserProfile.Instance.UploadChooseFileCallback("true");
                    break;
                case "bcf":

                    break;
                case "project":

                    break;
                case "xml":

                    break;
                case "assetbundle":

                    break;
                case "package":

                    break;
                default:
                    break;
            }
        }
        else
        {
            // Handle uploaded Files
            switch (DataProxyResponse_Document.package.documentType)
            {
                case "usericon":
                    Page_UserProfile.Instance.UploadChooseFileCallback("false");
                    break;
                case "bcf":

                    break;
                case "project":

                    break;
                case "xml":

                    break;
                case "assetbundle":

                    break;
                case "package":

                    break;
                default:
                    break;
            }
        }
        */

    }

    public void UploadFileComplete_Callback(string _result)
    {
        
    }

    public void UploadModelVersion_Callback(string _result)
    {
        JSCallBackPackage package = JSCallBackPackage.FromJson(_result);
        string convertionAction = package.messages.Find(x => x.key == "convertionAction").value;
   

        if (convertionAction != "")
        {
            // send result to project mananger (use to be workspace)
            Page_Workspace.Instance.OnUploadModel_CallBack(package);
        }
        else
        {
            Debug.Log("Call back with empty convertionAction");
            LoadingHandler.Instance.OnFullPageLoadingEnd();
        }
    }


    //=================================================================================================================
    //================ General Information Check Using JS =============================================================
    //=================================================================================================================


    public void CheckEndpoint_Callback(string _result)
    {
        JSCallBackPackage package = JSCallBackPackage.FromJson(_result);
        Debug.Log("OnCheckEndpoint: " + package.result);
    }

    public void RequestToClearCache_Callback(string _result)
    {
        JSCallBackPackage package = JSCallBackPackage.FromJson(_result);
        Debug.Log("OnRequestToClearCache_Callback: " + package.result);
    }


    public void OnRequestRedirectURL_Callback(string _result)
    {
        Debug.Log("OnRequestURL_Callback: " + _result);
        AppController.Instance.OnLoadRedirectLink_Callback(_result);
    }

}



[Serializable]
public class JSCallBackPackage{
    public bool result;
    public List<JSMessage> messages;


    public static JSCallBackPackage FromJson(string _json)
    {
        return JsonUtility.FromJson<JSCallBackPackage>(_json);
    }
}


[Serializable]
public class JSMessage
{
    public string key = "";
    public string value = "";
    public string condition = "";

    public JSMessage(string key, string value, string condition)
    {
        this.key = key;
        this.value = value;
        this.condition = condition;

    }

}
