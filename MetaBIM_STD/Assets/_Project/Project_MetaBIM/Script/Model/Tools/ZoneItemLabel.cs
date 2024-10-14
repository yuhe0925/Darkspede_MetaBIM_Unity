using MetaBIM;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ZoneItemLabel : MonoBehaviour
{
    public TextMeshProUGUI Text_ZoneName;
    public Transform target;
    public RectTransform Rect_FloatLabel;
    public ZoneItem LinkedObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(target.position);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(ZoneManagement.Instance.CanvasRect, screenPos, null, out localPoint);
        Rect_FloatLabel.anchoredPosition = localPoint + new Vector2(0, 0);
    }
  
}
