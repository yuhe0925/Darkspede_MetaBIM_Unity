using MetaBIM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;

public class UIBlock_Project_ModelItem : MonoBehaviour
{
    public Project Item;
    public string ItemSelectedVersionURLbase;
    public GameObject SelectionObject;
 
    public TextMeshProUGUI Text_Name;
    public TextMeshProUGUI Text_Date;
    public TextMeshProUGUI Text_Detial;

    public float ProgressBarRectSize = 200;
    public float currentProgress = 0f;
    public RectTransform Rect_ProgressBar;

    public UIController_Toggle Toggle_Selected;
    public TMP_Dropdown Dropdown_VersionSelection;
    public MC_GetWebIcon Preview;
    
    public bool IsVersionLoaded;
    public bool IsUpdateLoaded;
    public bool IsModelLoaded;

    public UnityEvent<Project, int> Redirect;

    // Start is called before the first frame update
    void Start()
    {
        Toggle_Selected.OnResetUI(false); 
        OnDeselected();
    }



    public void SetBlock(Project _item, bool isComplete = false)
    {

        Item = _item;

        // set general value
        Text_Name.text = Item.projectName;
        Text_Date.text = new DateTime(long.Parse(Item.updated)).ToString("yyyy-MM-dd HH:mm");
        Text_Detial.text = "Elements: " + Item.elements.Count;

        // set version list
        Dropdown_VersionSelection.options.Clear();
       
        foreach (MetaBIM.Version version in Item.versions)
        {
            Dropdown_VersionSelection.options.Add(new TMP_Dropdown.OptionData(version.versionID + ", "+version.originalFileName));
        }

        Rect_ProgressBar.localScale = new Vector3(0, 1, 1);
        // set progress value
        // check if the progress has been loaded
        // check if the progress need to be download

        if (Item.versions.Count == 0)
        {
            Text_Detial.text = "No element in model" ;
            ItemSelectedVersionURLbase = Item.projectSnaphotUrl;
        }
        else
        {
            MetaBIM.Version currentVersion = Item.versions[0];

            if (currentVersion.processingStatus == "complete")
            {
                Text_Detial.text = "Elements: " + Item.versions[0].numberOfElements;
                ItemSelectedVersionURLbase = currentVersion.GetBaseUrl() + currentVersion.guid + ".png";
            }
            else
            {
                Text_Detial.text = "Waiting to process";
                ItemSelectedVersionURLbase = Item.projectSnaphotUrl;
            }

        }

        SetModelSnapshot(ItemSelectedVersionURLbase);

        if (isComplete)
        {
            SetComplete();
        }
    }



    public void SetModelSnapshot(string _url)
    {
        Preview.SetBlock(_url);
    }
    
    
    public void OnValueChange_VersionSelect(int _value)
    {
        MetaBIM.Version version = Item.versions[_value];

        // also need to set the element number
        if (version.processingStatus == "complete")
        {
            Text_Detial.text = "Elements: " + version.numberOfElements;
            ItemSelectedVersionURLbase = version.GetBaseUrl() + version.guid + ".png";
        }
        else
        {
            Text_Detial.text = "Waiting to process";
            ItemSelectedVersionURLbase = Item.projectSnaphotUrl;
        }

        SetModelSnapshot(ItemSelectedVersionURLbase);
        Redirect.Invoke(Item, _value);
    }

    // 0-1f
    public void SetProgress(float _value, int _elementCount)
    { 
        if (_value <= 0 || _value > 1f)
        {
            return;
        }

        currentProgress = _value;

        Rect_ProgressBar.localScale = new Vector3(currentProgress, 1, 1);

        Text_Detial.text = "Loading Elements: " + _elementCount + "/"+Item.elements.Count;
    }

    // 0-1f
    public void SetLoadingProgress(float _value, string _message)
    {
        if (_value <= 0 || _value > 1f)
        {
            return;
        }

        currentProgress = _value;
        
        Rect_ProgressBar.localScale = new Vector3(currentProgress, 1, 1);

        Text_Detial.text = _message + (int)(currentProgress * 100) + "%";
    }
    
    public void SetProgressMessage(string _message)
    {
        Text_Detial.text = _message;
    }

    public bool IsToggled()
    {
        return Toggle_Selected.IsToggled;
    }

    // During Process the tree hirerachy
    public void SetHierarchy(int _value)
    {
        Text_Detial.text = "Processing hierarchy: " + _value;
    }
    
    public void SetComplete()
    {
        Rect_ProgressBar.localScale = new Vector3(1, 1, 1);
        MetaBIM.Version version = Item.versions[Dropdown_VersionSelection.value];
        Text_Detial.text = "Loaded Elements: " + version.numberOfElements;
    }


    public void SetCleared(int _element = 0)
    {
        Rect_ProgressBar.localScale = new Vector3(0, 1, 1);
        MetaBIM.Version version = Item.versions[Dropdown_VersionSelection.value];
        Text_Detial.text = "Elements: " + version.numberOfElements;
    }


    public void OnSelected()
    {
        if (SelectionObject != null)
        {
            SelectionObject.SetActive(true);
        }
    }

    public void OnDeselected()
    {
        if (SelectionObject != null)
        {
            SelectionObject.SetActive(false);
        }
    }


    
}
