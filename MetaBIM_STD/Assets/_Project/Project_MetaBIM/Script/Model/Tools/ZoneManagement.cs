using IfcToolkit.IfcSpec;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



namespace MetaBIM
{

    public class ZoneManagement : MonoBehaviour
    {
        public static ZoneManagement Instance;
        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            MainPanel.OnOpenAction = OnOpenAction;
            MainPanel.OnCloseAction = OnCloseAction;
        }


        [Header("UI Element")]
        public PanelChange MainPanel;
        public ZoneManangementZoneBoxAdapter ZoneBoxAdapter;
        public ZoneManangementSelectedElementAdapter SelectedElementAdapter;

        public TMP_InputField Text_SelectedZoneName;
        public Transform ZoneLabelParent;
        public RectTransform CanvasRect;

        [Header("Data Buffer")]
        public List<ZoneItem> zoneItems = new List<ZoneItem>();
        public Dictionary<string, List<ElementZone>> zones= new Dictionary<string, List<ElementZone>>();
        public GameObject SelectedZone;
        public GameObject SelectedZoneElement;

        [Header("Status")]
        public float DragInterval = 1f;  // need to move to configuration
        public int CurrentModelCount = -1;
        public bool IsEditing = false;
        public bool IsZoneOpened = false;
        public bool IsZoneDragging = false;


        public void OnOpenAction()
        {
            IsZoneOpened = true;


            // load zone items from database

            // loop through zone items to:
            // created zone box item in scenen using prefabs
            // create zone item in scroll viewer

            OnRequestLoadZones();
        }


        public void OnCloseAction()
        {
            IsZoneOpened = false;
            SelectedZone = null;

            DestroyZoneItems();
        }


        public void DestroyZoneItems()
        {
            foreach (var ob in zoneItems)
            {
                if (ob != null)
                {
                    Destroy(ob.gameObject);
                }
            }

            zoneItems.Clear();
        }


        public void Update()
        {
            if (!IsEditing)
            {
   
            }

        }

        public void OnRequestLoadZones()
        {
            // get all actived model
            List<string> models = ProjectModelHandler.Instance.GetActiveModelGuids();
            CurrentModelCount = models.Count;


            if (models.Count < 1)
            {
                // no models, but will this really happen?
                MCPopup.Instance.SetInformation("No model is loaded");
                OnClick_Close();
                return;
            }
            zones.Clear();

            foreach (var item in models)
            {

                StartCoroutine(DataProxy.Instance.GetElementZones("attachedProject", item, OnRequestLoadZones_Callback));
            }

        }


        public void OnRequestLoadZones_Callback(bool _result, string _message)
        {
            CurrentModelCount = CurrentModelCount - 1;

            if (_result)
            {              
                var response = DataProxyResponse<ElementZone>.FromJson(_message);

                if (response.result)
                {
                    foreach (var item in response.package)
                    {
                        if (!zones.ContainsKey(item.attachedProject))
                        {
                            zones.Add(item.attachedProject, new List<ElementZone>());
                        }

                        zones[item.attachedProject].Add(item);
                    }
                }
          
                if(CurrentModelCount == 0)
                {
                    InitZoneItems();
                }
            }
            else
            {
                Debug.Log(_message);
                MCPopup.Instance.SetInformation("Load zone information failed, please try again later.");
            }
        }


        public void InitZoneItems()
        {
            // create and generate zone items
            OnGenerateItemBlock();

            // aquire all selected elements
            AquireItemsForZone();

            // render zone item in scroll view (UI) and scene
            OnRenderZoneItemBlock();
        }




        public void OnGenerateItemBlock()
        {
            List<ElementZone> zoneList = new List<ElementZone>();
            DestroyZoneItems();

            foreach (var item in zones)
            {
                foreach(var zone in item.Value)
                {
                    zoneList.Add(zone);
                    ZoneItem ob = GenerateZoneItem(zone);

                    zoneItems.Add(ob);
                }
            }

            ZoneBoxAdapter.SetItems(zoneList);
        }


        public void AquireItemsForZone()
        {
            foreach (var pair in zones)
            {
                var model = ProjectModelHandler.Instance.GetActiveModelByGuid(pair.Key);
                if (model != null)
                {
                    foreach (var ob in model.Structures)
                    {
                        if (ob.element != null)
                        {
                            foreach (var zoneItem in zoneItems)
                            {
                                if (zoneItem.Item.elements.Contains(ob.element.BimObject.elementID))
                                {
                                    zoneItem.selectedElements.Add(ob.element.gameObject);
                                }
                            }
                        }
                    }
                }
            }
        }


        public void OnRenderZoneItemBlock()
        {
            foreach (var item in zoneItems)
            {
                item.InitBoxFromSelectedElement();   // only the selection mode is enabled for now, need to be conditional later
                item.IsEditing = false;
                item.EndEditing();

                item.ZoneLabel.Text_ZoneName.text = item.Item.zoneName;
                item.ZoneLabel.gameObject.SetActive(true);
            }
        }



        #region Zone UI Actions

        public void OnClick_Open()
        {
            MainPanel.OnPanelOpen();
        }


        public void OnClick_Close()
        {
            MainPanel.OnPanelClose();
        }


        public void OnClick_AddNewZone()
        {
            // open editor panel
            // create a new zone item
            // which model is the zone attached to

            OnEditingPanelOpen();
        }


        public void OnClick_EditZone()
        {
            if (SelectedZone == null)
            {
                MCPopup.Instance.SetInformation(StringBuffer.Zone_NoZoneItemSelect.S);
                return;
            }

     
            OnEditingPanelOpen(SelectedZone.GetComponent<UIBlock_BimViewer_ZoneManagement_ZoneBoxItem>().Item);
        }


        public void OnClick_RemoveZone()
        {
            if (SelectedZone == null)
            {
                MCPopup.Instance.SetInformation(StringBuffer.Zone_NoZoneItemSelect.S);
                return;
            }

            MCPopup.Instance.SetConfirm(OnRemoveZoneConfirm_Callback, "", "Confirm to remove selected zone?");

        }


        public void OnRemoveZoneConfirm_Callback(bool _result, string _message)
        {
            if (_result)
            {
                var zone = SelectedZone.GetComponent<UIBlock_BimViewer_ZoneManagement_ZoneBoxItem>().Item;

                var zoneList = zones[zone.attachedProject]; // may be bug here

                var zoneItem = GetZoneItemByGuid(zone.guid);
                zoneItems.Remove(zoneItem);

                Destroy(zoneItem.gameObject);

                zoneList.Remove(zone);
                LoadingHandler.Instance.OnFullPageLoadingStart("Updating, please wait.");
                StartCoroutine(DataProxy.Instance.DeleteElementZoneByGuid(zone.guid, OnRemoveZone_Callback));
            }
        }


        public void OnRemoveZone_Callback(bool _result, string _message)
        {
            LoadingHandler.Instance.OnFullPageLoadingEnd();
            if (_result)
            {
                SelectedZone = null;
                OnRequestLoadZones();
            }
            else
            {
                Debug.LogError("OnRemoveZone_Callback£º " + _message);
            }
        }


        public void OnClick_SetZoneColor(GameObject gameObject)
        {
            var item = gameObject.GetComponent<UIBlock_BimViewer_ZoneManagement_ZoneBoxItem>();


            // Just for now
            Color color = OnColorPick_Callback(new Color());

            item.Item.zoneColor = new List<float> { color.r, color.g, color.b, color.a };
            item.LinkedObject.GetComponent<ZoneItem>().SetZoneBoxColor(color);  
            item.Text_Icon.color = color;


        }


        public void OnClick_Reload()
        {
            MCPopup.Instance.SetConfirm(OnReloadConfirm_Callback, "", "Reload will lose unsaved changes, to confirm reload?");
        }


        public void OnReloadConfirm_Callback(bool _result, string _message)
        {
            if (_result)
            {
                OnRequestLoadZones();
            }
        }


        public void OnClick_GroupSelectedElement()
        {

        }

        public void OnClick_UpdateElementSelection()
        {
            if (SelectedZone == null)
            {
                MCPopup.Instance.SetInformation(StringBuffer.Zone_NoZoneItemSelect.S);
                return;
            }


        }



        public void OnClick_SelectZoneItem(GameObject gameObject)
        {
            Debug.Log("OnClick_SelectZoneItem: " + gameObject.name);

            if (SelectedZone != null && SelectedZone != gameObject)
            {
                Debug.Log("OnClick_SelectZoneItem: ");
                var _item = SelectedZone.GetComponent<UIBlock_BimViewer_ZoneManagement_ZoneBoxItem>();
                _item.OnDeselect();
            }

            SelectedZone = gameObject;
            var item = SelectedZone.GetComponent<UIBlock_BimViewer_ZoneManagement_ZoneBoxItem>();

            item.OnSelect();
            Text_SelectedZoneName.text = item.Item.zoneName +" [" + item.Item.elements.Count + "]"; 
            CurrentZoneObject = item.LinkedObject;

            UpdateElementViewofSelectedZone();

            //Page_BIMViewer.Instance.SelectViewItem("View Zone");
      

            // Do we need to highlight the selected elements?
            if (item.IsVisable)
            {
                //CloneGameObjectList(CurrentZoneObject.selectedElements, ProjectModelHandler.Instance.SelectedElements);
                Page_BIMViewer.Instance.SelectMeshObjects(CurrentZoneObject.selectedElements);
            }
            else
            {
                Page_BIMViewer.Instance.SelectMeshObject(null, false, false);
            }
        }


        public void UpdateElementViewofSelectedZone()
        {
            List<StructureNode> structureNodes = new List<StructureNode>();

            Debug.Log("UpdateElementViewofSelectedZone: " + CurrentZoneObject.selectedElements.Count);

            if (CurrentZoneObject != null)
            {
                foreach (var li in CurrentZoneObject.selectedElements)
                {
                    structureNodes.Add(li.GetComponent<BIMElement>().LinkedNodeItem);
                }
            }

            SelectedElementAdapter.SetItems(structureNodes);
        }



        public void OnClick_SelectZoneItemBlock(GameObject gameObject)
        {
            
            Debug.Log("OnClick_SelectZoneItemBlock: " + gameObject.name);

            ZoneItem item = gameObject.GetComponent<ZoneItemLabel>().LinkedObject;
            OnClick_SelectZoneItem(item.UIBlock.gameObject);

            //if (SelectedZone != null)
            //{
            //    SelectedZone.GetComponent<UIBlock_BimViewer_ZoneManagement_ZoneBoxItem>().OnDeselect();
            //}



            //item.UIBlock.OnSelect();

            //SelectedZone = item.UIBlock.gameObject;
            //Text_SelectedZoneName.text = item.Item.zoneName + " [" + item.Item.elements.Count + "]";
            //CurrentZoneObject = item.UIBlock.LinkedObject;

            //Page_BIMViewer.Instance.SelectViewItem("View Zone");
        }



        public void OnToggle_ZoneVisiable(GameObject _gameObject)
        {
            _gameObject.GetComponent<UIBlock_BimViewer_ZoneManagement_ZoneBoxItem>().OnSetBoxVisiable();
        }


        public void OnRightClickMenu_AddElementToZone()
        {
            if (!IsZoneOpened)
            {
                MCPopup.Instance.SetInformation("No zone is selected");
                return;
            }

            if (CurrentZoneObject == null)
            {
                MCPopup.Instance.SetInformation("No zone is selected");
                return;
            }


            // the elements may from different models
            List<GameObject> selected = new List<GameObject>();
            CloneGameObjectList(ProjectModelHandler.Instance.SelectedElements, selected);
            var existed  = CurrentZoneObject.Item.elements;

            string modelGuid = CurrentZoneObject.Item.attachedProject;

            Bounds newbounds = new Bounds();
            newbounds.center = Vector3D.FromVecter3D(CurrentZoneObject.Item.zoneCenter);
            newbounds.extents = Vector3D.FromVecter3D(CurrentZoneObject.Item.zoneExtense);

            int index = 0;


            foreach(var _item in CurrentZoneObject.selectedElements)
            {
                if(index == 0)
                {
                    newbounds = _item.GetComponent<BIMElement>().Collider.bounds;
                }
                else
                {
                    newbounds.Encapsulate(_item.GetComponent<BIMElement>().Collider.bounds);
                }
                index++;
            }

            //Debug.Log("Select zone to add: " + selected.Count);


            foreach (var item in selected)
            {
                var element = item.GetComponent<BIMElement>();
                if(element.BimObject.attachedProject == modelGuid)
                {
                    Debug.Log("Selected Element: " + CurrentZoneObject.selectedElements.Count);

                    if (!existed.Contains(element.BimObject.elementID))   // this line may casue performance issue
                    {
                        CurrentZoneObject.selectedElements.Add(item);
                        existed.Add(element.BimObject.elementID);
                        Debug.Log("New item : " + item.name);


                        if (index == 0)
                        {
                            newbounds = item.GetComponent<BIMElement>().Collider.bounds;
                        }
                        else
                        {
                            newbounds.Encapsulate(item.GetComponent<BIMElement>().Collider.bounds);
                        }

                        ApplyZoneInformationToElement(CurrentZoneObject.Item, element);

                        index++;
                    }


        
                }
            }
            //Debug.Log("----------");
            //Debug.Log("Selected zone Element: " + CurrentZoneObject.selectedElements.Count);
            //Debug.Log("Selected view Element: " + ProjectModelHandler.Instance.SelectedElements.Count);



            CloneGameObjectList(CurrentZoneObject.selectedElements, ProjectModelHandler.Instance.SelectedElements);

            Page_BIMViewer.Instance.SelectMeshObjects(ProjectModelHandler.Instance.SelectedElements);
            //Debug.Log("Selected zone Element: " + CurrentZoneObject.selectedElements.Count);


            CurrentZoneObject.InitBoxFromSelectedElementBound(newbounds);
            //Debug.Log("Selected zone Element: " + CurrentZoneObject.selectedElements.Count);

            // update list in zone view
            List<StructureNode> structureNodes = new List<StructureNode>();

            foreach(var item in CurrentZoneObject.selectedElements)
            {
                structureNodes.Add(item.GetComponent<BIMElement>().LinkedNodeItem);
            }
            //Debug.Log("Selected zone Element: " + CurrentZoneObject.selectedElements.Count);


            SelectedElementAdapter.SetItems(structureNodes);
            // update zone header

            Text_SelectedZoneName.text = CurrentZoneObject.Item.zoneName + " [" + CurrentZoneObject.selectedElements.Count+ "]";

            //Log("Selected zone Element: " + CurrentZoneObject.selectedElements.Count);
            // update zone information

            UpdateSelectedElements();
            //Debug.Log("Selected zone Element: " + CurrentZoneObject.selectedElements.Count);

            StartCoroutine(DataProxy.Instance.UpdateElementZone(ElementZone.ToJson(CurrentZoneObject.Item), OnRequestUpdateZone_Callback));

            Page_BIMViewer.Instance.CloseRightClickMenu();
            //Debug.Log("Selected zone Element: " + CurrentZoneObject.selectedElements.Count);
        }



        public void OnRightClickMenu_ResetElementToZone()
        {
            if (!IsZoneOpened)
            {
                MCPopup.Instance.SetInformation("No zone is selected");
                UIController_RightClickMenu.Instance.OnMenuClose();
                return;
            }

            if (CurrentZoneObject == null)
            {
                MCPopup.Instance.SetInformation("No zone is selected");
                UIController_RightClickMenu.Instance.OnMenuClose();
                return;
            }


            // the elements may from different models
            var selected = ProjectModelHandler.Instance.SelectedElements;
            var existed = CurrentZoneObject.Item.elements;

            string modelGuid = CurrentZoneObject.Item.attachedProject;

            Bounds newbounds = new Bounds();


            int index = 0;

            //remove zone information from element
            foreach (var item in CurrentZoneObject.selectedElements)
            {
                var element = item.GetComponent<BIMElement>();
                ApplyInitZoneInformationToElement(element);
            }

            // clear selected elements
            CurrentZoneObject.selectedElements.Clear();
            existed.Clear();

            
            foreach (var item in selected)
            {
                var element = item.GetComponent<BIMElement>();
                if (element.BimObject.attachedProject == modelGuid)
                {
                    if (index == 0)
                    {
                        newbounds = item.GetComponent<BIMElement>().Collider.bounds;
                    }
                    else
                    {
                        newbounds.Encapsulate(item.GetComponent<BIMElement>().Collider.bounds);
                    }

                    CurrentZoneObject.selectedElements.Add(item);
                    existed.Add(element.BimObject.elementID);
                    ApplyZoneInformationToElement(CurrentZoneObject.Item, element);

                    index++;
                }
            }



            CloneGameObjectList(CurrentZoneObject.selectedElements, ProjectModelHandler.Instance.SelectedElements);
            Page_BIMViewer.Instance.SelectMeshObjects(ProjectModelHandler.Instance.SelectedElements);

            CurrentZoneObject.InitBoxFromSelectedElementBound(newbounds);


            // update list in zone view
            List<StructureNode> structureNodes = new List<StructureNode>();

            foreach (var item in CurrentZoneObject.selectedElements)
            {
                structureNodes.Add(item.GetComponent<BIMElement>().LinkedNodeItem);
            }

            SelectedElementAdapter.SetItems(structureNodes);
            // update zone header

            Text_SelectedZoneName.text = CurrentZoneObject.Item.zoneName + " [" + CurrentZoneObject.selectedElements.Count + "]";

            UIController_RightClickMenu.Instance.OnMenuClose();


            Page_BIMViewer.Instance.CloseRightClickMenu();
        }



        public void OnClick_SelectZoneBoxElement(GameObject gameObject)
        {
            // unselect last selected element if any
            if(SelectedZoneElement != null)
            {
                SelectedZoneElement.GetComponent<UIBlock_BimViewer_ZoneManagement_ZoneSelectedItem>().OnDeselected();
            }

            SelectedZoneElement = gameObject;

            // set the selected element to current selected element
            var item  = SelectedZoneElement.GetComponent<UIBlock_BimViewer_ZoneManagement_ZoneSelectedItem>();
            item.OnSelected();

            // set the selected element to call onselect
            if(item.Item.element != null)
            {
                ProjectModelHandler.Instance.SelectedElements.Clear();
                ProjectModelHandler.Instance.SelectedElements.Add(item.Item.element.gameObject);
                Page_BIMViewer.Instance.SelectMeshObjects(ProjectModelHandler.Instance.SelectedElements);
            }


            // trace back the item to tree list

            // highlight the selected element in view


        }


        #endregion


        #region Zone Editing Function
        [Header("Editor UI Element -------------------")]

        public PanelChange EditorPanel;
        public TMP_InputField Text_ZoneName;
        public TMP_InputField Text_ZoneDescription;
        public TMP_Dropdown Dropdown_AttachedProject;
        public TMP_Dropdown Dropdown_ZoneType;
        public TMP_Dropdown Dropdown_SelectionMode;


        public Dictionary<string, string> ModelNames;

        public List<string> TypeSelection_EN;
        public List<string> TypeSelection_CH;

        public List<string> ModeSelection_EN;
        public List<string> ModeSelection_CH;


        [Header("Editor BUffer")]
        //public Zone CurrentEditingZone;
        public bool IsNewZone = false;
        public ZoneItem CurrentZoneObject;
        public void OnEditingPanelOpen(ElementZone _zone = null)
        {
            EditorPanel.OnPanelOpen();

            ModelNames = ProjectModelHandler.Instance.GetActiveModelNames();

            List<string> models = new List<string>();
            string defaultModel = "";

            foreach (var item in ModelNames)
            {
                if(defaultModel == "")
                {
                    defaultModel = item.Value;

                }
                models.Add(item.Key);
            }

            Dropdown_AttachedProject.ClearOptions();
            Dropdown_AttachedProject.AddOptions(models);
            List<StructureNode> structureNodes = new List<StructureNode>();

            if (_zone == null)
            {
                IsNewZone = true;
                ElementZone CurrentEditingZone = new ElementZone();
                CurrentEditingZone.zoneColor = new List<float> { Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 0.1f };
                
                // TODO, Create a new zone item in scene and add to list

                CurrentZoneObject = Instantiate(ResourceHolder.Instance.GetPrefabItem("Tool_ZoneItem")).GetComponent<ZoneItem>();

                CurrentZoneObject.Item = CurrentEditingZone;

                CloneGameObjectList(ProjectModelHandler.Instance.SelectedElements, CurrentZoneObject.selectedElements);
                // create a new bound by selection
                Bounds targetBound = new Bounds();

                if (ProjectModelHandler.Instance.SelectedElements.Count == 0)
                {
                    //var bimObject = ProjectModelHandler.Instance.GetActiveModelByGuid(defaultModel);
                    //targetBound = bimObject.CombinedBound;
                    //structureNodes = bimObject.Structures;  // enable this will add all elements in model to zone
                }
                else
                {
                    int index = 0;
                    foreach (var ob in CurrentZoneObject.selectedElements)
                    {
                        var element = ob.GetComponent<BIMElement>();

                        if (element.Collider != null)
                        {

                            if (index == 0)
                            {
                                targetBound = element.Collider.bounds;
                            }
                            else
                            {
                                targetBound.Encapsulate(element.Collider.bounds);
                            }

                            index++;

                            structureNodes.Add(element.LinkedNodeItem);  

                        }
                    }
                }


                CurrentZoneObject.InitBoxFromSelectedElementBound(targetBound);
      
            }
            else
            {
                IsNewZone = false;
                CurrentZoneObject = GetZoneItemByGuid(_zone.guid);
            }

            SelectedElementAdapter.SetItems(structureNodes);
            CurrentZoneObject.StartEditing();
   


            // render zone information
            OnInitEditingPanel();
        }


        public void OnInitEditingPanel()
        {
            Text_ZoneName.text = CurrentZoneObject.Item.zoneName;
            Text_ZoneDescription.text = CurrentZoneObject.Item.zoneDescription;



            Dropdown_ZoneType.ClearOptions();
            Dropdown_SelectionMode.ClearOptions();

            if (ProjectConfiguration.Instance.DefaultLanguage == ProjectConfiguration.LocationType.ZH)
            {
                Dropdown_ZoneType.AddOptions(TypeSelection_CH);
                Dropdown_SelectionMode.AddOptions(ModeSelection_CH);
            }
            else
            {
                Dropdown_ZoneType.AddOptions(TypeSelection_EN);
                Dropdown_SelectionMode.AddOptions(ModeSelection_EN);
            }


            // set dropdown value
            Dropdown_AttachedProject.value = Dropdown_AttachedProject.options.FindIndex(option => option.text == CurrentZoneObject.Item.attachedProject);
            Dropdown_ZoneType.value = Dropdown_ZoneType.options.FindIndex(option => option.text == CurrentZoneObject.Item.zoneType);
            Dropdown_SelectionMode.value = Dropdown_SelectionMode.options.FindIndex(option => option.text == CurrentZoneObject.Item.elementSelection);


            // TODO, set color?


            foreach(var zoneItem in zoneItems)
            {
                zoneItem.SetZoneBoxUnvisable();
            }

            CurrentZoneObject.SetZoneBoxVisable();
            IsEditing = true;
        }


        public ZoneItem GenerateZoneItem(ElementZone _zone)
        {
            var CurrentZoneObject = Instantiate(ResourceHolder.Instance.GetPrefabItem("Tool_ZoneItem")).GetComponent<ZoneItem>();
            CurrentZoneObject.Item = _zone;
            return CurrentZoneObject;
        }



        public void OnClick_Cancel()
        {
 
            if (IsNewZone)
            {
                Destroy(CurrentZoneObject.gameObject);
            }

            CurrentZoneObject = null;
            IsNewZone = false;
            IsEditing = false;

            EditorPanel.OnPanelClose();

            // remove all listed item in view
            SelectedElementAdapter.SetItems(new List<StructureNode>());

        }


        public void OnClick_Submit()
        {
            // fatch data from UI
            if (string.IsNullOrEmpty(Text_ZoneName.text))
            {
                MCPopup.Instance.SetInformation("Zone name cannot be empty");
                return;
            }


            var attachedGUID = ModelNames[Dropdown_AttachedProject.options[Dropdown_AttachedProject.value].text];
            CurrentZoneObject.Item.attachedProject = attachedGUID;
            CurrentZoneObject.Item.elementSelection = Dropdown_SelectionMode.options[Dropdown_SelectionMode.value].text;
            CurrentZoneObject.Item.zoneType = Dropdown_ZoneType.options[Dropdown_ZoneType.value].text;
            CurrentZoneObject.Item.zoneDescription = Text_ZoneDescription.text;

            if (IsZoneItemExist(Text_ZoneName.text, attachedGUID, CurrentZoneObject.Item.guid))
            {
                MCPopup.Instance.SetInformation("Name of zone["+ Text_ZoneName.text + "] exists in selected model");
                return;
            }

            CurrentZoneObject.Item.zoneName = Text_ZoneName.text;


            var zoneItem = CurrentZoneObject.GetComponent<ZoneItem>();

            // ? do we allow element to have multiple zones?

            // update data
            if (IsNewZone)
            {
       
                if (!zones.ContainsKey(CurrentZoneObject.Item.attachedProject))
                {
                    zones.Add(CurrentZoneObject.Item.attachedProject, new List<ElementZone>());

                }

                foreach(var item in zones)
                {
                    if(item.Key == CurrentZoneObject.Item.attachedProject)
                    {
                        item.Value.Add(CurrentZoneObject.Item);
                        break;
                    }
                }
                LoadingHandler.Instance.OnFullPageLoadingStart("Updating, please wait.");


                zoneItems.Add(zoneItem);
                UpdateSelectedElements();
                StartCoroutine(DataProxy.Instance.AddElementZone(ElementZone.ToJson(CurrentZoneObject.Item), OnRequestUpdateZone_Callback));
            }
            else
            {
                UpdateSelectedElements();
                StartCoroutine(DataProxy.Instance.UpdateElementZone(ElementZone.ToJson(CurrentZoneObject.Item), OnRequestUpdateZone_Callback));
            }





            CurrentZoneObject.ZoneLabel.Text_ZoneName.text = CurrentZoneObject.Item.zoneName;
            // TODO, unselect current selected zone ?
            CurrentZoneObject = null;
            IsNewZone = false;
            IsEditing = false;
            zoneItem.EndEditing();

            EditorPanel.OnPanelClose();

        }




        // TODO, update/add zone information to all selected bim elements
        public void UpdateSelectedElements()
        {
            CurrentZoneObject.Item.elements.Clear();

            foreach (var element in CurrentZoneObject.selectedElements)
            {
                CurrentZoneObject.Item.elements.Add(element.GetComponent<BIMElement>().BimObject.elementID);
            }
        }



        public void OnRequestUpdateZone_Callback(bool _result, string _message)
        {
            LoadingHandler.Instance.OnFullPageLoadingEnd();

            if (_result)
            {
                OnRequestLoadZones();
            }
            else
            {
                MCPopup.Instance.SetInformation("Reload zone");
                OnRequestLoadZones();
            }
        }






        public void OnValueChange_ChangeAttachedProject(int _value)
        {
            var item = ModelNames[Dropdown_AttachedProject.options[_value].text];
            CurrentZoneObject.Item.attachedProject = item;
        }

        public void OnValueChange_ChangeSelectionMode(int _value)
        {
            CurrentZoneObject.Item.elementSelection = Dropdown_SelectionMode.options[_value].text;
        }


        public void OnValueChange_ChangeZoneType(int _value)
        {
            CurrentZoneObject.Item.zoneType = Dropdown_ZoneType.options[_value].text;
        }


        // TODO apply information to element in zone
        public void ApplyZoneInformationToElement(ElementZone _zone, BIMElement _element)
        {
            Debug.Log("ApplyZoneInformationToElement: " + _zone.zoneName);

            MetaBIM_IfcZone ifczone = _element.BimObject.records[0].ifcZone;

            ifczone.attributes.Clear();
            ifczone.values.Clear();

            ifczone.SetNewValue("Zone Identifier", _zone.zoneType);
            ifczone.SetNewValue("Zone Name", _zone.zoneName);
            ifczone.SetNewValue("Zone Type", _zone.zoneType);
            ifczone.SetNewValue("Zone Description", _zone.zoneDescription);

            // TODO zone name


        }


        public void ApplyInitZoneInformationToElement(BIMElement _element)
        {
            MetaBIM_IfcZone ifczone = _element.BimObject.records[0].ifcZone;

            ifczone.attributes.Clear();
            ifczone.values.Clear();

            //hard coding for now, need to do localization string for this
            ifczone.SetNewValue("Zone Identifier", "");
            ifczone.SetNewValue("Zone Name", "");
            ifczone.SetNewValue("Zone Type", "");
            ifczone.SetNewValue("Zone Description", "");

            // TODO zone name


        }

        #endregion











        #region MISC


        public ZoneItem GetZoneItemByGuid(string _guid)
        {
            foreach (var item in zoneItems)
            {
                if (item.Item.guid == _guid)
                {
                    return item;
                }
            }

            return null;
        }

        public void ColorPicker()
        {
            // TODO, open color picker assedt
        }

        public Color OnColorPick_Callback(Color color)
        {
            // Just for now
            return new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 0.3f);
        }

        public bool IsZoneItemExist(string _name, string _project, string _zoneGuid)
        {

            if (!zones.ContainsKey(_project))
            {
                return false;
            }


            var list = zones[_project];

            foreach (var zone in list)
            {
                if (zone.zoneName.CompareTo(_name) == 0 && zone.guid != _zoneGuid)
                {
                    return true;
                }
            }

            return false;
        }   

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
        #endregion
    }
}
