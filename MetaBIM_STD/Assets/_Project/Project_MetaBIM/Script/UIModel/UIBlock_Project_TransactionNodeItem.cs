using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using MetaBIM;

public class UIBlock_Project_TransactionNodeItem : MonoBehaviour
{
    public Image Image_Type;
    public Transaction Item;

    public TextMeshProUGUI Text_ChainNode;
    public TextMeshProUGUI Text_CreateTime;
    public TextMeshProUGUI Text_Type;
    public TextMeshProUGUI Text_Account;
    public TextMeshProUGUI Text_Confirmed;
    public TextMeshProUGUI Text_Token;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    public void SetBlock(Transaction _Transaction)
    {
        Item = _Transaction;

        Text_CreateTime.text = new DateTime(long.Parse(Item.created)).ToString("yyyy-MM-dd HH:mm");
        Text_ChainNode.text = Utility.GetLastPartOfGuid(Item.guid).Substring(0, 2);
        Text_Token.text = Utility.GetLastPartOfGuid(Item.guid);

        //Text_FileName
        string nodeType = Item.TransactionAction.ToLower();
        Image_Type.color = ResourceHolder.Instance.GetThemeColor("node_" + nodeType);


    }
}
