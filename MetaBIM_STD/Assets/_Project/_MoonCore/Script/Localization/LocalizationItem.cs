using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEditor;

[ExecuteInEditMode]
[Serializable]
public class LocalizationItem : MonoBehaviour
{

    public TextType Type = TextType.text;

    public enum TextType
    {
        text,
        input,
        icon
    }

    public string ItemID;
    public TextMeshProUGUI TargetText;
    public TMP_InputField TargetField;

    [Header("Localization")]
    public LocalString LocalString;
    public string DefaultText;

    private int instanceID = 0;

    void Awake()
    {
        if (instanceID != GetInstanceID())
        {
            if (instanceID == 0)
            {
                instanceID = GetInstanceID();
                ItemID = Guid.NewGuid().ToString();
            }
            else
            {
                instanceID = GetInstanceID();
                if (instanceID < 0)
                {
                    ItemID = Guid.NewGuid().ToString();
                }
            }
        }
    }
    


    public void SetText(string _text)
    {
        Debug.Log(ItemID + " is set to: " + _text);
        switch (Type)
        {
            case TextType.text:
                TargetText.text = _text;
                break;
            case TextType.input:
                TargetField.text = _text;
                break;
            default:
                break;
        }

  
    }

    public void SetLocalize(string _location)
    {
        Debug.Log("SetLocalize: " + _location);
        string setText = "Unknow";
        
        switch (_location.ToUpper())
        {
            case "EN":
                LocalString.Location = _location;
                if(LocalString.EN != "")
                {
                    setText = LocalString.EN;
                }
                break;
            case "CH":
                LocalString.Location = _location;
                if (LocalString.CH != "")
                {
                    setText = LocalString.CH;
                }
                break;
            case "ZH":
                LocalString.Location = _location;
                if (LocalString.CH != "")
                {
                    setText = LocalString.CH;
                }
                break;
            default:
                LocalString.Location = "EN";
                if (LocalString.EN != "")
                {
                    setText = LocalString.EN;
                }
                break;
        }

        SetText(setText);
    }

    public void SetDefaultText()
    {
        LocalString.ID = ItemID;
        LocalString.Name = transform.gameObject.name;


        if(GetComponent<TextMeshProUGUI>() != null)
        {
            TargetText = GetComponent<TextMeshProUGUI>();
            Type = TextType.text;
        }
        else if(GetComponent<TMP_InputField>() != null)
        {
            TargetField = GetComponent<TMP_InputField>();
            Type = TextType.input;
        }
        else
        {
            Debug.LogError(gameObject.name + " is not able to set as localization item");
            return;
        }


        if (!LocalString.Ignored && LocalString.Location == "")
        {

            if (Type == TextType.text)
            {
                LocalString.EN = TargetText.text;
            }

            if (Type == TextType.input)
            {
                LocalString.EN = TargetField.text;
            }
        }
    }



    public void SetToCreatedText_EN()
    {
        LocalString.Location = "EN";
        
        if (GetComponent<TextMeshProUGUI>() != null)
        {
            TargetText = GetComponent<TextMeshProUGUI>();
            LocalString.EN = TargetText.text;
        }
        else if (GetComponent<TMP_InputField>() != null)
        {
            TargetField = GetComponent<TMP_InputField>();
            LocalString.EN = TargetField.text;
        }

    }
}



[Serializable]
public class LocalString{
    public string ID = "0000";
    public string Name = "";
    public bool Ignored = false;
    public string Location = "";
    public string EN = "";
    public string CH = "";
}




