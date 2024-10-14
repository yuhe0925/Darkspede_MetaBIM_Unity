using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using UnityEngine;
using static MetaBIM.CodeChecking.CodeRule;

/// <summary>
/// Summary description for Sycamore_Document
/// </summary>
namespace MetaBIM.CodeChecking
{
    [Serializable]
    public class CodeRule : IModel
    {
        public string codeSourceText = "";
        public string codeDescription = "";
        
        public CheckingTarget checkingTarget = CheckingTarget.element;
        public CodeCategory codeCategory = CodeCategory.attribute;
        public CodeConditionType codeCondition = CodeConditionType.equal;

        public string checkingClass = "";
        public string checkingKey = "";
        public string checkingValue = "";
        
        public List<string> rangeSection;   // 0-100-200


        //buffer
        public List<GameObject> CheckedObject;

          
        public CodeRule()
        {
            rangeSection = new List<string>();
        }
        public string GetTargetValue()
        {
            switch (codeCondition)
            {
                case CodeConditionType.has:
                    return "check key existance";
                case CodeConditionType.equal:
                    return "= " + checkingValue;
                case CodeConditionType.great:
                    return "> " + checkingValue;
                case CodeConditionType.less:
                    return "< " + checkingValue;
                case CodeConditionType.within:
                    return rangeSection[0] + " - "+ rangeSection[1];
                case CodeConditionType.selection:
                    string collection = "";
                    foreach(var item in rangeSection)
                    {
                        collection = collection + item + " | ";
                    }
                    return collection;
                default:
                    return "General Type";
            }
        }
        public string GetCodeConditionTypeString()
        {
            return CodeConditionTypeString(codeCondition);
        }

        public string CodeConditionTypeString(CodeConditionType _type)
        {
            switch (_type)
            {
                case CodeConditionType.has:
                    return "Has";
                case CodeConditionType.equal:
                    return "Equal to";
                case CodeConditionType.great:
                    return "Grent than";
                case CodeConditionType.less:
                    return "Less than";
                case CodeConditionType.within:
                    return "Contain";
                case CodeConditionType.selection:
                    return "In Collection";
                default:
                    return "General Type";
            }
        }

        public enum CodeConditionType
        {
            has = 0,        // target has key
            equal = 1,      // target key have value that is equal to checking value
            great = 2,      // target key have value that is greater than checking value
            less = 3,       // target key have value that is less than checking value
            within = 4,     // target key have value that is between rangeSection (0, count-1)
            selection = 5,  // target key have value that is one of the item in rangeSection
        }

        public string GetCodeCategoryString()
        {
            return CodeCategoryString(codeCategory);
        }
        public string CodeCategoryString(CodeCategory _type)
        {
            switch (_type)
            {
                case CodeCategory.content:
                    return "Content";
                case CodeCategory.geometry:
                    return "Geometry";
                case CodeCategory.attribute:
                    return "Attribute";
                case CodeCategory.material:
                    return "Material";
                case CodeCategory.bounds:
                    return "Bounds";

                default:
                    return "General";
            }
        }

        public enum CodeCategory
        {
            content,    // general content = text
            geometry,
            attribute,
            material,
            bounds
        }

        public enum CodeSource
        {
            ifc,
            dwg,
            bim
        }

        public enum CheckingTarget
        {
            file,
            element,
            plan,
        }

        #region JSON
        public static string ToJson(CodeRule _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static CodeRule FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<CodeRule>(_json);
        }

        public static List<CodeRule> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<CodeRule>>(_json);
        }
        #endregion
    }



    public static class CodeCondition
    {
        public static string eq = "equal";
    }


}