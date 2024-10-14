using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScrollViewItem
{
    
    public void SetItem();
    
    public void OnItemSelect();
    
    public void OnItemDeselect();

    public void OnItemReset();
    
    
}
