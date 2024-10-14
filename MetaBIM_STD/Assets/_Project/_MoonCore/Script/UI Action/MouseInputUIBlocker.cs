using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class MouseInputUIBlocker : MonoBehaviour
{
    public static bool BlockedByUI;
    private EventTrigger eventTrigger;

    
    public bool IsConnectWithJS = false;
    public bool IsShowDebugMessage = false;
    public bool IsOnUI = false;

    private void Start()
    {
        eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger != null)
        {
            EventTrigger.Entry enterUIEntry = new EventTrigger.Entry();
            
            // Pointer Enter
            enterUIEntry.eventID = EventTriggerType.PointerEnter;
            enterUIEntry.callback.AddListener((eventData) => { EnterUI(); });
            eventTrigger.triggers.Add(enterUIEntry);

            //Pointer Exit
            EventTrigger.Entry exitUIEntry = new EventTrigger.Entry();
            exitUIEntry.eventID = EventTriggerType.PointerExit;
            exitUIEntry.callback.AddListener((eventData) => { ExitUI(); });
            eventTrigger.triggers.Add(exitUIEntry);
        }
    }


    public void OnUIOptionMenu()
    {

    }

    public void EnterUI()
    {
        BlockedByUI = true;
        IsOnUI = true;

       
        if (IsConnectWithJS)
        {
            JSCaller.OnEnablePointEvent();
        }

        if (IsShowDebugMessage)
        {
            Debug.Log("MouseInputUIBlocker: Enter UI, ");
        }
    }


    public void ExitUI()
    {
        BlockedByUI = false;
        IsOnUI = false;

        Cursor.SetCursor(ResourceHolder.Instance.DefaultCursor, Vector2.zero, CursorMode.Auto);

        if (IsConnectWithJS)
        {
            JSCaller.OnDisablePointEvent();
        }

        if (IsShowDebugMessage)
        {
            Debug.Log("MouseInputUIBlocker: Exit UI, ");
        }
    }

}