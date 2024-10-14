using IfcToolkit.IfcSpec;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BIMViewerElementHandler : MonoBehaviour
{
    public static BIMViewerElementHandler Instance;

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }


    [Header("Widget Element")]

    public List<WidgetElement> WidgetElement;

    public List<WidgetElement> WidgetElement_SharedView;



    public void OnSetViewToGeneralView()
    {
        foreach (var item in WidgetElement)
        {
            item.ElementObject.SetActive(true);
        }
    }
    

    public void OnSetViewToSharedView()
    {
        foreach (var item in WidgetElement_SharedView)
        {
            item.ElementObject.SetActive(false);
        }
    }



    public void SetWidget(string _widgetName, bool _enable = true)
    {
        foreach(var item in WidgetElement)
        {
            if(item.WidgetName == _widgetName)
            {
                item.ElementObject.SetActive(true);
            }
        }
    }
    
}


[Serializable]
public class WidgetElement
{
    public string WidgetName;
    public GameObject ElementObject;



}
