using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MetaBIM;
using System;
using UnityEngine.Networking;

public class UIBlock_BimViewer_BCFItem : MonoBehaviour
{
    public BCF Item;
    public MC_GetWebIcon BCFImage;

    public TextMeshProUGUI timeText;
    public TextMeshProUGUI titleText;
    public Image priorityImage;
    public TextMeshProUGUI priorityText;
    public TextMeshProUGUI assignedText;

    // Start is called before the first frame update
    void Start()
    {

    }


    public void SetBlock(BCF _Item)
    {
        Item = _Item;

        timeText.text =  new DateTime(long.Parse(Item.updated)).ToString("yyyy/MM/dd HH:mm");
        titleText.text = Item.issueTitle;
        priorityText.text = Item.priority;
        priorityImage.color = ResourceHolder.Instance.GetThemeColor(string.Format("Priority_{0}", Item.priority));
        string userName = Page_Project.Instance.CollaborationUsers.Find(x => x.guid == Item.assignedTo).fullName;
        assignedText.text = string.Format("Assigned to @{0}", userName);

        string imageUrl = Config.BCFImage_Path + Item.guid + "/" + Item.snapshotImageUrl;
        Debug.Log("Fetching image from " + imageUrl);

        BCFImage.SetBlock(imageUrl);

    }


}
