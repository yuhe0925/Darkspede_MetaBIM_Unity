using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Events;

public class DragPanelWorld : MonoBehaviour, IPointerDownHandler, IDragHandler {

    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition;
    private Vector3 offsetToOriginal;
    [SerializeField] private RectTransform panelRectTransform;
    [SerializeField] private RectTransform parentRectTransform;
    private Vector3 parentRectTransformRectMin;
    private Vector3 parentRectTransformRectMax;

    private Vector3 panelRectTransformRectMin;
    private Vector3 panelRectTransformRectMax;
    private Vector3 CamPosition;
    private Vector3 minPosition;
    private Vector3 maxPosition;

    public UnityEvent OnDragRedirectAction;


    void Awake()
    {
        if (panelRectTransform == null)
        {
            panelRectTransform = transform.parent as RectTransform;
        }

        if (parentRectTransform == null)
        {
            parentRectTransform = panelRectTransform.parent as RectTransform;
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        originalPanelLocalPosition = panelRectTransform.localPosition;
        CamPosition = data.position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, data.position, data.pressEventCamera, out originalLocalPointerPosition);

        if (OnDragRedirectAction != null)
        {
            OnDragRedirectAction.Invoke();
        }

        parentRectTransformRectMin = parentRectTransform.rect.min;
        parentRectTransformRectMax = parentRectTransform.rect.max;
        panelRectTransformRectMin = panelRectTransform.rect.min;
        panelRectTransformRectMax = panelRectTransform.rect.max;

        minPosition = parentRectTransformRectMin - panelRectTransformRectMin;
        maxPosition = parentRectTransformRectMax - panelRectTransformRectMax;

        panelRectTransform.transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData data)
    {
        if (panelRectTransform == null || parentRectTransform == null)
            return;

        PageStatus.IsPanelDragging = true;

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, data.position, data.pressEventCamera, out localPointerPosition))
        {
            offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
            panelRectTransform.localPosition = originalPanelLocalPosition + offsetToOriginal;
        }



        ClampToWindow();
    }

    public void OnEndDrag(PointerEventData data)
    {
        PageStatus.IsPanelDragging = false;
    }

    // Clamp panel to area of parent
    void ClampToWindow()
    {
        Vector3 pos = panelRectTransform.localPosition;

        pos.x = Mathf.Clamp(panelRectTransform.localPosition.x, minPosition.x, maxPosition.x);
        pos.y = Mathf.Clamp(panelRectTransform.localPosition.y, minPosition.y, maxPosition.y);

        panelRectTransform.localPosition = pos;
    }
}
