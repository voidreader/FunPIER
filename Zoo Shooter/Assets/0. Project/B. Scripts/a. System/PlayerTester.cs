using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTester : MonoBehaviour
{

    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        player.SetSpriteDirection(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)) {
            player.Aim();
            GameManager.isPlaying = true;
        }


        if (Input.GetKeyDown(KeyCode.C)) {
            player.SetSpriteDirection(!player.isLeft);
            GameManager.isPlaying = true;
            // player.weapon.CurrentAim.ResetAim();
        }
    }
}
