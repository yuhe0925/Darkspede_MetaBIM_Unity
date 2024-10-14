using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IFCController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


public static class IfcclassMapper
{
    public static Dictionary<string, string> categoryNames = new Dictionary<string, string>
        {
            {"Ac","Activities" },
            {"Co","Complexes" },
            {"EF","Elements and Functions" },
            {"En","Entities" },
            {"FI","Form of Information" },
            {"PM","Project Management" },
            {"Pr","Products" },
            {"Ro","Roles" },
            {"SL","Spaces and Locations" },
            {"Ss","Systems" },
            {"TE","Tools and Equipment" },
            {"Zz","CAD" },
        };
}
