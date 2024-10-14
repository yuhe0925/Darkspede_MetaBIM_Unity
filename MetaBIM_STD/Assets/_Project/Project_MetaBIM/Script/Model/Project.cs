
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UnityEngine;

/// <summary>
/// Summary description for Sycamore_Document
/// </summary>
namespace MetaBIM
{

    [Serializable]
    public class Project : IModel
    {
        public string tableName;
        public string createdBy;
        public string projectName;
        public string projectDescription;
        public string projectSnaphotUrl = "default";
        public int projectStatus = 0;   // 0,active, 4,archived

        public List<Version> versions;
        public List<Annotation> annotations;
        public List<Transaction> transactions;
        public List<Permission> members;    // record of list of profile guid belong to this project

        public Location projectLocation;
        public List<SuppliedMaterial> suppliedMaterials;


        // Object list 
        public List<string> elements;


        public Project()
        {
            tableName = this.GetType().Name;
            annotations = new List<Annotation>();
            versions = new List<Version>();
            transactions = new List<Transaction>();
            members = new List<Permission>();
            elements = new List<string>();
            suppliedMaterials = new List<SuppliedMaterial>();
        }

        public Version GetVersion(string _guid)
        {
            foreach(var version in versions)
            {
                if(version.guid == _guid)
                {
                    return version;
                }
            }

            return null;
        }

        public Project(string _createdBy,string _projectName)
        {
            createdBy = _createdBy;
            projectName = _projectName;

            tableName = this.GetType().Name;
            annotations = new List<Annotation>();
            versions = new List<Version>();
            transactions = new List<Transaction>();
            members = new List<Permission>();
        }

        public int GetProjectDataUsage()
        {
            int count = 0;
            foreach (MetaBIM.Version item in versions)
            {
                count = count + item.sourceFileSize + item.xmlFileSize;
            }
            return count;
        }


        public static string ToJson(Project _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static string ToJsonList(List<Project> _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static Project FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<Project>(_json);
        }

        public static List<Project> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<Project>>(_json);
        }
    }



}