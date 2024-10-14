using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIController_SideTabButtonGroup : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Button> TabButtonGroup;
    public List<Vector2> TabButtonPosition;

    public ScrollRect ScrollView_TabSilder;

    public void OnClick_SelectSideButton(int _index)
    {
        if(_index < TabButtonGroup.Count)
        {
            TabButtonGroup[_index].onClick.Invoke();
            ScrollView_TabSilder.normalizedPosition = TabButtonPosition[_index];
        }
    }



    public void OnValueChange_TabSilderPosition(Vector2 _vector)
    {
        //Debug.Log("OnValueChange_TabSilderPosition: " + _vector);
    }
}
