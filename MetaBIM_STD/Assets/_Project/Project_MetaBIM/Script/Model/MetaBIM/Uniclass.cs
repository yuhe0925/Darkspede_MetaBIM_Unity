using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;



namespace MetaBIM
{
    [Serializable]
    public class Uniclass : IModel
    {

        public string Code = Config.None;
        public string Group = Config.None;
        public string Sub_Group = Config.None;
        public string Section = Config.None;
        public string Object = Config.None;
        public string Title = Config.None;
        public string NBS_Code = Config.None;
        public string COBie = Config.None;
        public string NRM1 = Config.None;
        public string CESMM = Config.None;

        // mapped to a if class
        public string IFC = Config.None;
        public string IFC_Type = Config.None;
        public int level = 4;
        public string parentCode;

        // additional information
        public string documentName;
        public string documentNameShort;

        public List<Uniclass> children = new List<Uniclass>();


        // additional information, for display
        public bool IsCollapsed = true;
        public bool IsSearched = true;
        public Uniclass parent;
        
        public Uniclass()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        public void SetDocName(string _shortName, string _name)
        {
            documentName = _name;
            documentNameShort = _shortName;
        }


        // create a row of records based on the input document
        public static Uniclass CreateNew(List<string> _record, string _document = "pr", string _documentShort = "pr")
        {
            Uniclass item = new Uniclass();

            item.Code = _record[0];
            item.Group = _record[1];
            item.Sub_Group = _record[2];
            item.Section = _record[3];
            item.Object = _record[4];
            item.Title = _record[5];
            item.NBS_Code = _record[6];
            item.COBie = _record[7];
            item.NRM1 = _record[8];
            item.IFC = _record[9];
            item.CESMM = _record[10];


            if (item.IFC != Config.None)
            {
                string[] li = item.IFC.Split('.');

                if (li.Length > 1)
                {
                    item.IFC = li[0];
                    item.IFC_Type = li[1];
                }
            }


            item.documentName = _document;
            item.documentNameShort = _documentShort;

            return item;
        }

    }


    public static class UniclassMapper
    {
        public static Dictionary<string, string> documentNames = new Dictionary<string, string>
        {
            {"Ac","Activities" },
            {"Co","Complexes" },
            {"EF","Elements and Functions" },
            {"En","Entities" },
            {"FI","Form of Information" },
            {"PM","Project Management" },
            {"Pr","Products" },
            {"Ro","Roles" },
            {"SL","Spaces and Locations" },
            {"Ss","Systems" },
            {"TE","Tools and Equipment" },
            {"Zz","CAD" },

        };
    }

}