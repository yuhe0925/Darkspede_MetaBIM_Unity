using IfcToolkit;
using MetaBIM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Page_BIMCompare : MonoBehaviour
{
    public static Page_BIMCompare Instance;
    public PanelChange Panel;
    public XMLController XML;

    public Project CurrentProject;


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
        //Camera_Main.enabled = false;
        Camera_Main.cullingMask = MainCameraDisableMask;

        //Camera_Main.transform.gameObject.GetComponent<RenderReplacementShaderToTexture>().OnRequestClear();

        Camera_CMP.gameObject.SetActive(true);
        Camera_ORG.gameObject.SetActive(true);
        Camera_CMP.enabled = true;
        Camera_ORG.enabled = true;



        //MARK, TODO: need better way of getting project
        CurrentProject = Page_Project.Instance.CurrentProject;
        //ScrollViewer_CompareItems.LoadData(IfcCompareNodeList);
        // loading convertion request
        // rendering convertion version
        RenderConvertionItem();
        CompareNodeType = "All";
        isCameraSynced = false;
    }


    public void OnCloseAction()
    {
        //Camera_Main.enabled = true;
        Camera_Main.cullingMask = MainCameraEnableMask;
        //Camera_Main.transform.gameObject.GetComponent<RenderReplacementShaderToTexture>().OnRequestInit();
        Camera_CMP.enabled = false;
        Camera_ORG.enabled = false;
        Camera_CMP.gameObject.SetActive(false);
        Camera_ORG.gameObject.SetActive(false);
        Destroy(LoadedSourceAsset);
        Destroy(LoadedTargetAsset);

        isCameraSynced = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        VersionItemPrefab.SetActive(false);
        SourceAttributeItemPrefab.SetActive(false);
        TargetAttributeItemPrefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        ConfigDuelCamera();

        SyncCameraTransform();
    }


    [Header("Camera Setup")]
    public Camera Camera_ORG;
    public RectTransform TargetRectTransform_ORG;
    public Camera Camera_CMP;
    public RectTransform TargetRectTransform_CMP;

    public Camera Camera_Main;
    public Canvas TargetCanvas;

    public LayerMask MainCameraDisableMask;
    public LayerMask MainCameraEnableMask;

    public Vector2 ViewPortOffset = new Vector2();
    private Rect TargetRect = new Rect();

    public void ConfigDuelCamera()
    {

        // Setup ORG Camera
        TargetRect = new Rect();
        TargetRect.x = ViewPortOffset.x * TargetCanvas.scaleFactor / Screen.width;
        TargetRect.y = (TargetRectTransform_ORG.sizeDelta.y / 2 - ViewPortOffset.y) / (Screen.height / TargetCanvas.scaleFactor);
        TargetRect.width = TargetRectTransform_ORG.sizeDelta.x / Screen.width * TargetCanvas.scaleFactor;
        TargetRect.height = TargetRectTransform_ORG.sizeDelta.y / Screen.height * TargetCanvas.scaleFactor;
        Camera_ORG.rect = TargetRect;

        // Setup CMP Camera
        TargetRect = new Rect();
        TargetRect.x = (ViewPortOffset.x + TargetRectTransform_ORG.sizeDelta.x) * TargetCanvas.scaleFactor / Screen.width;
        TargetRect.y = (TargetRectTransform_CMP.sizeDelta.y / 2 - ViewPortOffset.y) / (Screen.height / TargetCanvas.scaleFactor);
        TargetRect.width = TargetRectTransform_CMP.sizeDelta.x / Screen.width * TargetCanvas.scaleFactor;
        TargetRect.height = TargetRectTransform_CMP.sizeDelta.y / Screen.height * TargetCanvas.scaleFactor;
        Camera_CMP.rect = TargetRect;


    }



    [Header("Camera Operation")]
    public bool isCameraSynced = false;
    public bool isORGCameraSelected = false;
    public bool isCMPCameraSelected = false;

    public FreeCameraNav FreeCamera_ORG;
    public FreeCameraNav FreeCamera_CMP;

    public void OnClick_SetCameraInSync()
    {
        isCameraSynced = true;
    }

    public void OnClick_DisableCameraInSync()
    {
        isCameraSynced = false;
    }

    public void OnClick_SwithCamera()
    {

    }

    public void SyncCameraTransform()
    {
        if (isCameraSynced)
        {
            if (FreeCamera_ORG.isMouseInViewPort)
            {
                Camera_CMP.transform.position = Camera_ORG.transform.position;
                Camera_CMP.transform.rotation = Camera_ORG.transform.rotation;
            }
            else
            if (FreeCamera_CMP.isMouseInViewPort)
            {
                Camera_ORG.transform.position = Camera_CMP.transform.position;
                Camera_ORG.transform.rotation = Camera_CMP.transform.rotation;
            }
        }

    }

    public void OnClick_RequestXmlCompare()
    {
        XML.OnRequestCompareXml("1d8e6297-e63f-4e0f-8788-dce3e32ccd19", "3e08ed2f-7813-443a-8b56-150a1e283858");
        //XML.OnRequestCompareXml("testSource", "testTarget");

    }

    public void OnClick_SetHome_View_CRP()
    {
        //Vector3 target = Page_BIMViewer.Instance.SpatialRootItem.BoundCenter;

       // Camera_CMP.transform.LookAt(target);
    }

    public void OnClick_SetHome_View_ORG()
    {
        //Vector3 target = Page_BIMViewer.Instance.SpatialRootItem.BoundCenter;

       // Camera_ORG.transform.LookAt(target);
    }

    [Header("UI Interaction")]
    public GameObject SourceVersion;
    public GameObject SelectedVersion;
    public GameObject SelectedCompareItem;

    public List<GameObject> VersionItems;
    public GameObject VersionItemPrefab;
    public Transform VersionItemParent;


    public GameObject SourceAttributeItemPrefab;
    public List<GameObject> SourceAttributeItems;
    public Transform SourceAttributeItemParent;

    public GameObject TargetAttributeItemPrefab;
    public List<GameObject> TargetAttributeItems;
    public Transform TargetAttributeItemParent;

    //public ScrollViewer_CompareItems ScrollViewer_CompareItems;


    public Material[] SelectedSourceMaterial;
    public Material[] SelectedTargetMaterial;

    public void RenderConvertionItem()
    {
        ClearObjectList(VersionItems);

        /*
        int index = CurrentProject.convertRequests.Count;
        foreach (ConvertRequest item in CurrentProject.convertRequests)
        {
            index--;
            GameObject ob = Instantiate(VersionItemPrefab, VersionItemParent);
            if (index == 0)
            {
                SourceGuid = item.guid;
                ob.GetComponent<UIBlock_BimCompare_VersionItem>().SetBlock(item, "1." + index, true);
            }
            else
            {
                ob.GetComponent<UIBlock_BimCompare_VersionItem>().SetBlock(item, "1." + index);
            }

            ob.SetActive(true);
            VersionItems.Insert(0, ob);

        }
        */
    }


    public void OnClick_RequestVersionCompare()
    {
        if (SelectedVersion != null)
        {
            MCPopup.Instance.SetConfirm(OnRequestVersionCompare_ConfirmCallback, "", "Process version comparsion");
        }
        else
        {
            MCPopup.Instance.SetWarning("No version selected", "Version");
        }

    }

    public void OnRequestVersionCompare_ConfirmCallback(bool _result, string _message)
    {
        if (_result)
        {

        }
    }

    public void OnClick_SelectVersionItem(GameObject _gameObject)
    {
        if (SelectedVersion != null)
        {
            //SelectedVersion.GetComponent<UIBlock_BimCompare_VersionItem>().OnDeselect();
        }




    }


    public void OnClick_SelectCompareItem(GameObject _gameObject)
    {
        if (SelectedCompareItem != null)
        {
            SelectedCompareItem.GetComponent<UIBlock_BimCompare_CompareItem>().OnDeselect();
            GameObject obs = IfcRootLists_Source.FindIfcGameObject(SelectedCompareItem.GetComponent<UIBlock_BimCompare_CompareItem>().Item.IfcId);
            GameObject obt = IfcRootLists_Target.FindIfcGameObject(SelectedCompareItem.GetComponent<UIBlock_BimCompare_CompareItem>().Item.IfcId);


            if (obs != null && SelectedSourceMaterial != null)
            {
                ChangeMaterial(obs, SelectedSourceMaterial[0]);
            }

            if (obt != null && SelectedTargetMaterial != null)
            {
                ChangeMaterial(obt, SelectedTargetMaterial[0]);
            }
        }

        SelectedCompareItem = _gameObject;
        UIBlock_BimCompare_CompareItem script = SelectedCompareItem.GetComponent<UIBlock_BimCompare_CompareItem>();

        script.OnSelect();

        // Render Selected Compare Item In

        if (script.Item.NodeType == "Added")
        {
            GameObject obt = IfcRootLists_Target.FindIfcGameObject(script.Item.IfcId);

            // Set Source View
            SetAttributeView(null, SourceAttributeItems, SourceAttributeItemPrefab, SourceAttributeItemParent);

            // Set Target View
            SetAttributeView(obt, TargetAttributeItems, TargetAttributeItemPrefab, TargetAttributeItemParent);

            SelectedSourceMaterial = null;

            if (obt.GetComponent<MeshRenderer>() != null)
            {
                SelectedTargetMaterial = obt.GetComponent<MeshRenderer>().materials;
                ChangeMaterial(obt, ResourceHolder.Instance.BIM_OBJECT_SELECTION);
            }
        }
        else if (script.Item.NodeType == "Removed")
        {
            GameObject obs = IfcRootLists_Source.FindIfcGameObject(script.Item.IfcId);

            // Set Source View
            SetAttributeView(obs, SourceAttributeItems, SourceAttributeItemPrefab, SourceAttributeItemParent);

            // Set Target View
            SetAttributeView(null, TargetAttributeItems, TargetAttributeItemPrefab, TargetAttributeItemParent);

            SelectedTargetMaterial = null;

            if (obs.GetComponent<MeshRenderer>() != null)
            {
                SelectedSourceMaterial = obs.GetComponent<MeshRenderer>().materials;
                ChangeMaterial(obs, ResourceHolder.Instance.BIM_OBJECT_SELECTION);
            }

        }
        else if (script.Item.NodeType == "Updated")
        {
            GameObject obs = IfcRootLists_Source.FindIfcGameObject(script.Item.IfcId);
            GameObject obt = IfcRootLists_Target.FindIfcGameObject(script.Item.IfcId);

            CompareIfcGameObejctsElements(obs, obt);
            // Set Source View
            SetAttributeView(obs, SourceAttributeItems, SourceAttributeItemPrefab, SourceAttributeItemParent);

            // Set Target View
            SetAttributeView(obt, TargetAttributeItems, TargetAttributeItemPrefab, TargetAttributeItemParent);
            if (obs.GetComponent<MeshRenderer>() != null)
            {
                SelectedSourceMaterial = obs.GetComponent<MeshRenderer>().materials;
                ChangeMaterial(obs, ResourceHolder.Instance.BIM_OBJECT_SELECTION);
            }

            if (obt.GetComponent<MeshRenderer>() != null)
            {
                SelectedTargetMaterial = obt.GetComponent<MeshRenderer>().materials;
                ChangeMaterial(obt, ResourceHolder.Instance.BIM_OBJECT_SELECTION);
            }
        }
        else if (script.Item.NodeType == "NoChange")
        {
            GameObject obs = IfcRootLists_Source.FindIfcGameObject(script.Item.IfcId);
            GameObject obt = IfcRootLists_Target.FindIfcGameObject(script.Item.IfcId);
            // Set Source View
            SetAttributeView(obs, SourceAttributeItems, SourceAttributeItemPrefab, SourceAttributeItemParent);

            // Set Target View
            SetAttributeView(obt, TargetAttributeItems, TargetAttributeItemPrefab, TargetAttributeItemParent);

            if (obs.GetComponent<MeshRenderer>() != null)
            {
                SelectedSourceMaterial = obs.GetComponent<MeshRenderer>().materials;
                ChangeMaterial(obs, ResourceHolder.Instance.BIM_OBJECT_SELECTION);
            }

            if (obt.GetComponent<MeshRenderer>() != null)
            {
                SelectedTargetMaterial = obt.GetComponent<MeshRenderer>().materials;
                ChangeMaterial(obt, ResourceHolder.Instance.BIM_OBJECT_SELECTION);
            }
        }


        // change material

    }

    /// <summary>
    /// SetAttributeView
    /// </summary>
    /// <param name="meshObject"></param>
    /// <param name="_viewList"></param>
    /// <param name="_prefab"></param>
    /// <param name="_parent"></param>
    public void SetAttributeView(GameObject meshObject, List<GameObject> _viewList, GameObject _prefab, Transform _parent)
    {
        ClearObjectList(_viewList);

        if (meshObject != null)
        {
            Debug.Log("Object Selected: " + meshObject.name);

            // Setup IfcAttributes
            if (meshObject.GetComponent<IfcAttributes>() != null)
            {
                IfcAttributes ifcAttributes = meshObject.GetComponent<IfcAttributes>();

                GameObject attributeHeader = Instantiate(_prefab, _parent);
                attributeHeader.SetActive(true);
                attributeHeader.GetComponent<UIBlock_BimViewer_IfcAttributesItem>().SetAttributeBlock("Attributes", ifcAttributes);
                _viewList.Add(attributeHeader);
            }

            // Setup IfcProperties
            if (meshObject.GetComponent<IfcProperties>() != null)
            {
                IfcProperties ifcProperties = meshObject.GetComponent<IfcProperties>();

                int index = 0;
                int pIndex = 0;
                UIBlock_BimViewer_IfcAttributesItem block = null;

                foreach (string name in ifcProperties.properties)
                {

                    if (name == "PsetName")
                    {
                        GameObject attributeHeader = Instantiate(_prefab, _parent);
                        attributeHeader.SetActive(true);
                        block = attributeHeader.GetComponent<UIBlock_BimViewer_IfcAttributesItem>();
                        block.SetPropertyBlock(ifcProperties.nominalValues[index]);
                        _viewList.Add(attributeHeader);
                        pIndex = 0;
                    }
                    else
                    {
                        if (block != null)
                        {
                            pIndex++;

                            CompareElement element = UpdateProperties.Find(x => x.ifcid == name);

                            if (element != null)
                            {

                                block.AddProperty(name, ifcProperties.nominalValues[index], pIndex, element.diff);
                            }
                            else
                            {
                                block.AddProperty(name, ifcProperties.nominalValues[index], pIndex);
                            }
                        }
                    }

                    index++;
                }
            }

            if (meshObject.GetComponent<IfcMaterials>() != null)
            {
                IfcMaterials ifcMaterials = meshObject.GetComponent<IfcMaterials>();
                GameObject attributeHeader = Instantiate(_prefab, _parent);
                attributeHeader.SetActive(true);
                attributeHeader.GetComponent<UIBlock_BimViewer_IfcAttributesItem>().SetMeterialBlock("Materials", ifcMaterials);
                _viewList.Add(attributeHeader);
            }

            if (meshObject.GetComponent<IfcTypes>() != null)
            {
                IfcTypes ifcTypes = meshObject.GetComponent<IfcTypes>();

                GameObject attributeHeader = Instantiate(_prefab, _parent);
                attributeHeader.SetActive(true);
                attributeHeader.GetComponent<UIBlock_BimViewer_IfcAttributesItem>().SetTypeBlock("Types", ifcTypes);

                _viewList.Add(attributeHeader);
            }


            if (meshObject.GetComponent<IfcQuantities>() != null)
            {
                IfcQuantities ifcQuantities = meshObject.GetComponent<IfcQuantities>();

                GameObject attributeHeader = Instantiate(_prefab, _parent);
                attributeHeader.SetActive(true);
                attributeHeader.GetComponent<UIBlock_BimViewer_IfcAttributesItem>().SetQuantityBlock("Quantites", ifcQuantities);

                _viewList.Add(attributeHeader);
            }

        }

    }


    // Group is Add, removed, updated item
    public void OnClick_SetCompareFilterGroup(GameObject _gameObject)
    {

    }

    public string CompareNodeType = "All";
    // type is by IFC propertiy
    public void OnClick_SetCompareFilterType(string _index)
    {
        CompareNodeType = _index;
        LoadCompareData();
    }

    public void OnClick_SetCompareFilterGroup(string _index)
    {
        LoadCompareData();
    }


    [Header("ScrollViews")]
    public ScrollRect SourceScroll;
    public ScrollRect TargetScroll;


    public void OnValueChange_SourceScrollView(Vector2 _input)
    {
        Debug.Log("Source: " + _input);

        if (isCameraSynced)
        {
            TargetScroll.normalizedPosition = _input;
        }
    }

    public void OnValueChange_TargetScrollView(Vector2 _input)
    {
        Debug.Log("Target: " + _input);

        if (isCameraSynced)
        {

        }
    }

    // Asset loading 

    [Header("Assets")]

    public string SourceGuid = "a47ca387-320b-4ea6-af11-02899d0ffca6";
    public string TargetGuid = "3ba04cb5-8c9a-4f0f-bb37-54f20fa17389";

    public Transform AssetRoot;
    public GameObject LoadedSourceAsset;
    public GameObject LoadedTargetAsset;
    public IfcRootLists IfcRootLists_Source;
    public IfcRootLists IfcRootLists_Target;



    public void StartLoadingAssets()
    {
        LoadSourceAssets(SourceGuid);
    }

    public void LoadSourceAssets(string _fileName)
    {
        //StartCoroutine(DataProxy.Instance.LoadAssetBundle(_fileName, OnLoadSourceAsset_Callback));
    }

    public void OnLoadSourceAsset_Callback(bool _result, string _message, AssetBundle bundle)
    {
        if (_result)
        {
            if (LoadedSourceAsset != null)
            {
                Destroy(LoadedSourceAsset);
            }

            GameObject prefab = bundle.LoadAsset<GameObject>(_message);
            bundle.Unload(false);
            LoadedSourceAsset = Instantiate(prefab, AssetRoot);
            int layer = LayerMask.NameToLayer("MasterModel");
            Debug.Log("layer: " + layer);
            SetLayerRecursively(LoadedSourceAsset, layer);
            IfcRootLists_Source = LoadedSourceAsset.GetComponent<IfcRootLists>();

            //OnFocus(LoadedSourceAsset.transform, Camera_ORG);


            Camera_ORG.transform.LookAt(LoadedSourceAsset.transform);

            LoadTargetAssets(TargetGuid);
        }
        else
        {
            MCPopup.Instance.SetWarning("Source Asset Load Failed", "Error");
            AppController.Instance.SetPage(AppController.PageIndex.project);
        }
    }

    public void LoadTargetAssets(string _fileName)
    {
        //StartCoroutine(DataProxy.Instance.LoadAssetBundle(_fileName, OnLoadTargetAsset_Callback));
    }

    public void OnLoadTargetAsset_Callback(bool _result, string _message, AssetBundle bundle)
    {
        if (_result)
        {
            if (LoadedTargetAsset != null)
            {
                Destroy(LoadedTargetAsset);
            }

            GameObject prefab = bundle.LoadAsset<GameObject>(_message);
            bundle.Unload(false);
            LoadedTargetAsset = Instantiate(prefab, AssetRoot);
            int layer = LayerMask.NameToLayer("CompareModel");
            Debug.Log("layer: " + layer);
            SetLayerRecursively(LoadedTargetAsset, layer);
            IfcRootLists_Target = LoadedTargetAsset.GetComponent<IfcRootLists>();
            //OnFocus(LoadedTargetAsset.transform, Camera_CMP);

            Camera_CMP.transform.LookAt(LoadedTargetAsset.transform);
            // Start Compare
            //LoadingHandler.Instance.StartLoading();
            //StartCoroutine(OnCompareVersion
            OnCompareVersion();

        }
        else
        {
            MCPopup.Instance.SetWarning("Target Asset Load Failed", "Error");
            AppController.Instance.SetPage(AppController.PageIndex.dashboard);
        }
    }

    [Header("Processed List")]
    public Dictionary<string, GameObject> Processed_Added = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> Processed_Removed = new Dictionary<string, GameObject>();

    public Dictionary<string, GameObject> Processed_Target = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> Processed_Source = new Dictionary<string, GameObject>();

    public Dictionary<string, GameObject> Processed_Changed = new Dictionary<string, GameObject>();


    public List<IfcCompareNode> IfcCompareNodeList = new List<IfcCompareNode>();
    public List<IfcCompareNode> IfcCompareNodeListFiltered = new List<IfcCompareNode>();


    public List<CompareElement> UpdateAttributes = new List<CompareElement>();
    public List<CompareElement> UpdateProperties = new List<CompareElement>();
    public List<CompareElement> UpdateQuantities = new List<CompareElement>();
    public List<CompareElement> UpdateMaterials = new List<CompareElement>();
    public List<CompareElement> UpdateTypes = new List<CompareElement>();


    public void OnCompareVersion()
    {

        if (IfcRootLists_Target.ifcGameObject.Count > 0 && IfcRootLists_Source.ifcGameObject.Count > 0)
        {
            Processed_Added.Clear();
            Processed_Removed.Clear();
            Processed_Changed.Clear();
            Processed_Target.Clear();
            Processed_Source.Clear();
            IfcCompareNodeList.Clear();

            foreach (GameObject item in IfcRootLists_Source.ifcGameObject)
            {
                string ifcId = item.GetComponent<IfcAttributes>().Find("id");
                Processed_Source.Add(ifcId, item);
            }

            foreach (GameObject item in IfcRootLists_Target.ifcGameObject)
            {
                string ifcId = item.GetComponent<IfcAttributes>().Find("id");
                Processed_Target.Add(ifcId, item);
            }

            foreach (GameObject item in IfcRootLists_Source.ifcGameObject)
            {
                // looking for node that is exsit in target list
                string ifcId = item.GetComponent<IfcAttributes>().Find("id");
                GameObject ob = IfcRootLists_Target.FindIfcGameObject(ifcId);

                //if not find, mark as remvoe
                // removed item alway exsits in source list

                if (ob == null) // found
                {
                    Processed_Source.Remove(ifcId);
                    IfcCompareNodeList.Add(new IfcCompareNode("Removed", ifcId, item));
                }

            }

            Debug.Log("Processed_Source: " + Processed_Source.Count);

            foreach (GameObject item in IfcRootLists_Target.ifcGameObject)
            {
                // looking for node that is exsit in source list
                string ifcId = item.GetComponent<IfcAttributes>().Find("id");
                GameObject ob = IfcRootLists_Source.FindIfcGameObject(ifcId);

                //if not find, mark as remvoe
                // removed item alway exsits in source list
                if (ob == null)
                {
                    Processed_Target.Remove(ifcId);
                    IfcCompareNodeList.Add(new IfcCompareNode("Added", ifcId, item));
                }
            }


            // compare item in each list:
            // Processed_Source
            // Processed_Target

            // Save result in Processed_Changed


            foreach (KeyValuePair<string, GameObject> sourceItem in Processed_Source)
            {

                GameObject targetItem;
                if (Processed_Target.TryGetValue(sourceItem.Key, out targetItem))
                {
                    if (!CompareIfcGameObejcts(sourceItem.Value, targetItem))
                    {
                        IfcCompareNodeList.Add(new IfcCompareNode("Updated", sourceItem.Key, sourceItem.Value));
                    }
                    else
                    {
                        IfcCompareNodeList.Add(new IfcCompareNode("NoChange", sourceItem.Key, sourceItem.Value));
                    }
                }
                else
                {
                    Debug.LogError("Item in source list is not found in target list");
                }
            }

            Debug.Log("Processed_Target: " + Processed_Target.Count);
            //LoadingHandler.Instance.CompleteLoading();
            LoadCompareData();
            //yield return null;

        }
    }

    public void LoadCompareData()
    {
        // reset selected
        if (SelectedCompareItem != null)
        {
            SelectedCompareItem.GetComponent<UIBlock_BimCompare_CompareItem>().OnDeselect();
            GameObject obs = IfcRootLists_Source.FindIfcGameObject(SelectedCompareItem.GetComponent<UIBlock_BimCompare_CompareItem>().Item.IfcId);
            GameObject obt = IfcRootLists_Target.FindIfcGameObject(SelectedCompareItem.GetComponent<UIBlock_BimCompare_CompareItem>().Item.IfcId);


            if (obs != null && SelectedSourceMaterial != null)
            {
                ChangeMaterial(obs, SelectedSourceMaterial[0]);
            }

            if (obt != null && SelectedTargetMaterial != null)
            {
                ChangeMaterial(obt, SelectedTargetMaterial[0]);
            }
        }

        SelectedCompareItem = null;
        SelectedTargetMaterial = null;
        SelectedSourceMaterial = null;

        // reset list
        IfcCompareNodeListFiltered.Clear();

        if (CompareNodeType == "All")
        {
            foreach (IfcCompareNode item in IfcCompareNodeList)
            {
                if (item.NodeType != "NoChange")
                {
                    IfcCompareNodeListFiltered.Add(item);
                }
            }
        }
        else
        {
            foreach (IfcCompareNode item in IfcCompareNodeList)
            {
                if (item.NodeType == CompareNodeType)
                {
                    IfcCompareNodeListFiltered.Add(item);
                }
            }
        }


        //ScrollViewer_CompareItems.LoadData(IfcCompareNodeListFiltered);

        //Reset attribute view

        SetAttributeView(null, SourceAttributeItems, SourceAttributeItemPrefab, SourceAttributeItemParent);

        // Set Target View
        SetAttributeView(null, TargetAttributeItems, TargetAttributeItemPrefab, TargetAttributeItemParent);
    }

    public bool CompareIfcGameObejcts(GameObject _sourceNode, GameObject _targetNode)
    {
        // checking for attruibutes
        if (_sourceNode.GetComponent<IfcAttributes>() != null && _targetNode.GetComponent<IfcAttributes>() != null)
        {
            IfcAttributes sourceAttributes = _sourceNode.GetComponent<IfcAttributes>();
            IfcAttributes targetAttributes = _targetNode.GetComponent<IfcAttributes>();

            foreach (string ab in sourceAttributes.attributes)
            {
                if (targetAttributes.Find(ab) == null || sourceAttributes.Find(ab) != targetAttributes.Find(ab))
                {
                    // removed in target
                    return false;
                }

            }


            foreach (string ab in targetAttributes.attributes)
            {
                if (sourceAttributes.Find(ab) == null)
                {
                    // added in target
                    return false;  
                }
            }
        }
        else if (_sourceNode.GetComponent<IfcAttributes>() == null && _targetNode.GetComponent<IfcAttributes>() == null)
        {
            // same
        }
        else
        {
            return false;
        }


        // checking for properties
        if (_sourceNode.GetComponent<IfcProperties>() != null && _targetNode.GetComponent<IfcProperties>() != null)
        {
            IfcProperties sourceProperties = _sourceNode.GetComponent<IfcProperties>();
            IfcProperties targetProperties = _targetNode.GetComponent<IfcProperties>();

            foreach (string ab in sourceProperties.properties)
            {
                if (targetProperties.Find(ab) == null || sourceProperties.Find(ab) != targetProperties.Find(ab))
                {
                    return false;  // changed or removed in target
                }
            }


            foreach (string ab in targetProperties.properties)
            {
                if (sourceProperties.Find(ab) == null)
                {
                    return false;    // added in target
                }
            }
        }
        else if (_sourceNode.GetComponent<IfcProperties>() == null && _targetNode.GetComponent<IfcProperties>() == null)
        {

        }
        else
        {
            return false; // 
        }

        // checking for quantities
        if (_sourceNode.GetComponent<IfcQuantities>() != null && _targetNode.GetComponent<IfcQuantities>() != null)
        {
            IfcQuantities sourceQuantites = _sourceNode.GetComponent<IfcQuantities>();
            IfcQuantities targetQuantites = _targetNode.GetComponent<IfcQuantities>();

            foreach (string ab in sourceQuantites.quantities)
            {
                if (targetQuantites.Find(ab) == null || sourceQuantites.Find(ab) != targetQuantites.Find(ab))
                {
                    return false;  // changed or removed in target
                }
            }


            foreach (string ab in targetQuantites.quantities)
            {
                if (sourceQuantites.Find(ab) == null)
                {
                    return false;    // added in target
                }
            }
        }
        else if (_sourceNode.GetComponent<IfcQuantities>() == null && _targetNode.GetComponent<IfcQuantities>() == null)
        {

        }
        else
        {
            return false; // 
        }

        // checking for materiais
        if (_sourceNode.GetComponent<IfcMaterials>() != null && _targetNode.GetComponent<IfcMaterials>() != null)
        {
            IfcMaterials sourceMeterials = _sourceNode.GetComponent<IfcMaterials>();
            IfcMaterials targetMeterials = _targetNode.GetComponent<IfcMaterials>();

            foreach (string ab in sourceMeterials.materials)
            {
                if (targetMeterials.Find(ab) == null || sourceMeterials.Find(ab) != targetMeterials.Find(ab))
                {
                    return false;  // changed or removed in target
                }
            }


            foreach (string ab in targetMeterials.materials)
            {
                if (sourceMeterials.Find(ab) == null)
                {
                    return false;    // added in target
                }
            }
        }
        else if (_sourceNode.GetComponent<IfcMaterials>() == null && _targetNode.GetComponent<IfcMaterials>() == null)
        {

        }
        else
        {
            return false; // 
        }

        // checking for types
        if (_sourceNode.GetComponent<IfcTypes>() != null && _targetNode.GetComponent<IfcTypes>() != null)
        {
            IfcTypes sourceTypes = _sourceNode.GetComponent<IfcTypes>();
            IfcTypes targetTypes = _targetNode.GetComponent<IfcTypes>();

            foreach (string ab in sourceTypes.types)
            {
                if (targetTypes.Find(ab) == null || sourceTypes.Find(ab) != targetTypes.Find(ab))
                {
                    return false;  // changed or removed in target
                }
            }


            foreach (string ab in targetTypes.types)
            {
                if (sourceTypes.Find(ab) == null)
                {
                    return false;    // added in target
                }
            }
        }
        else if (_sourceNode.GetComponent<IfcTypes>() == null && _targetNode.GetComponent<IfcTypes>() == null)
        {

        }
        else
        {
            return false; // 
        }


        return true;
    }

    public void CompareIfcGameObejctsElements(GameObject _sourceNode, GameObject _targetNode)
    {
        UpdateAttributes.Clear();
        UpdateProperties.Clear();
        UpdateQuantities.Clear();
        UpdateMaterials.Clear();
        UpdateTypes.Clear();

        // checking for IfcAttributes
        if (_sourceNode.GetComponent<IfcAttributes>() != null && _targetNode.GetComponent<IfcAttributes>() != null)
        {
            IfcAttributes sourceAttributes = _sourceNode.GetComponent<IfcAttributes>();
            IfcAttributes targetAttributes = _targetNode.GetComponent<IfcAttributes>();


            foreach (string ab in sourceAttributes.attributes)
            {
                if (targetAttributes.Find(ab) == null)
                {
                    // removed in target
                    //return false;
                    CompareElement element = new CompareElement(ab, "Removed", "source");
                    UpdateAttributes.Add(element);
                }
                else if (sourceAttributes.Find(ab) != targetAttributes.Find(ab))
                {
                    CompareElement element = new CompareElement(ab, "Updated", "source");
                    UpdateAttributes.Add(element);
                }

            }

            foreach (string ab in targetAttributes.attributes)
            {
                if (sourceAttributes.Find(ab) == null)
                {
                    CompareElement element = new CompareElement(ab, "Added", "source");
                    UpdateAttributes.Add(element);
                }
            }
        }

        // checking for IfcProperties
        if (_sourceNode.GetComponent<IfcProperties>() != null && _targetNode.GetComponent<IfcProperties>() != null)
        {
            IfcProperties sourceProperties = _sourceNode.GetComponent<IfcProperties>();
            IfcProperties targetProperties = _targetNode.GetComponent<IfcProperties>();


            foreach (string ab in sourceProperties.properties)
            {
                if (targetProperties.Find(ab) == null)
                {
                    // removed in target
                    //return false;
                    CompareElement element = new CompareElement(ab, "Removed", "source");
                    UpdateProperties.Add(element);
                }
                else if (sourceProperties.Find(ab) != targetProperties.Find(ab))
                {
                    CompareElement element = new CompareElement(ab, "Updated", "source");
                    UpdateProperties.Add(element);
                }

            }

            foreach (string ab in targetProperties.properties)
            {
                if (sourceProperties.Find(ab) == null)
                {
                    CompareElement element = new CompareElement(ab, "Added", "source");
                    UpdateProperties.Add(element);
                }
            }
        }

        // checking for IfcQuantities
        if (_sourceNode.GetComponent<IfcQuantities>() != null && _targetNode.GetComponent<IfcQuantities>() != null)
        {
            IfcQuantities sourceQuantities = _sourceNode.GetComponent<IfcQuantities>();
            IfcQuantities targetQuantities = _targetNode.GetComponent<IfcQuantities>();


            foreach (string ab in sourceQuantities.quantities)
            {
                if (targetQuantities.Find(ab) == null)
                {
                    // removed in target
                    //return false;
                    CompareElement element = new CompareElement(ab, "Removed", "source");
                    UpdateQuantities.Add(element);
                }
                else if (sourceQuantities.Find(ab) != targetQuantities.Find(ab))
                {
                    CompareElement element = new CompareElement(ab, "Updated", "source");
                    UpdateQuantities.Add(element);
                }

            }

            foreach (string ab in targetQuantities.quantities)
            {
                if (sourceQuantities.Find(ab) == null)
                {
                    CompareElement element = new CompareElement(ab, "Added", "source");
                    UpdateQuantities.Add(element);
                }
            }
        }

        // checking for IfcMaterials
        if (_sourceNode.GetComponent<IfcMaterials>() != null && _targetNode.GetComponent<IfcMaterials>() != null)
        {
            IfcMaterials sourceMaterials = _sourceNode.GetComponent<IfcMaterials>();
            IfcMaterials targetMaterials = _targetNode.GetComponent<IfcMaterials>();


            foreach (string ab in sourceMaterials.materials)
            {
                if (targetMaterials.Find(ab) == null)
                {
                    // removed in target
                    //return false;
                    CompareElement element = new CompareElement(ab, "Removed", "source");
                    UpdateMaterials.Add(element);
                }
                else if (sourceMaterials.Find(ab) != targetMaterials.Find(ab))
                {
                    CompareElement element = new CompareElement(ab, "Updated", "source");
                    UpdateMaterials.Add(element);
                }

            }

            foreach (string ab in targetMaterials.materials)
            {
                if (sourceMaterials.Find(ab) == null)
                {
                    CompareElement element = new CompareElement(ab, "Added", "source");
                    UpdateMaterials.Add(element);
                }
            }
        }

        // checking for IfcTypes
        if (_sourceNode.GetComponent<IfcTypes>() != null && _targetNode.GetComponent<IfcTypes>() != null)
        {
            IfcTypes sourceTypes = _sourceNode.GetComponent<IfcTypes>();
            IfcTypes targetTypes = _targetNode.GetComponent<IfcTypes>();


            foreach (string ab in sourceTypes.types)
            {
                if (targetTypes.Find(ab) == null)
                {
                    // removed in target
                    //return false;
                    CompareElement element = new CompareElement(ab, "Removed", "source");
                    UpdateTypes.Add(element);
                }
                else if (sourceTypes.Find(ab) != targetTypes.Find(ab))
                {
                    CompareElement element = new CompareElement(ab, "Updated", "source");
                    UpdateTypes.Add(element);
                }

            }

            foreach (string ab in targetTypes.types)
            {
                if (sourceTypes.Find(ab) == null)
                {
                    CompareElement element = new CompareElement(ab, "Added", "source");
                    UpdateTypes.Add(element);
                }
            }
        }

    }





    public static List<string> GetChangedProperties(System.Object A, System.Object B)
    {
        if (A.GetType() != B.GetType())
        {
            throw new System.InvalidOperationException("Objects of different Type");
        }
        List<string> changedProperties = ElaborateChangedProperties(A.GetType().GetProperties(), B.GetType().GetProperties(), A, B);
        return changedProperties;
    }

    public static List<string> ElaborateChangedProperties(PropertyInfo[] pA, PropertyInfo[] pB, System.Object A, System.Object B)
    {
        List<string> changedProperties = new List<string>();
        foreach (PropertyInfo info in pA)
        {
            object propValueA = info.GetValue(A, null);
            object propValueB = info.GetValue(B, null);
            if (propValueA != propValueB)
            {
                changedProperties.Add(info.Name);
            }
        }
        return changedProperties;
    }

    // TOOD get property by name string

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

    public void SetLayerRecursively(GameObject _ob, int _layer)
    {
        _ob.layer = _layer;

        foreach (Transform trans in _ob.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = _layer;
        }
    }

    public void OnFocus(Transform _target, Camera _cam)
    {
        UnityEngine.Object[] rList = _target.GetComponentsInChildren(typeof(Renderer));
        if (rList != null && rList.Length > 0)
        {
            FocusCameraOnGameObject(_target.gameObject, _cam, rList);
        }
    }

    public void FocusCameraOnGameObject(GameObject go, Camera _cam, UnityEngine.Object[] _rlist)
    {
        Bounds b = CalculateBounds(go, _rlist);
        Vector3 max = b.size;
        float radius = Mathf.Max(max.x, Mathf.Max(max.y, max.z));
        float dist = radius / (Mathf.Sin(_cam.fieldOfView * Mathf.Deg2Rad / 2f));
        Debug.Log("Radius = " + radius + " dist = " + dist);
        Vector3 pos = UnityEngine.Random.onUnitSphere * dist + b.center;
        _cam.transform.position = pos;
        _cam.transform.LookAt(b.center);
    }


    public Bounds CalculateBounds(GameObject go, UnityEngine.Object[] _rlist)
    {
        Bounds b = new Bounds(go.transform.position, Vector3.zero);
        foreach (Renderer r in _rlist)
        {
            b.Encapsulate(r.bounds);
        }

        return b;
    }

    public void ChangeMaterial(GameObject _ob, Material _mat)
    {
        List<Material> ms = new List<Material>();

        for (int i = 0; i < _ob.GetComponent<MeshRenderer>().materials.Length; i++)
        {
            ms.Add(_mat);
        }

        _ob.GetComponent<MeshRenderer>().materials = ms.ToArray();
    }

    public void FixIfcGlassMaterial(GameObject _ob, Material _mat)
    {
        Material[] ms = _ob.GetComponent<MeshRenderer>().materials;

        if (ms.Length > 0)
        {
            int index = 0;
            foreach (Material mat in ms)
            {
                if (mat.name.ToLower().Contains("glass"))
                {
                    ms[index] = _mat;
                }

                index++;
            }

            _ob.GetComponent<MeshRenderer>().materials = ms;

        }
    }


}

[Serializable]
public class CompareElement
{
    public string ifcid;
    public string source; // from source or target list
    public string diff;   // added removed updated

    public CompareElement(string ifcid,string diff, string source)
    {
        this.ifcid = ifcid;
        this.source = source;
        this.diff = diff;
    }


}
