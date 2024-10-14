using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MetaBIM;
using UnityEngine.UI;
using TMPro;
using System;

public class UIBlock_Project_AssetChainNodeItem : MonoBehaviour
{
    //public TextMeshProUGui Text_FileSize;

    public TextMeshProUGUI Text_FileSize;
    public TextMeshProUGUI Text_CreateTime;
    public TextMeshProUGUI Text_Name;
    public TextMeshProUGUI Text_Token;
    public TextMeshProUGUI Text_FileName;
    public TextMeshProUGUI Text_Version;

    public GameObject UI_LineReach;
    public GameObject UI_LineAppend;
    public GameObject UI_LineFileNode;
    public GameObject UI_FileNode;

    public Image Image_Type;


    public Transaction RelatedTransaction;
    public Project RelatedProject;



    public void SetBlock(Transaction _Transaction, Project _Project, bool _isNext = true, string _fileName = "")
    {
        RelatedTransaction = _Transaction;
        RelatedProject = _Project;

        Text_FileSize.text = String.Format("{0} Kb", ((int)(RelatedTransaction.assetSize / 1024)).ToString());
        Text_CreateTime.text = new DateTime(long.Parse(RelatedTransaction.created)).ToString("yyyy-MM-dd HH:mm");

        Text_Name.text = Utility.GetLastPartOfGuid(RelatedTransaction.guid).Substring(0,2);
        Text_Token.text  = Utility.GetLastPartOfGuid(RelatedTransaction.guid);

        //Text_FileName
        string nodeType =  RelatedTransaction.TransactionAction.ToLower();

        Image_Type.color = ResourceHolder.Instance.GetThemeColor("node_" + nodeType);

        Text_FileName.text = "";

        if (nodeType == "create")
        {
            UI_LineAppend.SetActive(false);
        }
        else if (nodeType == "convertion")
        {

        }
        else if (nodeType == "upload")
        {
            Text_FileName.text = "";
        }
        else if (nodeType == "modify")
        {

        }

        // 

        if (!_isNext)
        {
            UI_LineReach.SetActive(false);
        }


        if(_fileName != "")
        {
            Text_Version.text = _fileName;
        }
        else
        {
            UI_LineFileNode.SetActive(false);
            UI_FileNode.SetActive(false);
        }
    
    }
}



