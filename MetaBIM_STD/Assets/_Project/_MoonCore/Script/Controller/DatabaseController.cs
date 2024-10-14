using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MetaBIM;

/// <summary>
/// DatabaseController 的摘要说明
/// </summary>
public class DatabaseController
{


    /*
     * Database instance created for each API call, 
     */
    private MongoClient client;
    private IMongoDatabase _db;


    private void OnConnect(string _databaseName)
    {
        client = new MongoClient(Config.DatabaseConnectionString);
        _db = client.GetDatabase(_databaseName);

    }


    public DatabaseController()
    {
        OnConnect(Config.DatabaseName);
    }





    #region MetaBIM Collection Operation

    public string AddItem<T>(T _item, string _collectionName = "")
    {
        IMongoCollection<T> collection = _db.GetCollection<T>(_collectionName != "" ? _collectionName : typeof(T).Name.ToLower());

        try
        {
            collection.InsertOne(_item);
            return "done";
        }
        catch (MongoWriteException mongoWriteException)
        {
            return "MongoWriteException: " + mongoWriteException.WriteError.Code.ToString();
        }
    }

    public string DeleteItem<T>(string _guid, string _collectionName = "")
    {
        IMongoCollection<T> collection = _db.GetCollection<T>(_collectionName != "" ? _collectionName : typeof(T).Name.ToLower());
        FilterDefinition<T> filter = Builders<T>.Filter.Eq("guid", _guid);

        try
        {
            DeleteResult result = collection.DeleteOne(filter);

            if (result.DeletedCount == 1)
            {
                return "done";
            }
            else
            {
                return "No Item Deleted";
            }
        }
        catch (MongoWriteException mongoWriteException)
        {
            return "MongoWriteException: " + mongoWriteException.WriteError.Code.ToString();
        }

    }

    public string UpdateItem<T>(T _item, string index = "guid", string _collectionName = "") where T : IModel
    {
        IMongoCollection<T> collection = _db.GetCollection<T>(_collectionName != "" ? _collectionName : typeof(T).Name.ToLower());
        FilterDefinition<T> filter = Builders<T>.Filter.Eq(index, _item.guid);

        try
        {
            ReplaceOneResult result = collection.ReplaceOne(filter, _item);

            if (result.IsAcknowledged)
            {
                if (result.MatchedCount == 1)
                {
                    return "done";
                }
                else
                {
                    return "MatchedCount: " + result.MatchedCount;
                }
            }
            else
            {
                return "Not Acknowledged";
            }

        }
        catch (MongoWriteException mongoWriteException)
        {
            return "MongoWriteException: " + mongoWriteException.WriteError.Code.ToString();
        }

    }


    public string UpsertItem<T>(T _item, string index = "guid", string _collectionName = "") where T : IModel
    {
        IMongoCollection<T> collection = _db.GetCollection<T>(_collectionName != "" ? _collectionName : typeof(T).Name.ToLower());
        FilterDefinition<T> filter = Builders<T>.Filter.Eq(index, _item.guid);
        
        try
        {
            ReplaceOneResult result = collection.ReplaceOne(filter, _item, new ReplaceOptions { IsUpsert = true });

            if (result.IsAcknowledged)
            {
                if (result.MatchedCount == 1)
                {
                    return "done";
                }
                else
                {
                    return "MatchedCount: " + result.MatchedCount;
                }
            }
            else
            {
                return "Not Acknowledged";
            }

        }
        catch (MongoWriteException mongoWriteException)
        {
            return "MongoWriteException: " + mongoWriteException.WriteError.Code.ToString();
        }

    }



    
    public string GetItemByGuid<T>(string guid, out List<T> results, string _collectionName = "")
    {
        IMongoCollection<T> collection = _db.GetCollection<T>(_collectionName != "" ? _collectionName : typeof(T).Name.ToLower());
        FilterDefinition<T> filter = Builders<T>.Filter.Eq("guid", guid);

        try
        {
            results = collection.Find(filter).ToList();
            return "done";
        }
        catch (MongoWriteException mongoWriteException)
        {
            results = new List<T>();
            return "MongoWriteException: " + mongoWriteException.WriteError.Code.ToString();
        }
    }

    public string GetItems<T>(Package filterItem, out List<T> results, string _collectionName = "")
    {
        IMongoCollection<T> collection = _db.GetCollection<T>(_collectionName != "" ? _collectionName : typeof(T).Name.ToLower());
        FilterDefinitionBuilder<T> builder = Builders<T>.Filter;
        FilterDefinition<T> filter = builder.Eq("status", Config.DevelopmentStage);

        try
        {
            foreach (Filter i in filterItem.filters)
            {
                filter = filter & builder.Eq(i.key, i.value);

                // TODO, add more filter types (greater, lesser, within, etc)
                // https://stackoverflow.com/questions/55477711/how-to-chain-filterdefinitionbuilders-in-mongo-net-driver
            }
            results = collection.Find(filter).ToList();
            return "done";
        }
        catch (MongoWriteException mongoWriteException)
        {
            results = new List<T>();
            return "MongoWriteException: " + mongoWriteException.WriteError.Code.ToString();
        }
    }

    /// <summary>
    /// Get items using a batch of guids
    /// </summary>
    public string GetBatchItems<T>(Package filterItem, out List<T> results, string _collectionName = "")
    {
        // input a list of item guids, and return a list of item from database
        IMongoCollection<T> collection = _db.GetCollection<T>(_collectionName != "" ? _collectionName : typeof(T).Name.ToLower());
        
        List<string> itemPool = new List<string>();

        try
        {
            List<T> items = new List<T>();
            FilterDefinition<T> filter;
            
            foreach (string guid in itemPool)
            {
                filter = Builders<T>.Filter.Eq("guid", guid);

                List<T> results_item = collection.Find(filter).ToList();
                
                if(results_item.Count == 1)
                {
                    items.Add(results_item[0]);
                }
            }

            results = items;
            return "done";
        }
        catch (MongoWriteException mongoWriteException)
        {
            results = new List<T>();
            return "MongoWriteException: " + mongoWriteException.WriteError.Code.ToString();
        }
    }


    public string CreateNewCollection<T>(string _collectionName) where T : IModel
    {
        // Create a new collection in mongoDB database
        CreateCollectionOptions option = new CreateCollectionOptions();

        try
        {
            _db.CreateCollection(_collectionName, option);
            IMongoCollection<T> collection = _db.GetCollection<T>(_collectionName);

            var indexModel = new CreateIndexModel<T>(
                Builders<T>.IndexKeys.Combine(
                    Builders<T>.IndexKeys.Text(obj => obj.guid)
                ), new CreateIndexOptions()
                {
                    Unique = true,
                    Name = "guid",
                }
            ); ;

            var result = collection.Indexes.CreateOne(indexModel);

            return result;
        }
        catch (Exception e)
        {
            return e.Message;
        }


    }

    #endregion


}



public class FilterCondition
{
    public static string eq = "eq";
    
}
