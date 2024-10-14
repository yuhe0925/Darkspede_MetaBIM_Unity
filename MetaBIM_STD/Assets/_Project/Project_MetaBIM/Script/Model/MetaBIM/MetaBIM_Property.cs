using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace MetaBIM
{
    [Serializable]
    public class MetaBIM_Property
    {
        public int mappedID;
        public string elementID;
        public string elementName;

        
        public MetaBIM_Geometry geometry;


        public List<MetaBIM_GeneralItem> generalItems;


        public MetaBIM_IfcAttributes ifcAttribute;
        public MetaBIM_IfcProperties ifcProperties;
        public MetaBIM_IfcTypes ifcTypes;
        public MetaBIM_IfcMaterials ifcMaterials;

        public MetaBIM_IfcUniclass ifcUniclass;
        public MetaBIM_IfcUniclassMap ifcUniclassMap;
        public MetaBIM_IfcParameter ifcParameter;
        public MetaBIM_IfcZone ifcZone;
        public MetaBIM_IfcValidation IfcValidation;
        public MetaBIM_IfcEpicClass ifcEpicClass;

        public Dictionary<string, string> ExportResult;
        public MetaBIM_Property()
        {
            InitMetaBIMGeneralProperty();
        }


        public MetaBIM_Property(int mappedID,  BimObject _BimObject)
        {
            this.mappedID = mappedID;
            this.elementID = _BimObject.elementID;

            ExportResult = new Dictionary<string, string>();

            if (_BimObject.records.Count > 0)
            {
                PassIFCClass(_BimObject.records[0]);
            }

            InitMetaBIMGeneralProperty();
        }



        // tempare way to pass the class value
        public void PassIFCClass(BimObjectRecord _bimOB)
        {
            elementName = _bimOB.objectName;


            ifcAttribute = _bimOB.ifcAttribute;
            ifcProperties = _bimOB.ifcProperties;
            ifcTypes = _bimOB.ifcTypes;
            ifcMaterials = _bimOB.ifcMaterials;
            ifcUniclass = _bimOB.ifcUniclass;
            ifcUniclassMap = _bimOB.ifcUniclassMap;
            ifcParameter = _bimOB.ifcParameter; 
            ifcZone = _bimOB.ifcZone;
            IfcValidation = _bimOB.IfcValidation;
            ifcEpicClass = _bimOB.ifcEpicClass;


            // geometry
            // geometry.vertics = _bimOB.geometryVertices.ToList();
            // geometry.triangles =  _bimOB.geometryTriangles.ToList();

        }


        public void InitMetaBIMGeneralProperty()
        {
            generalItems = new List<MetaBIM_GeneralItem>();

            generalItems.Add(new MetaBIM_GeneralItem("center", "Center", "0"));
            generalItems.Add(new MetaBIM_GeneralItem("anchor", "Anchor", "0"));
            generalItems.Add(new MetaBIM_GeneralItem("position", "Position", "0, 0, 0"));
            generalItems.Add(new MetaBIM_GeneralItem("rotiation", "Rotation", "0"));
            generalItems.Add(new MetaBIM_GeneralItem("scale", "Scale", "1,1,1"));
            generalItems.Add(new MetaBIM_GeneralItem("volume", "Mesh Volume", "0"));

            // localization?

        }

    }



    [Serializable]
    public class MetaBIM_Geometry
    {

        // bound
        public Vector3D meshCenter;
        public Vector3D meshAnchor;

        // transform
        public Vector3D meshPosition;
        public Vector3D meshRotation;
        public Vector3D meshScale;

        public List<Vector3D> vertics;
        public List<int> triangles;
        public List<Vector3D> normals;

        // line?
        // face?

    }

    public class MetaBIM_GeneralItem
    {
        public string key;
        public string value;
        public string displayName;  // for localization?
        public int sortOrder = 1;
        public int itemType = 1;

        public MetaBIM_GeneralItem(string key, string displayName, string value)
        {
            this.key = key;
            this.value = value;

            this.displayName = displayName;
        }



    }

}




