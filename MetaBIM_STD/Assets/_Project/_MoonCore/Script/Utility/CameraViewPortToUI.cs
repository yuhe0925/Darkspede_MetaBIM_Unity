using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CameraViewPortToUI : MonoBehaviour
{
    public Camera TargetCame;
    public RectTransform TargetRectTransform;
    public Canvas TargetCanvas;
    public float HeightOffset = 70;
    public float WidthOffset = 484;

    // Start is called before the first frame update
    void Start()
    {
        OnReset();
        Debug.Log("CameraViewPortToUI anchoredPosition:" + TargetRectTransform.anchoredPosition);
        Debug.Log("CameraViewPortToUI Center          :" + TargetRectTransform.rect.center);
        Debug.Log("CameraViewPortToUI position        :" + TargetRectTransform.TransformPoint(TargetRectTransform.rect.center));
        Debug.Log("CameraViewPortToUI TR position     :" + TargetRectTransform.gameObject.transform.position);
        Debug.Log("CameraViewPortToUI Size            :" + TargetRectTransform.sizeDelta);
        Debug.Log("CameraViewPortToUI Screen          :" + Screen.width  + " | " +  Screen.height);
        Debug.Log("CameraViewPortToUI Scale           :" + TargetCanvas.scaleFactor);

        OnMatch();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  
    public void OnMatch()
    {
        Rect viewport = TargetCame.rect;

        Vector2 size = TargetRectTransform.sizeDelta;
        Vector2 position = TargetRectTransform.anchoredPosition;
        Vector2 screen = new Vector2(Screen.width, Screen.height);

        float x = WidthOffset * TargetCanvas.scaleFactor / Screen.width;
        float y = (size.y / 2 - HeightOffset ) / (Screen.height / TargetCanvas.scaleFactor);
        float w = size.x / Screen.width * TargetCanvas.scaleFactor;
        float h = size.y / Screen.height * TargetCanvas.scaleFactor;

        TargetCame.rect = new Rect(x,y,w,h);
    }

    public void OnReset()
    {
        TargetCame.rect = new Rect(0,0,1,1);
    }



}



