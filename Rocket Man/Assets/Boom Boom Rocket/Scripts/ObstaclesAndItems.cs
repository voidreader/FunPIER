using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesAndItems : MonoBehaviour
{

    GameObject PlayerObj;

    void Start()
    {
        PlayerObj = GameObject.Find("Player");
    }



    void Update()
    {
        CalculateDistanceToPlayer();
    }


    void CalculateDistanceToPlayer()
    {
        if (transform.position.y < PlayerObj.transform.position.y - 15)
        {
            GameObject.Find("ObstacleManager").GetComponent<ObstacleManager>().MakeNextObstacleGroup();
            Destroy(gameObject);
        }
    }



}
