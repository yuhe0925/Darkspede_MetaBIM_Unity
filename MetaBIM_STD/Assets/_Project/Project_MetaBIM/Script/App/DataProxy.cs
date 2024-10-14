using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using System.IO;
using UnityEditor;

namespace MetaBIM
{
    [ExecuteInEditMode]
    public class DataProxy : MonoBehaviour
    {
        public static DataProxy Instance;

        public static DataProxy GetInstace()
        {
            if (Instance == null)
            {
                Debug.Log("DataProxy.GetInstace: ");
                Instance = GameObject.Find("DataProxy").GetComponent<DataProxy>();
            }

            return Instance;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }


        [Header("Status")]
        public bool IsLoadingPanelActive = false;


        public IEnumerator OnLoadWebImage(string URL, System.Action<bool, string, Texture> _Callback)
        {
            //

            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(URL))
            {
                // Wait for download to complete
                yield return www.SendWebRequest();

                while (!www.isDone)
                {
                    yield return www;
                }


                if (www.isNetworkError || www.isHttpError)
                {
                    //
                    _Callback(false, www.error, null);
                }
                else
                {
                    //
                    Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    _Callback(true, "", myTexture);
                }
            }
        }
        public string OnRequestGoogleStaticMapImageUrl(string _address, int _zoom = 18)
        {
            string url = Config.API_GoogleStaticMap +
                AddRequest("key", Config.API_GoogleAPIKey) +
                AddRequest("center", _address) +
                AddRequest("markers", _address) +
                AddRequest("format", "jpg") +
                AddRequest("maptype", "hybrid") +
                AddRequest("zoom", _zoom.ToString()) +
                AddRequest("size", "800x600", true);
            return url;
        }
        private string AddRequest(string _key, string _value, bool isEnd = false)
        {
            return _key + "=" + _value + (isEnd ? "" : "&");
        }







        #region General Request
        // System Request
        public IEnumerator OnRequestLoginByToken(string _guid, string _token, System.Action<bool, string> _Callback)
        {
            Package package = new Package();
            package.profileGuid = _guid;
            package.key = Config.API_Key;
            package.target = "Profile";
            package.filters.Add(new Filter("token", _token));

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());
            

            string link = Config.API_Domain + Config.API_OnRequestLoginByToken;

            Debug.Log("DataProxy.OnRequestLoginByToken" + MethodBase.GetCurrentMethod().Name + ": " + link); 
            Debug.Log(package.ToJson());

            

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {

                yield return www.SendWebRequest();

                while (!www.isDone)
                {
                    yield return www;
                }

                
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log("[ERROR]DataProxy.OnRequestLoginByToken: " + www.error);
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }


            }
        }

        public IEnumerator OnRequestAuthCodeByMobile(string _mobile, System.Action<bool, string> _Callback)
        {
            Package package = new Package();
            package.profileGuid = "guest";
            package.key = Config.API_Key;
            package.target = "Profile";
            package.filters.Add(new Filter("mobile", _mobile));

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_OnRequestAuthCodeByMobile;

            //Debug.Log("DataProxy.OnRequestAuthCodeByMobile " + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());

            

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {

                yield return www.SendWebRequest();

                while (!www.isDone)
                {
                    yield return www;
                }

                

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log("[ERROR]DataProxy.OnRequestAuthCodeByMobile: " + www.error);
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator OnRequestLoginByMobile(string _mobile, string _authCode, string _token, System.Action<bool, string> _Callback)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Profile";
            package.filters.Add(new Filter("mobile", _mobile));
            package.filters.Add(new Filter("authCode", _authCode));
            package.filters.Add(new Filter("token", _token));

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_OnRequestLoginByMobile;

            //Debug.Log("DataProxy.OnRequestLoginByMobile" + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            //Debug.Log(package.ToJson());

            


            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {

                yield return www.SendWebRequest();

                while (!www.isDone)
                {
                    yield return www;
                }

                

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log("[ERROR]DataProxy.OnRequestLoginByMobile: " + www.error);
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator OnProcessRequest(string _itemGuid, string _action, string _role, System.Action<bool, string> _Callback)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Request";
            package.itemGuid = _itemGuid;
            package.filters.Add(new Filter("action", _action));
            package.filters.Add(new Filter("role", _role));



            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());


            string link = Config.API_Domain + Config.API_OnProcessRequest;

            Debug.Log("DataProxy.OnProcessRequest" + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            Debug.Log(package.ToJson());

            

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {

                yield return www.SendWebRequest();

                while (!www.isDone)
                {
                    yield return www;
                }

                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }


            }
        }

        public IEnumerator OnLoadWebImage(string URL, Image _Image, UnityAction _CallbackDone, UnityAction<string> _CallbackError)
        {
            yield return null;
        }

        public IEnumerator OnLoadAsset(string _assetURLPrefix, string _assetName, Action<float> _downloadProgressCallback,Action<float> _loadProgressCallback, Action<bool, string, GameObject, AssetBundle> _completeCallback)
        {
#if (UNITY_WEBGL && !UNITY_EDITOR)
            string url = _assetURLPrefix + _assetName + "_webgl.unity3d";
#else
            string url = _assetURLPrefix + _assetName + "_standalone.unity3d";
#endif

            bool result = false;
            string message = "none";
            GameObject obj = null;
            AssetBundle bundle = null;
            
            //Debug.Log("Download Asset: " + url);
            UnityWebRequest bundleRequest = UnityWebRequestAssetBundle.GetAssetBundle(url,0,0);
            UnityWebRequestAsyncOperation operation = bundleRequest.SendWebRequest();

            //Debug.Log("Start Downloading: " + url);
            while (!operation.isDone)
            {
                _downloadProgressCallback(operation.progress);
                yield return new WaitForEndOfFrame();
            }

            if (operation.webRequest.result != UnityWebRequest.Result.Success)
            {
                message = "Asset not downloaded";
                goto _completeCallback;
            }
            
            yield return new WaitForEndOfFrame();

            bundle = DownloadHandlerAssetBundle.GetContent(bundleRequest);
            AssetBundleRequest asset = bundle.LoadAssetAsync(_assetName);
            
            //Debug.Log("Start Loading Bundle: ");
            yield return new WaitForEndOfFrame();

            while (!asset.isDone)
            {
                _loadProgressCallback(asset.progress);
                yield return new WaitForEndOfFrame();
            }

            obj = asset.asset as GameObject;

            bundle.Unload(false);
            bundleRequest.Dispose();
            
            result = true;
            message = "Asset Loading Complete";
            
            _completeCallback:
            _completeCallback(result, message, obj, bundle);
 
        }

        public IEnumerator LoadXML(string _url, System.Action<bool, string, Action> _Callback, Action _action)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(_url))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(www.result.ToString());
                    _Callback(false, www.error, _action);
                }
                else
                {
                    string xmlData = www.downloadHandler.text;

                    _Callback(true, xmlData, _action);
                }
            }
        }

        public IEnumerator LoadXML(string _url, System.Action<bool, string> _Callback)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(_url))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(www.result.ToString());
                    _Callback(false, www.error);
                }
                else
                {
                    string xmlData = www.downloadHandler.text;

                    _Callback(true, xmlData);
                }
            }
        }

        public IEnumerator OnRequestDownloadRevitPlugin(string _guid, System.Action<bool, string> _Callback = null)
        {
            WWWForm form = new WWWForm();
            form.AddField("key", Config.API_Key);
            form.AddField("guid", _guid);


            string link = Config.API_Domain + Config.API_OnRequestDownloadRevitPlugin;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link);

            

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        #endregion


        #region Profile
        public IEnumerator AddProfile(string _item, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Profile";
            package.package = _item;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_AddProfile;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());

            


            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator UpdateProfile(string _package, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Profile";
            package.package = _package;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_ModifyProfile;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());

            

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetProfileByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Profile";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_GetProfileByGuid;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            //

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                //

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetProfiles(string _filterName, string _filterValue, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Profile";
            package.filters.Add(new Filter(_filterName, _filterValue));


            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_GetProfiles;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            



            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator DeleteProfileByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Profile";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());


            string link = Config.API_Domain + Config.API_DeleteProfileByGuid;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        #endregion


        #region Workspace
        public IEnumerator AddWorkspace(string _item, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Workspace";
            package.package = _item;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_AddWorkspace;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());

            


            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator UpdateWorkspace(string _package, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Workspace";
            package.package = _package;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_ModifyWorkspace;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetWorkspaceByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Workspace";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_GetWorkspaceByGuid;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            //

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                //

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetWorkspaces(string _filterName, string _filterValue, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Workspace";
            package.filters.Add(new Filter(_filterName, _filterValue));


            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_GetWorkspaces;

            //Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            
   



            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    Debug.Log("progress: " + www.downloadProgress);
                    yield return www;
                }


                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator DeleteWorkspaceByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Workspace";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());


            string link = Config.API_Domain + Config.API_DeleteWorkspaceByGuid;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());


            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator OnRequestExampleWorkspace(System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Workspace";

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_OnRequestExampleWorkspace;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());

            


            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }



        #endregion


        #region BimObject (TODO has bugs)
        public IEnumerator AddBimObject(string _item, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "BimObject";
            package.package = _item;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_AddBimObject;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());

            


            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator UpdateBimObject(string _package, string _workspaceGuid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = _workspaceGuid;
            package.package = _package;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_ModifyBimObject;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetBimObjectByGuid(string _guid, string _projectGuid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = _projectGuid;
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_GetBimObjectByGuid;

            //Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            //

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                //

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetBimObjects(string _collectionName, string _filterName, string _filterValue, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = _collectionName;
            package.filters.Add(new Filter(_filterName, _filterValue));


            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_GetBimObjects;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            



            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator DeleteBimObjectByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "BimObject";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());


            string link = Config.API_Domain + Config.API_DeleteBimObjectByGuid;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        #endregion


        #region Request
        public IEnumerator AddRequest(string _item, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Request";
            package.package = _item;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_AddRequest;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());

            


            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator UpdateRequest(string _package, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Request";
            package.package = _package;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_ModifyRequest;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetRequestByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Request";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_GetRequestByGuid;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            //

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                //

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetRequests(string _filterName, string _filterValue, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Request";
            if (_filterName != "")
            {
                package.filters.Add(new Filter(_filterName, _filterValue));
            }


            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_GetRequests;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            



            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator DeleteRequestByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Request";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());


            string link = Config.API_Domain + Config.API_DeleteRequestByGuid;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        #endregion


        #region VersionUpdate
        public IEnumerator AddVersionUpdate(string _item, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "VersionUpdate";
            package.package = _item;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_AddVersionUpdate;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());

            


            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator UpdateVersionUpdate(string _package, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "VersionUpdate";
            package.package = _package;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_ModifyVersionUpdate;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetVersionUpdateByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "VersionUpdate";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_GetVersionUpdateByGuid;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            //

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                //

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetVersionUpdates(string _filterName, string _filterValue, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "VersionUpdate";
            package.filters.Add(new Filter(_filterName, _filterValue));


            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_GetVersionUpdates;

            //Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());

            



            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    Debug.Log("progress: " + www.downloadProgress);
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator DeleteVersionUpdateByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "VersionUpdate";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());


            string link = Config.API_Domain + Config.API_DeleteVersionUpdateByGuid;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

    

        #endregion


        #region Classification

        public IEnumerator OnRequestUniclassUpdate(string _filterName, string _filterValue, System.Action<bool, string, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Uniclass";
            if (_filterName != "")
            {
                package.filters.Add(new Filter(_filterName, _filterValue));
            }


            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_OnRequestUniclassUpdate;

            //Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            



            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, _filterValue, "Network Error");
                }
                else
                {
                    _Callback(true, _filterValue, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator OnRequestIFCclassUpdate(string _filterName, string _filterValue, System.Action<bool, string, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "ifcclass";
            if (_filterName != "")
            {
                package.filters.Add(new Filter(_filterName, _filterValue));
            }


            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_OnRequestIfcclassUpdate;

            //Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            



            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, _filterValue, "Network Error");
                }
                else
                {
                    _Callback(true, _filterValue, www.downloadHandler.text);
                }
            }
        }


        public IEnumerator OnRequestEpicClassUpdate(string _filterName, string _filterValue, System.Action<bool, string, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "epicclass";
            if (_filterName != "")
            {
                package.filters.Add(new Filter(_filterName, _filterValue));
            }


            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_OnRequestEpicClassUpdate;

            //Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); 
            //Debug.Log(package.ToJson());
            



            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, _filterValue, "Network Error");
                }
                else
                {
                    _Callback(true, _filterValue, www.downloadHandler.text);
                }
            }
        }


        #endregion


        #region Model

        /// <summary>
        /// A native method of upload model file from PC build runtime
        /// </summary>
        /// <param name="_package"></param>
        /// <param name="_Callback"></param>
        /// <returns></returns>
        public IEnumerator OnUploadModelVersionNative(string _workspace, string _project, string _profile, string _convertionAction, string _filePath, string _fileBytes, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "version";
            package.itemGuid = _workspace;
            package.profileGuid = _profile;

            package.filters.Add(new Filter("workspaceGuid", _workspace));
            package.filters.Add(new Filter("projectGuid", _project));
            package.filters.Add(new Filter("fileName", Path.GetFileName(_filePath)));
            package.filters.Add(new Filter("convertionAction", _convertionAction));

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());
            form.AddField("file", _fileBytes);

            string link = Config.API_Domain + Config.API_OnUploadModelVersionNative;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    Debug.Log(www.uploadProgress);
                    yield return www;
                }

                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        #endregion


        #region ElementSplit

        public IEnumerator AddElementSplit(string _item, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "ElementSplit";
            package.package = _item;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_AddElementSplit;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());

            


            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator UpdateElementSplit(string _package, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "ElementSplit";
            package.package = _package;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_ModifyElementSplit;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetElementSplitByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "ElementSplit";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_GetElementSplitByGuid;

            //Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            //

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                //

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetElementSplits(string _filterName, string _filterValue, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "ElementSplit";
            package.filters.Add(new Filter(_filterName, _filterValue));


            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_GetElementSplits;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            



            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator DeleteElementSplitByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "ElementSplit";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());


            string link = Config.API_Domain + Config.API_DeleteElementSplitByGuid;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        #endregion


        #region ElementZone
        public IEnumerator AddElementZone(string _item, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "ElementZone";
            package.package = _item;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_AddElementZone;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());

            


            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator UpdateElementZone(string _package, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "ElementZone";
            package.package = _package;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_ModifyElementZone;

            //Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetElementZoneByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "ElementZone";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_GetElementZoneByGuid;

            //Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            //

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                //

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetElementZones(string _filterName, string _filterValue, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "ElementZone";
            package.filters.Add(new Filter(_filterName, _filterValue));


            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_GetElementZones;

            //Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            



            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator DeleteElementZoneByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "ElementZone";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());


            string link = Config.API_Domain + Config.API_DeleteElementZoneByGuid;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        #endregion


        #region Supplier
        public IEnumerator AddSupplier(string _item, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Supplier";
            package.package = _item;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_AddSupplier;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());

            


            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator UpdateSupplier(string _package, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Supplier";
            package.package = _package;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_ModifySupplier;

            //Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetSupplierByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Supplier";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_GetSupplierByGuid;

            //Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            //

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                //

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetSuppliers(string _filterName, string _filterValue, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Supplier";
            package.filters.Add(new Filter(_filterName, _filterValue));


            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_GetSuppliers;

            //Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            



            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator DeleteSupplierByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "Supplier";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());


            string link = Config.API_Domain + Config.API_DeleteSupplierByGuid;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }
                

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        #endregion


        #region RemoteCommand
        public IEnumerator AddRemoteCommand(string _item, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "RemoteCommand";
            package.package = _item;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_AddRemoteCommand;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }


                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator UpdateRemoteCommand(string _package, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "RemoteCommand";
            package.package = _package;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_ModifyRemoteCommand;

            //Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());


            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }



                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetRemoteCommandByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "RemoteCommand";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_GetRemoteCommandByGuid;

            //Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            //

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                //

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetRemoteCommands(string _filterName, string _filterValue, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "RemoteCommand";
            package.filters.Add(new Filter(_filterName, _filterValue));


            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_GetRemoteCommands;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());




            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }


                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator DeleteRemoteCommandByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "RemoteCommand";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());


            string link = Config.API_Domain + Config.API_DeleteRemoteCommandByGuid;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());


            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }


                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        #endregion

        #region CustomStructure
        public IEnumerator AddCustomStructure(string _item, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "CustomStructure";
            package.package = _item;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_AddCustomStructure;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());




            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }


                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator UpdateCustomStructure(string _package, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "CustomStructure";
            package.package = _package;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_ModifyCustomStructure;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());


            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }



                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetCustomStructureByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "CustomStructure";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_GetCustomStructureByGuid;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            //

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                //

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetCustomStructures(string _filterName, string _filterValue, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "CustomStructure";
            package.filters.Add(new Filter(_filterName, _filterValue));


            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.API_Domain + Config.API_GetCustomStructures;

            //Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());





            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    Debug.Log("progress: " + www.downloadProgress);
                    yield return www;
                }


                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator DeleteCustomStructureByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "CustomStructure";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());


            string link = Config.API_Domain + Config.API_DeleteCustomStructureByGuid;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());


            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }


                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }


        #endregion



        #region EasycarbonProject
        public IEnumerator AddEasycarbonProject(string _item, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "EasycarbonProject";
            package.package = _item;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.Domain_EasyCarbon+ "api/client/" + Config.API_AddEasycarbonProject;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }


                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator UpdateEasycarbonProject(string _package, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "EasycarbonProject";
            package.package = _package;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.Domain_EasyCarbon+ "api/client/" + Config.API_ModifyEasycarbonProject;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());


            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }



                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetEasycarbonProjectByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "EasycarbonProject";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.Domain_EasyCarbon+ "api/client/" + Config.API_GetEasycarbonProjectByGuid;

            //Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());
            //

            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }

                //

                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator GetEasycarbonProjects(string _filterName, string _filterValue, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "EasycarbonProject";
            package.filters.Add(new Filter(_filterName, _filterValue));


            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());

            string link = Config.Domain_EasyCarbon+ "api/client/" + Config.API_GetEasycarbonProjects;

            //Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());




            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }


                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        public IEnumerator DeleteEasycarbonProjectByGuid(string _guid, System.Action<bool, string> _Callback = null)
        {
            Package package = new Package();
            package.profileGuid = AppController.GetInstance().CurrentProfile.guid == null ? "guest" : AppController.GetInstance().CurrentProfile.guid;
            package.key = Config.API_Key;
            package.target = "EasycarbonProject";
            package.itemGuid = _guid;

            WWWForm form = new WWWForm();
            form.AddField("package", package.ToJson());


            string link = Config.Domain_EasyCarbon+ "api/client/" + Config.API_DeleteEasycarbonProjectByGuid;

            Debug.Log("DataProxy." + MethodBase.GetCurrentMethod().Name + ": " + link); Debug.Log(package.ToJson());


            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {
                yield return www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return www;
                }


                if (www.isNetworkError || www.isHttpError)
                {
                    _Callback(false, "Network Error");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text);
                }
            }
        }

        #endregion
    }

}
