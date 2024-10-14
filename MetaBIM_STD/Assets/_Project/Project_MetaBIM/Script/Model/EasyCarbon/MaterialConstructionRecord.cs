using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;


namespace MetaBIM
{


    [BsonIgnoreExtraElements]
    [Serializable]
    public class MaterialConstructionRecord : IModel
    {


        // display product status
        public string materialStatus = "in stock";
       


        #region MISC
        public static string ToJson(MaterialConstructionRecord _item)
        {

            return JsonConvert.SerializeObject(_item);
        }

        public static MaterialConstructionRecord FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<MaterialConstructionRecord>(_json);
        }

        public static List<MaterialConstructionRecord> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<MaterialConstructionRecord>>(_json);
        }

        public new string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }



        #endregion

    }


}