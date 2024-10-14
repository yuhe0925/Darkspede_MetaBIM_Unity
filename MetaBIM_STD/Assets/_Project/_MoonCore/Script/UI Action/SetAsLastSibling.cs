using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetAsLastSibling : MonoBehaviour, IPointerDownHandler
{
    public Transform target;

    public void OnPointerDown(PointerEventData eventData)
    {
        target.SetAsLastSibling();
    }
}
