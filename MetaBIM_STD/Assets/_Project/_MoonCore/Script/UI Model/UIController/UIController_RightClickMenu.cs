using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using UnityEngine.Events;
using System.Linq;

public class UIController_RightClickMenu : MonoBehaviour
{
    public static UIController_RightClickMenu Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public PanelChange Menu;
    public RectTransform MenuRect;
    public RectTransform MenuRect_Inner;
    public Camera CuurentCamera;

    [Header("Menu Content")]
    public Transform MenuParent;
    public List<UIController_RightClickMenuItem> MenuItems;
    public List<UIController_RightClickMenuItem> input;


    [Header("Status")]
    private Vector3 LastRightClick;
    public bool IsMenuOpend;
    public GameObject ClickTarget;
    public MenuType currentMenuType;



    public void AssignMenuAction(List<UIController_RightClickMenuItem> _MenuItems)
    {
        Menu.OnPanelOpen();
        
        // Set Contents
        foreach (UIController_RightClickMenuItem item in MenuItems)
        {
            if (_MenuItems.Contains(item))
            {
                item.gameObject.SetActive(true);
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }

        // Set Position
        // the mouse position in ui using  .|__  system 
        LastRightClick = Input.mousePosition;

        if (LastRightClick.x > Screen.width - MenuRect.sizeDelta.x)
        {
            LastRightClick.x = Screen.width - MenuRect.sizeDelta.x;
        }

        if (LastRightClick.y  < MenuRect_Inner.sizeDelta.y )
        {
            LastRightClick.y = MenuRect_Inner.sizeDelta.y;
        }

        MenuRect.position = LastRightClick;

    }

    
    public void OnMenuOpen(MenuType _type, GameObject _target = null)
    {

        input.Clear();

        // should keep the last click?
        ClickTarget = _target;
        currentMenuType = _type;
        switch (currentMenuType)
        {
            case MenuType.cameraView:

                input = MenuItems.Where(x=>
                    //x.buttonType == UIController_RightClickMenuItem.MenuClassType.getelementID ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.objectfocus ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.objectisolate ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.addToZone ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.split ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.addelementtonode ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.resetZone ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.objecthide ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.unselect ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.unhide ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.cancelIsolation 

                ).ToList();
                    
                break;
            case MenuType.bimstructure:

                input = MenuItems.Where(x =>
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.itemfocus ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.getelementID ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.objectisolate||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.objecthide ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.unselect ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.unhide ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.cancelIsolation
                ).ToList();

                break;
            case MenuType.customstructure:

                input = MenuItems.Where(x =>
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.itemfocus ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.getelementID ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.objectisolate ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.objecthide ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.renamenode ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.addnode ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.removenode ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.unselect ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.unhide ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.cancelIsolation
                ).ToList();

                break;
            case MenuType.zone:

                input = MenuItems.Where(x =>
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.addToZone ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.removeFromZone ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.resetZone ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.unselect ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.unhide ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.cancelIsolation
                ).ToList();
                break;
            case MenuType.textfield:

                input = MenuItems.Where(x =>
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.copy ||
                    x.buttonType == UIController_RightClickMenuItem.MenuClassType.paste
                ).ToList();

                break;
        }

        AssignMenuAction(input);

    }


    public void OnMenuClose()
    {

        Menu.OnPanelClose();
    }

    
    public enum MenuType
    {
        // targeted to BIM object
        cameraView,
        bimstructure,
        customstructure,
        attribute,
        zone,
        textfield,

    }

}






