using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SearchSetHandler : MonoBehaviour
{

    public static SearchSetHandler Instance;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        MainPanel.OnOpenAction = OnOpenAction;
        MainPanel.OnCloseAction = OnCloseAction;
    }

    [Header("UI Element")]

    public PanelChange MainPanel;



    [Header("Search Configuration")]

    public List<SavedAdvancedSearchFilter> SavedSets = new List<SavedAdvancedSearchFilter>(); // IO database


    public void OnOpenAction()
    {
        // load search sets from playerprefs

        //PlayerPrefs.DeleteAll();
        OnLoadSearchSets();

    }


    public void OnCloseAction()
    {

    }

    public void OnClick_Open()
    {
        MainPanel.OnPanelOpen();
    }


    public void OnClick_Close()
    {
        MainPanel.OnPanelClose();
    }





    public void OnClick_DeleteSet(GameObject _gameObject)
    {

    }




    public void OnValueChange_()
    {

    }



    public void OnLoadSearchSets()
    {
        // load search sets from playerprefs

        //
        if (!PlayerPrefs.HasKey("SearchSets"))
        {
            PlayerPrefs.SetString("SearchSets","" );
        }

        string _json = PlayerPrefs.GetString("SearchSets");

        if(_json != "")
        {
            SavedSets = SavedAdvancedSearchFilter.FromJsonList(_json);
        }
        else
        {
            SavedSets = new List<SavedAdvancedSearchFilter>();
        }
    }


    public void OnLoadSearchSets_Callback()
    {

    }


    public void OnUpdateSearchSets()
    {
        string _json = JsonConvert.SerializeObject(SavedSets);
        PlayerPrefs.SetString("SearchSets", _json);
    }

    public void OnUpdateSearchSets_Callback()
    {

    }   


    public void ClearSearchSets()
    {
        PlayerPrefs.DeleteKey("SearchSets");
    }


}



[Serializable]
public class SavedAdvancedSearchFilter:IModel
{
    public string attachedModelID;    // the model of project that is been searched
    public string filterName = "New Search Set";           // for list display

    public string filterContent = "";        // search text for title, name and id
    public List<ElementSearchSet> SearchSets = new List<ElementSearchSet>(); // IO database



    public SavedAdvancedSearchFilter(string _attachedModelID)
    {
        Debug.Log("SavedAdvancedSearchFilter: " + _attachedModelID);
        attachedModelID = _attachedModelID;
        Reset();
    }


    public void Reset()
    {
        filterName = "New Search Set";
        filterContent = "";
        SearchSets = new List<ElementSearchSet>
        {
            new ElementSearchSet()
        };

    }


    #region MISC
    public static string ToJson(SavedAdvancedSearchFilter _item)
    {
        return JsonConvert.SerializeObject(_item);
    }

    public static SavedAdvancedSearchFilter FromJson(string _json)
    {
        return JsonConvert.DeserializeObject<SavedAdvancedSearchFilter>(_json);
    }

    public static List<SavedAdvancedSearchFilter> FromJsonList(string _json)
    {
        return JsonConvert.DeserializeObject<List<SavedAdvancedSearchFilter>>(_json);
    }

    #endregion
}