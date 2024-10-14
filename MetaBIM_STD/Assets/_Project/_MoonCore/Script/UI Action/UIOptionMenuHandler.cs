using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIOptionMenuHandler : MonoBehaviour, IPointerClickHandler
{
    public PanelChange Panel_OptionMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (MouseInputUIBlocker.BlockedByUI)
        {
            return;
        }


        if (Input.GetKeyUp(KeyCode.Mouse1))
        {

        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Destroys item from players inventory when pressed
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (MouseInputUIBlocker.BlockedByUI)
            {
                return;
            }
            else
            {

            }
        }
    }
}
