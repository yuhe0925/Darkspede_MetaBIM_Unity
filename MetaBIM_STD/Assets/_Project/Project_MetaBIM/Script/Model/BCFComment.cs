using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Sycamore_Document
/// </summary>
namespace MetaBIM
{
    [Serializable]
    public class BCFComment
    {
        public string guid;
        public string created;
        public string updated;
        public string status;
        public string tableName;

        public string commentedBy = "";
        public string commentText = "";
        public string commentTarget = "";


        public BCFComment()
        {
            guid = Guid.NewGuid().ToString();
            created = DateTime.Now.Ticks.ToString();
            updated = DateTime.Now.Ticks.ToString();
            tableName = this.GetType().Name;
            status = Config.DevelopmentStage;
        }


        public static string ToJson(BCFComment _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static BCFComment FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<BCFComment>(_json);
        }
    }
}