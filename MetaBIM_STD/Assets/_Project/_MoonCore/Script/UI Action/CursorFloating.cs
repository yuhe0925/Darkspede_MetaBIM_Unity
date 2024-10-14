using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorFloating : MonoBehaviour 
{ 

    public TextMeshProUGUI Text_FloatText;
    public CanvasGroup CanvasGroup;
    public Canvas parentCanvas;
    private float Alpha;

    public bool IsTriggred;
    public float AppearSpeed;
    public float FadeSpeed;
    // Start is called before the first frame update
    void Start()
    {
        IsTriggred = false;
        OnHintFade();
    }

    // Update is called once per frame
    void Update()
    {

    }



    public void SetCursorHint( Vector2 _position)
    {
       Vector2 pos;
       if(RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, _position, Camera.main, out pos))
        {
            transform.position = parentCanvas.transform.TransformPoint(pos + new Vector2(0, -40));
        }
    }


    public void OnHintAppear(string _cursorText)
    {
        LeanTween.alphaCanvas(CanvasGroup, 1, AppearSpeed);
        Text_FloatText.text = _cursorText;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)CanvasGroup.transform);
    }


    public void OnHintFade()
    {
        LeanTween.alphaCanvas(CanvasGroup, 0f, FadeSpeed);
    }



}
