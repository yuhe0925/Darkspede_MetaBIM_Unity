using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Sycamore_Document
/// </summary>
namespace MetaBIM
{
    [Serializable]
    public class BCF
    {
        public string guid;
        public string created;
        public string updated;
        public string status;
        public string tableName;

        public string issueTitle = "none";
        public string issueContent = "none";

        public string issuedBy = "";
        public string assignedTo = "";
        public string dueDate = "";

        public string issueType = ""; // Information, Error, Clash
        public string priority = ""; // High, Medium, Low
        public string amendingStatus = "Opened"; // Opened, Closed, Resolved
        public string snapshotImageUrl = "";

        public string elementID;
        public List<BCFComment> comments;

        public string attachedProject;

        public BCF()
        {
            guid = Guid.NewGuid().ToString();
            created = DateTime.Now.Ticks.ToString();
            updated = DateTime.Now.Ticks.ToString();
            tableName = this.GetType().Name;
            status = Config.DevelopmentStage;
        }
        public static string ToJson(BCF _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static BCF FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<BCF>(_json);
        }
    }
}