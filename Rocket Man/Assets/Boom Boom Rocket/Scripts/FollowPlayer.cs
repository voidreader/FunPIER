using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    public GameObject PlayerObj;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    public float yOffset;


    float lastYPosition;


    void FixedUpdate()
    {
        Vector3 targetPosition = new Vector3(PlayerObj.transform.position.x, PlayerObj.transform.position.y + yOffset, -10);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);


        if (transform.position.y < lastYPosition)
        {
            transform.position = new Vector3(transform.position.x, lastYPosition, -10);
        }
        else
        {
            lastYPosition = transform.position.y;
        }

    }

}