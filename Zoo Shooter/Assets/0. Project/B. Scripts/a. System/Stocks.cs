using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stocks : MonoBehaviour
{
    public static Stocks main = null;

    public List<Weapon> ListWeapons; // 무기 데이터 
    public List<Sprite> ListWeaponSprites; // 무기 스프라이트

    public GameObject prefabNormalEnemy; // 일반 에너미 
    public GameObject prefabEnemyWeapon; // 적 무기 
    public GameObject prefabEnemyBullet; // 적 총알 


    private void Awake() {
        main = this;
    }
}
