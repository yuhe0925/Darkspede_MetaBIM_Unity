using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



[Serializable]
public class IfcAttributeItem 
{
    public string AttributeKey;
    public string AttributeValue;
    public bool IsHeader;
    public bool IsCollapsed;
    public int ListIndex;
    public int ItemId;
    public int checkType = -1;

    public AttributeType Type = AttributeType.general;

    public GameObject AttributeItemObject;
    public List<IfcAttributeItem> Childs;


    public IfcAttributeItem(string _key, string _value, bool _isCollapsed, bool _isHeader, int _checkType = 0)
    {
        AttributeKey = _key;
        AttributeValue = _value;
        IsCollapsed = _isCollapsed;
        IsHeader = _isHeader;

        Childs = new List<IfcAttributeItem>();
        checkType = _checkType;
    }


    public enum AttributeType
    {
        general,
        ifcParameter,
        uniclass,
        zone,
        validation,
        coderule,
        sustainability,
    }
}
