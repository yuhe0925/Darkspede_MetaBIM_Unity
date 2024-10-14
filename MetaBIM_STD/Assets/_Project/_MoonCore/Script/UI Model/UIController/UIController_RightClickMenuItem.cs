using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIController_RightClickMenuItem : MonoBehaviour
{
    public static bool IsOnUI;
    
    public int buttonId;
    public MenuClassType buttonType;
    public string ButtonIcon;
    public string ButtonName;

    public GameObject Parent;

    [Header("Items")]
    public UnityEvent<GameObject> ButtonAction;


    public enum MenuClassType
    {
        // general
        copy,
        paste,
        getelementID,

        // targeted object
        objectisolate,
        objecthide,
 

        // camera
        objectfocus,
        resetview,

        // UI - Tree
        expend,
        collapse,
        itemfocus,
        // Split
        split,
        // UI - Tree - custom node
        addnode,
        removenode,
        renamenode,
        addelementtonode,
        removeelementfromnode,

        // Zone
        addToZone,
        removeFromZone,
        resetZone,

        // selection
        unselect,
        unhide,
        cancelIsolation,


        // misc
        reserved, // this is for the button that is not in use

          

    }


    public void EnterUI()
    {
        IsOnUI = true;

    }


    public void ExitUI()
    {
        IsOnUI = false;
    }

}
