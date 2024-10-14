using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;


namespace MetaBIM
{
    [BsonIgnoreExtraElements]
    [Serializable]
    public class Supplier : IModel
    {

        // display informatioin 
        public string supplierName;

        public Location supplierLocation;

        public List<SupplierMaterial> supplierStock;

        public Supplier(string supplierName, List<SupplierMaterial> supplierStock)
        {
            this.supplierName = supplierName;
            this.supplierStock = supplierStock;
        }





        public Supplier()
        {
        }



        #region MISC
        public static string ToJson(Supplier _item)
        {

            return JsonConvert.SerializeObject(_item);
        }

        public static Supplier FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<Supplier>(_json);
        }

        public static List<Supplier> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<Supplier>>(_json);
        }

        public new string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }



        #endregion

    }






}