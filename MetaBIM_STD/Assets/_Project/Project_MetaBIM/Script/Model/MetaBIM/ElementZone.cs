using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace MetaBIM
{
    [BsonIgnoreExtraElements]
    [Serializable]
    public class ElementZone: IModel
    {

        public string attachedProject = "";

        // display informatioin 
        public string zoneID = "";
        public string zoneName = "";
        public string zoneDescription = "";
        public string zoneType;
        public string elementSelection;
        public List<float> zoneColor; // in [r,g,b,a]

        // statistics
        public int zoneVolume;
        public int materialWeight;


        // selection
        public List<string> elements = new List<string>();

        // zone space information
        public Vector3D zoneCenter;
        public Vector3D zoneExtense;

        public ElementZone()
        {
            //zoneColor = new List<float> { UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), 0.1f };
        }


        public ElementZone(string _attachedProject, string _zoneName)
        {
            this.attachedProject = _attachedProject;
            this.zoneName = _zoneName;
            //zoneColor = new List<float> { UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), 0.1f };
        }


        #region MISC
        public static string ToJson(ElementZone _item)
        {

            return JsonConvert.SerializeObject(_item);
        }

        public static ElementZone FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<ElementZone>(_json);
        }

        public static List<ElementZone> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<ElementZone>>(_json);
        }

        public new string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        #endregion

    }

}