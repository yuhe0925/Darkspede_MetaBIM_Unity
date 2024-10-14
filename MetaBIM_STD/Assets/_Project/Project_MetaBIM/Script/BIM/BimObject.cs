using IfcToolkit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UnityEngine;
using UnityEngine.Rendering;

namespace MetaBIM
{

    [Serializable]
    public class BimObject : IModel
    {
        /* data structure */
        public string attachedVersion;
        public string attachedProject;
        public string attachedWorkspace;
        // 0 is a null object, object version starts with 1
        public int versionID = 0;


        /* ifc related */
        public string objectIstanceID = "none"; // unity object id, may not be needed
        public string elementID = "none"; // if using ifc, it is ifc id, is using dwg, it is dwg elment id
        public int ifcGeometryType = BIM_GEOMETRY_TYPE.Geometry;  // geometry is a 3D object, other is not

        /* renderer ralated */
        public string parent = "none";          // none is no parent, parent is to BimObject ID
        public List<BimObjectRecord> records;    // the actual data for this object, ordered in version


        public BimObject()
        {
            records = new List<BimObjectRecord>();
        }


        public BimObject(string attachedVersion, string attachedProject, string attachedWorkspace)
        {
            records = new List<BimObjectRecord>();

            this.attachedVersion = attachedVersion;
            this.attachedProject = attachedProject;
            this.attachedWorkspace = attachedWorkspace;
        }


        public string GetObjectName(int _index)
        {
            if(records.Count > _index)
            {
                return records[_index].objectName;
            }
            else
            {
                return "";
            }

        }

        public BimObjectRecord GerRecord(int _index)
        {
            if (_index < records.Count)
            {
                return records[_index];
            }
            else
            {
                return null;
            }
        }


        public string GetAttributeValue(int _index, string _attributeName)
        {
            return records[_index].ifcAttribute.Find(_attributeName);
        }


        public static string ToJson(BimObject _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static BimObject FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<BimObject>(_json);
        }

        public static List<BimObject> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<BimObject>>(_json);
        }

    }

    [Serializable]
    public class BimObjectRecord
    {
        public int versionType = BIM_VERSION_TYPE.Create;
        public int ifcGeometryType = BIM_GEOMETRY_TYPE.Geometry;  // geometry is a 3D object, other is not
        public string objectName = "";

        // Transform information
        public Vector3D transformPosition;
        public Vector3D transformRotation;
        public Vector3D transformScale;

        // Bound
        public Vector3D transformCenter;
        public Vector3D transformSize;

        // Geometry information
        public Vector3D[] geometryVertices;
        public Vector2D[] geometryUV;
        public int[] geometryTriangles;
        public int[] geometryIndics;
        public Vector3D[] geometryNormals;
        public string materialName = "default";

        // the property online for MetaBIM object
        public MetaBIM_Property property;

        // IFC information
        public MetaBIM_IfcHeader ifcHeader;
        public MetaBIM_IfcUnits ifcUnits;
        public MetaBIM_IfcFile ifcFile;

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




        public BimObjectRecord()
        {
            IfcValidation = new MetaBIM_IfcValidation();
        }




    }



    [Serializable]
    public class MetaBIM_Zone
    {
        public string zoneName = "New Zone";
        public int zoneID = 0;
        public Bounds bounds;

    }



    [Serializable]
    public class Vector3D
    {
        public float x;
        public float y;
        public float z;

        public Vector3D()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public Vector3D(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3D FromVecter3(Vector3 _vector)
        {
            return new Vector3D(_vector.x, _vector.y, _vector.z);
        }

        public static Vector3 FromVecter3D(Vector3D _vector)
        {
            return new Vector3(_vector.x, _vector.y, _vector.z);
        }
    }


    [Serializable]
    public class Vector2D
    {
        public float x;
        public float y;

        public Vector2D()
        {
            x = 0;
            y = 0;
        }

        public Vector2D(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2D FromVecter3(Vector2 _vector)
        {
            return new Vector2D(_vector.x, _vector.y);
        }

        public static Vector2 FromVecter2D(Vector2D _vector)
        {
            return new Vector2(_vector.x, _vector.y);
        }
    }


    public static class BIM_VERSION_TYPE
    {
        public static int Add = 0;       // add new element in the data
        public static int Update = 1;     // change existing element data
        public static int Remove = 2;    // remove the element
        public static int Create = 3;    // first time object is created
    }


    public static class BIM_GEOMETRY_TYPE
    {
        public static int Geometry = 1;
        public static int NonGeometry = 0;
    }




}