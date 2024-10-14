using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Threading.Tasks;
using System.IO;

public class AppThemeEditor : MonoBehaviour
{


    [MenuItem("MC UIFrame/Theme/Reload Theme")]
    static void CollectAllItems()
    {
        Debug.Log("Reload Theme");
        ResourceHolder.GetInstance();

        SetThemeColorToText[] obs = GameObject.FindObjectsOfType<SetThemeColorToText>(true);

        foreach (SetThemeColorToText item in obs)
        {
            item.SetToThemeColor();
        }

        SetThemeColor[] obsImage = GameObject.FindObjectsOfType<SetThemeColor>(true);

        foreach (SetThemeColor item in obsImage)
        {
            item.SetToThemeColor();
        }

    }


}
