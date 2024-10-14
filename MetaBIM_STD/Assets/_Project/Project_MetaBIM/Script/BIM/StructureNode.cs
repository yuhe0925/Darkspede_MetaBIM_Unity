using IfcToolkit;
using System;
using System.Collections.Generic;
using UnityEngine;




[Serializable]
public class StructureNode
{
    public int ItemScrollIndex = -1;
    public int ItemIndex = -1;
    /*Data*/
    public bool IsGeometry = true;
    public BimModel.StructureType structureType = BimModel.StructureType.node;


    public string itemName;  // ifc element name
    public string itemID;    // ifc element id
    public int nodeDepth;
    public int nodeID;


    public string Content;  // displayed in the tree
    public string ObjectType = "object";
    public bool IsIfcTypeGroup = false;   // a group ifc node
    public int PropertyNodesCount = 0;

    public int ColorCode = 0;   

    // Direct Child
    public BIMElement element;
    public GameObject linkedObject;
    public StructureNode parentNode;
    public int parentNodeIndex = -1;
    public List<StructureNode> childrenNodes = new List<StructureNode>();

    
    // data buffer
    public int VerticeCount;
    public int TriangleCount;
    public int MaterialCount;

    public UIBlock_BimViewer_IfcStructureItem UILinkObject;
    public bool IsCollapsed = true;
    public bool IsHided = false;

    // used for displaying search result
    // -1, not searched
    // 0, not matched
    // > 1, number of child nodes matched
    public int IsSearchMatched = -1;

    // a editable node can be renamed and removed
    public bool IsEditable = false;


    public StructureNode(StructureNode _parent = null, BIMElement _element = null)
    {
        parentNode = _parent;
        IsCollapsed = true;
  
        //ItemScrollIndex = -1;

        if (_element != null)
        {
            element = _element;

            linkedObject = _element.gameObject;
            Content = linkedObject.name;

            if (element.gameObject.GetComponent<IfcAttributes>() != null)
            {
                itemID = element.gameObject.GetComponent<IfcAttributes>().Find("id");
            }
        }
    }


    public void CopyValue(StructureNode _targetNode)
    {
        _targetNode.itemName = itemName;
        _targetNode.itemID = itemID;
        _targetNode.nodeID = nodeID;
        _targetNode.Content = Content;
        _targetNode.ObjectType = ObjectType;
        _targetNode.IsIfcTypeGroup = IsIfcTypeGroup;
        _targetNode.PropertyNodesCount = PropertyNodesCount;
        _targetNode.element = element;
        _targetNode.linkedObject = linkedObject;
        _targetNode.parentNode = parentNode;
    }




    public void OnHideObject(bool _ifHideChild = true)
    {
        HideElement(this);
    }
    
    public void HideElement(StructureNode _node, bool _ifChild = true)
    {
        if(_node.element != null)
        {
            _node.element.SetToHideMode();
        }

        _node.IsHided = true;
        
        if (_node.UILinkObject != null)
        {
            if (_node.IsHided)
            {
                _node.UILinkObject.OnHideObject();
            }
            else
            {
                _node.UILinkObject.OnUnhideObject();
            }
        }

        if (_ifChild)
        {
            if (_node.childrenNodes.Count > 0)
            {
                foreach (var item in _node.childrenNodes)
                {
                    item.HideElement(item);
                }
            }
        }
    }
   
    public void UnHideObject(bool _ifHideChild = true)
    {
        UnhideElement(this);

    }

    public void UnhideElement(StructureNode _node, bool _ifChild = true)
    {
        if (_node.element != null)
        {
            _node.element.RestoreObject();
        }
        _node.IsHided = false;
        
        if (_node.UILinkObject != null)
        {
            if (_node.IsHided)
            {
                _node.UILinkObject.OnHideObject();
            }
            else
            {
                _node.UILinkObject.OnUnhideObject();
            }
        }

        if (_ifChild)
        {
            if (_node.childrenNodes.Count > 0)
            {
                foreach (var item in _node.childrenNodes)
                {
                    item.UnhideElement(item);
                }
            }
        }
    }





}



// this class is used to test the tree structure of the ifc model
public class StructureNodeExpress
{
    public bool isDisplay = true;
    public bool isCollapsed = true;
    public bool isNodeConnectNode;

    public string nodeContent;
    public int nodeIndex;
    public int nodeDepth;

    public List<StructureNodeExpress> childrenNodes = new List<StructureNodeExpress>();

}

