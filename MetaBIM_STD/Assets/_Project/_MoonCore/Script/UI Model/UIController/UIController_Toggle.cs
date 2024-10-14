using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIController_Toggle : MonoBehaviour
{

    public bool IsToggled = false;
    
    public GameObject ToggleON;
    public GameObject ToggleOff;

    public UnityEvent<bool> EventRedirect;

    

    public void OnResetUI(bool _isToggle = false)
    {
        IsToggled = _isToggle;

        ToggleON.SetActive(IsToggled);
        if (ToggleOff != null)
        {
            ToggleOff.SetActive(!IsToggled);
        }

    }

    public void OnReset(bool _isToggle = false)
    {
        IsToggled = _isToggle;

        ToggleON.SetActive(IsToggled);
        if (ToggleOff != null)
        {
            ToggleOff.SetActive(!IsToggled);
        }

        EventRedirect.Invoke(IsToggled);
    }
    
    public void OnClick()
    {
        // turned off
        if (IsToggled)
        {
            ToggleON.SetActive(false);
            if (ToggleOff != null)
            {
                ToggleOff.SetActive(true);
            }
            IsToggled = false;
        }
        //turned on
        else
        {
            ToggleON.SetActive(true);
            if (ToggleOff != null)
            {
                ToggleOff.SetActive(false);
            }
            IsToggled = true;
        }

        if (EventRedirect != null)
        {
            EventRedirect.Invoke(IsToggled); 
        }
    }
}
