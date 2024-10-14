using MetaBIM;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;


[BsonIgnoreExtraElements]
[Serializable]
public class RemoteCommand : IModel
{
    public string commandHeader;  // command type
    public string targetModel;    // model guid
    public string targetElement;  // which element or group in the model
    public string targetAction;   // what action to be performed

    public string commandStatus = "pending"; // pending = waiting to be executed,
                                             // processing = being executed,
                                             // completed = executed,
                                             // failed = failed to execut
    public string commnadResult;


    public static string ToJson(RemoteCommand _item)
    {
        return JsonConvert.SerializeObject(_item);
    }

    public static string ToJsonList(List<RemoteCommand> _item)
    {
        return JsonConvert.SerializeObject(_item);
    }

    public static RemoteCommand FromJson(string _json)
    {
        return JsonConvert.DeserializeObject<RemoteCommand>(_json);
    }

    public static List<RemoteCommand> FromJsonList(string _json)
    {
        return JsonConvert.DeserializeObject<List<RemoteCommand>>(_json);
    }
}
