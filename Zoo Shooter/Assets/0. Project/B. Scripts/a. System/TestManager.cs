using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TestManager : MonoBehaviour
{
    public Enemy enemy;
    public Stair stair;
    public Player player;
    public GameObject Bullet;


    private void Start() {
        InitEnemy();

    }



    void InitEnemy() {
        enemy.SetEnemy(EnemyType.Normal, "Penguin");
        stair.SetEnemey(enemy);
    }


    void Update() {
     

        if (Input.GetKeyDown(KeyCode.A)) {
            player.Aim();
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            player.Shoot();
        }
    }

    #region
    public GameObject firePoint;
    public void SpawnVFX() {

    }

    #endregion

}
