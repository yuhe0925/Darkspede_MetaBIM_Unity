using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using System.Linq;

namespace IfcToolkit {

/// <summary>Class that adds IFC GameObject an optional import settings window to the Unity editor top menu bar. </summary>
public class IfcSettingsWindow : EditorWindow
{
    public static Dictionary<string, bool> options = new Dictionary<string, bool>()
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

    [MenuItem("Edit/IFC Importer Settings...")]
    public static void ShowWindow()
    {
        EditorWindow settingsWindow = EditorWindow.GetWindow(typeof(IfcSettingsWindow));
        settingsWindow.titleContent = new GUIContent("IFC importer settings");
        settingsWindow.minSize = new Vector2(440,250);
    }
    
    void OnGUI()
    {
        float originalValue = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 400;
        options["meshCollidersEnabled"] = EditorGUILayout.Toggle ("Add mesh colliders", options["meshCollidersEnabled"]);
        options["materialsEnabled"] = EditorGUILayout.Toggle ("Import IFC material layer sets", options["materialsEnabled"]);
        options["propertiesEnabled"] = EditorGUILayout.Toggle ("Import IFC property sets", options["propertiesEnabled"]);
        options["attributesEnabled"] = EditorGUILayout.Toggle ("Import IFC attributes", options["attributesEnabled"]);

        options["typesEnabled"] = EditorGUILayout.Toggle ("Import IFC types", options["typesEnabled"]);
        options["headerEnabled"] = EditorGUILayout.Toggle ("Import IFC header", options["headerEnabled"]);
        options["unitsEnabled"] = EditorGUILayout.Toggle ("Import IFC units", options["unitsEnabled"]);
        options["quantitiesEnabled"] = EditorGUILayout.Toggle ("Import IFC quantities", options["quantitiesEnabled"]);

        options["parallelProcessingEnabled"] = EditorGUILayout.Toggle ("Extract model and metadata in parallel (memory intensive)", options["parallelProcessingEnabled"]);
        options["keepOriginalPositionEnabled"] = EditorGUILayout.Toggle ("Keep the original position of the model. Otherwise moved to origin.", options["keepOriginalPositionEnabled"]);
        options["keepOriginalPositionForPartsEnabled"] = EditorGUILayout.Toggle ("Keep original position of individual parts. Otherwise moved near origin.", options["keepOriginalPositionForPartsEnabled"]);
        EditorGUIUtility.labelWidth = originalValue;
    }

    //Updates the values on the window to correspond to the stored preferences.
    void OnFocus()
    {
        // Need to call ToList() because we're modifying the dictionary inside the loop
        foreach(string key in options.Keys.ToList<string>())
        {
            if (EditorPrefs.HasKey(key))
            {
                options[key] = EditorPrefs.GetBool(key);
            }
        }
    }

    //Stores the values on the window to the stored preferences when the window loses focus.
    void OnLostFocus()
    {
        foreach(string key in options.Keys)
        {
            EditorPrefs.SetBool(key, options[key]);
        }
    }

    //Stores the values on the window to the stored preferences when the window is closed.
    void OnDestroy()
    {
        foreach(string key in options.Keys)
        {
            EditorPrefs.SetBool(key, options[key]);
        }
    }
}
}
