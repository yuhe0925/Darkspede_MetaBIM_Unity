using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnStartDisable : MonoBehaviour
{
    public float Delay = 0; 
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Action", Delay);
    }

    // Update is called once per frame
    void Action()
    {
        Destroy(this.transform.gameObject);
    }
}
