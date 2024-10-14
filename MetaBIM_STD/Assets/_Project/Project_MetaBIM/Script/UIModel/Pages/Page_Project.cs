using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MetaBIM;
using UnityEngine.UI;
using System;

public class Page_Project : MonoBehaviour
{
    public static Page_Project Instance;
    public PanelChange Panel;
    
 
    public Project CurrentProject;
    public MetaBIM.Version SelcetedVersion;
    public GameObject SelectedVersionObject;
    
    
    [Header("Prject Information")]
    public TMP_InputField Text_ProjectNameOnHeader;


    [Header("Version")]
    public ProjectVersionItemScrollView ScrollView_VersionItem;

    public int MaxDataUsageInProject = 1024;

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
        SelectedVersionObject = null;

        if (!ScrollView_VersionItem.IsInitialized)
        {
            ScrollView_VersionItem.Init();
        }
        RenderProject();
    }


    public void OnCloseAction()
    {
        if (SelectedVersionObject != null)
        {
            SelectedVersionObject.GetComponent<UIBlock_Project_VersionItem>().OnDeselected();
        }

        SelectedVersionObject = null;
    }


    public void RefreshProject()
    {   
        StartCoroutine(DataProxy.Instance.GetWorkspaces("createdBy", AppController.Instance.CurrentProfile.guid, RefreshProject_Callback));
    }
    
    public void RefreshProject_Callback(bool _result, string _message)
    {
        if (_result)
        {
            DataProxyResponse<Workspace> payload = JsonUtility.FromJson<DataProxyResponse<Workspace>>(_message);
            DataSet.MyWorkspaces = payload.package;

            if (payload.result)
            {
                DataSet.MyWorkspaces.Sort((x, y) => y.updated.CompareTo(x.updated));  //default sort
                RenderProject();
            }
            else
            {
                MCPopup.Instance.SetWarning(payload.message);
            }

        }
        else
        {
            MCPopup.Instance.SetWarning("Refresh failed");
        }
    }





    public void RenderProject()
    {
        /*
        CurrentProject = AppController.Instance.GetProjectFromDataset(AppController.Instance.SelectedProjectGuid);
        
        if (CurrentProject == null)
        {
            AppController.Instance.SetPage(AppController.PageIndex.dashboard);
        }

        CurrentProject.versions.Sort((x, y) => x.updated.CompareTo(y.updated));
        RenderProjectInformation();

        StartCoroutine(UpdateScrollView());
        */
    }

    // This methed is to be call via Invoke
    public IEnumerator UpdateScrollView()
    {
        yield return new WaitForSeconds(0.05f);
        ScrollView_VersionItem.SetItems(CurrentProject.versions);
    }


    public void RenderProjectInformation()
    {
        Text_ProjectNameOnHeader.text = CurrentProject.projectName;
    }


    public void RenderProjectGIS()
    {

    }

    public void OnClick_OnUpdateProjectName(string _value)
    {
        if (_value != "" && _value != CurrentProject.projectName) {
            CurrentProject.projectName = _value;
            Workspace SelectedWorkspace = DataSet.MyWorkspaces.Find(x => x.guid == AppController.Instance.SelectedWorkspaceGuid);
            StartCoroutine(DataProxy.Instance.UpdateWorkspace(Workspace.ToJson(SelectedWorkspace), OnUpdateProjectCallback));
        }
    }

    public void OnUpdateProjectCallback(bool _result, string _message)
    {
        if (!_result)
        {
            MCPopup.Instance.SetWarning("Operation can not be completed, please try again later.");
        }
        else
        {
            MCPopup.Instance.SetComplete("Project updated.");
        }
    }



    public void OnClick_VersionItem(GameObject _gameObject)
    {
        if(SelectedVersionObject != null)
        {
            SelectedVersionObject.GetComponent<UIBlock_Project_VersionItem>().OnDeselected();
        }

        SelectedVersionObject = _gameObject;
        SelcetedVersion = SelectedVersionObject.GetComponent<UIBlock_Project_VersionItem>().Item;
        SelectedVersionObject.GetComponent<UIBlock_Project_VersionItem>().OnSelected();
    }

    public void OnClick_ProjectBimViwer()
    {
        if (SelectedVersionObject == null)
        {
            MCPopup.Instance.SetWarning("Please select a model");
            AppController.Instance.SelectedVersionGuid = "";
            return;
        }

        

        if (SelectedVersionObject.GetComponent<UIBlock_Project_VersionItem>().Item.processingStatus != "completed")   // Hardcoded status, moves to configuration?
        {
            MCPopup.Instance.SetWarning("Model is still processing.");
            AppController.Instance.SelectedVersionGuid = "";
            return;
        }
        
        AppController.Instance.SelectedVersionGuid = SelectedVersionObject.GetComponent<UIBlock_Project_VersionItem>().Item.guid;


        MCPopup.Instance.SetConfirm(OnOpenProjectBimViwer_Callback, "", "Loading " + SelcetedVersion.originalFileName + ", may take a while to load");
        
 
    }


    public void OnOpenProjectBimViwer_Callback(bool _result, string _message)
    {
        if (_result)
        {
            // Maybe downloading version bimobject data before loading page?
            AppController.Instance.SetPage(AppController.PageIndex.bimview);
        }
        else
        {
           
        }
    }







    [Header("Collabration")]
    public GameObject SelectedCollabration;
    public UIController_InputField InputFieldInviteEmail;

    public List<Collaboration> CurrentCollaboration;
    public List<Profile> CollaborationUsers;

    public void LoadCollaborations()
    {
        //StartCoroutine(DataProxy.Instance.GetAllCollaborations("associatedProject", CurrentProject.guid, OnGetAllCollaborations_Callback));
    }

    public void OnGetAllCollaborations_Callback(bool _success, string _message)
    {
        if (_success)
        {
            //CurrentCollaboration = JsonUtility.FromJson<DataProxyResponse_BatchCollaboration>(_message).package;
            //CollaborationUsers = new List<Profile>();
            CurrentCollaboration.Sort((x, y) => x.updated.CompareTo(y.updated));  //default sort

            List<Collaboration> sorted = new List<Collaboration>();
            Collaboration owner = CurrentCollaboration.Find(x => x.permission == "owner");
            if (owner != null) // Owner at the top
            {
                sorted.Add(owner);
                CurrentCollaboration.Remove(owner);
            }

            Collaboration myself = CurrentCollaboration.Find(x => x.collaborator == AppController.Instance.CurrentProfile.guid);
            if (myself != null) // Myself at the second
            {
                sorted.Add(myself);
                CurrentCollaboration.Remove(myself);
            }
            sorted.AddRange(CurrentCollaboration);

            CurrentCollaboration = sorted;

            RenderMemberItem();
        }
        else
        {
            MCPopup.Instance.SetWarning("Render member item failed. Message: " + _message);
        }
    }

    public void RenderMemberItem()
    {

    }

    public void OnClick_SelectCollaborationItem(GameObject _gameObject)
    {

    }


    public void OnClick_SendCollaborationInvite()
    {
        if(CurrentCollaboration.Find(x => x.collaborator == AppController.Instance.CurrentProfile.guid).permission != "owner")
        {
            MCPopup.Instance.SetWarning("Only owner can send invitations.");
            return;
        }

        if (InputFieldInviteEmail.IsValidated)
        {
            string email = InputFieldInviteEmail.InputField.text;
            /*
            StartCoroutine(DataProxy.Instance.OnRequestSendCollaborationInvite(
                AppController.Instance.CurrentUser.guid,
                CurrentProject.guid,
                email,
                "collaborator",
                OnClick_SendCollaborationInviteCallback));
            */
        }
        else
        {
            MCPopup.Instance.SetWarning("Invalid email format. Please enter a valid email.");
            return;
        }
    }

    public void OnClick_SendCollaborationInviteCallback(bool success, string message)
    {
        if (success)
        {
            MCPopup.Instance.SetComplete("Invitation sent.");
            RefreshProject();
            InputFieldInviteEmail.InputField.text = "";
        }
        else
        {
            MCPopup.Instance.SetWarning(message);
        }
    }

    public void OnClick_AcceptInvitation(GameObject memberBlock)
    {
        Collaboration item = memberBlock.GetComponent<UIBlock_Project_CollaborationItem>().Item;
        item.collaborationStatus = "accepted";

        //StartCoroutine(DataProxy.Instance.UpdateCollaboration(Collaboration.ToJson(item), OnClick_AcceptInvitationCallback));
    }

    public void OnClick_AcceptInvitationCallback(bool success, string message)
    {
        if (success)
        {
            MCPopup.Instance.SetComplete("Invitation accepted");
            LoadCollaborations();
        }
        else
        {
            MCPopup.Instance.SetWarning("Could not accept invitation");
        }
    }




    #region UI Action


    
    public void OnClick_SelectTransactionItem(GameObject _gameObject)
    {

    }


    public void OnClick_AddNewUpdate()
    {

    }

    public void OnClick_SendInvitation()
    {

    }

    public void OnClick_VerifyVersion()
    {
        AppController.Instance.SetPage(AppController.PageIndex.bimcompare);
    }


    public void OnClick_OpenNodeTransactinNodePanel(GameObject _gameObject)
    {
       
    }


    public void OnClick_CloseNodeTransactinNodePanel()
    {
       
    }


    public void OnClick_UploadModel()
    {
        if (AppController.Instance.CurrentProfile == null || AppController.Instance.SelectedWorkspaceGuid == "" || AppController.Instance.SelectedProjectGuid == "")
        {
            MCPopup.Instance.SetWarning("Please select a project.");
        }
        else
        {
            MCPopup.Instance.SetConfirm(OnUploadModelConfirm, "", "Uploading a new IFC model, size limit 50MB","");
        }
    }

    public void OnUploadModelConfirm(bool _result, string _message)
    {
        if (_result)
        {

            LoadingHandler.Instance.OnFullPageLoadingStart("Model Uploading");

            AppController.Instance.OnUploadProjectVersion("","");
        }
    }

    public void OnUploadModel_CallBack(JSCallBackPackage _callbackPackage)
    {
        if (!_callbackPackage.result)
        {
            MCPopup.Instance.SetWarning("Upload model only support IFC and size less than 50MB");
        }
        else
        {

            string progress = _callbackPackage.messages.Find(x => x.key == "progress").value;
            string action = _callbackPackage.messages.Find(x => x.key == "action").value;
            string fileName = _callbackPackage.messages.Find(x => x.key == "fileName").value;
            string convertionAction = _callbackPackage.messages.Find(x => x.key == "convertionAction").value;
            
            switch (action)
            {
                case "upload":
                    // Show progress;
                    MCPopup.Instance.SetWarning("Selected version of file [" + fileName + "], is not supported at the moment.", "Model Upload");
                    break;
                case "complete":
                    MCPopup.Instance.SetInformation("Model Processing takes up to 10 minutes. Please wait for the model to be processed before you can view it.", "Upload Complete");
                    break;
                case "nofile":
                    MCPopup.Instance.SetInformation("No model file is uploaded", "Upload Complete");
                    break;
                case "oversize":
                    MCPopup.Instance.SetInformation("Model size is too big to upload at the moment (50MB)", "Upload Complete");
                    break;
                default:
                    MCPopup.Instance.SetWarning("Selected version of file [" + fileName + "], is not supported at the moment.", "Model Upload");
                    break;
            }

            RefreshProject();
        }

        LoadingHandler.Instance.OnFullPageLoadingEnd();

    }

    #endregion


    #region MISC
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





