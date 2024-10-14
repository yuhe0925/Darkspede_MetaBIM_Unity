using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBIM
{
    public class DataSet
    {
        public static List<Project> MyProjects = new List<Project>();
        public static List<Collaboration> MyCollabrations = new List<Collaboration>();
        public static List<Transaction> MyTransactions = new List<Transaction>();


        public static List<Workspace> MyWorkspaces = new List<Workspace>();

        // Model Holder
        public Dictionary<string, List<BimObject>> MyModelObject = new Dictionary<string, List<BimObject>>();






        public static List<string> PostStatus = new List<string>() {
            "Open",
            "Pending",
            "Close",
            "Expired",
        };

        public static List<string> CompanyTypes = new List<string>() {
            "Sole Trader",
            "Company",
            "Partnership",
            "Trust",
            "Not Stated",
        };


        public static List<string> UserPermission = new List<string>() {
            "Owner",
            "Administrator",
            "Member",
            "Guest",
        };


        public static string GetUploadFileTargetString(UploadFileTarget _target)
        {
            switch (_target) {
                case UploadFileTarget.usericon:
                    return "usericon";
                case UploadFileTarget.bcf:
                    return "bcf";
                case UploadFileTarget.project:
                    return "project";
                case UploadFileTarget.xml:
                    return "xml";
                case UploadFileTarget.assetbundle:
                    return "assetbundle";
                case UploadFileTarget.package:
                    return "package";
                default:
                    break;
            }

            return "";
        }

        public static string GetFileTypeString(FileType _target)
        {
            switch (_target)
            {
                case FileType.Image:
                    return "image/png,image/jpeg";
                case FileType.PDF:
                    return "application/pdf";
                case FileType.ZIP:
                    return "application/zip";
                case FileType.Text:
                    return "text/plain, text/csv";
                case FileType.Table:
                    return "text/csv";
                case FileType.Docx:
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case FileType.Xlsx:
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case FileType.Video:
                    return "video/mp4";
                // binary file such as fbx, assets, bin, dll
                default:
                    break;
            }

            return "text/plain";
        }


        public enum UploadFileTarget
        {
            usericon,
            bcf,
            project,
            xml,
            assetbundle,
            package,
        }


        public enum FileType
        {
            Image,      // png, jpg
            PDF,        // pdf
            ZIP,        // zip
            Text,       // txt
            Table,      // csv
            Docx,       // docx
            Xlsx,       // xlsx
            Video,
            // binary file such as fbx, assets, bin, dll
        }


        public enum SortGroup
        {
            name,
            update,
            status,
            request,
            transaction,
            permission,
        }


        public static List<string> SearchPropertyCondition = new List<string>() {
            "Contains",
            "Is",
            "Greater",
            "Lesser",
        };

        public static List<string> SearchPropertyCondition_Number = new List<string>() {
            "Equal",
            "Greater",
            "Lesser",
            "Within",
        };


        public static List<string> ModelProperties = new List<string>();



    }


}
