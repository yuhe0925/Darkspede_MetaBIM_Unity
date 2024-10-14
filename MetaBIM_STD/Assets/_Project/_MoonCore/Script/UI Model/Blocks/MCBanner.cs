using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MetaBIM;
using UnityEngine.UI;


public class MCBanner : MonoBehaviour
{
    public static MCBanner Instance;

    public GameObject BackButton;
    public GameObject UserProfile;

    public PanelChange Panel_User;
    public PanelChange Panel_Language;

    [Header("UI Element")]

    public TextMeshProUGUI Text_UserName;
    public TextMeshProUGUI Text_Email;
    public TMP_InputField Text_Code;

    public MC_GetWebIcon Icon_UserPicture;


    
    // 
    private bool IsUserPanelOpened = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        SetBackButton(false);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (IsUserPanelOpened)
            {
                Invoke("DelayClose", 0.1f);
            }
        }
    }

    /// <summary>
    ///  this method is to resolve the issue that panel close before the actual button that need to be click is clicked
    /// </summary>
    public void DelayClose()
    {
        if (IsUserPanelOpened)
        {
            Panel_User.OnPanelClose();
            IsUserPanelOpened = false;
        }
    }

    public void SetBlock()
    {
        if(AppController.Instance.CurrentProfile != null)
        {
            Text_UserName.text = AppController.Instance.CurrentProfile.fullName;
            Text_Email.text = AppController.Instance.CurrentProfile.email;
            Text_Code.text = "#" + Utility.GetHashedMobileNUmber(AppController.Instance.CurrentProfile.mobile, Config.MOBILE_MASKHASH);

            // Set User Icon
            Icon_UserPicture.SetBlock(AppController.Instance.CurrentProfile.iconUrl);
        }
    }

    public void SetBackButton(bool _value)
    {
        BackButton.SetActive(_value);
    }
    

    public void SetUserProfile(bool _value)
    {
        UserProfile.SetActive(_value);
    }


    public void OnClick_UserIcon()
    {

        if (AppController.Instance.CurrentProfile == null || AppController.Instance.CurrentProfile.guid=="")
        {
            MCPopup.Instance.SetInformation("Please sign in with your account to access the platform.", "Welcome to MetaBIM");
            return;
        }

        if (IsUserPanelOpened)
        {
            Panel_User.OnPanelClose();
            IsUserPanelOpened = false;
        }
        else
        {
            Panel_User.OnPanelOpen();
            IsUserPanelOpened = true;
        }
    }



    public void OnClick_Back()
    {
        // Need warning?
        AppController.Instance.SiteMap_Back();

    }


    public void OnClick_UserProfile()
    {
        // to User Profile page?
        AppController.Instance.SetPage(AppController.PageIndex.userprofile);
        Panel_User.OnPanelClose();
    }

    public void OnClick_Language()
    {
        //Panel_Language.OnPanelOpen();
        Panel_User.OnPanelClose();

        if (ProjectConfiguration.Instance.DefaultLanguage == ProjectConfiguration.LocationType.ZH)
        {
            AppController.Instance.OnSetToEnglish();
        }
        else
        {
            AppController.Instance.OnSetToChinese();
        }
    }


    public void OnClick_Setting()
    {
        // to setting page?
    }


    public void OnClick_Admin()
    {
        if (AppController.Instance.CurrentProfile == null || AppController.Instance.CurrentProfile.guid == "")
        {
            return;
        }

        if (AppController.Instance.CurrentProfile.profileRole.roleType != "admin")
        {
            return;
        }

        if (AppController.Instance.CurrentProfile.profileRole.status != "active")
        {
            return;
        }
        
        AppController.Instance.SetPage(AppController.PageIndex.admin);
        Panel_User.OnPanelClose();
    }

    public void OnClick_Signout()
    {
        if (AppController.Instance.CurrentProfile != null)
        {
            Panel_User.OnPanelClose();
            IsUserPanelOpened = false;
            AppController.Instance.OnRequestUserSignOut();
        }
        else
        {
            MCPopup.Instance.SetInformation("You are not signed in","Erh...");
        }
    }


    public void OnClick_Download()
    {
        //Application.OpenURL(Config.Download_RevitPlugin);
        MCPopup.Instance.SetInformation("Preparing your plugin package...", "Downloading");

        StartCoroutine(
            DataProxy.Instance.OnRequestDownloadRevitPlugin(
                AppController.Instance.CurrentProfile.guid, OnDownload_Callback));
    }

    public void OnClick_Support()
    {
        Application.OpenURL(Config.SupportSite);
        MCPopup.Instance.SetInformation("Open support site in new window.", "Redirect");
    }
    
    public void OnDownload_Callback(bool _result, string _message)
    {
        if (_result)
        {
            DataProxyResponse<IModel> item = DataProxyResponse<IModel>.FromJson(_message);
            string downloadURL = item.message;

            Debug.Log(downloadURL);
            Application.OpenURL(downloadURL);
            MCPopup.Instance.SetInformation("Click anywhere to download your package", "Complete");
            Panel_User.OnPanelClose();
        }
        else
        {
            MCPopup.Instance.SetWarning("Your package is not ready.", "Plugin Error");
        }
    }


    public void OnClick_ClearSystemCache()
    {
        AppController.Instance.CleacModelCache();   // it is best to set a confirmation , TODO
    } 



    public void OnClick_CopyUserCode()
    {
        if (Text_Code.text != "")
        {
            // ?
        }
    }
}
