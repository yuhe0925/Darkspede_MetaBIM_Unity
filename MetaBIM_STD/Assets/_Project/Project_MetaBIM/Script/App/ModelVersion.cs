using IfcToolkit;
using IfcToolkit.IfcSpec;
using JetBrains.Annotations;
using MetaBIM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class ModelVersion : MonoBehaviour
{

    [Header("Model Data")]
    public int ID;
    public string workspaceGuid;
    public Project Project;
    public MetaBIM.Version laodingVersion;


    public int ElementCount;
    public int LoadedCount;
    public int DownloadLoadedCount;
    
    public int LoadingUpdateID;

    public Vector3 ModelOffset;
    public int NodeProcessIndex = 0;
    public int ModelSize = 0;

    [Header("UI Element -> Workspace")]
    public UIBlock_Project_ModelItem Viewer;

    [Header("Splited Elements")]
    public List<ElementSplit> SplitedElements = new List<ElementSplit>();

    [Header("Linked Object")]
    public GameObject gameObjectRoot;
    public BimModel bimModel;  
    AssetBundle bundle;



    [Header("Operation Data")]
    public int VersionUpdateStatus = 0;  // 1 is loading , 2 is loaded, 3 is modify not saved, 4 error
    public string MasterVersion = "";
    public List<VersionUpdate> VersionUpdates;
    public VersionUpdate CurrentLoadedVersionUpdate;



    // add zone updated

    // add export items

    // add 


    [Header("Status")]
    public bool IsActive = false;
    public ModelLoadingStatus loadingStatus = ModelLoadingStatus.notstart;


    #region BUFFER

    #endregion

    public void OnEnable()
    {
        if(loadingStatus == ModelLoadingStatus.loaded)
        {
            if (Viewer != null)
            {
                Viewer.SetComplete();
                Viewer.IsModelLoaded = true;
            }
        }
    }

    // or contuin loading
    public void StartLoading(VersionUpdate _update)
    {
        CurrentLoadedVersionUpdate = _update;

        if (laodingVersion.processingStatus.ToLower() !=  "complete")
        {
            MCPopup.Instance.SetWarning("Model is processing, please wait it to complete.");
            return;
        }


        if (loadingStatus == ModelLoadingStatus.loading)
        {
            MCPopup.Instance.SetWarning("Model is downloading, please wait it to complete.");
        }
        else if (loadingStatus == ModelLoadingStatus.notstart)
        {
            //MCPopup.Instance.SetInformation("Start model loading, you can open model after loading is complete");
            loadingStatus = ModelLoadingStatus.loading;
            ModelSize = 0;
            OnStartLoadingAsset(laodingVersion.versionID);
        }
        else
        {
            MCPopup.Instance.SetWarning("Model is downloaded, clear it to download again");
        }
    }


    public void StartClear()
    {
        if(gameObjectRoot != null)
        {
            Destroy(gameObjectRoot);
            GC.Collect();
        } 

        if(bundle != null)
        {
            bundle.Unload(true);
        }
        
        bundle = null;
        gameObjectRoot = null;
        bimModel = null;
        loadingStatus = ModelLoadingStatus.notstart;
        Viewer.IsModelLoaded = false;
        if (Viewer != null)
        {
            Viewer.SetCleared();
        }
    }

    

    public void OnRequestVersionUpdates()
    {
        VersionUpdateStatus = 1;
        StartCoroutine(DataProxy.Instance.GetVersionUpdates("attachedProject", Project.guid, OnRequestVersionUpdates_Callback));
    }

    public void OnRequestVersionUpdates_Callback(bool _result, string _message)
    {
        if (_result)
        {

            DataProxyResponse<VersionUpdate> payload = JsonUtility.FromJson<DataProxyResponse<VersionUpdate>>(_message);

            if (payload.result)
            {
                VersionUpdateStatus = 2;
                VersionUpdates = payload.package;
            }
            else
            {
                VersionUpdateStatus = 4;
            }

        }
        else
        {
            VersionUpdateStatus = 5;
        }
    }


    public void OnLoadingAssetCallback_Complete()
    {
        loadingStatus = ModelLoadingStatus.loaded;
        Viewer.IsModelLoaded = true;
        if (Viewer != null)
        {
            Viewer.SetComplete();
        }
    }


    public string GetAssetURLByUpdateID(int _updateID)
    {
        return Config.Workspace_Path + workspaceGuid + "/" + Project.guid + "/" + laodingVersion.guid + "/";
    }

    public void OnStartLoadingAsset(int _updateID)
    {
        loadingStatus = ModelLoadingStatus.loading;

        StartCoroutine(DataProxy.Instance.OnLoadAsset(
            GetAssetURLByUpdateID(_updateID),
            laodingVersion.guid,
            OnLoadingAssetCallback_Download, 
            OnLoadingAssetCallback_Load, 
            OnLoadingAssetCallback_Complete));
    }

    public void OnLoadingAssetCallback_Download(float _progress)
    {
        if (Viewer != null)
        {
            Viewer.SetLoadingProgress(_progress / 2, "Downloading ");
        }
        
        if (AppController.Instance.CurrentShareLinkPackage.shared)
        {
            AppController.Instance.CurrentShareLinkPackage.LoadingProgress(_progress / 2);
        }
    }

    public void OnLoadingAssetCallback_Load(float _progress)
    {
        if (Viewer != null)
        {
            Viewer.SetLoadingProgress(_progress + 0.5f, "Processing ");
        }
        
        if (AppController.Instance.CurrentShareLinkPackage.shared)
        {
            AppController.Instance.CurrentShareLinkPackage.LoadingProgress(_progress + 0.5f);
        }
    }

    public void OnLoadingAssetCallback_Complete(bool _result, string _message, GameObject _gameObject, AssetBundle _bundle)
    {
        if (_result)
        {
            if (Viewer != null)
            {
                Viewer.SetProgressMessage("Getting Ready");
            }
            bundle = _bundle;
            StartCoroutine(LoadAssetIntoScene(_gameObject, InstantiateAsset_Callback));
        }
        else
        {
            MCPopup.Instance.SetWarning("Model Loading Fail");
        }
    }




    public IEnumerator LoadAssetIntoScene(GameObject _prefab, Action<GameObject> _Callback)
    {
        GameObject instance = Instantiate(_prefab);
        yield return new WaitForSeconds(1f);
        _Callback(instance);
    }


    public void InstantiateAsset_Callback(GameObject _instance)
    {
        gameObjectRoot = _instance;
        bimModel = gameObjectRoot.GetComponent<BimModel>();
        if (Viewer != null)
        {
            Viewer.SetComplete();
        }
        loadingStatus = ModelLoadingStatus.loaded;
        Viewer.IsModelLoaded = true;
        gameObjectRoot.name = laodingVersion.originalFileName.Replace(".ifc", "");
        bimModel.StructureSpatialRoot.Content = gameObjectRoot.name;  // need to fix this in the processing server


        Bounds bound = getBounds(gameObjectRoot);
        bimModel.CombinedBound = bound;

        // may need to process else where
        bimModel.ProcessLevelBound();
        DisableModel();

        OnRequest_GetElementSplitByModel();

        if (AppController.Instance.CurrentShareLinkPackage.shared)
        {
            AppController.Instance.CurrentShareLinkPackage.CompleteCallback(true);
        }
    }

    
    public int SelectedVersion()
    {
        return Viewer.Dropdown_VersionSelection.value = 0;
    }




    public void SetModelVersion(UIBlock_Project_ModelItem _item)
    {
        Viewer = _item;
        Project = _item.Item;

      
    }

    public void SelectVersion(int _version)
    {
        laodingVersion = Project.versions[_version];
    }

    public float GetUpdates()
    {
        return 0;
    }

    public List<GameObject> GetModelObjects()
    {
        return gameObjectRoot.GetComponent<IfcRootLists>().ifcGameObject;
    }


    public ElementSplit GetSplitElmentByGuid(string _elementID)
    {
        foreach(var element in SplitedElements)
        {
            if(element.elementID == _elementID)
            {
                return element;
            }
        }

        return null;
    }


    public List<BimMaterial> bimMaterials = new List<BimMaterial>();

    public void CreateMaterialList()
    {
        bimMaterials = new List<BimMaterial>();
        foreach (GameObject ob in gameObjectRoot.GetComponent<IfcRootLists>().ifcGameObject)
        {

            BIMElement element = ob.GetComponent<BIMElement>();

            //string materialName = element.MaterialName;
            //string objectName = element.ObjectName;

            // get the material name of 
        }
    }



    public int SearchMateriaiList(string _matName, string _objName)
    {
        int index = 0;
        foreach(var item in bimMaterials)
        {
            if(item.materialName == _matName && item.elementName == _objName)
            {
                return index;
            }
        }

        return -1;
    }




    public void AddElementSplit(ElementSplit _element)
    {
        SplitedElements.Add(_element);
    }


    // get all current metabim element from the model version
    public void OnRequest_GetElementSplitByModel()
    {
        string guid = Project.guid;

        StartCoroutine(DataProxy.Instance.GetElementSplits("attachedProject", guid, OnRequest_GetElementSplitByModel_Callback));

    }


    public void OnRequest_GetElementSplitByModel_Callback(bool _result, string _message)
    {
        if (_result)
        {
            DataProxyResponse<ElementSplit> payload = JsonUtility.FromJson<DataProxyResponse<ElementSplit>>(_message);

            if (payload.result)
            {
                SplitedElements = payload.package;

                Debug.Log("OnRequest_GetElementSplitByModel_Callback: Split Element Loaded: " + SplitedElements.Count);
            }
        }
    }



    // on request update the element
    public void OnRequestUpdate_ElementSplit(ElementSplit _element)
    {
        StartCoroutine(DataProxy.Instance.UpdateElementSplit(ElementSplit.ToJson(_element ), OnRequestUpdate_ElementSplit_Callback)) ;
    }

    public void OnRequestUpdate_ElementSplit_Callback(bool _result, string _message)
    {
        //TODO, add sync notice somehow to let use know the update is complete
        Debug.Log("OnRequestUpdate_ElementSplit_Callback:" + _message);
        LoadingHandler.Instance.OnFullPageLoadingEnd();

        if (_result)
        {
             // request for new split element
        }
    }



    #region viewer operation
    public void EnableModel()
    {
        if (bimModel != null)
        {
            IsActive = true;
            bimModel.gameObject.SetActive(true); // this take time
        }
    }

    public void DisableModel()
    {
        if (bimModel != null)
        {
            IsActive = false;
            bimModel.gameObject.SetActive(false);
        }
    }
    #endregion



    public bool FindLoadedElement(string _guild)
    {
 


        return false;
    }





    public GameObject FindGameObjectByIfcID(string _ifcID)
    {

        return null;
    }



    public enum ModelLoadingStatus
    {
        notstart = 0,
        loading = 1,
        loaded = 2,
    }

    Bounds getBounds(GameObject objeto)
    {
        Bounds bounds;
        Renderer childRender;
        bounds = getRenderBounds(objeto);
        if (bounds.extents.x == 0)
        {
            bounds = new Bounds(objeto.transform.position, Vector3.zero);
            foreach (Transform child in objeto.transform)
            {
                childRender = child.GetComponent<Renderer>();
                if (childRender)
                {
                    bounds.Encapsulate(childRender.bounds);
                }
                else
                {
                    bounds.Encapsulate(getBounds(child.gameObject));
                }
            }
        }
        return bounds;
    }

    Bounds getRenderBounds(GameObject objeto)
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        Renderer render = objeto.GetComponent<Renderer>();
        if (render != null)
        {
            return render.bounds;
        }
        return bounds;
    }
}
