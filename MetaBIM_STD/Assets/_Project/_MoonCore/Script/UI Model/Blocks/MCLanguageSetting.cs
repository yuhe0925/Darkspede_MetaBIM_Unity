using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBlock_LanguageSetting : MonoBehaviour
{
    public static UIBlock_LanguageSetting Instance;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    public void SetBlock()
    {

    }


}
