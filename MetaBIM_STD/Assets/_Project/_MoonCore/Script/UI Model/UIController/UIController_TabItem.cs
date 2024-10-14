using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController_TabItem : MonoBehaviour
{
    public GameObject OnSelectEffect;
    public TextMeshProUGUI Text_Tab;
    public string Key;


    // Start is called before the first frame update

    void Start()
    {
        OnSelectEffect.SetActive(false);
    }



    public void OnSelect()
    {
        Debug.Log("UIController_TabItem:OnSelect");
        OnSelectEffect.SetActive(true);
    }
     

    public void Deselect()
    {
        Debug.Log("UIController_TabItem:Deselect");
        OnSelectEffect.SetActive(false);
    }
}
