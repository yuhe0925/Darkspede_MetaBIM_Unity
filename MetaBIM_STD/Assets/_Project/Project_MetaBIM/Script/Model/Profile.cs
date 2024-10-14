
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace MetaBIM
{

    [Serializable]
    public class Profile : IModel
    {

        public string tableName;

        // User security
        public string password = "N"; // stored as SHA
        public string twofactorToken = "";
        public string accesstoken = Guid.NewGuid().ToString("N");
        public string tokenExpire = DateTime.Now.Ticks.ToString();
        public string securityCode = Guid.NewGuid().ToString("N");
        public string codeExpire = DateTime.Now.Ticks.ToString();

        // Must have
        public string mobile = "61400000015";

        // Update Later
        public string username = "N";
        public string fullName = "Default User";
        public string email = "N";

        public string location = "N";
        public string companyName = "N";
        public string companyType = "Not Stated";

        public string iconUrl = "resource/usericon/default.png";
        public string fcmToken = "";
        public string wallet = "";

        // Configuration status of user activied module
        public ProfileConfig config;
        // General profile role of system
        public ProfileRole profileRole;

        public Profile()
        {
            tableName = this.GetType().Name;
            config = new ProfileConfig();
            profileRole = new ProfileRole();
        }



        public bool IsCodeExpired()
        {
            if (DateTime.Now.Ticks < long.Parse(codeExpire))
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public bool IsTokenExpired()
        {
            if (DateTime.Now.Ticks < long.Parse(tokenExpire))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string ToJson(Profile _item, bool _isMasked = true)
        {
            if (_isMasked)
            {
                _item.password = "******";
                _item.securityCode = "******";
            }

            return JsonConvert.SerializeObject(_item);
        }

        public static Profile FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<Profile>(_json);
        }

        public static List<Profile> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<Profile>>(_json);
        }

        public new string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    [Serializable]
    public class ProfileConfig
    {
        public ProfileAction registration = new ProfileAction();
        public ProfileAction emailConfirmation = new ProfileAction();
        public ProfileAction mobileConfirmation = new ProfileAction();
        public ProfileAction passwordConfirmation = new ProfileAction();
        public ProfileAction twofactorConfirmation = new ProfileAction();
    }



    [Serializable]
    public class ProfileAction
    {
        public string action = Config.None;
        public string created = DateTime.Now.Ticks.ToString();
        public string platform = "";
        public string token = "";
        public string status = "pending";   // pending / active / suspend
    }


    [Serializable]
    public class ProfileRole
    {
        public string roleType = ProfileRoleType.Guest;   // default guest role
        public string created = DateTime.Now.Ticks.ToString();
        public string platform = "";
        public string status = "pending";   // pending / active / suspend
    }


    public static class ProfileRoleType
    {
        /// <summary>
        /// Permission to delete profile, create admin profile, in all system
        /// </summary>
        public static String Master { get { return "master"; } }
        /// <summary>
        /// Permission to change any date with its own content (workspace, members) 
        /// </summary>
        public static String Admin { get { return "admin"; } }
        public static String Tester { get { return "tester"; } }
        public static String Client { get { return "client"; } }
        public static String Guest { get { return "guest"; } }


        public static string GetRole(string _roleType)
        {
            switch (_roleType.ToLower())
            {
                case "master":
                    return ProfileRoleType.Master;
                case "admin":
                    return ProfileRoleType.Admin;
                case "tester":
                    return ProfileRoleType.Tester;
                case "client":
                    return ProfileRoleType.Client;
                case "guest":
                    return ProfileRoleType.Guest;
                default:
                    return ProfileRoleType.Guest;
            }


        }
    }




}