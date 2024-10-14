using IfcToolkit;
using MetaBIM;
using MetaBIM.CodeChecking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;



[Serializable]
public class BimModel : MonoBehaviour
{
    public string attachedVersion;

    // Main structures, with all element attached
    public List<StructureNode> Structures = new List<StructureNode>();
  
    public int StructureItemCount;

    public StructureNode StructureSpatialRoot;

    public StructureNode StructureObjectRoot;
    public StructureNode StructureIfcClassRoot;
    public StructureNode StructureUniClassRoot;
    public StructureNode StructureZoneRoot;
    public StructureNode StructureCodeCheckingRoot;
    public StructureNode StructureCustomRoot;  // this is 
    public StructureNode StructureRoomRoot;  // from IFC SPACE


    // added for material 240502
    public StructureNode StructureMaterialRoot;

    [Header("Status")]
    public Bounds CombinedBound;
    public List<string> IfcProcessFilter;

    public Color LineColor = Color.black;
    public float LineWidth = 1f;


    [Header("Config")]
    public bool IsDrawEdgeLine;
    public bool IsDisplaySearchResult;
    public bool IsIsolationMode;
    public bool IsMutipleSelection;

    // Extra Information
    [Header("Extra Information")]

    public List<BimLevel> levels = new List<BimLevel>();
    public List<Material> materials = new List<Material>();
    public List<BimMaterial> bimMaterials = new List<BimMaterial>();

    private int NodeIDCount = 0;

    public void Add(StructureNode _node)
    {
        if (_node != null)
        {
            _node.nodeID = 0;
            Structures.Add(_node);
            StructureItemCount++;
            NodeIDCount++;

            if (_node.element.Renderer != null)
            {
                _node.IsGeometry = true;

                IfcAttributes attributes = _node.element.gameObject.GetComponent<IfcAttributes>();
                string _ifcName = _node.element.name;

                if (attributes != null)
                {
                    _ifcName = attributes.Find("IfcElementType");
                }

                if (!IfcProcessFilter.Exists(x => x == _ifcName))
                {
                    EncapsulateBound(_node.element.GetComponent<Collider>().bounds);
                }
                else
                {
                    Debug.Log("Skip object :" + _ifcName);
                }

            }
            else
            {
                _node.IsGeometry = false;
            }
        }

        StructureItemCount = Structures.Count;


    }

    public void Sort()
    {
        Structures.Sort((x, y) => (x.itemID.CompareTo(y.itemID)));
    }

    public void EncapsulateBound(Bounds _newBound)
    {
        CombinedBound.Encapsulate(_newBound);
    }


    public GameObject GetObject(string _ifcID)
    {
        return null;
    }


    public void SetLineConfig(Color lineColor, float lineWidth)
    {
        LineColor = lineColor;
        LineWidth = lineWidth;
    }


    public void AddMaterial(Material _materials)
    {

        foreach (Material mat in materials)
        {
            if (mat.name == _materials.name)
            {
                return;
            }

        }

        materials.Add(_materials);
    }


    public void ProcessLevelBound()
    {
        levels.Sort((p1, p2) => p2.LevelHeightMin.CompareTo(p1.LevelHeightMin));

        foreach(BimLevel item in levels)
        {
            item.levelBounds = CombinedBound;
        }
    }




    #region process_data
    


    public void ProcessObjectHierachy_Sptial()
    {

        ProcessObjectHierachy_SptialLoop(StructureSpatialRoot, null);
    }


    public void ProcessObjectHierachy_SptialLoop(StructureNode _node, StructureNode _parentNode)
    {

 
        if (_node.element != null) 
        {
            if (_node.element.LinkedNodeItem != null)
            {
                _node.element.LinkedNodeItem = _node;
                _node.element.LinkedNodeItem.parentNode = _parentNode;
            }
            _node.element.RestoreObject();
        }

        _node.IsHided = false;
            
        if(_node.nodeDepth > 3)
        {
            _node.IsCollapsed = true;
        }
        else
        {
            _node.IsCollapsed = false;
        }
        
        if (_node.childrenNodes.Count > 0)
        {
            foreach(var node in _node.childrenNodes)
            {
                ProcessObjectHierachy_SptialLoop(node, _node);
            }
        }
    }
    

    public void ProcessObjectHierachy_Object()
    {

        BIMElement element = StructureSpatialRoot.element;

        Dictionary<string, List<StructureNode>> structureBuffer = new Dictionary<string, List<StructureNode>>();

        int index = 1;
        StructureObjectRoot = new StructureNode(null, element);
        StructureObjectRoot.nodeDepth = 1;
        StructureObjectRoot.itemName = element.name;
        StructureObjectRoot.nodeID = 0;
        StructureObjectRoot.IsCollapsed = false;

        bimMaterials = new List<BimMaterial>();

        for (int i = 1; i < Structures.Count; i++)
        {
            StructureNode node = Structures[i];
      
            string objectKey = node.element.BimObject.GerRecord(node.element.SelectedVersion).ifcAttribute.Find("PredefinedType");

            if (objectKey == "" || objectKey == null)
            {
                objectKey = "Not Defined";
            }

            // Add to material list
            string material = GetMaterialNameFromBimObject(node);
            AddToBIMMaterialList(material, objectKey);

            // create new object node
            StructureNode newNode = new StructureNode();


            // Set Value
            node.CopyValue(newNode);// why need to copy value?
            newNode.nodeDepth = 3;
            newNode.element.LinkedNodeItem = newNode;
            
            // Restore Object
            newNode.element.RestoreObject();

            if (!structureBuffer.ContainsKey(objectKey))
            {
                // Create new Pair
                structureBuffer.Add(objectKey, new List<StructureNode>() { newNode });
            }
            else
            {
                // create connect object
                structureBuffer[objectKey].Add(newNode);
            }
            index++;
        }


        foreach (KeyValuePair<string, List<StructureNode>> pair in structureBuffer)
        {
            // create new connect node
            StructureNode newNode = new StructureNode();

            // Set Value
            newNode.nodeDepth = 2;
            newNode.itemName = pair.Key;

            newNode.childrenNodes = pair.Value;
            newNode.structureType = StructureType.connect;
            newNode.IsCollapsed = true;


            // add to root
            StructureObjectRoot.childrenNodes.Add(newNode);
            newNode.parentNode = StructureObjectRoot;
            newNode.Content = pair.Key;


            foreach (StructureNode item in pair.Value)
            {
                item.parentNode = newNode;
            }

            //newNode.childrenNodes.Sort((x, y) => x.itemName.CompareTo(y.itemName));
        }

        // Sort 
        StructureObjectRoot.childrenNodes.Sort((x, y) => x.itemName.CompareTo(y.itemName));

    }


    public void ProcessObjectHierachy_IfcClass()
    {
        BIMElement element = StructureSpatialRoot.element;

        Dictionary<string, List<StructureNode>> structureBuffer = new Dictionary<string, List<StructureNode>>();

        int index = 1;
        StructureIfcClassRoot = new StructureNode(null, element);
        StructureIfcClassRoot.nodeDepth = 1;
        StructureIfcClassRoot.itemName = element.name;
        StructureIfcClassRoot.nodeID = 0;
        StructureIfcClassRoot.IsCollapsed = false;

        
        for (int i = 1; i < Structures.Count; i++)
        {
            StructureNode node = Structures[i];
    
            
            string objectKey = node.element.BimObject.GerRecord(node.element.SelectedVersion).ifcParameter.Find("Export to IFC As");

            if (objectKey == "" || objectKey == null)
            {
                objectKey = "Unassigned";
            }

            // create new object node
            StructureNode newNode = new StructureNode();
            // Set Value
            node.CopyValue(newNode);
            newNode.nodeDepth = 3;
            newNode.element.LinkedNodeItem = newNode;
            
            // Restore Object
            newNode.element.RestoreObject();
            
            if (!structureBuffer.ContainsKey(objectKey))
            {
                // Create new Pair
                structureBuffer.Add(objectKey, new List<StructureNode>() { newNode });
            }
            else
            {
                // create connect object
                structureBuffer[objectKey].Add(newNode);
            }
            index++;
        }


        foreach (KeyValuePair<string, List<StructureNode>> pair in structureBuffer)
        {
            // create new connect node
            StructureNode newNode = new StructureNode();

            // Set Value
            newNode.nodeDepth = 2;
            newNode.itemName = pair.Key;

            newNode.childrenNodes = pair.Value;
            newNode.structureType = StructureType.connect;
            newNode.IsCollapsed = true;


            // add to root
            StructureIfcClassRoot.childrenNodes.Add(newNode);
            newNode.parentNode = StructureIfcClassRoot;
            newNode.Content = pair.Key;

            foreach (StructureNode item in pair.Value)
            {
                item.parentNode = newNode;
            }

            //newNode.childrenNodes.Sort((x, y) => x.itemName.CompareTo(y.itemName));
        }

        // Sort 
        StructureIfcClassRoot.childrenNodes.Sort((x, y) => x.itemName.CompareTo(y.itemName));

    }


    public void ProcessObjectHierachy_Uniclass()
    {
        BIMElement element = StructureSpatialRoot.element;

        Dictionary<string, List<StructureNode>> structureBuffer = new Dictionary<string, List<StructureNode>>();

        int index = 1;
        StructureUniClassRoot = new StructureNode(null, element);
        StructureUniClassRoot.nodeDepth = 1;
        StructureUniClassRoot.itemName = element.name;
        StructureUniClassRoot.nodeID = 0;
        StructureUniClassRoot.IsCollapsed = false;
        
        for (int i = 1; i < Structures.Count; i++)
        {
            StructureNode node = Structures[i];

            string objectKey = node.element.BimObject.GerRecord(node.element.SelectedVersion).ifcUniclass.Find("Code");

            if (objectKey == "" || objectKey == null)
            {
                objectKey = "Unassigned";
            }

            // create new object node
            StructureNode newNode = new StructureNode();
            // Set Value
            node.CopyValue(newNode);
            newNode.nodeDepth = 3;
            newNode.element.LinkedNodeItem = newNode;
            
            // Restore Object
            newNode.element.RestoreObject();
            
            if (!structureBuffer.ContainsKey(objectKey))
            {
                // Create new Pair
                structureBuffer.Add(objectKey, new List<StructureNode>() { newNode });
            }
            else
            {
                // create connect object
                structureBuffer[objectKey].Add(newNode);
            }
            index++;
        }


        foreach (KeyValuePair<string, List<StructureNode>> pair in structureBuffer)
        {
            // create new connect node
            StructureNode newNode = new StructureNode();

            // Set Value
            newNode.nodeDepth = 2;
            newNode.itemName = pair.Key;

            newNode.childrenNodes = pair.Value;
            newNode.structureType = StructureType.connect;
            newNode.IsCollapsed = true;
            
            // add to root
            StructureUniClassRoot.childrenNodes.Add(newNode);
            newNode.parentNode = StructureUniClassRoot;
            newNode.Content = pair.Key;

            foreach (StructureNode item in pair.Value)
            {
                item.parentNode = newNode;
            }

            //newNode.childrenNodes.Sort((x, y) => x.itemName.CompareTo(y.itemName));
        }

        // Sort 
        StructureUniClassRoot.childrenNodes.Sort((x, y) => x.itemName.CompareTo(y.itemName));

    }

    public void ProcessObjectHierachy_Custom()
    {
        BIMElement element = StructureSpatialRoot.element;

        Dictionary<string, List<StructureNode>> structureBuffer = new Dictionary<string, List<StructureNode>>();

        int index = 1;
        StructureCustomRoot = new StructureNode(null, element);
        StructureCustomRoot.nodeDepth = 1;
        StructureCustomRoot.itemName = element.name;
        StructureCustomRoot.nodeID = 0;
        StructureCustomRoot.IsCollapsed = false;

        for (int i = 1; i < Structures.Count; i++)
        {
            StructureNode node = Structures[i];

            string objectKey = node.element.BimObject.GerRecord(node.element.SelectedVersion).ifcUniclass.Find("Code");

            if (objectKey == "" || objectKey == null)
            {
                objectKey = "Unassigned";
            }

            // create new object node
            StructureNode newNode = new StructureNode();
            // Set Value
            node.CopyValue(newNode);
            newNode.nodeDepth = 3;
            newNode.element.LinkedNodeItem = newNode;

            // Restore Object
            newNode.element.RestoreObject();

            if (!structureBuffer.ContainsKey(objectKey))
            {
                // Create new Pair
                structureBuffer.Add(objectKey, new List<StructureNode>() { newNode });
            }
            else
            {
                // create connect object
                structureBuffer[objectKey].Add(newNode);
            }
            index++;
        }


        foreach (KeyValuePair<string, List<StructureNode>> pair in structureBuffer)
        {
            // create new connect node
            StructureNode newNode = new StructureNode();

            // Set Value
            newNode.nodeDepth = 2;
            newNode.itemName = pair.Key;

            newNode.childrenNodes = pair.Value;
            newNode.structureType = StructureType.connect;
            newNode.IsCollapsed = true;

            // add to root
            StructureUniClassRoot.childrenNodes.Add(newNode);
            newNode.parentNode = StructureUniClassRoot;
            newNode.Content = pair.Key;

            foreach (StructureNode item in pair.Value)
            {
                item.parentNode = newNode;
            }

            //newNode.childrenNodes.Sort((x, y) => x.itemName.CompareTo(y.itemName));
        }

        // Sort 
        StructureCustomRoot.childrenNodes.Sort((x, y) => x.itemName.CompareTo(y.itemName));

    }


    public void ProcessObjectHierachy_Material()
    {

        BIMElement element = StructureSpatialRoot.element;

        Dictionary<string, List<StructureNode>> structureBuffer = new Dictionary<string, List<StructureNode>>();

        int index = 1;
        StructureZoneRoot = new StructureNode(null, element);
        StructureZoneRoot.nodeDepth = 1;
        StructureZoneRoot.itemName = element.name;
        StructureZoneRoot.nodeID = 0;
        StructureZoneRoot.IsCollapsed = false;

        for (int i = 1; i < Structures.Count; i++)
        {
            StructureNode node = Structures[i];

            if (ProjectConfiguration.Instance.IsShowSplitedObjects && node.element.SplitedObjects.Count > 0)
            {
                //Debug.Log("ProcessObjectHierachy_Zone: " + PageStatus.IsShowZoneSplitedObjects);
                node.element.RestoreObject();

                foreach (GameObject subItem in node.element.SplitedObjects)
                {
                    BIMElement subElement = subItem.GetComponent<BIMElement>();

                    string subKey = subElement.BimObject.GerRecord(subElement.SelectedVersion).ifcMaterials.Find("Zone Name");
                    //Debug.Log("Zone: " + subKey + " from, " + subElement.name);

                    if (subKey == "" || subKey == null)
                    {
                        subKey = "Not Defined";
                    }

                    // create new object node
                    StructureNode newSubNode = new StructureNode();
                    // Set Value
                    //node.CopyValue(newSubNode);
                    newSubNode.element = subElement;
                    newSubNode.itemName = subElement.name;
                    newSubNode.Content = newSubNode.itemName;
                    newSubNode.ObjectType = node.ObjectType;
                    newSubNode.IsIfcTypeGroup = node.IsIfcTypeGroup;

                    newSubNode.nodeDepth = 3;
                    newSubNode.element.LinkedNodeItem = newSubNode;

                    // Restore Object
                    newSubNode.element.RestoreObject();

                    if (!structureBuffer.ContainsKey(subKey))
                    {
                        // Create new Pair
                        structureBuffer.Add(subKey, new List<StructureNode>() { newSubNode });
                    }
                    else
                    {
                        // create connect object
                        structureBuffer[subKey].Add(newSubNode);
                    }
                    index++;
                }

            }
            // just use the original objects
            else
            {
                string objectKey = node.element.BimObject.GerRecord(node.element.SelectedVersion).ifcMaterials.Find("Zone Name");

                if (objectKey == "" || objectKey == null)
                {
                    objectKey = "Not Defined";
                }

                // create new object node
                StructureNode newNode = new StructureNode();
                // Set Value
                node.CopyValue(newNode);
                newNode.nodeDepth = 3;
                newNode.element.LinkedNodeItem = newNode;

                // Restore Object
                newNode.element.RestoreObject();

                if (!structureBuffer.ContainsKey(objectKey))
                {
                    // Create new Pair
                    structureBuffer.Add(objectKey, new List<StructureNode>() { newNode });
                }
                else
                {
                    // create connect object
                    structureBuffer[objectKey].Add(newNode);
                }
                index++;
            }

            // Process Tree for split item in ZONE

        }





        foreach (KeyValuePair<string, List<StructureNode>> pair in structureBuffer)
        {
            // this is hard code filter

            if (ProjectConfiguration.Instance.IsShowSplitedObjects)
            {
                if (pair.Key.ToLower() == "unassigned")
                {
                    continue;
                }
            }

            // create new connect node
            StructureNode newNode = new StructureNode();

            // Set Value
            newNode.nodeDepth = 2;
            newNode.itemName = pair.Key;

            newNode.childrenNodes = pair.Value;
            newNode.structureType = StructureType.connect;
            newNode.IsCollapsed = true;

            // add to root
            StructureZoneRoot.childrenNodes.Add(newNode);
            newNode.parentNode = StructureZoneRoot;
            newNode.Content = pair.Key;

            foreach (StructureNode item in pair.Value)
            {
                item.parentNode = newNode; ;
            }

            //newNode.childrenNodes.Sort((x, y) => x.itemName.CompareTo(y.itemName));
        }

        // Sort 
        StructureZoneRoot.childrenNodes.Sort((x, y) => x.itemName.CompareTo(y.itemName));

    }


    public void ProcessObjectHierachy_Zone()
    {

        BIMElement element = StructureSpatialRoot.element;

        Dictionary<string, List<StructureNode>> structureBuffer = new Dictionary<string, List<StructureNode>>();

        int index = 1;
        StructureZoneRoot = new StructureNode(null, element);
        StructureZoneRoot.nodeDepth = 1;
        StructureZoneRoot.itemName = element.name;
        StructureZoneRoot.nodeID = 0;
        StructureZoneRoot.IsCollapsed = false;
        
        for (int i = 1; i < Structures.Count; i++)
        {
            StructureNode node = Structures[i];

            if (ProjectConfiguration.Instance.IsShowSplitedObjects && node.element.SplitedObjects.Count > 0)
            {
                //Debug.Log("ProcessObjectHierachy_Zone: " + PageStatus.IsShowZoneSplitedObjects);
                node.element.RestoreObject();
                
                foreach (GameObject subItem in node.element.SplitedObjects)
                {
                    BIMElement subElement = subItem.GetComponent<BIMElement>();

                    string subKey = subElement.BimObject.GerRecord(subElement.SelectedVersion).ifcZone.Find("Zone Name");
                    //Debug.Log("Zone: " + subKey + " from, " + subElement.name);

                    if (subKey == "" || subKey == null)
                    {
                        subKey = "Not Defined";
                    }

                    // create new object node
                    StructureNode newSubNode = new StructureNode();
                    // Set Value
                    //node.CopyValue(newSubNode);
                    newSubNode.element = subElement;
                    newSubNode.itemName = subElement.name;
                    newSubNode.Content = newSubNode.itemName;
                    newSubNode.ObjectType = node.ObjectType;
                    newSubNode.IsIfcTypeGroup = node.IsIfcTypeGroup;

                    newSubNode.nodeDepth = 3;
                    newSubNode.element.LinkedNodeItem = newSubNode;
                    
                    // Restore Object
                    newSubNode.element.RestoreObject();

                    if (!structureBuffer.ContainsKey(subKey))
                    {
                        // Create new Pair
                        structureBuffer.Add(subKey, new List<StructureNode>() { newSubNode });
                    }
                    else
                    {
                        // create connect object
                        structureBuffer[subKey].Add(newSubNode);
                    }
                    index++;
                }

            }
            // just use the original objects
            else
            {
                string objectKey = node.element.BimObject.GerRecord(node.element.SelectedVersion).ifcZone.Find("Zone Name");

                if (objectKey == "" || objectKey == null)
                {
                    objectKey = "Not Defined";
                }

                // create new object node
                StructureNode newNode = new StructureNode();
                // Set Value
                node.CopyValue(newNode);
                newNode.nodeDepth = 3;
                newNode.element.LinkedNodeItem = newNode;
                
                // Restore Object
                newNode.element.RestoreObject();

                if (!structureBuffer.ContainsKey(objectKey))
                {
                    // Create new Pair
                    structureBuffer.Add(objectKey, new List<StructureNode>() { newNode });
                }
                else
                {
                    // create connect object
                    structureBuffer[objectKey].Add(newNode);
                }
                index++;
            }

            // Process Tree for split item in ZONE

        }





        foreach (KeyValuePair<string, List<StructureNode>> pair in structureBuffer)
        {
            // this is hard code filter

            if (ProjectConfiguration.Instance.IsShowSplitedObjects)
            {
                if (pair.Key.ToLower() == "unassigned")
                {
                    continue;
                }
            }
            
            // create new connect node
            StructureNode newNode = new StructureNode();

            // Set Value
            newNode.nodeDepth = 2;
            newNode.itemName = pair.Key;

            newNode.childrenNodes = pair.Value;
            newNode.structureType = StructureType.connect;
            newNode.IsCollapsed = true;
            
            // add to root
            StructureZoneRoot.childrenNodes.Add(newNode);
            newNode.parentNode = StructureZoneRoot;
            newNode.Content = pair.Key;

            foreach (StructureNode item in pair.Value)
            {
                item.parentNode = newNode;;
            }

            //newNode.childrenNodes.Sort((x, y) => x.itemName.CompareTo(y.itemName));
        }

        // Sort 
        StructureZoneRoot.childrenNodes.Sort((x, y) => x.itemName.CompareTo(y.itemName));

    }



    public void ProcessObjectHierachy_Room()
    {

        if(RoomHandler.Instance.roomStatus == RoomHandler.RoomStatus.processd)
        {
            ProjectModelHandler.Instance.IsolatedActiveModels();
            return;
        }

        BIMElement element = StructureSpatialRoot.element;

        Dictionary<string, List<StructureNode>> structureBuffer = new Dictionary<string, List<StructureNode>>();

        int index = 1;
        StructureRoomRoot = new StructureNode(null, element);
        StructureRoomRoot.nodeDepth = 1;
        StructureRoomRoot.itemName = "Rooms (IfcSpace)";
        StructureRoomRoot.nodeID = 0;
        StructureRoomRoot.IsCollapsed = false;

        // create list of storey
        // create list of room for each storey

        StructureRoomRoot.childrenNodes= RoomGenerator.Instance.structureNodes;


        // make sure element in the list in belong to the room 

        for (int i = 1; i < Structures.Count; i++)
        {
            StructureNode node = Structures[i];
            RoomGenerator.Instance.AddElementIntoRoomNode(node);


        }





        RoomHandler.Instance.roomStatus = RoomHandler.RoomStatus.processd;   
    }


    // WIP
    // create sub node for each result
    // add 
    public void ProcessObjectHierachy_CodeChecking(CodeRule _codeRule = null)
    {
        BIMElement element = StructureSpatialRoot.element;

        Dictionary<string, List<StructureNode>> structureBuffer = new Dictionary<string, List<StructureNode>>();

        int index = 1;
        StructureCodeCheckingRoot = new StructureNode(null, element);
        StructureCodeCheckingRoot.nodeDepth = 1;
        StructureCodeCheckingRoot.itemName = element.name;
        StructureCodeCheckingRoot.nodeID = 0;
        StructureCodeCheckingRoot.IsCollapsed = false;

        
        for (int i = 1; i < Structures.Count; i++)
        {
            StructureNode node = Structures[i];

            // this need to change to find result bassed on all passess
            // if all pass, return pass
            // if one fail, return fail
            // if one warning, return warning
            // if one not checked, return not checked

            //string objectKey = node.element.BimObject.GerRecord(node.element.SelectedVersion).IfcValidation.validation;
            BimObjectRecord record = node.element.BimObject.GerRecord(node.element.SelectedVersion);

            string objectKey;
            
            bool ToSave = false;
            
            if (CodeCheckingController.Instance.SelectedCodeRule.guid != "-1")
            {
                if (record.IfcValidation.IsCheckedForCodeID(CodeCheckingController.Instance.SelectedCodeRule.guid))
                {
                    ToSave = true;
                }
            }
            else
            {
                ToSave = true;
            }


            if (ToSave)
            {
                objectKey = record.IfcValidation.GetResultByID(CodeCheckingController.Instance.SelectedCodeRule.guid);
            }
            else
            {
                objectKey = record.IfcValidation.validation;

                if (objectKey == "" || objectKey == null)
                {
                    objectKey = "Unvalidated";
                }

            }

            // TO DO, this is not safe way to set key
            objectKey = objectKey.ToLower();

            // create new object node
            StructureNode newNode = new StructureNode();
            // Set Value
            node.CopyValue(newNode);
            newNode.nodeDepth = 3;
            newNode.element.LinkedNodeItem = newNode;
            
            // Restore Object
            newNode.element.RestoreObject();
    

            if (!structureBuffer.ContainsKey(objectKey))
            {
                // Create new Pair
                
                if (ToSave)
                {
                    structureBuffer.Add(objectKey, new List<StructureNode>() { newNode });
                }
                else
                {
                    structureBuffer.Add(objectKey, new List<StructureNode>());
                }
  
            }
            else
            {
                if (ToSave)
                {
                    // create connect object
                    structureBuffer[objectKey].Add(newNode);
                }
                
            }

            index++;
     
        }


        foreach (KeyValuePair<string, List<StructureNode>> pair in structureBuffer)
        {
            


            // create new connect node
            StructureNode newNode = new StructureNode();

            // Set Value
            newNode.nodeDepth = 2;
            newNode.itemName = pair.Key;
            newNode.childrenNodes = pair.Value;
            newNode.structureType = StructureType.connect;
            newNode.IsCollapsed = true;
            
    
            
            // add to root
            StructureCodeCheckingRoot.childrenNodes.Add(newNode);
            newNode.parentNode = StructureCodeCheckingRoot;
            newNode.Content = pair.Key;

            foreach (StructureNode item in pair.Value)
            {
                item.parentNode = newNode;
            }

            //newNode.childrenNodes.Sort((x, y) => x.itemName.CompareTo(y.itemName));
        }

        // Sort 
        StructureCodeCheckingRoot.childrenNodes.Sort((x, y) => x.itemName.CompareTo(y.itemName));

    }

    public void ProcessObjectHierachy_CodeChecking_IfcClass()
    {
        BIMElement element = StructureSpatialRoot.element;

        Dictionary<string, List<StructureNode>> structureBuffer = new Dictionary<string, List<StructureNode>>();

        int index = 1;
        StructureCodeCheckingRoot = new StructureNode(null, element);
        StructureCodeCheckingRoot.nodeDepth = 1;
        StructureCodeCheckingRoot.itemName = element.name;
        StructureCodeCheckingRoot.nodeID = 0;
        StructureCodeCheckingRoot.IsCollapsed = false;


        for (int i = 1; i < Structures.Count; i++)
        {
            StructureNode node = Structures[i];

            string objectKey = node.element.BimObject.GerRecord(node.element.SelectedVersion).ifcParameter.Find("Export to IFC As");

            if (objectKey == "" || objectKey == null)
            {
                objectKey = "Unassigned";
            }

            // create new object node
            StructureNode newNode = new StructureNode();
            // Set Value
            node.CopyValue(newNode);
            newNode.nodeDepth = 3;
            newNode.element.LinkedNodeItem = newNode;


            //check result
            MetaBIM_IfcValidation metaBIM_IfcValidation = newNode.element.BimObject.GerRecord(newNode.element.SelectedVersion).IfcValidation;

            bool isFailed = false;

            if(metaBIM_IfcValidation.result.Count == 0)
            {
                newNode.ColorCode = 0;
            }
            else
            {
                newNode.ColorCode = 2;  // passed
                
                for (int j = 0; j< metaBIM_IfcValidation.result.Count; j++)
                {
                    if (metaBIM_IfcValidation.result[j] == "failed")
                    {
                        isFailed = true;
                        newNode.ColorCode = 1; // failed
                        break;
                    }
                }
            }



            // Restore Object
            newNode.element.RestoreObject();

            if (!structureBuffer.ContainsKey(objectKey))
            {
                // Create new Pair
                structureBuffer.Add(objectKey, new List<StructureNode>() { newNode });
            }
            else
            {
                // create connect object
                structureBuffer[objectKey].Add(newNode);
            }
            index++;
        }


        foreach (KeyValuePair<string, List<StructureNode>> pair in structureBuffer)
        {
            // create new connect node
            StructureNode newNode = new StructureNode();

            // Set Value
            newNode.nodeDepth = 2;
            newNode.itemName = pair.Key;

            newNode.childrenNodes = pair.Value;
            newNode.structureType = StructureType.connect;
            newNode.IsCollapsed = true;


            // add to root
            StructureCodeCheckingRoot.childrenNodes.Add(newNode);
            newNode.parentNode = StructureIfcClassRoot;
            newNode.Content = pair.Key;

            int pass_count = 0;
            int fail_count = 0;

            foreach (StructureNode item in pair.Value)
            {
                item.parentNode = newNode;
                MetaBIM_IfcValidation metaBIM_IfcValidation = item.element.BimObject.GerRecord(item.element.SelectedVersion).IfcValidation;

                for (int j = 0; j < metaBIM_IfcValidation.result.Count; j++)
                {
                    if (metaBIM_IfcValidation.result[j] == "failed")
                    {
                        fail_count++;
                    }else if (metaBIM_IfcValidation.result[j] == "passed")
                    {
                        pass_count++;
                    }
                }

                string _resultContent = "[" + pass_count + "|" + fail_count + "] " + item.Content;
                item.Content = _resultContent;

            }

            newNode.childrenNodes.Sort((y, x) => x.ColorCode.CompareTo(y.ColorCode));
        }

        // Sort 
        StructureCodeCheckingRoot.childrenNodes.Sort((x, y) => x.itemName.CompareTo(y.itemName));

    }


    public string GetLevelName(float _elementCenterHeight, int Unit = 1000)
    {
        float _height = _elementCenterHeight * Unit;  // unit is in mm
        //Debug.Log("_height: " + _height);


        if (_height > levels[0].LevelCurrentHeight || _height < levels[levels.Count-1].LevelCurrentHeight)
        {
            //Debug.Log("Max: " + levels[0].LevelCurrentHeight);
            //Debug.Log("Mix: " + levels[levels.Count - 1].LevelCurrentHeight);

            return "";
        }

        for(int i = 0; i < levels.Count-1; i++)
        {
            float top = levels[i].LevelCurrentHeight;
            float bottom = levels[i+1].LevelCurrentHeight;

            //Debug.Log("top: " + top);
            //Debug.Log("bottom: " + bottom);

            if (_height >= bottom && _height <= top)
            {
                return levels[i + 1].LevelName;
            }
        }

        return "";
    }


    public void RestoreAllElement()
    {
        foreach (StructureNode node in Structures)
        {
            if (node.element != null)
            {
                node.element.RestoreObject();
            }
        }
    }

    public void IsolateAllElement()
    {
        foreach (StructureNode node in Structures)
        {
            if (node.element != null)
            {
                node.element.SetToIsolatedMode();
            }
        }
    }

    #endregion



    #region Model Configuration

    public enum StructureType
    {
        node,
        connect,
    }



    #endregion


    #region Data Contrller

    public string GetElementID()
    {
        return "";
    }


    public StructureNode GetStructureNodeByItemID(string _id)
    {
        foreach(var node in Structures)
        {
            if(node.itemID == _id)
            {
                return node;
            }
        }
        return null;
    }

    public void AddToBIMMaterialList(string _materialName, string _elementName)
    {
        if(_materialName == "")
        {
            return;
        }

        int index = SearchBIMMaterialListByName(_materialName, _elementName);

        BimMaterial _bimMaterial = new BimMaterial(_elementName, _materialName);

        if (index != -1)
        {
            _bimMaterial = bimMaterials[index];
        }
        else
        {
            // new material
            _bimMaterial.emissionfactorUnit = "m3";
            _bimMaterial.emissionfactorID = "0";
            _bimMaterial.emissionfactor = (UnityEngine.Random.Range(0.8f, 1.6f) * 300f).ToString("#.00") + "kg Co2e";
            _bimMaterial.materialSource = _bimMaterial.materialName; 
            _bimMaterial.materialAssigned = _bimMaterial.materialSource.Split("-")[0].Trim();

            bimMaterials.Add(_bimMaterial);

            // get carbon id

        }

        _bimMaterial.elementCount++;
        _bimMaterial.volumeCount += UnityEngine.Random.Range(100, 5255);


    }

    public int SearchBIMMaterialListByName(string _materialName, string _elementName)
    {
        int index = -1;

        //Debug.Log("Search MAT: " + _materialName + ", " + _elementName);

        for (int i = 0; i < bimMaterials.Count; i++)
        {
            if (bimMaterials[i].materialName == _materialName && bimMaterials[i].elementName == _elementName)
            {
                index = i;
                //Debug.Log("NEW MAT: " + _materialName + ", " + _elementName);
                break;
            }
        }

        //Debug.Log("Fount MAT: " + _materialName + ", " + _elementName);
        return index;
    }


    public string GetMaterialNameFromBimObject(StructureNode _node)
    {
        IfcMaterials[] mats = _node.element.gameObject.GetComponents<IfcMaterials>();

        string _materialName = "";

        if(mats.Length == 0)
        {
            return _materialName;
        }

        foreach(var mat in mats)
        {
            if(mat.materials.Count > 0)
            {
                foreach(var s in mat.materials)
                {
                    if(s.ToLower() != "id")
                    {
                        _materialName = s;
                        return _materialName;
                    }
                }

            }
        }

        return _materialName;
    }



    public void GetPropertyList(List<string> _list)
    {
        foreach (StructureNode node in Structures)
        {
            if (node.element != null)
            {
                BIMElement element = node.element;
      
                if(element.BimObject.records == null || element.BimObject.records.Count < 1)
                {
                    continue;
                }

                BimObjectRecord ifcData = element.BimObject.records[0];

                if (ifcData.ifcAttribute != null)
                {
                    foreach (var item in ifcData.ifcAttribute.attributes)
                    {
                        if (!_list.Exists(x => x == item))
                        {
                            _list.Add(item);
                        }
                    }
                }

                if (ifcData.ifcProperties != null)
                {
                    foreach (var item in ifcData.ifcProperties.properties)
                    {
                        if (!_list.Exists(x => x == item))
                        {
                            _list.Add(item);
                        }
                    }
                }
            }
        }
    }


    #endregion

}

