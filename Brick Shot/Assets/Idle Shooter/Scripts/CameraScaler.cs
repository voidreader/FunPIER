using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    public bool maintainWidth = true;
    [Range(-1, 1)]
    public int adaptPosition;


    float defaultWidth;
    float defaultHeight;


    Vector3 CameraPos;

    // Use this for initialization
    void Start()
    {

        CameraPos = Camera.main.transform.position;

        defaultHeight = 5;
        defaultWidth = 5f * 720f/1280f;

        if (maintainWidth)
        {

            Camera.main.orthographicSize = defaultWidth / Camera.main.aspect;


            //CameraPos.y was added in case camera in case camera's y is not in 0
            Camera.main.transform.position = new Vector3(CameraPos.x, CameraPos.y + adaptPosition * (defaultHeight - Camera.main.orthographicSize), CameraPos.z);


        }
        else
        {
            //CameraPos.x was added in case camera in case camera's x is not in 0
            Camera.main.transform.position = new Vector3(CameraPos.x + adaptPosition * (defaultWidth - Camera.main.orthographicSize * Camera.main.aspect), CameraPos.y, CameraPos.z);

        }
    }

    // Update is called once per frame
    void Update()
    {

        


    }
}
