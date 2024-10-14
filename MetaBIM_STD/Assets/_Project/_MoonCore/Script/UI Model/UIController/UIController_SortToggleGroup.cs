using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIController_SortToggleGroup : MonoBehaviour
{
    public List<UIController_SortToggle> Toggles;
    public bool IsMultSelectionAllowed = false;

    [Header("Direct event")]
    public UnityEvent<UIController_SortToggle> Event;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void OnClick_SelectToggle(UIController_SortToggle _select)
    {
        if (_select.isSelected)
        {
            _select.OnSwitch();
        }
        else
        {
            _select.OnSelect();
        }

        // Rerender toggle image and icon for items not selected
        // not best practice

        foreach (UIController_SortToggle item in Toggles)
        {
            if (item.Key != _select.Key)
            {
                item.OnDeselect();
            }
        }


        if (Event != null)
        {
            Event.Invoke(_select);
        }
        else
        {
            Debug.Log("UIController_SortToggleGroup.DirectEvent is not assigned");
        }
    }





}
