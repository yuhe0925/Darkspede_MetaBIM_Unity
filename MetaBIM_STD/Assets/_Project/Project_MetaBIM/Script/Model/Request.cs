
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MetaBIM
{

    [Serializable]
    public class Request : IModel
    {

        public string requestFirstName;
        public string requestLastName;
        public string requestEmail;
        public string requestMobile;
        public string requestMessage;
        public string requestStatus;

        /* Request Types
         * demo:  request to the system from webpage form
         * invite: request to a mobile to join the workspace
         * collab: request to join a workspace
         */
        public string requyestType;


        /* requestStatus */

        public Request()
        {

        }

        public Request(string requestFirstName, string requestLastName, string requestEmail, string requestMobile, string requestMessage, string requyestType = "Demo")
        {
            this.requestFirstName = requestFirstName;
            this.requestLastName = requestLastName;
            this.requestEmail = requestEmail;
            this.requestMobile = requestMobile;
            this.requestMessage = requestMessage;
            this.requyestType = requyestType;
            requestStatus = "pending";
        }


        public static Request FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<Request>(_json);
        }

        public static List<Request> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<Request>>(_json);
        }

        public new string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}