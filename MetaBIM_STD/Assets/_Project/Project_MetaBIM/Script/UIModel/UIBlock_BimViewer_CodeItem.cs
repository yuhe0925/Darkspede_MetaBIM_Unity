using MetaBIM;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBlock_BimViewer_CodeItem : MonoBehaviour
{
    public IfcAttributeItem Item;


    [Header("Item Elements")]
    public TextMeshProUGUI Text_Key;
    public TextMeshProUGUI Text_KeyEditor;
    public TextMeshProUGUI Text_Value;
    public Image Background;
    public GameObject Splitor;
    public GameObject SelectEffect;
    public GameObject MarkExpended;
    public GameObject MarkCollapsed;

    public Button ButtonEdit;
    public OnCursorHover Hover;


    public void SetBlock(IfcAttributeItem _item)
    {
        Item = _item;

        
        Text_Key.text = Item.AttributeKey;
        Text_KeyEditor.text = Item.AttributeKey;
        
        Splitor.SetActive(Item.IsHeader);

        SelectEffect.SetActive(false);
        
        if (Item.IsHeader)
        {
            Text_Key.fontStyle = FontStyles.Bold;
            Text_KeyEditor.fontStyle = FontStyles.Bold;
            
            Text_Value.text = "";
            Text_Value.gameObject.SetActive(false);

            MarkExpended.SetActive(!_item.IsCollapsed);
            MarkCollapsed.SetActive(_item.IsCollapsed);

            Background.color = ResourceHolder.Instance.GetThemeColor("MainTheme_LightGray");
            
            Hover.enabled = true;

            ButtonEdit.interactable = false;


        }
        else
        {
            Text_Key.fontStyle = FontStyles.Normal;
            Text_KeyEditor.fontStyle = FontStyles.Normal;
            Text_Value.text = Item.AttributeValue;
            Text_Value.gameObject.SetActive(true);

            MarkExpended.SetActive(false);
            MarkCollapsed.SetActive(false);
            
            Hover.enabled = false;

            /*
            if (Item.ListIndex % 2 == 0)
            {
                Background.color = ResourceHolder.Instance.GetThemeColor("Text_White");
            }
            else
            {
                Background.color = ResourceHolder.Instance.GetThemeColor("TableRowGray");
            }
            */

            Background.color = ResourceHolder.Instance.GetThemeColor("Text_White");
            ButtonEdit.interactable = true;

            if (Item.Type == IfcAttributeItem.AttributeType.validation )
            {
                Debug.Log("Set Attribute Display: " + Item.AttributeValue);
                Debug.Log("Frome: " + Item.AttributeKey);
                
                if (Item.AttributeValue == "passed")
                {
                    Text_Value.color = ResourceHolder.Instance.GetThemeColor("Result_Pass");
                }
                else if(Item.AttributeValue == "failed")
                {
                    Text_Value.color = ResourceHolder.Instance.GetThemeColor("Result_Failed");
                }
                else
                {
                    Text_Value.color = ResourceHolder.Instance.GetThemeColor("Text_Theme");
                }
            }
            else
            {
                Text_Value.color = ResourceHolder.Instance.GetThemeColor("Text_Theme");
            }

        }
    }

    public void OnClick_Collapse()
    {
        MarkExpended.SetActive(false);
        MarkCollapsed.SetActive(true);
    }

    public void OnClick_Expend()
    {
        MarkExpended.SetActive(true);
        MarkCollapsed.SetActive(false);
    }




    public void OnSelected()
    {
        SelectEffect.SetActive(true);
    }

    public void OnDeselected()
    {
        SelectEffect.SetActive(false);
    }


}
