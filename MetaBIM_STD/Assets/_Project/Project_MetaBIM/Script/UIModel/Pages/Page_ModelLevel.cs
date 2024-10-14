using MetaBIM;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Page_ModelLevel : MonoBehaviour
{
    public static Page_ModelLevel Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        Panel.OnOpenAction = OnOpenAction;
        Panel.OnCloseAction = OnCloseAction;
    }


    public PanelChange Panel;
    public bool IsPageOpend;

    public void OnOpenAction()
    {
        IsPageOpend = true;

    }


    public void OnCloseAction()
    {
        IsPageOpend = false;
        //ZoneBoxController.Instance.StopLevelRender();
    }


    private void Start()
    {
        LevelItemPrefab.SetActive(false);
    }


    public void OpenPanel(List<BimLevel> _levels)
    {
        Panel.OnPanelOpen();
        OnRenderLevelItems(_levels);
    }

    public void ClosePanel()
    {
        Panel.OnPanelClose();
    }



    // ================================


    public GameObject LevelItemPrefab;
    public Transform LevelItemParent;
    public List<UIBlock_BimViewer_LevelItem> LevelItems;

    public List<BimLevel> levels;

    public void OnRenderLevelItems(List<BimLevel> _levels)
    {
        levels = _levels;

        DisplayItems();
    }


    public void DisplayItems()
    {
        OnClearList();

        foreach(var item in levels) 
        {
            GameObject ob = Instantiate(LevelItemPrefab, LevelItemParent);
            ob.SetActive(true);
            UIBlock_BimViewer_LevelItem _item = ob.GetComponent<UIBlock_BimViewer_LevelItem>();
            _item.SetBlock(item);
            LevelItems.Add(_item);
        }

        RecalucalteHeight();

        // if zone is opened; this is may not be a good way
        if (Page_BIMViewer.Instance.Panel_ZoneWidget.IsOpened)
        {
            ZoneBoxController.Instance.RenderLevel();
        }
    }



    public void OnClearList()
    {
        if(LevelItems.Count >0)
        {
            foreach(var item in LevelItems)
            {
                Destroy(item.gameObject);
            }

            LevelItems.Clear();
        }
    }


    public void OnValueChange_SetOffet(UIBlock_BimViewer_LevelItem _item)
    {
        int offset = 0;
        string value = _item.Input_LevelOffset.text;

        int.TryParse(value, out offset);

        _item.Item.LevelOffset = offset;

        RecalucalteHeight();
    }

    public void OnClick_Reset(UIBlock_BimViewer_LevelItem _item)
    {
        _item.Item.LevelOffset = 0;
        RecalucalteHeight();
    }


    public void RecalucalteHeight()
    {
        // set min and max height

        // set bounds

        // set level visualizer

        float arcmetitedOffset = 0;

        for(int i = levels.Count - 1; i >=  0; i--)
        {            
            BimLevel level = levels[i];
            arcmetitedOffset += level.LevelOffset;
            level.LevelCurrentHeight = level.LevelHeightMin + arcmetitedOffset;

            if(i + 1 < levels.Count - 1)
            {
                levels[i + 1].LevelHeightMax = levels[i].LevelHeightMin;
            }

        }

        foreach (var item in LevelItems)
        {
            item.SetBlock();
        }


    }
}
