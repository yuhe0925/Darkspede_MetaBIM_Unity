using MetaBIM;
using System;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;


public class ProjectModelHandler : MonoBehaviour
{
    public static ProjectModelHandler Instance;

    [Header("Original Data")]
    public GameObject ModelVersionPrefabs;  
    public List<ModelVersion> modelVersions = new List<ModelVersion>();


    [Header("Interactive Element")]
    public GameObject CurrentHoveringObject;  // include Last one
    public List<GameObject> IsolatedElements = new List<GameObject>();
    //public List<GameObject> HidedElements = new List<GameObject>();
    public List<GameObject> SelectedElements = new List<GameObject>();

    [Header("Zone Buffer")]
    public List<GameObject> SplitedElements = new List<GameObject>();



    [Header("Buffer")]
    public ModelVersion CurrentModel;  // not in use

    public void OnReset()
    {
        CurrentHoveringObject = null;
        IsolatedElements.Clear();
        //HidedElements.Clear();
        SelectedElements.Clear();

    }
        


    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public ModelVersion GetDefaultActiveModel()
    {
        foreach (ModelVersion mv in ProjectModelHandler.Instance.modelVersions)
        {
            if (mv.IsActive)
            {
                return mv;
            }
        }
        
        return null;
    }

    // 
    public ModelVersion GetModelVersion(string _projectGuid)
    {
        foreach (ModelVersion item in modelVersions)
        {
            if(item.Project.guid == _projectGuid)
            {
                return item;
            }
        }

        return null;
    }
    
    public void AddNewModelToHandle(UIBlock_Project_ModelItem _item)
    {
        foreach(ModelVersion mv in modelVersions)
        {
            if(_item.Item.guid == mv.Project.guid)
            {
                return;
            }
        }

        GameObject newMVOB = Instantiate(ModelVersionPrefabs, transform);
       
        ModelVersion newMV = newMVOB.GetComponent<ModelVersion>();
        newMV.SetModelVersion(_item);
        newMV.Viewer = _item;
        newMV.workspaceGuid = AppController.Instance.SelectedWorkspaceGuid;
        newMV.IsActive = false;
        modelVersions.Add(newMV);
        newMV.OnRequestVersionUpdates();
    }
    

    /* this is for shared link */
    public void AddNewModelToHandle(string _workspace, MetaBIM.Version _version, Project _project)
    {

        GameObject newMVOB = Instantiate(ModelVersionPrefabs, transform);

        ModelVersion newMV = newMVOB.GetComponent<ModelVersion>();
        newMV.Project = _project;
        newMV.workspaceGuid = _workspace;
        newMV.laodingVersion = _version;
        newMV.IsActive = false;
        modelVersions.Add(newMV);
        newMV.OnRequestVersionUpdates();
    }


    public bool CheckVersionStatus(UIBlock_Project_ModelItem _item)
    {
        foreach (ModelVersion mv in modelVersions)
        {
            if (_item.Item.guid == mv.Project.guid)
            {
                if (mv.loadingStatus == ModelVersion.ModelLoadingStatus.loaded){
                    return true;
                }
            }
        }

        return false;
    }

    
    public void SetProjectActive()
    {
        
    }


    public void SetProjectVersionSelection()
    {
        
    }



    

    // on check if model can be download, and start the loading progress
    public bool OnModelDownload(string _project, string _version, VersionUpdate _update)
    {
        foreach (ModelVersion item in modelVersions)
        {
            if (item.Project.guid == _project)
            {
                foreach (MetaBIM.Version version in item.Project.versions)
                {
                    if (version.guid == _version)
                    {
                        item.laodingVersion = version;

                        if (_update == null)
                        {
                            item.StartLoading(item.VersionUpdates[0]);  // this maybe course bug if there is no version updates
                        }
                        else
                        {
                            item.StartLoading(_update);
                        }
                        return true;
                    }
                }
            }         
        }
        

        
        
        MCPopup.Instance.SetWarning("No version of this model can be loaded");
        return false;
    }

    // on check if the mode can be cleared
    public bool OnCheckModelForClear(string _project, string _version)
    {
        foreach (ModelVersion item in modelVersions)
        {
            if (item.Project.guid == _project)
            {
                foreach (MetaBIM.Version version in item.Project.versions)
                {
                    if (version.guid == _version)
                    {
                        item.laodingVersion = version;

                        if (item.laodingVersion.processingStatus.ToLower() != "complete")
                        {
                            MCPopup.Instance.SetWarning("Model is processing, please wait it to complete.");
                            return false;
                        }

                        if (item.loadingStatus == ModelVersion.ModelLoadingStatus.loading)
                        {
                            MCPopup.Instance.SetWarning("Can not clear model while it's loading");
                            return false;
                        }
                        else if (item.loadingStatus == ModelVersion.ModelLoadingStatus.loaded)
                        {
                            // set loading status here ?
                            item.loadingStatus = ModelVersion.ModelLoadingStatus.notstart;
                            return true;
                        }
                        else
                        {
                            MCPopup.Instance.SetWarning("Model is not loaded");
                            return false;
                        }

                    }
                }
            }
        }

        MCPopup.Instance.SetWarning("This model can not be clear");
        return false;
    }
    public void OnModelClear(string _project, string _version)
    {
        foreach (ModelVersion item in modelVersions)
        {
            if (item.Project.guid == _project)
            {
                foreach (MetaBIM.Version version in item.Project.versions)
                {
                    if (version.guid == _version)
                    {
                        item.StartClear();
                        MCPopup.Instance.SetComplete("This model is cleared");
                        return;
                    }
                }
            }
        }
    }



    public void OnModelRebuild(string _project, string _version, Action<bool, string> _callback)
    {
        foreach (ModelVersion item in modelVersions)
        {
            if (item.Project.guid == _project)
            {
                foreach (MetaBIM.Version version in item.Project.versions)
                {
                    if (version.guid == _version)
                    {
   
                        return;
                    }
                }
            }
        }
    }

    public List<GameObject> GetModelObjects()
    {
        return new List<GameObject>();
    }

    // Called by page exit
    public void DisableAllModels()
    {
        foreach (ModelVersion item in modelVersions)
        {
            item.DisableModel();
        }
    }

    public void SwitchToGroupStructure(string _structure)
    {
        
    }

    // WIP
    public StructureNode FindIfcStructureItem(string _ifcID)
    {
  
        return null;
    }

    // WIP
    public void SearchStructureList(StructureNode node, string _ifcID, int _Level = 0)
    {
        foreach (StructureNode item in node.childrenNodes)
        {
            if (item.itemID == _ifcID)
            {
                // do some thre
                
                return;
            }
            else
            {
                if (item.childrenNodes.Count > 0)
                {
                    SearchStructureList(item, _ifcID, 1);
                }
            }
        }
    }


    #region Tree Viewer Process

    public List<StructureNode> OnRequestProcessStructureNode_Sptail()
    {
        List<StructureNode> list = new List<StructureNode>();

        foreach (ModelVersion mv in modelVersions)
        {
            if (mv.IsActive)
            {
                mv.bimModel.ProcessObjectHierachy_Sptial();
                list.Add(mv.bimModel.StructureSpatialRoot);
            }
        }

        //Debug.Log("OnRequestProcessStructureNode_Sptail: get model = " + list.Count);
        return list;
    }


    public List<StructureNode> OnRequestProcessStructureNode_Objects()
    {
        List<StructureNode> list = new List<StructureNode>();

        foreach (ModelVersion mv in modelVersions)
        {
            if (mv.IsActive)
            { 
                mv.bimModel.ProcessObjectHierachy_Object();
                list.Add(mv.bimModel.StructureObjectRoot);
            }
        }

        return list;
    }

    public List<StructureNode> OnRequestProcessStructureNode_Class()
    {
        List<StructureNode> list = new List<StructureNode>();

        foreach (ModelVersion mv in modelVersions)
        {
            if (mv.IsActive)
            {
                mv.bimModel.ProcessObjectHierachy_IfcClass();
                list.Add(mv.bimModel.StructureIfcClassRoot);
            }
        }


        return list;
    }


    public List<StructureNode> OnRequestProcessStructureNode_Uniclass()
    {
        List<StructureNode> list = new List<StructureNode>();

        foreach (ModelVersion mv in modelVersions)
        {
            if (mv.IsActive)
            {
                mv.bimModel.ProcessObjectHierachy_Uniclass();
                list.Add(mv.bimModel.StructureUniClassRoot);
            }
        }

        return list;
    }
    public List<StructureNode> OnRequestProcessStructureNode_Custom()
    {
        List<StructureNode> list = new List<StructureNode>();

        foreach (ModelVersion mv in modelVersions)
        {
            if (mv.IsActive)
            {
                mv.bimModel.ProcessObjectHierachy_Uniclass();
                list.Add(mv.bimModel.StructureUniClassRoot);
            }
        }

        return list;
    }

    public List<StructureNode> OnRequestProcessStructureNode_Material()
    {
        List<StructureNode> list = new List<StructureNode>();

        foreach (ModelVersion mv in modelVersions)
        { 
            if (mv.IsActive)
            {
                mv.bimModel.ProcessObjectHierachy_Material();
                list.Add(mv.bimModel.StructureUniClassRoot);
            }
        }

        return list;
    }

    public List<StructureNode> OnRequestProcessStructureNode_Zone()
    {
        List<StructureNode> list = new List<StructureNode>();

        foreach (ModelVersion mv in modelVersions)
        {
            if (mv.IsActive)
            {
                mv.bimModel.ProcessObjectHierachy_Zone();
                list.Add(mv.bimModel.StructureZoneRoot);
            }
        }

        return list;
    }


    public List<StructureNode> OnRequestProcessStructureNode_Room()
    {
        List<StructureNode> list = new List<StructureNode>();

        foreach (ModelVersion mv in modelVersions)
        {
            if (mv.IsActive)
            {
                mv.bimModel.ProcessObjectHierachy_Room();
                list.Add(mv.bimModel.StructureRoomRoot);
            }
        }

        return list;
    }



    public List<StructureNode> OnRequestProcessStructureNode_Validation()
    {
        List<StructureNode> list = new List<StructureNode>();

        foreach (ModelVersion mv in modelVersions)
        {
            if (mv.IsActive)
            {
                //mv.bimModel.ProcessObjectHierachy_CodeChecking();
                mv.bimModel.ProcessObjectHierachy_CodeChecking_IfcClass();
                list.Add(mv.bimModel.StructureCodeCheckingRoot);
            }
        }

        return list;
    }



    /* CUSTOM DATA TREE OPERATION */
    
    public void AttacheBIMElementtoStructureNode(StructureNode _node, BIMElement _element)
    {
        _node.element = _element;
    }


    public void AddSelectedElementToTreeNode(StructureNode _node)
    {
        foreach (GameObject item in SelectedElements)
        {
            BIMElement element = item.GetComponent<BIMElement>();
            AttacheBIMElementtoStructureNode(_node, element);
        }
    }

    public void IsolatedActiveModels()
    {

        foreach (ModelVersion mv in modelVersions)
        {
            if (mv.IsActive)
            {
                mv.bimModel.IsolateAllElement();
            }
        }

    }

    public void RestoreActiveModels()
    {

        foreach (ModelVersion mv in modelVersions)
        {
            if (mv.IsActive)
            {
                mv.bimModel.RestoreAllElement();
            }
        }

    }

    /* END */


    public List<BimLevel> GetModelLevel()
    {
        BimModel model = null;
        int actived = 0;

        foreach (ModelVersion mv in modelVersions)
        {
            if (mv.IsActive)
            {
                model = mv.bimModel;
                actived++;
            }
        }

        if (actived == 1 && model != null)
        {
            return model.levels;
        }

        return null;

    }


    public bool GetActiveModel()
    {
        CurrentModel = null;
        int actived = 0;

        foreach (ModelVersion mv in modelVersions)
        {
            if (mv.IsActive)
            {
                CurrentModel = mv;
                actived++;
            }
        }

        if (actived == 1 && CurrentModel != null)
        {
            return true;
        }

        return false;

    }


    public List<BimModel> GetActiveModels()
    {
        List<BimModel> models = new List<BimModel>();

        foreach (ModelVersion mv in modelVersions)
        {
            if (mv.IsActive)
            {
                models.Add(mv.bimModel);
            }
        }

        return models;
    }


    public BimModel GetActiveModelByGuid(string _guid)
    {

        foreach (ModelVersion mv in modelVersions)
        {
            if (mv.IsActive && mv.Project.guid == _guid)
            {
                return mv.bimModel;
            }
        }

        return null;
    }



    public List<string> GetActiveModelGuids()
    {
        List<string> models = new List<string>();

        foreach (ModelVersion mv in modelVersions)
        {
            if (mv.IsActive)
            {
                models.Add(mv.Project.guid);
            }
        }

        return models;
    }



    public Dictionary<string, string> GetActiveModelNames()
    {
        Dictionary<string, string> models = new Dictionary<string, string>();

        foreach (ModelVersion mv in modelVersions)
        {
            if (mv.IsActive)
            {
                models.Add(mv.Project.guid,mv.Project.projectName);
            }
        }

        return models;
    }

    public List<string> GetActiveModelPropertyList()
    {
        List<string> properties = new List<string>();

        foreach (ModelVersion mv in modelVersions)
        {
            if (mv.IsActive)
            {
                 mv.bimModel.GetPropertyList(properties);
            }
        }

        // sort by name
        properties.Sort();
        return properties;
    }


    #endregion



}
