using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIBlock_BimViewer_IfcAttributesValueItem : MonoBehaviour
{

    public TextMeshProUGUI Text_Key;
    public TextMeshProUGUI Text_Value;
    public Image Background;


    public void SetBlock(string _key, string _Text_Value, int _index, string _type = "")
    {
        Text_Key.text = _key;
        Text_Value.text = _Text_Value;



        if (_type == "")
        {
            if (_index % 2 == 0)
            {
                Background.color = ResourceHolder.Instance.GetThemeColor("Text_White");
            }
            else
            {
                Background.color = ResourceHolder.Instance.GetThemeColor("TableRowGray");
            }
        }
        else
        {
            Background.color = ResourceHolder.Instance.GetThemeColor(_type);
        }



    }


}
