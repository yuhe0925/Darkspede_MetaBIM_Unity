using MetaBIM;
using netDxf.Blocks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class TreeDataHandler : MonoBehaviour
{
    public static TreeDataHandler Instance;

    public PanelChange PanelChange;

    public bool IsInCustomTreeMode = false;

    [Header("Data ")]
    public CustomStructureObject DefaultStructure;
    public List<CustomStructureObject> customStructureObjects = new List<CustomStructureObject>();
    public CustomStructureObject CurrentStructure;   // the structure currently displayed


    [Header("Element")]
    public UIController_RightClickMenu RightClickMenu;
    public CustomStructureItemsAdapter CustomStructureItemsAdapter;
    public UIController_Tab UIController_Tab;

    [Header("Operation Buffer")]
    public List<GameObject> SelectedTreeNodes = new List<GameObject>();
    public GameObject SelectedNode;
    public UIBlock_BimViewer_IfcStructureItem SelecetdStructureUINode; // for right click menu
    private List<BimModel> bimModels;

    /*Fields*/
    [Header("Status")]
    private string CurrentTabName;
    private bool IsMenuOpend = true;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;    
        }
        PanelChange.SetAction(OnPanelOpen, OnPanelClose);
    }


    public void ClosePanel()
    {
        PanelChange.OnPanelClose();
    }

    public void OpenPanel()
    {
        PanelChange.OnPanelOpen();
    }




    public void OnPanelClose()
    {

    }

    public void OnPanelOpen()
    {
        // Load Saved Custom Structure Trees from (DB)
        if (ProjectModelHandler.Instance != null)
        {
            bimModels = ProjectModelHandler.Instance.GetActiveModels();
            OnLoadTree();
        }

    }





    public void OnLoadTree()
    {
        LoadingHandler.Instance.OnFullPageLoadingStart("Loading Structures");
        StartCoroutine(DataProxy.Instance.GetCustomStructures("AttachedWorkspace", Page_Workspace.Instance.SelectedWorkspace.guid, LoadTree_Callback));
    }



    public void LoadTree_Callback(bool _result, string _message)
    {
        LoadingHandler.Instance.OnFullPageLoadingEnd();

        if (_result)
        {
            DataProxyResponse<CustomStructure> payload = JsonUtility.FromJson<DataProxyResponse<CustomStructure>>(_message);

            customStructureObjects.Clear();

            if (payload.package.Count == 0)
            {
                // create add default structure
                DefaultStructure  = new CustomStructureObject(Page_Workspace.Instance.SelectedWorkspace.guid);
                customStructureObjects.Add(DefaultStructure);
                CurrentStructure = DefaultStructure;
                AddCurrentStructure();

                // upload this new structure to the server
            }
            else
            {

                foreach (CustomStructure _structure in payload.package)
                {
                    //Debug.Log("LoadTree_Callback: for loop Structure: " + _structure.StructureName);
                    customStructureObjects.Add(ConvertFromCustomStructure(_structure));
                }

                CurrentStructure = customStructureObjects[0];
            }


            UIController_Tab.InitTab(GetStructureNames()); // => OnSelectStructureTab()
            UIController_Tab.SetTab(CurrentStructure.StructureName);
        }

    }

    /* DataProxy */
    #region DataProxy
    public void AddCurrentStructure()
    {
        StartCoroutine(DataProxy.Instance.AddCustomStructure(CustomStructure.ToJson(CurrentStructure.CustomStructure), AddStructure_Callback));
    }

    public void AddStructure_Callback(bool _result, string _message)
    {
        if (_result)
        {
            Debug.Log("Structure Added");
        }
        else
        {
            OnLoadTree();
            MCPopup.Instance.SetWarning("Failed to add the structure.");
        }
    }

    public void UpdateCurrentStructure()
    {
        StartCoroutine(DataProxy.Instance.UpdateCustomStructure(CustomStructure.ToJson(CurrentStructure.CustomStructure), UpdateCurrentStructure_Callback));
    }

    public void UpdateCurrentStructure_Callback(bool _result, string _message)
    {
        if (_result)
        {
            Debug.Log("Update Completed!");
        }
        else
        {
            OnLoadTree();
            MCPopup.Instance.SetWarning("Failed to update the structure, restore structure");
        }
    }


    public void RemoveCurrentStructure()
    {
        StartCoroutine(DataProxy.Instance.DeleteCustomStructureByGuid(CurrentStructure.CustomStructure.guid, RemoveStructure_Callback));
    }

    public void RemoveStructure_Callback(bool _result, string _message)
    {
        if (_result)
        {
            customStructureObjects.Remove(CurrentStructure);
            UIController_Tab.InitTab(GetStructureNames());
            UIController_Tab.SetTab(customStructureObjects[0].StructureName);
            MCPopup.Instance.SetInformation("Structure Deleted!");
        }
        else
        {
            OnLoadTree();
            MCPopup.Instance.SetWarning("Failed to remove the structure.");
        }
    }


    #endregion
    /* END DataProxy */


    // call back function for UIController_Tab
    // UIController_Tab.InitTab(GetStructureNames());
    public void OnSelectStructureTab(string _structureName)
    {
        Debug.Log("Select Structure Tab: " + _structureName);
        CurrentTabName = _structureName;
        // get the current structure by name
        if (GetStructureByName(_structureName) != -1)
        {
            CurrentStructure = customStructureObjects[GetStructureByName(_structureName)];
        }
        else
        {
            CurrentStructure = DefaultStructure;
        }
         
        ProcessTreeView(false);
    }

    public int TreeViewItemScrollIndex = 0;


    // this is the main function to render the tree view
    public void ProcessTreeView(bool _isOpenEmptyNode)
    {
        Debug.Log("ProcessTreeView: " + CurrentStructure.StructureName);
        CurrentStructure.StructureNodes.Clear();
        TreeViewItemScrollIndex = 0;
        processingCount = 0;

        CurrentStructure.RootNode.ItemIndex = TreeViewItemScrollIndex;
        TreeViewItemScrollIndex++;
        CurrentStructure.StructureNodes.Add(CurrentStructure.RootNode);


        RenderTreeList(CurrentStructure.RootNode, 1, _isOpenEmptyNode);

        SelecetdStructureUINode = null;

        UpdateStructureTreeView();


        
    }
    public int processingCount = 0;


    public void RenderTreeList(StructureNode node, int _Level = 0, bool _isOpenEmptyNode = false)
    {
        bool _isRender = false;

        if (!node.IsCollapsed)
        {
            _isRender = true;
    
        }
        else
        {
            if (_isOpenEmptyNode)
            {
                node.IsCollapsed = false;
                _isRender = true;
            }
        }


        if (_isRender)
        {
            foreach (StructureNode item in node.childrenNodes)
            {
                //Debug.Log("Child Node: " + item.itemName);

                processingCount++;
                if (ProjectConfiguration.Instance.IsDisplaySearchResult)
                {
                    if (item.IsSearchMatched > 0 || item.structureType == BimModel.StructureType.connect)
                    {
                        CurrentStructure.StructureNodes.Add(item);
                        item.ItemIndex = TreeViewItemScrollIndex;
                        TreeViewItemScrollIndex++;
                        RenderTreeList(item, _Level + 1, _isOpenEmptyNode);
                    }
                }
                else
                {
                    CurrentStructure.StructureNodes.Add(item);
                    item.ItemIndex = TreeViewItemScrollIndex;
                    TreeViewItemScrollIndex++;
                    RenderTreeList(item, _Level + 1, _isOpenEmptyNode);
                }
            }
        }


    }

    public void UpdateStructureTreeView()
    {
        Debug.Log("UpdateStructureTreeView: " + CurrentStructure.StructureName);
        CustomStructureItemsAdapter.SetItems(CurrentStructure.StructureNodes);
        //IfcStructureItemsAdapter.ForceUpdateVisibleItems();
    }


    public void OnClick_ExpendTreeItem(GameObject _gameObject)
    {
        UIBlock_BimViewer_IfcStructureItem block = _gameObject.GetComponent<UIBlock_BimViewer_IfcStructureItem>();
        block.Item.IsCollapsed = false;
        ProcessTreeView(false);
    }


    // WIP
    public void OnClick_CollapseTreeItem(GameObject _gameObject)
    {
        UIBlock_BimViewer_IfcStructureItem block = _gameObject.GetComponent<UIBlock_BimViewer_IfcStructureItem>();
        block.Item.IsCollapsed = true;
        ProcessTreeView(false);
    }


    public void OnClick_HideTreeItem(UIBlock_BimViewer_IfcStructureItem _gameObject)
    {
        if (_gameObject.Item.IsHided)
        {
            _gameObject.Item.UnHideObject();
        }
        else
        {
            _gameObject.Item.OnHideObject();
        }
        ProcessTreeView(false);
    }



    public void SearchNode()
    { 

    }


    public void OpenRightClickMenu_CustomStructureTree(UIBlock_BimViewer_IfcStructureItem _item)
    {
        if (_item != null)
        {
            if(SelecetdStructureUINode != null)
            {
                SelecetdStructureUINode.OnDeselect();
            }
            SelecetdStructureUINode = _item;
            SelecetdStructureUINode.OnSelect();

            OpenRightClickMenu_IFCStructure();
        }
        else
        {
            RightClickMenu.OnMenuClose();
        }


    }


    public void OpenRightClickMenu_IFCStructure()
    {
        Debug.Log("OpenRightClickMenu_IFCStructure: " + SelecetdStructureUINode.name);
        //MenuRect.position = Input.mousePosition;
        RightClickMenu.OnMenuOpen(UIController_RightClickMenu.MenuType.customstructure);
        IsMenuOpend = true;
    }




    /* UI Action */

    public void OnClick_CreateNewTree()
    {
        MCPopup.Instance.SetInput(CreateNewTree_Confirm, "Name this new element structure.", "New Structure");
    }

    public void CreateNewTree_Confirm(bool _result, string _message)
    {
        if (_result)
        {
            if(_message == "" )
            {
                MCPopup.Instance.SetWarning("Name of structure can not be empty.");
                return;
            }
            else if (GetStructureByName(_message) != -1)
            {
                MCPopup.Instance.SetWarning("The name " + _message + " is already exists.");
                return;

            }

            CustomStructureObject _newStructure = new CustomStructureObject(Page_Workspace.Instance.SelectedWorkspace.guid, _message);
            CurrentStructure = _newStructure;
            AddCurrentStructure();

            customStructureObjects.Add(CurrentStructure);
            UIController_Tab.InitTab(GetStructureNames());
            UIController_Tab.SetTab(CurrentStructure.StructureName);
        }
    }

    public void OnClick_RenameTree()
    {
        // get current sttructure name
        var _currentStructure = CurrentStructure.StructureName;

        MCPopup.Instance.SetInput(RenameTree_Confirm, "Name this structure.", _currentStructure);
    }


    public void RenameTree_Confirm(bool _result, string _message)
    {
        if (_result)
        {
            if (_message == "")
            {
                MCPopup.Instance.SetWarning("Name of structure can not be empty.");
                return;
            }

            if (GetStructureByName(_message) != -1)
            {
                MCPopup.Instance.SetWarning("The name "+_message+" is already exists.");
                return;

            }

            CurrentStructure.StructureName = _message;
            CurrentStructure.RootNode.itemName = _message;
            CurrentStructure.RootNode.Content = _message;
            CurrentStructure.CustomStructure.StructureName = _message;
            CurrentStructure.CustomStructure.root.itemName = _message;
            // TODO update the strucuture
            UpdateCurrentStructure();

            UIController_Tab.InitTab(GetStructureNames());
            UIController_Tab.SetTab(CurrentStructure.StructureName);

        }
    }


    /// <summary>
    /// Remove current tree from the list
    /// </summary>
    public void OnClick_RemoveTree()
    {
        // get current sttructure name
        var _currentStructure = CurrentStructure.StructureName;
        if(customStructureObjects.Count < 2)
        {
            MCPopup.Instance.SetWarning("Can not remove the last structure.");
            return;
        }

        MCPopup.Instance.SetConfirm(RemoveTree_Confirm, _currentStructure, "Are you sure to remove "+ _currentStructure + "?");

    }

    public void RemoveTree_Confirm(bool _result, string _message)
    {
        if (_result)
        {
            RemoveCurrentStructure();
        }
    }


    /* Node Operation*/

    public void OnClick_RenameNode()
    {

        if(SelecetdStructureUINode == null)
        {
            MCPopup.Instance.SetWarning("Please select a node first.");
            return;
        }

       

        MCPopup.Instance.SetInput(RenameNode_InputConfirm, "Name this new element structure.");
    }

    public void RenameNode_InputConfirm(bool _result, string _message)
    {
        if (_result)
        {
            if (_message == "")
            {
                MCPopup.Instance.SetWarning("Name can not be empty.");
                return;
            }

            var item = SelecetdStructureUINode.Item;
            item.itemName = _message;
            item.Content = _message;

            // also update the custom structure node
            var customNode = CurrentStructure.CustomStructure.GetNodeByID(item.itemID);
            if(customNode == null)
            {
                return;
            }
            customNode.itemName = _message;


            // update the structure
            UpdateCurrentStructure();
            // update the tree view
            ProcessTreeView(false);
        }
    }


    public void OnClick_RemoveNode()
    {
        if (SelecetdStructureUINode == null)
        {
            MCPopup.Instance.SetWarning("Please select a node first.");
        }


        if (SelecetdStructureUINode.Item.structureType != BimModel.StructureType.connect)
        {
            MCPopup.Instance.SetWarning("Selected node is not a structure node");
            return;
        }

        if (SelecetdStructureUINode.Item.childrenNodes.Count > 0)
        {
            MCPopup.Instance.SetConfirm(OnRemoveComfirm_Callback, SelecetdStructureUINode.Item.itemName, "Removing this node will also remove its subcontent. Do you wish to continue?");
        }
        else
        {
            OnRemoveComfirm_Callback(true,"");
        }


    }


    public void OnRemoveComfirm_Callback(bool _result, string _message)
    {
        if (_result)
        {
            // get parent node
            var parent = SelecetdStructureUINode.Item.parentNode;
 
            if(parent == null)
            {
                Debug.Log("No Parent Node Found: " + SelecetdStructureUINode.Item.itemName);
                return;
            }

            parent.childrenNodes.Remove(SelecetdStructureUINode.Item);


            // get custom structure node from structure
            var customParentNode = CurrentStructure.CustomStructure.GetNodeByID(parent.itemID);

            if (customParentNode == null)
            {
                Debug.Log("No Parent Custom Node Found: " + parent.itemID);
                return;
            }

            var customNode = CurrentStructure.CustomStructure.GetNodeByID(SelecetdStructureUINode.Item.itemID);
            if (customNode == null)
            {
                Debug.Log("No Custom Node Found: " + SelecetdStructureUINode.Item.itemID);
                return;
            }


            // remove from custom structure node 
            // TODO check if server delete is OK
            customParentNode.childrenNodes.Remove(customNode);

            // update the structure
            UpdateCurrentStructure();
            // update the tree view
            ProcessTreeView(false);

        }
    }


    public void OnClick_AddNode()
    {
        if (SelecetdStructureUINode == null)
        {
            MCPopup.Instance.SetWarning("Please select a node first.");
        }


        MCPopup.Instance.SetInput(AddNode_InputConfirm, "Name this node.", SelecetdStructureUINode.Item.itemName);
    }


    public void AddNode_InputConfirm(bool _result, string _message)
    {
        if (_result)
        {
            if (_message == "")
            {
                MCPopup.Instance.SetWarning("Name can not be empty.");
                return;
            }

            // create a new structure node
            StructureNode newNode = GetNewNode(_message, SelecetdStructureUINode.Item.nodeDepth + 1);

            Debug.Log("GetNodeByID: " + newNode.itemID);

            // get custom structure node from structure
            var customNode = CurrentStructure.CustomStructure.GetNodeByID(SelecetdStructureUINode.Item.itemID);

            if(customNode != null)
            {
                // update 
                var newCustomNode = ConvertStructureNodeToCustomStructureNode(newNode);
                newNode.itemID = newCustomNode.nodeID;
                newNode.IsCollapsed = false;
                newNode.parentNode = SelecetdStructureUINode.Item;
                customNode.childrenNodes.Add(newCustomNode);

                SelecetdStructureUINode.Item.childrenNodes.Add(newNode);

                UpdateCurrentStructure();

                UIController_Tab.InitTab(GetStructureNames());
                UIController_Tab.SetTab(CurrentStructure.StructureName);
            }
            else 
            {
                MCPopup.Instance.SetWarning("Can not find data of selected element");
            }


            SelecetdStructureUINode = null;

        }
    }


    public void OnClick_ResetTree()
    {

    }


    public void OnClick_SelectTreeNode(UIBlock_BimViewer_IfcStructureItem __UIObject)
    {
        Debug.Log("OnClick_SelectTreeNode: " + __UIObject.name);

        if(SelecetdStructureUINode != null)
        {
            SelecetdStructureUINode.OnDeselect();

            if (SelecetdStructureUINode == __UIObject)
            {
                SelecetdStructureUINode = null;
            }
            else
            {
                SelecetdStructureUINode = __UIObject;
                SelecetdStructureUINode.OnSelect();

                Page_BIMViewer.Instance.OnSelectTreeItem(SelecetdStructureUINode.gameObject);

                //if (SelecetdStructureUINode.Item.structureType == BimModel.StructureType.node)
                //{
                //    Page_BIMViewer.Instance.SelectMeshObject(SelecetdStructureUINode.Item.linkedObject, false, true);

                //}
            }
    
        }
        else
        {
            SelecetdStructureUINode = __UIObject;
            SelecetdStructureUINode.OnSelect();

            Page_BIMViewer.Instance.OnSelectTreeItem(SelecetdStructureUINode.gameObject);

        }




    }


    public void OnClick_LoadSavedTree()
    {

    }


    public void OnClick_InsertSelectedElementToNode()
    {
        if (SelecetdStructureUINode == null)
        {
            MCPopup.Instance.SetWarning("Please select a structure node first!");
            return;
        }


        var selectedItem = ProjectModelHandler.Instance.SelectedElements;

        if(selectedItem.Count == 0)
        {
            MCPopup.Instance.SetWarning("No element is selected!");
            return;
        }


        var structure = SelecetdStructureUINode.Item;

        if(structure.structureType != BimModel.StructureType.connect)
        {
            MCPopup.Instance.SetWarning("Please select a structure node to insert!");
            return;
        }

        bool isUpdated = false;
        
        // for each selected element
        foreach(var item in selectedItem)
        {
            var element = item.GetComponent<BIMElement>();

            if(element == null)
            {
                continue;
            }
            

            bool isExist = false;

            // find if the element is already exist in the node
            foreach(var n in structure.childrenNodes)
            {
                if(n.itemID == element.LinkedNodeItem.itemID)
                {
                    isExist = true;
                }
            }

            // if not exist, add it
            if (!isExist)
            {
                isUpdated = true;
                // duplicate the element node
                var newNode = new StructureNode();

                element.LinkedNodeItem.CopyValue(newNode);
                newNode.parentNode = structure;
                newNode.nodeDepth = structure.nodeDepth + 1;
                newNode.structureType = BimModel.StructureType.node;

                structure.childrenNodes.Add(newNode);
                structure.IsCollapsed = false; // open it

                //insert id into custom structure node
                var customNode = CurrentStructure.CustomStructure.GetNodeByID(structure.itemID);

                var newCustomNode = ConvertStructureNodeToCustomStructureNode(newNode);
                newCustomNode.nodeType = 1;
                customNode.childrenNodes.Add(newCustomNode);

            }
            else
            {
                Debug.Log("Element is already exist in the node.");
            }
       

        }


        if (isUpdated)
        {
            UpdateCurrentStructure();
            ProcessTreeView(false);
        }
 
    }


    public void OnClick_RemoveSelectedElementFromNode()
    {
        if (SelecetdStructureUINode == null)
        {
            MCPopup.Instance.SetWarning("Please select a element node first!");
            return;
        }

        if(SelecetdStructureUINode.Item.structureType != BimModel.StructureType.node)
        {
            MCPopup.Instance.SetWarning("Selected node is not element");
            return;
        }

        // remove the element from structure node 
        var result = CurrentStructure.OnRemoveElementNode(SelecetdStructureUINode.Item.itemID);

        // remove the element 
        if (result)
        {
            var _result = CurrentStructure.CustomStructure.OnRemoveItemNode(SelecetdStructureUINode.Item.itemID);

            if (_result)
            {
                UpdateCurrentStructure();
                ProcessTreeView(false);
            }
            else
            {
                MCPopup.Instance.SetWarning("Failed to remove the element.");
            }

        }
        else
        {
            MCPopup.Instance.SetWarning("Failed to locate the element.");
        }


    }

    /* Data Action */


    public void OnValueChange_Search(string _value)
    {

    }

    public int GetStructureByName(string _name)
    {
        foreach (CustomStructureObject _structure in customStructureObjects)
        {
            if (_structure.StructureName == _name)
            {
                return customStructureObjects.IndexOf(_structure);
            }
        }

        return -1;
    }   


    public StructureNode GetNewNode(string _nodeName, int _depth)
    {
        StructureNode _newNode = new StructureNode();
        _newNode.structureType = BimModel.StructureType.connect;
        _newNode.itemName = _nodeName;
        _newNode.nodeDepth = _depth;
        _newNode.Content = _nodeName;
        return _newNode;

    }


    public List<string> GetStructureNames()
    {
        List<string> _names = new List<string>();

        foreach (CustomStructureObject _structure in customStructureObjects)
        {
            _names.Add(_structure.StructureName);
        }

        return _names;
    }



    public CustomStructureObject ConvertFromCustomStructure(CustomStructure _structure)
    {
        // create a new structure object from a custom structure node
        CustomStructureObject _newStructure = new CustomStructureObject(Page_Workspace.Instance.SelectedWorkspace.guid);
        _newStructure.CustomStructure = _structure;
        _newStructure.StructureName = _structure.StructureName;
        _newStructure.IsNodeMapped = false;

        // mapping the root node
        _newStructure.RootNode = new StructureNode();
        _newStructure.RootNode.itemName = _structure.root.itemName;
        _newStructure.RootNode.Content = _structure.root.itemName;
        _newStructure.RootNode.nodeDepth = 0; // the root
        _newStructure.RootNode.structureType = BimModel.StructureType.connect;
        _newStructure.RootNode.itemID = _structure.root.nodeID;
        _newStructure.RootNode.IsCollapsed = false;
        // process all the children nodes

 


        foreach (var node in _structure.root.childrenNodes)
        {
            OnConvertFromCustomStructure(_newStructure.RootNode, node);

         
        }
     

        return _newStructure;
    }


    public void OnConvertFromCustomStructure(StructureNode _parentNode, CustomStructureNode _node)
    {
        //Debug.Log("ConvertFromCustomStructure: " + _node.itemName);

        // the 
        StructureNode structure = new StructureNode();
      
        // check for node that is element
        if (_node.nodeType == 1)
        {
            foreach (var bim in bimModels)
            {
                var snode = bim.GetStructureNodeByItemID(_node.itemID);
                if (snode != null)
                {
                    snode.CopyValue(structure);
                    structure.parentNode = _parentNode;
                    structure.nodeDepth = _parentNode.nodeDepth + 1;
                    structure.structureType = BimModel.StructureType.node;
                }
            }
        }
        else
        {  
            // create a new structure object from a custom structure node
            structure.itemID = _node.nodeID;
            structure.itemName = _node.itemName;
            structure.nodeDepth = _node.nodeDepth;
            structure.Content = _node.itemName;
            structure.parentNode = _parentNode;
            structure.structureType = BimModel.StructureType.connect;

        }

        if (_node.childrenNodes.Count > 0 )  // this is because the node can not have linked elements if it has children node
        {
            structure.IsCollapsed = false;
            // contine for the children
            foreach (CustomStructureNode item in _node.childrenNodes)
            {
                OnConvertFromCustomStructure(structure, item);
            }
        }
        else
        {
            structure.IsCollapsed = true;
            // map elements
            foreach (LinkedElement item in _node.elements)
            {
                var node = GetElementStructurnNodeByID(item.ElementID);
                //node.parentNode = _parentNode;
                if (node != null)
                {
                    _parentNode.childrenNodes.Add(node);
                }
            }
        }

        _parentNode.childrenNodes.Add(structure);
    }


    public CustomStructureNode ConvertStructureNodeToCustomStructureNode(StructureNode _node)
    {
        CustomStructureNode customStructureNode = new CustomStructureNode(_node.itemName, _node.nodeDepth);
        customStructureNode.itemID = _node.itemID;
        return customStructureNode;
    }




    public StructureNode GetElementStructurnNodeByID(string _id)
    {

        foreach (var bim in bimModels)
        {
            foreach (var node in bim.Structures)
            {
                if(node.itemID == _id)
                {
                    return node;
                }
            }
        }

        return null;
    }

}



[Serializable]
public class CustomStructureObject
{
    public string StructureName;

    public StructureNode RootNode = new StructureNode();
    public CustomStructure CustomStructure;

    // nodes that display in the tree view
    public List<StructureNode> StructureNodes = new List<StructureNode>();

    public bool IsNodeMapped = false;

    public CustomStructureObject(string _attachedGuid, string _name = "Set")
    {
        StructureName = _name;
        IsNodeMapped = false;
        StructureNodes = new List<StructureNode>(); 

        RootNode.structureType = BimModel.StructureType.connect;
        RootNode.itemName = _name;
        RootNode.Content = _name;
        CustomStructure = new CustomStructure(_name, _attachedGuid);
        CustomStructure.root = new CustomStructureNode(_name, 0);
        RootNode.itemID = CustomStructure.root.nodeID;

    }



    public bool OnRemoveElementNode(string _itemID)
    {
        bool result = DeleteElementNodeByItemID(RootNode, _itemID);
        return result;
    }



    public bool DeleteElementNodeByItemID(StructureNode currentNode, string _itemID)
    {
        Debug.Log("DeleteNodeByID: " + _itemID);
        // Traverse through the children of the current node
        for (int i = 0; i < currentNode.childrenNodes.Count; i++)
        {
            var child = currentNode.childrenNodes[i];

            // If the child has the matching NodeID, remove it from the list
            if (child.itemID == _itemID)
            {
                currentNode.childrenNodes.RemoveAt(i);
                return true; // Node successfully removed
            }

            // Recursively try to delete in the subtree
            if (DeleteElementNodeByItemID(child, _itemID))
            {
                return true; // Node deleted in the subtree
            }
        }

        // If the node was not found in this subtree, return false
        return false;
    }




    public bool RemoveElementFromStructureNode(StructureNode _node)
    {
        var resultNode = FindStructureNodeInSubtree(RootNode, _node);
        if(resultNode != null)
        {
            resultNode.childrenNodes.Remove(_node);
            return true;
        }

        return false;
    }


    public static StructureNode FindStructureNodeInSubtree(StructureNode currentNode, StructureNode targetNode)
    {
        // If current node is the target node, return current node
        if (currentNode.nodeID == targetNode.nodeID)
        {
            return currentNode;
        }

        // Traverse through the children of the current node
        foreach (var child in currentNode.childrenNodes)
        {
            // Recursively check if the target node belongs to any of the subtrees
            var foundNode = FindStructureNodeInSubtree(child, targetNode);
            if (foundNode != null)
            {
                return foundNode; // Return the node that contains the target
            }
        }

        // If not found in any subtree, return null
        return null;
    }
}

