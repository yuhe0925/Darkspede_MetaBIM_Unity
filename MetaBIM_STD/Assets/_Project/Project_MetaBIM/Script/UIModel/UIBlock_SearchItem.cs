using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIBlock_SearchItem : MonoBehaviour
{
    [Header("Block Element")]
    public SearchItem Item;

    [Header("Block Type")]
    public TextMeshProUGUI Text_Name;
    

    public void SetBlock(SearchItem _item)
    {
        Item = _item;
        Text_Name.text = Item.name;
    }
        
}
