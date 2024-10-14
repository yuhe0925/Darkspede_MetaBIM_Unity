using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MetaBIM;
using UnityEngine.UI;
using System;
using System.Linq;


public class Page_Workspace : MonoBehaviour
{
    public static Page_Workspace Instance;
    public PanelChange Panel;



    [Header("Data Buffer")]
    public bool IsLoadingData = false;
    public int LoadProjectStatus = 0;
    public Workspace SelectedWorkspace;


    [Header("Create New Project")]
    public TMP_InputField Input_CreateProjectName;
    public int MaxProjectNameCount = 20;
    //public WorkspaceProjectItemScrollView ScrollView_ProjectItem;


    [Header("Insights")]
    public TextMeshProUGUI Text_WorkspaceName;
    public TextMeshProUGUI Text_LastUpdate;
    public TextMeshProUGUI Text_ProjectNumbers;
    public TextMeshProUGUI Text_ModelNumber;
    public TextMeshProUGUI Text_DataUsage;


    [Header("UI Elements")]
    public GameObject ArchiveModelButtonObject;
    public GameObject UnarchiveModelButtonObject;

    // Tab Select Widget
    public string CurrentTabFileName = "ifc";
    public string CurrentTabName;

    [Header("Preset Objects")]
    public List<GameObject> PresetObjects;

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
        ProjectModelItemPrefab.SetActive(false);
        ProjectModelVersionItemPrefab.SetActive(false);
        ProjectModelUpdateItemPrefab.SetActive(false);
        
        ClearPresetObjects();
        
        Init(DataSet.MyWorkspaces.Find(x => x.guid == AppController.Instance.SelectedWorkspaceGuid));
    }


    public void OnCloseAction()
    {
        IsLoadingData = false;

        // LoadingHandler.Instance.OnFullPageLoadingEnd();
        // Unload information?
    }


    public void Init(Workspace _workspace)
    {
        // find workspace by guid in a list
        if (_workspace == null)
        {
            return;
        }

        SelectedWorkspace = _workspace;
        //AppController.Instance.SelectedProjectGuid = "";

        if (SelectedWorkspace == null)
        {
            AppController.Instance.SelectedWorkspaceGuid = "";
            AppController.Instance.SetPage(AppController.PageIndex.dashboard);
        }
        else
        {
            // Scroll View not load on fast started, set a delay to wait for scroll view init
            OnRenderProjectModels();
            RenderInsight();

            // setup data handler
        }
    }




    [Header("Project List")]
    [Header("-----------------")]
    public GameObject ProjectModelItemPrefab;
    public Transform ProjectModelItemParent;
    public List<GameObject> GameObjectProjectModelItems;
    public GameObject SelectedModelItemObject;

    private GameObject FindProjectExsit(string _projectGuid)
    {

        foreach (GameObject obj in GameObjectProjectModelItems)
        {
            UIBlock_Project_ModelItem item = obj.GetComponent<UIBlock_Project_ModelItem>();
            if (item.Item.guid == _projectGuid)
            {
                return obj;
            }
        }

        return null;
    }

    private void OnClearModelItemList()
    {
        if (GameObjectProjectModelItems == null)
        {
            GameObjectProjectModelItems = new List<GameObject>();
            return;
        }

        if (GameObjectProjectModelItems.Count > 0)
        {
            foreach (GameObject obj in GameObjectProjectModelItems)
            {
                obj.SetActive(false);
            }
        }
    }

    public void OnRenderProjectModels()
    {
        Debug.Log("OnRenderProjectModels");

        OnClearModelItemList();

        foreach (MetaBIM.Project project in SelectedWorkspace.projects)
        {
            if (project.projectStatus == LoadProjectStatus)
            {
                GameObject target = FindProjectExsit(project.guid);

                if (target != null)
                {
                    UIBlock_Project_ModelItem modelItem = target.GetComponent<UIBlock_Project_ModelItem>();
                    modelItem.SetBlock(project, ProjectModelHandler.Instance.CheckVersionStatus(modelItem));
                    target.SetActive(true);
                }
                else
                {
                    target = Instantiate(ProjectModelItemPrefab, ProjectModelItemParent);
                    target.SetActive(true);
                    UIBlock_Project_ModelItem item = target.GetComponent<UIBlock_Project_ModelItem>();
                    item.SetBlock(project);
                    item.OnSelected();
                    GameObjectProjectModelItems.Add(target);

                    // add to data handler
                    ProjectModelHandler.Instance.AddNewModelToHandle(item);

                }
            }
        }

        SortProjectModels();

        // re select the items if was selected before
        if (SelectedModelItemObject != null)
        {
            OnClick_SelectModelItem(SelectedModelItemObject);
        }
    }

    public void SortProjectModels()
    {
        //GameObjectProjectModelItems.Sort((x, y) =>x.GetComponent<UIBlock_Project_ModelItem>().Item.updated.CompareTo(y.GetComponent<UIBlock_Project_ModelItem>().Item.updated));
    }


    public void OnClick_SelectModelItem(GameObject _selecetedObject)
    {
        if (SelectedModelItemObject != null)
        {
            SelectedModelItemObject.GetComponent<UIBlock_Project_ModelItem>().OnDeselected();
        }

        SelectedModelItemObject = _selecetedObject;
        UIBlock_Project_ModelItem item = SelectedModelItemObject.GetComponent<UIBlock_Project_ModelItem>();
        item.OnSelected();
        AppController.Instance.SelectedProjectGuid = item.Item.guid;
        // OnRenderProjectVersion(item.Item.versions);

        OnFileTabSelect("ifc");

    }

    public void OnValueChange_DropdownVersionItem(Project _project, int _valueIndex)
    {
        if (GameObjectProjectModelVersionItems.Count > _valueIndex)
        {
            OnClick_SelectVersionItem(GameObjectProjectModelVersionItems[_valueIndex]);
        }
    }

    public void OnToggle_ModelToggled(UIBlock_Project_ModelItem _item)
    {
        // Enable this block to set the model selection for single selection

        /*
        foreach (GameObject obj in GameObjectProjectModelItems)
        {
            UIBlock_Project_ModelItem item = obj.GetComponent<UIBlock_Project_ModelItem>();
            
            
            if(item.Item.guid != _item.Item.guid)
            {
                item.Toggle_Selected.OnReset(); 
            }
        }
       */
    }




    [Header("Version List")]
    [Header("-----------------")]
    public GameObject ProjectModelVersionItemPrefab;
    public Transform ProjectModelVersionItemParent;
    public List<GameObject> GameObjectProjectModelVersionItems;
    public TextMeshProUGUI Text_VersionItemHeader;
    public GameObject VersionItemPlaceHolder;
    public GameObject SelectedVersionItemObject;

    private void OnClearModelVersionItemList()
    {
        if (GameObjectProjectModelVersionItems == null)
        {
            GameObjectProjectModelVersionItems = new List<GameObject>();
            return;
        }


        if (GameObjectProjectModelVersionItems.Count > 0)
        {
            foreach (GameObject obj in GameObjectProjectModelVersionItems)
            {
                Destroy(obj);
            }

            GameObjectProjectModelVersionItems.Clear();
        }
    }


    public void OnRenderProjectVersion(List<MetaBIM.Version> _versionList)
    {
        OnClearModelVersionItemList();
        _versionList.Sort((y, x) => x.updated.CompareTo(y.updated));


        foreach (MetaBIM.Version version in _versionList)
        {
            if (version.versionStatus == LoadProjectStatus)
            {
                GameObject target = Instantiate(ProjectModelVersionItemPrefab, ProjectModelVersionItemParent);
                target.SetActive(true);
                target.GetComponent<UIBlock_Project_ModelVersionItem>().SetBlock(version);

                GameObjectProjectModelVersionItems.Add(target);
            }
        }


        if (GameObjectProjectModelVersionItems.Count == 0)
        {
            VersionItemPlaceHolder.SetActive(true);

            // clear version item
            if (SelectedVersionItemObject != null)
            {
                SelectedVersionItemObject.GetComponent<UIBlock_Project_ModelVersionItem>().OnDeselected();
                SelectedVersionItemObject = null;
            }

            // clear update item
            if (SelectedUpdateItemObject != null)
            {
                SelectedUpdateItemObject.GetComponent<UIBlock_Project_ModelUpdateItem>().OnDeselected();
                SelectedUpdateItemObject = null;
            }

        }
        else
        {
            OnClick_SelectVersionItem(GameObjectProjectModelVersionItems[0]);
            VersionItemPlaceHolder.SetActive(false);
        }


    }



    public void OnClick_SelectVersionItem(GameObject _selecetedObject)
    {
        if (SelectedVersionItemObject != null)
        {
            SelectedVersionItemObject.GetComponent<UIBlock_Project_ModelVersionItem>().OnDeselected();
        }

        SelectedVersionItemObject = _selecetedObject;
        UIBlock_Project_ModelVersionItem item = SelectedVersionItemObject.GetComponent<UIBlock_Project_ModelVersionItem>();
        item.OnSelected();


        OnTabSelect();
    }


    [Header("Update List")]
    [Header("-----------------")]
    public GameObject ProjectModelUpdateItemPrefab;
    public Transform ProjectModelUpdateItemParent;
    public List<GameObject> GameObjectProjectModelUpdateItems;
    public TextMeshProUGUI Text_UpdateItemHeader;
    public GameObject UpdateItemPlaceHolder;
    public GameObject SelectedUpdateItemObject;
    private void OnClearModelUpdateItemList()
    {
        if (GameObjectProjectModelUpdateItems == null)
        {
            GameObjectProjectModelUpdateItems = new List<GameObject>();
            return;
        }

        if (GameObjectProjectModelUpdateItems.Count > 0)
        {
            foreach (GameObject obj in GameObjectProjectModelUpdateItems)
            {
                Destroy(obj);
            }

            GameObjectProjectModelUpdateItems.Clear();

        }
    }


    public void OnRenderProjectModelUpdates(MetaBIM.Version _version)
    {
        OnClearModelUpdateItemList();

        string projectGuild = SelectedModelItemObject.GetComponent<UIBlock_Project_ModelItem>().Item.guid;
        ModelVersion mv = ProjectModelHandler.Instance.GetModelVersion(projectGuild);


        if (mv.VersionUpdateStatus != 2)
        {
            UpdateItemPlaceHolder.SetActive(true);
            return;
        }
        else
        {
            UpdateItemPlaceHolder.SetActive(false);
        }

        

        //_version.updates.Sort((y, x) => x.updated.CompareTo(y.updated));


        foreach (VersionUpdate updates in mv.VersionUpdates)
        {
            if (updates.attachedVersion == _version.guid)
            {
                GameObject target = Instantiate(ProjectModelUpdateItemPrefab, ProjectModelUpdateItemParent);
                target.SetActive(true);
                string versionGuild = SelectedVersionItemObject.GetComponent<UIBlock_Project_ModelVersionItem>().Item.guid;

                target.GetComponent<UIBlock_Project_ModelUpdateItem>().SetBlock(updates, projectGuild, versionGuild);
                GameObjectProjectModelUpdateItems.Add(target);
            }
        }

        if (GameObjectProjectModelUpdateItems.Count > 0)
        {
            OnClick_SelectUpdateItem(GameObjectProjectModelUpdateItems[0]);
        }
        else
        {
            if (SelectedUpdateItemObject != null)
            {
                SelectedUpdateItemObject.GetComponent<UIBlock_Project_ModelUpdateItem>().OnDeselected();
                SelectedUpdateItemObject = null;
            }
        }
        

    }


    public void OnClick_SelectUpdateItem(GameObject _selecetedObject)
    {
        if (SelectedUpdateItemObject != null)
        {
            SelectedUpdateItemObject.GetComponent<UIBlock_Project_ModelUpdateItem>().OnDeselected();
        }

        SelectedUpdateItemObject = _selecetedObject;
        UIBlock_Project_ModelUpdateItem item = SelectedUpdateItemObject.GetComponent<UIBlock_Project_ModelUpdateItem>();
        item.OnSelected();

    }

    public void OnClick_UpdateItem_Merge(GameObject _updateObject)
    {
        MCPopup.Instance.SetInformation("Merge is not available in the version");
    }


    public void OnClick_UpdateItem_Build(GameObject _updateObject)
    {

        MCPopup.Instance.SetConfirm(UpdateItem_Build_ConfirmCallBack, "", "Request to rebuild the model?");
    }


    public void UpdateItem_Build_ConfirmCallBack(bool _result, string _message)
    {
        if (_result)
        {
            UIBlock_Project_ModelUpdateItem item = SelectedUpdateItemObject.GetComponent<UIBlock_Project_ModelUpdateItem>();
            ProjectModelHandler.Instance.OnModelRebuild(item.attachedProject, item.attachedVersion, UpdateItem_Build_CallBack);

            MCPopup.Instance.SetInformation("Rebuild the model, please wait the process to complete.");
        }
    }


    public void UpdateItem_Build_CallBack(bool _result, string _message)
    {
        if (_result)
        {
            OnClick_RefreshWorkspace();
        }
    }



    public void OnClick_UpdateItem_Reject(GameObject _updateObject)
    {
        MCPopup.Instance.SetInformation("Request update reject failed");
    }

    public void OnClick_UpdateItem_Load(GameObject _updateObject)
    {
        UIBlock_Project_ModelUpdateItem item = SelectedUpdateItemObject.GetComponent<UIBlock_Project_ModelUpdateItem>();
        //bool result = ProjectModelHandler.Instance.OnModelDownload(item.attachedProject, item.attachedVersion, item.Item.UpdateID);
        bool result = ProjectModelHandler.Instance.OnModelDownload(item.attachedProject, item.attachedVersion, item.Item);
        if (result)
        {

        }
        else
        {

        }
    }

    public void OnClick_UpdateItem_Clear(GameObject _updateObject)
    {
        UIBlock_Project_ModelUpdateItem item = SelectedUpdateItemObject.GetComponent<UIBlock_Project_ModelUpdateItem>();

        bool result = ProjectModelHandler.Instance.OnCheckModelForClear(item.attachedProject, item.attachedVersion);

        if (result)
        {
            MCPopup.Instance.SetConfirm(UpdateItem_Clear_ConfirmCallBack,"", "Unload this model?");
        }
        else
        {

        }
    }

    public void UpdateItem_Clear_ConfirmCallBack(bool _result, string _message) 
    {
        if (_result)
        {
            UIBlock_Project_ModelUpdateItem item = SelectedUpdateItemObject.GetComponent<UIBlock_Project_ModelUpdateItem>();
            ProjectModelHandler.Instance.OnModelClear(item.attachedProject, item.attachedVersion);
        }
    }


    public void OnClick_UpdateItem_Reset(GameObject _updateObject)
    {
        UIBlock_Project_ModelUpdateItem item = SelectedUpdateItemObject.GetComponent<UIBlock_Project_ModelUpdateItem>();
        MCPopup.Instance.SetConfirm(UpdateItem_Reset_ConfirmCallBack, "", "Reset this model and process it again?");
    }

    public void UpdateItem_Reset_ConfirmCallBack(bool _result, string _message)
    {
        if (_result)
        {
            UIBlock_Project_ModelUpdateItem item = SelectedUpdateItemObject.GetComponent<UIBlock_Project_ModelUpdateItem>();

        

        }
    }

    // =============================
    // General Action
    // =============================

    public void OnClick_RefreshWorkspace()
    {
        LoadingHandler.Instance.OnFullPageLoadingStart("Loading Projects");
        //StartCoroutine(DataProxy.Instance.GetWorkspaces("createdBy", AppController.Instance.CurrentProfile.guid, RefreshWorkspace_Callback));
        StartCoroutine(DataProxy.Instance.GetWorkspaceByGuid(SelectedWorkspace.guid, RefreshWorkspace_Callback));
    }

    public void RefreshWorkspace_Callback(bool _result, string _message)
    {
        LoadingHandler.Instance.OnFullPageLoadingEnd();

        if (_result)
        {
            DataProxyResponse<Workspace> payload = JsonUtility.FromJson<DataProxyResponse<Workspace>>(_message);


            if (payload.result)
            {
                Init(payload.package[0]);
            }
            else
            {
                AppController.Instance.SelectedWorkspaceGuid = "";
                AppController.Instance.SetPage(AppController.PageIndex.dashboard);
                SelectedWorkspace = null;

                MCPopup.Instance.SetWarning(payload.message);  // not good
            }

        }
        else
        {
            AppController.Instance.SelectedWorkspaceGuid = "";
            AppController.Instance.SetPage(AppController.PageIndex.dashboard);
            SelectedWorkspace = null;

            MCPopup.Instance.SetWarning("Refresh failed");   // not good
        }
    }

    public void RenderInsight()
    {

        Text_WorkspaceName.text = SelectedWorkspace.workspaceName;
        Text_LastUpdate.text = "Last Update: " + Utility.TimeFromTick(SelectedWorkspace.updated);
        Text_ProjectNumbers.text = SelectedWorkspace.projects.Count.ToString();
        Text_ModelNumber.text = SelectedWorkspace.GetModelCount().ToString();

        int usage = SelectedWorkspace.GetWorkspaceDataUsage();

        Text_DataUsage.text = Utility.FormatFileSize(usage);

        Input_CreateProjectName.text = "";
    }

    public void OnClick_CreateProject()
    {
        string name = Input_CreateProjectName.text.Trim();

        // check for project import
        if (name == "")
        {
            MCPopup.Instance.SetWarning("Please enter a project name");
            return;
        }

        // Check if the name is already been used locally.
        foreach (MetaBIM.Project item in SelectedWorkspace.projects)
        {
            Debug.Log("Cheching Project Names:" + item.projectName + " | " + name);

            if (item.projectName == name)
            {
                MCPopup.Instance.SetWarning("Project with same name already exists. Please use another name.");
                return;
            }
        }

        if (Input_CreateProjectName.text.Length > MaxProjectNameCount)
        {
            MCPopup.Instance.SetWarning("Project name length can not be longer than " + MaxProjectNameCount + " characters.");
            return;
        }


        // TODO, add condition of numbers of project in this workspace (limit for each account)

        MCPopup.Instance.SetConfirm(CreateProjectConfirm_Callback, Input_CreateProjectName.text, "Creating new project " + Input_CreateProjectName.text);

    }

    public void CreateProjectConfirm_Callback(bool _result, string _message)
    {
        if (_result)
        {
            MetaBIM.Project newProject = new MetaBIM.Project();
            newProject.projectName = _message;
            newProject.createdBy = SelectedWorkspace.createdBy;
            SelectedWorkspace.projects.Add(newProject);
            LoadingHandler.Instance.OnFullPageLoadingStart("Creating Project");
            StartCoroutine(DataProxy.Instance.UpdateWorkspace(Workspace.ToJson(SelectedWorkspace), OnCreateProject_Callback));
        }
        else
        {
            MCPopup.Instance.SetWarning("Add project failed");
        }
    }

    public void OnCreateProject_Callback(bool _result, string _message)
    {
        Debug.Log("OnCreateProject_Callback: ");
        Debug.Log(_message);
        LoadingHandler.Instance.OnFullPageLoadingEnd();

        if (_result)
        {
            OnClick_RefreshWorkspace();
        }
        else
        {
            MetaBIM.Project removePorject = SelectedWorkspace.projects.Find(x => x.projectName == Input_CreateProjectName.text);
            if (removePorject != null)
            {
                SelectedWorkspace.projects.Remove(removePorject);
            }
            MCPopup.Instance.SetWarning("Add project failed");
        }
    }

    public void OnClick_ViewUnarchivedModels()
    {
        LoadProjectStatus = 0;
        Init(SelectedWorkspace);
    }

    public void OnClick_ViewArchivedModels()
    {
        LoadProjectStatus = 4;
        Init(SelectedWorkspace);
    }


    public void OnClick_OpenModel()
    {
        if (SelectedModelItemObject == null)
        {
            MCPopup.Instance.SetWarning(StringBuffer.ProjectManage_NoProjectSelected.S);
            return;
        }

        // select all the toggled model
        int opened = 0;

        foreach (ModelVersion mv in ProjectModelHandler.Instance.modelVersions)
        {
            if (mv.Viewer.IsToggled())
            {
                if (mv.loadingStatus == ModelVersion.ModelLoadingStatus.loaded)
                {
                    ProjectModelHandler.Instance.CurrentModel = mv;
                    opened++;

                }
                else
                {
                    MCPopup.Instance.SetInformation("One or more selected model is still loading, please wait for it to complete or select to open loaded models.");
                    return;
                }
            }
        }

        if (opened == 0)
        {
            MCPopup.Instance.SetInformation("Please select a loaded model to open");
        }
        else
        {
            MCPopup.Instance.SetConfirm(OnOpenModelConfirmed, opened.ToString(), "Opening " + opened.ToString() + " model(s)");
        }
    }




    public void OnOpenModelConfirmed(bool _result, string _message)
    {
        if (_result)
        {
            Debug.Log("Start loading: (Replace with JS loading screen)");
            // this is the way for single model selection
            foreach (ModelVersion mv in ProjectModelHandler.Instance.modelVersions)
            {
                if (mv.Viewer.IsToggled())
                {
                    if (mv.loadingStatus == ModelVersion.ModelLoadingStatus.loaded)
                    {
                        mv.EnableModel(); // this take some time
                    }
                }
            }

            //Debug.Log("End loading: ");

            AppController.Instance.SetPage(AppController.PageIndex.bimview);
            Page_BIMViewer.Instance.SelectedVersion = SelectedVersionItemObject.GetComponent<UIBlock_Project_ModelVersionItem>().Item;
            Page_BIMViewer.Instance.OnLoadBimObjectsIntoSceneFromhandler();
            // change to multiply model selection here , TODO
        }
        else
        {

        }

    }


    public void OnClick_ArchiveModel()
    {
        if (SelectedModelItemObject == null)
        {
            MCPopup.Instance.SetWarning(StringBuffer.ProjectManage_NoProjectSelected.S);
            return;
        }

        UIBlock_Project_ModelItem item = SelectedModelItemObject.GetComponent<UIBlock_Project_ModelItem>();

        // TODO, set project status
        item.Item.projectStatus = 4;

        SelectedModelItemObject = null;

        StartCoroutine(DataProxy.Instance.UpdateWorkspace(Workspace.ToJson(SelectedWorkspace), OnCreateProject_Callback));
    }


    public void OnClick_UnarchiveModel()
    {
        if (SelectedModelItemObject == null)
        {
            MCPopup.Instance.SetWarning(StringBuffer.ProjectManage_NoProjectSelected.S);
            return;
        }

        UIBlock_Project_ModelItem item = SelectedModelItemObject.GetComponent<UIBlock_Project_ModelItem>();

        // TODO, set project status
        item.Item.projectStatus = 0;

        SelectedModelItemObject = null;

        StartCoroutine(DataProxy.Instance.UpdateWorkspace(Workspace.ToJson(SelectedWorkspace), OnCreateProject_Callback));

    }


    public void OnClick_UploadNewModel()
    {
        if (SelectedModelItemObject == null)
        {
            MCPopup.Instance.SetWarning(StringBuffer.ProjectManage_NoProjectSelected.S);
            return;
        }

        if (AppController.Instance.CurrentProfile == null || AppController.Instance.SelectedWorkspaceGuid == "" || AppController.Instance.SelectedProjectGuid == "")
        {
            MCPopup.Instance.SetWarning(StringBuffer.ProjectManage_NoProjectSelected.S);
        }
        else
        {

            switch (CurrentTabFileName)
            {
                case "ifc":
                    MCPopup.Instance.SetConfirm(OnUploadModelConfirm, CurrentTabFileName, "Uploading a new IFC model, size limit 200MB", "Upload IFC");
                    break;
                case "image":
                    //MCPopup.Instance.SetConfirm(OnUploadModelConfirm, CurrentTabFileName, "Uploading a new IFC model, size limit 200MB", "Upload IFC");
                    break;
                case "document":
                    //MCPopup.Instance.SetConfirm(OnUploadModelConfirm, CurrentTabFileName, "Uploading a new IFC model, size limit 200MB", "Upload IFC");
                    break;
                    
                default:
                    MCPopup.Instance.SetConfirm(OnUploadModelConfirm, CurrentTabFileName, "Uploading a new IFC model, size limit 200MB", "Upload IFC");
                    break;
            }
        }
    }

    public void OnUploadModelConfirm(bool _result, string _message)
    {
        if (_result)
        {
            LoadingHandler.Instance.OnFullPageLoadingStart("Model Uploading, DO NOT Close");


            switch (CurrentTabFileName)
            {
                case "ifc":
                    AppController.Instance.OnUploadProjectVersion(".ifc,.IFC", _message);
                    break;
                case "image":

                    break;
                case "document":

                    break;
                default:
                    AppController.Instance.OnUploadProjectVersion(".ifc,.IFC", _message);
                    break;
            }
        }
        else
        {
            LoadingHandler.Instance.OnFullPageLoadingEnd();
        }
    }

    // Call back from JS 
    public void OnUploadModel_CallBack(JSCallBackPackage _callbackPackage)
    {
        LoadingHandler.Instance.OnFullPageLoadingEnd();

        if (!_callbackPackage.result)
        {
            MCPopup.Instance.SetWarning("Upload model only support IFC and file size less than 50MB");

        }
        else
        {
            string progress = _callbackPackage.messages.Find(x => x.key == "progress").value;
            string action = _callbackPackage.messages.Find(x => x.key == "action").value;
            string fileName = _callbackPackage.messages.Find(x => x.key == "fileName").value;

            switch (action)
            {
                case "upload":
                    // Show progress;
                    MCPopup.Instance.SetWarning("Selected version of file [" + fileName + "], is not supported at the moment.", "Model Upload");
                    break;
                case "complete":
                    MCPopup.Instance.SetInformation("Model Processing takes up to 10 minutes. Please wait for the model to be processed before you can view it.", "Upload Complete");
                    OnClick_RefreshWorkspace();
                    break;
                case "nofile":
                    MCPopup.Instance.SetInformation("No model file is uploaded", "Upload Complete");
                    break;
                case "oversize":
                    MCPopup.Instance.SetInformation("Model size is too big to upload at the moment (50MB)", "Upload Complete");
                    break;
                case "editor":
                    MCPopup.Instance.SetInformation("Upload is not functional in editor", "Upload Complete");
                    break;
                case "multiselection":
                    MCPopup.Instance.SetInformation("Please select one file to upload", "Upload Complete");
                    break;
                default:
                    MCPopup.Instance.SetWarning("Selected version of file [" + fileName + "], is not supported at the moment.", "Model Upload");
                    break;
            }


        }



    }




    // file tab
    public void OnFileTabSelect(string _tabName = "")
    {
        string selectTab = "";

        if (_tabName == "")
        {
            selectTab = CurrentTabFileName;
        }
        else
        {
            CurrentTabFileName = _tabName;
            selectTab = _tabName;
        }


        // extra work on each tab

        switch (selectTab)
        {
            case "ifc":
                if (SelectedModelItemObject != null)
                {
                    OnRenderProjectVersion(SelectedModelItemObject.GetComponent<UIBlock_Project_ModelItem>().Item.versions);
                }
                break;
            case "dwg":
                if (SelectedModelItemObject != null)
                {
                   
                }
                break;
            case "csv":
                if (SelectedModelItemObject != null)
                {
                 
                }
                break;
            case "img":
                if (SelectedModelItemObject != null)
                {
                
                }
                break;
            default:
                if (SelectedModelItemObject != null)
                {
                    
                }
                break;
        }
    }




    // version tab
    public void OnTabSelect(string _tabName = "")
    {
        string selectTab = "";

        if (_tabName == "")
        {
            selectTab = CurrentTabName;
        }
        else
        {
            CurrentTabName = _tabName;
            selectTab = _tabName;
        }

        // clear list
        OnClearModelUpdateItemList();
        ClearPresetObjects();

        switch (selectTab)
        {
            case "update":
                ShowTab_Update();
                break;
            case "export":
                ShowTab_Export();
                break;
            case "collaboration":
                ShowTab_Collaboration();
                break;
            case "setting":
                ShowTab_Setting();
                break;
            default:
                break;
        }
    }


    public void ShowTab_Update()
    {
        if (SelectedVersionItemObject == null)
        {
            
            return;
        }
        
        UIBlock_Project_ModelVersionItem item = SelectedVersionItemObject.GetComponent<UIBlock_Project_ModelVersionItem>();

        if (item != null)
        {
            OnRenderProjectModelUpdates(item.Item);
        }

    }

    public void ShowTab_Export()
    {
        if (SelectedVersionItemObject == null)
        {
            return;
        }

    }

    public void ShowTab_Collaboration()
    {
        if (SelectedVersionItemObject == null)
        {
            return;
        }
        
        //TODO, check permission
        PresetObjects[0].SetActive(true);
        PresetObjects[1].SetActive(true);
        PresetObjects[2].SetActive(true);
    }

    public void ShowTab_Setting()
    {
        if (SelectedVersionItemObject == null)
        {
            return;
        }


 
    }
    



    public void OnClick_CopyModelShareLink()
    {
        UIBlock_Project_ModelVersionItem item = SelectedVersionItemObject.GetComponent<UIBlock_Project_ModelVersionItem>();

        string sharelink =
                    Config.Domain.Remove(Config.Domain.Length-1) + "?" +
                   item.Item.attachedWorkspace + "&" +
                   item.Item.attachedProject + "&" +
                   item.Item.guid + "&" +
                   item.Item.versionID + "&" +
                   "EN";

        GUIUtility.systemCopyBuffer = sharelink;
        MCPopup.Instance.SetInformation(sharelink, "Share link created");

    }


    public void OnClick_RedirectToModelDashboard()
    {
        UIBlock_Project_ModelVersionItem item = SelectedVersionItemObject.GetComponent<UIBlock_Project_ModelVersionItem>();

        string sharelink =
                    Config.Domain + "?" +
                   item.Item.attachedWorkspace + "&" +
                   item.Item.attachedProject + "&" +
                   item.Item.guid;

        Application.OpenURL(sharelink);
    }

    /// <summary>
    /// Create list of material from the active model
    /// 
    /// </summary>
    public void OnClick_CreateMaterialList()
    {
        // get selected modelversion
        if(SelectedVersionItemObject == null)
        {
            MCPopup.Instance.SetInformation("No model is selected!");
            return;
        }

        UIBlock_Project_ModelItem item = SelectedModelItemObject.GetComponent<UIBlock_Project_ModelItem>();

        // check if the model is loaded
        if (!item.IsModelLoaded)
        {
            MCPopup.Instance.SetInformation("Selected model is not loaded.");
            return;
        }

        // get the root object

        ModelVersion mv = ProjectModelHandler.Instance.GetModelVersion(item.Item.guid);
        
        if(mv == null)
        {
            MCPopup.Instance.SetInformation("Selected model is not exist.");
            return;
        }

        string materiallink  = Config.Domain_EasyCarbon + "?p=" + mv.Project.guid;

        MCPopup.Instance.SetConfirm(OnCreatingMaterialList_Confirm, materiallink, "Your @Easy Carbon link " + materiallink + ", is copied to clipboard, would you like to open the link?");
        GUIUtility.systemCopyBuffer = materiallink;
    }




    public void OnCreatingMaterialList_Confirm(bool _result, string _message)
    {
        if (_result)
        {
            Application.OpenURL(_message);
            MCPopup.Instance.SetInformation("@Easy Carbon is opened in a new tab.");
        }

    }

    #region Utility


    #endregion


    #region MISC


    public void ClearPresetObjects()
    {
        // clear list
        foreach (var item in PresetObjects)
        {
            if (item != null)
            {
                item.SetActive(false);
            }
        }


    }

    public void ClearObjectList(List<GameObject> _list, bool isBlockDestroy = false)
    {
        if (_list.Count > 0)
        {
            foreach (GameObject item in _list)
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
