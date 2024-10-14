using EzySlice;
using IfcToolkit;
using Linefy;
using MetaBIM.CodeChecking;
using MongoDB.Bson;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;


namespace MetaBIM
{
    public class Page_BIMViewer : MonoBehaviour
    {
        public static Page_BIMViewer Instance;

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


        [Header("Camera")]
        [Header("=======================================")]

        public Transform CenterObject;    // this is where to put the mesh in 
        public FreeCameraNav NavCam;

        [Header("Right Click Menu")]
        [Header("=======================================")]
        public UIController_RightClickMenu RightClickMenu;

        public bool IsMenuOpend;
        public LayerMask ObjectSelectionLayer;
        public CanvasGroup ActionInfoPanel;
        public TextMeshProUGUI Text_ActionInfo;

        public GameObject RightClickSelectedObject;
        public StructureNode RightClickSelectedItem;
        public string CopiedContent;


        public Vector3 LastRightClick;
        public Vector3 LastLeftClick;

        [Header("Viewers")]
        [Header("=======================================")]
        public PanelChange Panel_Detail_IFC;
        public PanelChange Panel_Attributes;
        public PanelChange Panel_List_BCF;
        public PanelChange Panel_Detail_BCF;
        public PanelChange Panel_Detail_Transform;
        public PanelChange Panel_SearchViewer;

        public PanelChange Panel_ClassificationSelector;
        public PanelChange Panel_ZoneWidget;
        public PanelChange Panel_CodeCheckingWidget;

        [Header("Toggle Group")]
        [Header("=======================================")]
        public UIController_Toggle Toggle_DrawEdge;
        public UIController_Toggle Toggle_SectionBox;

        //public UIController_Toggle Toggle_DrawReferenceLineOrPlane;
        public UIController_Toggle Toggle_Measurement;

        // Zone
        public UIController_Toggle Toggle_ZoneBoxShowSection;


        // setting
        public UIController_Toggle Toggle_ShowSplitedElement;


        // Tree element viewer
        public UIController_Toggle Toggle_SelectedOnly;

        [Header("Tab Select")]
        [Header("=======================================")]
        public UIController_Tab Tab_TreeView;


        [Header("Model Data")]
        [Header("=======================================")]

        [Header("Scroll View Adapter")]
        [Header("=======================================")]
        public IfcStructureItemsAdapter IfcStructureItemsAdapter;
        public IfcAttributesAdapter IfcAttributesAdapter;

        public MetaBIM.Version SelectedVersion;

        public TMP_InputField InputField_ID;

        [Header("BIM Attributes")]
        [Header("=======================================")]
        public BimObjectRecord AttributesRecord;
        public TextMeshProUGUI Text_AttributePanel_Header;

        [Header("Tree Node")]
        [Header("=======================================")]
        public List<GameObject> AttributeItems;

        // Tree rendering Data


        public List<StructureNode> RootNodeList = new List<StructureNode>();
        public List<StructureNode> IfcStructureItems = new List<StructureNode>();
        public List<StructureNode> IfcStructureItems_SearchResult = new List<StructureNode>();

        //
        public List<StructureNode> SelectedIfcStructureItems;

        public int NodeProcessIndex = 0;
        public int ProcessedIndex;

        [Header("Selection")]
        [Header("=======================================")]
        public GameObject SelectedStructureTreeItem;
        public GameObject SelectedBCF;
        public GameObject SelectedBIMElementModel;


        [Header("BIM BUFFER")]
        [Header("=======================================")]
        //public IfcRootLists IfcRootLists;

        public List<Profile> CollaborationUsers;
        public Project CurrentProject;
        public BCF CurrentBCF;

        public StructureNode SearchedIfcStructureItem;

        public List<StructureNode> HidedObjects;

        public List<GameObject> SiteSections;

        [Header("Status")]
        [Header("=======================================")]
        public int ProcessCounter = 0;
        public float MeshScale = 1f;

        public GameObject ClickSelectedObject;

        public int LevelLimit = 3;

        /* New BIM Process */
        // display the list in inspector is causing lag
        //public List<BimObject> ModelBimObjects = new List<BimObject>();

        public GameObject ModelRootGameObject;
        public Vector3 ModelOffset;


        [Header("BIM Objects")]
        [Header("=======================================")]
        public string LoadedVersionGuid;
        public int LoadedVersion;
        public bool IsReloadModelRequired;   // general flag

        public List<string> MaterialNames;
        public List<string> ObjectNameList;


        [Header("Viewer Tools")]
        [Header("=======================================")]
        public GameObject SectionBoxObject;
        public List<GameObject> LevelObjects;
        public SectionPlaneHandler SectionPlaneHandler;
        public SelectionBox SelectionBox;
        public SectionBoxController SectionBoxController;


        [Header("Shared Viewer Object")]
        [Header("=======================================")]
        public List<GameObject> SharedObjectViewer;

        // =================================================================================

        public void OnOpenAction()
        {
            GUIUtility.systemCopyBuffer = "";
            ProjectConfiguration.Instance.IsDisplaySearchResult = false;
            IsPageOpend = true;

            Panel_Attributes.OnPanelClose();

        }


        public void OnSetToSharedViewerMode()
        {
            if (AppController.Instance.CurrentShareLinkPackage.shared)
            {
                foreach (var item in SharedObjectViewer)
                {
                    if (item != null)
                    {
                        item.SetActive(false);
                    }
                }

            }
        }


        public void OnCloseAction()
        {
            CurrentProject = null;

            ProjectModelHandler.Instance.DisableAllModels();
            // clear tree 
            IfcStructureItems.Clear();
            IfcAttributeItems.Clear();
            Panel_Attributes.OnPanelClose();

            if (IfcAttributesAdapter != null)
            {
                IfcAttributesAdapter.SetItems(IfcAttributeItems);
            }

            if (IfcStructureItemsAdapter != null)
            {
                IfcStructureItemsAdapter.SetItems(IfcStructureItems);

            }


            if (ModelRootGameObject != null)
            {
                ModelRootGameObject.SetActive(false);
            }


            // init all widget
            SelectionBox.OnDeselect();
            SectionBoxController.OnSectionBoxDisable();

            IsPageOpend = false;



            // ClearBuffer
            // Reset toggles on UI
            Toggle_DrawEdge.OnReset();
            Toggle_SectionBox.OnReset();
            //Toggle_DrawReferenceLineOrPlane.OnReset();
            Toggle_Measurement.OnReset();

            //Toggle_ZoneBoxShowOriginalModel.OnReset(true);  // this will display 
            Toggle_ZoneBoxShowSection.OnReset();

            // seting 

            Toggle_ShowSplitedElement.OnReset(ProjectConfiguration.Instance.IsShowSplitedObjects);
            Toggle_SelectedOnly.OnReset(ProjectConfiguration.Instance.IsDisplaySelectedElement);


            RightClickSelectedObject = null;
            ClickSelectedObject = null;
            SelectedStructureTreeItem = null;
            ProjectModelHandler.Instance.OnReset();


            // close windows
            Panel_CodeCheckingWidget.OnPanelClose();
            Panel_ZoneWidget.OnPanelClose();
            Panel_ClassificationSelector.OnPanelClose();
            Panel_Attributes.OnPanelOpen();


            PageStatus.OnReset();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        Ray ray;
        RaycastHit hit;
        // Update is called once per frame
        void Update()
        {

            if (!IsPageOpend)
            {
                return;
            }


            // Menu Action
            RightClickMenuAction_3DView();
            // View Operation


            if (Input.GetMouseButtonDown(0))
            {
                if (!UIController_RightClickMenuUIBlock.Instance.IsOnUI)
                {
                    CloseRightClickMenu();
                }

                LastLeftClick = Input.mousePosition;
            }


            if (MouseInputUIBlocker.BlockedByUI)
            {
                // disable hover effect if any
                HoverOnMeshObject(null);   // Need to investiage for performance issue
                return;
            }

            // NavCma may be disabled by other control model
            if (!NavCam.enabled)
            {
                return;
            }

            // selection is not working in editing
            if (ObjectSplitHandler.Instance.IsEditingEnabled)
            {
                return;
            }

            if (ZoneManagement.Instance.IsEditing)
            {
                return;
            }



            // on mouse hovering detection

            if (!NavCam.isOnPenning && !NavCam.isOnRotating)
            {


                // dont trigger hover on drag selecting
                if (!ObjectSelectionHandler.Instance.isDragSelecting)
                {
                    // hover disable in isolation mode, if only one object is isolated
                    if (ProjectModelHandler.Instance.IsolatedElements.Count == 1)
                    {
                        //return;
                    }

                    ray = NavCam.MainCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        // BIM objects
                        if (hit.collider.gameObject.GetComponent<BIMElement>() != null)
                        {

                            HoverOnMeshObject(hit.collider.gameObject);
                        }
                        else
                        {
                            // hit wrong thing
                            HoverOnMeshObject(null);
                        }
                    }
                    else
                    {
                        // nothing hit
                        HoverOnMeshObject(null);
                    }
                }


                //ZoneBoxController.IsInteractable = false;

                // need to be within the same condition for hovering and clicking

                //replace with box selection 
                if (Input.GetMouseButtonUp(0))
                {
                    if (LastLeftClick == Input.mousePosition)
                    {
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ObjectSelectionLayer))
                        {
                            if (hit.collider.gameObject.GetComponent<BIMElement>() != null)
                            {
                                debug("Select: " + hit.collider.gameObject.name);

                                // Multi selection
                                if (Input.GetKey(KeyCode.LeftShift))
                                {
                                    SelectMeshObject(hit.collider.gameObject, true, false);
                                }
                                else
                                {
                                    SelectMeshObject(hit.collider.gameObject, false, false);
                                }

                
                            }
                        }
                        else
                        {
                            // un select object
                            SelectMeshObject(null, false, false);
                        }

                    }

                }
            }
            else
            {

            }
        }



        public void OnBoxSelection(Vector2 _minValue, Vector2 _maxValue)
        {

            // get object list
            List<BimModel> models = new List<BimModel>();
            ProjectModelHandler.Instance.SelectedElements.Clear();

            models = ProjectModelHandler.Instance.GetActiveModels();

            foreach (BimModel model in models)
            {
                foreach (StructureNode node in model.Structures)
                {

                    if (node.element != null)
                    {
                        if (node.element.IsElementHide || node.element.IsIsolated)
                        {
                            continue;
                        }


                        if (node.element.Collider != null)
                        {
                            Vector3 objOnScreent = NavCam.MainCamera.WorldToScreenPoint(node.element.Collider.bounds.center);
                            node.element.InScenePosition = objOnScreent;


                            if (
                                objOnScreent.x > _minValue.x &&
                                objOnScreent.x < _maxValue.x &&
                                objOnScreent.y > _minValue.y &&
                                objOnScreent.y < _maxValue.y)
                            {
                                node.element.SetToSelectedMode();
                                node.element.IsSelected = true;
                                ProjectModelHandler.Instance.SelectedElements.Add(node.element.gameObject);
                                //Debug
                                //ResourceHolder.Instance.DebugReferencePoiint.transform.position = node.element.Collider.bounds.center;
                            }
                            else
                            {
                                node.element.RestoreObject();
                                node.element.IsSelected = false;
                            }
                        }
                    }
                }
            }


        }




        // =================================================================================
        // Mouse Action

        public void HoverOnMeshObject(GameObject _object)
        {
            // remove hover meterial for previous hover if nothing detected
            if (_object == null)
            {
                if (ProjectModelHandler.Instance.CurrentHoveringObject != null)
                {
                    ProjectModelHandler.Instance.CurrentHoveringObject.GetComponent<BIMElement>().RestoreObject();
                    ProjectModelHandler.Instance.CurrentHoveringObject = null;
                }

                return;
            }


            // hovers on same object, no action need
            if (ProjectModelHandler.Instance.CurrentHoveringObject != null)
            {
                if (_object.GetInstanceID() == ProjectModelHandler.Instance.CurrentHoveringObject.GetInstanceID())
                {
                    return;
                }
            }

            // hovered object is in the seletect object, no action
            foreach (var item in ProjectModelHandler.Instance.SelectedElements)
            {
                if (item.GetInstanceID() == _object.GetInstanceID())
                {
                    return;
                }
            }

            // remove hover meterial for previous hover
            if (ProjectModelHandler.Instance.CurrentHoveringObject != null)
            {
                ProjectModelHandler.Instance.CurrentHoveringObject.GetComponent<BIMElement>().RestoreObject();
            }

            ProjectModelHandler.Instance.CurrentHoveringObject = _object;
            ProjectModelHandler.Instance.CurrentHoveringObject.GetComponent<BIMElement>().SetToHoverMode();
        }


        public void SelectMeshObject(GameObject _object, bool _isMultSelect = false, bool _isSelectedOnTreeItem = true)
        {
            // If Muti select, do not clear previous selection
            if (!_isMultSelect)
            {
                foreach (var item in ProjectModelHandler.Instance.SelectedElements)
                {
                    item.GetComponent<BIMElement>().RestoreObject();
                }

                ProjectModelHandler.Instance.SelectedElements.Clear();
            }

            // clear hover object, so when change hover object, no need to change the material
            ProjectModelHandler.Instance.CurrentHoveringObject = null;

            if (_object == null)
            {
                SetAttributesView(null);
                return;
            }

            BIMElement element = _object.GetComponent<BIMElement>();
            //Debug.Log("Selected Element: " + element.name);
            element.SetToSelectedMode();

            // focus to the selected object

            ProjectModelHandler.Instance.SelectedElements.Add(_object);

            // Set Attribute
            SetAttributesView(ProjectModelHandler.Instance.SelectedElements);

            if (!_isSelectedOnTreeItem)
            {
                OnSelectMeshObjectReflectToTree(element);
            }
        }


        public void SelectMeshObjects(List<GameObject> _objects, bool _isMultSelect = false, bool _isSelectedOnTreeItem = true)
        {
            Debug.Log("SelectMeshObjects: " + _objects.Count);  


            if (_objects.Count == 1)
            {
                SelectMeshObject(_objects[0], _isMultSelect, _isSelectedOnTreeItem);
                return;
            }


            if (_objects.Count == 0)
            {
                return;
            }



            // If Muti select, do not clear previous selection
            foreach (var item in ProjectModelHandler.Instance.SelectedElements)
            {
                item.GetComponent<BIMElement>().RestoreObject();
            }

            ProjectModelHandler.Instance.SelectedElements.Clear();
            ProjectModelHandler.Instance.CurrentHoveringObject = null;


            SetAttributesView(ProjectModelHandler.Instance.SelectedElements);


            foreach (var ob in _objects)
            {
                BIMElement ele = ob.GetComponent<BIMElement>();
                if (ele != null)
                {
                    ele.SetToSelectedMode();
                    ProjectModelHandler.Instance.SelectedElements.Add(ob);
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_object"></param>
        public void OnSelectMeshObjectReflectToTree(BIMElement _element)
        {
            // Select tree view item
            SelectStructureItemExpend(_element.LinkedNodeItem);

            // render tree view item
            ProcessTreeView();

            // get item index in tree view



            if (_element.LinkedNodeItem != null)
            {

                int scrollId = _element.LinkedNodeItem.ItemIndex;

                //Debug.Log("Get LinkedNodeItem Index: " + scrollId);

                if (scrollId != -1)
                {
                    IfcStructureItemsAdapter.SmoothScrollTo(scrollId, PageStatus.SCROLL_TO_ITEM_TIME, 0.5f, 0.5f);
                    StartCoroutine(OnSelectTreeItemDelayCall(_element, false, PageStatus.SCROLL_TO_ITEM_TIME));
                }
            }
        }


        public IEnumerator OnSelectTreeItemDelayCall(BIMElement _element, bool _isTree, float _delaySecond)
        {
            yield return new WaitForSeconds(_delaySecond + 0.2f);

            // Maybe the tree viewer is closed
            if (_element.LinkedNodeItem.UILinkObject != null)
            {

                OnSelectTreeItem(_element.LinkedNodeItem.UILinkObject.gameObject, _isTree);
            }

        }


        public void SelectStructureItemExpend(StructureNode _node)
        {
            _node.IsCollapsed = false;


            //Debug.Log("SelectStructureItemExpend: 1" + _node.itemName + ", " + _node.ItemIndex);

            if (_node.parentNode != null)
            {
                SelectStructureItemExpend(_node.parentNode);
            }



        }




        #region Start Loading Model

        // =================================================================================

        public void OnLoadBimObjectsIntoSceneFromhandler(bool isShared = false)
        {


            // set View Focus
            ModelVersion version = ProjectModelHandler.Instance.GetDefaultActiveModel();

            if (version != null)
            {
                OnInitFitView(version.bimModel.CombinedBound);
            }

            // process initial tree view
            // TODO, need to set to open the tree view by default
            // SelectViewItem("View Spatial");
            Tab_TreeView.SetDefault();


            if (isShared)
            {
                OnSetToSharedViewerMode();
            }

        }


        #endregion


        //====================================================================

        #region  Load Node in tree view list

        public void ProcessModelStructureNodes(List<StructureNode> _rootNodeList)
        {
            Debug.Log("ProcessModelStructureNodes: size of " + _rootNodeList.Count);
            RootNodeList = _rootNodeList;
            // apply search here?
            SearchHandler.Instance.OnRequestSearchList(RootNodeList);
            ProcessTreeView();

        }


        public int TreeViewItemScrollIndex = 0;


        // this is the main function to render the tree view
        public void ProcessTreeView()
        {
            IfcStructureItems.Clear();
            TreeViewItemScrollIndex = 0;



            if (RootNodeList.Count > 0)
            {
           
                foreach (StructureNode rootNode in RootNodeList)
                {
                    rootNode.ItemIndex = TreeViewItemScrollIndex;
                    TreeViewItemScrollIndex++;
                    IfcStructureItems.Add(rootNode);


                    RenderTreeList(rootNode, 1);
                }


                //Debug.Log("ProcessTreeView: " + IfcStructureItems.Count);

                IfcStructureItemsAdapter.SetItems(IfcStructureItems);
                IfcStructureItemsAdapter.ForceUpdateVisibleItems();
            }
        }



        public void RenderTreeList(StructureNode node, int _Level = 0)
        {
            if (!node.IsCollapsed)
            {
                foreach (StructureNode item in node.childrenNodes)
                {
                    if (ProjectConfiguration.Instance.IsDisplaySearchResult)
                    {
                        if (item.IsSearchMatched > 0 || item.structureType == BimModel.StructureType.connect)
                        {
         
                            IfcStructureItems.Add(item);
                            item.ItemIndex = TreeViewItemScrollIndex;
                            TreeViewItemScrollIndex++;
                        }
                        else
                        {
              
                        }
                    }
                    else
                    {
                        IfcStructureItems.Add(item);
                        item.ItemIndex = TreeViewItemScrollIndex;
                        TreeViewItemScrollIndex++;
                    }

                    RenderTreeList(item, _Level + 1);
                }
            }
            else
            {

            }
        }



        #endregion

        // Widget Buttons

        #region Widget Buttons
        public void OnClick_UnhideAll()
        {

            foreach (StructureNode item in HidedObjects)
            {
                item.UnHideObject();
            }

            HidedObjects.Clear();

            IfcStructureItemsAdapter.SetItems(IfcStructureItems);

            CloseRightClickMenu();
            debug("Unhide all objects");
        }

        public void OnClick_UnselectAll()
        {
            foreach (GameObject ob in ProjectModelHandler.Instance.SelectedElements)
            {
                if (ob == null)
                {
                    continue;
                }

                BIMElement element = ob.GetComponent<BIMElement>();

                if (ProjectConfiguration.Instance.IsShowSplitedObjects && element.SplitedObjects.Count > 0)
                {
                    foreach (GameObject subItem in element.SplitedObjects)
                    {
                        subItem.GetComponent<BIMElement>().RestoreObject();
                    }
                }

                ob.GetComponent<BIMElement>().RestoreObject();
            }


            ProjectModelHandler.Instance.SelectedElements.Clear();

            SelectionBox.OnDeselect();

            CloseRightClickMenu();

        }

        public void OnClick_CancelIsolation()
        {

            ProjectConfiguration.Instance.IsInInsolateMode = false;

            foreach (var models in ProjectModelHandler.Instance.modelVersions)
            {
                if (models.IsActive)
                {
                    foreach (GameObject ob in models.gameObjectRoot.GetComponent<IfcRootLists>().ifcGameObject)
                    {

                        BIMElement element = ob.GetComponent<BIMElement>();
                        element.SetSplitedObjects();
                        //element.RestoreObject();
                        element.RestoreObjectFromIsolate();

                        // check for splited object 


                        if (ProjectConfiguration.Instance.IsShowSplitedObjects)
                        {

                        }

                        if (element.SplitedObjects.Count > 0)
                        {
                            foreach (GameObject subItem in element.SplitedObjects)
                            {
                                //subItem.GetComponent<BIMElement>().RestoreObject();
                                subItem.GetComponent<BIMElement>().RestoreObjectFromIsolate();
                            }
                        }
                    }
                }

            }

            SelectionBox.isDrawing = false;
            ProjectModelHandler.Instance.IsolatedElements.Clear();

            OnClick_UnselectAll();
            SetAttributeView(null);
        }

        public void OnClick_OpenPanel_Detail_IFC()
        {
            Panel_Detail_IFC.OnPanelOpen();
            Panel_Detail_BCF.OnPanelClose();
            Panel_List_BCF.OnPanelClose();
            //Panel_Attributes.OnPanelOpen();

            Tab_TreeView.SetTab();

        }

        public void OnClick_ClosePanel_Detail_IFC()
        {
            Panel_Detail_IFC.OnPanelClose();
            //Panel_Attributes.OnPanelClose();
        }

        public void OnClick_SetView_Origin()
        {
            ModelVersion version = ProjectModelHandler.Instance.GetDefaultActiveModel();
            if (version != null)
            {
                OnInitFitView(version.bimModel.CombinedBound);
            }
        }



        public void OnClick_SetView_Top()
        {
            // check if there is selected elments
            if (ProjectModelHandler.Instance.SelectedElements.Count > 0)
            {
                Bounds bound = new Bounds();

                foreach (var item in ProjectModelHandler.Instance.SelectedElements)
                {
                    BIMElement element = item.GetComponent<BIMElement>();
                    if (element != null)
                    {
                        bound.Encapsulate(element.Collider.bounds);
                    }
                }

                OnInitFitView_Top(bound);
            }
            else
            {
                ModelVersion version = ProjectModelHandler.Instance.GetDefaultActiveModel();
                if (version != null)
                {
                    OnInitFitView_Top(version.bimModel.CombinedBound);
                }
            }
        }

        public void OnClick_SetView_SideLeft()
        {


            // check if there is selected elments
            if (ProjectModelHandler.Instance.SelectedElements.Count > 0)
            {
                Bounds bound = new Bounds();

                foreach (var item in ProjectModelHandler.Instance.SelectedElements)
                {
                    BIMElement element = item.GetComponent<BIMElement>();
                    if (element != null)
                    {
                        bound.Encapsulate(element.Collider.bounds);
                    }
                }

                OnInitFitView_TurnLeft(bound);
            }
            else
            {
                ModelVersion version = ProjectModelHandler.Instance.GetDefaultActiveModel();
                if (version != null)
                {
                    OnInitFitView_TurnLeft(version.bimModel.CombinedBound);
                }
            }

        }

        public void OnClick_SetView_SideRight()
        {
            // check if there is selected elments
            if (ProjectModelHandler.Instance.SelectedElements.Count > 0)
            {
                Bounds bound = new Bounds();

                foreach (var item in ProjectModelHandler.Instance.SelectedElements)
                {
                    BIMElement element = item.GetComponent<BIMElement>();
                    if (element != null)
                    {
                        bound.Encapsulate(element.Collider.bounds);
                    }
                }

                OnInitFitView_TurnRight(bound);
            }
            else
            {
                ModelVersion version = ProjectModelHandler.Instance.GetDefaultActiveModel();
                if (version != null)
                {
                    OnInitFitView_TurnRight(version.bimModel.CombinedBound);
                }
            }

        }




        // WIP
        public void OnClick_OpenPanel_GraphicSettting()
        {

        }

        // WIP
        public void OnClick_ClosePanel_GraphicSettting()
        {

        }

        //WIP
        public void OnClick_SetViewNav(bool _isFPS)
        {
            if (_isFPS)
            {

            }
            else
            {

            }
        }

        RenderTexture rt;
        // Snapshot Related
        public void OnClick_TakeProjectSnapShot()
        {
            if (currentEditorMode == "edit")
            {
                return; // Upload BCF image function disabled.
            }

            rt = new RenderTexture(Screen.width, Screen.height, 24);
            NavCam.MainCamera.targetTexture = rt;

            Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            NavCam.MainCamera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);

            NavCam.MainCamera.targetTexture = null;
            RenderTexture.active = null;

            screenShot = Utility.ResampleAndCrop(screenShot, 512, 512);
            byte[] bytes = screenShot.EncodeToPNG();
            imageBase64 = Convert.ToBase64String(bytes);

            // open image viewer

            //StartCoroutine(DataProxy.Instance.ProjectSnapshotImageUpload(Page_Dashboard.Instance.SelectedProject.guid, imageBase64, ProjectSnaphotImageUploadCallback));

        }

        public void ProjectSnaphotImageUploadCallback(bool success, string message)
        {
            Destroy(rt);

            if (success)
            {
                Debug.Log(DataProxyResponse<IModel>.FromJson(message).package);
                MCPopup.Instance.SetComplete("Model Snapshot Saved");
            }
        }


        // need move to model handler
        public StructureNode SearchedNode;
        public bool SearchedNodeResult = false;

        public StructureNode FindNodeInStructureView(string _ifcID)
        {
            SearchedNodeResult = false;
            SearchedNode = null;

            foreach (var node in RootNodeList)
            {
                if (!SearchedNodeResult)
                {
                    SearchStructureNode(node, _ifcID);
                }
            }

            return SearchedNode;
        }


        public void SearchStructureNode(StructureNode _node, string _ifc)
        {
            if (_node.itemID == _ifc)
            {
                SearchedNode = _node;
                SearchedNodeResult = true;
            }

            if (_node.childrenNodes.Count > 0)
            {
                foreach (var node in _node.childrenNodes)
                {
                    // no find, contiune next
                    if (!SearchedNodeResult)
                    {
                        SearchStructureNode(node, _ifc);
                    }
                }
            }

        }

        // ============== Toggle Control ===========================

        // =======================================================
        // Tools

        public bool IsSectionBoxInited;

        public void OnToggle_DrawEdge(bool _isOn)
        {
            if (_isOn)
            {
                PageStatus.IsDrawingModelEdge = true;
            }
            else
            {
                PageStatus.IsDrawingModelEdge = false;
            }
        }


        public void OnToggle_SectionBox(bool _isOn)
        {
            if (_isOn)
            {
                if (!IsSectionBoxInited)
                {
                    if (ProjectModelHandler.Instance.CurrentModel != null) {
                        SectionBoxController.OnSectionBoxEnable(ProjectModelHandler.Instance.CurrentModel.bimModel.CombinedBound, Vector3.zero);
                        IsSectionBoxInited = true;
                    }
                }
                else
                {
                    SectionBoxController.OnSectionBoxEnable();
                }
            }
            else
            {
                NavCam.enabled = true;
                SectionBoxController.OnSectionBoxDisable();
            }
        }


        public void OnToggle_DrawReferenceLienOrPlane(bool _isOn)
        {
            if (_isOn)
            {

            } else {

            }

        }

        public void OnToggle_Measurement(bool _isOn)
        {
            if (_isOn)
            {
                MCPopup.Instance.SetInformation("Measurement under development");
            }
            else
            {
                //DrawSplitPlaneHandler.isDrawingReady = false;
            }

        }



        public void OnToggle_ShowSplitedElement(bool _isOn)
        {
            if (_isOn)
            {
                ProjectConfiguration.Instance.IsShowSplitedObjects = true;
            }
            else
            {
                ProjectConfiguration.Instance.IsShowSplitedObjects = false;
            }

            if (!ObjectSplitHandler.Instance.IsEditingEnabled)
            {
                //refresh all object to apply the setting
                OnApplyViewerSetting();
            }

        }



        public void OnToggle_ShowSelectedElement(bool _isOn)
        {
            if (_isOn)
            {
                ProjectConfiguration.Instance.IsDisplaySelectedElement = true;
            }
            else
            {
                ProjectConfiguration.Instance.IsDisplaySelectedElement = false;
            }

            Tab_TreeView.SetTab();
        }


        public void OnToggle_SetRemoteCommand(bool _isOn)
        {
            if (_isOn)
            {
                CommandController.Instance.isCommandQueueRunning = 1;
                MCPopup.Instance.SetConfirm(OnToggleStartRemoteCommand_Confirm, "","Remote Command is enabled, do you wish to reload material list?");
            }
            else
            {
                CommandController.Instance.isCommandQueueRunning = -1;
            }
        }
        

        public void OnToggleStartRemoteCommand_Confirm(bool _result, string _messsage)
        {
            string modelGuild = ProjectModelHandler.Instance.GetDefaultActiveModel().Project.guid;

            CommandController.Instance.StartRemoteCommandMode(modelGuild);

            if (_result)
            {

                // get current bimMOdel
                var mv = ProjectModelHandler.Instance.GetDefaultActiveModel();
                var model = mv.bimModel;

                if (model.bimMaterials.Count == 0)
                {
                    MCPopup.Instance.SetWarning("Material list is not ready to upload, check the model in OBJECT tab and try again");
                    return;
                }

                EasycarbonProject newCarbonProject = new EasycarbonProject();
                newCarbonProject.guid = mv.Project.guid;
                newCarbonProject.projectName = mv.Project.projectName;
                newCarbonProject.attachedMetaBIMProject = mv.Project.guid;
                newCarbonProject.organizationName = "Darkspede Pty Ltd";

                /* ------------ HARDCODE ---------------- */
                //TODO, hardcoded address

                newCarbonProject.projectLocation = new Location(
                    "4/75 Mark Street",
                    "",
                    "North Melboure",
                    "VIC",
                    3051,
                    "Australia"
                    );

                newCarbonProject.heading = 17f;
                newCarbonProject.baseEvluation = 5.71f;
                newCarbonProject.emissionFactorDatabase = "AusLCI";
                newCarbonProject.baseLength = 35.2f;
                newCarbonProject.baseWidth = 20.5f;
                newCarbonProject.baseHeight = 9.8f;

                newCarbonProject.projectLocation.SetCoordinate(-37.7945821, 144.9373748, 0);

                /* ------------ END ----------------*/


                newCarbonProject.materials = model.bimMaterials;


                newCarbonProject.materials.Sort((x, y) => x.elementName.CompareTo(y.elementName));

                StartCoroutine(DataProxy.Instance.UpdateEasycarbonProject(EasycarbonProject.ToJson(newCarbonProject), OnUpdateCarbonProject_Callback));
            }
            else
            {
                MCPopup.Instance.SetInformation("Remote Sync is enabled");
            }


    
        }

        public void OnUpdateCarbonProject_Callback(bool _result, string _message)
        {

            if (_result)
            {
                var response = DataProxyResponse<EasycarbonProject>.FromJson(_message);

                if (response.result)
                {
                    MCPopup.Instance.SetInformation("Material list is uploaded");
                }
                else
                {
                    MCPopup.Instance.SetInformation("Upload failed");
                }
            }
            else
            {
                MCPopup.Instance.SetInformation("Upload with unknow erorr.");
            }
        }



        // =======================================================
        // Data Visualizations

        public void OnToggle_LevelSetion(bool _isOn)
        {
            if (_isOn)
            {
                SectionPlaneHandler.OnViewEnable();
            }
            else
            {
                SectionPlaneHandler.OnViewDisable();
            }
        }


        public void OnToggle_RoomSections(bool _isOn)
        {

        }

        public void OnClick_OpenModelLevel()
        {
            Debug.Log("OnClick_OpenModelLevel");
            List<BimLevel> level = ProjectModelHandler.Instance.GetModelLevel();

            if (level != null)
            {
                Page_ModelLevel.Instance.OpenPanel(level);
            }
            else
            {
                MCPopup.Instance.SetInformation("Can not level window when there are more them one model opened.");
            }

        }


        // ============== END Toggle Control ===========================


        #endregion


        #region Right Click Menu (3D View)

        public void RightClickMenuAction_3DView()
        {

            if (ObjectSplitHandler.Instance.IsEditingEnabled)
            {
                return;
            }

            // Menu Action
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (!MouseInputUIBlocker.BlockedByUI)
                {
                    // blocked by UI, do not open menu
                    if (IsMenuOpend)
                    {
                        CloseRightClickMenu();
                    }

                    // but we do need to open if it is one some UI

                }

            }


            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                if (IsMenuOpend)
                {
                    CloseRightClickMenu();
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {

                LastRightClick = Input.mousePosition;
                RightClickSelectedObject = null;
                CloseRightClickMenu();
            }

            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                // if the click position is not at the same place
                if (LastRightClick != Input.mousePosition)
                {
                    return;
                }


                if (MouseInputUIBlocker.BlockedByUI)
                {
                    RightClickSelectedObject = null;

                    if (RightClickMenu.currentMenuType == UIController_RightClickMenu.MenuType.cameraView)
                    {
                        CloseRightClickMenu();  // 暂时先关闭， 之后需要检查UI物件
                    }
                }
                else
                {
                    // Right Click Menu
                    RaycastHit hit;
                    Ray ray = NavCam.MainCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        RightClickSelectedObject = hit.transform.gameObject;
                        //CopyMessage.SetActive(true);
                        CopiedContent = RightClickSelectedObject.name;
                    }
                    else
                    {
                        RightClickSelectedObject = null;
                        //CopyMessage.SetActive(false);
                    }


                    OpenRightClickMenu_3DView();
                }
            }

        }

        public void OpenRightClickMenu_3DView()
        {
            if (GUIUtility.systemCopyBuffer != "")
            {
                //PasteMessage.SetActive(true);
                //PasteContent.text = GUIUtility.systemCopyBuffer;

                // TODO past content to a target
            }
            else
            {
                // PasteMessage.SetActive(false);
            }

            RightClickMenu.OnMenuOpen(UIController_RightClickMenu.MenuType.cameraView);
            IsMenuOpend = true;

        }

        public void CloseRightClickMenu()
        {
            RightClickMenu.OnMenuClose();
            IsMenuOpend = false;
            RightClickSelectedObject = null;
        }

        #endregion


        #region Right Click Menu (BIM Tree View Panel)

        [Header("Right Click Menu (BIM Tree View Panel)")]
        public UIBlock_BimViewer_IfcStructureItem SelecetdTreeNode;

        public void OpenRightClickMenu_BIMStructureTree(UIBlock_BimViewer_IfcStructureItem _item)
        {
            if (_item != null)
            {
                SelecetdTreeNode = _item;
                OpenRightClickMenu_IFCStructure();
            }
            else
            {
                RightClickMenu.OnMenuClose();
            }


        }


        public void OpenRightClickMenu_IFCStructure()
        {
            Debug.Log("OpenRightClickMenu_IFCStructure: " + SelecetdTreeNode.name);
            //MenuRect.position = Input.mousePosition;
            RightClickMenu.OnMenuOpen(UIController_RightClickMenu.MenuType.bimstructure);
            IsMenuOpend = true;
        }



        public void TreeMenuAction_IsolateObject()
        {
            Debug.Log("TreeMenuAction_IsolateObject: ");

            if (SelecetdTreeNode != null)
            {
                StructureNode node = SelecetdTreeNode.Item;


            }


            CloseRightClickMenu();
        }

        public void TreeMenuAction_FitView()
        {
            if (SelecetdTreeNode != null)
            {
                StructureNode node = SelecetdTreeNode.Item;

                if (node.element.Renderer != null)
                {
                    FitView(node.element.gameObject, 1f);
                }
                else
                {
                    debug("SelecetdTreeNode has No Object linked");
                }
            }


            CloseRightClickMenu();
        }

        #endregion


        #region Menu Actions

        public void MenuAction_FitView()
        {
            if (RightClickSelectedObject != null)
            {
                FitView(1f);
            }
            else
            {
                debug("No Object Selected");
            }

            CloseRightClickMenu();
        }

        public void MenuAction_GetElementID()
        {
            if (RightClickSelectedObject != null && RightClickSelectedObject.GetComponent<BIMElement>() != null)
            {
                string ifcID = RightClickSelectedObject.GetComponent<BIMElement>().BimObject.elementID;
                debug("Element ID: " + ifcID);

            }

            CloseRightClickMenu();
        }

        public void MenuAction_Copy()
        {
            //PasteContent.text = CopiedContent.text;
            GUIUtility.systemCopyBuffer = CopiedContent;
            debug("Copy: " + GUIUtility.systemCopyBuffer);
            CloseRightClickMenu();
        }

        public void MenuAction_Paste()
        {

            CloseRightClickMenu();
        }

        public void MenuAction_ModelSnapshot()
        {
            // TODO
            CloseRightClickMenu();
        }

        public void MenuAction_HideObject()
        {
            if (RightClickSelectedObject != null)
            {
                BIMElement element = RightClickSelectedObject.GetComponent<BIMElement>();


                if (element == null)
                {
                    return;
                }

                string ifcID = RightClickSelectedObject.GetComponent<BIMElement>().BimObject.elementID;

                Debug.Log("RightClickSelectedObject: " + ifcID);

                StructureNode node = FindNodeInStructureView(ifcID);


                if (node != null)
                {
                    debug("Hide Object: " + ifcID);
                    node.OnHideObject(false);
                    HidedObjects.Add(node);

                    // OSA Data
                    IfcStructureItemsAdapter.SetItems(IfcStructureItems);
                }
                else
                {

                    Debug.Log("not found: ");
                }


            }
            else
            {
                debug("No Object Selected");
            }

            CloseRightClickMenu();
        }

        public void MenuAction_IsolateObjects()
        {
            Debug.Log("MenuAction_IsolateObject: " + RightClickMenu.currentMenuType);


            switch (RightClickMenu.currentMenuType)
            {
                case UIController_RightClickMenu.MenuType.bimstructure:
                    OnIsolateNodeObject();
                    break;
                case UIController_RightClickMenu.MenuType.cameraView:
                    OnIsolate3DViewerObject();
                    break;
            }


            //SetAttributesView(ProjectModelHandler.Instance.IsolatedElements);
            CloseRightClickMenu();
        }

        public void MenuAction_SplitSelectedElement()
        {
            OnClick_OnObjectSpliting();


        }

        // select objects in SelectedObjects


        // Select object to isolate from tree viewer
        public void OnIsolateNodeObject()
        {
            /*
             1. select objects and place in list
            2. restore other meterial 
            3. set material for all object in the list,
            4. set attributes menu
             */
            ProjectModelHandler.Instance.IsolatedElements.Clear();

            Debug.Log("OnIsolateNodeObject: " + RightClickMenu.currentMenuType + ", " + SelecetdTreeNode.name);

            if (SelecetdTreeNode != null)
            {
                OnRequestSelectObjectsFromNode(SelecetdTreeNode.Item);
            }

            Debug.Log("Process Objects: " + ProjectModelHandler.Instance.IsolatedElements.Count);

            if (ProjectModelHandler.Instance.IsolatedElements.Count < 1)
            {
                return;
            }

            // set isolating effect
            foreach (var models in ProjectModelHandler.Instance.modelVersions)
            {
                if (models.IsActive)
                {
                    foreach (GameObject ob in models.gameObjectRoot.GetComponent<IfcRootLists>().ifcGameObject)
                    {
                        BIMElement element = ob.GetComponent<BIMElement>();
                        element.SetSplitedObjects();

                        if (ProjectModelHandler.Instance.IsolatedElements.Contains(ob))
                        {
                            element.RestoreObject();

                            if (ProjectConfiguration.Instance.IsShowSplitedObjects && element.SplitedObjects.Count > 0)
                            {
                                foreach (GameObject subItem in element.SplitedObjects)
                                {
                                    if (ProjectModelHandler.Instance.IsolatedElements.Contains(subItem))
                                    {
                                        subItem.GetComponent<BIMElement>().SetToIsolatedMode();
                                    }
                                    else
                                    {
                                        subItem.GetComponent<BIMElement>().RestoreObject();
                                    }

                                }
                            }

                        }
                        else
                        {
                            element.SetToIsolatedMode();

                            if (ProjectConfiguration.Instance.IsShowSplitedObjects && element.SplitedObjects.Count > 0)
                            {
                                foreach (GameObject subItem in element.SplitedObjects)
                                {
                                    if (!ProjectModelHandler.Instance.IsolatedElements.Contains(subItem))
                                    {
                                        subItem.GetComponent<BIMElement>().SetToIsolatedMode();
                                    }
                                    else
                                    {
                                        subItem.GetComponent<BIMElement>().RestoreObject();
                                    }
                                }
                            }


                        }


                    }
                }
            }


            ProjectModelHandler.Instance.CurrentHoveringObject = null;
            SelectionBox.isDrawing = false;
        }

        public void OnIsolateNodeObjectWithClass(string _ifcclass)
        {
   
            ProjectModelHandler.Instance.IsolatedElements.Clear();
  
            // set isolating effect
            foreach (var models in ProjectModelHandler.Instance.modelVersions)
            {
                if (models.IsActive)
                {
                    foreach (GameObject ob in models.gameObjectRoot.GetComponent<IfcRootLists>().ifcGameObject)
                    {
                        BIMElement element = ob.GetComponent<BIMElement>();
                        string ifcclass = "";

                        if (element != null && element.BimObject.records.Count > 0)
                        {
                            ifcclass = element.BimObject.records[0].ifcAttribute.Find("IfcElementType");
                        }
                        else
                        {
                            continue;
                        }
                       
                        element.SetSplitedObjects();

                        if (ifcclass == _ifcclass)
                        {
                            element.RestoreObject();

                            if (ProjectConfiguration.Instance.IsShowSplitedObjects && element.SplitedObjects.Count > 0)
                            {
                                foreach (GameObject subItem in element.SplitedObjects)
                                {
                                    subItem.GetComponent<BIMElement>().RestoreObject();

                                }
                            }

                        }
                        else
                        {
                            element.SetToIsolatedMode();

                            if (ProjectConfiguration.Instance.IsShowSplitedObjects && element.SplitedObjects.Count > 0)
                            {
                                foreach (GameObject subItem in element.SplitedObjects)
                                {
                                    subItem.GetComponent<BIMElement>().SetToIsolatedMode();
                                }
                            }


                        }


                    }
                }
            }


            ProjectModelHandler.Instance.CurrentHoveringObject = null;
            SelectionBox.isDrawing = false;
        }


        public void OnIsolateNodeObjectWithObjectName(string _objectName)
        {

            ProjectModelHandler.Instance.IsolatedElements.Clear();
            Bounds newbound = new Bounds();
            // set isolating effect
            foreach (var models in ProjectModelHandler.Instance.modelVersions)
            {
                if (models.IsActive)
                {
                    foreach (GameObject ob in models.gameObjectRoot.GetComponent<IfcRootLists>().ifcGameObject)
                    {
                        BIMElement element = ob.GetComponent<BIMElement>();
                        string objectName = "";

                        if (element != null && element.BimObject.records.Count > 0)
                        {
                            objectName = element.BimObject.records[0].ifcAttribute.Find("PredefinedType");         
                        }
                        else
                        {
                            continue;
                        }

                        element.SetSplitedObjects();

                        //
                        if(objectName == null || objectName == "")
                        {
                            element.SetToIsolatedMode();

                            if (ProjectConfiguration.Instance.IsShowSplitedObjects && element.SplitedObjects.Count > 0)
                            {
                                foreach (GameObject subItem in element.SplitedObjects)
                                {
                                    subItem.GetComponent<BIMElement>().SetToIsolatedMode();
                                }
                            }

                            continue;
                        }

                        if (objectName.ToLower() == _objectName.ToLower())
                        {
                            element.RestoreObject();
                            // adding object to bound ecapsulation
                            if (element.Collider != null)
                            {
                                newbound.Encapsulate(element.Collider.bounds);
                            }

                            if (ProjectConfiguration.Instance.IsShowSplitedObjects && element.SplitedObjects.Count > 0)
                            {
                                foreach (GameObject subItem in element.SplitedObjects)
                                {
                                    subItem.GetComponent<BIMElement>().RestoreObject();

                                }
                            }

                        }
                        else
                        {
                            element.SetToIsolatedMode();

                            if (ProjectConfiguration.Instance.IsShowSplitedObjects && element.SplitedObjects.Count > 0)
                            {
                                foreach (GameObject subItem in element.SplitedObjects)
                                {
                                    subItem.GetComponent<BIMElement>().SetToIsolatedMode();
                                }
                            }


                        }


                    }
                }
            }

         
            OnInitFitView(newbound);

            ProjectModelHandler.Instance.CurrentHoveringObject = null;
            SelectionBox.isDrawing = false;
        }


        private void OnRequestSelectObjectsFromNode(StructureNode _node)
        {
            if (_node.element != null)
            {
                ProjectModelHandler.Instance.IsolatedElements.Add(_node.element.gameObject);

                // need to double check
                if (_node.element.SplitedObjects.Count > 0 && ProjectConfiguration.Instance.IsShowSplitedObjects)
                {
                    foreach (GameObject subItem in _node.element.SplitedObjects)
                    {
                        ProjectModelHandler.Instance.IsolatedElements.Add(subItem);
                    }
                }
            }





            if (_node.childrenNodes.Count > 0)
            {
                foreach (StructureNode node in _node.childrenNodes)
                {
                    // also add the object split from zone into isolate

                    OnRequestSelectObjectsFromNode(node);
                }
            }
        }


        // isolated selected objects
        public void OnIsolate3DViewerObject()
        {

            //Debug.Log("OnIsolate3DViewerObject: ");

            // maybe not good for mutiple object isolation
            //RightClickSelectedObject.GetComponent<BIMElement>().RestoreObject();

            // isolate from selection
            if (ProjectModelHandler.Instance.SelectedElements.Count > 0)
            {
                ProjectModelHandler.Instance.IsolatedElements.Clear();

                // this block is essiental the unisolated function
                foreach (var models in ProjectModelHandler.Instance.modelVersions)
                {
                    if (models.IsActive)
                    {
                        var root = models.gameObjectRoot.GetComponent<IfcRootLists>().ifcGameObject;
                        Debug.Log("OnIsolate3DViewerObject: " + root.Count);

                        foreach (GameObject ob in root)
                        {
                            BIMElement element = ob.GetComponent<BIMElement>();


                            element.SetSplitedObjects();
                            element.SetToIsolatedMode();

                            if (ProjectConfiguration.Instance.IsShowSplitedObjects && element.SplitedObjects.Count > 0)
                            {
                                foreach (GameObject subItem in element.SplitedObjects)
                                {
                                    subItem.GetComponent<BIMElement>().SetToIsolatedMode();
                                }
                            }

                        }
                    }
                }

                foreach (GameObject OB in ProjectModelHandler.Instance.SelectedElements)
                {
                    BIMElement element = OB.GetComponent<BIMElement>();

                    if (ProjectConfiguration.Instance.IsShowSplitedObjects && element.SplitedObjects.Count > 0)
                    {
                        foreach (GameObject subItem in element.SplitedObjects)
                        {
                            subItem.GetComponent<BIMElement>().RestoreObject();
                        }
                    }
                    else
                    {
                        element.RestoreObject();
                    }

                    ProjectModelHandler.Instance.IsolatedElements.Add(OB);
                }

                ProjectConfiguration.Instance.IsInInsolateMode = true;
                SelectionBox.isDrawing = false;
            }

            ProjectModelHandler.Instance.CurrentHoveringObject = null;           // needed?

        }



        public void OnApplyViewerSetting()
        {
            //Debug.Log("OnApplyViewerSetting: ");

            // this block is essiental the unisolated function
            foreach (var models in ProjectModelHandler.Instance.modelVersions)
            {
                if (models.IsActive)
                {
                    var root = models.gameObjectRoot.GetComponent<IfcRootLists>().ifcGameObject;
                    Debug.Log("OnIsolate3DViewerObject: " + root.Count);

                    foreach (var node in root)
                    {
                        if (node != null)
                        {
                            BIMElement element = node.GetComponent<BIMElement>();

                            element.SetSplitedObjects();
                        }
                    }
                }
            }



        }




        #endregion


        #region BCF

        [Header("BCF Information")]
        public GameObject BCFItemTemplate;
        public List<GameObject> BCFItemList;
        public Transform BCFListParent;

        public GameObject AddNewValidateButton;
        public GameObject UpdateButton;
        public MC_GetWebIcon Image_CapturedSnapshot;

        public BCF newBCF = new BCF();
        public TextMeshProUGUI guidText;
        public TextMeshProUGUI timeText;

        public TMP_InputField BCFTitle;
        public TMP_InputField BCFDescription;
        public TMP_Dropdown BCFType;
        public TMP_Dropdown BCFPriority;
        public TMP_Dropdown BCFAssignTo;
        public TMP_Dropdown BCFStatus;

        string currentEditorMode = "";

        string imageBase64 = "";

        public void OnClick_OpenPanel_List_BCF()
        {
            Panel_List_BCF.OnPanelOpen();
            Panel_Detail_IFC.OnPanelClose();
            Panel_Detail_BCF.OnPanelClose();
            RenderBCFList();
        }

        public void OnClick_ClosePanel_List_BCF()
        {
            Panel_Detail_BCF.OnPanelClose();
            Panel_List_BCF.OnPanelClose();
        }

        public void OnClick_OpenPanel_Detail_BCF(string mode)
        {
            currentEditorMode = mode;
            Debug.Log("Editor mode: " + currentEditorMode);
            Panel_Detail_IFC.OnPanelClose();
            Panel_Detail_BCF.OnPanelOpen();
            if (mode == "new")
            {
                NewBCFForm();
            }
            else if (mode == "edit")
            {
                LoadBCFForm();
            }
        }

        public void OnClick_ClosePanel_Detail_BCF()
        {
            Panel_Detail_BCF.OnPanelClose();
        }

        // Render list and Select BCF
        public void RenderBCFList()
        {
            ClearObjectList(BCFItemList);

            /*
            foreach (BCF item in CurrentProject.bfcs)
            {
                GameObject ob = Instantiate(BCFItemTemplate, BCFListParent);
                ob.SetActive(true);
                ob.GetComponent<UIBlock_BimViewer_BCFItem>().SetBlock(item);          
                BCFItemList.Add(ob);
            }
            */
        }

        public void OnClick_SelectBCF(GameObject _gameObject)
        {
            if (_gameObject.GetComponent<UIBlock_BimViewer_BCFItem>() != null)
            {
                CurrentBCF = _gameObject.GetComponent<UIBlock_BimViewer_BCFItem>().Item;
                //BCFImage.GetComponent<RawImage>().texture = _gameObject.GetComponent<UIBlock_BimViewer_BCFItem>().BCFImage.Target.texture;
                MCPopup.Instance.SetConfirm(OnSelectBCFConfirm_Callback, CurrentBCF.issueTitle, "Open project  " + CurrentBCF.issueTitle);
            }
            else
            {
                MCPopup.Instance.SetWarning("Invalid BCF");
            }
        }

        public void OnSelectBCFConfirm_Callback(bool _success, string message)
        {
            if (_success)
            {
                OnClick_OpenPanel_Detail_BCF("edit");
            }
            else
            {
                CurrentBCF = null;
            }
        }

        // BCF Detail Panel
        public void NewBCFForm()
        {
            newBCF = new BCF();

            BCFTitle.text = "";
            BCFDescription.text = "";
            imageBase64 = "";
            BCFType.value = 0;
            BCFPriority.value = 0;
            BCFAssignTo.value = 0;
            BCFStatus.value = 0;

            guidText.text = "ID: " + Utility.GetLastPartOfGuid(newBCF.guid);
            timeText.text = DateTime.Now.ToString("yyyy/MM/dd");
            UpdateButton.SetActive(false);
            AddNewValidateButton.SetActive(true);

            Image_CapturedSnapshot.SetBlock("");
        }

        public void LoadBCFForm()
        {
            Image_CapturedSnapshot.SetBlock(Config.BCFImage_Path + CurrentBCF.guid + "/" + CurrentBCF.snapshotImageUrl);

            guidText.text = "ID: " + Utility.GetLastPartOfGuid(CurrentBCF.guid);
            timeText.text = new DateTime(long.Parse(CurrentBCF.updated)).ToString("yyyy/MM/dd");


            BCFTitle.text = CurrentBCF.issueTitle;
            BCFDescription.text = CurrentBCF.issueContent;

            for (int i = 0; i < BCFType.options.Count; i++)
            {
                if (BCFType.options[i].text == CurrentBCF.issueType)
                {
                    BCFType.value = i;
                    break;
                }
            }

            for (int i = 0; i < BCFPriority.options.Count; i++)
            {
                if (BCFPriority.options[i].text == CurrentBCF.priority)
                {
                    BCFPriority.value = i;
                    break;
                }
            }

            for (int i = 0; i < CollaborationUsers.Count; i++)
            {
                if (CurrentBCF.assignedTo == CollaborationUsers[i].guid)
                {
                    BCFAssignTo.value = i;
                    break;
                }
            }

            for (int i = 0; i < BCFStatus.options.Count; i++)
            {
                if (BCFStatus.options[i].text == CurrentBCF.amendingStatus)
                {
                    BCFStatus.value = i;
                    break;
                }
            }

            UpdateButton.SetActive(true);
            AddNewValidateButton.SetActive(false);

        }

        // Snapshot Related
        public void OnClick_TakeSnapShot()
        {
            if (currentEditorMode == "edit")
            {
                return; // Upload BCF image function disabled.
            }

            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
            NavCam.MainCamera.targetTexture = rt;

            Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            NavCam.MainCamera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);

            NavCam.MainCamera.targetTexture = null;
            RenderTexture.active = null;
            byte[] bytes = screenShot.EncodeToPNG();
            imageBase64 = Convert.ToBase64String(bytes);


            Destroy(rt);


            //StartCoroutine(DataProxy.Instance.BCFImageUpload(newBCF.guid, imageBase64, BCFImageUploadCallback));

        }

        public void BCFImageUploadCallback(bool success, string message)
        {
            if (success)
            {
                newBCF.snapshotImageUrl = DataProxyResponse<IModel>.FromJson(message).message;
                string imageURL = Config.BCFImage_Path + newBCF.guid + "/" + newBCF.snapshotImageUrl;
                //reload image into

                Debug.Log(imageURL);
                Image_CapturedSnapshot.SetBlock(imageURL);

            }
        }

        // Update or Validate Button
        public void OnClick_ValidateBCF()
        {
            /*
            bool duplicate = false;
            foreach(BCF item in CurrentProject.bfcs)
            {
                if(item.issueTitle == BCFTitle.text)
                {
                    duplicate = true;
                    break;
                }
            }

            if (duplicate)
            {
                MCPopup.Instance.SetWarning("BCF with same title already exists. Please use another title.");
                return;
            }

            if(newBCF.snapshotImageUrl == "" || newBCF.snapshotImageUrl == null)
            {
                MCPopup.Instance.SetWarning("Please add a snapshot. If added, please wait while uploading.");
                return;
            }

            if(BCFTitle.text != "" && BCFDescription.text != "")
            {
                MCPopup.Instance.SetConfirm(AddNewBCFConfirm_Callback, BCFTitle.text, "Add new BCF\n" + BCFTitle.text);
            }
            else
            {
                MCPopup.Instance.SetWarning("Please enter title and description.");
                return;
            }
            */
        }

        public void AddNewBCFConfirm_Callback(bool success, string message)
        {
            if (success)
            {
                newBCF.issueTitle = BCFTitle.text;
                newBCF.issueContent = BCFDescription.text;
                newBCF.issuedBy = AppController.Instance.CurrentProfile.guid;

                newBCF.assignedTo = CollaborationUsers[BCFAssignTo.value].guid;
                newBCF.dueDate = DateTime.Now.Ticks.ToString();
                newBCF.issueType = BCFType.options[BCFType.value].text;
                newBCF.priority = BCFPriority.options[BCFPriority.value].text;
                newBCF.amendingStatus = BCFStatus.options[BCFStatus.value].text;
                newBCF.comments = new List<BCFComment>();
                newBCF.attachedProject = CurrentProject.guid;
                newBCF.snapshotImageUrl = newBCF.guid + ".png";
                // TODO: Element ID from selected object
                newBCF.elementID = Guid.NewGuid().ToString();

                //CurrentProject.bfcs.Add(newBCF);
                //StartCoroutine(DataProxy.Instance.UpdateProject(Project.ToJson(CurrentProject), UpdateProjectCallback));
            }
        }

        public void UpdateProjectCallback(bool success, string message)
        {
            if (success)
            {
                CurrentProject = DataProxyResponse<Project>.FromJson(message).package[0];
                MCPopup.Instance.SetComplete("Update Project Succeeded.");
                Panel_Detail_BCF.OnPanelClose();
                RenderBCFList();
                return;
            }
            else
            {
                MCPopup.Instance.SetWarning("Update Project Failed.");
                return;
            }
        }

        public void OnClick_UpdateBCF()
        {
            /*
            bool duplicate = false;
            foreach (BCF item in CurrentProject.bfcs)
            {
                if (item.issueTitle == BCFTitle.text)
                {
                    if(item.guid != CurrentBCF.guid) // User trying to name two BCF with different guid with same name 
                    {
                        duplicate = true; 
                        break;
                    }
                }
            }

            if (duplicate)
            {
                MCPopup.Instance.SetWarning("BCF with same title already exists. Please use another title.");
                return;
            }


            if (BCFTitle.text != "" && BCFDescription.text != "")
            {
                MCPopup.Instance.SetConfirm(UpdateBCFConfirm_Callback, BCFTitle.text, "Update BCF " + BCFTitle.text);
            }
            else
            {
                MCPopup.Instance.SetWarning("Please enter title and description.");
                return;
            }
            */
        }



        public void UpdateBCFConfirm_Callback(bool _success, string message)
        {
            /*
            CurrentBCF.issueTitle = BCFTitle.text;
            CurrentBCF.issueContent = BCFDescription.text;

            CurrentBCF.assignedTo = CollaborationUsers[BCFAssignTo.value].guid;
            CurrentBCF.dueDate = DateTime.Now.Ticks.ToString();
            CurrentBCF.issueType = BCFType.options[BCFType.value].text;
            CurrentBCF.priority = BCFPriority.options[BCFPriority.value].text;
            CurrentBCF.amendingStatus = BCFStatus.options[BCFStatus.value].text;

            BCF BCFtoDelete = CurrentProject.bfcs.Find(x => x.guid == CurrentBCF.guid);
            CurrentProject.bfcs.Remove(BCFtoDelete);
            CurrentProject.bfcs.Add(CurrentBCF);

            StartCoroutine(DataProxy.Instance.UpdateProject(Project.ToJson(CurrentProject), UpdateProjectCallback));
            */
        }

        #endregion

        public string CurrentViewName;

        public void OnClick_SelectViewItem(string _viewerName = "")
        {
            SelectViewItem(_viewerName);
        }



        private void SelectViewItem(string _viewerName = "")
        {
            string toView = "";

            if (_viewerName == "")
            {
                toView = CurrentViewName;
            }
            else
            {
                CurrentViewName = _viewerName;
                toView = _viewerName;
            }

            Debug.Log("Open Tab " + toView);

            /*Reset other viewer */

            OnCloseCodeCheckingWidget();   // ?????


            // clear list viewer
            IfcStructureItems.Clear();
            PageStatus.OnReset();

            // if the tree viewer is not opened
            // set it to open

            Panel_Detail_IFC.OnPanelOpen();

            // Close the 
         

            switch (toView)
            {
                case "View Spatial":
                    ProcessModelStructureNodes(ProjectModelHandler.Instance.OnRequestProcessStructureNode_Sptail());
                    //ZoneBoxController.ZoneViewSwitch(false);
                    RoomHandler.Instance.DisableRoom();
                    break;
                case "View Objects":
                    ProcessModelStructureNodes(ProjectModelHandler.Instance.OnRequestProcessStructureNode_Objects());
                    //ZoneBoxController.ZoneViewSwitch(false);
                    RoomHandler.Instance.DisableRoom();
                    break;
                case "View IFC":
                    ProcessModelStructureNodes(ProjectModelHandler.Instance.OnRequestProcessStructureNode_Class());
                    //ZoneBoxController.ZoneViewSwitch(false);
                    RoomHandler.Instance.DisableRoom();
                    break;
                case "View Uniclass":
                    ProcessModelStructureNodes(ProjectModelHandler.Instance.OnRequestProcessStructureNode_Uniclass());
                    //ZoneBoxController.ZoneViewSwitch(false);
                    RoomHandler.Instance.DisableRoom();
                    break;
                case "Material":
                    //ProcessModelStructureNodes(ProjectModelHandler.Instance.OnRequestProcessStructureNode_Uniclass());
                    break;
                case "View Custom":
                    ProcessModelStructureNodes(ProjectModelHandler.Instance.OnRequestProcessStructureNode_Custom());
                    RoomHandler.Instance.DisableRoom();
                    break;
                case "View Type":

                    break;
                case "View System":

                    break;
                case "View Workset":

                    break;
                case "View Zone":
                    ProcessModelStructureNodes(ProjectModelHandler.Instance.OnRequestProcessStructureNode_Zone());
                    ZoneManagement.Instance.OnClick_Open();
                    RoomHandler.Instance.DisableRoom();
                    //TO DO, check if need to close other opertion window
                    break;
                case "View Room":
                    ProcessModelStructureNodes(ProjectModelHandler.Instance.OnRequestProcessStructureNode_Room());
                    RoomHandler.Instance.EnableRoom();
      
                    break;
                case "View Code Checking":
                    ProcessModelStructureNodes(ProjectModelHandler.Instance.OnRequestProcessStructureNode_Validation());
                    //OnOpenCodeCheckingWidget();
                    //ZoneBoxController.ZoneViewSwitch(false);
                    RoomHandler.Instance.DisableRoom();
                    break;
                case "View Set":

                    break;
                default:
                    ProcessModelStructureNodes(ProjectModelHandler.Instance.OnRequestProcessStructureNode_Sptail());
                    //ZoneBoxController.ZoneViewSwitch(false);
                    RoomHandler.Instance.DisableRoom();
                    break;
            }


        }

        public void OnClick_SelectTreeItem(GameObject _gameObject)
        {
            OnSelectTreeItem(_gameObject);

        }




        /// <summary>
        /// set is tree to false if you call from non-ui event, and dont want to trigger the infinit loop
        /// </summary>
        /// <param name="_gameObject"></param>
        /// <param name="_isTree"></param>
        public void OnSelectTreeItem(GameObject _gameObject, bool _isTree = true)
        {
            UIBlock_BimViewer_IfcStructureItem block;

            if (SelectedStructureTreeItem != null)
            {
                block = SelectedStructureTreeItem.GetComponent<UIBlock_BimViewer_IfcStructureItem>();
                block.OnDeselect();
            }

            block = _gameObject.GetComponent<UIBlock_BimViewer_IfcStructureItem>();

            GameObject ob;

            /*
             * to do,
             * group select all items in the next level
             */


            if (block.Item.element == null)
            {
                List<GameObject> _nodes = new List<GameObject>();

                if (block.Item.childrenNodes.Count > 0)
                {
                    GetALLChildNodes(_nodes, block.Item);
                }
                SelectMeshObjects(_nodes);
            }
            else
            {
                if (block.Item.element.Renderer == null)
                {
                    List<GameObject> _nodes = new List<GameObject>();

                    if (block.Item.childrenNodes.Count > 0)
                    {
                        GetALLChildNodes(_nodes, block.Item);
                    }

                    //Debug.Log("Node Selected: " + _nodes.Count);
                    SelectMeshObjects(_nodes);
                    block.OnSelect(); // Added, 2024-10-03, to show the selected item in the tree view
                }
                else
                {
                    ob = block.Item.element.gameObject;
                    SelectedStructureTreeItem = _gameObject;

                    if (_isTree)
                    {
                        SelectMeshObject(ob);
                    }

                    block.OnSelect();
                }
            }

        }

        public void GetALLChildNodes(List<GameObject> _nodes, StructureNode _node)
        {

            foreach (var node in _node.childrenNodes)
            {
                if (ProjectConfiguration.Instance.IsDisplaySearchResult)
                {
                    if (node.linkedObject != null && node.IsSearchMatched > 0)
                    {
                        _nodes.Add(node.linkedObject);
                    }
                }
                else
                {
                    if (node.linkedObject != null)
                    {
                        _nodes.Add(node.linkedObject);
                    }
                }


                if (node.childrenNodes.Count > 0)
                {
                    GetALLChildNodes(_nodes, node);
                }
            }


        }


        [Header("Attribute Tree")]
        public List<IfcAttributeItem> IfcAttributeItems;


        //Set AttributeView Single mode
        public void SetAttributeView(GameObject meshObject)
        {
            if (meshObject == null)
            {
                Panel_Attributes.OnPanelClose();
                return;
            }

            if (meshObject.GetComponent<BIMElement>().BimObject == null || meshObject.GetComponent<BIMElement>().BimObject.records.Count == 0)
            {
                Panel_Attributes.OnPanelClose();
                return;
            }


            //Debug.Log("Object Selected: " + meshObject.name);

            debug(meshObject.name);

            Panel_Attributes.OnPanelOpen();

            IfcAttributeItems.Clear();
            IfcAttributesAdapter.SetItems(IfcAttributeItems);

            /*
            if (!Panel_Detail_IFC.UI_Block.activeSelf)
            {
                return;
            }
            */

            int index = 0;
            int itemID = 0;

   

            Text_AttributePanel_Header.text = StringBuffer.AttributeViewer_Header_Prefix.S;

            // Setup IfcAttributes
            if (meshObject.GetComponent<BIMElement>() != null)
            {

         
                //MetaBIM_IfcCustomProperty ifcCustomProperties = CustomDataTree.Instance.metaBIM_IfcCustomProperty;

                //if (ifcCustomProperties != null && ifcCustomProperties.properties.Count > 0)
                //{

                //    index = 0;
                //    int pIndex = 0;
                //    IfcAttributeItem newItem = new IfcAttributeItem("", "", false, false);

                //    foreach (string name in ifcCustomProperties.properties)
                //    {
                //        var value = ifcCustomProperties.nominalValues[index];

                //        if (value == "header")
                //        {
                //            pIndex = 0;
                //            newItem = new IfcAttributeItem(ifcCustomProperties.properties[index], "", false, true);
                //            newItem.ItemId = itemID;
                //            IfcAttributeItems.Add(newItem);
                //            itemID++;
                //            newItem.AttributeItemObject = meshObject;
                //        }
                //        else
                //        {

                //            pIndex++;
                //            IfcAttributeItem newAttribute = new IfcAttributeItem(name, ifcCustomProperties.nominalValues[index], false, false);
                //            newAttribute.ItemId = itemID;
                //            newAttribute.ListIndex = pIndex;
                //            newItem.Childs.Add(newAttribute);
                //            IfcAttributeItems.Add(newAttribute);
                //            itemID++;
                //        }


                //        index++;
                //    }
                //}


                MetaBIM_IfcUniclass ifcUniclass = meshObject.GetComponent<BIMElement>().BimObject.records[LoadedVersion].ifcUniclass;

                if (ifcUniclass.attributes.Count >= 0)
                {
                    IfcAttributeItem newItem = new IfcAttributeItem("Uniclass", "", false, true);
                    newItem.ItemId = itemID;
                    itemID++;
                    IfcAttributeItems.Add(newItem);
                    index = 0;
                    newItem.Type = IfcAttributeItem.AttributeType.uniclass;
                    newItem.AttributeItemObject = meshObject;

                    foreach (string name in ifcUniclass.attributes)
                    {
                        if (ifcUniclass.Find(name) != "" && ifcUniclass.Find(name) != Config.None)
                        {
                            IfcAttributeItem newAttribute = new IfcAttributeItem(name, ifcUniclass.Find(name), false, false);
                            newAttribute.ListIndex = index;
                            newAttribute.ItemId = itemID;
                            newItem.Childs.Add(newAttribute);
                            IfcAttributeItems.Add(newAttribute);
                            itemID++;
                            index++;
                        }
                    }
                }
                

                MetaBIM_IfcUniclassMap ifcUniclassMap = meshObject.GetComponent<BIMElement>().BimObject.records[LoadedVersion].ifcUniclassMap;

                if (ifcUniclassMap.attributes.Count >= 0)
                {
                    IfcAttributeItem newItem = new IfcAttributeItem("Reference", "", false, true);
                    newItem.ItemId = itemID;
                    itemID++;
                    IfcAttributeItems.Add(newItem);
                    newItem.AttributeItemObject = meshObject;
                    index = 0;

                    foreach (string name in ifcUniclassMap.attributes)
                    {
                        if (ifcUniclassMap.Find(name) != "" && ifcUniclassMap.Find(name) != Config.None)
                        {
                            IfcAttributeItem newAttribute = new IfcAttributeItem(name, ifcUniclassMap.Find(name), false, false);
                            newAttribute.ListIndex = index;
                            newAttribute.ItemId = itemID;
                            newItem.Childs.Add(newAttribute);
                            IfcAttributeItems.Add(newAttribute);
                            itemID++;
                            index++;
                        }

                    }
                }


                MetaBIM_IfcParameter ifcParameter = meshObject.GetComponent<BIMElement>().BimObject.records[LoadedVersion].ifcParameter;

                if (ifcParameter.attributes.Count >= 0)
                {
                    IfcAttributeItem newItem = new IfcAttributeItem("IFC Parameter", "", false, true);
                    newItem.ItemId = itemID;
                    itemID++;
                    IfcAttributeItems.Add(newItem);
                    index = 0;
                    newItem.Type = IfcAttributeItem.AttributeType.ifcParameter;
                    newItem.AttributeItemObject = meshObject;

                    foreach (string name in ifcParameter.attributes)
                    {
                        IfcAttributeItem newAttribute = new IfcAttributeItem(name, ifcParameter.Find(name), false, false);
                        newAttribute.ListIndex = index;
                        newAttribute.ItemId = itemID;
                        newItem.Childs.Add(newAttribute);
                        IfcAttributeItems.Add(newAttribute);
                        itemID++;
                        index++;
                    }
                }


                MetaBIM_IfcZone ifcZone = meshObject.GetComponent<BIMElement>().BimObject.records[LoadedVersion].ifcZone;

                if (ifcZone.attributes.Count >= 0)
                {
                    IfcAttributeItem newItem = new IfcAttributeItem("Zone", "", false, true);
                    newItem.ItemId = itemID;
                    itemID++;
                    IfcAttributeItems.Add(newItem);
                    index = 0;
                    newItem.Type = IfcAttributeItem.AttributeType.zone;
                    newItem.AttributeItemObject = meshObject;

                    foreach (string name in ifcZone.attributes)
                    {
                        IfcAttributeItem newAttribute = new IfcAttributeItem(name, ifcZone.Find(name), false, false);
                        newAttribute.ListIndex = index;
                        newAttribute.ItemId = itemID;
                        newItem.Childs.Add(newAttribute);
                        IfcAttributeItems.Add(newAttribute);
                        itemID++;
                        index++;
                    }
                }


                MetaBIM_IfcAttributes ifcAttributes = meshObject.GetComponent<BIMElement>().BimObject.records[LoadedVersion].ifcAttribute;

                if (ifcAttributes.attributes.Count > 0)
                {
                    IfcAttributeItem newItem = new IfcAttributeItem("Attributes", "", false, true);
                    newItem.ItemId = itemID;
                    itemID++;
                    IfcAttributeItems.Add(newItem);
                    index = 0;
                    newItem.Type = IfcAttributeItem.AttributeType.general;
                    newItem.AttributeItemObject = meshObject;

                    int attributeIndex = 0;

                    foreach (string name in ifcAttributes.attributes)
                    {
                        IfcAttributeItem newAttribute = new IfcAttributeItem(name, ifcAttributes.Find(name), false, false);

                        if (ifcAttributes.checkedType.Count > 0 && attributeIndex < ifcAttributes.checkedType.Count)
                        {
                            newAttribute = new IfcAttributeItem(name, ifcAttributes.Find(name), false, false, ifcAttributes.checkedType[attributeIndex]);
                        }
        
                       
                        newAttribute.ListIndex = index;
                        newAttribute.ItemId = itemID;
                        newItem.Childs.Add(newAttribute);
                        IfcAttributeItems.Add(newAttribute);
                        itemID++;
                        index++;
                        attributeIndex++;
                    }
                }


                MetaBIM_IfcProperties ifcProperties = meshObject.GetComponent<BIMElement>().BimObject.records[LoadedVersion].ifcProperties;

                if (ifcProperties.properties.Count > 0)
                {

                    index = 0;
                    int pIndex = 0;
                    IfcAttributeItem newItem = new IfcAttributeItem("", "", false, false);

                    foreach (string name in ifcProperties.properties)
                    {

                        if (name == "PsetName")
                        {
                            pIndex = 0;
                            newItem = new IfcAttributeItem(ifcProperties.nominalValues[index], "", false, true);
                            newItem.ItemId = itemID;
                            IfcAttributeItems.Add(newItem);
                            itemID++;
                            newItem.AttributeItemObject = meshObject;
                        }
                        else
                        {

                            pIndex++;
                            IfcAttributeItem newAttribute = new IfcAttributeItem(name, ifcProperties.nominalValues[index], false, false);
                            newAttribute.ItemId = itemID;
                            newAttribute.ListIndex = pIndex;
                            newItem.Childs.Add(newAttribute);
                            IfcAttributeItems.Add(newAttribute);
                            itemID++;
                        }


                        index++;
                    }
                }


                MetaBIM_IfcMaterials ifcMaterials = meshObject.GetComponent<BIMElement>().BimObject.records[LoadedVersion].ifcMaterials;

                if (ifcMaterials.materials.Count > 0)
                {
                    IfcAttributeItem newItem = new IfcAttributeItem("Materials", "", false, true);
                    newItem.ItemId = itemID;
                    itemID++;
                    IfcAttributeItems.Add(newItem);
                    index = 0;
                    newItem.AttributeItemObject = meshObject;

                    foreach (string name in ifcMaterials.materials)
                    {
                        IfcAttributeItem newAttribute = new IfcAttributeItem(name, ifcMaterials.Find(name), false, false);
                        newAttribute.ItemId = itemID;
                        newAttribute.ListIndex = index;
                        newItem.Childs.Add(newAttribute);
                        IfcAttributeItems.Add(newAttribute);
                        itemID++;
                        index++;
                    }
                }


                MetaBIM_IfcTypes ifcTypes = meshObject.GetComponent<BIMElement>().BimObject.records[LoadedVersion].ifcTypes;

                if (ifcTypes.types.Count > 0)
                {
                    IfcAttributeItem newItem = new IfcAttributeItem("Types", "", false, true);
                    newItem.ItemId = itemID;
                    itemID++;
                    IfcAttributeItems.Add(newItem);
                    index = 0;
                    newItem.AttributeItemObject = meshObject;

                    foreach (string name in ifcTypes.types)
                    {
                        IfcAttributeItem newAttribute = new IfcAttributeItem(name, ifcTypes.Find(name), false, false);
                        newAttribute.ItemId = itemID;
                        newAttribute.ListIndex = index;
                        newItem.Childs.Add(newAttribute);
                        IfcAttributeItems.Add(newAttribute);
                        itemID++;
                        index++;
                    }
                }



                // Display validation result
                // TODO, finsh this section
                MetaBIM_IfcValidation ifcValidation = meshObject.GetComponent<BIMElement>().BimObject.records[LoadedVersion].IfcValidation;

                if (ifcValidation.result.Count > 0)
                {
                    // header
                    IfcAttributeItem newItem = new IfcAttributeItem("Validation", "", false, true);
                    newItem.ItemId = itemID;
                    itemID++;
                    IfcAttributeItems.Add(newItem);
                    index = 0;
                    newItem.AttributeItemObject = meshObject;
                    //newItem.Type = IfcAttributeItem.AttributeType.validation;


                    int prIndex = 0;
                    foreach (string result in ifcValidation.result)
                    {
                        CodeRule rule = CodeCheckingController.GetRule(ifcValidation.ruleID[prIndex]);

                        // Result
                        IfcAttributeItem newRuleChecking = new IfcAttributeItem("Code Rule: " + ifcValidation.ruleID[prIndex], result, false, false);
                        newRuleChecking.ItemId = itemID;
                        newRuleChecking.ListIndex = index;
                        newRuleChecking.Type = IfcAttributeItem.AttributeType.validation;
                        newItem.Childs.Add(newRuleChecking);
                        IfcAttributeItems.Add(newRuleChecking);
                        itemID++;
                        index++;

                        // Detail

                        IfcAttributeItem newRuleCheckingDetail = new IfcAttributeItem(rule.GetCodeConditionTypeString(), ifcValidation.value[prIndex], false, false);
                        newRuleCheckingDetail.ItemId = itemID;
                        newRuleCheckingDetail.ListIndex = index;
                        //newRuleCheckingDetail.Type = IfcAttributeItem.AttributeType.validation;
                        newItem.Childs.Add(newRuleCheckingDetail);
                        IfcAttributeItems.Add(newRuleCheckingDetail);
                        itemID++;
                        index++;

                        IfcAttributeItem newRuleCheckingValue = new IfcAttributeItem("Value", rule.GetTargetValue(), false, false);
                        newRuleCheckingValue.ItemId = itemID;
                        newRuleCheckingValue.ListIndex = index;
                        //newRuleCheckingValue.Type = IfcAttributeItem.AttributeType.validation;
                        newItem.Childs.Add(newRuleCheckingValue);
                        IfcAttributeItems.Add(newRuleCheckingValue);
                        itemID++;
                        index++;

                        prIndex++;
                    }
                }




                MetaBIM_IfcEpicClass ifcEpicClass = meshObject.GetComponent<BIMElement>().BimObject.records[LoadedVersion].ifcEpicClass;

                if (ifcEpicClass.attributes.Count >= 0)
                {
                    IfcAttributeItem newItem = new IfcAttributeItem("Sustainability", "", false, true);
                    newItem.ItemId = itemID;
                    itemID++;
                    IfcAttributeItems.Add(newItem);
                    index = 0;
                    newItem.Type = IfcAttributeItem.AttributeType.sustainability;
                    newItem.AttributeItemObject = meshObject;

                    foreach (string name in ifcEpicClass.attributes)
                    {
                        IfcAttributeItem newAttribute = new IfcAttributeItem(name, ifcEpicClass.Find(name), false, false);
                        newAttribute.ListIndex = index;
                        newAttribute.ItemId = itemID;
                        newItem.Childs.Add(newAttribute);
                        IfcAttributeItems.Add(newAttribute);
                        itemID++;
                        index++;
                    }
                }


            }

            IfcAttributesAdapter.SetItems(IfcAttributeItems);
            StartCoroutine(DisplayAttributesAgain());
        }


        //Set AttributeView with multiple objects 
        public void SetAttributesView(List<GameObject> meshObjects)
        {

            if (meshObjects == null || meshObjects.Count == 0)
            {
                SetAttributeView(null);
                return;
            }

            if (meshObjects.Count == 1)
            {
                SetAttributeView(meshObjects[0]);
                return;
            }

            debug(meshObjects.Count + " objects selected");

            Panel_Attributes.OnPanelOpen();

            IfcAttributeItems.Clear();
            //IfcAttributesAdapter.SetItems(IfcAttributeItems);

            /*
            if (!Panel_Detail_IFC.UI_Block.activeSelf)
            {
                return;
            }
            */

            int index = 0;
            int itemID = 0;

            //SelectedObjects = meshObjects;  // why?

            if (meshObjects != null)
            {
                Text_AttributePanel_Header.text = StringBuffer.AttributeViewer_Header_Prefix.S + meshObjects.Count + StringBuffer.AttributeViewer_Header_Suffix.S;

                AttributesRecord = CompareAttributes(meshObjects);

                // Setup IfcAttributes
                if (AttributesRecord != null)
                {
                    MetaBIM_IfcUniclass ifcUniclass = AttributesRecord.ifcUniclass;

                    if (ifcUniclass.attributes.Count > 0)
                    {
                        IfcAttributeItem newItem = new IfcAttributeItem("Uniclass", "", false, true);
                        newItem.ItemId = itemID;
                        itemID++;
                        IfcAttributeItems.Add(newItem);
                        index = 0;
                        newItem.Type = IfcAttributeItem.AttributeType.uniclass;
                        //newItem.AttributeItemObject = meshObject;

                        foreach (string name in ifcUniclass.attributes)
                        {
                            if (ifcUniclass.Find(name) != "" && ifcUniclass.Find(name) != Config.None)
                            {
                                IfcAttributeItem newAttribute = new IfcAttributeItem(name, ifcUniclass.Find(name), false, false);
                                newAttribute.ListIndex = index;
                                newAttribute.ItemId = itemID;
                                newItem.Childs.Add(newAttribute);
                                IfcAttributeItems.Add(newAttribute);
                                itemID++;
                                index++;
                            }


                        }
                    }

                    MetaBIM_IfcUniclassMap ifcUniclassMap = AttributesRecord.ifcUniclassMap;

                    if (ifcUniclassMap.attributes.Count > 0)
                    {
                        IfcAttributeItem newItem = new IfcAttributeItem("Reference", "", false, true);
                        newItem.ItemId = itemID;
                        itemID++;
                        IfcAttributeItems.Add(newItem);
                        //newItem.AttributeItemObject = meshObject;
                        index = 0;

                        foreach (string name in ifcUniclassMap.attributes)
                        {
                            if (ifcUniclassMap.Find(name) != "" && ifcUniclassMap.Find(name) != Config.None)
                            {
                                IfcAttributeItem newAttribute = new IfcAttributeItem(name, ifcUniclassMap.Find(name), false, false);
                                newAttribute.ListIndex = index;
                                newAttribute.ItemId = itemID;
                                newItem.Childs.Add(newAttribute);
                                IfcAttributeItems.Add(newAttribute);
                                itemID++;
                                index++;
                            }

                        }
                    }

                    MetaBIM_IfcParameter ifcParameter = AttributesRecord.ifcParameter;

                    if (ifcParameter.attributes.Count > 0)
                    {
                        IfcAttributeItem newItem = new IfcAttributeItem("IFC Parameter", "", false, true);
                        newItem.ItemId = itemID;
                        itemID++;
                        IfcAttributeItems.Add(newItem);
                        index = 0;
                        newItem.Type = IfcAttributeItem.AttributeType.ifcParameter;
                        //newItem.AttributeItemObject = meshObject;

                        foreach (string name in ifcParameter.attributes)
                        {

                            IfcAttributeItem newAttribute = new IfcAttributeItem(name, ifcParameter.Find(name), false, false);
                            newAttribute.ListIndex = index;
                            newAttribute.ItemId = itemID;
                            newItem.Childs.Add(newAttribute);
                            IfcAttributeItems.Add(newAttribute);
                            itemID++;
                            index++;

                        }
                    }

                    MetaBIM_IfcZone ifcZone = AttributesRecord.ifcZone;

                    if (ifcZone.attributes.Count > 0)
                    {
                        IfcAttributeItem newItem = new IfcAttributeItem("Zone", "", false, true);
                        newItem.ItemId = itemID;
                        itemID++;
                        IfcAttributeItems.Add(newItem);
                        index = 0;
                        newItem.Type = IfcAttributeItem.AttributeType.zone;
                        //newItem.AttributeItemObject = meshObject;

                        foreach (string name in ifcZone.attributes)
                        {
                            IfcAttributeItem newAttribute = new IfcAttributeItem(name, ifcZone.Find(name), false, false);
                            newAttribute.ListIndex = index;
                            newAttribute.ItemId = itemID;
                            newItem.Childs.Add(newAttribute);
                            IfcAttributeItems.Add(newAttribute);
                            itemID++;
                            index++;
                        }
                    }


                    MetaBIM_IfcValidation ifcValidation = AttributesRecord.IfcValidation;

                    if (ifcValidation.result.Count > 0)
                    {
                        IfcAttributeItem newItem = new IfcAttributeItem("Validation", "", false, true);
                        newItem.ItemId = itemID;
                        itemID++;
                        IfcAttributeItems.Add(newItem);
                        index = 0;
                        newItem.Type = IfcAttributeItem.AttributeType.zone;
                        //newItem.AttributeItemObject = meshObject;

                        foreach (string name in ifcZone.attributes)
                        {

                            IfcAttributeItem newAttribute = new IfcAttributeItem(name, ifcZone.Find(name), false, false);
                            newAttribute.ListIndex = index;
                            newAttribute.ItemId = itemID;
                            newItem.Childs.Add(newAttribute);
                            IfcAttributeItems.Add(newAttribute);
                            itemID++;
                            index++;

                        }
                    }


                    /*
                     No Epic Class
                     to DO ADD
                     */
                }
            }


            IfcAttributesAdapter.SetItems(IfcAttributeItems);
            StartCoroutine(DisplayAttributesAgain());

        }


        // render the list twice to make the list work for the first time
        public IEnumerator DisplayAttributesAgain()
        {
            yield return new WaitForEndOfFrame();
            IfcAttributesAdapter.SetItems(IfcAttributeItems);
        }



        public void OnClick_ExpendAttributeTree(GameObject _gameObject)
        {
            Debug.Log("OnClick_ExpendAttributeTree:");
            _gameObject.GetComponent<UIBlock_BimViewer_IfcAttributeItem>().OnClick_Collapse();
            IfcAttributeItem item = _gameObject.GetComponent<UIBlock_BimViewer_IfcAttributeItem>().Item;
            item.IsCollapsed = false;

            int itemIndex = IfcAttributeItems.IndexOf(item);

            int index = 0;

            foreach (var node in item.Childs)
            {
                IfcAttributeItems.Insert(itemIndex + index + 1, node);
                index++;
            }

            IfcAttributesAdapter.SetItems(IfcAttributeItems);
        }



        public void OnClick_CollapseAttributeTree(GameObject _gameObject)
        {
            Debug.Log("OnClick_CollapseAttributeTree:" + _gameObject.name);
            _gameObject.GetComponent<UIBlock_BimViewer_IfcAttributeItem>().OnClick_Expend();
            IfcAttributeItem item = _gameObject.GetComponent<UIBlock_BimViewer_IfcAttributeItem>().Item;
            item.IsCollapsed = true;

            foreach (var node in item.Childs)
            {
                IfcAttributeItems.Remove(node);
            }


            IfcAttributesAdapter.SetItems(IfcAttributeItems);

        }


        //Editing Attribute item
        public void OnClick_AttributeItem(Page_ClassificationSelector.ClassificationModeType _type, GameObject _element)
        {
            Page_ClassificationSelector.Instance.OpenView(_type, ProjectModelHandler.Instance.SelectedElements);
        }



        public BimObjectRecord CompareAttributes(List<GameObject> _objects)
        {
            string result = "-";

            BimObjectRecord record = new BimObjectRecord();
            record.ifcParameter = new MetaBIM_IfcParameter();
            record.ifcParameter.Setup();
            record.ifcUniclass = new MetaBIM_IfcUniclass();
            record.ifcUniclass.Setup();
            record.ifcUniclassMap = new MetaBIM_IfcUniclassMap();
            record.ifcUniclassMap.Setup();
            record.ifcZone = new MetaBIM_IfcZone();
            record.ifcZone.Setup();

            BimObjectRecord _record;

            int i = 0;
            foreach (GameObject ob in _objects)
            {
                _record = ob.GetComponent<BIMElement>().BimObject.records[0];

                if (_record != null)
                {
                    if (i == 0)
                    {
                        foreach (var item in _record.ifcParameter.attributes)
                        {
                            record.ifcParameter.SetValue(item, _record.ifcParameter.Find(item));
                        }

                        foreach (var item in _record.ifcUniclass.attributes)
                        {
                            record.ifcUniclass.SetValue(item, _record.ifcUniclass.Find(item));
                        }

                        foreach (var item in _record.ifcUniclassMap.attributes)
                        {
                            record.ifcUniclassMap.SetValue(item, _record.ifcUniclassMap.Find(item));
                        }

                        foreach (var item in _record.ifcZone.attributes)
                        {
                            record.ifcZone.SetValue(item, _record.ifcZone.Find(item));
                        }
                    }
                    else
                    {
                        foreach (var item in _record.ifcParameter.attributes)
                        {
                            if (_record.ifcParameter.Find(item) != record.ifcParameter.Find(item))
                            {
                                record.ifcParameter.SetValue(item, "-");
                            }
                        }

                        foreach (var item in _record.ifcUniclass.attributes)
                        {
                            if (_record.ifcUniclass.Find(item) != record.ifcUniclass.Find(item))
                            {
                                record.ifcUniclass.SetValue(item, "-");
                            }
                        }


                        foreach (var item in _record.ifcUniclassMap.attributes)
                        {
                            if (_record.ifcUniclassMap.Find(item) != record.ifcUniclassMap.Find(item))
                            {
                                record.ifcUniclassMap.SetValue(item, "-");
                            }
                        }

                        foreach (var item in _record.ifcZone.attributes)
                        {
                            if (_record.ifcZone.Find(item) != record.ifcZone.Find(item))
                            {
                                record.ifcZone.SetValue(item, "-");
                            }
                        }
                    }
                }


                i++;
            }

            return record;
        }



        [Header("Transform Viewer")]
        public UIBlock_BimViewer_TransformItem TransformViewer;



        public void SetTransformViewer(GameObject gameObject)
        {
            TransformViewer.SetBlock(gameObject.transform);
        }


        // WIP
        public void OnClick_ExpendTreeItem(GameObject _gameObject)
        {
            UIBlock_BimViewer_IfcStructureItem block = _gameObject.GetComponent<UIBlock_BimViewer_IfcStructureItem>();
            block.Item.IsCollapsed = false;
            ProcessTreeView();

            IfcStructureItemsAdapter.ForceUpdateVisibleItems();
        }


        // WIP
        public void OnClick_CollapseTreeItem(GameObject _gameObject)
        {
            UIBlock_BimViewer_IfcStructureItem block = _gameObject.GetComponent<UIBlock_BimViewer_IfcStructureItem>();
            block.Item.IsCollapsed = true;
            ProcessTreeView();

            IfcStructureItemsAdapter.ForceUpdateVisibleItems();
        }

        public void OnClick_HideTreeItem(UIBlock_BimViewer_IfcStructureItem _gameObject)
        {
            Debug.Log("OnClick_HideTreeItem: " + _gameObject.Text_Content.text);

            if (_gameObject.Item.IsHided)
            {
                _gameObject.Item.UnHideObject();
            }
            else
            {
                _gameObject.Item.OnHideObject();
            }


            //IfcStructureItemsAdapter.ForceUpdateVisibleItems();
        }



        public void OnValueChange_SearchBIMObject(string _value)
        {
            string content = _value;

            if (content != "")
            {
                ProjectConfiguration.Instance.IsDisplaySearchResult = true;
                SearchHandler.Instance.CurrentSearchSet.filterContent  = content;       
                
            }
            else
            {
                OnClick_SearchBIMObjectClear();
            }



            Tab_TreeView.SetTab();

            // ?? do we need this?
            SelectMeshObjects(SearchHandler.Instance.SearchResults);

        }


        public void OnClick_SearchBIMObjectClear()
        {
            ProjectConfiguration.Instance.IsDisplaySearchResult = false;
            SearchHandler.Instance.CurrentSearchSet.filterContent = "";
            SearchHandler.Instance.SearchResults.Clear();

        }


        public void OnSelectAllSearchResultElemets()
        {

        }



        [Header("Zone (OLD)")]
        [Header("=======================================")]


        public List<GameObject> DebugPoints;
        public List<Vector3> TargetObjectVertics;






        #region Object Element Editing (Spliting)



        public void OnClick_OnObjectSpliting()
        {
            // for now, you can only select on element for editing
            if (ProjectModelHandler.Instance.SelectedElements.Count != 1)
            {
                MCPopup.Instance.SetInformation("Please select one element for editing");

            }
            else
            {
                BIMElement targetElement = ProjectModelHandler.Instance.SelectedElements[0].GetComponent<BIMElement>();



                if (targetElement == null)
                {
                    MCPopup.Instance.SetInformation("Selected object is not a BIM element!");
                }
                else
                {
                    if (targetElement.IsSubElement)
                    {
                        targetElement = targetElement.transform.parent.GetComponent<BIMElement>();
                    }

                    if (!targetElement.IsIsolated && !targetElement.IsIsolated)
                    {
                        CloseRightClickMenu();
                        OnIsolate3DViewerObject();
                        ObjectSplitHandler.Instance.OpenPanel(targetElement, CurrentProject);
                    }
                    else
                    {
                        MCPopup.Instance.SetInformation("Selected object is not in condition (hiding or isolated) to edit!");
                    }
                }
            }
        }





        public void OnClick_OnObjectSplitingComplete(ElementSplit _splitelement)
        {

            // disable Isolation
            OnClick_CancelIsolation();

            // TODO: 
            // select the previous seleceted element???


            // Update the tree view


            // update the element to database 

        }


        public void OnSplitElementUpdate_Callback()
        {

        }


        #endregion






        #region CodeChecking

        [Header("Code Checking")]
        [Header("=======================================")]

        public CodeCheckingController CodeCheckingController;

        public void OnOpenCodeCheckingWidget()
        {
            Panel_CodeCheckingWidget.OnPanelOpen();
        }


        public void OnCloseCodeCheckingWidget()
        {
            Panel_CodeCheckingWidget.OnPanelClose();
        }



        public void OnLoadCodeChecking()
        {

        }



        public void OnClick_StartCodeValidating()
        {
#if (UNITY_WEBGL && !UNITY_EDITOR)
             MCPopup.Instance.SetInformation("Code validation is not avaiable in web plaform, please use PC platform for this operation!", "Zoning");
#else
            MCPopup.Instance.SetConfirm(OnStartCodeValidatin_Confirm, "", StringBuffer.Validation_StartValiatingConfirm.S);
#endif
        }

        public void OnStartCodeValidatin_Confirm(bool _result, string _message)
        {
            if (_result)
            {
                Page_ProcessingLog.Instance.OnProcess_Start(OnProcessPause, OnProcessStop, ProjectModelHandler.Instance.CurrentModel.GetModelObjects().Count);
                ProcessedIndex = 0;
                StartCoroutine(ProcessingCodeValidation(OnStartCodeValidatin_Complet));
            }
        }


        public IEnumerator ProcessingCodeValidation(Action _complete)
        {    


            yield return new WaitForEndOfFrame();

            foreach (GameObject go in ProjectModelHandler.Instance.CurrentModel.GetModelObjects())
            {
         
                BIMElement element = go.GetComponent<BIMElement>();

                if (element.BimObject.records.Count == 0)
                {
                    Page_ProcessingLog.Instance.OnProcess("Connecting element, skip...", ProcessedIndex);
                    ProcessedIndex++;
                    continue;
                }

                string objectKey = element.BimObject.records[element.BimObject.versionID].ifcParameter.Find("Export to IFC As");
                string message = "Validating  " + element.name + " (" + ProcessedIndex + "/" + ProjectModelHandler.Instance.CurrentModel.GetModelObjects().Count + ")";

                Page_ProcessingLog.Instance.OnProcess(message, ProcessedIndex);
                CodeCheckingController.CheckObject(go);
                ProcessedIndex++;
                if (ProcessedIndex % ProjectConfiguration.Instance.MAX_IENUMERATOR_RATE == 0)
                {
                    yield return new WaitForEndOfFrame();
                }
      
            }

            _complete();
        }


        public void OnStartCodeValidatin_Complet()
        {
            LoadingHandler.Instance.OnFullPageLoadingEnd();
            MCPopup.Instance.SetComplete(StringBuffer.Validation_CompleteProcessingWarning.S);
            Tab_TreeView.SetTab("View Code Checking");
        }


        public void OnProcessPause(string _message)
        {

        }

        public void OnProcessStop(string _message)
        {

        }

        public void OnClick_PasueCodeValidating()
        {
#if (UNITY_WEBGL && !UNITY_EDITOR)
        MCPopup.Instance.SetInformation("Code validation is not avaiable in web plaform, please use PC platform for this operation!", "Zoning");
#else
            //
#endif
        }



        public void OnClick_ValidateClass(string _className)
        {

        }


        public void OnClick_ZoneSetupOnAttributes(int _codeID)
        {
            CodeRule filterRule = CodeCheckingController.GetCodeRuleByID(_codeID);

            if (filterRule != null)
            {

            }
            else
            {
                MCPopup.Instance.SetWarning("No code found!", "Warning");
            }
        }



        #endregion


        #region Sustainability

        //TODO refactor this mehtod
        public void OnClick_SustainabilitySetupOnAttributes(GameObject _gameObject)
        {
            BIMElement element = _gameObject.GetComponent<BIMElement>();
            if (element == null)
            {
                MCPopup.Instance.SetWarning(StringBuffer.Zone_ObjectNotFound.S);
                return;
            }


        }



        #endregion


        #region Model Data Operation

        public void OnClick_SyncModelData()
        {
            /*
             * 1. pop out ask for save mode (1, save to current, 2, save to new version)
             * 
             */
            MCPopup.Instance.SetConfirm(OnSyncModelData_Confirm, "", "Sync all modified data?", "Sync Data");
        }

        public void OnSyncModelData_Confirm(bool _result, string _message)
        {
            if (_result)
            {
                // TODO

                /*
                1. Collection update information, include zone, bimelement
                2. update to current versionUpdate
                3. send to database API for sync
                 */

                MCPopup.Instance.SetComplete("No change need to be synced!");
            }

        }

        public void OnSyncModelData_Callback(bool _result, string _message)
        {
            if (_result)
            {
                /* refresh update version data */
            }
            else
            {

            }
        }

        // Export CSV

        public void OnClick_ExportModel()
        {
            if (ProjectModelHandler.Instance.CurrentModel == null)
            {
                MCPopup.Instance.SetWarning("No model is available to export.", "Export CSV");
            }
            else
            {
                DataTransferHandler.Instance.Panel_Main.OnPanelOpen();
            }

        }


        public void OnClick_ExportModelToCSV()
        {
            if (ProjectModelHandler.Instance.CurrentModel == null)
            {
                MCPopup.Instance.SetWarning("No model data is select to export.", "Export CSV");
            }
            else
            {
                string name = ProjectModelHandler.Instance.CurrentModel.Project.versions[0].originalFileName;
                MCPopup.Instance.SetConfirm(OnExportModelToCSV_Confirm, name, "Export Model " + name + " to csv?", "Export CSV");
            }

#if (UNITY_WEBGL && !UNITY_EDITOR)

#else

#endif

        }



        public void OnExportModelToCSV_Confirm(bool _result, string _message)
        {
            if (_result)
            {
                // start loading 
                var extensionList = new[] {
                    new ExtensionFilter("Text", "csv"),
                };
                var path = StandaloneFileBrowser.SaveFilePanel("Export to CSV", "", _message, extensionList);

                if (path != "")
                {
                    DataTransferHandler.Instance.StartExport(path, DataTransferHandler.DataExportFormat.csv);
                }


            }

        }



        public void OnClick_ExportModelToJSON()
        {
            if (ProjectModelHandler.Instance.CurrentModel == null)
            {
                MCPopup.Instance.SetWarning("No model data is select to export.", "Export JSON");
            }
            else
            {
                string name = ProjectModelHandler.Instance.CurrentModel.Project.versions[0].originalFileName;
                MCPopup.Instance.SetConfirm(OnExportModelToJson_Confirm, name, "Export Model " + name + " to json?", "Export JSON");
            }

#if (UNITY_WEBGL && !UNITY_EDITOR)

#else

#endif

        }


        public void OnExportModelToJson_Confirm(bool _result, string _message)
        {
            if (_result)
            {
                // start loading 
                var extensionList = new[] {
                    new ExtensionFilter("Text", "json"),
                };
                var path = StandaloneFileBrowser.SaveFilePanel("Export to JSON", "", _message, extensionList);

                if (path != "")
                {
                    DataTransferHandler.Instance.StartExport(path, DataTransferHandler.DataExportFormat.json);
                }


            }

        }



        public void OnOpenExportPath(bool _result, string _path)
        {
            Debug.Log("OnOpenExportCSVPath:" + _path);

            if (_result)
            {
                Application.OpenURL(Path.GetDirectoryName(_path));
            }
        }



        public void OnClick_OpenModelInformation()
        {
            MCPopup.Instance.SetInformation("Model Information is under development.");
        }


        #endregion



        /* View Mode */

        public Material OverlayMaterial;

        public void OnClick_MeshOverlayOn()
        {

        }

        public void OnClick_MeshOverlayOff()
        {

        }


        public void OnClick_NodeOverlayOn()
        {

        }

        public void OnClick_NodeOverlayoOff()
        {

        }


        public void OnClick_OpenClassification()
        {
            Panel_ClassificationSelector.OnPanelOpen();
        }





        /* Utility */
        #region Utility


        public void ClearObjectList(List<GameObject> _list, bool isBlockDestroy = false)
        {
            if (_list.Count > 0)
            {
                foreach (GameObject item in _list)
                {
                    if (!isBlockDestroy)
                    {
                        //Destroy(item);
                    }
                    else
                    {
                        // TODO

                    }

                    Destroy(item);
                }
            }

            _list.Clear();
        }


        // Indictor

        public void debug(string _message, float _time = 1f)
        {
            ActionInfoPanel.alpha = 1f;
            Text_ActionInfo.text = _message;
            LeanTween.cancelAll();
            CancelInvoke();
            Invoke("OnMessageFade", _time);
        }

        public void OnMessageFade()
        {
            LeanTween.alphaCanvas(ActionInfoPanel, 0, 2f).setOnComplete(() =>
            {
                Text_ActionInfo.text = "";
            });
        }


        /* Scroll Viewer */

        public int GetItemCount()
        {
            return IfcStructureItems.Count;
        }



        public GameObject FindGameObjectByIfcID(string _ifcID)
        {
            for (int i = 0; i < ProjectModelHandler.Instance.CurrentModel.GetModelObjects().Count; i++)
            {
                BIMElement element = ProjectModelHandler.Instance.CurrentModel.GetModelObjects()[i].GetComponent<BIMElement>();

                if (element != null)
                {
                    if (element.BimObject.elementID == _ifcID)
                    {
                        //return ProjectModelHandler.Instance.CurrentModel.GetModelObjects()[i];
                    }
                }

            }

            return null;
        }



        public void OnFocusBound(Bounds _bounds, float marginPercentage = 1f)
        {
            Camera camera = NavCam.MainCamera;

            float maxExtent = _bounds.extents.magnitude;
            float minDistance = (maxExtent * marginPercentage) / Mathf.Sin(Mathf.Deg2Rad * camera.fieldOfView / 2f);
            Vector3 newPos = _bounds.center - Vector3.forward * minDistance;

            // Set camera Position
            camera.transform.position = _bounds.center - Vector3.forward * minDistance;
            camera.transform.LookAt(_bounds.center);
        }





        public void OnFocusTransform(Transform _target, Camera _cam)
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


        #endregion


        #region Camera View

        private int ViewOrider = 0;
        private Vector3 newCamPos;
        private Vector3 newTargetPos;

        public void FitView(float _distance = 1.0f, float _leanTime = 0.5f)
        {
            LeanTween.cancel(this.gameObject);

            Bounds bounds = RightClickSelectedObject.GetComponent<MeshRenderer>().bounds;

            float cameraDistance = _distance; // Constant factor

            Vector3 objectSizes = bounds.max - bounds.min;
            float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
            float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * NavCam.MainCamera.fieldOfView); // Visible height 1 meter in front
            float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
            distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object

            Vector3 newposition = bounds.center - distance * NavCam.MainCamera.transform.forward;

            LeanTween.move(NavCam.MainCamera.gameObject, newposition, _leanTime);
            // need rotation?

        }

        public void FitView(GameObject target, float _distance = 1.0f, float _leanTime = 0.5f)
        {
            LeanTween.cancel(this.gameObject);

            Bounds bounds = target.GetComponent<MeshRenderer>().bounds;
            NavCam.SetTargetPoint(bounds.center);
            float cameraDistance = _distance; // Constant factor

            Vector3 objectSizes = bounds.max - bounds.min;
            float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
            float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * NavCam.MainCamera.fieldOfView); // Visible height 1 meter in front
            float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
            distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object

            Vector3 newposition = bounds.center - distance * NavCam.MainCamera.transform.forward;

            LeanTween.move(NavCam.MainCamera.gameObject, newposition, _leanTime);
            // need rotation?

        }

        public void OnInitFitView(Bounds _bounds, float _distance = 1.0f, float _leanTime = 1f)
        {
            LeanTween.cancel(this.gameObject);

            Bounds bounds = _bounds;
            NavCam.SetTargetPoint(_bounds.center);
            float cameraDistance = _distance;

            Vector3 objectSizes = bounds.max - bounds.min;
            float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
            float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * NavCam.MainCamera.fieldOfView); // Visible height 1 meter in front
            float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
            distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object

            Vector3 newposition = bounds.center - distance * NavCam.MainCamera.transform.forward;

            LeanTween.move(NavCam.MainCamera.gameObject, newposition, _leanTime);
        }



        public void OnInitFitView_Top(Bounds _bounds, float _distance = 1.0f, float _leanTime = 1f)
        {
            LeanTween.cancel(this.gameObject);

            Bounds bounds = _bounds;
            NavCam.SetTargetPoint(_bounds.center);
            float cameraDistance = _distance;
            ViewOrider = 0;

            Vector3 objectSizes = bounds.max - bounds.min;
            float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
            float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * NavCam.MainCamera.fieldOfView); // Visible height 1 meter in front
            float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
            distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object

            newCamPos = new Vector3(bounds.center.x, bounds.center.y + distance, bounds.center.z); ;
            newTargetPos = bounds.center;

            LeanTween.move(NavCam.MainCamera.gameObject, newCamPos, _leanTime).setOnUpdate(update_cam_lean);
        }


        public void OnInitFitView_TurnLeft(Bounds _bounds, float _distance = 1.0f, float _leanTime = 1f)
        {
            LeanTween.cancelAll();

            Bounds bounds = _bounds;
            NavCam.SetTargetPoint(_bounds.center);
            float cameraDistance = _distance;



            Vector3 objectSizes = bounds.max - bounds.min;
            float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
            float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * NavCam.MainCamera.fieldOfView); // Visible height 1 meter in front
            float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
            distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object

            newCamPos = getViewPosition(bounds.center, distance, ModelOffset.y);
            newTargetPos = bounds.center;

            LeanTween.move(NavCam.MainCamera.gameObject, newCamPos, _leanTime).setOnUpdate(update_cam_lean); ;

            ViewOrider++;

            if (ViewOrider > 3)
            {
                ViewOrider = 0;
            }
        }


        public void OnInitFitView_TurnRight(Bounds _bounds, float _distance = 1.0f, float _leanTime = 1f)
        {
            LeanTween.cancelAll();

            Bounds bounds = _bounds;
            NavCam.SetTargetPoint(_bounds.center);
            float cameraDistance = _distance;



            Vector3 objectSizes = bounds.max - bounds.min;
            float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
            float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * NavCam.MainCamera.fieldOfView); // Visible height 1 meter in front
            float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
            distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object

            newCamPos = getViewPosition(bounds.center, distance, ModelOffset.y);
            newTargetPos = bounds.center;

            LeanTween.move(NavCam.MainCamera.gameObject, newCamPos, _leanTime).setOnUpdate(update_cam_lean); ;

            ViewOrider--;

            if (ViewOrider < 0)
            {
                ViewOrider = 3;
            }
        }

        private void update_cam_lean(float update)
        {
            NavCam.transform.LookAt(newTargetPos);
        }


        private Vector3 getViewPosition(Vector3 _target, float _distance, float _rotateOffset = 0)
        {

            Vector3 CamPos = new Vector3();

            if (ViewOrider == 0)   // top down
            {
                CamPos = new Vector3(_target.x, _target.y, _target.z - _distance);
            }
            else
            if (ViewOrider == 1)  // side left
            {
                CamPos = new Vector3(_target.x - _distance, _target.y, _target.z);
            }
            else
            if (ViewOrider == 2) // side right
            {
                CamPos = new Vector3(_target.x, _target.y, _target.z + _distance);
            }
            else
            if (ViewOrider == 3) // side right
            {
                CamPos = new Vector3(_target.x + _distance, _target.y, _target.z);
            }

            return CamPos;

        }


        #endregion



        public void HideObject(GameObject _item)
        {
            if (_item.GetComponent<MeshRenderer>() != null)
            {
                _item.GetComponent<MeshRenderer>().enabled = false;
            }
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

        // Process Element data after spliting

        public void UpdateIfcId(BimObject _bimObject, string _newIfcName)
        {
            _bimObject.elementID = _newIfcName;
            _bimObject.records[_bimObject.versionID].ifcAttribute.SetValue("id", _newIfcName);
            _bimObject.records[_bimObject.versionID].ifcParameter.SetValue("IFC Guid", _newIfcName);
            // _bimObject.records[_bimObject.versionID].ifcProperties.SetValue("id", _newIfcName);
            // Any more ?

        }

        public void FixSplitedMaterial(GameObject _item)
        {

            // fix material
            // TODO ,
            Material[] mats = _item.GetComponent<Renderer>().materials;

            if (mats.Length > 1)
            {
                List<Material> newMaterial = mats.ToList();
                newMaterial.RemoveAt(mats.Length - 1);
                _item.GetComponent<Renderer>().materials = newMaterial.ToArray();
            }

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



    }


}



