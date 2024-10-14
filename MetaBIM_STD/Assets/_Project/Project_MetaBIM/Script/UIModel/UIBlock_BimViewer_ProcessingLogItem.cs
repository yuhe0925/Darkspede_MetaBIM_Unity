using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBlock_BimViewer_ProcessingLogItem : MonoBehaviour
{
  
    public int Index;
    public ProcessingLogItem Item;
    public TextMeshProUGUI Text_Time;
    public TextMeshProUGUI Text_Message;
    public GameObject SelectObject;

    public void SetBlock(ProcessingLogItem _item, int _index)
    {
        Index = _index;
        Text_Time.text = _item.DateTime.ToString("HH:mm:ss");
        Text_Message.text = _item.Message;
        Item.Index = _index;
    }
}


