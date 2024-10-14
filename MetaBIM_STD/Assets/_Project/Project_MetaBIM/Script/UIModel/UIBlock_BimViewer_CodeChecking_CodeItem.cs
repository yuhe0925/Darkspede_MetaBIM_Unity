using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBlock_BimViewer_CodeChecking_CodeItem : MonoBehaviour
{
    [Header("Block Element")]
    public MetaBIM.CodeChecking.CodeRule Item;

    public TMP_InputField Text_Header;

    public TMP_InputField Text_ClassTarget;
    public TMP_InputField Text_Condition;
    
    //public TMP_InputField Text_AttributeTarget;
    public TMP_InputField Text_CheckValue;
    public TMP_InputField Text_Range;


    public void SetBlock(MetaBIM.CodeChecking.CodeRule _item, int _index = 0)
    {
        Item = _item;

        Text_Header.text = "Code: " + Item.guid;
        Text_ClassTarget.text = Item.checkingClass;
        Text_Condition.text = Item.GetCodeConditionTypeString();

        Text_CheckValue.text = Item.checkingKey;
        Text_Range.text = Item.GetTargetValue();

        /*
        if (Item.rangeSection.Count > 0)
        {
            Text_RangeMin.text = Item.rangeSection[0];
            Text_RangeMax.text = Item.rangeSection[Item.rangeSection.Count - 1];
        }
        else
        {
            Text_RangeMin.text = "-";
            Text_RangeMax.text = "-";
        }
        */
    }

}
