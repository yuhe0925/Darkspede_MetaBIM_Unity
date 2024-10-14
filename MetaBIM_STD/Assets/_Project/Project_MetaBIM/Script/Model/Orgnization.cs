
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace MetaBIM
{

    [Serializable]
    public class Orgnization : IModel
    {

        public string orgnizationName;
        public string orgnizationLocation;
        public string orgnizationSite;
        public string orgnizationIcon;

        public Orgnization()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        public static string ToJson(Orgnization _item, bool _isMasked = true)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static Orgnization FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<Orgnization>(_json);
        }

        public static List<Orgnization> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<Orgnization>>(_json);
        }

        public new string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}