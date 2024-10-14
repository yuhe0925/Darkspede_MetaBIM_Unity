using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Text;
using MetaBIM;

public class UIBlock_Dashboard_WorkspaceItem : MonoBehaviour
{
    public Workspace Item;

    [Header("UI Component")]
    public Image HeaderBar;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI DateTime;
    public TextMeshProUGUI SummaryInform;
    public MC_GetWebIcon PreviewImage;

    
    public void SetBlock(Workspace _item)
    {
        Item = _item;
        Name.text = Item.workspaceName;
        DateTime.text = Utility.TimeFromTick(Item.created);
        SummaryInform.text = String.Format("Project: {0}, Model: {1}", Item.projects.Count, Item.GetModelCount());

        //Debug.Log("UIBlock_Dashboard_WorkspaceItem: " + Item.workspaceName);
        PreviewImage.SetBlock(Item.workspaceSnapshotUrl);
    }

}
