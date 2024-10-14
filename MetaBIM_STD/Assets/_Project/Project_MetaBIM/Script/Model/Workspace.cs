
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
    public class Workspace : IModel
    {
        public string tableName;

        public string createdBy;
        public string workspaceName;
        public string workspaceDescription;
        public string workspaceSnapshotUrl = "default";
        public string workspaceType = "general";
        public int workspaceStatus = 0;   // 0,active, 4,archived

        public Orgnization orgnization;
        public Location location;   // site location, in general
        public List<Project> projects;
        public List<Permission> members;   // databinding to <profile>



        public Workspace()
        {
            tableName = this.GetType().Name;
            orgnization = new Orgnization();
            location = new Location();
            projects = new List<Project>();
            members = new List<Permission>();
        }


        public Project GetProject(string _guid)
        {
            foreach(var project in projects)
            {
                if(project.guid == _guid)
                {
                    return project;
                }
            }

            return null;
        }

        // How many versions there are
        public int GetModelCount()
        {
            int count = 0;
            foreach (Project project in projects)
            {
                count = count + project.versions.Count;
            }


            return count;
        }


        public int GetElementCount()
        {
            int count = 0;
            foreach (Project project in projects)
            {
                count = count + project.elements.Count;
            }

            return count;
        }

        public int GetWorkspaceDataUsage()
        {
            int count = 0;
            foreach (Project item in projects)
            {
                count = count + item.GetProjectDataUsage();
            }

            return count;
        }


        public static string ToJson(Workspace _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static string ToJsonList(List<Workspace> _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static Workspace FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<Workspace>(_json);
        }

        public static List<Workspace> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<Workspace>>(_json);
        }
    }
}