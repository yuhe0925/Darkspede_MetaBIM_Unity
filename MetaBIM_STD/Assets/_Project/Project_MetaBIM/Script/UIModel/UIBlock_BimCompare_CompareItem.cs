using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MetaBIM;
using TMPro;
using UnityEngine.UI;

public class UIBlock_BimCompare_CompareItem : MonoBehaviour
{
    public GameObject SelectedEffect;
    public IfcCompareNode Item;
    
    public TextMeshProUGUI Text_Marker;
    public Image Imaeg_Maker;
    public TextMeshProUGUI Text_ObjectName;


    // Start is called before the first frame update
    void Start()
    {
        OnDeselect();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBlock(IfcCompareNode _item)
    {
        Item = _item;
        Text_Marker.text = Item.NodeType;
        Imaeg_Maker.color = ResourceHolder.Instance.GetThemeColor(Item.NodeType);
        Text_ObjectName.text = Item.IfcObject.name;

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
