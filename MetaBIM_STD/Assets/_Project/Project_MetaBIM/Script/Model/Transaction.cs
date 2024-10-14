using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;



/// <summary>
/// Summary description for Sycamore_Document
/// </summary>
namespace MetaBIM
{
    [Serializable]
    public class Transaction
    {
        public string guid;
        public string created;
        public string updated;
        public string status;
        public string tableName;




        public string createdBy;
        public string attachedProject;
        public string TransactionTitle = "Init"; // Discription
        public string TransactionAction = "create";   // which type
        public string TransactionToken = "0000";
        public string TransactionStatus = "pending";
        public int assetSize = 0;

        public string recipient;




        public Transaction()
        {
            guid = Guid.NewGuid().ToString();
            created = DateTime.Now.Ticks.ToString();
            updated = DateTime.Now.Ticks.ToString();
            tableName = this.GetType().Name;
            status = Config.DevelopmentStage;




        }




        public enum TransactionType
        {
            create = 1,
            modify = 2,
            owner = 3,
            complete = 4,
            archive = 5,
            convertion = 6,
            upload = 7,
            commenting = 8,
        }





        #region JSON
        public static string ToJson(Transaction _item)
        {
            return JsonConvert.SerializeObject(_item);
        }




        public static Transaction FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<Transaction>(_json);
        }
        #endregion
    }
}