
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Dummy 的摘要说明
/// </summary>
public class Dummy : IModel
{
    public string target;
    public string content;
    public float size;


    public Dummy(string target, string content, float size)
    {
        this.target = target;
        this.content = content;
        this.size = size;
    }

    public static string ToJson(Dummy _item, bool _isMasked = true)
    {
        return JsonConvert.SerializeObject(_item);
    }

    public static Dummy FromJson(string _json)
    {
        return JsonConvert.DeserializeObject<Dummy>(_json);
    }

    public static List<Dummy> FromJsonList(string _json)
    {
        return JsonConvert.DeserializeObject<List<Dummy>>(_json);
    }

    public new string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}