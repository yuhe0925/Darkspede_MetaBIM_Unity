using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace MetaBIM
{

    public class AppController : MonoBehaviour
    {
        public static AppController Instance;
     
        public static AppController GetInstance()
        {
            if (Instance == null)
            {
                Instance = GameObject.Find("AppController").GetComponent<AppController>();
            }

            return Instance;
        }


        public Camera MainCamera;

        [SerializeField]
        private bool IsUserSignin = false;
        public bool IsAppLoaded = false;



        [Header("General Data")]
        public Profile CurrentProfile;
        public ShareLinkPackage CurrentShareLinkPackage;

        [Header("Page Information")]
        // Page 0 = sign in
        public List<PanelChange> Pages;
        public MCBanner Banner;

        [Header("Debug Information")]
        public string RedirectLink = "";

        public List<PageIndex> PageHistory = new List<PageIndex>();
        public PageIndex CurrentPage;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }


        // Start is called before the first frame update
        void Start()
        {
            MainCamera = MainCamera == null ? Camera.main : MainCamera;

#if (UNITY_WEBGL && !UNITY_EDITOR)
            OnLoadRedirectLink();
#else
            OnLoadRedirectLink_Callback(RedirectLink);
#endif

        }


        /// <summary>
        /// ask the app to enter general mode
        /// </summary>
        public void SetAppToGeneralMode()
        {
            if (PlayerPrefs.HasKey(Config.Pref_SavedUser))
            {
                string userJson = PlayerPrefs.GetString(Config.Pref_SavedUser);

                if (userJson != "")
                {
                    Profile user = Profile.FromJson(userJson);

                    // Auto Login Attempt
                    StartCoroutine(DataProxy.Instance.OnRequestLoginByToken(user.guid, user.accesstoken, OnUserLoginByTokenCallBack));
                }
                else
                {
                    OnSuccessSignOut();
                    Debug.Log("No user saved, stay in landing");
                }
            }
            else
            {
                OnSuccessSignOut();
                Debug.Log("No user saved, stay in landing");
            }
        }


        public void OnLoadRedirectLink()
        {
            JSCaller.OnRequestRedirectURL("");
        }

        
        public void OnLoadRedirectLink_Callback(string _message)
        {
            Debug.Log("OnLoadRedirectLink_Callback: " + _message);
            
            if (_message == "false")
            {
                Debug.Log("OnLoadRedirectLink_Callback: false");
                SetAppToGeneralMode();
                return;
            }
            
            string[] url_elements = _message.Split("?");

            if (url_elements.Length == 1)
            {
                string domain = url_elements[0];
                Config.SetURL(domain + "/");
            }
            else if (url_elements.Length == 2)
            {
                // extract parameters
                string[] parameters = url_elements[1].Split("&");

                if (parameters.Length != 5)
                {
                    Debug.Log("OnLoadRedirectLink_Callback: " + parameters.Length);
                    SetAppToGeneralMode();
                    return;
                }

                string workspaceGUID = parameters[0];
                string projectGUID = parameters[1];
                string versionGUID = parameters[2];
                string updateGUID = parameters[3];
                string location = parameters[4];

                OnValidateShareLink(workspaceGUID, projectGUID, versionGUID, updateGUID, location);
            }
        }


        public void OnValidateShareLink(string _workspace, string _project, string _version, string _update, string _location)
        {
            CurrentShareLinkPackage = new ShareLinkPackage();
            CurrentShareLinkPackage.workspaceGuid = _workspace;
            CurrentShareLinkPackage.projectGuid = _project;
            CurrentShareLinkPackage.versionGuid = _version;
            CurrentShareLinkPackage.update = int.Parse(_update);
            CurrentShareLinkPackage.location = _location;

            

            StartCoroutine(DataProxy.Instance.GetWorkspaceByGuid(_workspace, OnValidateShareLink_Callback));
        }

        public void OnValidateShareLink_Callback(bool _result, string _message)
        {
            Debug.Log("OnValidateShareLink_Callback: " + _message);
            CurrentShareLinkPackage.shared = false;
            
            if (_result)
            {
                DataProxyResponse<Workspace> payload = JsonUtility.FromJson<DataProxyResponse<Workspace>>(_message);

                if (payload.result)
                {
                    if (payload.package[0] != null)
                    {
                        Workspace workspace = payload.package[0];
                        CurrentShareLinkPackage.workspace = workspace;

                        foreach (var project in workspace.projects)
                        {
                            if(project.guid == CurrentShareLinkPackage.projectGuid)
                            {
                                CurrentShareLinkPackage.project = project;
                                
                                foreach (var version in project.versions)
                                {
                                    if(version.guid == CurrentShareLinkPackage.versionGuid)
                                    {
                                        CurrentShareLinkPackage.version = version;
                                        Debug.Log("Open Share Link");
                                        CurrentShareLinkPackage.shared = true;

                                        if (AppController.Instance.CurrentShareLinkPackage.location.ToUpper() == "EN")
                                        {
                                            if (ProjectConfiguration.Instance.DefaultLanguage != ProjectConfiguration.LocationType.EN)
                                            {
                                                AppController.Instance.OnSetToEnglish();
                                            }
                                        }
                                        else
                                        {
                                            if (ProjectConfiguration.Instance.DefaultLanguage == ProjectConfiguration.LocationType.EN)
                                            {
                                                AppController.Instance.OnSetToChinese();
                                            }
                                        }


                                        SetPage(PageIndex.sharelink);
                                        
                                        return;
                                    }
                                }
                            }
                        }

                        SetAppToGeneralMode();
                    }
                    else
                    {
                        SetAppToGeneralMode();
                    }
                }
                else
                {
                    SetAppToGeneralMode();
                }
            }

            
        }

        public void OnUserLoginByTokenCallBack(bool _success, string _message)
        {
            Debug.Log("OnUserLoginCallBack: " + _message);

            if (_success)
            {
                DataProxyResponse<Profile> response = DataProxyResponse<Profile>.FromJson(_message);
                if (response.result && response.package.Count == 1)
                {
                    Profile profile = response.package[0];
                    OnSuccessSignin(profile);
                }
                else
                {
                    OnSuccessSignOut();
                    Debug.Log("Response content eroor(fail result or package size incorrect): " + _message);
                }

            }
            else
            {
                Debug.Log("Token Sign Fail, stay in landing, " + _message);
                OnSuccessSignOut();
            }

            IsAppLoaded = true;
        }

        public void OnSuccessSignin(Profile _profile)
        {
            IsUserSignin = true;
            CurrentProfile = _profile;
            string savedUser = Profile.ToJson(CurrentProfile);
            PlayerPrefs.SetString(Config.Pref_SavedUser, savedUser);

            // Set Banner
            Banner.SetBlock();
            


            // Check User Status, init config
            if (CurrentProfile.config.registration.status == Config.None || CurrentProfile.config.registration.status == Config.None.ToLower())
            {
                SetPage(PageIndex.userprofile);
            }
            else
            {
                SetPage(ProjectConfiguration.Instance.DefaultMainPage);
            }


            // Loading Data

            // Get Project I own
            //StartCoroutine(DataProxy.Instance.GetAllProjects("createdBy", CurrentUser.guid, OnGetAllProjects_Callback));

            // Get Project I Have access to
            // TODO
        }

        public void GetCurrentProfileInformation(Action<bool, string> _callback)
        {
            StartCoroutine(DataProxy.Instance.GetProfileByGuid(CurrentProfile.guid, _callback));
        }


        public void OnGetAllProjects_Callback(bool _result, string _message)
        {

        }



        /* Section Selection */
        public void OnRequestUserSignOut()
        {
            MCPopup.Instance.SetConfirm(OnRequestUserSignOut_Callback, "","You are about to sign out from your account!","Signing Out");
        }

        public void OnRequestUserSignOut_Callback(bool _result, string _message)
        {
            if (_result)
            {
                OnSuccessSignOut();
            }
        }

        public void OnSuccessSignOut()
        {
            PlayerPrefs.DeleteKey(Config.Pref_SavedUser);
            CurrentProfile = null;
            // SetBannerInformation
            SetPage(PageIndex.signin);
        }

        public void GetUserNotification_Callbacking(bool _result, string _message)
        {
            if (_result)
            {

            }
        }


        #region Page Handle


        public enum PageIndex {
            [EnumMember(Value = "Current")]
            current = -1,
            [EnumMember(Value = "Sign In")]
            signin = 0,
            [EnumMember(Value = "Darhboard")]
            dashboard = 1,
            [EnumMember(Value = "Workspace")]
            workspace = 2,
            [EnumMember(Value = "Project")]
            project = 3,
            [EnumMember(Value = "BIM Viewer")]
            bimview = 4,
            [EnumMember(Value = "User Profile")]
            userprofile = 5,
            [EnumMember(Value = "BIM Compare")]
            bimcompare = 6,
            [EnumMember(Value = "Admin")]
            admin = 7,
            [EnumMember(Value = "Share Link")]
            sharelink = 8,

        }

        public void SetPage(PageIndex _pageIndex = PageIndex.signin)
        {
            //Debug.Log("Open Page: " + (int)_pageIndex);
            
            // Keep Page Histroy
            if (PageHistory.Count  >= Config.MAX_PAGE_HISTORY)
            {
                PageHistory.RemoveAt(PageHistory.Count - 1);
            }

            PageHistory.Insert(0, CurrentPage);
            
            CurrentPage = _pageIndex;
            SetDefaultCursor();
            
            // Set Page
            for (int i = 0; i < Pages.Count; i++)
            {
                if((int)_pageIndex == i)
                {
                    Pages[i].OnPanelOpen();
                }
                else
                {
                    Pages[i].OnPanelClose();
                }
            }

            // Set Banner Navigation
            switch (CurrentPage)
            {
                case PageIndex.dashboard:
                    Banner.SetBackButton(false);
                    break;

                case PageIndex.workspace:
                    Banner.SetBackButton(true);
                    break;

                case PageIndex.project:
                    Banner.SetBackButton(true);
                    break;

                case PageIndex.signin:
                    Banner.SetBackButton(false);
                    break;

                case PageIndex.userprofile:
                    Banner.SetBackButton(true);
                    break;

                case PageIndex.bimview:
                    if (!AppController.Instance.CurrentShareLinkPackage.shared)
                    {
                        Banner.SetBackButton(true);
                    }
                    break;

                case PageIndex.bimcompare:
                    Banner.SetBackButton(true);
                    break;
                    
                case PageIndex.admin:
                    Banner.SetBackButton(true);
                    break;
                case PageIndex.sharelink:
                    Banner.SetUserProfile(false);
                    Banner.SetBackButton(false);
                    break;

                default:
                    Banner.SetBackButton(false);
                    break;
            }


        }

        // Need rework this hardcode site map
        public bool SiteMap_Back()
        {
            switch (CurrentPage) {
                case PageIndex.dashboard:
                    return false;
                case PageIndex.workspace:
                    SetPage(PageIndex.dashboard);
                    return true;
                case PageIndex.project:
                    SetPage(PageIndex.workspace);
                    return true;
                case PageIndex.signin:
                    return false;
                case PageIndex.userprofile:
                    SetPage(PageIndex.dashboard);
                    return false;
                case PageIndex.bimview:
                    SetPage(PageIndex.workspace);
                    return true;
                case PageIndex.bimcompare:
                    SetPage(PageIndex.workspace);    // this is a wrong mapping, remove after fix
                    return true;
                case PageIndex.admin:
                    SetPage(PageHistory[0]);
                    return true;
                default:
                    SetPage(PageHistory[0]);
                    return false;
            }
        }


        public void SetDefaultCursor()
        {
            UnityEngine.Cursor.SetCursor(
                ResourceHolder.Instance.DefaultCursor,
                Vector2.zero, 
                CursorMode.Auto);
        }



        #endregion


        #region Localization
        
        public void OnSetToEnglish()
        {
  
            ProjectConfiguration.Instance.DefaultLanguage = ProjectConfiguration.LocationType.EN;

            LocalizationItem[] obs = GameObject.FindObjectsOfType<LocalizationItem>(true);
            
            foreach (LocalizationItem item in obs)
            {
                if (item != null)
                {
                    if (item.Type != LocalizationItem.TextType.icon)
                    {
                        item.SetLocalize("EN");
                    }
                }
            }
        }


        public void OnSetToChinese()
        {
            ProjectConfiguration.Instance.DefaultLanguage = ProjectConfiguration.LocationType.ZH;
            
            LocalizationItem[] obs = GameObject.FindObjectsOfType<LocalizationItem>(true);
            
            foreach (LocalizationItem item in obs)
            {
                if (item != null)
                {
                    if (item.Type != LocalizationItem.TextType.icon)
                    {
                        item.SetLocalize("CH");
                    }
                }
            }
        }


        #endregion




        #region Data Handle
        [Header("Buffer Data")]
        public List<Workspace> LoadedWorkspaces;
        [Header("App Data Flow")]
        public string SelectedWorkspaceGuid;
        public string SelectedProjectGuid;
        public string SelectedVersionGuid;

        // Viewer data
        public List<MetaBIM.Version> ComparingVersions;
        public List<MetaBIM.Project> SelectedProjects;

        public string GetProfileIconUrl(string _guid)
        {
            return Config.UserIcon_Path + _guid + ".png";
        }

        public void OnLoadMyWorkspace(Action<bool, string> _callback)
        {
            StartCoroutine(DataProxy.Instance.GetWorkspaces("createdBy", CurrentProfile.guid, _callback));
        }


        public void SetWorkspace(List<Workspace> _data)
        {
            DataSet.MyWorkspaces = _data; 
        }
        

        public void OnUpdateWorkspace(Workspace _worksapce, Action<bool, string> _callback)
        {         
            string payload = _worksapce.ToJson();
            StartCoroutine(DataProxy.Instance.UpdateWorkspace(payload, _callback));
        }


        public Workspace GetWorkspaceFromDataset(string _workspaceGuid)
        {
            return DataSet.MyWorkspaces.Find(x => x.guid == _workspaceGuid);
        }

        public Project GetProjectFromDataset(string _projectGuid)
        {
            foreach (Workspace workspace in DataSet.MyWorkspaces)
            {
                Project project = workspace.projects.Find(x => x.guid == _projectGuid);
                if (project != null)
                {
                    return project;
                }
            }

            return null;
        }

        public Version GetVersionFromDataset(string _versionID)
        {
            foreach (Workspace workspace in DataSet.MyWorkspaces)
            {
                foreach (Project project in workspace.projects)
                {
                    Version version = project.versions.Find(x => x.guid == _versionID);
                    if (version != null)
                    {
                        return version;
                    }
                }
            }
            
            return null;
        }

        #endregion


        #region Operation with Platform Specific


        public void OnUploadProjectVersion(string _fileType, string _convertionAction)
        {


#if (UNITY_WEBGL && !UNITY_EDITOR)
            JSCaller.RequestUploadModelVersion(SelectedWorkspaceGuid, SelectedProjectGuid, CurrentProfile.guid, _fileType, _convertionAction);
#else

            string extension = _fileType.Replace(".", "");
            string[] extensions = extension.Split(',');

            ExtensionFilter[] extensionFilters = new ExtensionFilter[extensions.Length];

            for (int i = 0; i < extensions.Length; i++)
            {
                extensionFilters[i] = new ExtensionFilter("", extensions[i]);
            }

            var paths = StandaloneFileBrowser.OpenFilePanel("Select a file", "", extensionFilters, false);

            if (paths.Length > 0)
            {
                ReadFileAsync(
                    SelectedWorkspaceGuid,
                    SelectedProjectGuid,
                    CurrentProfile.guid,
                    _fileType,
                    _convertionAction,
                    paths[0]);
            }
            else
            {

            }
#endif

        }

        string ConvertionAction = "";
        string FileName = "";
        
        
        void ReadFileAsync(string _workspaceGuid, string _projectGuid, string _profileGuid, string _fileType, string _convertionAction, string _fileUrl)
        {
            string filePath = _fileUrl;
            FileName = Path.GetFileName(filePath);
            
            ConvertionAction = _convertionAction;
            //Use ThreadPool to avoid freezing
            ThreadPool.QueueUserWorkItem(delegate
            {
                bool success = false;
                byte[] fileBytes = new byte[0];
                string encodedText = "";

                FileStream fileStream = null;

                try
                {
                    fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);

                    int length = (int)fileStream.Length;
                    fileBytes = new byte[length];
                    int count;
                    int sum = 0;

                    // read until Read method returns 0
                    while ((count = fileStream.Read(fileBytes, sum, length - sum)) > 0)
                    {
                        //Debug.Log("count: " + count);
                        sum += count;

                    }

                    success = true;

                    encodedText = Convert.ToBase64String(fileBytes);
                }
                catch (Exception e)
                {
                    MCPopup.Instance.SetWarning("File is used by other program.", StringBuffer.Messaege_Popup_Information.S);
                    Debug.Log("Fail: " + e.Message);
                }
                finally
                {
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                }

                if (success)
                {

                    UnityThread.executeInUpdate(() =>
                    {
                        StartCoroutine(DataProxy.Instance.OnUploadModelVersionNative(_workspaceGuid, _projectGuid, _profileGuid, _convertionAction, filePath, encodedText, OnRequestModelVersionUpload_Callback));
                    });
                }
                else
                {
                    FileName = "";
                    ConvertionAction = "";
                    Debug.Log("Not Success: ");
                }
            });
        }


        public void OnRequestModelVersionUpload_Callback(bool _result, string _message)
        {
            Debug.Log(_message);

            if (_result)
            {
                JSCallBackPackage package = new JSCallBackPackage();

                package.result = true;
                package.messages = new List<JSMessage>();
                package.messages.Add(new JSMessage("action", "complete", "eq"));
                package.messages.Add(new JSMessage("progress", "0", "eq"));
                package.messages.Add(new JSMessage("fileName", FileName, "eq"));
                package.messages.Add(new JSMessage("convertionAction", ConvertionAction, "eq"));
                OnJavascriptCallBack.Instance.UploadModelVersion_Callback(JsonUtility.ToJson(package));
            }
            else
            {
                JSCallBackPackage package = new JSCallBackPackage();

                package.result = true;
                package.messages = new List<JSMessage>();
                package.messages.Add(new JSMessage("action", "upload", "eq"));
                package.messages.Add(new JSMessage("progress", "0", "eq"));
                package.messages.Add(new JSMessage("fileName", FileName, "eq"));
                package.messages.Add(new JSMessage("convertionAction", ConvertionAction, "eq"));
                OnJavascriptCallBack.Instance.UploadModelVersion_Callback(JsonUtility.ToJson(package));
            }

            FileName = "";
            ConvertionAction = "";
        }



        public void CleacModelCache()
        {

            //AssetBundles are cached in the browser cache in Unity 2022.1
            // UnityEngine.Caching.ClearCache();
            // need a JS call to clear Caches
            
#if (UNITY_WEBGL && !UNITY_EDITOR)
            //UnityEngine.Caching.ClearCache();
#else   
            UnityEngine.Caching.ClearCache();
#endif
            MCPopup.Instance.SetInformation("All cached data is cleared.");
        }

        
        //WIP add confirmation panel
        public void OnRestartApplicationQuietly()
        {
            Scene scene = SceneManager.GetActiveScene();
            LeanTween.cancelAll();
            SceneManager.LoadScene(scene.name);
        }

        //WIP add confirmation panel
        public void OnCloseApplication()
        {
            Application.Quit(0);
        }

        #endregion


        public void OnApplicationQuit()
        {
            Debug.Log("OnApplicationQuit: " + Application.productName);
        }
    }

}