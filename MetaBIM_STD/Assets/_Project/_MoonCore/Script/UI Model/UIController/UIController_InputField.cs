using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController_InputField : MonoBehaviour
{
    public InputType Type = InputType.Titled;
    public TMP_InputField InputField;
    public string NonePlaceHolder;


    [Header("Type Items")]
    public GameObject InputFieldTitle;


    [Header("Input Field Validation")]
    public ValidateType ValidateType = ValidateType.None;
    public GameObject[] Icon_ValidContent;
    public bool AllowEmpty = false;
    public bool IsValidated = false;


    [Header("Transaction")]
    public float TransactSpeed = 0.5f;
    public float TransactOriginX = 0f;
    public float TransactTargetX = 0f;

    public float TransactOriginY = 0f;
    public float TransactTargetY = 24f;


    [SerializeField]
    private RectTransform TitleRect;

    // Start is called before the first frame update
    void Start()
    {
        InputField.onFocusSelectAll = false;
        if(NonePlaceHolder != "")
        {
            InputField.text = NonePlaceHolder;
        }

        InitTransaction();
    }



    public void InitTransaction()
    {
        //Debug.Log("UIController_InputField.InitTransaction");

        Icon_ValidContent[0].SetActive(false);
        Icon_ValidContent[1].SetActive(false);

        if (!AllowEmpty && InputField != null && InputField.text == "")
        {
            OnTitleTransacting(false);
        }
        else
        {
            OnTitleTransacting(true);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnInput()
    {
        //Keyboard.current.SetImeEnabled(true);
    }

    public void OnInputDone()
    {
        // validating
       
    }


    private void OnTitleTransacting(bool _isIdle)
    {
        if (_isIdle)
        {
            // move up
            LeanTween.moveX(TitleRect, TransactTargetX, TransactSpeed);
            LeanTween.moveY(TitleRect, TransactTargetY, TransactSpeed);
        }
        else
        {
            // move back
            LeanTween.moveX(TitleRect, TransactOriginX, TransactSpeed);
            LeanTween.moveY(TitleRect, TransactOriginY, TransactSpeed);
        }
    }


    public void SetBlock(string _content)
    {
        InputField.text = _content;

        if (!AllowEmpty && InputField != null && InputField.text == "")
        {
            OnTitleTransacting(false);
        }
        else
        {
            OnTitleTransacting(true);
        }
    }

    public void OnClear()
    {
        InputField.text = NonePlaceHolder;
    }
    

    public void OnStartEditing()
    {
        Icon_ValidContent[0].SetActive(false);
        Icon_ValidContent[1].SetActive(false);

        // Move tile to upper position
        OnTitleTransacting(true);
    }

    public void OnValidateInput()
    {

        IsValidated = false;

        if (!AllowEmpty && InputField != null && InputField.text == "")
        {
            SetValidate(false);
            return;
        }


        switch (ValidateType)
        {
            case ValidateType.None:
                break;
            case ValidateType.Email:
                if (!Utility.ValidateEmail(InputField.text)){
                    SetValidate(false);
                    return;
                }
                break;
                // ... add more case
        }

        SetValidate(true);

        IsValidated = true;


    }


    public void SetValidate(bool _valid)
    {
        if (_valid)
        {
            Icon_ValidContent[0].SetActive(true);
            Icon_ValidContent[1].SetActive(false);
        }
        else
        {
            Icon_ValidContent[0].SetActive(false);
            Icon_ValidContent[1].SetActive(true);
        }

        if (InputField.text == "")
        {
            OnTitleTransacting(false);
        }
     }

}


public enum InputType
{
    Titled,
    Iconed,
    Plane,
}

public enum ValidateType
{
    None,
    Email,
    ID,
    Name,
}


