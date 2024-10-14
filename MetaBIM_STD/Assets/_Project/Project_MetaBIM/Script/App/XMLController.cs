using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Linq;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.XmlDiffPatch;

namespace MetaBIM
{
    public class XMLController : MonoBehaviour
    {
        public static XMLController Instance;

        public XmlDocument XML;

        public StructureNode SpatialRootItem;
        public Dictionary<string, List<StructureNode>> SpatialObjectItems = new Dictionary<string, List<StructureNode>>();



        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }


        public void OnoLoadXMLFromFile(string _fileName)
        {
            Debug.Log("Start Loading XML : " + Application.dataPath + "/IFCExample/" + _fileName);
            XML = new XmlDocument();
            XML.Load(Application.dataPath + "/IFCExample/" + _fileName);
            Debug.Log("Loading Complete");
        }

        public void OnLoadXMLFromString(string _data)
        {
            Debug.Log("Start Loading XML : ");
            XML = new XmlDocument();
            XML.LoadXml(_data);
            Debug.Log("Loading Complete");
        }



        public void OnLoadXMLFromUrl(string _url, Action _action)
        {
            Debug.Log("OnLoadXMLFromUrl, URL = " + _url);
            StartCoroutine(DataProxy.Instance.LoadXML(_url, OnLoadXMLUrl_Callback, _action));
        }

        


        public void OnLoadXMLUrl_Callback(bool _result, string _message, Action _action)
        {
            if (_result)
            {
                XML = new XmlDocument();
                XML.LoadXml(_message);
                _action();
            }
            else
            {

            }
        }



        public XmlNode GetNodeByName(string _name)
        {
            return XML.SelectSingleNode(_name);
        }

        public XmlNode GetNodeByPath(string _path)
        {
            string xmlPathPattern = _path;
            XmlNode baseNode = XML.SelectSingleNode(xmlPathPattern);
            return baseNode;
        }

        int NodeCount = 0;

        /// <summary>
        /// Get the spatial structure of the IFC - XML
        /// </summary>
        public void GetStructure()
        {
            string xmlPathPattern = "//ifc/decomposition";
            XmlNode baseNode = XML.SelectSingleNode(xmlPathPattern);

            NodeCount = 0;
            StartCoroutine(LoadChildNode(baseNode));

            Debug.Log("Node Count = " + NodeCount);
        }

        public IEnumerator LoadNodeLevel(XmlNode _node)
        {
            // count level

            yield return null;
        }

        public IEnumerator LoadChildNode(XmlNode _node)
        {
            foreach (XmlNode node in _node.ChildNodes)
            {
                Debug.Log(node.Name + " -> " + node.ChildNodes.Count);
                NodeCount++;
                yield return null;
                if (node.ChildNodes.Count > 0)
                {
                    StartCoroutine(LoadChildNode(node));
                }
            }
        }


        public void GroupNodes(XmlDocument _doc)
        {


        }




        string sourceXml;
        string targetXml;

        string sourceXmlString;
        string targetXmlString;

 

        public void OnRequestCompareXml(string _source, string _target)
        {
            sourceXml = Config.XML_Path + _source + "_xml.xml";
            targetXml = Config.XML_Path + _target + "_xml.xml";

            Debug.Log("Loading: " + sourceXml);
            StartCoroutine(DataProxy.Instance.LoadXML(sourceXml, OnRequestCompareXml_LoadSource_Callback,null));

        }


        public void OnRequestCompareXml_LoadSource_Callback(bool _result, string _message, Action _action)
        {
            if (_result)
            {
                sourceXmlString = _message;
                Debug.Log("Size: " + sourceXmlString.Length);
                Debug.Log("Loading: " + targetXml);
                StartCoroutine(DataProxy.Instance.LoadXML(targetXml, OnRequestCompareXml_LoadTarget_Callback, null));
            }
            else
            {

            }
 
        }

        public void OnRequestCompareXml_LoadTarget_Callback(bool _result, string _message, Action _action)
        {
            if (_result)
            {
                targetXmlString = _message;
                Debug.Log("Size: " + targetXmlString.Length);
                //CompareXML();
                CompareXML();

            }
            else
            {

            }
        }


        public void CompareXML()
        {

            Debug.Log("OnRequestCompareXml.CompareXML");
            Debug.Log("OnRequestCompareXml.sourceXml: " + sourceXml);
            Debug.Log("OnRequestCompareXml.targetXml: " + targetXml);

            string result = "No Result";
            XmlWriterSettings settings = new XmlWriterSettings();

            XmlWriter diffgramWriter = XmlWriter.Create(result, settings);

            XmlDiff xmldiff = new XmlDiff(XmlDiffOptions.IgnoreChildOrder |
                                 XmlDiffOptions.IgnoreNamespaces |
                                 XmlDiffOptions.IgnorePrefixes);
            bool bIdentical = xmldiff.Compare(sourceXmlString, targetXmlString, false, diffgramWriter);
            diffgramWriter.Close();

            Debug.Log("Result:");
            Debug.Log(result);
        }


        XmlDiffOptions diffOptions = new XmlDiffOptions();
        XmlDiff diff = new XmlDiff();
        bool compareFragments = false;

        public void DoCompare(string file1, string file2)
        {

            //This method sets the diff.Options property.
            SetDiffOptions();

            bool isEqual = false;
            MemoryStream stream = new MemoryStream();
            XmlTextWriter tw = new XmlTextWriter(new StreamWriter(stream));
            String result = System.Text.Encoding.UTF8.GetString(stream.ToArray(), 0, (int)stream.Length);


            //Now compare the two files.
            try
            {
                isEqual = diff.Compare(file1, file2, compareFragments, tw);
                Debug.Log("Compare Complated: ");
            }
            catch (XmlException xe)
            {
               Debug.Log("An exception occured while comparing\n" + xe.StackTrace);
            }
            finally
            {
                tw.Close();
                Debug.Log(result);
            }

            if (isEqual)
            {
                //This means the files were identical for given options.
                Debug.Log("Files Identical for the given options");
                return; //dont need to show the differences.
            }



            //Done!
        }

        /// <summary>
        /// This method reads the diff options set on the 
        /// menu and configures the XmlDiffOptions object.
        /// </summary>
        private void SetDiffOptions()
        {
            //Reset to None and refresh the options from the menuoptions
            //else eventually all options may get set and the menu changes will
            // not be reflected.
            diffOptions = XmlDiffOptions.None;

            //Read the options settings and OR the XmlDiffOptions enumeration.
            diffOptions = diffOptions | XmlDiffOptions.IgnorePI;
            diffOptions = diffOptions | XmlDiffOptions.IgnoreChildOrder;
            diffOptions = diffOptions | XmlDiffOptions.IgnoreComments;
            diffOptions = diffOptions | XmlDiffOptions.IgnoreDtd;
            diffOptions = diffOptions | XmlDiffOptions.IgnoreNamespaces;
            diffOptions = diffOptions | XmlDiffOptions.IgnorePrefixes;
            diffOptions = diffOptions | XmlDiffOptions.IgnoreWhitespace;
            diffOptions = diffOptions | XmlDiffOptions.IgnoreXmlDecl;

            //Default algorithm is Auto.
            diff.Algorithm = XmlDiffAlgorithm.Auto;

            diff.Options = diffOptions;
        }


    }
}
