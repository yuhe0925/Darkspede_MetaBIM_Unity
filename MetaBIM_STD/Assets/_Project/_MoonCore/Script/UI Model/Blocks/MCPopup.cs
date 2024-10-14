using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using JetBrains.Annotations;
using SR = StringBuffer;
using static MCPopup;

public class MCPopup : MonoBehaviour
{

    public static MCPopup Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    [Header("Input Panel & 2 Selection")]
    public PanelChange InputSelection2PagePanel;
    public TextMeshProUGUI Text_InputSelection2TextTitle;
    public TMP_InputField InputSelection2TextInput;
    public TextMeshProUGUI Text_Selection1Title;
    public TMP_Dropdown InputSelection2Dropdown1;
    public TextMeshProUGUI Text_Selection2Title;
    public TMP_Dropdown InputSelection2Dropdown2;
    public Action<bool, string, string, string> InputSelection2Callback;

    public void SetInputSelection2(Action<bool, string, string, string> _callback,
        string _inputName,
        string _select1Name,
        List<string> _selection1,
         string _select2Name,
        List<string> _selection2, 
        string _title = "")
    {
        if (_title == "")
        {
            _title = SR.Messaege_Popup_Confirm.S;
        }

        InputSelection2PagePanel.OnPanelOpen();
        InputSelection2Callback = _callback;
        Text_InputSelection2TextTitle.text = _inputName;
        InputSelection2TextInput.text = "";

        Text_Selection1Title.text = _select1Name;
        Text_Selection2Title.text = _select2Name;
        InputSelection2Dropdown1.options.Clear();
        InputSelection2Dropdown2.options.Clear();

        InputSelection2Dropdown1.AddOptions(_selection1);
        InputSelection2Dropdown2.AddOptions(_selection2);

    }

    public void OnClick_InputSelection2Accept()
    {
        InputSelection2PagePanel.OnPanelClose();
        InputSelection2Callback(true, InputSelection2TextInput.text, InputSelection2Dropdown1.options[InputSelection2Dropdown1.value].text, InputSelection2Dropdown2.options[InputSelection2Dropdown2.value].text);
    }

    public void OnClick_InputSelection2Cancel()
    {
        InputSelection2PagePanel.OnPanelClose();
        InputSelection2Callback(false, InputSelection2TextInput.text, InputSelection2Dropdown1.options[InputSelection2Dropdown1.value].text, InputSelection2Dropdown2.options[InputSelection2Dropdown2.value].text);
    }






    [Header("Input Panel & Selection")]
    public PanelChange InputSelectionPagePanel;
    public TMP_InputField InputSelectionTextTitle;
    public TMP_InputField InputSelectionTextBody;
    public TMP_Dropdown InputSelectionDropdown;
    public Action<bool, string, string> InputSelectionCallback;

    public void SetInputSelection(Action<bool, string, string> _callback, string _title = "")
    {
        if (_title == "")
        {
            _title = SR.Messaege_Popup_Confirm.S;
        }

        InputSelectionPagePanel.OnPanelOpen();
        InputSelectionCallback = _callback;
        InputSelectionTextTitle.text = _title;
        InputSelectionTextBody.text = "";

    }

    public void OnClick_InputSelectionAccept()
    {
        InputSelectionPagePanel.OnPanelClose();
        InputSelectionCallback(true, InputSelectionTextBody.text, InputSelectionDropdown.options[InputSelectionDropdown.value].text);
    }

    public void OnClick_InputSelectionCancel()
    {
        InputSelectionPagePanel.OnPanelClose();
        InputSelectionCallback(false, InputSelectionTextBody.text, InputSelectionDropdown.options[InputSelectionDropdown.value].text);
    }


    [Header("Input Panel")]
    public PanelChange InputPagePanel;
    public TMP_InputField InputTextTitle;
    public TMP_InputField InputTextBody;
    public Action<bool, string> InputCallback;


    public void SetInput(Action<bool, string> _callback, string _title = "", string _defaultValue = "")
    {
        if (_title == "")
        {
            _title = SR.Messaege_Popup_Confirm.S;
        }
        InputPagePanel.OnPanelOpen();
        InputCallback = _callback;
        InputTextTitle.text = _title;
        InputTextBody.text = _defaultValue;

    }

    public void OnClick_InputAccept()
    {
        InputPagePanel.OnPanelClose();
        InputCallback(true, InputTextBody.text);
    }

    public void OnClick_InputCancel()
    {
        InputPagePanel.OnPanelClose();
        InputCallback(false, InputTextBody.text);
    }


    [Header("Confirm Panel")]
    public PanelChange ConfirmPagePanel;
    public TMP_InputField ConfirmTextTitle;
    public TMP_InputField ConfirmTextBody;

    public Action<bool, string> ConfirmCallback;
    public string ConfirmPayload;

    public void SetConfirm(Action<bool, string> _callback, string _payload, string _message, string _title = "")
    {
        if(_title == "")
        {
            _title = SR.Messaege_Popup_Confirm.S;
        }
        ConfirmPagePanel.OnPanelOpen();
        ConfirmCallback = _callback;
        ConfirmPayload = _payload;

        ConfirmTextTitle.text = _title;
        ConfirmTextBody.text = _message;
    }

    public void OnClick_ConfirmAccept()
    {
        ConfirmPagePanel.OnPanelClose();
        ConfirmCallback(true, ConfirmPayload);
    }

    public void OnClick_ConfirmCancel()
    {
        ConfirmPagePanel.OnPanelClose();
        ConfirmCallback(false, ConfirmPayload);
    }


    [Header("Information Panel")]
    public PanelChange InformationPagePanel;
    public TMP_InputField InformationTextTitle;
    public TMP_InputField InformationTextBody;


    public void SetInformation(string _message, string _title = "")
    {
        if (_title == "")
        {
            _title = SR.Messaege_Popup_Information.S;
        }
        InformationPagePanel.OnPanelOpen();
        InformationTextTitle.text = _title;
        InformationTextBody.text = _message;
    }

    public void OnClick_InformationAccept()
    {
        InformationPagePanel.OnPanelClose();
    }


    [Header("Warning Panel")]
    public PanelChange WarningPagePanel;
    public TMP_InputField WarningTextTitle;
    public TMP_InputField WarningTextBody;


    public void SetWarning(string _message, string _title = "")
    {
        if (_title == "")
        {
            _title = SR.Messaege_Popup_Warning.S;
        }
        WarningPagePanel.OnPanelOpen();
        WarningTextTitle.text = _title;
        WarningTextBody.text = _message;
    }
    
    public void OnClick_WarningAccept()
    {
        WarningPagePanel.OnPanelClose();
    }

    [Header("Complete Panel")]
    public PanelChange CompletePagePanel;
    public TMP_InputField CompleteTextTitle;
    public TMP_InputField CompleteTextBody;


    public void SetComplete(string _message, string _title = "")
    {
        if (_title == "")
        {
            _title = SR.Messaege_Popup_Complete.S;
        }
        
        CompletePagePanel.OnPanelOpen();
        CompleteTextTitle.text = _title;
        CompleteTextBody.text = _message;
    }

    public void OnClick_CompleteAccept()
    {
        CompletePagePanel.OnPanelClose();
    }



    [Header("Notification Panel")]
    public PanelChange NotificationPagePanel;
    public TextMeshProUGUI NotificationTextIcon;
    public TextMeshProUGUI NotificationTextMessage;
    public TextMeshProUGUI NotificationTextButton;
    public Action<bool, string> NotificationCallback;
    public List<NotificationIcon> Icons;
    public string NotificationPayload;
    public NotificationIconType notificationIconType;

    public void SetNotification(Action<bool, string> _callback, string _payload, string _message, string _button = "OK", NotificationIconType _icon = NotificationIconType.message_information)
    {
        NotificationCallback = _callback;
        NotificationPayload = _payload;
        NotificationPagePanel.OnPanelOpen();
        notificationIconType = _icon;

        NotificationIcon icon = GetIcon(_icon);


        NotificationTextIcon.text = icon.IconGlyph;
        NotificationTextIcon.color = icon.IconColor;

        NotificationTextMessage.text = _message;
        NotificationTextButton.text = _button;
    }

    public void OnClick_NotificationAccept()
    {
        switch (notificationIconType)
        {
            case NotificationIconType.message_complete:
                NotificationCallback(true, NotificationPayload);
                break;
            case NotificationIconType.message_error:
                break;
            case NotificationIconType.message_information:
                break;
            default:
                break;
        }

        NotificationPagePanel.OnPanelClose();
    }

    public enum NotificationIconType
    {
        message_complete,
        message_error,
        message_information,
    }

    public NotificationIcon GetIcon(NotificationIconType _icon)
    {
        switch (_icon)
        {
            case NotificationIconType.message_complete:
                return Icons[1];
            case NotificationIconType.message_error:
                return Icons[2];
            case NotificationIconType.message_information:
                return Icons[3];
            default:
                return Icons[0];
        }

    }
    

}


[Serializable]
public class NotificationIcon
{
    public string IconName = "";
    public string IconGlyph = "";
    public Color IconColor;
}