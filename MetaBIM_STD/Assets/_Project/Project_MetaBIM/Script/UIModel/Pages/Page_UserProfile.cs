using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MetaBIM;
using UnityEngine.UI;
using System;

public class Page_UserProfile : MonoBehaviour
{
    public static Page_UserProfile Instance;
    public PanelChange Panel;



    [Header("UI Element")]
    public TMP_InputField Text_UserName;
    public TMP_InputField Text_Email;
    public TMP_InputField Text_Location;
    public MC_GetWebIcon Icon_UserPicture;

    public UIController_Form Form;

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
        Debug.Log("Page_UserProfile.OnOpenAction");

        if(AppController.Instance.CurrentProfile.config.registration.action == Config.None || AppController.Instance.CurrentProfile.config.registration.action == Config.None.ToLower())
        {
            Text_UserName.text = "";
            Text_Email.text = "";
            Text_Location.text = "";
        }
        else
        {
            Debug.Log("Page_UserProfile.OnOpenAction: " + AppController.Instance.CurrentProfile.config.registration.action);

            Text_UserName.text  = AppController.Instance.CurrentProfile.fullName;
            Text_Email.text     = AppController.Instance.CurrentProfile.email;
            Text_Location.text  = AppController.Instance.CurrentProfile.location;

            // This is maybe not a good way to do it, for init it is good
            Text_UserName.gameObject.GetComponent<UIController_InputField>().InitTransaction();
            Text_Email.gameObject.GetComponent<UIController_InputField>().InitTransaction();
            Text_Location.gameObject.GetComponent<UIController_InputField>().InitTransaction();
        }

        Icon_UserPicture.SetBlock(AppController.Instance.CurrentProfile.iconUrl);
    }

    public void OnCloseAction()
    {
        //Debug.Log("Page_UserProfile.OnCloseAction");
    }

    public void OnClick_ChooseFile()
    {
        //JSCaller.UploadModelVersion(DataSet.GetUploadFileTargetString(DataSet.UploadFileTarget.usericon), 
        //    AppController.Instance.CurrentProfile.guid, 0, DataSet.GetFileTypeString(DataSet.FileType.Image));
    }


    // Called from JS Callback Script
    public void UploadChooseFileCallback(string _result)
    {
        if (_result == "true")
        {
            AppController.Instance.GetCurrentProfileInformation(OnGetUserInformationCallback);
        }
        else
        {
            MCPopup.Instance.SetWarning("Error occured on uploading picture.", "Update Failed");
        }
    }

    public void OnGetUserInformationCallback(bool _result, string _message)
    {
        if (_result == true)
        {
            var package = DataProxyResponse<Profile>.FromJson(_message);


            if (package.result)
            {
                Profile user = package.package[0];

                // Override Profile, do we need to ?
                AppController.Instance.CurrentProfile = user;

                // Display Icon
                Icon_UserPicture.SetBlock(AppController.Instance.CurrentProfile.iconUrl);
                //Page_Dashboard.Instance.
            }
            else
            {
                MCPopup.Instance.SetWarning(package.message, "Update Failed");
            }

        }else
        {
            MCPopup.Instance.SetWarning("Network Error!", "Update Failed");
        }


    }

    public void OnClick_Submit()
    {
        if (Form.GetFormStatus())
        {
            AppController.Instance.CurrentProfile.fullName = Text_UserName.text;
            AppController.Instance.CurrentProfile.email = Text_Email.text;
            AppController.Instance.CurrentProfile.location = Text_Location.text;
            ProfileAction reg = new ProfileAction();
            reg.status = "active";
            reg.created = DateTime.Now.Ticks.ToString();
            AppController.Instance.CurrentProfile.config.registration = reg;

            UpdateUserProfile();
        }
        else
        {
            MCPopup.Instance.SetWarning("One or more field is required to complete", "Warning");
        }
    }

    
    
    public void UpdateUserProfile()
    {
        string package = AppController.Instance.CurrentProfile.ToJson();

        //TODO. update API need more security
        StartCoroutine(DataProxy.Instance.UpdateProfile(package, OnUpdateUserProfile_Callback));
    }

    

    public void OnUpdateUserProfile_Callback(bool _result, string _message)
    {
        if (_result)
        {

            var package = DataProxyResponse<Profile>.FromJson(_message);

            if (package.result)
            {
                MCPopup.Instance.SetComplete("Your profile information has been updated.", "Update Profile");
                // Direct to dashboard
                AppController.Instance.SetPage(AppController.PageIndex.dashboard);
                AppController.Instance.Banner.SetBlock();

            }
            else
            {
                MCPopup.Instance.SetWarning(package.message, "Update Failed");
            }
        }
        else
        {
            MCPopup.Instance.SetWarning("Network error!", "Update Failed");

        }
    }

    public void OnClick_Close()
    {
        if (AppController.Instance.CurrentProfile.config.registration.action == Config.None || AppController.Instance.CurrentProfile.config.registration.action == Config.None.ToLower())
        {
            MCPopup.Instance.SetInformation("Hello, new friend, thank you for using " 
                + Config.ProductionName + ", please fill in your basic information.", "Registeration Required");
        }
        else
        {
            AppController.Instance.SetPage(AppController.PageIndex.dashboard);
            AppController.Instance.Banner.SetBlock();
        }

    }
}
