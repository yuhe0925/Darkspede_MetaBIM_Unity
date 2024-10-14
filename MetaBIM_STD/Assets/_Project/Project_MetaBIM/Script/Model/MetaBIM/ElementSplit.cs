using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaBIM
{

    [Serializable]
    public class ElementSplit : IModel
    {

        public string elementID = ""; // the guid of bim guid element
        public string attachedVersion = "";
        public string attachedProject = "";
        public string attachedWorkspace = "";

        public bool IsSplited = false;

        public List<SplitPlane> splitingPlanes;



        public ElementSplit(string elementID, string attachedVersion, string attachedProject, string attachedWorkspace)
        {
            this.elementID = elementID;
            this.attachedVersion = attachedVersion;
            this.attachedProject = attachedProject;
            this.attachedWorkspace = attachedWorkspace;
        }


        public static string ToJson(ElementSplit _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static string ToJsonList(List<ElementSplit> _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static ElementSplit FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<ElementSplit>(_json);
        }

        public static List<ElementSplit> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<ElementSplit>>(_json);
        }
    }
}

