using MetaBIM;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBlock_BimViewer_IfcAttributeItem : MonoBehaviour
{
    public IfcAttributeItem Item;


    [Header("Item Elements")]
    public TextMeshProUGUI Text_Key;
    public TextMeshProUGUI Text_KeyEditor;
    public TextMeshProUGUI Text_Value;
    public Image Background;
    public GameObject Splitor;
    public GameObject MarkExpended;
    public GameObject MarkCollapsed;

    public Button ButtonEdit;
    public OnCursorHover Hover;

    public List<Color> BGColor;
    public Image BGImage;

    public void SetBlock(IfcAttributeItem _item)
    {
        Item = _item;

        
        Text_Key.text = Item.AttributeKey;
        Text_KeyEditor.text = Item.AttributeKey;
        
        Splitor.SetActive(Item.IsHeader);
        
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


            if (Item.checkType != -1)
            {
                BGImage.color = BGColor[Item.checkType];
            }
            else
            {
                BGColor[0] = BGImage.color;
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


    public void OnClick_EditProperty()
    {
        if (Item.Type == IfcAttributeItem.AttributeType.uniclass)
        {
            Page_BIMViewer.Instance.OnClick_AttributeItem(Page_ClassificationSelector.ClassificationModeType.uniclass, Item.AttributeItemObject);
        }
        else
        if (Item.Type == IfcAttributeItem.AttributeType.ifcParameter)
        {
            Page_BIMViewer.Instance.OnClick_AttributeItem(Page_ClassificationSelector.ClassificationModeType.ifc, Item.AttributeItemObject);
        }
        else
        if (Item.Type == IfcAttributeItem.AttributeType.zone)
        {
            //Page_BIMViewer.Instance.OnClick_ZoneSetupOnAttributes(Item.AttributeItemObject);
        }
        else
        if (Item.Type == IfcAttributeItem.AttributeType.sustainability)
        {
            Page_BIMViewer.Instance.OnClick_AttributeItem(Page_ClassificationSelector.ClassificationModeType.sustainability, Item.AttributeItemObject);
        }
    }


}
