using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EntitySceneHelper : MonoBehaviour
{
    public List<Connection> connections = new List<Connection>();

    public void OnMouseEnter()
    {
        if (!GraphMechanism.instance.ConnectionLine.gameObject.activeSelf && !GraphMechanism.instance.UIMouseBlock)
            transform.GetChild(5).GetComponent<SpriteRenderer>().enabled = true;
    }

    public void OnMouseExit()
    {
        transform.GetChild(5).GetComponent<SpriteRenderer>().enabled = false;
    }

}
