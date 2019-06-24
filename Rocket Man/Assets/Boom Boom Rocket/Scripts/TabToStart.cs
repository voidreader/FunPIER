using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabToStart : MonoBehaviour
{

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && gameObject.activeSelf == true)
        {
            gameObject.SetActive(false);
        }
    }

}
