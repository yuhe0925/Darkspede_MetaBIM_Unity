using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;


namespace MetaBIM
{


    [BsonIgnoreExtraElements]
    [Serializable]
    public class SupplierMaterial : IModel
    {

        // display product informatioin 
        public string materialName = "new material";
        public float materialUnitPrice = 0f;
        public string materialUnit = "m3";
        public string materialUnityPriceCurrency = "AUD";
        public string materialResourceID = "00_00"; // the resource ID of emission documents


        // display product status
        public string materialStatus = "in stock";
        public int materialStock = 0;


        #region MISC
        public static string ToJson(SupplierMaterial _item)
        {

            return JsonConvert.SerializeObject(_item);
        }

        public static SupplierMaterial FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<SupplierMaterial>(_json);
        }

        public static List<SupplierMaterial> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<SupplierMaterial>>(_json);
        }

        public new string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }



        #endregion

    }


}