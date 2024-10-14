﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeTypeHelper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        image = transform.GetChild(1).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    Image image;
    public void OnPointerClick(int i)
    {
        if (GraphMechanism.instance.previousTypeImage != null)
            GraphMechanism.instance.previousTypeImage.enabled = false;
        image.enabled = true;
        GraphMechanism.instance.previousTypeImage = image;
        GraphMechanism.instance.LineType = i;
    }
}
