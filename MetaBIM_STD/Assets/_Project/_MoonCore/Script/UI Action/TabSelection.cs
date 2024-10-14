using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TabSelection : MonoBehaviour
{
    EventSystem system;
    // Start is called before the first frame update

    public int ItemIndex = 0;
    public List<InputField> NavigationItems;

    void Start()
    {
        system = EventSystem.current;
    }

    private void OnEnable()
    {
        ItemIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (ItemIndex + 1 < NavigationItems.Count)
            {
                ItemIndex = ItemIndex + 1;
            }
            else
            {
                ItemIndex = 0;
            }

            InputField inputfield = NavigationItems[ItemIndex];

            if (inputfield != null)
                inputfield.OnPointerClick(new PointerEventData(system));  //if it's an input field, also set the text caret

            system.SetSelectedGameObject(inputfield.gameObject, new BaseEventData(system));

        }
    }
}
