using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnapToScrollPosition : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform contentPanel;

    public Vector2 RectOffSet;
    public float RectWidth;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SnapTo(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        contentPanel.anchoredPosition =
            (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
            - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);

        contentPanel.anchoredPosition = new Vector2(0, contentPanel.anchoredPosition.y);

        // Maybe using leantween ?

    }

    // Move to the center of the view
    public void SnapToBothDirection(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        float y = contentPanel.anchoredPosition.y;
        contentPanel.anchoredPosition =
            (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
            - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);

        RectWidth = target.sizeDelta.x;

        contentPanel.anchoredPosition = new Vector2(contentPanel.anchoredPosition.x + RectWidth, y);

        // Maybe using leantween ?


    }
}
