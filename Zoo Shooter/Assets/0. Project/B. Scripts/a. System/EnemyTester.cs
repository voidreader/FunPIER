using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google2u;

public class EnemyTester : MonoBehaviour
{
    public GameObject prefabNormalEnemy;
    public Enemy _enemy;
    public Stair _stair;

    IEnumerator Start() {
        SetNewEnemy();
        yield return new WaitForSeconds(1);

        _stair.SetReadyEnemy();
        yield return null;
    }


    private void Update() {
        if(Input.GetKeyDown(KeyCode.K)) {
            _enemy.KillEnemy();
            StartCoroutine(MakeingEnemyTest());
        }

    }




    /// <summary>
    /// 랜덤 노멀 에너미 아이디 가져오기
    /// </summary>
    /// <returns></returns>
    public string GetRandomNormalEnemyID() {
        return EnemyData.Instance.Rows[Random.Range(0, EnemyData.Instance.Rows.Count)]._identifier;
    }

    IEnumerator MakeingEnemyTest() {
        SetNewEnemy();
        yield return new WaitForSeconds(1f);
        _stair.SetReadyEnemy(); // 등장!
    }


    public void SetNewEnemy() {
        _enemy = GameObject.Instantiate(prefabNormalEnemy, Vector3.zero, Quaternion.identity).GetComponent<Enemy>();
        _enemy.SetEnemy(EnemyType.Normal, GetRandomNormalEnemyID()); // 정보 설정 
        _stair.SetEnemey(_enemy);


    }
}
