using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MoonCore_TextMaterialConfig : MonoBehaviour
{
    public Slider Slider_Softness;
    public Slider Slider_Dilate;
    public Slider Slider_Shapeness;
    public TMP_InputField Text_Softness;
    public TMP_InputField Text_Dilate;
    public TMP_InputField Text_Shapeness;
    public Material DSF_SSD;

    // Start is called before the first frame update
    void Start()
    {
        float value;

        value = DSF_SSD.GetFloat("_OutlineSoftness");
        Text_Softness.text = value.ToString();
        Slider_Softness.value = value;

        value = DSF_SSD.GetFloat("_FaceDilate");
        Text_Dilate.text = value.ToString();
        Slider_Dilate.value = value;

        value = DSF_SSD.GetFloat("_Sharpness");
        Text_Shapeness.text = value.ToString();
        Slider_Shapeness.value = value;


        Slider_Softness.onValueChanged.AddListener(OnValueChange_Slider_Softness);
        Slider_Dilate.onValueChanged.AddListener(OnValueChange_Slider_Dilate);
        Slider_Shapeness.onValueChanged.AddListener(OnValueChange_Slider_Shapeness);


    }

    // Update is called once per frame
    void Update()
    {

    }




    public void OnValueChange_Slider_Softness(float _value)
    {

        Text_Softness.text = _value.ToString();
        DSF_SSD.SetFloat("_OutlineSoftness", _value);

    }

    public void OnValueChange_Slider_Dilate(float _value)
    {


        Text_Dilate.text = _value.ToString();
        DSF_SSD.SetFloat("_FaceDilate", _value);

    }

    public void OnValueChange_Slider_Shapeness(float _value)
    {


        Text_Shapeness.text = _value.ToString();
        DSF_SSD.SetFloat("_Sharpness", _value);

    }
}
