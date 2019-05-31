using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public GameObject target;
    public static bool CameraIsMove = false;

    void Update()
    {
        if (CameraIsMove)
        {
            transform.position = new Vector3(transform.position.x, 
                                             Mathf.Lerp(transform.position.y, 
                                             target.transform.position.y + 2f, 0.02f), 
                                             transform.position.z);

            if (transform.position.y >= target.transform.position.y + 1.9f)
            {
                CameraIsMove = false;
            }
        }
    }
}
