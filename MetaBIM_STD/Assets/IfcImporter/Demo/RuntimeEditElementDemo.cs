using UnityEngine;
using IfcToolkit;

namespace IfcToolkitDemo {
public class RuntimeEditElementDemo : MonoBehaviour
{
    public GameObject ifcRootGameObject;
    public GameObject ifcElement;
    void Start()
    {
        // Change one of the element's properties
        ifcElement.GetComponent<IfcProperties>().ChangeValue("IsExternal", "true");

        // Export edited IFC model to the Unity project's root directory
        IfcExporter.Export("From_EditElementDemo.ifc", ifcRootGameObject);
    }
}
}
