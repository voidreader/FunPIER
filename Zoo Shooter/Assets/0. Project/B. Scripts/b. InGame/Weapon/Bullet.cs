using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;


/// <summary>
/// GabrielAguiarProductions 발사체만 사용할것. 
/// </summary>
public class Bullet : MonoBehaviour
{

    public bool isOn = false;
    public bool isBulletInstantiate = false;

    public Rigidbody2D rb;
    public float speed = 5; // 발사속도
    


    public GameObject BulletPrefab;
    public GameObject MuzzlePrefab;
    public GameObject HitPrefab;

    // 꼬리.
    public List<GameObject> trails;
    public bool collided = false;

    public bool isEnemy = false;
    Enemy hitEnemy;

    public bool isLeft = false;
    int direction = 1;


    Transform bulletSpawned;

    // Muzzle 효과
    Transform spawned;
    ParticleSystem spawnedVFX;

    // Hit 효과 
    Transform hitSpawned;
    ParticleSystem hitSpawnedVFX;


    void Start() {

    }

    void OnSpawned() {

        collided = false;

        // 웨폰 매니저 리스트에 추가
        WeaponManager.ListShootingBullets.Add(this);



        // 총알 처리 
        SetBullet(); 

        // 총구 효과 
        SetMuzzle();
    }

    void OnDespawned() {
        if (WeaponManager.ListShootingBullets.Contains(this))
            WeaponManager.ListShootingBullets.Remove(this);
    }

    private void OnDestroy() {
        if (WeaponManager.ListShootingBullets.Contains(this))
            WeaponManager.ListShootingBullets.Remove(this);
    }


    void SetBullet() {

        // 총알을 추가로 생성하는 경우 
        if (BulletPrefab != this.gameObject) {

            Debug.Log("Instantiate.. Bullet");
            isBulletInstantiate = true;

            // Bullet Prefab
            // 총알 생성 
            bulletSpawned = Instantiate(BulletPrefab, this.transform, true).transform;
            bulletSpawned.localPosition = new Vector3(0, 0, -1.2f);
            bulletSpawned.localEulerAngles = new Vector3(90, 0, 0);
        }
        else
            isBulletInstantiate = false;

    }

    /// <summary>
    /// 총구 효과 
    /// </summary>
    void SetMuzzle() {
        // Muzzle 총구 효과
        if (MuzzlePrefab) {
            spawned = PoolManager.Pools[ConstBox.poolGame].Spawn(MuzzlePrefab, transform.position, Quaternion.identity);
            spawned.forward = this.transform.forward;
            spawnedVFX = spawned.GetComponent<ParticleSystem>();

            if (spawnedVFX != null)
                PoolManager.Pools[ConstBox.poolGame].Despawn(spawned, spawnedVFX.main.duration);
            else {
                spawnedVFX = spawned.GetChild(0).GetComponent<ParticleSystem>();
                PoolManager.Pools[ConstBox.poolGame].Despawn(spawned, spawnedVFX.main.duration);
            }
        }
    }


    /// <summary>
    /// 히트 효과 
    /// </summary>
    void SetHit() {

        if (HitPrefab == null)
            return;

        hitSpawned = PoolManager.Pools[ConstBox.poolGame].Spawn(HitPrefab, this.transform.position, Quaternion.identity);
        hitSpawnedVFX = hitSpawned.GetComponent<ParticleSystem>();

        if(hitSpawnedVFX == null) {
            hitSpawnedVFX = hitSpawned.GetChild(0).GetComponent<ParticleSystem>();
            PoolManager.Pools[ConstBox.poolGame].Despawn(hitSpawned, hitSpawnedVFX.main.duration);
        }
        else {
            PoolManager.Pools[ConstBox.poolGame].Despawn(hitSpawned, hitSpawnedVFX.main.duration);
        }

    }


    public void SetBulletOn(bool pLeft) {
        isLeft = pLeft;
        isOn = true;

        if (isLeft)
            direction = -1;
        else
            direction = 1;

        // Destroy(gameObject, 1.5f);
        // rb.velocity = transform.right * 2 * direction;
            
    }

    



    private void FixedUpdate() {

        
        if (!isOn)
            return;

        // rb.position += transform.forward * speed * Time.deltaTime);
        if(isLeft)
            rb.transform.Translate(transform.right * speed * Time.deltaTime * -1);
        else
            rb.transform.Translate(transform.right * speed * Time.deltaTime * 1);
        
    }


    /// <summary>
    /// 총알 히트 관련 처리는 다 이곳에서 한다. 
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerEnter2D(Collider2D col) {
        if (collided)
            return;

        if (col.tag == "Player" && !isEnemy)
            return;

        collided = true;

        // 파티클 히트 효과 
        SetHit();


        // 플레이어 처리 
        if (isEnemy && col.tag == "Player") {
            // 플레이어 킬
            Debug.Log(">> Player is hit! <<");
            GameManager.main.player.KillPlayer();
            StartCoroutine(DestroyParticle(0f));
            return;
        } // 플레이어 처리 종료

        // Debug.Log("Bullet Trigger Enter :: " + col.tag + "/" + col.gameObject.name);


        if (col.tag == "Stair") {
            CameraShake.main.ShakeOnce(0.15f, 0.1f);
        }
        else if (col.tag == "Head") {
            hitEnemy = col.GetComponentInParent<Enemy>();
            hitEnemy.HitEnemy(GameManager.main.currentWeapon.Damage, true);
        }
        else if (col.tag == "Body") {
            hitEnemy = col.GetComponent<Enemy>();
            hitEnemy.HitEnemy(GameManager.main.currentWeapon.Damage, false);
        }

        StartCoroutine(DestroyParticle(0f));
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    public IEnumerator DestroyParticle(float waitTime) {

        /*
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
        */
        if(isBulletInstantiate) {
            Destroy(bulletSpawned.gameObject);
        }


        yield return new WaitForSeconds(waitTime);


        if (PoolManager.Pools[ConstBox.poolGame].IsSpawned(this.transform))
            PoolManager.Pools[ConstBox.poolGame].Despawn(this.transform);
        else
            Destroy(gameObject);
    }

}
