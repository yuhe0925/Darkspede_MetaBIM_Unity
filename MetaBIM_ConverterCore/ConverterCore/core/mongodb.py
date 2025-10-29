from pymongo import MongoClient
from bson import ObjectId
from core import dataset
import json


class MongoHelper:
    def __init__(self, collection: str):
        self.client = MongoClient(dataset.configuration["mongodb_uri"])
        self.db = self.client[dataset.configuration["database"]]
        self.collection = self.db[collection]

    def get_item_by_guid(self, guid: str):
        """
        Retrieve a document by its GUID.
        """
        result = self.collection.find_one({"guid": guid})
        if not result:
            print(f"[WARN] No item found with GUID: {guid}")
        return result

    def update_item_by_guid(self, guid: str, update_data: dict):

        result = self.collection.update_one(
            {"guid": guid},
            {"$set": update_data}
        )
        if result.matched_count == 0:
            print(f"[WARN] No item found with GUID: {guid}")
        else:
            print(f"[INFO] Updated {result.modified_count} document(s) for GUID: {guid}")
        return result

    def close(self):
        """Close MongoDB connection."""
        self.client.close()
