using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;



/// <summary>
/// Summary description for Sycamore_Document
/// </summary>
namespace MetaBIM.CodeChecking
{
    [Serializable]
    public class CodeCheckReport : IModel
    {
        public CodeRule appliedRule;
        public string targetElementName;
        public string targetElementId = Guid.NewGuid().ToString("N");

        public string targetAttributeName;
        public string targetAttributeValue;
        public string resultValue;
        public string resultRate;

        public List<Comment> comments;

        public CodeCheckReport()
        {
            comments = new List<Comment>();
        }


        
        #region JSON


        public static string ToJson(CodeCheckReport _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static CodeCheckReport FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<CodeCheckReport>(_json);
        }

        public static List<CodeCheckReport> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<CodeCheckReport>>(_json);
        }
        #endregion
    }
    


 
}