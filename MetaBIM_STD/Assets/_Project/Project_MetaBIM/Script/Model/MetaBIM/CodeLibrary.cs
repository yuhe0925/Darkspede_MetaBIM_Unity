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
    public class CodeLibrary : IModel
    {
        public string codeDocumentName;
        public Document document;
        public string LibraryName;
        public string LibraryDescription;

        public List<CodeRule> rules; 

        public CodeLibrary()
        {
            rules = new List<CodeRule>();
        }
        

        #region JSON
        public static string ToJson(CodeLibrary _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static CodeLibrary FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<CodeLibrary>(_json);
        }

        public static List<CodeLibrary> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<CodeLibrary>>(_json);
        }
        #endregion
    }




}