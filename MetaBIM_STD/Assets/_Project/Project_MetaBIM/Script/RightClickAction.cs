
 using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
 
 
 public class RightClickAction : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent<UIBlock_BimViewer_IfcStructureItem> RedirectAction;
    public UIBlock_BimViewer_IfcStructureItem Item;
    
    private GameObject ClickedObject;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ClickedObject = eventData.pointerCurrentRaycast.gameObject;
            
            if(ClickedObject != null)
            {              
                if(RedirectAction != null)
                {
                    RedirectAction.Invoke(Item);
                }
            }
        }
    }

}