using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MCMenuHandler : MonoBehaviour
{

    #region Instance 
    public static MCMenuHandler Instance;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    
    }


    static void GetInstance()
    {
        if (Instance == null)
        {
            Instance = GameObject.Find("MC_UIFrame_Editor").GetComponent<MCMenuHandler>();
        }
    }

    #endregion



    static void InstantiatePrafab(string _PrafabName)
    {
        //GetInstance();
        Object resource = Resources.Load(_PrafabName, typeof(GameObject));

        if (resource == null)
        {
            Debug.Log("MCMenuHandler.InstantiatePrafab: No Prefab [" + _PrafabName + "] Found!");
            return;
        }

        GameObject ob = (GameObject)PrefabUtility.InstantiatePrefab(resource);
        ob.transform.parent = Selection.activeTransform;
        PrefabUtility.UnpackPrefabInstance(ob, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        Debug.Log("MCMenuHandler.InstantiatePrafab: Prefab [" + _PrafabName + "] Added!");
        return;

    }


    #region General Items


    [MenuItem("GameObject/MC UIFrame/UI/MCPanel")]
    static void CreateMCItem_MCPanel()
    {
        InstantiatePrafab("MCPanel");
    }

    [MenuItem("GameObject/MC UIFrame/UI/MCLabel")]
    static void CreateMCItem_MCLabel()
    {
        InstantiatePrafab("MCLabel");
    }

    [MenuItem("GameObject/MC UIFrame/UI/MCLabelText")]
    static void CreateMCItem_MCLabelText()
    {
        InstantiatePrafab("MCLabelText");
    }

    [MenuItem("GameObject/MC UIFrame/UI/MCSprite")]
    static void CreateMCItem_MCSprite()
    {
        InstantiatePrafab("MCSprite");
    }

    [MenuItem("GameObject/MC UIFrame/UI/MCWebImage")]
    static void CreateMCItem_MCWebImage()
    {
        InstantiatePrafab("MCWebImage");
    }



    #endregion
}
