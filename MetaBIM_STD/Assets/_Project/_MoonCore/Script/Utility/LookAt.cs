using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform Target;

    public bool OnlyY = true;

    // Start is called before the first frame update
    void Start()
    {
        if (Target == null)
        {
            Target = Camera.main.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Target);

        if (OnlyY)
        {
            transform.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y, 0 );
        }
    }
}
