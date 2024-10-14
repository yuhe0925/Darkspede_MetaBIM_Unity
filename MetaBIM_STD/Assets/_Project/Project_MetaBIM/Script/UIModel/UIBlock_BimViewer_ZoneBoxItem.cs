using MetaBIM;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBlock_BimViewer_ZoneBoxItem : MonoBehaviour
{

    public ZoneBox Item;
    public VersionZone UpdateZone;
    
    public TextMeshProUGUI Text_ZoneBoxID;
    public TMP_InputField Input_ZoneBoxInfo;

    public GameObject SelectedEffect;



    public void SetBlock(ZoneBox _item)
    {
        Item = _item;
        Text_ZoneBoxID.text = Item.BoxIndex.ToString();
        Input_ZoneBoxInfo.text = Item.ZoneName;
    }

    public void OnChangeValue_ZoneBoxName(string _name)
    {
        Item.ZoneName = _name;
    }
    
    public void OnSelect()
    {
        SelectedEffect.SetActive(true);
    }
    
    public void OnDeselect()
    {
        SelectedEffect.SetActive(false);
    }
}
