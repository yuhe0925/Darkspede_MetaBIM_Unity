using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MetaBIM
{
    public class UIBlock_BimViewer_ObjectSplit_SubElementItem : MonoBehaviour
    {
        
        public StructureNode Item;
        public GameObject LinkedObject;
        public int Index;
        public TextMeshProUGUI Text_PlaneName;

        public GameObject SelectObject;
        public GameObject ShowObject;

        public bool IsShowElement = true;

        public void SetBlock(StructureNode _item, int _scrollItemIndex)
        {
            Item = _item;
            Index = _scrollItemIndex;

            Text_PlaneName.text = Item.Content;

            OnDeselected();

            OnSetShow();
        }


        public void OnSelected()
        {
            SelectObject.SetActive(true);
        }

        public void OnDeselected()
        {
            SelectObject.SetActive(false);
        }

        public void OnSetShow()
        {
            ShowObject.SetActive(!Item.element.IsElementHide);
            IsShowElement = !Item.element.IsElementHide;
        }
    }
}
