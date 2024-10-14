using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace MetaBIM
{
    // Serializable classes to match the JSON structure
    [System.Serializable]
    public class IfcRoomObject
    {
        public List<Room> rooms;
    }

    [System.Serializable]
    public class Room
    {
        public string room_id;
        public string room_name;
        public string room_long_name;
        public string storey_name;
        public float storey_elevation;
    }


    // Helper class to map JSON room objects to GameObjects
    [System.Serializable]
    public class RoomMapping
    {
        public Room room;
        public GameObject gameObject;
    }

    public class RoomGenerator : MonoBehaviour
    {
        public static RoomGenerator Instance;

        public string jsonUrl = "https://example.com/room_data.json"; // Replace with your URL
        public GameObject rootObject; // Root object to which all rooms will be added
        public List<RoomMapping> roomMappings = new List<RoomMapping>(); // List to map JSON rooms to GameObjects
        private Dictionary<string, Color> roomColors = new Dictionary<string, Color>(); // Store colors for each room_long_name
        private Color defaultColor = new Color(0.5f, 0.5f, 0.5f, 50f / 255f); // Transparent grey for unmapped rooms
        public LayerMask roomLayerMask; // Layer mask to filter room objects

        [Header("Buffer")]
        public List<StructureNode> structureNodes = new List<StructureNode>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Make sure the instance persists between scenes if needed
            }
            else
            {
                Destroy(gameObject); // Ensure there is only one instance of the MeshGenerator
            }
        }

        private void Start()
        {
            StartCoroutine(DownloadJson());
        }

        IEnumerator DownloadJson()
        {
            using (UnityWebRequest www = UnityWebRequest.Get(jsonUrl))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string jsonText = www.downloadHandler.text;
                    //Debug.Log("Received JSON: " + jsonText); // Debug the received JSON
                    try
                    {
                        IfcRoomObject ifcRoomObject = JsonUtility.FromJson<IfcRoomObject>(jsonText);
                        MapGameObjectsToRooms(ifcRoomObject);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("JSON Parsing Error: " + e.Message);
                    }
                }
                else
                {
                    Debug.LogError("Failed to download JSON: " + www.error);
                }
            }
        }

        void MapGameObjectsToRooms(IfcRoomObject ifcRoomObject)
        {
            // Clear previous mappings and colors
            roomMappings.Clear();
            roomColors.Clear();

            // Iterate through each child of the root object
            foreach (Transform child in rootObject.transform)
            {
                // Split the name by "_" and get the last part as the room_id
                string[] nameParts = child.name.Split('_');
                string roomObjectId = nameParts[nameParts.Length - 1];

                // Find the corresponding room in the JSON list by room_id
                Room matchingRoom = ifcRoomObject.rooms.Find(r => r.room_id == roomObjectId);

                if (matchingRoom != null)
                {
                    // Store the mapping between the JSON object and the GameObject
                    RoomMapping mapping = new RoomMapping
                    {
                        room = matchingRoom,
                        gameObject = child.gameObject
                    };
                    roomMappings.Add(mapping);

                    // Set color based on room_long_name
                    if (!roomColors.ContainsKey(matchingRoom.room_long_name))
                    {
                        // Generate a random color, excluding greyish shades
                        Color randomColor;
                        do
                        {
                            randomColor = Random.ColorHSV(0, 1, 0.5f, 1, 0.5f, 1);
                            randomColor.a = 80f / 255f; // Set transparency
                        }
                        while (randomColor.grayscale > 0.4f && randomColor.grayscale < 0.6f); // Avoid greyish colors

                        roomColors.Add(matchingRoom.room_long_name, randomColor);
                    }

                    // Apply the color to the GameObject's material with transparency settings
                    ApplyMaterialToGameObject(child.gameObject, roomColors[matchingRoom.room_long_name]);

                    // Optionally, update the GameObject's properties based on the room information
                    //Debug.Log($"Mapped Room: {matchingRoom.room_name} to GameObject: {child.name}");
                }
                else
                {
                    // Apply default grey color to unmapped rooms
                    ApplyMaterialToGameObject(child.gameObject, defaultColor);
                    //Debug.LogWarning($"No matching room found in JSON for GameObject: {child.name}. Assigned default grey color.");
                }
            }


            ConstructTreeNodes();
        }

        // Helper method to apply material with transparency settings
        void ApplyMaterialToGameObject(GameObject gameObject, Color color)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            BIMElement element = gameObject.GetComponent<BIMElement>();
            element.Filter = gameObject.GetComponent<MeshFilter>();
            element.Renderer = gameObject.GetComponent<MeshRenderer>();
            element.Collider = gameObject.GetComponent<MeshCollider>();

            //Debug.Log("ApplyMaterialToGameObject: " + gameObject.name);
            if (meshRenderer != null)
            {
                Material transparentMaterial = new Material(Shader.Find("Standard"));
                transparentMaterial.color = color;
                transparentMaterial.SetFloat("_Glossiness", 0.0f); // Optionally adjust glossiness
                transparentMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off); // Enable double-sided rendering

                // Set material to transparent mode
                transparentMaterial.SetFloat("_Mode", 3);
                transparentMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                transparentMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                transparentMaterial.SetInt("_ZWrite", 0);
                transparentMaterial.DisableKeyword("_ALPHATEST_ON");
                transparentMaterial.EnableKeyword("_ALPHABLEND_ON");
                transparentMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                transparentMaterial.renderQueue = 5000;

                meshRenderer.material = transparentMaterial;

  
                element.OriginalMaterials = new Material[] { transparentMaterial };


            }
        }


        public void ConstructTreeNodes_()
        {
            // Clear previous structure nodes
            structureNodes.Clear();
            // create a new tree structure base on storey and room
            foreach (RoomMapping mapping in roomMappings)
            {
                StructureNode storeyNode = structureNodes.Find(n => n.Content == mapping.room.storey_name);
   
                if (storeyNode == null)
                {
                    storeyNode = new StructureNode(null, null);
                    storeyNode.nodeDepth = 2;
                    storeyNode.itemID = mapping.room.storey_name;
                    storeyNode.itemName = mapping.room.storey_name;
                    storeyNode.Content = mapping.room.storey_name;
                    storeyNode.structureType = BimModel.StructureType.connect;
                    storeyNode.IsCollapsed = false;
                    structureNodes.Add(storeyNode);
                }

                StructureNode roomNode = new StructureNode(null, null);
                var element = mapping.gameObject.GetComponent<BIMElement>();

                roomNode.nodeDepth = 3;
                roomNode.itemID = mapping.room.room_id;
                roomNode.itemName = mapping.room.room_name;
                roomNode.Content = mapping.room.room_long_name + ":" + mapping.room.room_name;
                roomNode.structureType = BimModel.StructureType.node;
                roomNode.linkedObject = mapping.gameObject;
                roomNode.parentNode = storeyNode;
                element.LinkedNodeItem = roomNode;
                roomNode.element = element;
                roomNode.IsCollapsed = true;
                storeyNode.childrenNodes.Add(roomNode);
            }
        }

        public void ConstructTreeNodes()
        {
            // Clear previous structure nodes
            structureNodes.Clear();

            // Create a new tree structure based on storey, longName, and room
            foreach (RoomMapping mapping in roomMappings)
            {
                // Find or create the storey node
                StructureNode storeyNode = structureNodes.Find(n => n.Content == mapping.room.storey_name);

                if (storeyNode == null)
                {
                    storeyNode = new StructureNode(null, null)
                    {
                        nodeDepth = 2,
                        itemID = mapping.room.storey_name,
                        itemName = mapping.room.storey_name,
                        Content = mapping.room.storey_name,
                        structureType = BimModel.StructureType.connect,
                        IsCollapsed = false
                    };
                    structureNodes.Add(storeyNode);
                }

                // Find or create the longName node under the storey node
                StructureNode longNameNode = storeyNode.childrenNodes.Find(n => n.Content == mapping.room.room_long_name);

                if (longNameNode == null)
                {
                    longNameNode = new StructureNode(null, null)
                    {
                        nodeDepth = 3,
                        itemID = mapping.room.room_long_name,
                        itemName = mapping.room.room_long_name,
                        Content = mapping.room.room_long_name,
                        structureType = BimModel.StructureType.connect,
                        parentNode = storeyNode,
                        IsCollapsed = true
                    };
                    storeyNode.childrenNodes.Add(longNameNode);
                }

                // Create the room node under the longName node
                StructureNode roomNode = new StructureNode(null, null)
                {
                    nodeDepth = 4,
                    itemID = mapping.room.room_id,
                    itemName = mapping.room.room_name,
                    Content = mapping.room.room_long_name + ":" + mapping.room.room_name,
                    structureType = BimModel.StructureType.node,
                    linkedObject = mapping.gameObject,
                    parentNode = longNameNode,
                    IsCollapsed = true
                };

                // Link the room node to the BIM element
                var element = mapping.gameObject.GetComponent<BIMElement>();
                element.LinkedNodeItem = roomNode;
                roomNode.element = element;

                // Add the room node to the longName node
                longNameNode.childrenNodes.Add(roomNode);
            }
        }

        public Room GetRoomById(string id)
        {
            foreach (RoomMapping mapping in roomMappings)
            {
                if (mapping.room.room_id == id)
                {
                    return mapping.room;
                }
            }

            return null;
        }



        public Room IsGameObjectInRoomBound(GameObject gameObject, string room_id)
        {
            foreach (RoomMapping mapping in roomMappings)
            {
                
            }

            return null;
        }


        private bool PointInsideMesh(MeshCollider meshCollider, Vector3 point)
        {
            // Use raycasting to count intersections and determine if point is inside mesh
            int intersections = 0;
            Vector3 direction = Vector3.up; // Cast ray upwards

            // Cast a ray from the point and count intersections with the mesh
            Ray ray = new Ray(point, direction);
            if (meshCollider.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.layer == roomLayerMask)
                {
                    intersections++;
                }
            }

            // Cast another ray in the opposite direction to confirm
            ray = new Ray(point, -direction);
            if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.layer == roomLayerMask)
                {
                    intersections++;
                }
            }

            Debug.Log("PointInsideMesh: " + intersections);
            // If there are an odd number of intersections, the point is inside the mesh
            return intersections % 2 == 1;
        }


        public void AddElementIntoRoomNode(StructureNode node)
        {
            if (node.element != null && node.element.Collider != null)
            {

                string ifctype = node.element.BimObject.records[0].ifcAttribute.Find("IfcElementType");
                node.UnHideObject();
                node.element.SetToIsolatedMode();
                if (ifctype == null || ifctype.ToLower() != "ifcfurniture")
                {
                    return;
                }
                else
                {

                    node.element.RestoreObject();
                }


                Vector3 position = node.element.Collider.sharedMesh.bounds.center;

                //Debug.Log("AddElementIntoRoomNode: " + position);

                foreach (RoomMapping room in roomMappings)
                {
                    MeshCollider roomCollider = room.gameObject.GetComponent<MeshCollider>();
                    if (roomCollider != null)
                    {
                        // Perform a more accurate mesh-based check
                        //if (PointInsideMesh(roomCollider, position))
                        if(node.element.Collider.sharedMesh.bounds.Contains(position))
                        {
                            var roomNode = FindNodeByRoomID(room.room.room_id);

                            if (roomNode != null)
                            {
                                roomNode.childrenNodes.Add(node);
              
                                continue;
                            }
                        }
                    }
                }
            }

        }




        public StructureNode FindNodeByRoomID(string roomID)
        {
            foreach (StructureNode storeyNode in structureNodes)
            {
                // Check each storey node and its children for the room ID
                StructureNode foundNode = FindNodeInChildren(storeyNode, roomID);
                if (foundNode != null)
                {
                    return foundNode;
                }
            }

            return null; // No matching node found
        }

        // Helper method to recursively search for a node in the children nodes
        private StructureNode FindNodeInChildren(StructureNode parentNode, string roomID)
        {
            foreach (StructureNode childNode in parentNode.childrenNodes)
            {
                // If the current node matches the room ID, return it
                if (childNode.itemID == roomID)
                {
                    return childNode;
                }

                // Recursively search in the child's children
                StructureNode foundNode = FindNodeInChildren(childNode, roomID);
                if (foundNode != null)
                {
                    return foundNode;
                }
            }

            return null; // No matching node found in children
        }

    }
}
