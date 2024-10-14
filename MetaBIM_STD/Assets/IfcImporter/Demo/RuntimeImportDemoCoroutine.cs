using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IfcToolkit;

namespace IfcToolkitDemo {

/// <summary>An example Class that imports an IFC model on Start(). </summary>
/// <remarks>To run the demo, you need to
/// 1) Ensure you've completed the installation guide in the README.
/// 2) Ensure RuntimeImportDemo.cs is attached to a GameObject in the opened scene.
/// 3) Place an ifc file you wish to test inside the project folder.
/// 4) Edit the assetPath and filename below to point to your ifc-file.
/// 5) Press play. Your building should appear after a few seconds.
/// You can also build and run the scene. For this you should make sure both your ifc-file and IfcConvert executable are in the resulting build folder and the assetPath and the filename below are still correct.
/// </remarks>
public class RuntimeImportDemoCoroutine : MonoBehaviour
{
    public string assetPath = "Assets/IfcImporter/Demo/";
    public string filename = "demo_duplex.ifc";

    void Start()
    {
        StartCoroutine(Demo());
    }

    ///<summary>A coroutine for starting the Importer coroutine and printing out some debug information.</summary>
    IEnumerator Demo() {
        Debug.Log("Importing " + assetPath+filename + ", please wait.");
        DateTime starttime = System.DateTime.Now;     // for measuring optimization
        //An optional parameter to toggle various importing option. All are true by default.
        Dictionary<string, bool> options = new Dictionary<string, bool>()
        {
            {"meshCollidersEnabled", true},
            {"materialsEnabled", true},
            {"propertiesEnabled", true},
            {"attributesEnabled", true},
            {"typesEnabled", true},
            {"headerEnabled", true},
            {"unitsEnabled", true},
            {"quantitiesEnabled", true},
            {"parallelProcessingEnabled", true},
            {"keepOriginalPositionEnabled", true},
            {"keepOriginalPositionForPartsEnabled", true}
        };

        //Run the runtime importer with an optional callback function to get access to the root GameObject of the IFC hierarchy.
        yield return IfcImporter.RuntimeImportCoroutine(assetPath+filename, options, (rootObject) => {
            //Various parts of the building are rootObject's children.
            Debug.Log("Name of the created GameObject: " + rootObject.name);
            DateTime endtime = System.DateTime.Now;       // for measuring optimization
            Debug.Log(endtime - starttime);
        });
    }
}
}
