using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayManager : MonoBehaviour
{


    // -------------------------- Display Bound -------------------------- // 
    float mapX = 100.0f;
    float mapY = 100.0f;

    float minX;
    float maxX;
    float minY;
    float maxY;



    [HideInInspector]
    public static float DISPLAY_WIDTH;
    [HideInInspector]
    public static float DISPLAY_HEIGHT;

    [HideInInspector]
    public static float DISPLAY_LEFT;
    [HideInInspector]
    public static float DISPLAY_RIGHT;
    [HideInInspector]
    public static float DISPLAY_BOTTOM;
    [HideInInspector]
    public static float DISPLAY_TOP;


    void Awake()
    {
        CalcDisplayBound();
    }


    public void CalcDisplayBound()
    {
        float vertExtent = Camera.main.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        minX = horzExtent - mapX / 2.0f;
        maxX = mapX / 2.0f - horzExtent;
        minY = vertExtent - mapY / 2.0f;
        maxY = mapY / 2.0f - vertExtent;

        DISPLAY_LEFT = maxX - 50;
        DISPLAY_RIGHT = minX + 50;
        DISPLAY_TOP = minY + 50;
        DISPLAY_BOTTOM = maxY - 50;

        DISPLAY_WIDTH = DISPLAY_RIGHT - DISPLAY_LEFT;
        DISPLAY_HEIGHT = DISPLAY_TOP - DISPLAY_BOTTOM;
    }



}