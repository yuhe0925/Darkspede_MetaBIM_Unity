using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Sycamore_Collaboration
/// </summary>
namespace MetaBIM
{
    [Serializable]
    public class Collaboration
    {
        public string guid;
        public string created;
        public string updated;
        public string status;
        public string tableName;


        public string createdBy;
        public string associatedProject;
        public string collaborationStatus = "pending";
        public string collaborator;
        public string permission;



        public Collaboration()
        {
            guid = Guid.NewGuid().ToString();
            created = DateTime.Now.Ticks.ToString();
            updated = DateTime.Now.Ticks.ToString();
            tableName = this.GetType().Name;
            status = Config.DevelopmentStage;

        }





        public static string ToJson(Collaboration _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static Collaboration FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<Collaboration>(_json);
        }

        public static List<Collaboration> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<Collaboration>>(_json);
        }
    }

    public enum collaborationStatus
    {
        pending,
        accepted,
        deactivated,
    }

    public enum permission
    {
        collaborator,
        coowner,
        viewer,
        owner,
    }
}