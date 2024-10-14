using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface UIBlock_ScrollViewItem<T>
{
    public void SetBlock(T model);

    public void OnItemSelect();

    public void OnItemDeselect();

    public void OnItemReset();

}
