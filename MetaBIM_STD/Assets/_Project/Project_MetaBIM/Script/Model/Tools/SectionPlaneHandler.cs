using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionPlaneHandler : MonoBehaviour
{
    public GameObject SectionPlanePrefabs;
    public Transform SectionPlaneParent;
    
    public List<SectionPlane> Sections;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnViewEnable()
    {
        foreach (SectionPlane item in Sections)
        {
            item.isDrawing = true;
        }

    }


    public void OnViewDisable()
    {
        foreach (SectionPlane item in Sections)
        {
            item.isDrawing = false;
        }

    }


    public void ClearSections()
    {
        foreach(SectionPlane item in Sections)
        {
            Destroy(item.gameObject);
        }

        Sections.Clear();
    }


    public void OnCreateSection(List<GameObject> _objects, Bounds _targetBound)
    {
        ClearSections();

        if (_objects.Count == 0)
        {
            return;
        }

        foreach (GameObject item in _objects)
        {
            BIMElement element = item.GetComponent<BIMElement>();
            SectionPlane plane = Instantiate(SectionPlanePrefabs, SectionPlaneParent).GetComponent<SectionPlane>();

            string elevation = element.BimObject.GetAttributeValue(element.SelectedVersion, "Elevation");
            float elevationValue = 0;
            if (elevation != null)
            {
                elevationValue = float.Parse(elevation);
            }

            Debug.Log("OnCreateSection: " + item.name + ", " + elevationValue);

            elevationValue = elevationValue / 1000f;  // unit?

            Vector3 offset = new Vector3(0, elevationValue, 0);

            plane.SetItem(_targetBound, offset);

            Sections.Add(plane);
        }
    }
    
}
