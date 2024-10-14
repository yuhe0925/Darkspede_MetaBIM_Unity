using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;


namespace MetaBIM
{
    public class UIBlock_BimViewer_ZoneManagement_ZoneBoxItem : MonoBehaviour
    {
        public ElementZone Item;
        public int Index;
        public ZoneItem LinkedObject;
        public GameObject SelectObject;
        public GameObject ShowObject;
        public TextMeshProUGUI Text_BoxName;
        public TextMeshProUGUI Text_Icon;  // for icon color
        public bool IsVisable = true;  // not save in database
        

        public void SetBlock(ElementZone _item, ZoneItem _LinkedObject, int _index)
        {
            Item = _item;
            Text_BoxName.text = Item.zoneName;
            Index = _index;
            LinkedObject = _LinkedObject;   // TODO, handle null, if not linked, create new one
            LinkedObject.Index = _index;
            LinkedObject.UIBlock = this; 
            Text_Icon.color = new Color(Item.zoneColor[0], Item.zoneColor[1], Item.zoneColor[2], Item.zoneColor[3]);

            // create zoneitem from controller?
            ShowObject.SetActive(IsVisable);
            OnDeselect();
        }


        public void OnSelect()
        {
            SelectObject.SetActive(true);
        }

        public void OnDeselect()
        {
            SelectObject.SetActive(false);
        }


        public void OnSetBoxVisiable()
        {
            if (IsVisable)
            {
                IsVisable = false;
            }
            else
            {
                IsVisable = true;
            }

            ShowObject.SetActive(IsVisable);

            LinkedObject.OnSetVisiable(IsVisable);
        }
    }
}
