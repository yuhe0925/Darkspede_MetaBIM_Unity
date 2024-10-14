using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// A request that convert ifc into Assetbundle
/// </summary>
namespace MetaBIM
{
    [Serializable]
    public class ConvertRequest : IModel
    {

        public string tableName;

        public string requestStatus     = "pending";   // pending | converting | complete | error
        public float  requestProgress   = 0;
        public string requestSourceFile = "";   
        public string requestTargetFIle = "";
        public int fileSize = 0;
        public int convertSize = 0;

        public string attachedProject;

        public ConvertRequest()
        {
            tableName = this.GetType().Name;
        }


        public static string ToJson(ConvertRequest _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static ConvertRequest FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<ConvertRequest>(_json);
        }
    }
}