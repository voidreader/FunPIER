using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCircle : MonoBehaviour
{	

	float Speed;

	public float minSpeed;
	public float maxSpeed;


    void Start()
    {	
		Speed = Random.Range(minSpeed, maxSpeed);
    }


    void Update()
    {
        transform.Rotate(Vector3.forward * Time.deltaTime*Speed, Space.World);
    }
    
}
