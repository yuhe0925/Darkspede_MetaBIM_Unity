using IfcToolkit.IfcSpec;
using MetaBIM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Page_ClassificationSelector : MonoBehaviour
{


    public static Page_ClassificationSelector Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        Panel.OnOpenAction = OnOpenAction;
        Panel.OnCloseAction = OnCloseAction;
    }


    public PanelChange Panel;
    public bool IsPageOpend;


    void Start()
    {
        //TODO: Hard coded loading, remove later
        if (!IsIFCClassDataLoaded)
        {
            LoadClassification_IFCClass();
        }

        if (!IsUniclassDataLoaded)
        {
            LoadClassification_Uniclass();
        }

        if (!IsEpicClassDataLoaded)
        {
            LoadClassification_EpicClass();
        }
    }

    
    public void OnOpenAction()
    {
        IsPageOpend = true;
        
    }


    public void OnCloseAction()
    {
        IsPageOpend = false;

        UniclassDataItems.Clear();
        IFCClassDataItems.Clear();
        ClassifcationValues.Clear();

        ClassificationItemsAdapter_IFC.SetItems(IFCClassDataItems);
        ClassificationItemsAdapter_Uniclass.SetItems(UniclassDataItems);
        ClassificationItemValuesAdapter.SetItems(ClassifcationValues);

        IFC_TreeView.SetActive(false);
        UniClassTreeView.SetActive(false);
    }


    // two tree viwer adapter
    public ClassificationItemsAdapterIFC ClassificationItemsAdapter_IFC;
    public ClassificationItemsAdapterUniclass ClassificationItemsAdapter_Uniclass;
    public ClassificationItemsAdapterEpicClass ClassificationItemsAdapter_EpicClass;
    
    public ClassificationItemValuesAdapter ClassificationItemValuesAdapter;

    
    public GameObject IFC_TreeView;
    public GameObject UniClassTreeView;
    public GameObject Epic_TreeView;

    // Buffer Data,Data load from API
    [Header("Data Buffer")]
    public List<Uniclass> UniclassData = new List<Uniclass>();
    //[SerializeField] 
    public List<IFCClass> IFCClassData = new List<IFCClass>();
    public List<epicClass> EpicClassData = new List<epicClass>();
    
    [Header("Status")]
    public bool IsUniclassDataLoaded = false;
    public bool IsIFCClassDataLoaded = false;
    public bool IsEpicClassDataLoaded = false;

    // Data for rendering tree item
    private List<Uniclass> UniclassDataItems = new List<Uniclass>();
    //[SerializeField] 
    private List<IFCClass> IFCClassDataItems = new List<IFCClass>();
    private List<epicClass> EpicClassDataItems = new List<epicClass>();
    
    // Data for Render tree item values
    public List<ClassificationItemValue> ClassifcationValues = new List<ClassificationItemValue>();
    
    public GameObject SelectedItem;
    public GameObject SelectedItemValue;
    
    public IFCClass SelectedItemValue_IFC;
    public Uniclass SelectedItemValue_Uniclass;
    public epicClass SelectedItemValue_EpicClass;
    
    public string SelectedIFC_Value;

    public ClassificationModeType ClassificationMode;  


    public List<GameObject> TargetElements; 

    [Header("UI Element")]
    public TextMeshProUGUI Text_Header;
    public TMP_InputField Input_Search;
    public bool IsDisplaySearchResult;


    public void OpenView(ClassificationModeType _type, List<GameObject> _target)
    {
        if (_target.Count == 0)
        {
            return;
        }

        ClassificationMode = _type;
        CloneGameObjectList(_target, TargetElements);

         
        Debug.Log("OpenView: " + ClassificationMode);
            
        switch (ClassificationMode)
        {
            case ClassificationModeType.ifc:
                Panel.OnPanelOpen();

                // == set object view 
                IFC_TreeView.SetActive(true);

                if (UniClassTreeView.activeSelf)
                {
                    UniClassTreeView.SetActive(false);
                }

                if (Epic_TreeView.activeSelf)
                {
                    Epic_TreeView.SetActive(false);
                }

                // == set object view end 
                Input_Search.text = "";
                break;
            case ClassificationModeType.uniclass:
                Panel.OnPanelOpen();    
                
                // == set object view 
                UniClassTreeView.SetActive(true);

                if (Epic_TreeView.activeSelf)
                {
                    Epic_TreeView.SetActive(false);
                }
                
                if (IFC_TreeView.activeSelf)
                {
                    IFC_TreeView.SetActive(false);
                }
                
                // == set object view end 
                if (TargetElements.Count == 1)
                {
                    Input_Search.text = GetIFCKeyword(TargetElements[0]).ToLower();
                }
                else
                {
                    Input_Search.text = "";
                }

                break;
            case ClassificationModeType.sustainability:
                Panel.OnPanelOpen();

                // == set object view 
                Epic_TreeView.SetActive(true);
                
                if (IFC_TreeView.activeSelf)
                {
                    IFC_TreeView.SetActive(false);
                }

                if (UniClassTreeView.activeSelf)
                {
                    UniClassTreeView.SetActive(false);
                }

                // == set object view end 
                Input_Search.text = "";
                break;
        }

        // This is may not be the right to do it 
        // Just a work around method
        Invoke("DelayAction", 0.1f);
    }

    public void OpenIFC()
    {
        ClassificationMode = ClassificationModeType.ifc;

        Panel.OnPanelOpen();

        // == set object view 
        IFC_TreeView.SetActive(true);

        if (UniClassTreeView.activeSelf)
        {
            UniClassTreeView.SetActive(false);
        }

        if (Epic_TreeView.activeSelf)
        {
            Epic_TreeView.SetActive(false);
        }

        // == set object view end 
        Input_Search.text = "";

        Invoke("DelayAction", 0.1f);
    }


    public void DelayAction()
    {
        Debug.Log("DelayAction");
        
        switch (ClassificationMode)
        {
            case ClassificationModeType.ifc:
                SearchClassifcationNode_IFCClass(Input_Search.text);
                break;
            case ClassificationModeType.uniclass:
                SearchClassifcationNode_Uniclass(Input_Search.text);
                break;
            case ClassificationModeType.sustainability:
                SearchClassifcationNode_EpicClass(Input_Search.text);
                break;
        }
    }


    #region IFC Class

    public int IFCclassDocument_LoadCount;

    public void LoadClassification_IFCClass()
    {
        //Debug.Log("LoadClassification_IFCClass");

        IFCClassData.Clear();
        IFCClassDataItems.Clear();
        IFCclassDocument_LoadCount = 0;

        // sort first layer of uniclass
        for (int i = 0; i < IFCclassMapper.className.Count; i++)
        {
            string value = IFCclassMapper.className.ElementAt(i).Value;
            //Text_Header.text = StringBuffer.Classification_Loading_IFCclassDocument.S + value;
            OnRequestIFCClass(value);
        }

    }

    public void OnRequestIFCClass(string _group)
    {
        // load from player playerpref
        //Debug.Log("OnRequestIFCClass: " + _group);
        // load from API
        StartCoroutine(DataProxy.Instance.OnRequestIFCclassUpdate("className", _group, OnRequestIFCClass_Callback));
    }

    public void OnRequestIFCClass_Callback(bool _result, string _target, string _message)
    {
        //Debug.Log("OnRequestIFCClass_Callback: " + _message);

        if (_result)
        {
            DataProxyResponse<IFCClass> payload = JsonUtility.FromJson<DataProxyResponse<IFCClass>>(_message);

            if (payload.result && payload.package.Count == 1)
            {
                IFCclassDocument_LoadCount++;
                IFCClassData.Add(payload.package[0]);

            }
            else
            {

            }
        }

        if (IFCclassDocument_LoadCount == IFCclassMapper.className.Count)
        {
            IsIFCClassDataLoaded = true;
            IFCClassData.Sort((x, y) => x.className.CompareTo(y.className));

            //SetClassRelation_IFCClass();
        }
    }

    public void SetClassRelation_IFCClass()
    {
        foreach (IFCClass item in IFCClassData)
        {
            IFCClassDataItems.Add(item);
            SetClassRelationNode_IFCClass(item);
        }
    }
    
    public void SetClassRelationNode_IFCClass(IFCClass _node)
    {
        if (_node.children.Count > 0)
        {
            foreach (IFCClass item in _node.children)
            {
                item.parent = _node;
                SetClassRelationNode_IFCClass(item);
            }
        }
    }

    public void SearchClassifcationNode_IFCClass(string _keyword)
    {

     
        foreach (IFCClass Item in IFCClassData)
        {
            Item.IsCollapsed = _keyword == "" ? true : false;
            SearchClassifcationNextNode_IFCClass(Item, _keyword);
        }


        SetClassificationView_IFC();
    }

    public void SearchClassifcationNextNode_IFCClass(IFCClass _node, string _keyword)
    {
        //_node.IsCollapsed = true;
        
        if (_node.children.Count > 0)
        {
            foreach (IFCClass item in _node.children)
            {
                item.IsCollapsed = true;
                
                if (_keyword != "")
                {
                    if (item.className.ToLower().Contains(_keyword.ToLower()))
                    {
                        Debug.Log("Search find: " + item.className);
                        item.IsSearched = true;
                        ExpendParentsNode_IFCClass(item);
                    }
                    else
                    {
                        item.IsSearched = false;
                    }
                }
                else
                {
                    item.IsSearched = true;
                }

                SearchClassifcationNextNode_IFCClass(item, _keyword);
            }
        }

    }

    public void SetClassificationView_IFC()
    {

        Text_Header.text = "IFC Class";
        IFCClassDataItems.Clear();

        foreach (IFCClass item in IFCClassData)
        {
            IFCClassDataItems.Add(item);
            GetNextNode_IFC(item);          
        }

        ClassificationItemsAdapter_IFC.SetItems(IFCClassDataItems);
        SetClassifciationItemValueTree_IFC(null);
        SelectedIFC_Value = null;
    }

    public void GetNextNode_IFC(IFCClass _node)
    {
        if (Input_Search.text == "")
        {
            if (!_node.IsCollapsed)  // opened
            {
                if (_node.children.Count > 0)
                {
                    foreach (IFCClass item in _node.children)
                    {
                        if (item.IsSearched)
                        {
                            IFCClassDataItems.Add(item);
                        }

                        GetNextNode_IFC(item);
                    }
                }
            }
        }
        else
        {
            if (_node.children.Count > 0)
            {
                foreach (IFCClass item in _node.children)
                {
                    if (item.IsSearched)
                    {
                        IFCClassDataItems.Add(item);
                    }

                    GetNextNode_IFC(item);
                }
            }
        }




    }

    public void SetClassifciationItemValueTree_IFC(IFCClass _Node)
    {
        DeselectClassificationItemValue();
       
        ClassifcationValues.Clear();
        
        if (_Node == null)
        {
            goto End;
        }

        ClassificationItemValue header = new ClassificationItemValue(_Node.className, true);
        header.ifcItem = _Node;
        ClassifcationValues.Add(header);

        if (_Node.IsEnum)
        {
            foreach (string type in _Node.Type)
            {
                ClassificationItemValue item = new ClassificationItemValue(type, false);
                ClassifcationValues.Add(item);
            }
        }

        // Render list
        End:
        ClassificationItemValuesAdapter.SetItems(ClassifcationValues);
    }


    public void ExpendParentsNode_IFCClass(IFCClass _node)
    {
        _node.IsCollapsed = false;
        _node.IsSearched = true;

        if (_node.parent != null)
        {
            ExpendParentsNode_IFCClass(_node.parent);
        }
    }

    #endregion



    #region Uniclass 

    public int UniclassDocument_LoadCount;
    public void LoadClassification_Uniclass()
    {
        //Debug.Log("LoadClassification_Uniclass");
        
        UniclassData.Clear();
        UniclassDataItems.Clear();
        UniclassDocument_LoadCount = 0;

        // sort first layer of uniclass
        for (int i = 0; i<UniclassMapper.documentNames.Count; i++)
        {
            //ClassificationItem item = new ClassificationItem();
            //item.itemName = UniclassMapper.documentNames.ElementAt(i).Value;
            //Text_Header.text = StringBuffer.Classification_Loading_UniclassDocument.S + item.itemName;
            OnRequestUniclass(UniclassMapper.documentNames.ElementAt(i).Key);
        }
        
    }

    public void OnRequestUniclass(string _group)
    {
        // load from player playerpref

        // load from API
        StartCoroutine(DataProxy.Instance.OnRequestUniclassUpdate("documentNameShort", _group, OnRequestUniclass_Callback));
    }
    
    public void OnRequestUniclass_Callback(bool _result, string _target, string _message)
    {
        if (_result)
        {
            DataProxyResponse<Uniclass> payload = JsonUtility.FromJson<DataProxyResponse<Uniclass>>(_message);

            if (payload.result && payload.package.Count == 1)
            {
                UniclassDocument_LoadCount++;
                UniclassData.Add(payload.package[0]);              
            }
            else
            {
                
            }
        }

        if(UniclassDocument_LoadCount == UniclassMapper.documentNames.Count)
        {
            IsUniclassDataLoaded = true;
            UniclassData.Sort((x, y) => x.documentNameShort.CompareTo(y.documentNameShort));
            SetClassRelation_Uniclass();
        }
    }

    public void SetClassRelation_Uniclass()
    {
        foreach (Uniclass item in UniclassData)
        {
            UniclassDataItems.Add(item);
            SetClassRelationNode_Uniclass(item);
        }
    }
    
    public void SetClassRelationNode_Uniclass(Uniclass _node)
    {
        if (_node.children.Count > 0)
        {
            foreach (Uniclass item in _node.children)
            {
                item.parent = _node;
                SetClassRelationNode_Uniclass(item);
            }
        }
    }

    public void SearchClassifcationNode_Uniclass(string _keyword)
    {

        foreach (Uniclass Item in UniclassData)
        {
            Item.IsCollapsed = _keyword == ""? true : false;
            SearchClassifcationNextNode_Uniclass(Item, _keyword);
        }


        SetClassificationView_Uniclass();
    }

    public void SearchClassifcationNextNode_Uniclass(Uniclass _node, string _keyword)
    {
        if (_node.children.Count > 0)
        {
            foreach (Uniclass item in _node.children)
            {           
                if (_keyword != "")
                {
                    if (item.Title.ToLower().Contains(_keyword.ToLower()))
                    {
                        item.IsSearched = true;
                        ExpendParentsNode_Uniclass(item);
                    }
                    else
                    {
                        item.IsSearched = false;
                    }
                }
                else
                {
                    item.IsSearched = true;

                }

                SearchClassifcationNextNode_Uniclass(item, _keyword);
            }
        }

    }

    public void SetClassificationView_Uniclass()
    {
        Text_Header.text = "Uniclass";
        UniclassDataItems.Clear();

        foreach (Uniclass Item in UniclassData)
        {
            UniclassDataItems.Add(Item);
            GetNextNode_Uniclass(Item);
        }

        ClassificationItemsAdapter_Uniclass.SetItems(UniclassDataItems);
        SetClassifciationItemValueTree_Uniclass(null);
        SelectedItemValue_Uniclass = null;
    }

    public void GetNextNode_Uniclass(Uniclass _node)
    {
        if (!_node.IsCollapsed)  // opened
        {
            if (_node.children.Count > 0)
            {
                foreach (Uniclass item in _node.children)
                {
                    if (item.IsSearched)
                    {
                        UniclassDataItems.Add(item);
                    }

                    if (item.level < 3)
                    {
                        GetNextNode_Uniclass(item);
                    }
                }
            }
        }
    }

    public void SetClassifciationItemValueTree_Uniclass(Uniclass _node)
    {

        DeselectClassificationItemValue();
        ClassifcationValues.Clear();

        if (_node == null)
        {
            goto End;
        }
        
        string headerText = _node.Title;
        ClassificationItemValue header = new ClassificationItemValue(headerText, true);
        header.uniclassItem = _node;

        ClassifcationValues.Add(header);


        if (_node.level > 1)
        {
            foreach (Uniclass uniclass in _node.children)
            {

                string contentText = uniclass.Code + " " + uniclass.Title;
                ClassificationItemValue item = new ClassificationItemValue(contentText, false);
                item.uniclassItem = uniclass;
                ClassifcationValues.Add(item);
            }
        }

        End:
        ClassificationItemValuesAdapter.SetItems(ClassifcationValues);
    }

    public void ExpendParentsNode_Uniclass(Uniclass _node)
    {
        _node.IsCollapsed = false;
        _node.IsSearched = true;

        if (_node.parent != null)
        {
            ExpendParentsNode_Uniclass(_node.parent);
        }
    }
    #endregion


    #region EPiC Class

    public int EpicClassDocument_LoadCount;

    public void LoadClassification_EpicClass()
    {
        //Debug.Log("LoadClassification_IFCClass");

        EpicClassData.Clear();
        EpicClassDataItems.Clear();
        EpicClassDocument_LoadCount = 0;

        OnRequestEpicClass(Config.DevelopmentStage);
    }

    public void OnRequestEpicClass(string _value)
    {
        // load from player playerpref

        // load from API
        StartCoroutine(DataProxy.Instance.OnRequestEpicClassUpdate("status", _value, OnRequestEpicClass_Callback));
    }

    public void OnRequestEpicClass_Callback(bool _result, string _target, string _message)
    {
        if (_result)
        {
            DataProxyResponse<epicClass> payload = JsonUtility.FromJson<DataProxyResponse<epicClass>>(_message);

            if (payload.result )
            {
                IsEpicClassDataLoaded = true;
                EpicClassData = payload.package;
                EpicClassData.Sort((x, y) => x.category.CompareTo(y.category));
            }

        }

    }

    public void SetClassRelation_EpicClass()
    {
        foreach (epicClass item in EpicClassData)
        {
            EpicClassDataItems.Add(item);
            SetClassRelationNode_EpicClass(item);
        }
    }

    public void SetClassRelationNode_EpicClass(epicClass _node)
    {
        if (_node.children.Count > 0)
        {
            foreach (epicClass item in _node.children)
            {
                item.parent = _node;
                SetClassRelationNode_EpicClass(item);
            }
        }
    }

    public void SearchClassifcationNode_EpicClass(string _keyword)
    {
        Debug.Log("SearchClassifcationNode_EpicClass: " + EpicClassData.Count);

        foreach (epicClass Item in EpicClassData)
        {
            Item.IsCollapsed = _keyword == "" ? true : false;
            SearchClassifcationNextNode_EpicClass(Item, _keyword);
        }


        SetClassificationView_EPiC();
    }

    public void SearchClassifcationNextNode_EpicClass(epicClass _node, string _keyword)
    {
        if (_node.children.Count > 0)
        {
            foreach (epicClass item in _node.children)
            {
                if (_keyword != "")
                {
                    if (item.material.ToLower().Contains(_keyword.ToLower()))
                    {
                        item.IsSearched = true;
                        ExpendParentsNode_EpicClass(item);
                    }
                    else
                    {
                        item.IsSearched = false;
                    }
                }
                else
                {
                    item.IsSearched = true;
                }

                SearchClassifcationNextNode_EpicClass(item, _keyword);
            }
        }

    }

    public void SetClassificationView_EPiC()
    {
        Debug.Log("SetClassificationView_EPiC: ");
        
        Text_Header.text = "EPiC Library 2019";
        EpicClassDataItems.Clear();

        foreach (epicClass item in EpicClassData)
        {
            EpicClassDataItems.Add(item);
            GetNextNode_EPiC(item);
        }

        ClassificationItemsAdapter_EpicClass.SetItems(EpicClassDataItems);
        SetClassifciationItemValueTree_EPiC(null);
        SelectedIFC_Value = null;
    }

    public void GetNextNode_EPiC(epicClass _node)
    {
        if (!_node.IsCollapsed)  // opened
        {
            if (_node.children.Count > 0)
            {
                foreach (epicClass item in _node.children)
                {
                    if (item.IsSearched)
                    {
                        EpicClassDataItems.Add(item);
                    }

                    GetNextNode_EPiC(item);
                }
            }
        }



    }

    public void SetClassifciationItemValueTree_EPiC(epicClass _Node)
    {
        DeselectClassificationItemValue();

        ClassifcationValues.Clear();

        if (_Node == null)
        {
            goto End;
        }

        ClassificationItemValue header_1 = new ClassificationItemValue(_Node.Content, true);
        header_1.epicItem = _Node;
        ClassificationItemValue header_2 = new ClassificationItemValue("Unit: " + _Node.unit, true);
        header_2.epicItem = _Node;
        ClassificationItemValue header_3 = new ClassificationItemValue("Embodied Energ: " + _Node.embodiedEnergy + " MJ", true);
        header_3.epicItem = _Node;
        ClassificationItemValue header_4 = new ClassificationItemValue("Embodied Water: " + _Node.embodiedWater + " L", true);
        header_4.epicItem = _Node;
        ClassificationItemValue header_5 = new ClassificationItemValue("Greenhouse Gas Emissions: " + _Node.embodiedGreenhouseGasEmission + " kgCO2e", true);
        header_5.epicItem = _Node;


        ClassifcationValues.Add(header_1);
        ClassifcationValues.Add(header_2);
        ClassifcationValues.Add(header_3);
        ClassifcationValues.Add(header_4);
        ClassifcationValues.Add(header_5);
        /*
        if (_Node.IsEnum)
        {
            foreach (string type in _Node.Type)
            {
                ClassificationItemValue item = new ClassificationItemValue(type, false);
                ClassifcationValues.Add(item);
            }
        }
        */

        // Render list
        End:
        ClassificationItemValuesAdapter.SetItems(ClassifcationValues);
    }

    public void ExpendParentsNode_EpicClass(epicClass _node)
    {
        _node.IsCollapsed = false;
        _node.IsSearched = true;

        if (_node.parent != null)
        {
            ExpendParentsNode_EpicClass(_node.parent);
        }
    }


    
    #endregion





    
    public void OnClick_ExpendTreeItem(GameObject _gameObject)
    {
        switch (ClassificationMode)
        {
            case ClassificationModeType.ifc:
                UIBlock_BimViewer_ClassificationItem_IFC ifc = _gameObject.GetComponent<UIBlock_BimViewer_ClassificationItem_IFC>();
                ifc.Item.IsCollapsed = false;
                SetClassificationView_IFC();
                break;
            case ClassificationModeType.uniclass:
                UIBlock_BimViewer_ClassificationItem_Uniclass uniclass = _gameObject.GetComponent<UIBlock_BimViewer_ClassificationItem_Uniclass>();
                uniclass.Item.IsCollapsed = false;
                SetClassificationView_Uniclass();
                break;
            case ClassificationModeType.sustainability:
                UIBlock_BimViewer_ClassificationItem_EpicClass epicclass = _gameObject.GetComponent<UIBlock_BimViewer_ClassificationItem_EpicClass>();
                epicclass.Item.IsCollapsed = false;
                SetClassificationView_EPiC();
                break;
        }



    }

    public void OnClick_CollapseTreeItem(GameObject _gameObject)
    {
        switch (ClassificationMode)
        {
            case ClassificationModeType.ifc:
                UIBlock_BimViewer_ClassificationItem_IFC ifc = _gameObject.GetComponent<UIBlock_BimViewer_ClassificationItem_IFC>();
                ifc.Item.IsCollapsed = true;
                SetClassificationView_IFC();
                break;
            case ClassificationModeType.uniclass:
                UIBlock_BimViewer_ClassificationItem_Uniclass uniclass = _gameObject.GetComponent<UIBlock_BimViewer_ClassificationItem_Uniclass>();
                uniclass.Item.IsCollapsed = true;
                SetClassificationView_Uniclass();
                break;
            case ClassificationModeType.sustainability:
                UIBlock_BimViewer_ClassificationItem_EpicClass epicclass = _gameObject.GetComponent<UIBlock_BimViewer_ClassificationItem_EpicClass>();
                epicclass.Item.IsCollapsed = true;
                SetClassificationView_EPiC();
                break;
        }
    }

    public void ApplyValue(bool _isShowProcess)
    {
        if (TargetElements == null)
        {
            MCPopup.Instance.SetWarning("No element selected");  // ? string buffer?
            return;
        }

        if (_isShowProcess)
        {

            var model  = ProjectModelHandler.Instance.GetDefaultActiveModel();
            CloneGameObjectList(model.GetModelObjects(), TargetElements);

            //TODO: add warning for no element selected ?
            Page_ProcessingLog.Instance.OnProcess_Start(OnMappingPasue, OnMappingStop, TargetElements.Count);
            StartCoroutine(ValueMappingProcess());
        }
        else
        {
            StartCoroutine(ValueApplyProcess());
        }



    }

    public IEnumerator ValueMappingProcess()
    {
        int count = 0;
        string progress_message = "";



        foreach (GameObject TargetElement in TargetElements)
        {
            BIMElement element = TargetElement.GetComponent<BIMElement>();

            if (element.BimObject.records.Count > 0)
            {
                progress_message = "Updating " + element.BimObject.records[0].objectName + " to " + SelectedItemValue_IFC.className + " (" + count + "/" + TargetElements.Count + ")";
            }
            else
            {
                progress_message = "Connecting element, skip...";
            }

            Page_ProcessingLog.Instance.OnProcess(progress_message, count);
            count++;

            if (count % ProjectConfiguration.Instance.MAX_IENUMERATOR_RATE == 0)
            {
                yield return new WaitForEndOfFrame();
            }

        }

        yield return null;
        Page_BIMViewer.Instance.Tab_TreeView.SetTab("View IFC");
    }



    public IEnumerator ValueApplyProcess()
    {
        int count = 0;
  
        foreach (GameObject TargetElement in TargetElements)
        {
            BIMElement element = TargetElement.GetComponent<BIMElement>();

            MetaBIM_IfcParameter ifc = element.BimObject.records[element.BimObject.versionID].ifcParameter;
            MetaBIM_IfcUniclass uniclass = element.BimObject.records[element.BimObject.versionID].ifcUniclass;
            MetaBIM_IfcUniclassMap uniclassMap = element.BimObject.records[element.BimObject.versionID].ifcUniclassMap;
            MetaBIM_IfcEpicClass epic = element.BimObject.records[element.BimObject.versionID].ifcEpicClass;

            switch (ClassificationMode)
            {
                case ClassificationModeType.ifc:

                    if (SelectedIFC_Value != null)
                    {
                        ifc.SetValue("Export to IFC As", SelectedItemValue_IFC.className);
                        ifc.SetValue("IFC Predefined Type", SelectedIFC_Value);
                        ifc.SetValue("IFC Guid", element.BimObject.elementID);
                        Page_BIMViewer.Instance.SetAttributeView(TargetElement);
                    }
                    break;

                case ClassificationModeType.uniclass:
                    if (SelectedItemValue_Uniclass != null)
                    {
                        uniclass.SetValue("Code", SelectedItemValue_Uniclass.Code);
                        uniclass.SetValue("Group", SelectedItemValue_Uniclass.Group);
                        uniclass.SetValue("Sub_Group", SelectedItemValue_Uniclass.Sub_Group);
                        uniclass.SetValue("Section", SelectedItemValue_Uniclass.Section);
                        uniclass.SetValue("Object", SelectedItemValue_Uniclass.Object);
                        uniclass.SetValue("Title", SelectedItemValue_Uniclass.Title);
                        uniclass.SetValue("Table", SelectedItemValue_Uniclass.documentName);

                        uniclassMap.SetValue("NBS_Code", SelectedItemValue_Uniclass.NBS_Code);
                        uniclassMap.SetValue("COBie", SelectedItemValue_Uniclass.NBS_Code);
                        uniclassMap.SetValue("NRM1", SelectedItemValue_Uniclass.NBS_Code);
                        uniclassMap.SetValue("CESMM", SelectedItemValue_Uniclass.NBS_Code);

                        if (SelectedItemValue_Uniclass.IFC != "" && SelectedItemValue_Uniclass.IFC != Config.None)
                        {
                            uniclassMap.SetValue("IFC", SelectedItemValue_Uniclass.IFC);
                        }
                        else
                        {
                            uniclassMap.SetValue("IFC", ifc.Find("Export to IFC As"));
                            uniclassMap.SetValue("IFC_Type", ifc.Find("IFC Predefined Type"));
                        }

                        Page_BIMViewer.Instance.SetAttributeView(TargetElement);
                    }
                    break;
                case ClassificationModeType.sustainability:

                    Debug.Log("Set SelectedItemValue_EpicClass:" + SelectedItemValue_EpicClass.Content);

                    if (SelectedItemValue_EpicClass != null)
                    {
                        epic.attributes.Clear();
                        epic.values.Clear();

                        epic.SetNewValue("Category", SelectedItemValue_EpicClass.category);
                        epic.SetNewValue("Type", SelectedItemValue_EpicClass.type);
                        epic.SetNewValue("Material", SelectedItemValue_EpicClass.material);
                        epic.SetNewValue("Unit", SelectedItemValue_EpicClass.unit);

                        if (SelectedItemValue_EpicClass.unit == "m³")
                        {
                            float value_Volume = (float)Math.Round(element.Renderer.bounds.size.x * element.Renderer.bounds.size.y * element.Renderer.bounds.size.z / 2, 3);
                            epic.SetNewValue("Volume", value_Volume.ToString() + " " + SelectedItemValue_EpicClass.unit);

                            float value_Energy = (float)Math.Round(SelectedItemValue_EpicClass.embodiedEnergy * value_Volume, 3);
                            epic.SetNewValue("Embodied Energy", value_Energy.ToString() + " MJ");

                            float value_Water = (float)Math.Round(SelectedItemValue_EpicClass.embodiedWater * value_Volume, 3);
                            epic.SetNewValue("Embodied Water", value_Water.ToString() + " L");

                            float value_GG = (float)Math.Round(SelectedItemValue_EpicClass.embodiedGreenhouseGasEmission * value_Volume, 3);
                            epic.SetNewValue("Embodied Greenhouse Gas Emission", value_GG.ToString() + " kgCO2e");
                        }
                        else if (SelectedItemValue_EpicClass.unit == "kg")
                        {
                            float value_Volume = (float)Math.Round(element.Renderer.bounds.size.y * 1000f, 3);
                            epic.SetNewValue("Weight", value_Volume.ToString() + " " + SelectedItemValue_EpicClass.unit);

                            float value_Energy = (float)Math.Round(SelectedItemValue_EpicClass.embodiedEnergy * value_Volume, 3);
                            epic.SetNewValue("Embodied Energy", value_Energy.ToString() + " MJ");

                            float value_Water = (float)Math.Round(SelectedItemValue_EpicClass.embodiedWater * value_Volume, 3);
                            epic.SetNewValue("Embodied Water", value_Water.ToString() + " L");

                            float value_GG = (float)Math.Round(SelectedItemValue_EpicClass.embodiedGreenhouseGasEmission * value_Volume, 3);
                            epic.SetNewValue("Embodied Greenhouse Gas Emission", value_GG.ToString() + " kgCO2e");
                        }
                        else if (SelectedItemValue_EpicClass.unit == "no.")
                        {
                            float value_Volume = 1f;
                            epic.SetNewValue("Count", value_Volume.ToString());

                            float value_Energy = (float)Math.Round(SelectedItemValue_EpicClass.embodiedEnergy * value_Volume, 3);
                            epic.SetNewValue("Embodied Energy", value_Energy.ToString() + " MJ");

                            float value_Water = (float)Math.Round(SelectedItemValue_EpicClass.embodiedWater * value_Volume, 3);
                            epic.SetNewValue("Embodied Water", value_Water.ToString() + " L");

                            float value_GG = (float)Math.Round(SelectedItemValue_EpicClass.embodiedGreenhouseGasEmission * value_Volume, 3);
                            epic.SetNewValue("Embodied Greenhouse Gas Emission", value_GG.ToString() + " kgCO2e");
                        }
                        else if (SelectedItemValue_EpicClass.unit == "m²")
                        {
                            float value_Volume = (float)Math.Round(element.Renderer.bounds.size.y, 3);
                            epic.SetNewValue("Length", value_Volume.ToString() + " " + SelectedItemValue_EpicClass.unit);

                            float value_Energy = (float)Math.Round(SelectedItemValue_EpicClass.embodiedEnergy * value_Volume, 3);
                            epic.SetNewValue("Embodied Energy", value_Energy.ToString() + " MJ");

                            float value_Water = (float)Math.Round(SelectedItemValue_EpicClass.embodiedWater * value_Volume, 3);
                            epic.SetNewValue("Embodied Water", value_Water.ToString() + " L");

                            float value_GG = (float)Math.Round(SelectedItemValue_EpicClass.embodiedGreenhouseGasEmission * value_Volume, 3);
                            epic.SetNewValue("Embodied Greenhouse Gas Emission", value_GG.ToString() + " kgCO2e");
                        }
                        else
                        {

                        }


                        Page_BIMViewer.Instance.SetAttributeView(TargetElement);
                    }
                    break;

            }


            count++;

            if(count % 4 == 0)
            {
                yield return new WaitForSeconds(0.1f);
            }
            // Update BIM Element
            //OnRequestUpdateBIMElement(element);
        }

        yield return null;
        Page_BIMViewer.Instance.Tab_TreeView.SetTab();
    }






    public void OnMappingPasue(string _message)
    {

    }

    public void OnMappingStop(string _message)
    {

    }


    public void OnValueChange_Search(string _value)
    {
        switch (ClassificationMode)
        {
            case ClassificationModeType.ifc:
                SearchClassifcationNode_IFCClass(_value);
                break;
            case ClassificationModeType.uniclass:
                SearchClassifcationNode_Uniclass(_value);
                break;
            case ClassificationModeType.sustainability:
                SearchClassifcationNode_EpicClass(_value);
                break;
        }
    }


    public void OnClick_AutoMapping()
    {
        MCPopup.Instance.SetConfirm(OnAutoMapping_Confirm, "", "Automatically mapping IFC Class for all elements?", "IFC Class Mapping");
    }


    public void OnAutoMapping_Confirm(bool _result, string _message)
    {
        if (_result)
        {
            ApplyValue(true);
        }
    }

    public void OnClick_Apply()
    {
        ApplyValue(false);
    }

    
    public void OnClick_OK() 
    {
        ApplyValue(false);
        Panel.OnPanelClose();
    }

    public void OnClick_Cancel()
    {
        Panel.OnPanelClose();
    }
    public void OnClick_ClosePanel()
    {
        Panel.OnPanelClose();
    }


    public void OnItemSelected(GameObject _item)
    {
        ClassifcationValues.Clear();
        
        switch (ClassificationMode)
        {
            case ClassificationModeType.ifc:
                UIBlock_BimViewer_ClassificationItem_IFC ifc;
                
                if (SelectedItem!= null)
                {
                    ifc = SelectedItem.GetComponent<UIBlock_BimViewer_ClassificationItem_IFC>();
                    ifc.OnDeselect();
                }
                
                SelectedItem = _item;
                
                ifc = _item.GetComponent<UIBlock_BimViewer_ClassificationItem_IFC>();
                ifc.OnSelect();
                SelectedItemValue_IFC = ifc.Item;
                
                SetClassifciationItemValueTree_IFC(SelectedItemValue_IFC);
                break;
            case ClassificationModeType.uniclass:
                UIBlock_BimViewer_ClassificationItem_Uniclass uniclass;
                
                if (SelectedItem != null)
                {
                    uniclass = SelectedItem.GetComponent<UIBlock_BimViewer_ClassificationItem_Uniclass>();
                    uniclass.OnDeselect();
                }
                
                SelectedItem = _item;

                uniclass = _item.GetComponent<UIBlock_BimViewer_ClassificationItem_Uniclass>();
                uniclass.OnSelect();
                SelectedItemValue_Uniclass = uniclass.Item;
                
                SetClassifciationItemValueTree_Uniclass(SelectedItemValue_Uniclass);   
                break;
            case ClassificationModeType.sustainability:
                UIBlock_BimViewer_ClassificationItem_EpicClass epicClass;

                if (SelectedItem != null)
                {
                    epicClass = SelectedItem.GetComponent<UIBlock_BimViewer_ClassificationItem_EpicClass>();
                    epicClass.OnDeselect();
                }

                SelectedItem = _item;

                epicClass = _item.GetComponent<UIBlock_BimViewer_ClassificationItem_EpicClass>();
                epicClass.OnSelect();
                SelectedItemValue_EpicClass = epicClass.Item;
                
                SetClassifciationItemValueTree_EPiC(SelectedItemValue_EpicClass);

                break;
        }

    }


    public void OnSelectClassificationItemValue(GameObject _gameObject)
    {
        DeselectClassificationItemValue();
        
        SelectedItemValue = _gameObject;
        UIBlock_BimViewer_ClassificationValueItem item = _gameObject.GetComponent<UIBlock_BimViewer_ClassificationValueItem>();
        item.OnSelect();
        
        switch (ClassificationMode)
        {
            case ClassificationModeType.ifc:
  
                if (SelectedItemValue_IFC.IsEnum)
                {
                    SelectedIFC_Value = item.Item.itemName;
                }
                else
                {
                    SelectedIFC_Value = "";
                }
                
                break;
            case ClassificationModeType.uniclass:
                SelectedItemValue_Uniclass = item.Item.uniclassItem;
               
                break;
            case ClassificationModeType.sustainability:
                SelectedItemValue_EpicClass = item.Item.epicItem;

                break;
        }


   

        
    }

    public void DeselectClassificationItemValue()
    {
        UIBlock_BimViewer_ClassificationValueItem item;
        
        switch (ClassificationMode)
        {
            case ClassificationModeType.ifc:

                if (SelectedItemValue != null)
                {
                    item = SelectedItemValue.GetComponent<UIBlock_BimViewer_ClassificationValueItem>();
                    item.OnDeselect();
                }

                break;
            case ClassificationModeType.uniclass:
                if (SelectedItemValue != null)
                {
                    item = SelectedItemValue.GetComponent<UIBlock_BimViewer_ClassificationValueItem>();
                    item.OnDeselect();
                }
                break;
            case ClassificationModeType.sustainability:
                if (SelectedItemValue != null)
                {
                    item = SelectedItemValue.GetComponent<UIBlock_BimViewer_ClassificationValueItem>();
                    item.OnDeselect();
                }
                break;
        }
        
  
    }


    public string GetIFCKeyword(GameObject TargetElement)
    {
        string keyword = "";
        MetaBIM_IfcParameter ifc;
        
        if (TargetElement != null)
        {
            BIMElement element = TargetElement.GetComponent<BIMElement>();
            ifc = element.BimObject.records[element.BimObject.versionID].ifcParameter;
            string parameter = ifc.GetIfcParamenter(MetaBIM_IfcParameter.IfcParameterType.ExportToIfcAs);

            
            if (parameter != "")
            {
                string[] words = Utility.GetWordsInHyperWord(parameter);
                
                if(words.Length> 1)
                {
                    return words[1];
                }
                else
                    if (words.Length == 1)
                {
                    return words[0];
                }
                else
                {
                    return "";
                }
            }

            
            return ifc.Find(parameter);
        }



        return keyword;
    }

    
    public enum ClassificationModeType
    {
        ifc,
        uniclass,
        sustainability,
        general,
    }





    #region Data Sync 
    public void OnRequestUpdateBIMElement(BIMElement _item)
    {        
        StartCoroutine(DataProxy.Instance.UpdateBimObject(BimObject.ToJson(_item.BimObject), AppController.Instance.SelectedWorkspaceGuid,OnRequestUpdateBIMElement_Callback)); ;
    }



    public void OnRequestUpdateBIMElement_Callback(bool _result, string _message)
    {
        Debug.Log("OnRequestUpdateBIMElement_Callback: " + _message);
        if (_result)
        {

        }
        else
        {
            
        }
    }

    #endregion


    public void CloneGameObjectList(List<GameObject> _resourceList, List<GameObject> _targetList)
    {
        Debug.Log("CloneGameObjectList: " + _resourceList.Count);
        _targetList.Clear();

        foreach (var item in _resourceList)
        {
            _targetList.Add(item);
        }

        Debug.Log("TargetList: " + _targetList.Count);
    }
}





public class ClassificationItemValue
{
    public int level = 0;
    public string itemName;
    public bool isHeader;

    public int itemType;   // 0 not interactable
    public Uniclass uniclassItem;
    public IFCClass ifcItem;
    public epicClass epicItem;

    public ClassificationItemValue(string _name, bool _isHeader)
    {
        itemName = _name;
        isHeader = _isHeader;

    }
}
