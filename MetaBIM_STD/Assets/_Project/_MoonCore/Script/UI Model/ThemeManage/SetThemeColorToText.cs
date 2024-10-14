using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class SetThemeColorToText : MonoBehaviour
{
    public string Name = "Theme_Default";
    public Color Item;
    public TextMeshProUGUI Text;



    public void SetToThemeColor()
    {
        if (Text == null)
        {
            Text = this.GetComponent<TextMeshProUGUI>();
        }


        if (ResourceHolder.Instance != null)
        {
            Item = ResourceHolder.Instance.GetThemeColor(Name);
            Text.color = Item;
        }
    }
}
