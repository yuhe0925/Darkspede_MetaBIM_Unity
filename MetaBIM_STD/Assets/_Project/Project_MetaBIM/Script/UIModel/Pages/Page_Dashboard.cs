using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MetaBIM;
using UnityEngine.UI;
using System;
using System.ComponentModel;

public class Page_Dashboard : MonoBehaviour
{
    public static Page_Dashboard Instance;
    public PanelChange Panel;

    
    [Header("Page Configuration")]
    [Range(4, 100)]
    public int MaxWorkspaceNameCount = 4;
    public int MaxWorkspaceCount = 0;


    [Header("Data Buffer")]
    public bool IsLoadingData = false;
    

    #region Dashboard Insights

    [Header("Insights")]
    public TMP_InputField Text_Welcome;
    public TMP_InputField Text_ProjectNumber;
    public TMP_InputField Text_ModelNumber;

    public TMP_InputField Text_SystemVersion;
    public TMP_InputField Text_LastIP;
    public TMP_InputField Text_PlatformVersion;
    public TMP_InputField Text_MyTransactionToken;
    
    [Header("Workspaces")]
    public TMP_InputField Input_CreateWorkspaceName;
    public DashboardWorkspaceScrollView ScrollView_WorkspaceItems;

    
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

        if (!ScrollView_WorkspaceItems.IsInitialized)
        {
            ScrollView_WorkspaceItems.Init();
        }

        LoadingHandler.Instance.OnFullPageLoadingStart();
        LoadingHandler.Instance.OnFullPageLoadingSetProgress("Loading Projects");
        
        StartCoroutine(DataProxy.Instance.GetWorkspaces("createdBy", AppController.Instance.CurrentProfile.guid, OnGetOwnedWorkspace_Callback));

        // General Information
        Text_Welcome.text = string.Format("Hi {0}, \nWelcome to MataBIM! \nThe site is currently in beta, we are appreciate your patient and support :)", AppController.Instance.CurrentProfile.fullName);
        Text_PlatformVersion.text = "Platform: MetaBIM web";
        Text_LastIP.text = "Last IP: ***.***.***.107";  // Masked IP
        Text_SystemVersion.text = "Version: " + Application.version;
        Text_MyTransactionToken.text = Guid.NewGuid().ToString();



    }


    public void OnCloseAction()
    {
        IsLoadingData = false;

        // LoadingHandler.Instance.OnFullPageLoadingEnd();
        // Unload information?
    }

    // Start is called before the first frame update
    void Start()
    {

    }



    public void RenderInsights()
    {
        Text_ProjectNumber.text = DataSet.MyWorkspaces.Count.ToString();

        int modelCount = 0;
        for (int i = 0; i < DataSet.MyWorkspaces.Count; i++)
        {
            modelCount = modelCount + DataSet.MyWorkspaces[i].GetModelCount();
        }
        
        Text_ModelNumber.text = modelCount.ToString();

        Input_CreateWorkspaceName.text = "";
    }

    
    public void OnClick_ViewWorkspaces()
    {

    }

    public void OnClick_ViewAssets()
    {

    }

    public void OnClick_ReloadPage()
    {
        if (!IsLoadingData)
        {
            OnOpenAction();
        }
    }

    #endregion

    #region Workspaces


    

    public void OnClick_CreateWorksapce()
    {
        string name = Input_CreateWorkspaceName.text.Trim();
        // check for project import
        if (name == "")
        {
            MCPopup.Instance.SetWarning("Please enter a Project name");
            return;
        }

        // Check if the name is already been used locally.
        foreach (Workspace item in DataSet.MyWorkspaces)
        {
            if (item.workspaceName == name)
            {
                MCPopup.Instance.SetWarning("Project with same name already exists. Please use another name.");
                return;
            }
        }

        if(Input_CreateWorkspaceName.text.Length > MaxWorkspaceNameCount)
        {
            MCPopup.Instance.SetWarning("Project name length can not be longer than " + MaxWorkspaceNameCount + " characters.");
            return;
        }

        MCPopup.Instance.SetConfirm(CreateWorkspaceConfirm_Callback, Input_CreateWorkspaceName.text, "Creating new Project " + Input_CreateWorkspaceName.text);

    }


    public void CreateWorkspaceConfirm_Callback(bool _success, string _message)
    {
        if (_success)
        {
            Workspace newItem = new Workspace();
            newItem.workspaceName = _message;
            newItem.createdBy = AppController.Instance.CurrentProfile.guid;
            LoadingHandler.Instance.OnFullPageLoadingStart();
            LoadingHandler.Instance.OnFullPageLoadingSetProgress("Loading Projects");
            StartCoroutine(DataProxy.Instance.AddWorkspace(Workspace.ToJson(newItem), OnCreateWorkspace_Callback));
            
        }
        else
        {
            MCPopup.Instance.SetWarning("Add Project failed");
        }

    }


    public void OnCreateWorkspace_Callback(bool _success, string _message)
    {
        if (_success)
        {
            StartCoroutine(DataProxy.Instance.GetWorkspaces("createdBy", AppController.Instance.CurrentProfile.guid, OnGetOwnedWorkspace_Callback));
        }
        else
        {
            MCPopup.Instance.SetWarning("New Project Cancelled");
        }
    }


    public void OnGetOwnedWorkspace_Callback(bool _success, string _message)
    {
        IsLoadingData = false;
        LoadingHandler.Instance.OnFullPageLoadingEnd();

        if (_success)
        {
            DataProxyResponse<Workspace> payload = JsonUtility.FromJson<DataProxyResponse<Workspace>>(_message);

  
            
            if (payload.result)
            {
                AppController.Instance.LoadedWorkspaces = DataSet.MyWorkspaces;
                DataSet.MyWorkspaces = payload.package;  //default sort
                DataSet.MyTransactions.Sort((x, y) => y.updated.CompareTo(x.updated));  //default sort

                RenderInsights();
                RenderWorkspace();
                //RenderTransaction();
                
            }
            else
            {
                MCPopup.Instance.SetWarning(payload.message);
            }

        }
        else
        {
            MCPopup.Instance.SetWarning("Refresh Project failed");
        }
    }


    public void RenderWorkspace()
    {
        DataSet.MyWorkspaces.Sort((x, y) => y.updated.CompareTo(x.updated));
        StartCoroutine(UpdateScrollView());
        //ScrollView_WorkspaceItems.SetItems(DataSet.MyWorkspaces);
    }

    public IEnumerator UpdateScrollView()
    {
        yield return new WaitForSeconds(0.1f);
        ScrollView_WorkspaceItems.SetItems(DataSet.MyWorkspaces);
    }

    
    public void OnClick_SelectWorkspace(GameObject _gameObject)
    {
        //Debug.Log("OnClick_SelectWorkspace: " + _gameObject.name);
        Workspace worksapce = _gameObject.GetComponent<UIBlock_Dashboard_WorkspaceItem>().Item;
        AppController.Instance.SelectedWorkspaceGuid = worksapce.guid;
        MCPopup.Instance.SetConfirm(OnSelectWorkspaceConfirm_Callback, "", "Open Project  " + worksapce.workspaceName);
            
    }

    public void OnSelectWorkspaceConfirm_Callback(bool _success, string _message)
    {
        if (_success)
        {
            AppController.Instance.SetPage(AppController.PageIndex.workspace);
        }
        else
        {
            AppController.Instance.SelectedWorkspaceGuid = "";
        }
    }


    //sorting priority
    // premission => update => create => name =>


    // Dynamic Event Call

    public void OnClick_SortBy(UIController_SortToggle _toggle)
    {
        Debug.Log("OnClick_SortBy: " + _toggle.Key);

        switch (_toggle.Key)
        {
            case DataSet.SortGroup.name:
                if (_toggle.isSortAscent)
                {
                    DataSet.MyWorkspaces.Sort((x, y) => x.workspaceName.CompareTo(y.workspaceName));
                }
                else
                {
                    DataSet.MyWorkspaces.Sort((y, x) => x.workspaceName.CompareTo(y.workspaceName));
                }
                break;
            case DataSet.SortGroup.update:
                if (_toggle.isSortAscent)
                {
                    DataSet.MyWorkspaces.Sort((x, y) => x.updated.CompareTo(y.updated));
                }
                else
                {
                    DataSet.MyWorkspaces.Sort((y, x) => x.updated.CompareTo(y.updated));
                }
                break;

            /*
            case DataSet.SortGroup.permission:
                // Need to sort by permission
                break;
            */

            default:
                if (_toggle.isSortAscent)
                {
                    DataSet.MyWorkspaces.Sort((x, y) => x.updated.CompareTo(y.updated));
                }
                else
                {
                    DataSet.MyWorkspaces.Sort((y, x) => x.updated.CompareTo(y.updated));
                }
                break;
        }



        //Render sorted list
        RenderWorkspace();
    }



    
    public void OnClick_AddExampleWorkspace()
    {
        MCPopup.Instance.SetConfirm(OnAddExampleWorkspaceConfirm_Callback, "", "An Example Project will be added to your account.");
    }
    

    public void OnAddExampleWorkspaceConfirm_Callback(bool _success, string _message)
    {
        LoadingHandler.Instance.OnFullPageLoadingEnd();

        if (_success)
        {
           
            StartCoroutine(DataProxy.Instance.OnRequestExampleWorkspace(OnAddExampleWorkspace_Callback));
        }
        else
        {
    
        }
    }


    public void OnAddExampleWorkspace_Callback(bool _success, string _message)
    {
        LoadingHandler.Instance.OnFullPageLoadingEnd();
        if (_success)
        {
            DataProxyResponse<IModel> payload = DataProxyResponse<IModel>.FromJson(_message);

            if (payload.result)
            {
                OnOpenAction();   
            }
            else
            {
                MCPopup.Instance.SetWarning(payload.message);            
            }
        }
        else
        {
            MCPopup.Instance.SetWarning("Network Error");
        }
    }

    #endregion

    #region Recent Transaction

    [Header("Recent Transaction")]
    public TMP_InputField Input_Transaction;

    public GameObject TransactionItemTemplate;
    public List<GameObject> TransactionItems;
    public Transform TransactionParent;

    public int MaxTransactionDisplayed;



    public void RenderTransaction() 
    {

    }

    public void OnClick_TransactionItem()
    {

    }


    #endregion

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
