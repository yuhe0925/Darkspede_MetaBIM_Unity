
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaBIM
{
    // this class need to store into database
    // connected to project/workspace ?
    [Serializable]
    public class SplitPlane : IModel
    {
        public string planeID = ""; // different from IModel.Guid, displayed in UI
        public string planeName = "New Plane"; //  displayed in UI
        public string planeDescription = "New Plane"; //  displayed in UI

        public PlaneType planeType = PlaneType.vertical;  // "XY", "YZ", "XZ"
        public PlaneGroup planeGroup = PlaneGroup.model; // "model", "project", "workspace"

        public float planeRotation;
        public List<Vector3D> planeCorners = new List<Vector3D>();

        public Vector3D planePosition;
        public Vector3D planeDirection;


        public bool isApplied = true;

        public enum PlaneType
        {
            vertical,
            horizontal
        }


        public enum PlaneGroup
        {
            element,
            model,
        }



        public static string ToJson(SplitPlane _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static string ToJsonList(List<SplitPlane> _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static SplitPlane FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<SplitPlane>(_json);
        }

        public static List<SplitPlane> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<SplitPlane>>(_json);
        }


    }
}
