using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;



public class FixScrollRect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    public ScrollRect MainScroll;
    public bool isAvaliable = true;
    public bool isHover = false;
    public bool oldInteractive = false;



    void Update()
    {
        if (oldInteractive != this.GetComponent<TMP_InputField>().interactable)
        {
            if (this.GetComponent<TMP_InputField>().interactable)
            {
                //this.GetComponent<Image>().color = this.GetComponent<TMP_InputField>().colors.normalColor;
            }
            else
            {
                //this.GetComponent<Image>().color = this.GetComponent<TMP_InputField>().colors.disabledColor;
            }



            oldInteractive = this.GetComponent<TMP_InputField>().interactable;
        }



        if (this.GetComponent<TMP_InputField>().interactable)
        {
            if (Input.GetMouseButton(0))
            {
                if (!this.isHover)
                {
                    this.GetComponent<TMP_InputField>().enabled = false;
                }
            }
        }
    }



    void Awake()
    {
        if (this.GetComponent<TMP_InputField>().interactable)
        {
            this.GetComponent<TMP_InputField>().enabled = false;
            //this.GetComponent<Image>().color = this.GetComponent<TMP_InputField>().colors.normalColor;
        }
        else
        {
            //this.GetComponent<Image>().color = this.GetComponent<TMP_InputField>().colors.disabledColor;
        }
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        MainScroll.OnBeginDrag(eventData);



        if (this.GetComponent<TMP_InputField>().interactable)
        {
            //this.GetComponent<Image>().color = this.GetComponent<TMP_InputField>().colors.normalColor;



            this.GetComponent<TMP_InputField>().enabled = false;
            this.isAvaliable = false;
        }
    }



    public void OnDrag(PointerEventData eventData)
    {
        MainScroll.OnDrag(eventData);
    }



    public void OnEndDrag(PointerEventData eventData)
    {
        MainScroll.OnEndDrag(eventData);



        if (this.GetComponent<TMP_InputField>().interactable)
        {
            this.GetComponent<TMP_InputField>().enabled = false;
            this.isAvaliable = true;
        }
    }



    public void OnScroll(PointerEventData data)
    {
        MainScroll.OnScroll(data);
    }



    public void OnPointerUp(PointerEventData eventData)
    {
        if (this.GetComponent<TMP_InputField>().interactable)
        {
            if (isAvaliable)
            {
                this.GetComponent<TMP_InputField>().enabled = true;
                this.GetComponent<TMP_InputField>().Select();
            }



            //this.GetComponent<Image>().color = this.GetComponent<TMP_InputField>().colors.normalColor;
        }
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.GetComponent<TMP_InputField>().interactable)
        {
            if (eventData.pointerEnter == this.gameObject)
            {
                //this.GetComponent<Image>().color = this.GetComponent<TMP_InputField>().colors.pressedColor;



                this.GetComponent<TMP_InputField>().enabled = false;
                this.isAvaliable = true;
            }
        }
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.GetComponent<TMP_InputField>().interactable)
        {
            //this.GetComponent<Image>().color = this.GetComponent<TMP_InputField>().colors.highlightedColor;
        }



        this.isHover = true;
    }



    public void OnPointerExit(PointerEventData eventData)
    {
        if (this.GetComponent<TMP_InputField>().interactable)
        {
            //this.GetComponent<Image>().color = this.GetComponent<TMP_InputField>().colors.normalColor;
        }



        this.isHover = false;
    }
}
