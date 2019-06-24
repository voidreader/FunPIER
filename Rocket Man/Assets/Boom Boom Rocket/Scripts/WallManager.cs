using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{

    public GameObject Player;


	int wallWidth = 5;
	public  int mapWidth = 3;
    public GameObject leftWall;
    public GameObject rightWall;


    void Start()
    {	
		leftWall.transform.localScale = new Vector2(wallWidth, DisplayManager.DISPLAY_HEIGHT*2);
		rightWall.transform.localScale = new Vector2(wallWidth, DisplayManager.DISPLAY_HEIGHT*2);
    }


    void Update()
    {
        leftWall.transform.position = new Vector2(DisplayManager.DISPLAY_LEFT * mapWidth, Player.transform.position.y);
        rightWall.transform.position = new Vector2(DisplayManager.DISPLAY_RIGHT * mapWidth, Player.transform.position.y);
    }
}
