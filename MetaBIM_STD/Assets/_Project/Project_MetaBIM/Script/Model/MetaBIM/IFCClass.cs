using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;



namespace MetaBIM
{

    [Serializable]
    public class IFCClass : IModel
    {
        public string className;
        public bool IsEnum;
        public List<string> Type;
        public int layer = 0;
        public List<IFCClass> children;

        // additional information
        public bool IsCollapsed = true;
        public bool IsSearched = true;
        public IFCClass parent;

        public static string ToJson(IFCClass _item, bool _isMasked = true)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static IFCClass FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<IFCClass>(_json);
        }

        public static List<IFCClass> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<IFCClass>>(_json);
        }

        public new string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public static class IFCclassMapper
    {
        public static Dictionary<string, string> className = new Dictionary<string, string>
        {
            {"Actor","IfcActor" },
            {"Control","IfcControl" },
            {"Group","IfcGroup" },
            {"Resource","IfcResource" },
            {"Process","IfcProcess" },
            {"Product","IfcProduct" },
        };
    }


    public static class IFCclassTreeRender
    {
        public static Dictionary<string, string> className = new Dictionary<string, string>
        {
            {"Actor","IfcActor" },
            {"Control","IfcControl" },
            {"Group","IfcGroup" },
            {"Resource","IfcResource" },
            {"Process","IfcProcess" },
            {"Product","IfcProduct" },
        };
    }


    public static class EpicClassMapper
    {
        public static Dictionary<string, string> className = new Dictionary<string, string>
        {
            {"1","" },
            {"2","" },
            {"3","" },
            {"4","" },
            {"5","" },
            {"6","" },
            {"7","" },
            {"8","" },
        };
    }

}