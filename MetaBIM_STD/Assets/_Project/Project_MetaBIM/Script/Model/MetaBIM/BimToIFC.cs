using IfcToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static MetaBIM_IfcParameter;
using static UnityEngine.UI.CanvasScaler;

[Serializable]
public class MetaBIM_IfcFile
{
    public string ifcFileName = "";  // Written in IfcImporter.RuntimeImport() and IfcProcessor.OnPostprocessAllAssets()
    public string ifcFilePath = ""; // Written in IfcImporter.RuntimeImport() and IfcProcessor.OnPostprocessAllAssets()

    public string ifc_version = "";
    public string ifcVersionPostfix = "";
    public string geometryFileFormat = "";
    public string geometricRepresentationContextReference =   "";
    public string geometricRepresentationSubContextReference =   "";


    public MetaBIM_IfcFile FromObject(IfcFile _item)
    {
        // copy data over from orignal class
        ifcFileName = _item.ifcFileName;
        ifcFilePath = _item.ifcFilePath;

        ifc_version = _item.ifc_version;
        ifcVersionPostfix = _item.ifcVersionPostfix;
        geometryFileFormat = _item.geometryFileFormat;
        geometricRepresentationContextReference = _item.geometricRepresentationContextReference;
        geometricRepresentationSubContextReference = _item.geometricRepresentationSubContextReference;

        return this;
    }

    public static IfcFile ToObject(MetaBIM_IfcFile _item)
    {
        IfcFile obj = new IfcFile();
        obj.ifcFileName = _item.ifcFileName;
        obj.ifcFilePath = _item.ifcFilePath;

        obj.ifc_version = _item.ifc_version;
        obj.ifcVersionPostfix = _item.ifcVersionPostfix;
        obj.geometryFileFormat = _item.geometryFileFormat;
        obj.geometricRepresentationContextReference = _item.geometricRepresentationContextReference;
        obj.geometricRepresentationSubContextReference = _item.geometricRepresentationSubContextReference;

        return obj;
    }

}



[Serializable]
public class MetaBIM_IfcHeader
{
    public List<string> headers = new List<string>();
    public List<string> values = new List<string>();

    public MetaBIM_IfcHeader FromObject(IfcHeader _item)
    {
        headers = _item.headers;
        values = _item.values;
        return this;
    }

    public static IfcHeader ToObject(MetaBIM_IfcHeader _item)
    {
        IfcHeader obj = new IfcHeader();
        obj.headers = _item.headers;
        obj.values = _item.values;
        return obj;
    }

}




[Serializable]
public class MetaBIM_IfcUnits
{
    public List<string> unitType = new List<string>();
    public List<string> unitName = new List<string>();
    public List<string> si_equivalent = new List<string>();

    public MetaBIM_IfcUnits FromObject(IfcUnits _item)
    {
        unitType = _item.unitType;
        unitName = _item.unitName;
        si_equivalent = _item.si_equivalent;

        return this;
    }

    public static IfcUnits ToObject(MetaBIM_IfcUnits _item)
    {
        IfcUnits obj = new IfcUnits();
        obj.unitType = _item.unitType;
        obj.unitName = _item.unitName;
        obj.si_equivalent = _item.si_equivalent;
        return obj;
    }

}



[Serializable]
public class MetaBIM_IfcAttributes
{
    public List<string> attributes = new List<string>();
    public List<string> values = new List<string>();
    public List<int> checkedType = new List<int>();

    public MetaBIM_IfcAttributes FromObject(IfcAttributes _item)
    {
        attributes = _item.attributes;
        values = _item.values;

        return this;
    }

    public static IfcAttributes ToObject(MetaBIM_IfcAttributes _item)
    {
        IfcAttributes obj = new IfcAttributes();
        obj.attributes = _item.attributes;
        obj.values = _item.values;
        return obj;
    }

    public string Find(string name)
    {
        for (int i = 0; i < attributes.Count; i++)
        {
            if (attributes[i] == name)
            {
                return values[i];
            }
        }
        return null;
    }

    public void SetValue(string name, string value)
    {
        for (int i = 0; i < attributes.Count; i++)
        {
            if (attributes[i] == name)
            {
                values[i] = value;
            }
        }
    }


}



[Serializable]
public class MetaBIM_IfcProperties
{
    public List<string> properties = new List<string>();
    public List<string> nominalValues = new List<string>();


    public MetaBIM_IfcProperties FromObject(IfcProperties _item)
    {
        properties = _item.properties;
        nominalValues = _item.nominalValues;

        return this;
    }

    public static IfcProperties ToObject(MetaBIM_IfcProperties _item)
    {
        IfcProperties obj = new IfcProperties();
        obj.properties = _item.properties;
        obj.nominalValues = _item.nominalValues;
        return obj;
    }
    public string Find(string name)
    {
        for (int i = 0; i < properties.Count; i++)
        {
            if (properties[i] == name)
            {
                return nominalValues[i];
            }
        }
        return null;
    }


    public void SetValue(string name, string value)
    {
        for (int i = 0; i < properties.Count; i++)
        {
            if (properties[i] == name)
            {
                nominalValues[i] = value;
            }
        }
    }

}



[Serializable]
public class MetaBIM_IfcTypes
{
    public List<string> types = new List<string>();
    public List<string> values = new List<string>();

    public MetaBIM_IfcTypes FromObject(IfcTypes _item)
    {
        types = _item.types;
        values = _item.values;

        return this;
    }

    public static IfcTypes ToObject(MetaBIM_IfcTypes _item)
    {
        IfcTypes obj = new IfcTypes();
        obj.types = _item.types;
        obj.values = _item.values;
        return obj;
    }


    public string Find(string name)
    {
        for (int i = 0; i < types.Count; i++)
        {
            if (types[i] == name)
            {
                return values[i];
            }
        }
        return null;
    }

}



[Serializable]
public class MetaBIM_IfcMaterials
{

    public List<string> materials = new List<string>();
    public List<string> thicknesses = new List<string>();

    public MetaBIM_IfcMaterials FromObject(IfcMaterials _item)
    {
        materials = _item.materials;
        thicknesses = _item.thicknesses;

        return this;
    }

    public static IfcMaterials ToObject(MetaBIM_IfcMaterials _item)
    {
        IfcMaterials obj = new IfcMaterials();
        obj.materials = _item.materials;
        obj.thicknesses = _item.thicknesses;
        return obj;
    }
    public string Find(string name)
    {
        for (int i = 0; i < materials.Count; i++)
        {
            if (materials[i] == name)
            {
                return thicknesses[i];
            }
        }
        return null;
    }
}




[Serializable]
public class MetaBIM_IfcUniclass
{
    public string UniclassName;
    
    public List<string> attributes = new List<string>();
    public List<string> values = new List<string>();

    
    public string Find(string name)
    {
        for (int i = 0; i < attributes.Count; i++)
        {
            if (attributes[i] == name)
            {
                return values[i];
            }
        }
        return null;
    }
    public void SetValue(string _key, string _value)
    {
        for (int i = 0; i < attributes.Count; i++)
        {
            if (attributes[i] == _key)
            {
                values[i] = _value;
                return;
            }
        }
    }


    public MetaBIM_IfcUniclass()
    {


    }

    public void Setup()
    {
        attributes = new List<string> {
            "Code",
            "Group",
            "Sub_Group",
            "Section",
            "Object",
            "Title",
            "Table",
        };

        values = new List<string> {
            "",
            "",
            "",
            "",
            "",
            "",
            "",
        };

    }
}


[Serializable]
public class MetaBIM_IfcUniclassMap
{
    public string UniclassName;

    public List<string> attributes = new List<string>();
    public List<string> values = new List<string>();


    public string Find(string name)
    {
        for (int i = 0; i < attributes.Count; i++)
        {
            if (attributes[i] == name)
            {
                return values[i];
            }
        }
        return null;
    }
    public void SetValue(string _key, string _value)
    {
        for (int i = 0; i < attributes.Count; i++)
        {
            if (attributes[i] == _key)
            {
                values[i] = _value;
                return;
            }
        }
    }


    public MetaBIM_IfcUniclassMap()
    {

    }


    public void Setup()
    {
        attributes = new List<string> {
            "NBS_Code",
            "COBie",
            "NRM1",
            "CESMM",
            "IFC",
            "IFC_Type",
        };

        values = new List<string> {
            "",
            "",
            "",
            "",
            "",
            "",
        };
    }




}




[Serializable]
public class MetaBIM_IfcParameter
{
    public string UniclassName;

    public List<string> attributes = new List<string>();
    public List<string> values = new List<string>();


    public string Find(string name)
    {
        for (int i = 0; i < attributes.Count; i++)
        {
            if (attributes[i] == name)
            {
                return values[i];
            }
        }
        return null;
    }
    
    public void SetValue(string _key, string _value)
    {
        for (int i = 0; i < attributes.Count; i++)
        {
            if (attributes[i] == _key)
            {
                values[i] = _value;
                return;
            }
        }
    }



    public MetaBIM_IfcParameter()
    {

    }


    public void Setup()
    {
        attributes = new List<string> {
            "Export to IFC As",
            "IFC Predefined Type",
            "IFC Guid",
            "Export IFC",
        };

        values = new List<string> {
            "",
            "",
            "",
            "By Type",
        };
    }

    public enum IfcParameterType{
        ExportToIfcAs,
        IfcPredefinedType,
        IfcGuid,
        ExportIfc,
    }

    public string GetIfcParamenter(IfcParameterType _type)
    {
        switch (_type)
        {         
            case IfcParameterType.ExportToIfcAs:
                return Find("Export to IFC As");
            case IfcParameterType.IfcPredefinedType:
                return Find("IFC Predefined Type");
            case IfcParameterType.IfcGuid:
                return Find("IFC Guid");
            case IfcParameterType.ExportIfc:
                return Find("Export IFC");
            default:
                return "";
        }
    }
        
    

}





[Serializable]
public class MetaBIM_MetaBimProperty
{
    public List<string> properties = new List<string>();
    public List<string> nominalValues = new List<string>();


    public MetaBIM_MetaBimProperty FromObject(MetaBIM_MetaBimProperty _item)
    {
        properties = _item.properties;
        nominalValues = _item.nominalValues;

        return this;
    }

    public static MetaBIM_MetaBimProperty ToObject(MetaBIM_MetaBimProperty _item)
    {
        MetaBIM_MetaBimProperty obj = new MetaBIM_MetaBimProperty();
        obj.properties = _item.properties;
        obj.nominalValues = _item.nominalValues;
        return obj;
    }
    public string Find(string name)
    {
        for (int i = 0; i < properties.Count; i++)
        {
            if (properties[i] == name)
            {
                return nominalValues[i];
            }
        }
        return null;
    }


    public void SetValue(string name, string value)
    {
        bool _isFound = false;

        for (int i = 0; i < properties.Count; i++)
        {
            if (properties[i] == name)
            {
                nominalValues[i] = value;
                _isFound = true;
                break;
            }
        }

        if (!_isFound)
        {
            properties.Add(name);
            nominalValues.Add(value);
        }
    }

    

}


[Serializable]
public class MetaBIM_IfcZone
{
    public string UniclassName;

    public List<string> attributes = new List<string>();
    public List<string> values = new List<string>();


    public string Find(string name)
    {
        for (int i = 0; i < attributes.Count; i++)
        {
            if (attributes[i] == name)
            {
                return values[i];
            }
        }
        return null;
    }

    public void SetValue(string _key, string _value)
    {
        for (int i = 0; i < attributes.Count; i++)
        {
            if (attributes[i] == _key)
            {
                values[i] = _value;
                return;
            }
        }
    }



    public void SetNewValue(string _key, string _value)
    {
        attributes.Add(_key);
        values.Add(_value);
    }   



    public MetaBIM_IfcZone()
    {

    }


    public void Setup()
    {
        attributes = new List<string> {
            "Zone Identifier",
            "Zone Type",
            "Area",
            "Volume",
            "Unit",
            "Placement",
        };
        
        values = new List<string> {
            "Unassigned",
            "Construction",
            "0",
            "0",
            "m",
            "0 0 0",
        };
    }

    public enum ZoneType
    {
        Construction,
        Space,
        Room,
        Ground,
        Other,
    }

    public string GetZoneType(ZoneType _type)
    {
        switch (_type)
        {
            case ZoneType.Construction:
                return Find("Construction");
            case ZoneType.Space:
                return Find("Space");
            case ZoneType.Room:
                return Find("Room");
            case ZoneType.Ground:
                return Find("Ground");
            case ZoneType.Other:
                return Find("Other");
            default:
                return "Construction";
        }
    }



    public void SetValue(string _id, string _type, string _area, string _volume, string _unit  = "m")
    {
        values[0] = _id;
        values[1] = _type;
        values[2] = _area;
        values[3] = _volume;
        values[4] = _unit;
    }

    
    public void SetBounds(Bounds _bound)
    {
        string boundvale = _bound.center.x + " " + _bound.center.y + " " + _bound.center.z + " " + _bound.extents.x + " " + _bound.extents.y + " " + _bound.extents.z + " ";
        values[5] = boundvale;
    }
    
    
}




[Serializable]
public class MetaBIM_IfcValidation
{
    public string validation = "Unvalidated";
    
    public List<string> ruleID      = new List<string>();
    public List<string> checkedItem = new List<string>();
    public List<string> result      = new List<string>();
    public List<string> value       = new List<string>();

    public int failedCount = 0;


    public MetaBIM_IfcValidation()
    {

    }
    
    public void ClearData()
    {
        ruleID.Clear();
        checkedItem.Clear();
        result.Clear();
        value.Clear();
    }

    public string GetResultByID(string _id)
    {
        for (int i = 0; i < ruleID.Count; i++)
        {
            if (ruleID[i] == _id)
            {
                return result[i];
            }
        }

        return "";
    }
    
    public bool IsCheckedForCodeID(string _id)
    {
        foreach(var id in ruleID)
        {
            if(id == _id)
            {
                return true;
            }
        }

        return false;
    }
    
    public bool IsPassed()
    {
        
        return false;
    }

    public void SetValueItem(string _id, string _checkedItem, string _result, string _value)
    {
        if (_result == GetValidationResultType(ifcValidationResult.failed))
        {
            failedCount++;
        }
        
        ruleID.Add(_id);
        checkedItem.Add(_checkedItem);
        result.Add(_result);
        value.Add(_value);
    }


   

    public string GetValidationResultType(ifcValidationResult _type)
    {
        switch (_type)
        {
            case ifcValidationResult.passed:
                return "Passed";
            case ifcValidationResult.failed:
                return "Failed";
            case ifcValidationResult.ignored:
                return "Ignored";
            case ifcValidationResult.warning:
                return "Warning";
            default:
                return "";
        }
    }


    public enum ifcValidationResult
    {
        passed,
        failed,
        ignored,
        warning,
    }

    public enum ifcValidationFailType
    {
        missing,
        incorrectType,
        incorrectTarget,
        unknow,
    }
}


[Serializable]
public class MetaBIM_IfcEpicClass
{
    public string UniclassName;

    public List<string> attributes = new List<string>();
    public List<string> values = new List<string>();


    public string Find(string name)
    {
        for (int i = 0; i < attributes.Count; i++)
        {
            if (attributes[i] == name)
            {
                return values[i];
            }
        }
        return null;
    }

    public void SetValue(string _key, string _value)
    {
        for (int i = 0; i < attributes.Count; i++)
        {
            if (attributes[i] == _key)
            {
                values[i] = _value;
                return;
            }
        }
    }

    public void SetNewValue(string _key, string _value)
    {
        attributes.Add(_key);
        values.Add(_value);
    }



    public MetaBIM_IfcEpicClass()
    {

    }


    public void Setup()
    {
        attributes = new List<string> {
            "Category",
            "Type",
            "Material",
            "Unit",
            "Volume",
            "Energy",
            "Water",
            "Greenhouse Gas",
        };

        values = new List<string> {
            "N",
            "N",
            "N",
            "N",
            "N",
            "N",
            "N",
            "N",
        };
    }




}




[Serializable]
public class MetaBIM_IfcCustomProperty
{
    public List<string> properties = new List<string>();
    public List<string> nominalValues = new List<string>();

    public string Find(string name)
    {
        for (int i = 0; i < properties.Count; i++)
        {
            if (properties[i] == name)
            {
                return nominalValues[i];
            }
        }
        return null;
    }


    public void SetValue(string name, string value)
    {
        for (int i = 0; i < properties.Count; i++)
        {
            if (properties[i] == name)
            {
                nominalValues[i] = value;
            }
        }
    }

}

