using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MetaBIM;
using System.IO;
using TMPro;
using SFB;
using System.Xml.Linq;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using IfcToolkit.IfcSpec;
using static EzySlice.Triangulator;
using System.Reflection;
using UnityEngine.UIElements;
using netDxf.Blocks;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using System.Threading;







#if (UNITY_WEBGL && !UNITY_EDITOR)
        
#else
using System.Threading.Tasks;
#endif


public class DataTransferHandler : MonoBehaviour
{

    public static DataTransferHandler Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        Panel_Main.OnOpenAction = OnPlaneOpen;
        Panel_Main.OnCloseAction = OnPlaneClose;


    }



    public static int ExportCount;
    public static int ExportProgress;
    public static string ExportMessage;
    public static string SavePath;
    public static bool IsExportingOngoing = false;

    public static int DebugCount;
    public static string DebugString;
    public static bool IsThreadException = false; 


    [Header("Exporting Progress")]

    [SerializeField]
    private int _ExportCount;
    [SerializeField]
    private int _ExportProgress;
    [SerializeField]
    private string _SavePath;
    [SerializeField]
    private string _FileName;

    [SerializeField]
    private int _DebugCount;

    [SerializeField]
    private string _DebugString;
    [SerializeField]
    private string _ExceptingString;



    [Header("UI Elements")]
    public PanelChange Panel_Main;
    public BimPropertyItemAdapter PropertyItemAdapter;
    public TMP_InputField Text_ExportFileName;
    public TMP_Dropdown Dropdown_ExportSelection;
    public TMP_Dropdown Dropdown_ExportFormat;

    [Header("Buffer")]

    // the preset category of properties
    public List<BIMExportPropertyItem> PropertyCategory = new List<BIMExportPropertyItem>();

    // the sub category of properties
    public Dictionary<string, List<BIMExportPropertyItem>> PropertyItemGroups = new Dictionary<string, List<BIMExportPropertyItem>>();


    // the list of off all catergory to be displayed
    public List<BIMExportPropertyItem> PropertyItems = new List<BIMExportPropertyItem>();



    [Header("Exporting")]
    public static List<BimObject> exportDataItems = new List<BimObject>();

    public void OnPlaneOpen()
    {
        // reset all input fields
        Text_ExportFileName.text = ProjectModelHandler.Instance.CurrentModel.bimModel.name;
        Dropdown_ExportSelection.value = 0;
        Dropdown_ExportFormat.value = 0;

        // load properties from model
        PrepareExportPropertyCategory();
        PropertyItemAdapter.SetItems(PropertyItems);

    }


    public void PrepareExportPropertyCategory()
    {
        PropertyItems.Clear();
        PropertyItemGroups.Clear();

        foreach (var item in PropertyCategory)
        {
            PropertyItems.Add(item);
        }

        foreach (var item in ProjectModelHandler.Instance.CurrentModel.bimModel.Structures)
        {
            if (item.element != null)
            {
                if (item.element.BimObject.records.Count > 0)
                {
                    BimObjectRecord record = item.element.BimObject.records[0];

                    foreach (var cate in PropertyCategory)
                    {
                        string propertyName = cate.PropertyName;
                        var PropertyList = new List<BIMExportPropertyItem>();


                        if (PropertyItemGroups.ContainsKey(propertyName))
                        {
                            PropertyList = PropertyItemGroups[propertyName];
                        }
                        else
                        {
                            PropertyItemGroups.Add(propertyName, PropertyList);
                        }


                        string methodName = "ProcessPropertyCategory_" + propertyName.Replace(" ", "");
                        MethodInfo mi = GetType().GetMethod(methodName);
                        
                        if(mi != null)
                        {
                            mi.Invoke(this, new object[] { PropertyList, record });
                        }
                                                     
                    }
                }
            }
        }

        Debug.Log("PrepareExportPropertyCategory: PropertyItemGroups.Count: " + PropertyItemGroups.Count);
    }



    #region Process Property Category


    public void ProcessPropertyCategory_MetaBIMProperty(List<BIMExportPropertyItem> _propertItem, BimObjectRecord _record)
    {

        Debug.Log("ProcessPropertyCategory_MetaBIMProperty");

        MetaBIM_Property meta = new MetaBIM_Property();

        for (int i = 0; i < meta.generalItems.Count; i++)
        {
            string key = meta.generalItems[i].key;
     
            var result = CheckPropertyItemInList(_propertItem, key);

            if (!result)
            {
                BIMExportPropertyItem item = new BIMExportPropertyItem();
                item.PropertyName = key;
                item.ParentID = 0;
                item.IsSubItem = true;
                item.IsExpended = false;
                _propertItem.Add(item);
            }
        }
    }

    public void ProcessPropertyCategory_Geometry(List<BIMExportPropertyItem> _propertItem, BimObjectRecord _record)
    {

        // TODO
    }

    public void ProcessPropertyCategory_Material(List<BIMExportPropertyItem> _propertItem, BimObjectRecord _record)
    {
        if (_record.ifcMaterials == null)
        {
            return;
        }


        for (int i = 0; i < _record.ifcMaterials.materials.Count; i++)
        {
            string key = _record.ifcMaterials.materials[i];
            string value = _record.ifcMaterials.thicknesses[i];

            var result = CheckPropertyItemInList(_propertItem, key);

            if (!result)
            {
                BIMExportPropertyItem item = new BIMExportPropertyItem();
                item.PropertyName = key;
                item.ParentID = 0;
                item.IsSubItem = true;
                item.IsExpended = false;
                _propertItem.Add(item);
            }
        }

    }


    public void ProcessPropertyCategory_Zoning(List<BIMExportPropertyItem> _propertItem, BimObjectRecord _record)
    {
        if (_record.ifcZone == null)
        {
            return;
        }

        for (int i = 0; i < _record.ifcZone.attributes.Count; i++)
        {
            string key = _record.ifcZone.attributes[i];
            var result = CheckPropertyItemInList(_propertItem, key);

            if (!result)
            {
                BIMExportPropertyItem item = new BIMExportPropertyItem();
                item.PropertyName = key;
                item.ParentID = 0;
                item.IsSubItem = true;
                item.IsExpended = false;
                _propertItem.Add(item);
            }
        }

    }


    public void ProcessPropertyCategory_Validation(List<BIMExportPropertyItem> _propertItem, BimObjectRecord _record)
    {
        if (_record.IfcValidation == null)
        {
            return;
        }

        for (int i = 0; i < _record.IfcValidation.checkedItem.Count; i++)
        {
            string key = _record.IfcValidation.checkedItem[i];
            string value = _record.IfcValidation.result[i];

            var result = CheckPropertyItemInList(_propertItem, key);

            if (!result)
            {
                BIMExportPropertyItem item = new BIMExportPropertyItem();
                item.PropertyName = key;
                item.ParentID = 0;
                item.IsSubItem = true;
                item.IsExpended = false;
                _propertItem.Add(item);
            }
        }



    }


    public void ProcessPropertyCategory_Uniclass(List<BIMExportPropertyItem> _propertItem, BimObjectRecord _record)
    {
        if (_record.ifcUniclass == null)
        {
            return;
        }


        for (int i = 0; i < _record.ifcUniclass.attributes.Count; i++)
        {
            string key = _record.ifcUniclass.attributes[i];
            string value = _record.ifcUniclass.values[i];

            var result = CheckPropertyItemInList(_propertItem, key);

            if (!result)
            {
                BIMExportPropertyItem item = new BIMExportPropertyItem();
                item.PropertyName = key;
                item.ParentID = 0;
                item.IsSubItem = true;
                item.IsExpended = false;
                _propertItem.Add(item);
            }
        }



        for (int i = 0; i < _record.ifcUniclassMap.attributes.Count; i++)
        {
            string key = _record.ifcUniclassMap.attributes[i];
            string value = _record.ifcUniclassMap.values[i];

            var result = CheckPropertyItemInList(_propertItem, key);

            if (!result)
            {
                BIMExportPropertyItem item = new BIMExportPropertyItem();
                item.PropertyName = key;
                item.ParentID = 0;
                item.IsSubItem = true;
                item.IsExpended = false;
                _propertItem.Add(item);
            }
        }

    }


    public void ProcessPropertyCategory_IFCParameter(List<BIMExportPropertyItem> _propertItem, BimObjectRecord _record)
    {
        if(_record.ifcParameter == null) { 
            return;
        }

        for (int i = 0; i < _record.ifcParameter.attributes.Count; i++)
        {
            string key = _record.ifcParameter.attributes[i];
            string value = _record.ifcParameter.values[i];

            var result = CheckPropertyItemInList(_propertItem, key);

            if (!result)
            {
                BIMExportPropertyItem item = new BIMExportPropertyItem();
                item.PropertyName = key;    
                item.ParentID = 0;
                item.IsSubItem = true;
                item.IsExpended = false;
                _propertItem.Add(item);
            }
        }

    }


    public void ProcessPropertyCategory_IFCAttribute(List<BIMExportPropertyItem> _propertItem, BimObjectRecord _record)
    {
        if (_record.ifcAttribute == null)
        {
            return;
        }

        for (int i = 0; i < _record.ifcAttribute.attributes.Count; i++)
        {
            string key = _record.ifcAttribute.attributes[i];
            string value = _record.ifcAttribute.values[i];

            var result = CheckPropertyItemInList(_propertItem, key);

            if (!result)
            {
                BIMExportPropertyItem item = new BIMExportPropertyItem();
                item.PropertyName = key;
                item.ParentID = 0;
                item.IsSubItem = true;
                item.IsExpended = false;
                _propertItem.Add(item);
            }
        }

    }


    public void ProcessPropertyCategory_IFCProperty(List<BIMExportPropertyItem> _propertItem, BimObjectRecord _record)
    {
        if (_record.ifcProperties == null)
        {
            return;
        }

        for (int i = 0; i < _record.ifcProperties.properties.Count; i++)
        {
            string key = _record.ifcProperties.properties[i];

            var result = CheckPropertyItemInList(_propertItem, key);

            if (!result)
            {
                BIMExportPropertyItem item = new BIMExportPropertyItem();
                item.PropertyName = key;
                item.ParentID = 0;
                item.IsSubItem = true;
                item.IsExpended = false;
                _propertItem.Add(item);
            }
        }

    }

    public void ProcessPropertyCategory_IFCType(List<BIMExportPropertyItem> _propertItem, BimObjectRecord _record)
    {
        if (_record.ifcTypes == null)
        {
            return;
        }



        for (int i = 0; i < _record.ifcTypes.types.Count; i++)
        {
            string key = _record.ifcTypes.types[i];
            string value = _record.ifcTypes.values[i];

            var result = CheckPropertyItemInList(_propertItem, key);

            if (!result)
            {
                BIMExportPropertyItem item = new BIMExportPropertyItem();
                item.PropertyName = key;
                item.ParentID = 0;
                item.IsSubItem = true;
                item.IsExpended = false;
                _propertItem.Add(item);
            }
        }

    }



    #endregion



    public void OnPlaneClose()
    {
        // clear list
        exportDataItems.Clear();

    }


    public void OnClick_PanelClose()
    {
        Panel_Main.OnPanelClose();
    }


    public void OnToggle_IsExport(GameObject _ob)
    {

        Debug.Log("OnToggle_IsExport: PropertyItemGroups.Count: " + PropertyItemGroups.Count);

        var item = _ob.GetComponent<UIBlock_BimViewer_ObjectSplit_BIMDataPropertyItem>();

        item.Item.IsExport = !item.Item.IsExport;

        item.OnSet();

        if (!PropertyItemGroups.ContainsKey(item.Item.PropertyName))
        {
            return;
        }

        List<BIMExportPropertyItem> items = PropertyItemGroups[item.Item.PropertyName];

        if(item != null)
        {
            foreach(var i in items)
            {
                i.IsExpended = item.Item.IsExport;
            }
        }

        PropertyItemAdapter.SetItems(PropertyItems);
    }

    public GameObject SelectedBIMDataPropertyItem;




    // this need to be down in the service side
    public void OnClick_PropertyItem(GameObject gameObject)
    {
        UIBlock_BimViewer_ObjectSplit_BIMDataPropertyItem block;

        Debug.Log("OnClick_PropertyItem: PropertyItemGroups.Count: " + PropertyItemGroups.Count);

        if (SelectedBIMDataPropertyItem != null)
        {
            block  = SelectedBIMDataPropertyItem.GetComponent<UIBlock_BimViewer_ObjectSplit_BIMDataPropertyItem>();
            block.OnDeselect();
        }

        SelectedBIMDataPropertyItem = gameObject;
        block = SelectedBIMDataPropertyItem.GetComponent<UIBlock_BimViewer_ObjectSplit_BIMDataPropertyItem>();
        block.OnSelect();

        if (block.Item.IsSubItem)
        {
            return;
        }

        int index = PropertyItems.IndexOf(block.Item);


        if (!PropertyItemGroups.ContainsKey(block.Item.PropertyName))
        {
            Debug.Log("Key£º " + block.Item.PropertyName + " is not found");

            return;
        }

        List<BIMExportPropertyItem> list = PropertyItemGroups[block.Item.PropertyName];


        // to close
        if (block.Item.IsExpended)
        {
            for (int i = 0; i < list.Count; i++)
            {
                PropertyItems.Remove(list[i]);
            }

            block.Item.IsExpended = false;
        }
        // to open
        else
        {
      
            if(list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    PropertyItems.Insert(index + 1, list[i]);
                }   
            }

            block.Item.IsExpended = true;
        }

        // update the item
        PropertyItemAdapter.SetItems(PropertyItems);    

    }

    ExtensionFilter[] extensionList;

    public void OnClick_ExportModel()
    {

        if (IsExportingOngoing)
        {
            MCPopup.Instance.SetWarning("Ongoing exporting process, please try again later.", "Model Export");
            return;
        }

        // read from input field
        // validate all inputs
        if (Text_ExportFileName.text == "")
        {
            MCPopup.Instance.SetWarning("Please enter file name", "Model Export");
            return;
        }

        _FileName = Text_ExportFileName.text;
        _SavePath = "";

        int exportFormatValue = Dropdown_ExportFormat.value;
        DataExportFormat _formate = DataExportFormat.csv;


        switch (exportFormatValue)
        {
            case 0:
                _FileName = _FileName + ".csv";
                _formate = DataExportFormat.csv;
                extensionList = new[] {
                    new ExtensionFilter("Text", "csv"),
                };
                break;
            case 1:
                _FileName = _FileName + ".json";
                _formate = DataExportFormat.json;
                extensionList = new[] {
                    new ExtensionFilter("Text", "json"),
                };
                break;
            case 2:
                _FileName = _FileName + ".ifc";
                _formate = DataExportFormat.ifc;
                extensionList = new[] {
                    new ExtensionFilter("BIM", "ifc"),
                };
                break;
            case 3:
                _FileName = _FileName + ".dxf";
                _formate = DataExportFormat.dxf;
                extensionList = new[] {
                    new ExtensionFilter("CAD", "dxf"),
                };
                break;
            default:
                _FileName = _FileName + ".csv";
                _formate = DataExportFormat.csv;
                extensionList = new[] {
                    new ExtensionFilter("Text", "csv"),
                };
                break;
        }
        string _prefix = DateTime.Now.ToString("yyyyMMddHHmmss_");
        _FileName = _prefix + _FileName;

        var path = StandaloneFileBrowser.SaveFilePanel("Export Model", "", _FileName, extensionList);

        // Select path 
        int exportElementValue = Dropdown_ExportSelection.value;



        if (path != "")
        {
            _SavePath = path;
            OnExportConfig_Callback(true, path, _formate, exportElementValue);
        }

    }


    public void OnExportConfig_Callback(bool _result, string _message, DataExportFormat _format, int _selection)
    {
        if (_result)
        {
            exportDataItems.Clear();

            // collect objects tp export
            if (_selection == 2)
            {
                foreach(var ob in ProjectModelHandler.Instance.SelectedElements)
                {
                    var element = ob.GetComponent<BIMElement>();

                    if (ob != null)
                    {
                        exportDataItems.Add(element.BimObject);
                    }
                }
            }
            else
            {
                foreach(var item in ProjectModelHandler.Instance.CurrentModel.bimModel.Structures)
                {
                    if(item.element != null) 
                    { 
                        if(_selection == 1)
                        {
                            if (!item.element.IsElementHide)
                            {
                                exportDataItems.Add(item.element.BimObject);
                            }
                        }
                        // selection == 0
                        else
                        {
                            exportDataItems.Add(item.element.BimObject);
  
                        }
                    }
                }
            }

            IsExportingOngoing = true;
            ExportCount = exportDataItems.Count;
            _ExportCount = ExportCount;

            Debug.Log("["+ _selection + "]Export Start, Object colleced: " + _ExportCount);

            if(ExportCount == 0)
            {
                MCPopup.Instance.SetWarning("No element is available to export.", "Model Export");
            }

            StartExport(_message, _format);

        }
        else
        {

        }
    }


    public void OnExportComplete_Callback(bool _result)
    {
        if (_result)
        {        // close progress
            MCLoadingBar.Instance.OnLoadingComplete();
            IsExportingOngoing = false;
            MCPopup.Instance.SetNotification(OnOpenExportPath, _SavePath, "Export Completed", "Open Folder", MCPopup.NotificationIconType.message_complete);
        }
        else
        {

        }
    }


    public void OnOpenExportPath(bool _result, string _path)
    {
        if (_result)
        {
            UnityEngine.Application.OpenURL(Path.GetDirectoryName(_path));
        }
    }



#if (UNITY_WEBGL && !UNITY_EDITOR)
        
#else
    private Task CurrentTask;
    private CancellationTokenSource Logic1CancelerTokenSrc = new CancellationTokenSource();
#endif
    public void StartExport(string _savePath, DataExportFormat _formate)
    {
        Debug.Log("StartExportTo: " + _savePath);
        SavePath = _savePath;
        _SavePath = SavePath;
        ExportProgress = 0;
        

        MCLoadingBar.Instance.OnSetLoadingProgress(_ExportProgress);

#if (UNITY_WEBGL && !UNITY_EDITOR)
        
        if(_formate == DataExportFormat.csv)
        {
            StartCoroutine(ExportCSV());    
        }
        else 
        if(_formate == DataExportFormat.json)
        { 
            StartCoroutine(ExportJson());
        }

#else
        if (CurrentTask != null && CurrentTask.Status.Equals(TaskStatus.Running))
        {
            MCPopup.Instance.SetWarning("Ongoing exporting process, please try again later.", "Data Export");
            return;
        }

        try
        {
            Debug.Log("_formate: " + _formate);


            if (CurrentTask != null && CurrentTask.Status.Equals(TaskStatus.Running))
            {
                CurrentTask.Dispose();
            }

            CurrentTask = new Task(() =>
            {
                try
                {
                    if (_formate == DataExportFormat.csv)
                    {
                        ExportCSV();
                    }
                    else
                    if (_formate == DataExportFormat.json)
                    {
                        ExportJson();
                    }
                }
                catch(Exception e)
                {
                    IsThreadException = true;
                    _ExceptingString = _DebugString + "\n" + e.StackTrace;
                }
            });

            CurrentTask.Start();


        }
        catch (Exception ex)
        {
            MCPopup.Instance.SetWarning("Unknow Task Error", "Data Export");
            Debug.LogError("ExportInStandalone: " + ex.Message);
        }

#endif



    }

    public List<MetaBIM_Property> ExportingProperties = new List<MetaBIM_Property>();



#if (UNITY_WEBGL && !UNITY_EDITOR)
    public IEnumerator ExportJson()
#else
    public void ExportJson()
#endif
    {
        BimModel model = ProjectModelHandler.Instance.CurrentModel.bimModel;
        ExportDataItem exportData = new ExportDataItem();

        exportData.modelName = System.IO.Path.GetFileName(_SavePath);
        exportData.records = new List<BimObjectRecord>();
        ExportProgress = 1;

        foreach (var structure in model.Structures)
        {

            if (structure.element != null)
            {
                if (structure.element.IsElementHide && ProjectConfiguration.Instance.IsExportHideElement)
                {
                    continue;
                }

                if (structure.element.BimObject != null)
                {
                    if (structure.element.BimObject.records.Count == 0)
                    {
                        continue;
                    }
                    else
                    {
                        exportData.records.Add(structure.element.BimObject.records[0]);
                    }
                }


#if (UNITY_WEBGL && !UNITY_EDITOR)
             yield return new WaitForEndOfFrame(); 
#else

#endif
            }

            ExportProgress++;
        }


        string json = JsonUtility.ToJson(exportData, true);

        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }

        File.WriteAllText(SavePath, json);


    }


#if (UNITY_WEBGL && !UNITY_EDITOR)
    public IEnumerator ExportCSV()
#else
    public void ExportCSV()
#endif
    {
        IsExportingOngoing = true;

        ExportingProperties = new List<MetaBIM_Property>();

        int index = 0;
        _DebugCount = exportDataItems.Count;

        foreach (var item in exportDataItems)
        {
            //_DebugCount++;
            if (Logic1CancelerTokenSrc.IsCancellationRequested)
            {
                return;
            }

            if (item.records.Count != 0)
            {
                var row = new MetaBIM_Property(index, item);
                GetElementExportData_CSV(row, PropertyCategory);

                ExportingProperties.Add(row);
                index++;
            }

            ExportProgress++;

#if (UNITY_WEBGL && !UNITY_EDITOR)
            yield return new WaitForEndOfFrame(); 
#else

#endif
        }

   

        // create header
        var allKeys = new HashSet<string>();

        foreach (var obj in ExportingProperties)
        {
            if (Logic1CancelerTokenSrc.IsCancellationRequested)
            {
                return;
            }

            foreach (var key in obj.ExportResult.Keys)
            {
                allKeys.Add(key);
            }
        }

   

        var csv = new StringBuilder();
        csv.AppendLine(string.Join(",", allKeys)); // Header



        // create row
        foreach (var obj in ExportingProperties)
        {
            if (Logic1CancelerTokenSrc.IsCancellationRequested)
            {
                return;
            }
            // Each ro
            // w
            var row = allKeys.Select(key => obj.ExportResult.ContainsKey(key) ? $"\"{obj.ExportResult[key]}\"" : "None").ToArray();
            
            csv.AppendLine(string.Join(",", row));
        }
 

        // Create new exporting file
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }

        File.WriteAllText(SavePath, csv.ToString());



#if (UNITY_WEBGL && !UNITY_EDITOR)
        IsExportingOngoing = false;
#else
        ExportProgress = ExportCount;
#endif

    }


    public void GetElementExportData_CSV(MetaBIM_Property _Property, List<BIMExportPropertyItem> _exportPrioritis)
    {

        // must have
        _Property.ExportResult.Add("index", _Property.mappedID.ToString());
        _Property.ExportResult.Add("elementName", _Property.elementName);
        _Property.ExportResult.Add("elementID", _Property.elementID);

        foreach (var item in _exportPrioritis) // from category
        {
            if (item.IsExport)
            {
                if (!PropertyItemGroups.ContainsKey(item.PropertyName))
                {
                    break;

                }

                List<BIMExportPropertyItem> list = PropertyItemGroups[item.PropertyName];


                string methodName = "Convert_CSV_" + item.PropertyName.Replace(" ", "");
                DebugString = "Calling: " + methodName + ", " + _Property.elementName;

                var method = this.GetType().GetMethod(methodName);

                if (method != null)
                {
                    method.Invoke(this, new object[] { _Property, list});
                }

            }
        }
    }



    #region Convert CSV

    public bool CheckIsExport(List<BIMExportPropertyItem> _exportPrioritis, string _key)
    {
        foreach (var item in _exportPrioritis)
        {
            if (item.PropertyName == _key)
            {
                if (item.IsExport)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void Convert_CSV_MetaBIMProperty(MetaBIM_Property _Property, List<BIMExportPropertyItem> _exportPrioritis)
    {
        if(_Property.generalItems == null)
        {
            return;
        }

        for (int i = 0; i < _Property.generalItems.Count; i++)
        {
            string key = _Property.generalItems[i].key;

            if (CheckIsExport(_exportPrioritis, key))
            {
                string value = _Property.generalItems[i].value;
                if (!_Property.ExportResult.TryAdd(key, value))
                {
                    key = "MetaBIM " + key;
                    _Property.ExportResult.TryAdd(key, value);
                }
            }

        }

    }



    public void Convert_CSV_Geometry(MetaBIM_Property _Property, List<BIMExportPropertyItem> _exportPrioritis)
    {

        // TODO
    }

    public void Convert_CSV_Material(MetaBIM_Property _Property, List<BIMExportPropertyItem> _exportPrioritis)
    {
        if (_Property.ifcMaterials == null)
        {
            return;
        }

        for (int i = 0; i < _Property.ifcMaterials.materials.Count; i++)
        {
            string key = _Property.ifcMaterials.materials[i];

            if (CheckIsExport(_exportPrioritis, key))
            {
                string value = _Property.ifcMaterials.thicknesses[i];
                if (!_Property.ExportResult.TryAdd(key, value))
                {
                    key = "IFC Material " + key;
                    _Property.ExportResult.TryAdd(key, value);
                }
            }

        }

    }


    public void Convert_CSV_Zoning(MetaBIM_Property _Property, List<BIMExportPropertyItem> _exportPrioritis)
    {
        if (_Property.ifcZone == null)
        {
            return;
        }

        for (int i = 0; i < _Property.ifcZone.attributes.Count; i++)
        {
            string key = _Property.ifcZone.attributes[i];

            if (CheckIsExport(_exportPrioritis, key))
            {
                string value = _Property.ifcZone.values[i];
                if (!_Property.ExportResult.TryAdd(key, value))
                {
                    key = "Zone " + key;
                    _Property.ExportResult.TryAdd(key, value);
                }
            }

        }
    }


    public void Convert_CSV_Validation(MetaBIM_Property _Property, List<BIMExportPropertyItem> _exportPrioritis)
    {
        if (_Property.IfcValidation == null)
        {
            return;
        }

        for (int i = 0; i < _Property.IfcValidation.checkedItem.Count; i++)
        {
            string key = _Property.IfcValidation.checkedItem[i];

            if (CheckIsExport(_exportPrioritis, key))
            {
                string value = _Property.IfcValidation.result[i];
                if (!_Property.ExportResult.TryAdd(key, value))
                {
                    key = "Validation " + key;
                    _Property.ExportResult.TryAdd(key, value);
                }
            }

        }

    }


    public void Convert_CSV_Uniclass(MetaBIM_Property _Property, List<BIMExportPropertyItem> _exportPrioritis)
    {
        if (_Property.ifcUniclass == null || _Property.ifcUniclassMap == null)
        {
            return;
        }

        for (int i = 0; i < _Property.ifcUniclass.attributes.Count; i++)
        {
            string key = _Property.ifcUniclass.attributes[i];

            if (CheckIsExport(_exportPrioritis, key))
            {
                string value = _Property.ifcUniclass.values[i];
                if (!_Property.ExportResult.TryAdd(key, value))
                {
                    key = "Uniclass " + key;
                    _Property.ExportResult.TryAdd(key, value);
                }
            }

        }

        for (int i = 0; i < _Property.ifcUniclassMap.attributes.Count; i++)
        {
            string key = _Property.ifcUniclassMap.attributes[i];

            if (CheckIsExport(_exportPrioritis, key))
            {
                string value = _Property.ifcUniclassMap.values[i];
                if (!_Property.ExportResult.TryAdd(key, value))
                {
                    key = "Uniclass " + key;
                    _Property.ExportResult.TryAdd(key, value);
                }
            }

        }
    }


    public void Convert_CSV_IFCParameter(MetaBIM_Property _Property, List<BIMExportPropertyItem> _exportPrioritis)
    {
        if (_Property.ifcParameter == null)
        {
            return;
        }

        for (int i = 0; i < _Property.ifcParameter.attributes.Count; i++)
        {
            string key = _Property.ifcParameter.attributes[i];

            if (CheckIsExport(_exportPrioritis, key))
            {
                string value = _Property.ifcParameter.values[i];
                if (!_Property.ExportResult.TryAdd(key, value))
                {
                    key = "IFC Parameter " + key;
                    _Property.ExportResult.TryAdd(key, value);
                }
            }

        }


    }

 
    public void Convert_CSV_IFCAttribute(MetaBIM_Property _Property, List<BIMExportPropertyItem> _exportPrioritis)
    {
        if (_Property.ifcAttribute == null)
        {
            return;
        }

        for (int i = 0; i < _Property.ifcAttribute.attributes.Count; i++)
        {
            string key = _Property.ifcAttribute.attributes[i];

            if (CheckIsExport(_exportPrioritis, key))
            {
                string value = _Property.ifcAttribute.values[i];
                if (!_Property.ExportResult.TryAdd(key, value))
                {
                    key = "IFC Attribute " + key;
                    _Property.ExportResult.TryAdd(key, value);
                }
            }

        }

    }


    public void Convert_CSV_IFCProperty(MetaBIM_Property _Property, List<BIMExportPropertyItem> _exportPrioritis)
    {
        if (_Property.ifcProperties == null)
        {
            return;
        }

        for (int i = 0; i < _Property.ifcProperties.properties.Count; i++)
        {
            string key = _Property.ifcProperties.properties[i];

            if (CheckIsExport(_exportPrioritis, key))
            {
                string value = _Property.ifcProperties.nominalValues[i];
                if(!_Property.ExportResult.TryAdd(key, value))
                {
                    key = "IFC Property " + key;
                    _Property.ExportResult.TryAdd(key, value);
                }
            }

        }




    }


    public void Convert_CSV_IFCType(MetaBIM_Property _Property, List<BIMExportPropertyItem> _exportPrioritis)
    {
        if (_Property.ifcTypes == null)
        {
            return;
        }

        for (int i = 0; i < _Property.ifcTypes.types.Count; i++)
        {
            string key = _Property.ifcTypes.types[i];

            if (CheckIsExport(_exportPrioritis, key))
            {
                string value = _Property.ifcTypes.values[i];
                if (!_Property.ExportResult.TryAdd(key, value))
                {
                    key = "IFC Type " + key;
                    _Property.ExportResult.TryAdd(key, value);
                }
            }

        }
    }


    #endregion






    public void Update()
    {
        //Debug.Log("Items: " + PropertyItemGroups.Count);

#if (UNITY_WEBGL && !UNITY_EDITOR)

#else
        if (IsExportingOngoing)
        {
            _ExportProgress = ExportProgress;

            _DebugString = DebugString;
            _DebugCount++;


            float progress = (float)_ExportProgress / (float)ExportCount;
            //Debug.Log("Export Progress: " + progress);

            MCLoadingBar.Instance.OnSetLoadingProgress(progress);

            if (_ExportProgress >= _ExportCount)
            {
                IsExportingOngoing = false;
                OnExportComplete_Callback(true);
            }

            if (IsThreadException)
            {
                IsThreadException = false;
                IsExportingOngoing = false;

                if (CurrentTask != null)
                {
                    Logic1CancelerTokenSrc.Cancel();
                }

                // display notification?
                MCPopup.Instance.SetNotification(null, "", "Export failed (x400101).", "OK", MCPopup.NotificationIconType.message_information);
            }
        }

#endif 
    }





    private void OnApplicationQuit()
    {
#if (UNITY_WEBGL && !UNITY_EDITOR)

#else

        if (CurrentTask != null)
        {
            Logic1CancelerTokenSrc.Cancel();
        }
#endif
    }





    public enum DataExportFormat
    {
        csv,
        json,
        ifc,
        dxf,
    }



    public void GeneratePropertList()
    {
        // gen
    }


    public BIMExportPropertyItem GetBIMExportPropertyItem(int id)
    {
        for(int i =0; i< PropertyItems.Count; i++)
        {
            if (PropertyItems[i].PropertyID == id)
            {
                return PropertyItems[i];
            }
        }

        return null;
    }


    public bool CheckPropertyItemInList(List<BIMExportPropertyItem> _propertItem, string _name)
    {

        foreach (var item in _propertItem)
        {
            if (item.PropertyName == _name)
            {
                return true;
            }
        }

        return false;
    }
}


[Serializable]
public class BIMExportPropertyItem
{
    public int PropertyID;
    public string PropertyName;
    public int ParentID;
    public bool IsExport = true;
    public bool IsExpended = false;
    public bool IsSubItem = false;  
}



[Serializable]
public class ExportDataItem
{
    public string modelName;
    public string modelInfo;
    public List<BimObjectRecord> records = new List<BimObjectRecord>();
}



[Serializable]
public class ExportDataCSV
{
    public string modelName;
    public string modelInfo;
    public List<BimObjectRecord> records = new List<BimObjectRecord>();
    public List<string> header = new List<string>();
    public List<string> rows = new List<string>();
}


[Serializable]
public class BIMExportCell
{
    public string cellName;
    public string cellValue;
}








