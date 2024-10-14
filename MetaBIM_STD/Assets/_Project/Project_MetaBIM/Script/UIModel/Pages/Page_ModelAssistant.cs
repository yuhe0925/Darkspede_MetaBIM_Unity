using MetaBIM;
using System.Collections;
using System.Collections.Generic;
using Tayx.Graphy.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Page_ModelAssistant : MonoBehaviour
{
    public static Page_ModelAssistant Instance;

    public PanelChange MainPanel;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        MainPanel.OnOpenAction = OnPanelOpen;
        MainPanel.OnCloseAction = OnPanelClose; 
    }


    public void OnPanelOpen()
    {
        LoadingIcon.SetActive(false);
        AssistantMessageItems.Clear();
        IsOpen = true;
    }

    public void OnPanelClose()
    {
        IsOpen = false;
    }

    public bool IsOpen = false;

    public void Open()
    {
        if (IsOpen)
        {
            MainPanel.OnPanelClose();       
        }
        else
        {
            MainPanel.OnPanelOpen();
        }
    }




    public TMP_InputField InputField_UserMessage;
    public ScrollRect ChatScroll;
    public GameObject LoadingIcon;

    public GameObject AssistantMessageItem;
    public List<GameObject> AssistantMessageItems = new List<GameObject>();
    public Transform AssistantMessageParent;

    [Header("Preset Data")]
    public List<GameObject> ResponseItems = new List<GameObject>();

    public void OnClick_SendMessage()
    {
        string message = InputField_UserMessage.text;

        if (message != "")
        {
            // clear inpt   
            InputField_UserMessage.text = "";

            // create
            var messageItem = new ModelAssistantMessage();
            messageItem.Message = message;  
            messageItem.Sender = "user";    

            var newMessage = Instantiate(AssistantMessageItem, AssistantMessageParent);
            newMessage.SetActive(true);
            newMessage.GetComponent<UIBlock_BimViewer_ModelAssistentMessageItem>().SetBlock(messageItem);
            ChatScroll.normalizedPosition = new Vector2(1, 1);

            AssistantMessageItems.Add(newMessage);

            LoadingIcon.SetActive(true);
            Invoke("OnMessageResponse", Random.Range(2f, 6f));
        }



    }


    public int ResponseIndex = 0;
    public void OnMessageResponse()
    {
        Debug.Log("OnMessageResponse: ");
        LoadingIcon.SetActive(false);

        var messageItem = new ModelAssistantMessage();
        messageItem.Sender = "response";

        var newMessage = Instantiate(ResponseItems[ResponseIndex], AssistantMessageParent);
        newMessage.SetActive(true);
        newMessage.GetComponent<UIBlock_BimViewer_ModelAssistentMessageItem>().SetBlock(messageItem);
        AssistantMessageItems.Add(newMessage);

        ResponseIndex++;
    }






    public void OnFocusObjects(string _name)
    {
        Page_BIMViewer.Instance.OnIsolateNodeObjectWithClass(_name);
    }
}
