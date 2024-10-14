using UnityEngine;
using UnityEngine.UI;

public class SetColor : MonoBehaviour
{
    public Color initialColor, newColor;
    public ColorPicker colorDialog;

    Image thisImage = null;
    Vector3 colorPickerPosition;

    private void Start()
    {
        var scaleFactor = GameObject.Find("Canvas").GetComponent<Canvas>().scaleFactor;
        colorPickerPosition = new Vector3(transform.GetChild(1).GetChild(0).position.x , transform.GetChild(1).position.y);
        initialColor = GraphMechanism.instance.LineColor.color;

        thisImage = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        thisImage.color = initialColor;
    }

    public void EditColor()
    {
        var scaleFactor = GameObject.Find("Canvas").GetComponent<Canvas>().scaleFactor;
        colorPickerPosition = new Vector3(transform.GetChild(1).GetChild(0).position.x , transform.GetChild(1).position.y);
        ColorPickerPanel.Instance.CallColorPicker(GraphMechanism.instance.LineColor.color, colorPickerPosition, SetNewColor, ChangeColor);
        thisImage.color = GraphMechanism.instance.LineColor.color;
    }
    public void SetNewColor(Color color)
    {
        GraphMechanism.instance.LineColor.color = color;
        thisImage.color = GraphMechanism.instance.LineColor.color;
    }

    public void ChangeColor(Color color)
    {
        GraphMechanism.instance.LineColor.color = color;
        thisImage.color = GraphMechanism.instance.LineColor.color;
    }

    public void OnButtonDown()
    {
        colorDialog.gameObject.SetActive(!colorDialog.gameObject.activeSelf);
    }

    void Update()
    {
        if (thisImage != null)
            if (thisImage.color != GraphMechanism.instance.LineColor.color)
                thisImage.color = GraphMechanism.instance.LineColor.color;
    }
}
