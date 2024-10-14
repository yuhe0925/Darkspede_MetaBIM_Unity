using MetaBIM;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class SearchHandler : MonoBehaviour
{

    public static SearchHandler Instance;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        MainPanel.OnOpenAction = OnOpenAction;
        MainPanel.OnCloseAction = OnCloseAction;
    }



    [Header("Search Configuration")]
    public SavedAdvancedSearchFilter CurrentSearchSet;
    public List<GameObject> SearchResults = new List<GameObject>();
    public SearchPropertyItemAdapter SearchSetAdapter;

    [Header("Search Panel")]
    public PanelChange MainPanel;
    public TMP_Dropdown Dropdown_SearchModel;
    public TMP_InputField InputField_SearchText;



    [Header("Data Buffer")]
    public Dictionary<string, string> ModelList = new Dictionary<string, string>();

    public void OnOpenAction()
    {
        OnRenderList();
        SearchSetHandler.Instance.MainPanel.OnPanelClose();

    
    }


    public void OnCloseAction()
    {

    }


    public void OnClick_OpenAdvanceSearchFilter()
    {
        MainPanel.OnPanelOpen();
    }


    public void OnClick_CloseAdvanceSearchFilter()
    {
        MainPanel.OnPanelClose();
    }




    public void OnRenderList(SavedAdvancedSearchFilter _filter = null)
    {
        // get the current project guid
        ModelList = ProjectModelHandler.Instance.GetActiveModelNames();
        DataSet.ModelProperties = ProjectModelHandler.Instance.GetActiveModelPropertyList();
        Dropdown_SearchModel.ClearOptions();
        Dropdown_SearchModel.AddOptions(ModelList.Values.ToList());



        if (_filter == null)
        {
            ResetCurrentSearchSet(); // this will setitem of view(OSA)

        }
        else
        {
            // load search set from the filter
            CurrentSearchSet = _filter;
            SearchSetAdapter.SetItems(CurrentSearchSet.SearchSets);
        }
    }



    public void OnClick_AddNewProperty()
    {
        CurrentSearchSet.SearchSets.Add(new ElementSearchSet());

        SearchSetAdapter.SetItems(CurrentSearchSet.SearchSets);
    }

    public void OnClick_RemoveProperty(GameObject _gameObject)
    {
        var item = _gameObject.GetComponent<UIBlock_BimViewer_SearchHandler_PropertyItem>().Item;

        if(CurrentSearchSet.SearchSets.Count < 2)
        {
            MCPopup.Instance.SetWarning("At least one search filter should be kept.");
            return;
        }

        Debug.Log("Remove search filter: " + item.guid);
        CurrentSearchSet.SearchSets.Remove(item);
        SearchSetAdapter.SetItems(CurrentSearchSet.SearchSets);
        MCPopup.Instance.SetInformation("Search filter removed.");

    }

    public void OnClick_Reset()
    {
        MCPopup.Instance.SetConfirm(OnSearchSetReset_Confirm, "", "Reset current search filters?");
    }


    public void OnSearchSetReset_Confirm(bool _result, string _message)
    {
        if (_result)
        {
            ResetCurrentSearchSet();
        }
    }

    public void ResetCurrentSearchSet()
    {
        
        string selectedModel = ModelList.Keys.ToList()[Dropdown_SearchModel.value];
        string guid = CurrentSearchSet.guid;

        CurrentSearchSet = new SavedAdvancedSearchFilter(selectedModel);
        CurrentSearchSet.filterContent = InputField_SearchText.text;

        Debug.Log("Reset search set: filter = " + CurrentSearchSet.SearchSets.Count);
        SearchSetAdapter.SetItems(CurrentSearchSet.SearchSets);

    }


    public void OnClick_OpenSearchSets()
    {
        if (!SearchSetHandler.Instance.MainPanel.IsOpened)
        {
            SearchSetHandler.Instance.OnClick_Open();
        }
    }


    public void OnClick_AdvanceSearchFilter_Submit()
    {
        // get the text from search field
        var SearchText = InputField_SearchText.text;
        CurrentSearchSet.filterContent = SearchText;
        ProjectConfiguration.Instance.IsDisplaySearchResult = false;

        if (SearchText == "" && CurrentSearchSet.SearchSets.Count == 0)
        {
            MCPopup.Instance.SetWarning("Nothing to search");
            return;
        }

        SearchResults.Clear();
        

        foreach(var bim in ProjectModelHandler.Instance.GetActiveModels())
        {
            foreach(var node in bim.Structures)
            {
                if(OnProcessSearchingElement(node, CurrentSearchSet))
                {
                    node.IsSearchMatched = 1;
                    SearchResults.Add(node.linkedObject);
                }
                else
                {
                    node.IsSearchMatched = 0;
                }
            }
        }



        string resultString = ""; 

        if(SearchResults.Count > 0)
        {
            ProjectConfiguration.Instance.IsDisplaySearchResult = true;
            ProjectModelHandler.Instance.SelectedElements = SearchResults;
            Page_BIMViewer.Instance.SelectMeshObjects(ProjectModelHandler.Instance.SelectedElements);
            Page_BIMViewer.Instance.ProcessTreeView();  //??
            if(SearchResults.Count == 1)
            {
                resultString = SearchResults.Count + " element is selected.";
            }
            else
            {
                resultString = SearchResults.Count + " elements are selected.";
            }
        }
        else
        {
            resultString = "No element is selected.";
        }



        MCPopup.Instance.SetInformation(resultString);

    }

    public void OnClick_AdvanceSearchFilter_SendQuery()
    {

    }


    public void OnValueChange_()
    {

    }



    /* Search from the tree */

    public void OnRequestSearchList(List<StructureNode> _rootNodes) 
    {
        SearchResults.Clear();

        foreach (var root in _rootNodes)
        {
            int result = OnProcessList(root,1);


            //root.childrenNodes.Where<x => OnProcessSearchingElement(x) == >
        }
    }


    public int OnProcessList(StructureNode _rootNode, int _Level = 0)
    {
        int _anyChildMatch = 0;
        int _childCount = 0;

        foreach (StructureNode item in _rootNode.childrenNodes)
        {
            item.IsSearchMatched = -1;

            var matched = OnProcessSearchingElement(item, null);


            if (matched)
            {
                _rootNode.IsCollapsed = false;
                _anyChildMatch++;
                item.IsSearchMatched = 1;
            }
            else if(_rootNode.IsSearchMatched > 0) 
            {

                item.IsSearchMatched = 1;
            }
            else
            {
                item.IsSearchMatched = 0;
            }

            if (item.childrenNodes.Count > 0)
            {
                _childCount = OnProcessList(item, _Level + 1);
            }

            //if (ProjectConfiguration.Instance.IsDisplaySearchResult)
   
            //}

        }




        // if the par
        //if (!_rootNode.IsSearchMatched)
        //{
        //    // this maybe , not right
        //    if (_anyChildMatch == 0 || _childCount == 0)
        //    {
        //        _rootNode.IsSearchMatched = false;
        //    }
        //    else
        //    {
        //        _rootNode.IsSearchMatched = true;
        //    }
        //}

        return _anyChildMatch;
    }


    private bool OnProcessSearchingElement(StructureNode _node, SavedAdvancedSearchFilter _currentSet)
    {
        bool result = true;

        // a node without element, consider as a group node, that should be always shown
        if(_node.element == null)
        {
            return false;
        }

        // a element without content, consider as a group node, that should be always shown
        if (_node.element.Renderer == null)
        {
            return false;
        }


        if (_node.element.BimObject == null || _node.element.BimObject.records.Count < 1)
        {
            return false;
        }


        if (_currentSet == null)
        {
            return result;
        }


        // quick search by text
        if (_currentSet.filterContent != "")
        {
            result = OnQuickSearch(_node, _currentSet.filterContent);
            if (!result)
            {
                return false;
            }
        }


  

        if (_currentSet.SearchSets != null && _currentSet.SearchSets.Count > 0)
        {

            BimObjectRecord record = _node.element.BimObject.records[0];

            foreach (var filter in _currentSet.SearchSets)
            {
                string ifcAttributeTarget = record.ifcAttribute.Find(filter.searchTarget);
                string ifcPropertiesTarget = record.ifcProperties.Find(filter.searchTarget);

                switch (filter.condition)
                {
                    case "Is":
                        if (ifcAttributeTarget != null)
                        {
                            if (ifcAttributeTarget.ToLower() != filter.searchValue.ToLower())
                            {
                                return false;
                            }
                        }
                        else if (ifcPropertiesTarget != null)
                        {
                            if (ifcPropertiesTarget.ToLower() != filter.searchValue.ToLower())
                            {
                                return false;
                            }
                        }
                        else
                        {

                        }
                        break;
                    case "Contains":
                        if (ifcAttributeTarget != null)
                        {
                            if (!ifcAttributeTarget.ToLower().Contains(filter.searchValue.ToLower()))
                            {
                                return false;
                            }
                        }
                        else if (ifcPropertiesTarget != null)
                        {
                            if (!ifcPropertiesTarget.ToLower().Contains(filter.searchValue.ToLower()))
                            {
                                return false;
                            }
                        }
                        else
                        {

                        }
                        break;
                }
            }
        }
        else
        {
            return true;
        }



        return true;
    }


    public bool OnQuickSearch(StructureNode _node , string _content)
    {
        bool result = false;
        string targetTexts = _content.ToLower();
        string elementName = _node.element.BimObject.GetObjectName(0).ToLower();  // this using GameObject name, well.....may be not be the most correct way
        string elementID = _node.element.BimObject.elementID.ToLower();


        if (!string.IsNullOrEmpty(elementName))
        {
            if (elementName.Contains(targetTexts, StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("Result: " + elementName);
                return true;

            }

        }


        if (!string.IsNullOrEmpty(elementID))
        {
            if (elementID.Contains(targetTexts, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

    
        return false;
    }






}

