using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace MetaBIM
{
    public class UIBlock_BimViewer_ObjectSplit_PlaneItem : MonoBehaviour
    {
        public SplitPlane Item;
        public SplitingPlane LinkedObject;
        public GameObject SelectObject;
        public GameObject ShowObject;

        public int Index;

        public TextMeshProUGUI Text_PlaneName;

        public bool IsDrawPlane = true;

        public GameObject VerticalIcon;
        public GameObject HorizontalIcon;

        public void SetBlock(SplitPlane _item, SplitingPlane _LinkedObject, int _index)
        {
            Item = _item;
            LinkedObject = _LinkedObject;
            Index = _index;
            Text_PlaneName.text = Item.planeName;
            SelectObject.SetActive(false);

            OnSetPlaneDrawable();

            if(Item.planeType == SplitPlane.PlaneType.vertical)
            {
                VerticalIcon.SetActive(true);
                HorizontalIcon.SetActive(false);
            }
            else
            {
                VerticalIcon.SetActive(false);
                HorizontalIcon.SetActive(true);
            }
        }


        public void OnSelect()
        {
            SelectObject.SetActive(true);
        }

        public void OnDeselect()
        {
            SelectObject.SetActive(false);
        }


        public void OnSetPlaneDrawable()
        {
            ShowObject.SetActive(LinkedObject.isDrawing);
            IsDrawPlane = LinkedObject.isDrawing;
        }
    }
}
