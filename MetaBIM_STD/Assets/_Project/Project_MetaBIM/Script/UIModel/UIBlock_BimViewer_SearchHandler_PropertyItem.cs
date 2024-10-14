using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

public class UIBlock_BimViewer_SearchHandler_PropertyItem : MonoBehaviour
{

    public int Index;
    public ElementSearchSet Item;  

    public TMP_Dropdown Dropdown_SearchProperty;
    public TMP_Dropdown Dropdown_SearchPropertyCondition;
    public TMP_InputField InputField_SearchPropertyValue;



    public void SetBlock(ElementSearchSet _item, int _index = 0)
    {
        Debug.Log("UIBlock_BimViewer_SearchHandler_PropertyItem.SetBlock: " + _item.searchTarget);
        Index = _index;
        Item = _item;
        Dropdown_SearchProperty.ClearOptions();
        Dropdown_SearchProperty.AddOptions(MetaBIM.DataSet.ModelProperties);
        Dropdown_SearchPropertyCondition.ClearOptions();
        Dropdown_SearchPropertyCondition.AddOptions(MetaBIM.DataSet.SearchPropertyCondition);   // TODO, language setting?

        // if new, set defualt value
        if (Item.searchTarget != "default")
        {

            // get value index by search taret in item
            int index = MetaBIM.DataSet.ModelProperties.FindIndex(x => x == Item.searchTarget);
            Dropdown_SearchProperty.value = index;

            // do the same for condition
            index = MetaBIM.DataSet.SearchPropertyCondition.FindIndex(x => x == Item.condition);
            Dropdown_SearchPropertyCondition.value = index;

            InputField_SearchPropertyValue.text = Item.searchValue;
        }

    }


    public void OnValueChange()
    {
        Item.searchTarget = Dropdown_SearchProperty.options[Dropdown_SearchProperty.value].text;
        Item.condition = Dropdown_SearchPropertyCondition.options[Dropdown_SearchPropertyCondition.value].text;
        Item.searchValue = InputField_SearchPropertyValue.text;
    }
}
