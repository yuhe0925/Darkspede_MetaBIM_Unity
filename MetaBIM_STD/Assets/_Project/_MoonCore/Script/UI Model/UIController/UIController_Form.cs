using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController_Form : MonoBehaviour
{
    public List<UIController_InputField> InputFields;



    public bool GetFormStatus()
    {
        foreach(UIController_InputField input in InputFields)
        {
            if (!input.IsValidated)
            {
                return false;
            }
        }

        return true;
    }
}
