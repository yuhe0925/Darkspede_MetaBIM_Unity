using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;


namespace MetaBIM
{
    [BsonIgnoreExtraElements]
    [Serializable]
    public class SuppliedMaterial : IModel
    {
        public string attachedSupplier;
        public string attachedProject;
        public string attachedWorkspace;

        public SupplierMaterial material;

        public int materialQuantity = 0;
        public string transportType = "truck";



        #region MISC
        public static string ToJson(SuppliedMaterial _item)
        {

            return JsonConvert.SerializeObject(_item);
        }

        public static SuppliedMaterial FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<SuppliedMaterial>(_json);
        }

        public static List<SuppliedMaterial> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<SuppliedMaterial>>(_json);
        }

        public new string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }



        #endregion

    }

}