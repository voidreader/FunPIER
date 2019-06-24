using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    float speed;
    GameObject playerObj;
    void Start()
    {
        playerObj = GameObject.Find("Player");
        speed = Random.Range(0.0005f, 0.005f);
    }


    void Update()
    {
        if (playerObj.activeSelf == false) return;
        MoveDown();
    }


    void MoveDown()
    {
        Vector2 pos = transform.position;
        pos.y -= speed;
        transform.position = pos;
    }


}
