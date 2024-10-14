using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PanelChange : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public enum ChangeType
    {
        zoom,
        fade,
        slideHor,
        slideVert,
        noAnimation,
    }

    public ChangeType Type = PanelChange.ChangeType.fade;
    public GameObject UI_Block;
    public CanvasGroup CanvasGroup;

    public bool IsChanging = false;
    public float LeanFactor = 0.1f;

    public Action OnOpenAction;
    public Action OnCloseAction;

    public bool IsCloseOnStart = true;
    public bool IsAutoClose = false;
    public float CloseDelay = 10f;
    public bool IsPanelDragable = false;

    public bool IsOpened;

    [Header("Slider H")]
    public float OnX;
    public float OffX;

    [Header("Slider V")]
    public float OnY;
    public float OffY;


    [Header("Panel Drag")]
    [SerializeField] private Vector2 originalLocalPointerPosition;
    [SerializeField] private Vector3 originalPanelLocalPosition;
    [SerializeField] private RectTransform panelRectTransform;
    [SerializeField] private RectTransform parentRectTransform;

    public UnityEvent OnDragRedirectAction;


    void Awake()
    {
        if (panelRectTransform == null)
        {
            panelRectTransform = UI_Block.GetComponent<RectTransform>();
        }

        if (parentRectTransform == null)
        {
            parentRectTransform = panelRectTransform.parent as RectTransform;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (IsCloseOnStart)
        {
            OnPanelClose();
        }
        else
        {
            OnPanelOpen();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsChanging)
        {
            switch (Type)
            {
                case ChangeType.fade:
                    break;
                case ChangeType.zoom:
                    break;
                case ChangeType.slideHor:
                    break;
                case ChangeType.slideVert:
                    break;
                case ChangeType.noAnimation:
                    break;
                default:
                    break;
            }
        }
    }


    public void SetAction(Action _open, Action _close)
    {
        OnOpenAction = _open;
        OnCloseAction = _close;
    }


    public void OnPanelOpen()
    {
        //Debug.Log("OnPanelOpen: " + gameObject.name);
        IsOpened = true;

        switch (Type)
        {
            case ChangeType.fade:
                UI_Block.SetActive(true);
                IsChanging = true;
                LeanTween.cancel(CanvasGroup.gameObject);
                LeanTween.alphaCanvas(CanvasGroup, 1, LeanFactor).setOnComplete(() => {
                    IsChanging = false;
                });
                break;
            case ChangeType.zoom:
                UI_Block.transform.localScale = Vector3.zero;
                UI_Block.SetActive(true);
                IsChanging = true;
                LeanTween.cancel(UI_Block);
                LeanTween.scale(UI_Block, new Vector3(1,1,1), LeanFactor).setOnComplete(() => { 
                    IsChanging = false;
                });
                break;
            case ChangeType.slideHor:
                UI_Block.SetActive(true);
                IsChanging = true;
                LeanTween.cancel(UI_Block);
                LeanTween.moveX(UI_Block.GetComponent<RectTransform>(), OnX, LeanFactor).setOnComplete(() => {
                    IsChanging = false;
                });
                break;
            case ChangeType.slideVert:
                UI_Block.SetActive(true);
                IsChanging = true;
                LeanTween.cancel(UI_Block);
                LeanTween.moveY(UI_Block.GetComponent<RectTransform>(), OnY, LeanFactor).setOnComplete(() => {
                    IsChanging = false;
                });
                UI_Block.SetActive(true);
                break;
            case ChangeType.noAnimation:
                UI_Block.SetActive(true);
                IsChanging = false;
                break;
            default:
                UI_Block.SetActive(true);
                IsChanging = false;
                break;
        }

        if(OnOpenAction != null)
        {
            OnOpenAction();
        }

        if (IsAutoClose)
        {
            StartCoroutine(AutoClosing());    
        }
    }


    public IEnumerator AutoClosing()
    {
        yield return new WaitForSeconds(CloseDelay);
        if (IsOpened)
        {
            OnPanelClose();
        }
    }


    public void OnPanelClose(float _offset = 0f)
    {
        //Debug.Log("OnPanelClose: " + gameObject.name);
        IsOpened = false;

        switch (Type)
        { 
            case ChangeType.fade:
                IsChanging = true;
                LeanTween.cancel(CanvasGroup.gameObject);
                LeanTween.alphaCanvas(CanvasGroup,0, LeanFactor + _offset).setOnComplete(()=> {
                    IsChanging = false;
                    UI_Block.SetActive(false);
                });

                break;
            case ChangeType.zoom:
                UI_Block.transform.localScale = new Vector3(1, 1, 1);
                IsChanging = true;
                LeanTween.cancel(UI_Block);
                LeanTween.scale(UI_Block, Vector3.zero, LeanFactor).setOnComplete(() => {
                    IsChanging = false;
                    UI_Block.SetActive(false);
                });

                break;
            case ChangeType.slideHor:
                IsChanging = true;
                LeanTween.cancel(UI_Block);
                LeanTween.moveX(UI_Block.GetComponent<RectTransform>(), OffX, LeanFactor).setOnComplete(() => {
                    IsChanging = false;
                    UI_Block.SetActive(false);
                });

                break;
            case ChangeType.slideVert:
                IsChanging = true;
                LeanTween.cancel(UI_Block);
                LeanTween.moveY(UI_Block.GetComponent<RectTransform>(), OffY, LeanFactor).setOnComplete(() => {
                    IsChanging = false;
                    UI_Block.SetActive(false);
                });

                break;
            case ChangeType.noAnimation:
                IsChanging = false;
                UI_Block.SetActive(false);
                
                break;
            default:
                IsChanging = false;
                UI_Block.SetActive(false);

                break;
        }


        if (OnCloseAction != null)
        {
            OnCloseAction();
        }
    }




    /* Draw Interaction */
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsPanelDragable)
        {
            return;
        }

        originalPanelLocalPosition = panelRectTransform.localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, eventData.position, eventData.pressEventCamera, out originalLocalPointerPosition);

        if (OnDragRedirectAction != null)
        {
            OnDragRedirectAction.Invoke();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsPanelDragable)
        {
            return;
        }
        
        if (panelRectTransform == null || parentRectTransform == null)
            return;

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition))
        {
            Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
            panelRectTransform.localPosition = originalPanelLocalPosition + offsetToOriginal;
        }

        ClampToWindow();
    }


    // Clamp panel to area of parent
    void ClampToWindow()
    {
        Vector3 pos = panelRectTransform.localPosition;

        Vector3 minPosition = parentRectTransform.rect.min - panelRectTransform.rect.min;
        Vector3 maxPosition = parentRectTransform.rect.max - panelRectTransform.rect.max;

        pos.x = Mathf.Clamp(panelRectTransform.localPosition.x, minPosition.x, maxPosition.x);
        pos.y = Mathf.Clamp(panelRectTransform.localPosition.y, minPosition.y, maxPosition.y);

        panelRectTransform.localPosition = pos;
    }
}
