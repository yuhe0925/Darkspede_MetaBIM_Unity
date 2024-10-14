using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MetaBIM;
using UnityEngine.UI;
using System;

public class Page_Admin : MonoBehaviour
{
    public static Page_Admin Instance;
    public PanelChange Panel;
 
    
    [Header("Data Buffer")]
    public bool IsLoadingData = false;

    public List<Request> RequestItems;
    public List<Profile> ProfileItems;



    [Header("UI Element")]
    public AdminRequestItemScrollView ScrollView_RequestItem;


    [Header("Insights")]
    public TextMeshProUGUI Text_RequestNumber;
    public TextMeshProUGUI Text_LastUpdated;

    
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

        
        // Checl Permission

        if (AppController.Instance.CurrentProfile == null || AppController.Instance.CurrentProfile.guid == "")
        {
            goto Next;
        }

        if (AppController.Instance.CurrentProfile.profileRole.roleType != "admin")
        {
            goto Next;
        }

        if (AppController.Instance.CurrentProfile.profileRole.status != "active")
        {
            goto Next;
        }
        
        Text_LastUpdated.text = "Last Updated: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        RequestItems = new List<Request>();
        OnLoadRequest();
        return;

        Next:
        AppController.Instance.SetPage(AppController.PageIndex.dashboard);

    }

    
    public void OnCloseAction()
    {
        IsLoadingData = false;

        //LoadingHandler.Instance.OnFullPageLoadingEnd();
        // Unload information?
    }

    
    public void OnLoadRequest()
    {
        StartCoroutine(DataProxy.Instance.GetRequests("", "",OnLoadRequest_Callback));
    }


    public void OnLoadRequest_Callback(bool _result, string _message)
    {
        if (_result)
        {
            DataProxyResponse<Request> payload = JsonUtility.FromJson<DataProxyResponse<Request>>(_message);

            if (payload.result)
            {
                RequestItems = payload.package;
                RequestItems.Sort((x, y) => y.requestStatus.CompareTo(x.requestStatus));
                StartCoroutine(UpdateScrollView());
                RenderInsight();
            }
            else
            {
                MCPopup.Instance.SetWarning(payload.message);
            }
        }
        else
        {
            MCPopup.Instance.SetWarning(_message);
        }
        
    }

   

    // This methed is to be call via Invoke
    public IEnumerator UpdateScrollView()
    {
        yield return new WaitForSeconds(0.05f);
        Debug.Log("UpdateScrollView: " + RequestItems.Count);
        ScrollView_RequestItem.SetItems(RequestItems);
    }


    

    public void RenderInsight()
    {
        int pending=0;
        int accpet=0;
        
        foreach(Request item in RequestItems)
        {
            if (item.requestStatus == "pending")
            {
                pending++;
            }else 
            if (item.requestStatus == "accept")
            {
                accpet++;
            }
        }

        Text_RequestNumber.text = pending + " / " + accpet + " / " + RequestItems.Count;


    }





        

    public void OnClick_SelectProjectItem(GameObject _gameObject)
    {
        MetaBIM.Project project = _gameObject.GetComponent<UIBlock_Workspace_ProjectItem>().Item;
        AppController.Instance.SelectedProjectGuid = project.guid;
        //MCPopup.Instance.SetConfirm(OnSelectProjectConfirm_Callback, "", "Open project  " + project.projectName + "?");

        MCPopup.Instance.SetConfirm(OnSelectProjectConfirm_Callback, "", StringBuffer.Admin_Message_OpenProject.S + project.projectName + "?");

    }

    public void OnSelectProjectConfirm_Callback(bool _success, string _message)
    {
        if (_success)
        {
            AppController.Instance.SetPage(AppController.PageIndex.project);
        }
        else
        {
            AppController.Instance.SelectedProjectGuid = "";
        }

    }


    public void OnClick_SortBy(UIController_SortToggle _toggle)
    {
        Debug.Log("OnClick_SortBy: " + _toggle.Key);

        switch (_toggle.Key)
        {
            case DataSet.SortGroup.name:
                if (_toggle.isSortAscent)
                {
                    RequestItems.Sort((x, y) => x.requestStatus.CompareTo(y.requestStatus));
                }
                else
                {
                    RequestItems.Sort((y, x) => x.requestStatus.CompareTo(y.requestStatus));
                }
                break;
            case DataSet.SortGroup.update:
                if (_toggle.isSortAscent)
                {
                    RequestItems.Sort((x, y) => x.updated.CompareTo(y.updated));
                }
                else
                {
                    RequestItems.Sort((y, x) => x.updated.CompareTo(y.updated));
                }
                break;

            default:
                if (_toggle.isSortAscent)
                {
                    RequestItems.Sort((x, y) => x.updated.CompareTo(y.updated));
                }
                else
                {
                    RequestItems.Sort((y, x) => x.updated.CompareTo(y.updated));
                }
                break;
        }
        
        //Render sorted list
        StartCoroutine(UpdateScrollView());
    }




    public void OnClick_Action_AccpetRequestAsAdmin(GameObject _gameObject)
    {
        Request request = _gameObject.GetComponent<UIBlock_Admin_RequestItem>().Item;

        MCPopup.Instance.SetConfirm(OnProcessRequestAsAdminConfirm_Callback, request.guid, "Accept request " + request.requestMobile + " as Admin?");
    }

    public void OnClick_Action_AccpetRequestAsClient(GameObject _gameObject)
    {
        Request request = _gameObject.GetComponent<UIBlock_Admin_RequestItem>().Item;
        MCPopup.Instance.SetConfirm(OnProcessRequestAsAdminConfirm_Callback, request.guid, "Accept request " + request.requestMobile + " as Client?");
    }


    public void OnProcessRequestAsAdminConfirm_Callback(bool _success, string _message)
    {
        if (_success)
        {
            StartCoroutine(DataProxy.Instance.OnProcessRequest(_message, "accept", "admin", OnProcessRequest_Callback));
        }
    }

    public void OnProcessRequestAsClientConfirm_Callback(bool _success, string _message)
    {
        if (_success)
        {
            StartCoroutine(DataProxy.Instance.OnProcessRequest(_message, "accept", "client", OnProcessRequest_Callback));
        }
    }


    
    public void OnProcessRequest_Callback(bool _success, string _message)
    {
        Debug.Log("OnProcessRequest_Callback: ");
        Debug.Log(_message);
        
        if (_success)
        {
            DataProxyResponse<Request> payload = JsonUtility.FromJson<DataProxyResponse<Request>>(_message);


            if (payload.result)
            {
                OnLoadRequest();
            }
            else
            {
                MCPopup.Instance.SetWarning(payload.message);
            }
        }
        else
        {
            MCPopup.Instance.SetWarning(_message);
        }

    }


    public void OnClick_Action_IgnoreRequest(GameObject _gameObject)
    {

    }

    public void OnClick_Action_RevokeRequest(GameObject _gameObject)
    {

    }




    public void OnClick_Action_ProfileSetAdmin()
    {

    }

    public void OnClick_Action_ProfileMoreAdmin()
    {
        
    }
    

    #region Utility


    #endregion


    #region MISC


    public void ClearObjectList(List<GameObject> _list, bool isBlockDestroy = false)
    {
        if(_list.Count > 0)
        {
            foreach(GameObject item in _list)
            {
                if (!isBlockDestroy)
                {
                    Destroy(item);
                }
                else
                {
                    // TODO
   
                }
            }
        }

        _list.Clear();
    }


    #endregion
}
