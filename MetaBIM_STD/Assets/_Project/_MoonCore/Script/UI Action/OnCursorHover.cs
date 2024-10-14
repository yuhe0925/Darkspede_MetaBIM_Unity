using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnCursorHover : MonoBehaviour, IPointerExitHandler, IPointerClickHandler, IPointerEnterHandler
{
    public enum HoverType
    {
        Default,
        Button,
        Input,
        Copy,
        DragHorizontal,
        DragVertical,
        Drag,
    }


    [Header("Hover Cursor Type")]
    public GameObject OnHoverActionObject;
    public TextMeshProUGUI Text_HoverTip;
    public string Tooltip_Text;
    public bool IfReversed = false;
    public HoverType Type = HoverType.Default;

    Texture2D HoverCursor;
    Texture2D DefaultCursor;


    [Header("Cursor Tooptip")]
    CursorFloating TooltipCursor;
    public string Tooltip;
    public float Counter;
    public bool IsTriggered;
    public bool IsAppeared;
    public float AppearDelay = 0.5f;


    private Vector2 CursorHotSpot;
    // Start is called before the first frame update
    void Start()
    {
        if (TooltipCursor == null)
        {
            //TooltipCursor = GameObject.FindGameObjectWithTag("Tooltip").GetComponent<CursorFloating>();
        }

        if (OnHoverActionObject != null)
        {
            OnHoverActionObject.SetActive(IfReversed);
        }
        CursorHotSpot = Vector2.zero;

        switch (Type)
        {
            case HoverType.Button:
                HoverCursor = ResourceHolder.Instance.HoverButtonCursor;
                break;
            case HoverType.Input:
                HoverCursor = ResourceHolder.Instance.HoverInputCursor;
                break;
            case HoverType.Copy:
                HoverCursor = ResourceHolder.Instance.CopyCursor;
                break;
            case HoverType.DragHorizontal:
                HoverCursor = ResourceHolder.Instance.DragHorizontalCursor;
                CursorHotSpot = new Vector2(HoverCursor.width / 2, HoverCursor.height / 2);
                break;
            case HoverType.DragVertical:
                HoverCursor = ResourceHolder.Instance.DragVerticalCursor;
                CursorHotSpot = new Vector2(HoverCursor.width / 2, HoverCursor.height / 2);
                break;
            case HoverType.Drag:
                HoverCursor = ResourceHolder.Instance.DragCursor;
                CursorHotSpot = new Vector2(HoverCursor.width / 2, HoverCursor.height / 2);
                break;
            default:
                HoverCursor = ResourceHolder.Instance.DefaultCursor;
                break;

        }

        DefaultCursor = ResourceHolder.Instance.DefaultCursor;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnHoverActionObject != null)
        {
            OnHoverActionObject.SetActive(!IfReversed);
        }

        if (Text_HoverTip != null)
        {
            Text_HoverTip.text = Tooltip_Text;
        }


        Cursor.SetCursor(HoverCursor, CursorHotSpot, CursorMode.Auto);

        if (Tooltip != "")
        {
            IsTriggered = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnHoverActionObject != null)
        {
            OnHoverActionObject.SetActive(IfReversed);
        }

        if (Text_HoverTip != null)
        {
            Text_HoverTip.text = "";
        }

        Cursor.SetCursor(DefaultCursor, Vector2.zero, CursorMode.Auto);

        IsTriggered = false;
        IsAppeared = false;
        Counter = 0;
        //TooltipCursor.OnHintFade();

    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }
}
