using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController_Tab : MonoBehaviour
{

    public List<UIController_TabItem> Tabs;
    private UIController_TabItem SelectedTab;
    public int DefaultTabIndex = 0;
    public ScrollRect scrollRect;
    public RectTransform contentPanel;

    [Header("Direct event")]
    public UnityEvent<string> Event; 

    // Start is called before the first frame update
    void Start()
    {
   
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetDefault()
    {
        OnClick_SelectTabItem(Tabs[DefaultTabIndex]);
    }

    public void SetTab(string _key = "")
    {
        foreach (UIController_TabItem item in Tabs)
        {
            if(item.Key == _key)
            {
                SelectedTab = item;
            }
        }

        OnClick_SelectTabItem(SelectedTab);

    }

    public void OnClick_SelectTabItem(UIController_TabItem _select)
    {
        foreach (UIController_TabItem item in Tabs)
        {
            item.Deselect();
        }

        _select.OnSelect();

        if (Event != null)
        {
            SelectedTab = _select;
            Event.Invoke(_select.Key);
        }
        else
        {
            Debug.Log("UIController_Tab.DirectEvent is not assigned");
        }
    }


    public UIController_TabItem GetTabItem(string _tabKey)
    {
        foreach (UIController_TabItem item in Tabs)
        {
            if(item.Key == _tabKey)
            {
                return item;
            }
        }

        return Tabs[0];
    }

    public void OnClick_ShiftRightorDown()
    {
        /* TODO
        Canvas.ForceUpdateCanvases();

        contentPanel.anchoredPosition =
            (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
            - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
        */
    }

    public void OnClick_ShiftLeftorUp()
    {

    }


    public void InitTab(List<string> _tabNames)
    {
        List<UIController_TabItem> newTabs = new List<UIController_TabItem>();



        foreach(var tabName in _tabNames)
        {
            GameObject gameObject = Instantiate(Tabs[0].gameObject, contentPanel);
            var item = gameObject.GetComponent<UIController_TabItem>();
            item.Key = tabName;
            item.Text_Tab.text = tabName;
            newTabs.Add(item);
        }


        // destroy existing tabs
        foreach (var tab in Tabs)
        {
            Destroy(tab.gameObject);
        }

        Tabs = newTabs;
    }




}
