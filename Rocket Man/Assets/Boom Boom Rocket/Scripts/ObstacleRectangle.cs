using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleRectangle : MonoBehaviour
{

    float Speed;

    public float minSpeed;
    public float maxSpeed;

    void Start()
    {
        transform.Rotate(0, 0, Random.Range(0, 90));
        Speed = Random.Range(minSpeed, maxSpeed);
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * Time.deltaTime * Speed, Space.World);
    }
}
