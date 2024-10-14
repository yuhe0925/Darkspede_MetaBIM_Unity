using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MetaBIM
{
    public class UIBlock_BimViewer_ZoneManagement_ZoneSelectedItem : MonoBehaviour
    {
        
        public StructureNode Item;
        public GameObject LinkedObject;
        public int Index;
        public TextMeshProUGUI Text_SelectedElementName;

        public GameObject SelectObject;
        public GameObject ShowObject;

        public bool IsSelectedElement = true;

        public void SetBlock(StructureNode _item, int _scrollItemIndex)
        {
            Item = _item;
            Index = _scrollItemIndex;

            Text_SelectedElementName.text = Item.Content;

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
            IsSelectedElement = !Item.element.IsElementHide;
        }
    }
}
