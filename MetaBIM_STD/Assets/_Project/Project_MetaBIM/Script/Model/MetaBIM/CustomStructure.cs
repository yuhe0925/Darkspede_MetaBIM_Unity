using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;


namespace MetaBIM
{

    [Serializable]
    public class CustomStructure : IModel
    {

        public string StructureName;
        public string AttachedWorkspace; // the guid of the workspace (project)
        public CustomStructureNode root;

        public CustomStructure(string structureName, string attachedWorkspace)
        {
            StructureName = structureName;
            AttachedWorkspace = attachedWorkspace;
            root=  new CustomStructureNode(structureName, 0);
        }

        public CustomStructureNode GetNodeByID(string _nodeID)
        {
 
            return FindNodeByID(root, _nodeID);
        }

        public CustomStructureNode GetNodeByItemID(string _itemID)
        {

            return FindNodeByItemID(root, _itemID);
        }

        private CustomStructureNode FindNodeByID(CustomStructureNode currentNode, string _nodeID)
        {
            if (currentNode.nodeID == _nodeID)
            {
                return currentNode;
            }

            foreach (var child in currentNode.childrenNodes)
            {
                var foundNode = FindNodeByID(child, _nodeID);
                if (foundNode != null)
                {
                    return foundNode;
                }
            }

            return null;
        }

        private CustomStructureNode FindNodeByItemID(CustomStructureNode currentNode, string _itemID)
        {
            if (currentNode.itemID == _itemID)
            {
                return currentNode;
            }

            foreach (var child in currentNode.childrenNodes)
            {
                var foundNode = FindNodeByItemID(child, _itemID);
                if (foundNode != null)
                {
                    return foundNode;
                }
            }

            return null;
        }

        public bool OnRemoveItemNode(string _itemID)
        {
            bool result = DeleteNodeByItemID(root, _itemID);
            return result;
        }



        public bool DeleteNodeByItemID(CustomStructureNode currentNode, string _itemID)
        {
            Debug.Log("DeleteNodeByID: " + _itemID);
            // Traverse through the children of the current node
            for (int i = 0; i < currentNode.childrenNodes.Count; i++)
            {
                var child = currentNode.childrenNodes[i];

                // If the child has the matching NodeID, remove it from the list
                if (child.itemID == _itemID)
                {
                    currentNode.childrenNodes.RemoveAt(i);
                    return true; // Node successfully removed
                }

                // Recursively try to delete in the subtree
                if (DeleteNodeByItemID(child, _itemID))
                {
                    return true; // Node deleted in the subtree
                }
            }

            // If the node was not found in this subtree, return false
            return false;
        }

        // json convertion
        public static string ToJson(CustomStructure _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static string ToJsonList(List<CustomStructure> _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static CustomStructure FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<CustomStructure>(_json);
        }

        public static List<CustomStructure> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<CustomStructure>>(_json);
        }


    }





    [Serializable]
    public class CustomStructureNode
    {
        public string nodeID;  // mapped to itemID of StructureNode
        public string itemID;  // for attachemd element or object
        public string itemName;
        public int nodeDepth;
        public int nodeType; // 0 = group, 1 = element

        public List<CustomStructureNode> childrenNodes = new List<CustomStructureNode>();
        public List<LinkedElement> elements = new List<LinkedElement>();

        public CustomStructureNode(string _name, int _depth)
        {
            itemName = _name;
            nodeDepth = _depth;
            nodeID = Guid.NewGuid().ToString("N");

            childrenNodes = new List<CustomStructureNode>();
            elements = new List<LinkedElement>();
        }

    }

    [Serializable]
    public class LinkedElement
    {
        public string ElementName;
        public string ElementID;
        public bool IsLoaded;
    }

}
