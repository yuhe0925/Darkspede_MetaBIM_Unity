using MetaBIM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIBlock_Project_VersionItem : MonoBehaviour
{
    public MetaBIM.Version Item;

    public GameObject SelectionObject;

    public TextMeshProUGUI Text_FileName;
    public TextMeshProUGUI Text_Guid;
    public TextMeshProUGUI Text_DateTime;
    
    public TextMeshProUGUI Text_Type;
    public TextMeshProUGUI Text_Size;
    public TextMeshProUGUI Text_Status;


    // Start is called before the first frame update
    void Start()
    {
        OnDeselected();
    }

    

    public void SetBlock(MetaBIM.Version _item)
    {
        Item = _item;
        Text_FileName.text = Item.originalFileName;
        Text_Guid.text = Item.guid;
        Text_DateTime.text = Utility.TimeFromTick(Item.updated);
        Text_Type.text = "IFC";   // hard code
        Text_Size.text = Utility.FormatFileSize(Item.sourceFileSize);
        Text_Status.text = Item.processingStatus;
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
