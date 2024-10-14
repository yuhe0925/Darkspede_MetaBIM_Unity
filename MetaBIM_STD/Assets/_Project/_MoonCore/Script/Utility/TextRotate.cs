using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextRotate : MonoBehaviour
{
    public bool IsRotating = true;

    public float degreeInterval;
    public float interval;

    private float degree;
    private float intervalCount;



    private void LateUpdate()
    {
        if (!IsRotating)
        {
            return;
        }

        if (intervalCount > 0)
        {
            intervalCount = intervalCount - Time.deltaTime;
        }
        else
        {
            degree = degree + degreeInterval;
            if (degree >= 360 || degree <= -360)
            {
                degree = 0;
            }
            intervalCount = interval;

            transform.rotation = Quaternion.Euler(new Vector3(0, 0, degree));
        }
    }

}
