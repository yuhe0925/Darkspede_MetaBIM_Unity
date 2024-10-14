using MetaBIM;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBlock_Project_ModelUpdateItem : MonoBehaviour
{
    public VersionUpdate Item;
    public string attachedProject;
    public string attachedVersion;

    
    public GameObject SelectionObject;
    public GameObject Status_Selected;
    public GameObject Status_NoTSelected;

    public TextMeshProUGUI Text_Content;
    public TextMeshProUGUI Text_Date;
    public TextMeshProUGUI Text_Status;
    
    public MC_GetWebIcon Image_User;

    public bool IsDefault;


    public void SetBlock(VersionUpdate _item, string _projectGuid, string _versionGuid )
    {
        Item = _item;
        attachedProject = _projectGuid;
        attachedVersion = _versionGuid;
        

        Text_Content.text = _item.GetUpdateCount() + " updates has been made.";
        
        Text_Date.text = new DateTime(long.Parse(Item.updated)).ToString("yyyy-MM-dd HH:mm");

        if (IsDefault)
        {
            Text_Status.text = "Master";       
        }

        OnDeselected();
    }


    public void OnSelected()
    {
        if (SelectionObject != null)
        {
            SelectionObject.SetActive(true);
            Status_Selected.SetActive(true);
            Status_NoTSelected.SetActive(false);
        }
    }

    public void OnDeselected()
    {
        if (SelectionObject != null)
        {
            SelectionObject.SetActive(false);
            Status_Selected.SetActive(false);
            Status_NoTSelected.SetActive(true);
        }
    }



    public void OnClick_SelectUserIcon()
    {
        
    }


}
