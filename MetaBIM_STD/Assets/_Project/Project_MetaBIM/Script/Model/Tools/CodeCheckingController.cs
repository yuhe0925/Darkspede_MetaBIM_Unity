using MetaBIM;
using MetaBIM.CodeChecking;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;


public class CodeCheckingController : MonoBehaviour
{
    public static CodeCheckingController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        Panel.OnOpenAction = OnOpenAction;
        Panel.OnCloseAction = OnCloseAction;
    }
    public void OnOpenAction()
    {
 
    }


    public void OnCloseAction()
    {
        //CodeRuleAttributeItems.Clear();
        //CodeCheckingAttributeAdapter.SetItems(CodeRuleAttributeItems);
    }


    public List<CodeLibrary> CodeLaibraries;
    public List<CodeRule> CodeRules;

    [Header("Panel")]
    public PanelChange Panel;

    [Header("UI element")]

    public CodeCheckingAttributeAdapter CodeCheckingAttributeAdapter;
    public List<IfcAttributeItem> CodeRuleAttributeItems = new List<IfcAttributeItem>();

    [Header("Buffer")]
    public CodeRule SelectedCodeRule;
    public GameObject SelectedCodeRuleObject;


    public bool IsAmendElement = false;

    public void OnOpen()
    {
        Panel.OnPanelOpen();

    }
    
    public void OnClose()
    {
        Panel.OnPanelClose();
  
    }


    public void OnDisable()
    {
        //OnClosePanel();
        SelectedCodeRuleObject = null;
    }


    
    public void OnValueChange_LibrarySelect(int _value)
    {
        if (_value == 1)  //hardcode result
        {
            SetRuleAttributeView();
        }
    }



    public void SetRuleAttributeView()
    {

        CodeRuleAttributeItems.Clear();
        CodeCheckingAttributeAdapter.SetItems(CodeRuleAttributeItems);

        int index = 0;
        int itemID = 0;

        //Text_AttributePanel_Header.text = StringBuffer.AttributeViewer_Header_Prefix.S;


        Dictionary<string, List<CodeRule>> ruleTypes = new Dictionary<string, List<CodeRule>>();


        for(int i = 0; i < CodeRules.Count; i++)
        {
            CodeRule rule = CodeRules[i];




            string key = rule.checkingClass;
            

            if (!ruleTypes.ContainsKey(key))
            {
                // Create new Pair
                ruleTypes.Add(key, new List<CodeRule>() { rule });
            }
            else
            {
                // create connect object
                ruleTypes[key].Add(rule);
            }

        }



        foreach (KeyValuePair<string, List<CodeRule>> pair in ruleTypes)
        {
            IfcAttributeItem headerItem = new IfcAttributeItem(pair.Key, "", true, true);
            headerItem.ListIndex = 0;
            CodeRuleAttributeItems.Add(headerItem);
            index++;
            
            foreach (var rule in pair.Value)
            {
                /*
                IfcAttributeItem rultAttribute_id =         new IfcAttributeItem(Ru,         rule.guid, true, false);
                if(index % 2 != 0)
                {
                    index++;
                }
                rultAttribute_id.ListIndex = index;
                index++;
                IfcAttributeItem rultAttribute_target =     new IfcAttributeItem("Target",          rule.GetCodeCategoryString(), true, false);
                rultAttribute_target.ListIndex = index;
                //index++;
                IfcAttributeItem rultAttribute_condition =  new IfcAttributeItem("Condition",       rule.GetCodeConditionTypeString(), true, false);
                rultAttribute_condition.ListIndex = index;
                //index++;
                IfcAttributeItem rultAttribute_key =        new IfcAttributeItem(rule.checkingKey,  rule.GetTargetValue(), true, false);
                rultAttribute_key.ListIndex = index;
                //index++;
                
                headerItem.Childs.Add(rultAttribute_id);
                headerItem.Childs.Add(rultAttribute_target);
                headerItem.Childs.Add(rultAttribute_condition);
                headerItem.Childs.Add(rultAttribute_key);
                */



                string checkvalue = rule.GetTargetValue();

                IfcAttributeItem rultAttribute_id = new IfcAttributeItem(rule.checkingKey, checkvalue, true, false);
                rultAttribute_id.ListIndex = 1;
                rultAttribute_id.ItemId = int.Parse(rule.guid);
                headerItem.Childs.Add(rultAttribute_id);

            }


        }

        CodeCheckingAttributeAdapter.SetItems(CodeRuleAttributeItems);

        StartCoroutine(DisplayAgain());
    }


    public IEnumerator DisplayAgain()
    {
        yield return new WaitForEndOfFrame();
        CodeCheckingAttributeAdapter.SetItems(CodeRuleAttributeItems);
    }

    public CodeRule GetCodeRuleByID(int _id)
    {
        foreach(var item in CodeRules)
        {
            if(int.Parse(item.guid) == _id)
            {
                return item;
            }
        }

        return null;
    }

    public void OnClick_ExpendRuleAttributeTree(GameObject _gameObject)
    {
        Debug.Log("OnClick_ExpendRuleAttributeTree:");
        
        IfcAttributeItem item = _gameObject.GetComponent<UIBlock_BimViewer_CodeItem>().Item;
        item.IsCollapsed = false;

        int itemIndex = CodeRuleAttributeItems.IndexOf(item);

        int index = 0;

        foreach (var node in item.Childs)
        {
            CodeRuleAttributeItems.Insert(itemIndex + index + 1, node);
            index++;
        }

        CodeCheckingAttributeAdapter.SetItems(CodeRuleAttributeItems);
    }

    public void OnClick_CollapseRuleAttributeTree(GameObject _gameObject)
    {
        Debug.Log("OnClick_CollapseRuleAttributeTree:");
        IfcAttributeItem item = _gameObject.GetComponent<UIBlock_BimViewer_CodeItem>().Item;
        item.IsCollapsed = true;

        foreach (var node in item.Childs)
        {
            CodeRuleAttributeItems.Remove(node);
        }


        CodeCheckingAttributeAdapter.SetItems(CodeRuleAttributeItems);

    }




    public void OnClick_ClearSelectCodeItem()
    {
        SelectedCodeRule = new CodeRule();
        SelectedCodeRule.guid = "-1";
        Page_BIMViewer.Instance.Tab_TreeView.SetTab("View Code Checking");

        if (SelectedCodeRuleObject != null)
        {
            SelectedCodeRuleObject.GetComponent<UIBlock_BimViewer_CodeItem>().OnDeselected();
        }

        SelectedCodeRuleObject = null;
    }


    public void OnClick_CheckCodeResult(GameObject _gameObject)
    {
        if(SelectedCodeRuleObject != null)
        {
            SelectedCodeRuleObject.GetComponent<UIBlock_BimViewer_CodeItem>().OnDeselected();
        }
        SelectedCodeRuleObject = _gameObject;
        
        UIBlock_BimViewer_CodeItem item = SelectedCodeRuleObject.GetComponent<UIBlock_BimViewer_CodeItem>();
        item.OnSelected();
        
        SelectedCodeRule = GetCodeRuleByID(item.Item.ItemId);
        Page_BIMViewer.Instance.Tab_TreeView.SetTab("View Code Checking");
    }





    public CodeRule GetRule(string _id)
    {
        foreach(CodeRule rule in CodeRules)
        {
            if(rule.guid == _id)
            {
                return rule;
            }
        }
        
        return null;
    }

   
    public void ClearCheckedResult()
    {
        foreach (CodeRule rule in CodeRules)
        {
            rule.CheckedObject.Clear();
        }
    }

    
    public void CheckObject(GameObject _object)
    {
        BIMElement element = _object.GetComponent<BIMElement>();
        BimObjectRecord record = element.GetCurrentRecord();

        MetaBIM_IfcValidation validation = record.IfcValidation;

        int failCount = 0;
        int passCount = 0;
        

        if (element == null)
        {
            Debug.Log("CheckObject: no object");
        }
        
        string elementIFCType = element.GetCurrentRecord().ifcParameter.Find("Export to IFC As");

        validation.ClearData();
        MetaBIM_IfcAttributes ifcAttributes = element.BimObject.records[0].ifcAttribute;

        // reset checked type for validation result
        ifcAttributes.checkedType = new List<int>();

        foreach(var item in ifcAttributes.attributes)
        {
            ifcAttributes.checkedType.Add(0);  //set default value to 0;
        }




        foreach (CodeRule rule in CodeRules)
        {  
            // not the class we want to check
            if (elementIFCType.ToLower() != rule.checkingClass.ToLower())
            {
                //Debug.Log("Pass Class " + rule.checkingClass);
                continue;
            }


            if(Check_IfValidated(record, rule.guid))
            {
                continue;
            }

            //Debug.Log("Checking " + elementIFCType);

            rule.CheckedObject.Add(_object);
            string key = rule.checkingKey;
            string value = "Default";
            int type = 0;

            if (Check_HasAttribute(record, rule.checkingKey))
            {
                validation.SetValueItem(rule.guid, rule.checkingClass, "passed", rule.checkingKey);
                type = 0;
   
                switch (rule.codeCondition)
                {

                    case CodeRule.CodeConditionType.has:
                        if (Check_HasAttribute(record, rule.checkingKey))
                        {
                            validation.SetValueItem(rule.guid, rule.checkingClass, "passed", rule.checkingKey);
                            type = 0;
                            passCount++;
                        }
                        else
                        {
                            validation.SetValueItem(rule.guid, rule.checkingClass, "failed", rule.checkingKey);
                            type = 1;
                            failCount++;
                        }
                        break;
                    case CodeRule.CodeConditionType.within:
                        if (Check_WithinValue(record, rule.checkingKey, rule.rangeSection))
                        {
                            validation.SetValueItem(rule.guid, rule.checkingClass, "passed", rule.checkingKey);
                            type = 0;
                            passCount++;
                        }
                        else
                        {
                            validation.SetValueItem(rule.guid, rule.checkingClass, "failed", rule.checkingKey);
                            type = 1;
                            failCount++;
                        }
                        break;
                    case CodeRule.CodeConditionType.equal:
                        if (Check_EqualValue(record, rule.checkingKey, rule.checkingValue))
                        {
                            validation.SetValueItem(rule.guid, rule.checkingClass, "passed", rule.checkingKey);
                            type = 0;
                            passCount++;
                        }
                        else
                        {
                            validation.SetValueItem(rule.guid, rule.checkingClass, "failed", rule.checkingKey);
                            type = 1;
                            failCount++;
                        }
                        break;
                    case CodeRule.CodeConditionType.great:
                        if (Check_GreaterThanValue(record, rule.checkingKey, rule.checkingValue))
                        {
                            validation.SetValueItem(rule.guid, rule.checkingClass, "passed", rule.checkingKey);
                            type = 0;
                            passCount++;
                        }
                        else
                        {
                            validation.SetValueItem(rule.guid, rule.checkingClass, "failed", rule.checkingKey);
                            type = 1;
                            failCount++;
                        }
                        break;
                    case CodeRule.CodeConditionType.less:
                        if (Check_LessThanValue(record, rule.checkingKey, rule.checkingValue))
                        {
                            validation.SetValueItem(rule.guid, rule.checkingClass, "passed", rule.checkingKey);
                            type = 0;
                            passCount++;
                        }
                        else
                        {
                            validation.SetValueItem(rule.guid, rule.checkingClass, "failed", rule.checkingKey);
                            type = 1;
                            failCount++;
                        }
                        break;
                    case CodeRule.CodeConditionType.selection:
                        if (Check_InSelection(record, rule.checkingKey, rule.rangeSection))
                        {
                            validation.SetValueItem(rule.guid, rule.checkingClass, "passed", rule.checkingKey);
                            type = 0;
                            passCount++;
                        }
                        else
                        {
                            validation.SetValueItem(rule.guid, rule.checkingClass, "failed", rule.checkingKey);
                            type = 1;
                            failCount++;
                        }
                        break;
                    default:
                        break;


                }
            }
            else
            {
                validation.SetValueItem(rule.guid, rule.checkingClass, "failed", rule.checkingKey);
                type = 1;
                failCount++;


                switch (rule.codeCondition)
                {
                    case CodeRule.CodeConditionType.within:
                        value = rule.rangeSection[rule.rangeSection.Count - 1];
                        break;
                    case CodeRule.CodeConditionType.equal:
                        value = rule.checkingValue;
                        break;
                    case CodeRule.CodeConditionType.great:
                        value = rule.checkingValue;
                        break;
                    case CodeRule.CodeConditionType.less:
                        value = rule.checkingValue;
                        break;
                    case CodeRule.CodeConditionType.selection:
                        value = rule.rangeSection[rule.rangeSection.Count-1];
                        break;
                    default:
                        break;
                }

                if (IsAmendElement)
                {
                    AddAttributeToElement(ifcAttributes, rule.checkingKey, value, type);
                }
            }


          
        }





        if(failCount > 0)
        {
            validation.validation = validation.GetValidationResultType(MetaBIM_IfcValidation.ifcValidationResult.failed);
        }else

        if (failCount ==  0 && passCount > 0)
        {
            validation.validation = validation.GetValidationResultType(MetaBIM_IfcValidation.ifcValidationResult.passed);
        }

    }


    public void AddAttributeToElement(MetaBIM_IfcAttributes _item, string _key, string _value, int _type)
    {
        _item.attributes.Add(_key);
        _item.values.Add(_value);
        _item.checkedType.Add(_type);
    }

    public bool Check_IfValidated(BimObjectRecord _record, string _codeRule)
    {
        string vale = _record.IfcValidation.GetResultByID(_codeRule);

        if(vale == "")
        {
            return false;
        }
        else
        {
            return true;
        }

    }

    public bool Check_InSelection(BimObjectRecord _record, string _attributeName, List<string> _selection)
    {
        string attribute = _record.ifcAttribute.Find(_attributeName);

        if (attribute != null)
        {
            for(int i = 0; i<_selection.Count; i++)
            {
                if (_selection[i].ToLower() == attribute.ToLower())
                {
                    return true;
                }
            }
        }


        string preperty = _record.ifcProperties.Find(_attributeName);

        if (preperty != null)
        {
            for (int i = 0; i < _selection.Count; i++)
            {
                if (_selection[i].ToLower() == preperty.ToLower())
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool Check_LessThanValue(BimObjectRecord _record, string _attributeName, string _value)
    {
        string attribute = _record.ifcAttribute.Find(_attributeName);

        float compareTo = -1;

        if (TryExtractValue(_value, out compareTo))
        {
            if (attribute != null)
            {
                float value;

                if (TryExtractValue(attribute, out value))
                {
                    if (compareTo <= value)
                    {
                        return true;
                    }
                }
            }
        }



        string preperty = _record.ifcProperties.Find(_attributeName);

        if (preperty != null)
        {
            if (preperty != null)
            {
                float value;

                if (TryExtractValue(preperty, out value))
                {
                    if (compareTo <= value)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    
    public bool Check_GreaterThanValue(BimObjectRecord _record, string _attributeName, string _value)
    {
        string attribute = _record.ifcAttribute.Find(_attributeName);

        float compareTo = -1;
        
        if(TryExtractValue(_value, out compareTo))
        {
            if (attribute != null)
            {
                float value;

                if (TryExtractValue(attribute, out value))
                {
                    if (compareTo >= value)
                    {
                        return true;
                    }
                }
            }
        }



        string preperty = _record.ifcProperties.Find(_attributeName);

        if (preperty != null)
        {
            if (preperty != null)
            {
                float value;
                
                if (TryExtractValue(preperty, out value))
                {
                    if (compareTo >= value)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool Check_EqualValue(BimObjectRecord _record, string _attributeName, string _value)
    {
        string attribute = _record.ifcAttribute.Find(_attributeName);

        if (attribute != null)
        {
            if (attribute.ToLower() == _value.ToLower())
            {
                return true;
            }
        }


        string preperty = _record.ifcProperties.Find(_attributeName);

        if (preperty != null)
        {
            if (preperty.ToLower() == _value.ToLower())
            {
                return true;
            }
        }
        
        return false;
    }

    public bool Check_WithinValue(BimObjectRecord _record, string _attributeName, List<string> _range)
    {
        string attribute = _record.ifcAttribute.Find(_attributeName);

        float Max = -1;
        float Min = -1;

        TryExtractValue(_range[0], out Min);
        TryExtractValue(_range[1], out Max);
        
        if (attribute != null)
        {
            float value;

            if (TryExtractValue(attribute, out value))
            {
                if (value >= Min && value <= Max)
                {
                    return true;
                }
            }
        }


        string preperty = _record.ifcProperties.Find(_attributeName);

        if (preperty != null)
        {
            float value;

            if (TryExtractValue(preperty, out value))
            {
                if (value >= Min && value <= Max)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool Check_HasAttribute(BimObjectRecord _record, string _attributeName)      
    {

        string attribute = _record.ifcAttribute.Find(_attributeName);

        if (attribute != null)
        {
            return true;
        }

        string preperty = _record.ifcProperties.Find(_attributeName);
        
        if (preperty != null)
        {
            return true;
        }

        return false;
    }

    public bool TryExtractValue(string _value, out float _result)
    {
        string resultString = Regex.Match(_value, @"\d+").Value;

        float resultNumber = -1f;
        bool isNumber = float.TryParse(resultString, out resultNumber);

        _result = resultNumber;
        return isNumber;
    }   
    
}
