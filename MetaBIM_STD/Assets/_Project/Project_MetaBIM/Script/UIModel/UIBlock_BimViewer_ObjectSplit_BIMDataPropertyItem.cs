using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


namespace MetaBIM
{
    public class UIBlock_BimViewer_ObjectSplit_BIMDataPropertyItem : MonoBehaviour
    {
        public BIMExportPropertyItem Item;
        public GameObject SelectObject;
        public GameObject ShowObject;
        public GameObject Icon;
        public Image BgImage;
        public int Index;

        public Color ParentItemColor;

        public TextMeshProUGUI Text_Name;

        

        public bool IsSelected = true;

        public void SetBlock(BIMExportPropertyItem _item, int _index)
        {
            Item = _item;
            Index = _index;
            Text_Name.text = Item.PropertyName;
            SelectObject.SetActive(false);

            OnSet();

            Icon.SetActive(!Item.IsSubItem);

            BgImage.color = Item.IsSubItem ? Color.white: ParentItemColor;
        }


        public void OnSelect()
        {
            SelectObject.SetActive(true);
        }

        public void OnDeselect()
        {
            SelectObject.SetActive(false);
        }


        public void OnSet()
        {
            if (Item.IsExport)
            {
                ShowObject.SetActive(true);
            }
            else
            {
                ShowObject.SetActive(false);
            }
        }
    }
}
