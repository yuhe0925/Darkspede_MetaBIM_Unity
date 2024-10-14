using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using static OnlineMapsAMapSearchResult;
using Unity.VisualScripting;
using System;
using System.Reflection;
using UnityEngine.Networking;
using MetaBIM.Asset.SepeedLimitPoint;

namespace MetaBIM
{

    [DisallowMultipleComponent]

    public class MapController : MonoBehaviour

    {
        public static MapController Instance;
        public static OnlineMapsControlBase control;
        public OnlineMaps Map;
        public Color LineColor;
        public GISCamera gisCamera;


        [Header("Start Value (Melbourne CBD)")]
        public int DefaultZoom = 14;
        public double DefaultLongitude = 116.101373;
        public double DefaultLatitude = -31.766809;

        [Header("Select Lat and Lng")]

        public double lng1;
        public double lat1;
        public double lng2;
        public double lat2;
        public string coordinate_color;
        int cordOrder = 1;

        public List<Texture2D> MarkerTextures;

        [Header("Status")]
        public bool IsMapOn = true;


        public List<ProjectItem> Items;

        public void Start()
        {


            //getCrashPoints(lng1, lat1, lng2, lat2);
            //OnFindDirectioByGoogle(lng1, lat1, lng2, lat2);

            //findCrashWithRouteBtwTwoPoints(lng1, lat1, lng2, lat2, coordinate_color);

            //Map.control.OnMapClick += mousePointToGeoCord;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            if (control == null) control = OnlineMapsControlBase.instance;
            //Map.SetPositionAndZoom(DefaultLongitude, DefaultLatitude, DefaultZoom);
        }


        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //getCrashPoints(lng1, lat1, lng2, lat2, coordinate_color);
                //getSpeedLimitPoints(lng1, lat1, lng2, lat2, coordinate_color);

                // align camera to map
                Map.SetPositionAndZoom(DefaultLongitude, DefaultLatitude, DefaultZoom);
                gisCamera.transform.position = new Vector3(0, 1600, 0);
                gisCamera.transform.rotation = Quaternion.Euler(90, 0 , -180);

                gisCamera.OnPositionInit();

            }


            if (Input.GetKeyDown(KeyCode.K)) 
            { 


            }
        }










        public void LoadSupplyTransportInfo()
        {
            var sitePoint = Map.markerManager.items[0];

            for(int i = 1; i < Map.markerManager.items.Count; i++)
            {
                var supplyPoint = Map.markerManager.items[i];
                OnFindDirectioByGoogle(supplyPoint.position.x, supplyPoint.position.y, sitePoint.position.x, sitePoint.position.y);
            }
        }



        // find crash positions within the bounding box
        public void getCrashPoints(double _lng1, double _lat1, double _lng2, double _lat2, string color = "green")
        {

            StartCoroutine(OnRequestGeoSearch(_lng1, _lat1, _lng2, _lat2, color, getCrashPoints_CallBack));
        }
        public void getCrashPoints_CallBack(bool _success, string _message, string color)
        {
            Debug.Log("getCrashPoints_CallBack: " + _success);
            if (_success)
            {
                Asset.CrashPoint.CrashResponse response = JsonConvert.DeserializeObject<Asset.CrashPoint.CrashResponse>(_message);

      
                foreach (Asset.CrashPoint.Feature point in response.features)
                {
                    double lng, lat;
                    lng = point.geometry.x;
                    lat = point.geometry.y;
                    string name = point.attributes.ROAD_NAME;

                    Map.markerManager.Create(lng, lat, MarkerTextures[0], name);

                }
            }
        }



        public void getSpeedLimitPoints(double _lng1, double _lat1, double _lng2, double _lat2, string color = "green")
        {

            StartCoroutine(OnRequestGeoSearch_SpeedLimit(_lng1, _lat1, _lng2, _lat2, color, getpeedLimitPoints_CallBack));
        }

        public void getpeedLimitPoints_CallBack(bool _success, string _message, string color)
        {
            Debug.Log("getpeedLimitPoints_CallBack: " + _success);

            Debug.Log(_message);    


            if (_success)
            {
                Asset.SepeedLimitPoint.SpeedLimitResponse response = JsonConvert.DeserializeObject<Asset.SepeedLimitPoint.SpeedLimitResponse>(_message);


                foreach (Asset.SepeedLimitPoint.Feature point in response.features)
                {
                    double lng, lat;

                    if(point.geometry.paths.Count > 0)
                    {
                        lng = point.geometry.paths[0][0][0];
                        lat = point.geometry.paths[0][0][1];

                        string limitString = point.attributes.SPEED_LIMIT;
                        Debug.Log("limitString = " + limitString);

                        string[] speeds = limitString.Split("or");
                        string speedL = speeds[0].Trim();
                        // extract the speed limit from the speedL string
                        Texture2D texture = MarkerTextures[8];

                        if (speedL.Contains("20"))
                        {
                            texture = MarkerTextures[2];
                        }
                        else
                        if (speedL.Contains("30"))
                        {
                            texture = MarkerTextures[3];
                        }
                        else
                        if (speedL.Contains("40"))
                        {
                            texture = MarkerTextures[4];
                        }
                        else if (speedL.Contains("50"))
                        {
                            texture = MarkerTextures[5];
                        }
                        else if (speedL.Contains("60"))
                        {
                            texture = MarkerTextures[6];
                        }
                        else if (speedL.Contains("70"))
                        {
                            texture = MarkerTextures[7];
                        }
                        else if (speedL.Contains("80"))
                        {
                            texture = MarkerTextures[8];
                        }
                        else if (speedL.Contains("90"))
                        {
                            texture = MarkerTextures[9];
                        }
                        else if (speedL.Contains("100"))
                        {
                            texture = MarkerTextures[10];
                        }
                        else if (speedL.Contains("110"))
                        {
                            texture = MarkerTextures[11];
                        }
                        else if (speedL.Contains("120"))
                        {
                            texture = MarkerTextures[12];
                        }
                        else if (speedL.Contains("130"))
                        {
                            texture = MarkerTextures[13];
                        }
                        else
                        {
                            texture = MarkerTextures[8];
                        }

                        Map.markerManager.Create(lng, lat, texture, speedL);
                    }


                }
            }
        }




        //find path between two points
        public void OnFindDirectioByGoogle(double _lng1, double _lat1, double _lng2, double _lat2)
        {
            string googleAPIKey = MapConfig.API_Key_Google;
            OnlineMapsGoogleDirections request = new OnlineMapsGoogleDirections
           (
               googleAPIKey,
               new Vector2((float)_lng1, (float)_lat1), // FROM (string or Vector2)
               new Vector2((float)_lng2, (float)_lat2) // TO (string or Vector2)
           );

            // Specifies that search results must be sent to OnFindDirectionComplete.
            request.OnComplete += OnFindDirectionComplete;

            request.Send();
        }

        //call back fnction write my online maps
        private void OnFindDirectionComplete(string response)
        {
            // Get the result object.
            OnlineMapsGoogleDirectionsResult result = OnlineMapsGoogleDirections.GetResult(response);

            // Check that the result is not null, and the number of routes is not zero.
            if (result == null || result.routes.Length == 0)
            {
                Debug.Log("Find direction failed");
                Debug.Log(response);
                return;
            }

            // Showing the console instructions for each step.
            foreach (OnlineMapsGoogleDirectionsResult.Leg leg in result.routes[0].legs)
            {
                foreach (OnlineMapsGoogleDirectionsResult.Step step in leg.steps)
                {
                    Debug.Log(step.string_instructions);
                }
            }

            // Create a line, on the basis of points of the route.
            OnlineMapsDrawingLine route = new OnlineMapsDrawingLine(result.routes[0].overview_polylineD, LineColor, 4);

            // Add the line route on the map.
            Map.drawingElementManager.Add(route);
        }


        //input the points by mouse click
        public void mousePointToGeoCord()
        {

            double lng, lat;
            Map.control.GetCoords(out lng, out lat);

            if (cordOrder == 1)
            {
                lng1 = lng;
                lat1 = lat;
                cordOrder = 2;
                Debug.Log("lng: " + lng1.ToString() + " lat: " + lat1.ToString());
            }
            else
            {
                lng2 = lng;
                lat2 = lat;
                cordOrder = 1;
                Debug.Log("lng: " + lng2.ToString() + " lat: " + lat2.ToString());


                //OnFindDirectioByGoogle(lng1, lat1, lng2, lat2);
            }

        }


        // find crash positions within the bounding box with the route from p1 to p2
        public void findCrashWithRouteBtwTwoPoints(double _lng1, double _lat1, double _lng2, double _lat2, string color)
        {

            getCrashPoints(_lng1, _lat1, _lng2, _lat2, color);
            //OnFindDirectioByGoogle(lng1, lat1, lng2, lat2);


        }

        public IEnumerator OnRequestGeoSearch(double _lng1, double _lat1, double _lng2, double _lat2, string color, System.Action<bool, string, string> _Callback)
        {
            string geometry = $"{_lng1.ToString()},{_lat1.ToString()},{_lng2.ToString()},{_lat2.ToString()}";
            WWWForm form = new WWWForm();
            form.AddField("geometry", geometry);
            form.AddField("geometryType", MapConfig.CrashPointGeometryType);
            form.AddField("spatialRel", MapConfig.CrashPointSpatialRel);
            form.AddField("returnGeometry", MapConfig.CrashPointReturnGeometry);
            form.AddField("returnTrueCurves", MapConfig.CrashPointReturnTrueCurves);
            form.AddField("returnIdsOnly", MapConfig.CrashPointReturnIdsOnly);
            form.AddField("returnCountOnly", MapConfig.CrashPointReturnCountOnly);
            form.AddField("returnZ", MapConfig.CrashPointReturnZ);
            form.AddField("returnM", MapConfig.CrashPointReturnM);
            form.AddField("returnDistinctValues", MapConfig.CrashPointReturnDistinctValues);
            form.AddField("returnExtentsOnly", MapConfig.CrashPointReturnExtentsOnly);
            form.AddField("f", MapConfig.CrashPointFileType);

            string link = MapConfig.CrashPointAPI_Root;

            Debug.Log("DataProxy.OnRequestCrashPointSearch " + MethodBase.GetCurrentMethod().Name + ": " + link);


            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {

                yield return www.SendWebRequest();

                while (!www.isDone)
                {
                    yield return www;
                }

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);

                    _Callback(false, "Network Error", "");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text, color);

                }


            }
        }



        public IEnumerator OnRequestGeoSearch_SpeedLimit(double _lng1, double _lat1, double _lng2, double _lat2, string color, System.Action<bool, string, string> _Callback)
        {
            string geometry = $"{_lng1.ToString()},{_lat1.ToString()},{_lng2.ToString()},{_lat2.ToString()}";
            WWWForm form = new WWWForm();
            form.AddField("geometry", geometry);
            form.AddField("geometryType", MapConfig.CrashPointGeometryType);
            form.AddField("spatialRel", MapConfig.CrashPointSpatialRel);
            form.AddField("returnGeometry", MapConfig.CrashPointReturnGeometry);
            form.AddField("returnTrueCurves", MapConfig.CrashPointReturnTrueCurves);
            form.AddField("returnIdsOnly", MapConfig.CrashPointReturnIdsOnly);
            form.AddField("returnCountOnly", MapConfig.CrashPointReturnCountOnly);
            form.AddField("returnZ", MapConfig.CrashPointReturnZ);
            form.AddField("returnM", MapConfig.CrashPointReturnM);
            form.AddField("returnDistinctValues", MapConfig.CrashPointReturnDistinctValues);
            form.AddField("returnExtentsOnly", MapConfig.CrashPointReturnExtentsOnly);
            form.AddField("f", MapConfig.CrashPointFileType);
            form.AddField("outFields", "*");


            string link = MapConfig.SpeedLmitPointAPI_Root;

            Debug.Log("DataProxy.OnRequestCrashPointSearch " + MethodBase.GetCurrentMethod().Name + ": " + link);


            using (UnityWebRequest www = UnityWebRequest.Post(link, form))
            {

                yield return www.SendWebRequest();

                while (!www.isDone)
                {
                    yield return www;
                }

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);

                    _Callback(false, "Network Error", "");
                }
                else
                {
                    _Callback(true, www.downloadHandler.text, color);

                }


            }
        }

    }


    namespace Asset.CrashPoint
    {


        [Serializable]

        public class CrashResponse
        {
            public string displayFieldName { get; set; }
            public FieldAliases fieldAliases { get; set; }
            public string geometryType { get; set; }
            public SpatialReference spatialReference { get; set; }
            public List<Field> fields { get; set; }
            public List<Feature> features { get; set; }
            //public bool exceededTransferLimit { get; set; }
        }

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Attributes
        {
            public string ROAD_NAME { get; set; }
            public string SPEED_LIMIT { get; set; }
        }

        public class Feature
        {
            public Attributes attributes { get; set; }
            public Geometry geometry { get; set; }
        }

        public class Field
        {
            public string name { get; set; }
            public string type { get; set; }
            public string alias { get; set; }
            public int length { get; set; }
        }

        public class FieldAliases
        {
            public string ROAD_NAME { get; set; }
        }

        public class Geometry
        {
            public double x { get; set; }
            public double y { get; set; }
        }

        public class SpatialReference
        {
            public int wkid { get; set; }
            public int latestWkid { get; set; }
        }
    }



    namespace Asset.SepeedLimitPoint
    {

        public class Attributes
        {
            public int OBJECTID { get; set; }
            public string ROAD { get; set; }
            public string ROAD_NAME { get; set; }
            public string COMMON_USAGE_NAME { get; set; }
            public double START_SLK { get; set; }
            public double END_SLK { get; set; }
            public string CWY { get; set; }
            public double START_TRUE_DIST { get; set; }
            public double END_TRUE_DIST { get; set; }
            public string NETWORK_TYPE { get; set; }
            public string RA_NO { get; set; }
            public string RA_NAME { get; set; }
            public string LG_NO { get; set; }
            public string LG_NAME { get; set; }
            public string SPEED_LIMIT { get; set; }
            public int ROUTE_NE_ID { get; set; }

            [JsonProperty("GEOLOC.STLength()")]
            public double GEOLOCSTLength { get; set; }
        }

        public class Feature
        {
            public Attributes attributes { get; set; }
            public Geometry geometry { get; set; }
        }

        public class Field
        {
            public string name { get; set; }
            public string type { get; set; }
            public string alias { get; set; }
            public int? length { get; set; }
        }

        public class FieldAliases
        {
            public string OBJECTID { get; set; }
            public string ROAD { get; set; }
            public string ROAD_NAME { get; set; }
            public string COMMON_USAGE_NAME { get; set; }
            public string START_SLK { get; set; }
            public string END_SLK { get; set; }
            public string CWY { get; set; }
            public string START_TRUE_DIST { get; set; }
            public string END_TRUE_DIST { get; set; }
            public string NETWORK_TYPE { get; set; }
            public string RA_NO { get; set; }
            public string RA_NAME { get; set; }
            public string LG_NO { get; set; }
            public string LG_NAME { get; set; }
            public string SPEED_LIMIT { get; set; }
            public string ROUTE_NE_ID { get; set; }

            [JsonProperty("GEOLOC.STLength()")]
            public string GEOLOCSTLength { get; set; }
        }

        public class Geometry
        {
            public List<List<List<double>>> paths { get; set; }
        }

        public class SpeedLimitResponse
        {
            public string displayFieldName { get; set; }
            public FieldAliases fieldAliases { get; set; }
            public string geometryType { get; set; }
            public SpatialReference spatialReference { get; set; }
            public List<Field> fields { get; set; }
            public List<Feature> features { get; set; }
            public bool exceededTransferLimit { get; set; }
        }

        public class SpatialReference
        {
            public int wkid { get; set; }
            public int latestWkid { get; set; }
        }

    }




    //public string ToJson()
    //{
    //    return JsonConvert.SerializeObject(this);
    //}
    public class MapConfig
    {
        // API part for Geo point
        public static string CrashPointAPI_Root = "https://mrgis.mainroads.wa.gov.au/arcgis/rest/services/OpenData/RoadSafety_DataPortal/MapServer/2/query";
        public static string CrashPointGeometryType = "esriGeometryEnvelope";
        public static string CrashPointSpatialRel = "esriSpatialRelIntersects";
        public static string CrashPointReturnGeometry = "true";
        public static string CrashPointReturnTrueCurves = "false";
        public static string CrashPointReturnIdsOnly = "false";
        public static string CrashPointReturnCountOnly = "false";
        public static string CrashPointReturnZ = "false";
        public static string CrashPointReturnM = "false";
        public static string CrashPointReturnDistinctValues = "false";
        public static string CrashPointReturnExtentsOnly = "false";
        public static string CrashPointFileType = "json";


        public static string API_Key_Google = "AIzaSyAU8O4mZBHBWc_2Vfw-wL-HGuGXEDIf6xY";
        public static string API_Secret;


        public static string Instance_DataProxy = "DataProxy";
        public static string Instance_MapController = "MapController";




        public static string SpeedLmitPointAPI_Root = "https://mrgis.mainroads.wa.gov.au/arcgis/rest/services/OpenData/RoadAssets_DataPortal/MapServer/8/query?";

    }
}
