using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SetThemeColor : MonoBehaviour
{
    public string Name = "Theme_Default";
    public Color Item;
    public Image PanelImage;

    public void SetToThemeColor()
    {
        if (PanelImage == null)
        {
            PanelImage = this.GetComponent<Image>();
        }


        if (ResourceHolder.Instance != null)
        {
            Item = ResourceHolder.Instance.GetThemeColor(Name);
            PanelImage.color = Item;
        }
    }
}
