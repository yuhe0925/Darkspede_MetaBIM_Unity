using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;


/// <summary>
/// Summary description for Sycamore_Document
/// </summary>
namespace MetaBIM
{
    [BsonIgnoreExtraElements]
    [Serializable]
    public class EasycarbonProject : IModel
    {
        public string attachedMetaBIMProject;

        public string createdBy;
        public string projectName;
        public string projectDescription;
        public string organizationName;
        public string projectSnaphotUrl = "default";
        public int    projectStatus = 0;   // 0,active, 4,archived

        public Location projectLocation;

        // model information
        public string modelFormate = "IFC 4.0";
        public string modelStage = "Design, Transport, Construction";

        // geo information
        public string coordinateSystem = "WSG84";
        public float baseEvluation = 0f;
        public float heading = 0f;
        public float baseLength = 0f;
        public float baseWidth = 0f;
        public float baseHeight = 0f;

        public string emissionFactorDatabase = "default";
        public string emissionFactorVersion = "default";


        public List<BimMaterial> materials;


        public EasycarbonProject()
        {
            materials = new List<BimMaterial>();
        }



        public static string ToJson(EasycarbonProject _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static string ToJsonList(List<EasycarbonProject> _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static EasycarbonProject FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<EasycarbonProject>(_json);
        }

        public static List<EasycarbonProject> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<EasycarbonProject>>(_json);
        }
    }
}