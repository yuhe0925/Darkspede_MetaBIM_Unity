using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class SearchItem
{
    public string id;
    public string name;
    public string content;
    public string type = "N";
    public int index = 0;

    public SearchItem(string _id, string _name, int _index = 0)
    {
        id = _id;
        name = _name;
        index = _index;
    }

}
