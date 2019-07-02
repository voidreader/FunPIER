using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google2u;

public class Stocks : MonoBehaviour
{
    public static Stocks main = null;

    public List<Weapon> ListWeapons; // 무기 데이터 
    public List<Sprite> ListWeaponSprites; // 무기 스프라이트
    public List<Sprite> ListNormalEnemySprite; // 일반 적 스프라이트 
    public List<Sprite> ListBossSprite; // 보스 스프라이트 


    public GameObject prefabNormalEnemy; // 일반 에너미 
    public GameObject prefabBossEnemy; // 보스
    public GameObject prefabEnemyWeapon; // 적 무기 
    public GameObject prefabEnemyBullet; // 적 총알 


    private void Awake() {
        main = this;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static Sprite GetEnemySprite(string name) {
        for (int i = 0; i < main.ListNormalEnemySprite.Count; i++) {
            if (main.ListNormalEnemySprite[i].name == name)
                return main.ListNormalEnemySprite[i];
        }

        Debug.Log(">>> GetEnemySprite returns null :: " + name);

        return null;
    }

    /// <summary>
    /// 랜덤 노멀 에너미 아이디 가져오기
    /// </summary>
    /// <returns></returns>
    public static string GetRandomNormalEnemyID() {
        return EnemyData.Instance.Rows[Random.Range(0, 11)]._identifier;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static Sprite GetBossSprite(string name) {
        for (int i = 0; i < main.ListBossSprite.Count; i++) {
            if (main.ListBossSprite[i].name == name)
                return main.ListBossSprite[i];
        }

        Debug.Log(">>> GetBossSprite returns null :: " + name);
        return null;
    }

    public static EnemyDataRow GetBossDataRow(int level) {
        string sp = BossData.Instance.Rows[level]._sprite;

        for(int i=0; i<EnemyData.Instance.Rows.Count; i++) {
            if (EnemyData.Instance.Rows[i]._sprite == sp)
                return EnemyData.Instance.Rows[i];
        }

        return null;
    }

}
