
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// A request that convert ifc into Assetbundle
/// </summary>
namespace MetaBIM
{
    [Serializable]
    public class VersionUpdate : IModel
    {

        public string tableName = "VersionUpdate";

        public string attachedWorkspace;
        public string attachedProject;
        public string attachedVersion;
        public string createdBy = "system";        // who created this version, profile guid

        public string versionType = "master";  // master, branch
        public int versionID = 0;


        public List<VersionBimElement> elements;  // for ifc
        public List<VersionZone> zones;


        public VersionUpdate()
        {
            elements = new List<VersionBimElement>();
            zones = new List<VersionZone>();
        }


        public int GetUpdateCount()
        {
            return elements.Count + zones.Count;
        }
        
        public static string ToJson(VersionUpdate _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static VersionUpdate FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<VersionUpdate>(_json);
        }
    }



    [Serializable]
    public class VersionBimElement
    {
        public string elementID;
        public int versionID;
        public string updateType;  // update | delete | add | 

        public string orignalBimObjectGuid;
        public string parentBimObjectGuid;
        public string UpdatedBimObjectGuid;

        public VersionBimElement()
        {

        }
    }

    // by level
    [Serializable]
    public class VersionZone
    {
        public int zoneLevelIndex;   // 0 - n
        public string zoneLevelName = "Level 1";

        public int zoneIndex;   // 0 - n
        public string zoneName = "New Zone";
        public string zoneCatergory = "space";

        // space info
        public string zoneType = "free";  // contiuned , free
        public float heightRangeMin;
        public float heightRangeMax;

        public List<Vector3D> cornerArray;
        public List<Vector3D> planeArray;

        public VersionZone()
        {
            cornerArray = new List<Vector3D>();
            planeArray = new List<Vector3D>();
        }


    }






}