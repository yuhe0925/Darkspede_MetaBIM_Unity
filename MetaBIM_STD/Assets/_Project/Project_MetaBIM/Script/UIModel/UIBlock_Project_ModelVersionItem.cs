using MetaBIM;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBlock_Project_ModelVersionItem : MonoBehaviour
{
    public MetaBIM.Version Item;


    public GameObject SelectionObject;
    public TextMeshProUGUI Text_FileType;
    public TextMeshProUGUI Text_FileName;
    public TextMeshProUGUI Text_VersionID;
    public TextMeshProUGUI Text_Date;
    public TextMeshProUGUI Text_FileSize;

    public GameObject Status_Complete;
    public GameObject Status_Pending;


    public void SetBlock(MetaBIM.Version _item)
    {
        Item = _item;
        
        Text_FileName.text = Item.originalFileName;
        Text_VersionID.text = Item.guid;
        Text_Date.text = new DateTime(long.Parse(Item.updated)).ToString("yyyy-MM-dd HH:mm");
        Text_FileSize.text = Utility.FormatFileSize(Item.sourceFileSize + Item.xmlFileSize);

        if (Item.processingStatus == "complete" || Item.processingStatus == "completed")
        {
            Status_Complete.SetActive(true);
            Status_Pending.SetActive(false);
        }
        else
        {
            Status_Complete.SetActive(false);
            Status_Pending.SetActive(true);
        }

        OnDeselected();
    }


    public void OnRequestVersionStatusUpdate()
    {

    }

    
    public void OnRequestVersionStatusUpdate_Callback(bool _string, string _message)
    {

    }


    public void OnClick_CopyVersionGuid()
    {
        GUIUtility.systemCopyBuffer = Item.guid;
        MCPopup.Instance.SetInformation("Version ID saved to clipboard");
    }



    public void OnSelected()
    {
        if (SelectionObject != null)
        {
            SelectionObject.SetActive(true);
        }
    }

    public void OnDeselected()
    {
        if (SelectionObject != null)
        {
            SelectionObject.SetActive(false);
        }
    }

}
