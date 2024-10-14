using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBlock_BimViewer_LevelItem : MonoBehaviour
{

    public BimLevel Item;

    public TMP_InputField Text_LevelName;
    public TMP_InputField Text_Level_Elavation;


    public TMP_InputField Input_LevelOffset;

    public void SetBlock(BimLevel _item)
    {
        Item = _item;
        Text_LevelName.text = Item.LevelObject.name;
    }


    public void SetBlock()
    {
        Text_Level_Elavation.text = Item.LevelCurrentHeight.ToString();
        Input_LevelOffset.text = Item.LevelOffset.ToString();
    }
}
