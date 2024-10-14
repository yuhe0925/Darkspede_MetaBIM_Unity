using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;



namespace MetaBIM
{
   
    [Serializable]
    public class epicClass : IModel
    {
        public string category;
        public string type;
        public string material;
        public string unit;
        public float embodiedEnergy;
        public float embodiedWater;
        public float embodiedGreenhouseGasEmission;
        public string moreInfo;


        public int level;
        public string Content;
            
        public List<epicClass> children;


        public bool IsCollapsed = false;
        public bool IsSearched = false;
        public epicClass parent;

        public epicClass()
        {
            children = new List<epicClass>();
        }


        public void RecordToClass(List<string> _records)
        {
            category = _records[0];
            type = _records[1];
            material = _records[2];
            unit = _records[3];

            float ee = -1;

            float.TryParse(_records[4], out ee);
            embodiedEnergy = ee;

            float.TryParse(_records[5], out ee);
            embodiedWater = ee;

            float.TryParse(_records[6], out ee);
            embodiedGreenhouseGasEmission = ee;

            moreInfo = _records[7];
        }
    }


}