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
    public class Comment: IModel
    {

        public string tableName;
        public string commentedBy = "";
        public string commentText = "";
        public string commentTarget = "";

        public Comment(string commentedBy, string commentText, string commentTarget)
        {
            this.commentedBy = commentedBy;
            this.commentText = commentText;
            this.commentTarget = commentTarget;
            tableName = this.GetType().Name;
        }

        public static string ToJson(Comment _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static Comment FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<Comment>(_json);
        }
    }
}