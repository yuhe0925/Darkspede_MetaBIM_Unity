using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Xml;
using UnityEngine.UI;
using MetaBIM;

public class UIBlock_BimViewer_IfcStructureItem : MonoBehaviour
{
    [Header("Block Element")]
    public StructureNode Item;
    public bool isTreeExpend = true;
    public bool isSearchResult = false;
    public float SpaceWidth = 10f;
    public float SpaceHight= 30f;

    public List<Color> BGColor;

    [Header("Block Element")]
    public RectTransform Element;   
    public RectTransform Space;
    public Image BGImage;
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



    // Start is called before the first frame update
    void Start()
    {
    
    }


    public void UpdateBlock(int _scrollItemIndex)
    {
        Item.ItemScrollIndex = _scrollItemIndex;
        Item.UILinkObject = this;
    }

    public void SetBlock(StructureNode _item, int _scrollItemIndex)
    {
        //Layout.enabled = true;
        Item = _item;
        Item.ItemScrollIndex = _scrollItemIndex;

        Text_Content.text = Item.Content == "" ? "Not Provided" : Item.Content;

        if (Item.childrenNodes.Count > 0)
        {
            StatusBlock.SetActive(true);
            //if (ProjectConfiguration.Instance.IsDisplaySearchResult)
            //{
            //    ExpendMark.SetActive(false);
            //    CollapseMark.SetActive(false);
            //}
            //else
            //{
            //    ExpendMark.SetActive(Item.IsCollapsed);
            //    CollapseMark.SetActive(!Item.IsCollapsed);
            //}

            ExpendMark.SetActive(Item.IsCollapsed);
            CollapseMark.SetActive(!Item.IsCollapsed);


            if (ProjectConfiguration.Instance.IsDisplaySearchResult) 
            {
                int count = 0;
                foreach(var item in Item.childrenNodes)
                {
                    if (item.IsSearchMatched > 0)
                    {
                        count++;
                    }
                }

                Text_ItemCount.text = count.ToString();
            }
            else
            {

                Text_ItemCount.text = Item.childrenNodes.Count.ToString();
            }

        }
        else
        {
            StatusBlock.SetActive(false);
            ExpendMark.SetActive(false);
            CollapseMark.SetActive(false);
        }

        //if (ProjectConfiguration.Instance.IsDisplaySearchResult)
        //{
        //    Space.sizeDelta = new Vector2(SpaceWidth * 1, Space.sizeDelta.y);
        //}
        //else
        //{
        //    Space.sizeDelta = new Vector2(SpaceWidth * Item.nodeDepth, Space.sizeDelta.y);
        //}

        Space.sizeDelta = new Vector2(SpaceWidth * Item.nodeDepth, Space.sizeDelta.y);

        Item.UILinkObject = this;

  


        if (Item.IsHided)
        {
            OnHideObject();
        }
        else
        {
            OnUnhideObject();
        }
        /*
        if (Item.element != null)
        {
            OnHideObject(Item.element.IsElementHide);
        }
        else
        {
            OnHideObject(Item.IsHided);
        }
        */


        BGImage.color = BGColor[Item.ColorCode];


        // set layer type icon

        if (!Item.IsGeometry)
        {
            Text_Type.text = "";
        }
        else
        {
            if(Item.childrenNodes.Count > 0)
            {
                Text_Type.text = "";
            }
            else
            {
                Text_Type.text = "";
            }
        }

        SelectedEffect.SetActive(false);
        //Layout.enabled = false;
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


    /// <summary>
    /// Just the tick
    /// </summary>
    /// <param name="_status"></param>
    public void OnHideObject()
    {
        //Debug.Log("OnHideObject: " + Item.Content);
        HideItemChick.SetActive(false);
    }


    public void OnUnhideObject()
    {
        //Debug.Log("OnUnhideObject: " + Item.Content);
        HideItemChick.SetActive(true);
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
