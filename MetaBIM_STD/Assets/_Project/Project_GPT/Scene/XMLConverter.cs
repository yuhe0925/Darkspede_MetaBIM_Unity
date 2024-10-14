using System.Collections;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class XMLConverter : MonoBehaviour
{
    public string inputXmlFilePath = "Assets/input.xml";
    public string outputJsonFilePath = "Assets/output.json";
    public string outputXmlFilePath = "Assets/output_back.xml";

    void Start()
    {
        ConvertJson();
    }


    public void ConvertJson()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(inputXmlFilePath);
        string jsonText = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.None);

        //save json to file
        File.WriteAllText(outputJsonFilePath, jsonText);

        // convert back to xml
        XmlDocument xmlDoc = JsonConvert.DeserializeXmlNode(jsonText);

        //save xml to file
        xmlDoc.Save(outputXmlFilePath);
    }
    private IEnumerator ConvertXmlToJsonWithProgress()
    {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(inputXmlFilePath);

        // Calculate total nodes for progress
        int totalNodes = xmlDocument.DocumentElement?.SelectNodes("//*").Count ?? 0;
        ProgressState progressState = new ProgressState();

        using (JsonTextWriter jsonWriter = new JsonTextWriter(new StreamWriter(outputJsonFilePath)))
        {
            jsonWriter.Formatting = Newtonsoft.Json.Formatting.Indented;

            jsonWriter.WriteStartObject();
            yield return StartCoroutine(ProcessNodeToJson(jsonWriter, xmlDocument.DocumentElement, totalNodes, progressState));
            jsonWriter.WriteEndObject();
        }

        Debug.Log("Conversion to JSON complete.");
        yield return new WaitForSeconds(1f); // Optional delay before starting next conversion

        // Now convert back from JSON to XML
        StartCoroutine(ConvertJsonToXmlWithProgress());
    }

    private IEnumerator ProcessNodeToJson(JsonTextWriter jsonWriter, XmlNode node, int totalNodes, ProgressState progressState)
    {
        if (node.HasChildNodes && node.FirstChild.NodeType == XmlNodeType.Text)
        {
            jsonWriter.WritePropertyName(node.Name);
            jsonWriter.WriteValue(node.InnerText);
        }
        else
        {
            jsonWriter.WritePropertyName(node.Name);
            jsonWriter.WriteStartObject();

            foreach (XmlNode childNode in node.ChildNodes)
            {
                yield return StartCoroutine(ProcessNodeToJson(jsonWriter, childNode, totalNodes, progressState));
            }

            jsonWriter.WriteEndObject();
        }

        // Update and show progress
        progressState.ProcessedNodes++;
        ShowProgress(progressState.ProcessedNodes, totalNodes, "Converting to JSON");
        yield return null;  // Yield to avoid blocking the main thread
    }

    private IEnumerator ConvertJsonToXmlWithProgress()
    {
        string jsonContent = File.ReadAllText(outputJsonFilePath);
        JObject jsonObject = JObject.Parse(jsonContent);

        XmlDocument xmlDocument = new XmlDocument();
        XmlElement root = xmlDocument.CreateElement(jsonObject.Properties().First().Name);
        xmlDocument.AppendChild(root);

        int totalProperties = jsonObject.Descendants().Count();
        ProgressState progressState = new ProgressState();

        yield return StartCoroutine(ProcessJsonToXml(jsonObject, xmlDocument.DocumentElement, totalProperties, progressState));

        xmlDocument.Save(outputXmlFilePath);

        Debug.Log("Conversion back to XML complete.");
    }

    private IEnumerator ProcessJsonToXml(JObject jsonObject, XmlElement parentElement, int totalProperties, ProgressState progressState)
    {
        foreach (var property in jsonObject.Properties())
        {
            if (property.Value is JObject childObject)
            {
                XmlElement childElement = parentElement.OwnerDocument.CreateElement(property.Name);
                parentElement.AppendChild(childElement);
                yield return StartCoroutine(ProcessJsonToXml(childObject, childElement, totalProperties, progressState));
            }
            else
            {
                XmlElement childElement = parentElement.OwnerDocument.CreateElement(property.Name);
                childElement.InnerText = property.Value.ToString();
                parentElement.AppendChild(childElement);
            }

            // Update and show progress
            progressState.ProcessedNodes++;
            ShowProgress(progressState.ProcessedNodes, totalProperties, "Converting back to XML");
            yield return null;  // Yield to avoid blocking the main thread
        }
    }

    private void ShowProgress(int processed, int total, string operation)
    {
        double percentage = (double)processed / total * 100;
        Debug.Log($"{operation}: {processed}/{total} ({percentage:F2}%)");
    }

    private class ProgressState
    {
        public int ProcessedNodes { get; set; }
    }
}
