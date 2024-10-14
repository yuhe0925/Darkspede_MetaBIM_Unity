using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IfcCompareNode
{
    public string NodeType = "Update";
    public string IfcId = "";
    public GameObject IfcObject;

    public IfcCompareNode(string nodeType, string ifcId, GameObject ifcObject)
    {
        NodeType = nodeType;
        IfcId = ifcId;
        IfcObject = ifcObject;
    }
}


