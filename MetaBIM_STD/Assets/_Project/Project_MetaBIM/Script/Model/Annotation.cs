
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
    public class Annotation : IModel
    {
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
        public List<Comment> comments;

        public string attachedProject;

        public Annotation()
        {
            tableName = this.GetType().Name;
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