using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Xml;
using UnityEngine.UI;
using MetaBIM;

public class UIBlock_BimViewer_ClassificationItem_Uniclass : MonoBehaviour
{
    [Header("Block Element")]
    public Uniclass Item;
    public bool isTreeExpend = true;
    public bool isSearchResult = false;
    public float SpaceWidth = 10f;
    public float SpaceHight= 30f;

    [Header("Block Element")]
    public RectTransform Element;   
    public RectTransform Space;   
    public GameObject ExpendMark;
    public GameObject CollapseMark;
    public GameObject HideItemChick;
    public GameObject SelectedEffect;
    public TextMeshProUGUI Text_Icon;
    public TextMeshProUGUI Text_Content;
    public GameObject StatusBlock;
    public TextMeshProUGUI Text_ItemCount;
    public HorizontalLayoutGroup Layout;

    [Header("Block Type")]
    public TextMeshProUGUI Text_Type;




    public void SetBlock(Uniclass _item)
    {
        //Layout.enabled = true;
        Item = _item;


        if (_item.level == 0)
        {
            Text_Content.text = Item.documentName;
        }
        else
        {
            Text_Content.text = Item.Code + ": " + Item.Title;
        }


        if (Item.children.Count > 0)
        {
            if (Page_ClassificationSelector.Instance.IsDisplaySearchResult)
            {
                ExpendMark.SetActive(false);
                CollapseMark.SetActive(false);
            }
            else
            {
                ExpendMark.SetActive(Item.IsCollapsed);
                CollapseMark.SetActive(!Item.IsCollapsed);
            }

            int count = 0;
            foreach (var child in Item.children)
            {
                if (child.IsSearched)
                {
                    count++;
                }
            }

            if (count > 0)
            {
                Text_Type.text = "";
                StatusBlock.SetActive(true);
                Text_ItemCount.text = Item.children.Count.ToString();

                if (_item.level == 3)
                {
                    ExpendMark.SetActive(false);
                    CollapseMark.SetActive(false);
                }
            }
            else
            {
                Text_Type.text = "";
                StatusBlock.SetActive(false);
                ExpendMark.SetActive(false);
                CollapseMark.SetActive(false);
            }

        }
        else
        {
            Text_Type.text = "";
            StatusBlock.SetActive(false);
            ExpendMark.SetActive(false);
            CollapseMark.SetActive(false);
        }


        if (Page_ClassificationSelector.Instance.IsDisplaySearchResult)
        {
            Space.sizeDelta = new Vector2(SpaceWidth * 1, Space.sizeDelta.y);
        }
        else
        {
            Space.sizeDelta = new Vector2(SpaceWidth * Item.level, Space.sizeDelta.y);
        }

        SelectedEffect.SetActive(false);
        OnDeselect();


    }

    public void OnExpendTree()
    {
        isTreeExpend = true;
        ExpendMark.SetActive(false);
        CollapseMark.SetActive(true);
        // Open the tree
    }

    public void OnCollapseTree()
    {
        isTreeExpend = false;
        ExpendMark.SetActive(true);
        CollapseMark.SetActive(false);
    }


    public void OnHideObject(bool _status)
    {
        HideItemChick.SetActive(!_status);
    }


    public void OnSelect()
    {
        SelectedEffect.SetActive(true);
    }

    public void OnDeselect()
    {
        SelectedEffect.SetActive(false);
    }






}
