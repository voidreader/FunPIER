﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class PlayerBullet : MonoBehaviour
{

    // public static bool isHitEnemy = false;

    public bool bounce = false;
    public float bounceForce = 10;
    public float speed;
    [Tooltip("From 0% to 100%")]
    public float accuracy;
    public float fireRate;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    public AudioClip shotSFX;
    public AudioClip hitSFX;
    public List<GameObject> trails;

    private Vector2 startPos;
    private float speedRandomness;
    private Vector2 offset;
    private bool collided = false;
    public  Rigidbody2D rb;
    
    private GameObject target;

    public bool isEnemy = false;
    Enemy hitEnemy;

    // Start is called before the first frame update
    void Start() {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();

        // 포구
        if (muzzlePrefab != null) {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward;
            var ps = muzzleVFX.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(muzzleVFX, ps.main.duration);
            else {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }

        if (shotSFX != null && GetComponent<AudioSource>()) {
            GetComponent<AudioSource>().PlayOneShot(shotSFX);
        }
    }

    public void AddBulletForce(Transform weaponTransform, float direcrion) {

        if (rb == null)
            rb = this.GetComponent<Rigidbody2D>();

        rb.velocity = (weaponTransform.right * speed) * direcrion;
        Destroy(gameObject, 1.5f);
    }




    // vodi OnColl
    public IEnumerator DestroyParticle(float waitTime) {

        if (transform.childCount > 0 && waitTime != 0) {
            List<Transform> tList = new List<Transform>();

            foreach (Transform t in transform.GetChild(0).transform) {
                tList.Add(t);
            }

            while (transform.GetChild(0).localScale.x > 0) {
                yield return new WaitForSeconds(0.01f);
                transform.GetChild(0).localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                for (int i = 0; i < tList.Count; i++) {
                    tList[i].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
        }

        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }



    /// <summary>
    /// 중요한 부분. 
    /// 총알 히트 처리 
    /// </summary>
    /// <param name="co"></param>
    private void OnTriggerEnter2D(Collider2D co) {

        Debug.Log("OnTriggerEnter2D");
            

        if (collided)
            return;

        if (co.tag == "Player" && !isEnemy)
            return;

        

        collided = true;

        #region 꼬리 처리 및 hit 효과 

        // 꼬리.. 기타등등 파괴처리
        if (trails.Count > 0) {
            for (int i = 0; i < trails.Count; i++) {
                trails[i].transform.parent = null;
                var ps = trails[i].GetComponent<ParticleSystem>();
                if (ps != null) {
                    ps.Stop();
                    Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
                }
            }
        }

        speed = 0;
        //Vector3 pos = co.transform.position;
        Vector3 pos = transform.position;

        if (hitPrefab != null) {
            var hitVFX = Instantiate(hitPrefab, pos, Quaternion.identity) as GameObject;

            var ps = hitVFX.GetComponent<ParticleSystem>();
            if (ps == null) {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
            else
                Destroy(hitVFX, ps.main.duration);
        }

        #endregion


        // 파티클 파괴 
        StartCoroutine(DestroyParticle(0f));

        if(isEnemy && co.tag == "Player") {
            // 플레이어 킬
            Debug.Log(">> Player is hit! <<");

            GameManager.main.player.KillPlayer();
            return;
        } // 플레이어 처리 종료


        // 적 죽이기 바디샷
        if(co.tag == "Body") {
            Debug.Log("Body Shot!!");
            hitEnemy = co.GetComponent<Enemy>();

            if (hitEnemy.isKilled)
                return;

            hitEnemy.HitEnemy(GameManager.main.currentWeapon.Damage, false);
            // isHitEnemy = true;


            // Score 처리
            // 스코어는 레벨에 영향을 받는다.
            GameViewManager.main.AddScore(GameManager.main.CurrentLevelData._level + 1);

            // 보스일때는 데미지 표시
            if (hitEnemy.type == EnemyType.Boss)
                GameManager.main.ShowDamage(GameManager.main.currentWeapon.Damage, false);


        }
        else if(co.tag == "Head") { // 헤드샷
            Debug.Log("Head Shot!!");

            GameManager.main.ShowGetCoin(); // 코인 획득 

            hitEnemy = co.GetComponentInParent<Enemy>();
            hitEnemy.HitEnemy(GameManager.main.currentWeapon.Damage * 2, true); // 두배 

            //isHitEnemy = true;

            // Vector3 headshotPos = new Vector3(co.transform.parent.position.x, co.transform.parent.position.y + 2.7f, 0);

            PoolManager.Pools[ConstBox.poolGame].Spawn(ConstBox.prefabHeadshot, new Vector3(0, 5f, 0), Quaternion.identity);
            // co.transform.parent

            // 헤드샷은 두배
            GameViewManager.main.AddScore((GameManager.main.CurrentLevelData._level + 1) * 2, true);
            GameManager.main.Splash();

            // 보스일때는 데미지 표시
            if(hitEnemy.type == EnemyType.Boss)
                GameManager.main.ShowDamage(GameManager.main.currentWeapon.Damage * 2, true);



        }
        else  {
            Debug.Log("Hit Others");
            // GameManager.isMissed = true; // 빗나갔다!
            CameraShake.main.ShakeOnce(0.15f, 0.1f);

        }

        
    }


    private void OnCollisionEnter2D(Collision2D co) {
        Debug.Log("OnCollisionEnter2D");

    }





}

