using System.Collections;
using System.Collections.Generic;
using MetaBIM;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MetaBIM
{
    [Serializable]
    [BsonIgnoreExtraElements]
    public class BimMaterial : IModel
    {
        public string elementName;
        public string materialName;
        public int elementCount = 0;
        public int volumeCount = 0;

        public string emissionfactorID;
        public string emissionfactorUnit;
        public string emissionfactor;

        public string materialSource;
        public string materialAssigned;

        public BimMaterial(string elementName, string materialName)
        {
            this.elementName = elementName;
            this.materialName = materialName;
        }

        public static string ToJson(BimMaterial _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static string ToJsonList(List<BimMaterial> _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static BimMaterial FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<BimMaterial>(_json);
        }

        public static List<BimMaterial> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<BimMaterial>>(_json);
        }
    }
}
