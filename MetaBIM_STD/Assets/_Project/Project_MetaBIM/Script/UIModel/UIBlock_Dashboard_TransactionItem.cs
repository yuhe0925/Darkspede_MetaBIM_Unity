using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Text;
using MetaBIM;

public class UIBlock_Dashboard_TransactionItem : MonoBehaviour
{
    public Transaction RelatedTransaction;
    public Project RelatedProject;
    public TextMeshProUGUI Date;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Type;
    public TextMeshProUGUI AssetSize;
    public TextMeshProUGUI Account;
    public TextMeshProUGUI ChainNode;
    public TextMeshProUGUI Confirmed;
    public TextMeshProUGUI Owner;
    public TextMeshProUGUI Token;

    public void SetBlock(Transaction _Transaction, Project _Project)
    {
        RelatedTransaction = _Transaction;
        RelatedProject = _Project;

        Name.text = RelatedProject.projectName;
        Date.text = new DateTime(long.Parse(RelatedTransaction.created)).ToString("yyyy-MM-dd HH:mm");
        Type.text = RelatedTransaction.TransactionAction;



        // TODO
        Account.text = "@" + Utility.GetLastPartOfGuid(RelatedTransaction.createdBy);
        ChainNode.text = Utility.GetLastPartOfGuid(RelatedTransaction.guid);
        Confirmed.text = "@" + Utility.GetLastPartOfGuid(RelatedTransaction.recipient);
        Owner.text = "@" + Utility.GetLastPartOfGuid(RelatedProject.createdBy);

        AssetSize.text = String.Format("{0} Kb", ((int)(RelatedTransaction.assetSize/1024)).ToString());
        Token.text = Guid.NewGuid().ToString();
    }
}