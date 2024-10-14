using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class UIController_RightClickMenuUIBlock : MonoBehaviour
{
    public static UIController_RightClickMenuUIBlock Instance;
    public bool IsOnUI = false;
    
    private EventTrigger eventTrigger;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }


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
    public void EnterUI()
    {
        IsOnUI = true;
    }


    public void ExitUI()
    {
        IsOnUI = false;
    }

}
