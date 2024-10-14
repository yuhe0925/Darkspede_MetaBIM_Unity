using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBlock_BimViewer_ModelAssistentMessageItem : MonoBehaviour
{

    public ModelAssistantMessage Item;


    public TextMeshProUGUI Text_Content;
    public GameObject ActionObject;
    public Action RedirectAction;

    public bool isStop;

    public void SetBlock(ModelAssistantMessage _item)
    {
        Item = _item;

        if (Item.Sender != "user")
        {
            Item.Message = Text_Content.text;
            StartCoroutine(DisplayMessage());
        }
        else
        {
            Text_Content.text = Item.Message;
        }

        if (ActionObject != null)
        {
            ActionObject.SetActive(false);
        }
    }




    public IEnumerator DisplayMessage()
    {
        int process = 0;
        while(process < Item.Message.Length)
        {
            int next = process + UnityEngine.Random.Range(1,5);
            if(next >= Item.Message.Length)
            {
                next = Item.Message.Length;
            }
            Text_Content.text = Item.Message.Substring(0, next);
            process = next;
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.01f, 0.07f));
        }

        if(ActionObject != null)
        {
            ActionObject.SetActive(true);
        }
    }


    public void OnClick_StopMessage()
    {

    }

    public void OnClick_Action()
    {
        if(RedirectAction != null)
        {
            RedirectAction();
        }
    }
}



public class ModelAssistantMessage
{
    public string Message;
    public string Sender;

    public bool IsDisplayCollection;
    public bool IsDisplayMessage;
}
