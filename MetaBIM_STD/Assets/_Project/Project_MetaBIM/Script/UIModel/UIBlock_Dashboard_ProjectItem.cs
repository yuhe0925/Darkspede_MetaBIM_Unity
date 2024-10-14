using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Text;
using MetaBIM;

public class UIBlock_Dashboard_ProjectItem : MonoBehaviour
{
    public Request Item;

    public TextMeshProUGUI Text_ItemDate;
    public TextMeshProUGUI Text_ItemMobile;
    public TextMeshProUGUI Text_ItemType;
    public TextMeshProUGUI Text_ItemEmail;
    public TextMeshProUGUI Text_ItemFirstName;
    public TextMeshProUGUI Text_ItemLastName;


    public void SetBlock(Request _item)
    {
        Item = _item;

        Text_ItemDate.text = Utility.TimeFromTick(Item.created);
        Text_ItemMobile.text = Item.requestMobile;
        Text_ItemType.text = Item.requyestType;
        Text_ItemEmail.text = Item.requestEmail;
        Text_ItemFirstName.text = Item.requestFirstName;
        Text_ItemLastName.text = Item.requestLastName;

    }

}