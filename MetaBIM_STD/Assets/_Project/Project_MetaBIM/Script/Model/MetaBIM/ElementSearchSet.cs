using MetaBIM;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[BsonIgnoreExtraElements]
[Serializable]
public class ElementSearchSet : IModel
{
    public string searchTarget;
    public string condition;
    public string searchValue;


    public ElementSearchSet()
    {
        searchTarget = "default";
        condition = "Is";
        searchValue = "";
    }

    #region MISC
    public static string ToJson(ElementSearchSet _item)
    {

        return JsonConvert.SerializeObject(_item);
    }

    public static ElementSearchSet FromJson(string _json)
    {
        return JsonConvert.DeserializeObject<ElementSearchSet>(_json);
    }

    public static List<ElementSearchSet> FromJsonList(string _json)
    {
        return JsonConvert.DeserializeObject<List<ElementSearchSet>>(_json);
    }

    public new string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    #endregion
}
