using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace IfcToolkit {

/// <summary>Class hooking into the Unity editor's import pipeline to detect when an ifc-file needs importing, then imports it.</summary>
public class IfcProcessor : AssetPostprocessor
{
    ///<summary>Generates the .dae and .xml files for newly detected .ifc files.</summary>
    ///<remarks>OnPreprocessAsset() is called before a file is imported into the Unity editor.</remarks>
    void OnPreprocessAsset()
    {
        if (assetPath.EndsWith(".ifc"))
        {
            //UnityEngine.Debug.Log("Preprocessing " + assetPath);
            bool editor = true;
            IfcImporter.ProcessIfc(assetPath, IfcSettingsWindow.options, editor);
            
            //Import the newly created files
            string outputFile = Application.streamingAssetsPath + "/IfcImporter/Resources/" + System.IO.Path.GetFileNameWithoutExtension(assetPath);
            AssetDatabase.ImportAsset(outputFile + "_dae.dae", ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(outputFile + "_xml.xml", ImportAssetOptions.ForceUpdate);
        }
    }

    ///<summary>Creates prefabs for newly detected .ifc files.</summary>
    ///<remarks>OnPostprocessAllAssets() is called after a file is imported into the Unity editor.</remarks>
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string assetPath in importedAssets)
        {
            if (assetPath.EndsWith(".ifc"))
            {
                GameObject gameObject = IfcImporter.LoadDae(assetPath);

                if(gameObject){
                    string resourceName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
                    string xml_path = Application.streamingAssetsPath + "/IfcImporter/Resources/" + resourceName + "_xml.xml";
                    IfcXmlParser.parseXmlFile(xml_path, gameObject, IfcSettingsWindow.options);

                    //Store filename in Ifc File component
                    IfcFile ifcFile = gameObject.AddComponent<IfcFile>() as IfcFile;
                    ifcFile.ifcFileName = resourceName;
                    ifcFile.ifcFilePath = assetPath;
                    ifcFile.geometryFileFormat = "DAE";
                    
                    //Move the model to origin if keepOriginalPositionEnabled is false
                    if (!IfcXmlParser.CheckMenuCondition("keepOriginalPositionEnabled", IfcSettingsWindow.options)) {
                        IfcImporter.MoveToOrigin(gameObject);
                    }
                    if (!IfcXmlParser.CheckMenuCondition("keepOriginalPositionForPartsEnabled", IfcSettingsWindow.options)) {
                        IfcImporter.MovePartsToOrigin(gameObject);
                    }


                    PrefabSaver.savePrefab(gameObject, assetPath);
                }
            }
        }
    }
}

}
