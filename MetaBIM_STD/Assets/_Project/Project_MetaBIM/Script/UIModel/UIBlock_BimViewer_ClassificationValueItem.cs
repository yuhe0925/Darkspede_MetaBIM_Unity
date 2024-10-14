using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Xml;
using UnityEngine.UI;
using MetaBIM;
using System.IO;

public class UIBlock_BimViewer_ClassificationValueItem : MonoBehaviour
{
    [Header("Block Element")]
    public ClassificationItemValue Item;

    [Header("Block Element")]
    public GameObject SelectedEffect;
    public TextMeshProUGUI Text_Header;
    public TextMeshProUGUI Text_Content;
    public OnCursorHover Hover;

    public GameObject Object_Header;
    public GameObject Object_Content;


    public void SetBlock(ClassificationItemValue _item)
    {
        Item = _item;

        if (Item.isHeader)
        {
            Object_Content.SetActive(false);
            Object_Header.SetActive(true);
            Text_Header.text = Item.itemName;
            Hover.enabled = false;
        }
        else
        {
            Object_Content.SetActive(true);
            Object_Header.SetActive(false);
            Text_Content.text = Item.itemName; 
        }

        OnDeselect();
    }





    public void OnSelect()
    {
        if (Item.isHeader)
        {
            return;
        }
        SelectedEffect.SetActive(true);
    }

    public void OnDeselect()
    {
        if (Item.isHeader)
        {
            return;
        }
        SelectedEffect.SetActive(false);
    }






}
