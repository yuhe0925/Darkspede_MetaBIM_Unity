using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class UploaderConfiger : MonoBehaviour
{

    public string target;           // where to save
    public string action;           // what to do after save
    public string fileType;         // select from dropdown
    public string attachedUser;     // who initialized upload

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static string ToJson(UploaderConfiger _item)
    {
        return JsonUtility.ToJson(_item);
    }

    public static UploaderConfiger FromJson(string _json)
    {
        return JsonUtility.FromJson<UploaderConfiger>(_json);
    }
}
