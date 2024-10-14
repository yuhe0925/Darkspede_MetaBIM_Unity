using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetaBIM;

[Serializable]
public class LogItem
{
    public string   guid = Guid.NewGuid().ToString("N");
    public string   created = DateTime.Now.Ticks.ToString();
    public string   timestamp = DateTime.Now.ToString(Config.DateTimeStringLogger);
    public string   apiKey;
    public bool     result;
    public double   complete;
    public int      packageSize;

    public string targetUser        = "default";
    public string targetApi         = "default";
    public string targetItem        = "default";
    public string targetAction      = "default";
    public string resultDescription = "default";
    public string package           = "";
    public LogItem(
        string apiKey, 
        bool   result, 
        string targetUser, 
        string targetApi,
        string targetItem)
    {
        this.apiKey = apiKey;
        this.result = result;
        this.targetUser = targetUser;
        this.targetApi = targetApi;
        this.targetItem = targetItem;
    }

    public void SetLog(bool result, string targetAction, string resultDescription = "default", string package = "default")
    {
        this.result = result;
        this.targetAction = targetAction;
        this.resultDescription = resultDescription;
        this.package = package;
    }


}