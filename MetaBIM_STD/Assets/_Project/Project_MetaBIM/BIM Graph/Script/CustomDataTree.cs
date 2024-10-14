using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomDataTree : MonoBehaviour
{
    public static CustomDataTree Instance;
    


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    public StructureNode rootNode = new StructureNode();

    public MetaBIM_IfcCustomProperty metaBIM_IfcCustomProperty;

    public void AddNode(StructureNode _targetNode, StructureNode _newNode)
    {
        _newNode.nodeDepth = _targetNode.nodeDepth + 1;
        _newNode.parentNode = _targetNode;
        _targetNode.childrenNodes.Add(_newNode);
    }

    public void RemoveNode(StructureNode _targetNode)
    {
        _targetNode.parentNode.childrenNodes.Remove(_targetNode);
    }


}
