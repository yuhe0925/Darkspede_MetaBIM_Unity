
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
    public class Version : IModel
    {

        public string tableName;

        public string attachedWorkspace;
        public string attachedProject;
        public string createdBy;        // who created this version, profile guid
        public int versionID = 0;

        public string processingStatus = "pending";   // pending | converting | complete | error
        public float processingProgress = 0;

        public string originalFileName = ""; // need ?
        public string sourceFile = "";       // the file (ifc) uploaded to the system
        public string targetFile = "";       // the file (assetbundle, or fbx) create by system
        public string xmlFile = "";          // the file (xml) create by system   
        public string fileType = "";

        public int sourceFileSize = 0;
        public int targetFileSize = 0;
        public int xmlFileSize = 0;
        public int versionStatus = 0;   // 0,active, 4,archived

        public string convertionAction = "default";

        public int numberOfElements = 0;
        public int numberOfTriangles = 0;
        public int numberOfEdges = 0;
        public int numberOfFaces = 0;


        public int numberOfUpdates = 0;

        public int assetVersion;
        public int assetCRC;



        public List<string> versionUpdateRecord;


        public Version()
        {
            versionUpdateRecord = new List<string>();
        }

        public Version(string _attached, int _versionID)
        {
            versionID = _versionID;
            attachedProject = _attached;
            tableName = this.GetType().Name;

        }


        public static string ToJson(Version _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static Version FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<Version>(_json);
        }

        public string GetBaseUrl()
        {
            return Config.Workspace_Path + attachedWorkspace + "/" + attachedProject + "/" + guid + "/";
        }
    }

}