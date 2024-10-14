using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionBoxController : MonoBehaviour
{
    public GameObject SectionBoxObject;
    public SectionBox SectionBox;

    // Start is called before the first frame update
    void Start()
    {
        OnSectionBoxDisable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    


    public void OnSectionBoxEnable(Bounds _targetBound, Vector3 _offset)
    {
        SectionBoxObject.SetActive(true);
        SectionBox.OnInitSectionBox(_targetBound, _offset);
    }

    public void OnSectionBoxEnable()
    {
        SectionBoxObject.SetActive(true);
        SectionBox.isDrawing = true;
    }


    public void OnSectionBoxDisable()
    {
        SectionBox.isDrawing = false;
        SectionBoxObject.SetActive(false);
    }     
}
