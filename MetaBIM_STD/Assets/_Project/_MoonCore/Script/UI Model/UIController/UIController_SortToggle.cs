using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MetaBIM;   // change to other project 

public class UIController_SortToggle : MonoBehaviour
{

    public bool isSortAscent;
    public bool isSelected;
    public DataSet.SortGroup Key;


    public Image BackgroundImage;
    public TextMeshProUGUI Text_Icon;
    public string AscentText;
    public string DesentText;





    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnClick_Sort()
    {
        isSortAscent = !isSortAscent;
    }

    public void OnSelect()
    {
        BackgroundImage.color = ResourceHolder.Instance.GetThemeColor("Text_Theme");
        Text_Icon.color = ResourceHolder.Instance.GetThemeColor("Text_White");
        isSelected = true;

    }

    public void OnSwitch()
    {
        isSortAscent = !isSortAscent;

        if (isSortAscent)
        {
            Text_Icon.text = AscentText;
        }
        else
        {
            Text_Icon.text = DesentText;
        }

    }

    public void OnDeselect()
    {
        BackgroundImage.color = ResourceHolder.Instance.GetThemeColor("Text_White");
        Text_Icon.color = ResourceHolder.Instance.GetThemeColor("Text_Theme");
        isSelected = false;
    }
}
