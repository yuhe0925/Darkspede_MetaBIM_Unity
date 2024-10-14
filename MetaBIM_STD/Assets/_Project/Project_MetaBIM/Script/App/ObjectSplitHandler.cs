#define Test

using EzySlice;
using Linefy;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;






namespace MetaBIM
{
    public class ObjectSplitHandler : MonoBehaviour
    {
        public static ObjectSplitHandler Instance;

        public bool IsEditingEnabled;
        public PanelChange Panel;

        public RectTransform CanvasRect;

        public BoundBox EditingBound;
        public BIMElement TargetElement;        // the bim element that is being edited
        public Bounds TargetElementBound;
        public StructureNode TargetElementNode;
        public List<string> PlaneTypes;


        public ElementSplit SplitElement;  // the editing data of the element
        // ==========================



        [Header("UI Elements")]

        public TMP_InputField Text_ElementName;
        public TextMeshProUGUI Text_FloatPlaneInfor;
        public RectTransform Rect_FloatPlaneInfor;

        public SplitingPlaneItemAdapter SplitingPlaneItemAdapter;
        public List<GameObject> SplitingPlanes;
        public UIBlock_BimViewer_ObjectSplit_PlaneItem SelectedSplitPlaneUIItem;

        public SubelmentItemAdapter SubelmentItemAdapter;
        public List<StructureNode> SubelmentItems;

        // Plane Information Editing Plane

        public PanelChange Panel_PlaneEditor;
        public TMP_InputField InputField_PlaneName;
        public TMP_InputField InputField_TargetElement;

        // TODO, how to convert to CH? this is a bad way
        public TMP_Dropdown Dropdown_PlaneType;
        public List<string> Dropdown_PlaneType_Items = new List<string>() { "Vertical", "Horizontal"};
        public List<string> Dropdown_PlaneType_Items_CH = new List<string>() { "垂直平面", "水平平面" };

        public TMP_Dropdown Dropdown_PlaneGroup;
        public List<string> Dropdown_PlaneGroup_Items =  new List<string>() { "Model", "Project"};
        public List<string> Dropdown_PlaneGroup_Items_CH = new List<string>() { "模型层", "项目层" };

        public TMP_Dropdown Dropdown_PlaneAlignment;
        public List<string> Dropdown_PlaneAlignment_Items = new List<string>() { "Bound Center"};
        public List<string> Dropdown_PlaneAlignment_Items_CH = new List<string>() { "边界中心点" };


        [Header("Data Buffers")]

            
        public Project CurrentProject;
        public ModelVersion CurrentModelVersion;

        // in plane list
        public GameObject SelectedSplitingPlane;
        public SplitingPlane SelectedSplitPlaneItem;

        // in sub element list
        public GameObject SelectedSplitingSubelement;

        // spliting process
        public List<GameObject> SplitingSubelementPool;
        public List<GameObject> SplitingSubelementPool_ObjectToDestory;
        public List<GameObject> SplitingSubelementPool_Buffer;
        public int CutCount = 0;




        [Header("Spliting Plane Configuration")]
        public float DragInterval = 0;
        public bool IsEditingExitingPlaneInited = true;


        [Header("Data Restore")]
        public ElementSplit OriginalSplitElement;




     
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            Panel.SetAction(OnOpenAction, OnCloseAction);
            Panel_PlaneEditor.SetAction(OnOpenAction_PlaneEditor, OnCloseAction_PlaneEditor);
        }

        public void OnOpenAction()
        {
            // get the elements

            Rect_FloatPlaneInfor.gameObject.SetActive(false);


        }

        public void OnCloseAction()
        {
            Rect_FloatPlaneInfor.gameObject.SetActive(false);
        }



        private void Update()
        {
            if (IsEditingPlane)
            {
                if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
                {
                    // reset plane
                    OnClick_ResetPlane();
                }

                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
                {
                    // redo plane
                    
                }


                Transform center = SelectedSplitPlaneItem.CenterGrab;
                Vector2 screenPos = Camera.main.WorldToScreenPoint(center.position);
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasRect, screenPos, null, out localPoint);
                Rect_FloatPlaneInfor.anchoredPosition = localPoint + new Vector2(80f, -40f);

                if (SelectedSplitPlaneItem.Item.planeType == SplitPlane.PlaneType.vertical)
                {
                    Text_FloatPlaneInfor.text = "H: " + center.position.y.ToString("F2") + "m";

                }
                else
                {

                }
            }


            // display plane information

        }


        // ===========
        // General Process 
        // =========

        /*
        ----- Open and Init --------
        1. click on element, select split element
        2. isolate this element
        3. open element bound 
        4. open spliting UI
        5. setup the right click menu (enable/disable some option button if any)

        ----- Close ---------
        6. click on the exist button on the UI, top left 
        7. restore all object
        8. set the default selected element


        ------ Operation ------
        10. 

         */


        public void OpenPanel(BIMElement _element, Project _project)
        {
            Panel.OnPanelOpen();

            CurrentProject = _project;
            TargetElement = _element;

            CurrentModelVersion = ProjectModelHandler.Instance.GetModelVersion(TargetElement.BimObject.attachedProject);
            Text_ElementName.text = TargetElement.name;

 
            if (CurrentModelVersion != null)
            {
                SplitElement = CurrentModelVersion.GetSplitElmentByGuid(TargetElement.BimObject.elementID);

                if (SplitElement == null)
                {
                    Debug.Log("Add new Split element: ");
                    SplitElement = new ElementSplit(TargetElement.BimObject.elementID, TargetElement.BimObject.attachedVersion, TargetElement.BimObject.attachedProject, TargetElement.BimObject.attachedWorkspace);
                    SplitElement.splitingPlanes = new List<SplitPlane>();

                    CurrentModelVersion.SplitedElements.Add(SplitElement);
                }

                // save the original data, deep copy
                OriginalSplitElement = ElementSplit.FromJson(ElementSplit.ToJson(SplitElement));
            }
            else
            {
                MCPopup.Instance.SetInformation("Can not find the model version of the element");
                return;
            }

            Debug.Log("Object editing: " + TargetElement.name);


            IsEditingEnabled = true;

            // set bound box for the element
            EditingBound.InitBox(TargetElement.Renderer.bounds, Vector3.zero, Vector3.zero);
            TargetElementBound = EditingBound.SavedBound;



            // creating all spliting plane objects


            if (SplitElement.splitingPlanes.Count > 0)
            {
                foreach(var plane in SplitElement.splitingPlanes)
                {
                    var ob = GenerateSplitingPlaneObject(plane);

                    SplitingPlanes.Add(ob); 
                }
            }

            OnResetPlaneInformationPanel();
            // showing UI of spliting plane
            OnRenderSplitingPlanePanel();


            // display the sub element
            if (SplitElement.IsSplited && SplitElement.splitingPlanes.Count > 0)
            {

                if (TargetElement.SplitedObjects.Count > 0)
                {
                    OnDisplaySplitedElement();
                }
                else
                {
                    // resplit
                    OnSplitElementConfirm(true, "true");
                }
            }
   
        }

        public bool isUpdating = false;

        public void ClosePanel()
        {
            Panel.OnPanelClose();
            EditingBound.isDrawing = false;
            IsEditingEnabled = false;

            if (IsEditingPlane)
            {
                OnClick_ClosePlaneEditor();
            }

            // SAVE AND UPDATE THE SPLIT ELEMENT
            // TODO


            //disable all element spliting plane
            foreach (var plane in SplitingPlanes)
            {
                Destroy(plane);
            }

            SplitingPlanes.Clear();

            // clear the sub element tree view list
            SubelmentItems.Clear();
            SubelmentItemAdapter.SetItems(SubelmentItems);

            // also exit editing mode
            Page_BIMViewer.Instance.OnClick_OnObjectSplitingComplete(SplitElement);


            //update element

            LoadingHandler.Instance.OnFullPageLoadingStart("Saving Element...");
            CurrentModelVersion.OnRequestUpdate_ElementSplit(SplitElement);


        }


        private List<SplitPlane> RenderSplitinplanes;

        public void OnRenderSplitingPlanePanel()
        {
            RenderSplitinplanes = new List<SplitPlane>();

            foreach(var plane in SplitingPlanes)
            {
                RenderSplitinplanes.Add(plane.GetComponent<SplitingPlane>().Item);
            }


            SplitingPlaneItemAdapter.SetItems(RenderSplitinplanes);



            SelectedSplitPlaneUIItem = null;
            SelectedSplitingPlane = null;
            SelectedSplitPlaneItem = null;
            SelectedSplitingSubelement = null;


        }


        public void OnToggle_SetPlaneDrawable(GameObject gameObject)
        {
            var uiItem = gameObject.GetComponent<UIBlock_BimViewer_ObjectSplit_PlaneItem>();

            uiItem.LinkedObject.isDrawing = !uiItem.LinkedObject.isDrawing;

            uiItem.Item.isApplied = uiItem.LinkedObject.isDrawing;

            Debug.Log("OnToggle_SetPlaneDrawable: " + uiItem.LinkedObject.name);
            uiItem.OnSetPlaneDrawable();

        }


        public void LoadAndSetSplitingPlanes()
        {
            // load plane item from workspace

            // check if plane is default plane (from boundary)

            // 
            
            // 
        }



        public void OnClick_SelectSplitingPlane(GameObject _plane)
        {
            // set ui element
            // enable plane handler 3d Object
            // set plane height (get fro projectmodelhandler (todo))
            //

            var sp = _plane.GetComponent<UIBlock_BimViewer_ObjectSplit_PlaneItem>();

            if (SelectedSplitPlaneUIItem != null) {
                SelectedSplitPlaneUIItem.OnDeselect();
            }
            sp.OnSelect();

            SelectedSplitPlaneUIItem = sp;

            SelectedSplitingPlane = SelectedSplitPlaneUIItem.LinkedObject.gameObject;
            SelectedSplitPlaneItem = SelectedSplitPlaneUIItem.LinkedObject;

       
        }

 

        public void OnClick_ClearSplitedElements()
        {
            if (SubelmentItems.Count < 1)
            {
                MCPopup.Instance.SetInformation("No sub element to remove.");
            }


            // remove rendered data 

            SplitingSubelementPool.Clear();
            TargetElement.LinkedNodeItem.childrenNodes.Clear();


            // clear sub element nodes
            foreach (var item in TargetElement.SplitedObjects) 
            {
                CurrentModelVersion.bimModel.Structures.Remove(item.GetComponent<BIMElement>().LinkedNodeItem);
                Destroy(item);
            }

            // clear item status
            SplitElement.IsSplited = false;
            TargetElement.SplitedObjects.Clear();
            TargetElement.RestoreObject();

            // clear tree view items
            SubelmentItems.Clear();
            SubelmentItemAdapter.SetItems(SubelmentItems);
            SubelmentItemAdapter.ForceRebuildLayoutNow();

            // update the split plane item


            // set tree view
            Page_BIMViewer.Instance.OnSelectMeshObjectReflectToTree(TargetElement);
        }



        public void OnClick_SaveSplitingPlane()
        {

        }


        public GameObject GenerateSplitingPlaneObject(SplitPlane _panel)
        {
            var SelectedSplitPlaneObject = Instantiate(ResourceHolder.Instance.GetPrefabItem("Tool_SplitingPlane"));
            SelectedSplitPlaneObject.name = SelectedSplitPlaneObject.name + "_" + DateTime.Now.ToString("HHmmsss");
            var item = SelectedSplitPlaneObject.GetComponent<SplitingPlane>();
            item.Item = _panel;

            // set the plane position
            item.OnPlaneInitByObject(item.Item);
            item.DeactiveDrag();
 
            return SelectedSplitPlaneObject;

        }


   
        public bool isAddnew = false;

        public void OnClick_AddNewPlane()
        {
            if (IsEditingPlane)
            {
                MCPopup.Instance.SetInformation("Can not add new plane when modifing plane information!");
                return;
            }

            var SelectedSplitPlaneObject = Instantiate(ResourceHolder.Instance.GetPrefabItem("Tool_SplitingPlane"));
            SelectedSplitPlaneObject.name = SelectedSplitPlaneObject.name + "_" + DateTime.Now.ToString("HHmmsss");
            SelectedSplitPlaneItem = SelectedSplitPlaneObject.GetComponent<SplitingPlane>();
            SelectedSplitPlaneItem.Item = new SplitPlane();
            SelectedSplitPlaneItem.isDrawing = false;

            isAddnew = true;
            IsEditingPlane = true;

            Panel_PlaneEditor.OnPanelOpen();

            OnSetPlaneInformationPanel(SelectedSplitPlaneItem);

            InputField_TargetElement.text = TargetElement.name;
            InputField_PlaneName.text = "NewPlane";

            Rect_FloatPlaneInfor.gameObject.SetActive(true);


        }


        public void OnClick_EditNewPlane()
        {
            if (IsEditingPlane)
            {
                MCPopup.Instance.SetInformation("A plane editing is on going!");
                return;
            }

            // have to select a plane
            
            if (SelectedSplitingPlane == null)
            {
                MCPopup.Instance.SetInformation("Please select one spliting plane");
                return;
            }


            IsEditingExitingPlaneInited = false;
            IsEditingPlane = true;
            isAddnew = false;

            Panel_PlaneEditor.OnPanelOpen();
            SelectedSplitPlaneItem.isDrawing = true;
            OnSetPlaneInformationPanel(SelectedSplitPlaneItem);
            SelectedSplitPlaneItem.ActiveDrag(SelectedSplitPlaneItem.Item.planeType == SplitPlane.PlaneType.vertical?true:false);
            IsEditingExitingPlaneInited = true;


            Rect_FloatPlaneInfor.gameObject.SetActive(true);
        }

        public void OnClick_RemoveSplitingPlane()
        {
            if (IsEditingPlane)
            {
                MCPopup.Instance.SetInformation("Can not remove a plane when modifing plane information!");
                return;
            }

            // default planes (from boundary can not be deleted)
            if (SelectedSplitingPlane == null)
            {
                MCPopup.Instance.SetInformation("Please select a plane to remove");
                return;
            }

            SplitElement.splitingPlanes.Remove(SelectedSplitingPlane.GetComponent<SplitingPlane>().Item);


            SplitingPlanes.Remove(SelectedSplitingPlane);
            Destroy(SelectedSplitingPlane);

            OnRenderSplitingPlanePanel();

            

        }

        public GameObject GetSplitingPlaneObjectByGuid(string _guid)
        {
            foreach(var plane in SplitingPlanes)
            {
                if(plane.GetComponent<SplitingPlane>().Item.guid == _guid)
                {
                    return plane;
                }
            }

            return null;
        }


        public void OnClick_ClosePlaneEditor()
        {
            if (isAddnew)
            {
                isAddnew = false;
                Destroy(SelectedSplitPlaneItem.gameObject);
                SelectedSplitPlaneItem = null;
                SelectedSplitingPlane = null;
            }
            else
            {
                SelectedSplitPlaneItem.DeactiveDrag();
            }

            // this may be a bit redundant

            Rect_FloatPlaneInfor.gameObject.SetActive(false);
            //Debug.Log("OnClick_ClosePlaneEditor: " + Rect_FloatPlaneInfor.gameObject.activeSelf);

            Panel_PlaneEditor.OnPanelClose();
        }


        public void OnClick_ResetPlane()
        {
            // have to select a plane
            if (IsEditingPlane)
            {
                MCPopup.Instance.SetInformation("Can not remove a plane when modifing plane information!");
                return;
            }

        }


        public void OnClick_SplitElement()
        {
            if (IsEditingPlane)
            {
                MCPopup.Instance.SetInformation("Can not split a plane when modifing plane information!");
                return;
            }

            if(SplitingPlanes.Count < 1)
            {
                MCPopup.Instance.SetInformation("This element has no spliting plane!");
                return;
            }

            int count = 0;
            foreach(var p in SplitingPlanes)
            {
                if (p.GetComponent<SplitingPlane>().isDrawing)
                {
                    count++;
                }
            }

            if (count < 1)
            {
                MCPopup.Instance.SetInformation("You will need at least one active plane to split the element");
                return;
            }


            MCPopup.Instance.SetConfirm(OnSplitElementConfirm, "", "Spliting element with active plane?");
        }


        public void OnSplitElementConfirm(bool _result, string _message)
        {
            CutCount = 0;

            if (_result)
            {
                Debug.Log("Spliting Element: " + TargetElement.name);
                TargetElement.RestoreObject();
                
                foreach (var element in TargetElement.SplitedObjects)
                {
                    //Debug.Log("Destroy: " + element.name);
                    Destroy(element);
                }

                SplitingSubelementPool = new List<GameObject>();
                SplitingSubelementPool_Buffer = new List<GameObject>();
                TargetElement.SplitedObjects = new List<GameObject>();
                SplitingSubelementPool_ObjectToDestory = new List<GameObject>();

                SplitingSubelementPool.Add(TargetElement.gameObject);

                foreach (var plane in SplitingPlanes)
                {
                    //Debug.Log("Split Object with: " + plane.name);
                    var item = plane.GetComponent<SplitingPlane>();

                    if (!item.Item.isApplied)
                    {
                        continue;
                    }

                    Vector3 _position = item.CenterGrab.position;
                    Vector3 _direction = item.PlaneDirection;

                    List<GameObject> objects = ElementSplitingProcess(_position, _direction, plane.name);

                    SplitingSubelementPool.Clear();

                    foreach (var ob in objects)
                    {
                        SplitingSubelementPool.Add(ob);
                    }

  
                    CutCount++;
                }

                OnSplitElementComplete();
            }
        }



        public List<GameObject> ElementSplitingProcess(Vector3 _position, Vector3 _direction, string _planeName)
        {
            List<GameObject> objects = new List<GameObject>();

            foreach(GameObject ob in SplitingSubelementPool)
            {
                objects.Add(ob);
            }


            foreach (GameObject ob in SplitingSubelementPool)
            {
                GameObject[] slicedResult = ob.SliceInstantiate(_position, _direction);

                // Object Split error
                if (slicedResult == null)
                {
                    Debug.Log("Split Object: " + ob.name+ ", is cursing error with plane: " + _planeName);
                    //objects.Add(ob);
                }
                else
                {
                    //Debug.Log("Split Object: " + ob.name + ", Result = " + slicedResult.Length);
                    if (slicedResult.Length == 1)
                    {
                        // keep the orginal object
                        //objects.Add(slicedResult[0]);
                    }
                    else
                    {
        

                        if (CutCount != 0)
                        {
                            SplitingSubelementPool_ObjectToDestory.Add(ob);
                        }

                        objects.Remove(ob);

                        foreach (var sob in slicedResult)
                        {
                            sob.transform.parent = TargetElement.transform;
                            sob.transform.localPosition = Vector3.zero;
                            sob.transform.localRotation = Quaternion.identity;

                            FixSplitedMaterial(sob);

                            objects.Add(sob);
                        }
                    }
                }
            }




            return objects;
        }

        public void OnSplitElementComplete()
        {
            MCPopup.Instance.SetComplete("Element splited!");


            foreach(var ob in SplitingSubelementPool_ObjectToDestory)
            {
                Destroy(ob);
            }

            SplitingSubelementPool_ObjectToDestory.Clear();

            // transfer the sub element to the target element

            // render new element
            SubelmentItems = new List<StructureNode>();
            int index = 1;

            // remove all 

            foreach(var node in TargetElement.LinkedNodeItem.childrenNodes)
            {
                CurrentModelVersion.bimModel.Structures.Remove(node);
            }


            TargetElement.LinkedNodeItem.childrenNodes.Clear();



            foreach (var ob in SplitingSubelementPool)
            {
                TargetElement.SplitedObjects.Add(ob);
                ob.name = TargetElement.name + "_Sub_" + index;

                DuplicateBimElementOBject(ob, TargetElement.gameObject, index);


                ob.SetActive(true);
                //elements.Add(ob.GetComponent<BIMElement>());
                BIMElement element = ob.GetComponent<BIMElement>();
                element.RestoreObject();

                StructureNode node = new StructureNode();
                node.linkedObject = ob;
                node.element = element;
                node.itemName = element.name;
                node.Content = "Sub Element " + index;
                node.itemID = element.BimObject.elementID;
                node.nodeDepth = TargetElement.LinkedNodeItem.nodeDepth + 1;
                node.parentNode = TargetElement.LinkedNodeItem;
                element.IsSubElement = true;
                element.LinkedNodeItem = node;
                element.BimObject.elementID = element.BimObject.elementID + "_" + index;

                SubelmentItems.Add(node);
                TargetElement.LinkedNodeItem.childrenNodes.Add(node);

                CurrentModelVersion.bimModel.Structures.Add(node);

                index++;

            }

            // hide orginal element
            SubelmentItemAdapter.SetItems(SubelmentItems);

            // render the list
            // call viewer to update the element tree
            // Page_BIMViewer.Instance.OnClick_SelectTreeItem(TargetElementNode);

            if (TargetElement.SplitedObjects.Count > 0)
            {
                TargetElement.SetToHideMode();
                TargetElement.LinkedNodeItem.IsCollapsed = false;
                // update the tree view


                SplitElement.IsSplited = true;
            }
            else
            {
                TargetElement.LinkedNodeItem.IsCollapsed = true;

                TargetElement.RestoreObject();

                SplitElement.IsSplited = false;
            }

            Page_BIMViewer.Instance.OnSelectMeshObjectReflectToTree(TargetElement);

        }


        public void OnDisplaySplitedElement()
        {

            SubelmentItems = new List<StructureNode>();

;           foreach (var ob in TargetElement.SplitedObjects)
            {
                ob.SetActive(true);
                BIMElement element = ob.GetComponent<BIMElement>();
                element.RestoreObject();

                StructureNode node = element.LinkedNodeItem;
                SubelmentItems.Add(node);
            }

            TargetElement.SetSplitedObjectsInSplitingMode();


            // hide orginal element
            SubelmentItemAdapter.SetItems(SubelmentItems);

            Page_BIMViewer.Instance.OnSelectMeshObjectReflectToTree(TargetElement);

        }

        #region Plane Editor 


        public bool IsEditingPlane;

        public void OnOpenAction_PlaneEditor() 
        {
            Dropdown_PlaneType.options.Clear();
            Dropdown_PlaneGroup.options.Clear();

            IsEditingPlane = true;
            
            // this is a bad way for localization
            if (ProjectConfiguration.Instance.DefaultLanguage == ProjectConfiguration.LocationType.EN)
            {
                Dropdown_PlaneType.AddOptions(Dropdown_PlaneType_Items);
                Dropdown_PlaneGroup.AddOptions(Dropdown_PlaneGroup_Items);
            }
            else
            {
                Dropdown_PlaneType.AddOptions(Dropdown_PlaneType_Items_CH);
                Dropdown_PlaneGroup.AddOptions(Dropdown_PlaneGroup_Items_CH);
            }


            //trigger the first level of plane type
            if (isAddnew)
            {
                OnValueChange_PlaneType(0);
                OnValueChange_PlaneAlignment(0);
            }
        }
        

        public void OnCloseAction_PlaneEditor()
        {
            // also triggered by submit
            IsEditingPlane = false;

      
        }


        public void OnValueChange_PlaneType(int _value)
        {
            //Debug.Log("OnValueChange_PlaneType: " + _value);
            if (!IsEditingExitingPlaneInited)
            {
                return;
            }

            if (_value == 0)   // horizontal
            {

               Dropdown_PlaneAlignment.ClearOptions();
                if (ProjectConfiguration.Instance.DefaultLanguage == ProjectConfiguration.LocationType.ZH)
                {
                    Dropdown_PlaneAlignment.AddOptions(Dropdown_PlaneAlignment_Items_CH);
                }

                else
                {
                    Dropdown_PlaneAlignment.AddOptions(Dropdown_PlaneAlignment_Items);
                }

                SelectedSplitPlaneItem.Item.planeType = SplitPlane.PlaneType.horizontal;
            }
            else 
            if (_value  == 1) // Vertical
            {              // set content of plane alignment to level
                List<string> levelstrings = new List<string>();
                var levels = ProjectModelHandler.Instance.GetModelLevel();

                if (levels.Count > 0)
                {
                    foreach (var level in levels)
                    {
                        levelstrings.Add(level.LevelName);
                    }
                }
                else
                {
                    if (ProjectConfiguration.Instance.DefaultLanguage == ProjectConfiguration.LocationType.ZH)
                    {
                        levelstrings.Add("高程中点");
                    }
                    else
                    {
                        levelstrings.Add("Elevation Center");
                    }
                }

               

                Dropdown_PlaneAlignment.ClearOptions();
                Dropdown_PlaneAlignment.AddOptions(levelstrings);

                SelectedSplitPlaneItem.Item.planeType = SplitPlane.PlaneType.vertical;

            }


            OnValueChange_PlaneAlignment(Dropdown_PlaneAlignment.value);
        }


        public void OnValueChange_PlaneAlignment(int _value)
        {
            //Debug.Log("OnValueChange_PlaneAlignment: " + _value);

            if (!IsEditingExitingPlaneInited)
            {
                return;
            }

            // create a new plane
            if (SelectedSplitPlaneItem.Item.planeType == SplitPlane.PlaneType.horizontal)
            {
                if (_value == 0)  // Center X
                {
                    SelectedSplitPlaneItem.OnPlaneInitByHandlerHorizental(TargetElementBound, true);
                }
                else
                if (_value == 1) // Center Z
                {
                    SelectedSplitPlaneItem.OnPlaneInitByHandlerHorizental(TargetElementBound, false);
                }
          
            }
            else
            if (SelectedSplitPlaneItem.Item.planeType == SplitPlane.PlaneType.vertical)
            {
                var levels = ProjectModelHandler.Instance.GetModelLevel();

                float height = levels[_value].LevelHeightMin / 1000f;

                SelectedSplitPlaneItem.OnPlaneInitByHandlerVertical(TargetElementBound, height);

            }

        }

        
        public void OnClick_PlaneEditorSubmit()
        {
            // Does this action require conform? 
            // no in this case


            // validate the data reading from UI elments of editing of plane
            // if not valid, show error message

            string planeName = InputField_PlaneName.text;
            string targetElement = InputField_TargetElement.text;
            int planeType = Dropdown_PlaneType.value;
            int planeGroup = Dropdown_PlaneGroup.value;
            int planeAlignment = Dropdown_PlaneAlignment.value;

            if (planeName == "")
            {
                MCPopup.Instance.SetInformation("Please input plane name");
                return;
            }

            
            if (targetElement == "")
            {
                targetElement = TargetElement.name;
            }

            // get the element (TargetElement) guid from the name

            SplitPlane plane = SelectedSplitPlaneItem.Item;
            plane.planeName = planeName;
            plane.planeDescription = targetElement;
            plane.planeID = TargetElement.BimObject.elementID;

            plane.planeType = planeType == 0 ? SplitPlane.PlaneType.horizontal : SplitPlane.PlaneType.vertical;
            plane.planeGroup = planeGroup == 0 ? SplitPlane.PlaneGroup.element : SplitPlane.PlaneGroup.model;
            plane.planeCorners = new List<Vector3D>();


            foreach (var corner in SelectedSplitPlaneItem.CornerGrabs)
            {
                plane.planeCorners.Add(Vector3D.FromVecter3(corner.position));
            }

            SelectedSplitPlaneItem.DeactiveDrag();

            if (isAddnew)
            {
                SplitingPlanes.Add(SelectedSplitPlaneItem.gameObject);
                SplitElement.splitingPlanes.Add(plane);
            }

            // Is this needed?
            //  metaBIM_Plane.planeID = System.Guid.NewGuid().ToString();


            // exit plane
            Panel_PlaneEditor.OnPanelClose();

            OnRenderSplitingPlanePanel();
            


        }


        public void OnClick_SelectSubElement(GameObject _object)
        {
            var item  = _object.GetComponent<UIBlock_BimViewer_ObjectSplit_SubElementItem>();

            var element = item.Item.element;

            if (SelectedSplitingSubelement != null)
            {
                SelectedSplitingSubelement.GetComponent<UIBlock_BimViewer_ObjectSplit_SubElementItem>().OnDeselected();
            }
            item.OnSelected();

            SelectedSplitingSubelement = _object;

        }


        public void OnToggle_ShowSubElement(GameObject _object)
        {
            var item = _object.GetComponent<UIBlock_BimViewer_ObjectSplit_SubElementItem>();


            if (item.Item.element.IsElementHide)
            {
                item.Item.element.RestoreObject();
            }
            else
            {
                item.Item.element.SetToHideMode();
            }

            item.OnSetShow();

        }


        public void OnResetPlaneInformationPanel()
        {
            InputField_PlaneName.text = "";
            InputField_TargetElement.text = "";
            Dropdown_PlaneType.value = 0;
            Dropdown_PlaneGroup.value = 0;
            Dropdown_PlaneAlignment.value = 0;

            Dropdown_PlaneType.ClearOptions();
            Dropdown_PlaneGroup.ClearOptions();
            Dropdown_PlaneAlignment.ClearOptions();


            if (ProjectConfiguration.Instance.DefaultLanguage == ProjectConfiguration.LocationType.ZH)
            {
                Dropdown_PlaneType.AddOptions(Dropdown_PlaneType_Items_CH);
                Dropdown_PlaneGroup.AddOptions(Dropdown_PlaneGroup_Items_CH);
                Dropdown_PlaneAlignment.AddOptions(Dropdown_PlaneAlignment_Items_CH);
            }

            else
            {
                Dropdown_PlaneType.AddOptions(Dropdown_PlaneType_Items);
                Dropdown_PlaneGroup.AddOptions(Dropdown_PlaneGroup_Items);
                Dropdown_PlaneAlignment.AddOptions(Dropdown_PlaneAlignment_Items);
            }
        }


        public void OnSetPlaneInformationPanel(SplitingPlane _panel)
        {
            
            InputField_PlaneName.text = _panel.Item.planeName;
            InputField_TargetElement.text = _panel.Item.planeDescription;



            Dropdown_PlaneType.ClearOptions();
            Dropdown_PlaneGroup.ClearOptions();
            Dropdown_PlaneAlignment.ClearOptions();


            if (ProjectConfiguration.Instance.DefaultLanguage == ProjectConfiguration.LocationType.ZH)
            {
                Dropdown_PlaneType.AddOptions(Dropdown_PlaneType_Items_CH);
                Dropdown_PlaneGroup.AddOptions(Dropdown_PlaneGroup_Items_CH);
                Dropdown_PlaneAlignment.AddOptions(Dropdown_PlaneAlignment_Items_CH);
            }

            else
            {
                Dropdown_PlaneType.AddOptions(Dropdown_PlaneType_Items);
                Dropdown_PlaneGroup.AddOptions(Dropdown_PlaneGroup_Items);
                Dropdown_PlaneAlignment.AddOptions(Dropdown_PlaneAlignment_Items);
            }

            Dropdown_PlaneType.value = (int)_panel.Item.planeType;
            Dropdown_PlaneGroup.value = (int)_panel.Item.planeGroup;
            //Dropdown_PlaneAlignment.value = (int)_panel.Item.planealignment;

        }


        public void OnRequest_UpdateWorkspace()
        {
            
        }


        public void OnRequestUpdateWorkspace_Callback(bool _result, string _message)
        {
            if (_result)
            {

            }
            else
            {
                
            }
        }

        #endregion


        public void CreatePlaneByLevel(List<BimLevel> _levels, GameObject _planePrefabs, Transform _parent)
        {
            Debug.Log("SetLevels");

      
        }


        public void CreatePlaneByBoundCenter(bool _direction)
        {

        }


        public void FixSplitedMaterial(GameObject _item)
        {
            Material[] mats = _item.GetComponent<Renderer>().materials;

            if (mats.Length > 1)
            {
                List<Material> newMaterial = mats.ToList();
                newMaterial.RemoveAt(mats.Length - 1);
                _item.GetComponent<Renderer>().materials = newMaterial.ToArray();
            }


            //_item.GetComponent<>
        }


        public void ExtractGeometry(BIMElement _element)
        {
            _element.vertices = _element.Filter.sharedMesh.vertices;
            _element.triangles = _element.Filter.sharedMesh.triangles;


            if (_element.BimObject.ifcGeometryType == BIM_GEOMETRY_TYPE.Geometry)
            {
                // Iterate over each triangle in the mesh, this process is taking so many time
                for (int i = 0; i < _element.triangles.Length - 2; i += 3)
                {
                    // Get the three vertices that make up the triangle
                    Vector3 vertex1 = _element.transform.TransformPoint(_element.vertices[_element.triangles[i]]);
                    Vector3 vertex2 = _element.transform.TransformPoint(_element.vertices[_element.triangles[i + 1]]);
                    Vector3 vertex3 = _element.transform.TransformPoint(_element.vertices[_element.triangles[i + 2]]);

                    _element.AddToFace(vertex1, vertex2, vertex3);
                }


                foreach (var face in _element.Faces)
                {
                    face.RemoveSharedEdge();
                    _element.BorderEdgeCount += face.BorderEdges.Count;
                }


                // CreateBorderLineBatch
                _element.BorderLines = new Lines(_element.BorderEdgeCount);
                int EdgeCount = 0;

                foreach (var face in _element.Faces)
                {
                    for (int i = 0; i < face.BorderEdges.Count; i++)
                    {
                        _element.Lines.Add(new Linefy.Line(
                            face.BorderEdges[i].p1,
                            face.BorderEdges[i].p2,
                            _element.LineColor,
                            _element.LineColor,
                            _element.LineWidth,
                            _element.LineWidth)
                            );
                        EdgeCount++;
                    }
                }


                _element.isDrawBorder = false;

                _element.vertices = new Vector3[0];
                _element.triangles = new int[0];
                _element.Faces.Clear();

                _element.SetBorderLine();
            }

        }


        public void UpdateIfcId(BimObject _bimObject, string _newIfcName)
        {
            _bimObject.elementID = _newIfcName;
            _bimObject.records[_bimObject.versionID].ifcAttribute.SetValue("id", _newIfcName);
            _bimObject.records[_bimObject.versionID].ifcParameter.SetValue("IFC Guid", _newIfcName);
            // _bimObject.records[_bimObject.versionID].ifcProperties.SetValue("id", _newIfcName);
            // Any more ?

        }

        public void DuplicateBimElementOBject(GameObject _target, GameObject _source, int _index)
        {
            _target.layer = _source.layer;

            if (_target.GetComponent<BIMElement>() == null)
            {
                _target.AddComponent<BIMElement>();
            }

            if (_target.GetComponent<MeshCollider>() == null)
            {
                _target.AddComponent<MeshCollider>();
            }


            BIMElement element_target = _target.GetComponent<BIMElement>();
            BIMElement element_source = _source.GetComponent<BIMElement>();  // give the reference

            // Copy Data over?, need do a deep copy
            BimObject newBimObject = BimObject.FromJson(BimObject.ToJson(element_source.BimObject));
            newBimObject.guid = Guid.NewGuid().ToString();

            // new ifc id?
            string newifcId = newBimObject.records[newBimObject.versionID].ifcAttribute.Find("id") + "_" + _index;


            UpdateIfcId(newBimObject, newifcId);

            element_target.BimObject = newBimObject;
            element_target.SelectedVersion = element_source.SelectedVersion;


            element_target.Filter = _target.GetComponent<MeshFilter>();
            element_target.Renderer = _target.GetComponent<MeshRenderer>();
            element_target.Collider = _target.GetComponent<MeshCollider>();

            element_target.Collider.sharedMesh = element_target.Filter.sharedMesh;
            element_target.OriginalMaterials = element_target.Renderer.materials;

            element_target.LineColor = element_source.LineColor;
            element_target.LineWidth = element_source.LineWidth;

            // get edge
            ExtractGeometry(element_target);



        }


    }
}
