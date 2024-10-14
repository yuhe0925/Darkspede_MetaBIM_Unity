using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MetaBIM;
using System;

public class UIBlock_Workspace_ProjectItem : MonoBehaviour
{

    // in this case the project is the model item
    public Project Item;
    public Image ProjectHeaderBar;
    public TextMeshProUGUI ProjectName;
    public TextMeshProUGUI ProjectDateTime;
    public TextMeshProUGUI ProjectConvertion;
    public MC_GetWebIcon IconImage;


    public void SetBlock(Project _item)
    {
        Item = _item;

        ProjectName.text = Item.projectName;
        ProjectDateTime.text = Utility.TimeFromTick( Item.created);
        ProjectConvertion.text = String.Format("Model: {0}", Item.versions.Count.ToString());

        IconImage.SetBlock(Item.projectSnaphotUrl);

        //StartCoroutine(DataProxy.Instance.GetAllCollaborations("associatedProject", Project.guid, Callback_ChangeColor));
    }


}
