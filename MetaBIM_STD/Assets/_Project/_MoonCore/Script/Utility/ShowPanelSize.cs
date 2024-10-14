using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;


[ExecuteAlways]
public class ShowPanelSize : MonoBehaviour
{
    public RectTransform Panel;
    public Vector2 PanelDelta;
    public Vector2 PanelSize;
    public TextRenderer TextRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Panel == null)
        {
            Panel = GetComponent<RectTransform>();
        }
        PanelSize = Panel.rect.size;
        PanelDelta = Panel.sizeDelta;
    }
}
