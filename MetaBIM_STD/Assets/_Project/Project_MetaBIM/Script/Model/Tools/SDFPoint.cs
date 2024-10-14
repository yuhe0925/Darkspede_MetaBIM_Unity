using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class SDFPoint : MonoBehaviour
{

    public Transform SDF_Target;

    public float SDF_MinDistance = 1.0f;
    public float SDF_Factor = 0.01f;
    

    // Start is called before the first frame update
    void Start()
    {
        if(SDF_Target == null)
        {
            SDF_Target = Camera.main.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(SDF_Target == null)
        {
            return;
        }
        
        float distance = Vector3.Distance(transform.position, SDF_Target.position);
        
        if (distance < SDF_MinDistance)
        {
            distance = SDF_MinDistance;
        }

        Vector3 scale = new Vector3(1,1,1) * distance * SDF_Factor;

        transform.localScale = scale;
    }
    
}
