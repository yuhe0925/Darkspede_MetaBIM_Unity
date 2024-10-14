using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[Serializable]
public class Permission: IModel
{
    public string attachedProfile;
    public string attachedTarget;
    public int permissionType = 1;  // web/app/default
    public int permissionLevel = 1;


    public Permission(string _attachedProfile, string _attachedTarget)
    {
        attachedProfile = _attachedProfile;
        attachedTarget = _attachedTarget;
    }

    public static string ToJson(Permission _item)
    {
        return JsonConvert.SerializeObject(_item);
    }

    public static Permission FromJson(string _json)
    {
        return JsonConvert.DeserializeObject<Permission>(_json);
    }


}



// Permission



/* Access Type
 
0 = web
0 = app
0 = desktop
0 = api
 
 */



/* Access 
 
0 = read
0 = write
0 = delete
0 = duplicate/share
 
 */