using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class Bullet : MonoBehaviour
{

    public bool isOn = false;

    public Rigidbody2D rb;
    public float speed = 5; // 발사속도


    public GameObject BulletPrefab;
    public GameObject MuzzlePrefab;
    public GameObject HitPrefab;

    // 꼬리.
    public List<GameObject> trails;
    private bool collided = false;

    public bool isEnemy = false;
    Enemy hitEnemy;

    public bool isLeft = false;
    int direction = 1;

    int BodyMask;
    int HeadMask;
    int BulletMask;

    private void Awake() {
        BodyMask = LayerMask.GetMask("Body");
        HeadMask = LayerMask.GetMask("Head");
        BulletMask = ~(LayerMask.GetMask("Bullet"));


    }

    void Start() {

        // Bullet Prefab
        GameObject b = Instantiate(BulletPrefab, this.transform, true);
        b.transform.localPosition = new Vector3(0, 0, -1.2f);
        b.transform.localEulerAngles = new Vector3(90, 0, 0);
        
        // Muzzle 총구 효과
        if(MuzzlePrefab) {
            GameObject m = Instantiate(MuzzlePrefab, transform.position, Quaternion.identity);
            m.transform.forward = this.transform.forward;

            ParticleSystem vfx = m.GetComponent<ParticleSystem>();

            if(vfx != null)
                Destroy(m.gameObject, vfx.main.duration);
            else {
                vfx = m.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(m.gameObject, vfx.main.duration);
            }
        }
    }


    public void SetBulletOn(bool pLeft) {
        isLeft = pLeft;
        isOn = true;

        if (isLeft)
            direction = -1;
        else
            direction = 1;



        /*
        if (RayGunPoint())
            WeaponManager.isHit = true;
        */

        Destroy(gameObject, 1.5f);
    }


    /// <summary>
    /// 명중여부 체크 
    /// </summary>
    public bool RayGunPoint() {

        // 사용 이유. 
        // 쏘기전에 미리 명중 여부를 판단하기 위함. 
        // 총알마다 모두 속도가 다르기 때문에 
        // 빗나감 여부를 판단하기가 어렵다. 

        // Raycast 전에 본인 collider를 감춘다
        // RaycastHit2D[] hit = Physics2D.RaycastAll(this.transform.position, this.transform.right * direction);
        RaycastHit2D hitBody = Physics2D.Raycast(this.transform.position, this.transform.right * direction, float.MaxValue, 1<<LayerMask.NameToLayer("Body"));
        RaycastHit2D hitHead = Physics2D.Raycast(this.transform.position, this.transform.right * direction, float.MaxValue, 1 << LayerMask.NameToLayer("Head"));
        // RaycastHit2D hitHead = Physics2D.Raycast(this.transform.position, this.transform.right * direction, 100, HeadMask);


        if (hitBody.collider != null) {

            Debug.Log("RayGunPoint Body :: " + hitBody.collider.tag);
            return true;
        }

        if(hitHead.collider != null) {
            Debug.Log("RayGunPoint Head :: " + hitHead.collider.tag);
            return true;
        }

        RaycastHit2D hitBody2 = Physics2D.Raycast(this.transform.position, this.transform.right * -direction, 100, BodyMask);
        RaycastHit2D hitHead2 = Physics2D.Raycast(this.transform.position, this.transform.right * -direction, 100, HeadMask);


        return false;


        /*
        if (hit.Length == 0)
            return false;

        for (int i = 0; i < hit.Length; i++) {
            if (hit[i].collider.gameObject == this.gameObject)
                continue;

            // Body와 Head만 인정 
            if (hit[i].collider.tag == "Body" || hit[i].collider.tag == "Head")
                return true;

        }

        return false;
        */
        

        /*
        if(hit.collider != null) {
            Debug.Log("RayGunPoint :: " + hit.collider.tag);
        }
        */


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




        // 히트 효과 처리
        if (HitPrefab != null) {
            GameObject hitVFX = Instantiate(HitPrefab, this.transform.position, Quaternion.identity);

            ParticleSystem ps = hitVFX.GetComponent<ParticleSystem>();
            if (ps == null) {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
            else
                Destroy(hitVFX, ps.main.duration);
        } // 히트 효과 처리 종료

        

        



        // 플레이어 처리 
        if (isEnemy && col.tag == "Player") {
            // 플레이어 킬
            Debug.Log(">> Player is hit! <<");
            GameManager.main.player.KillPlayer();
            StartCoroutine(DestroyParticle(0f));
            return;
        } // 플레이어 처리 종료

        Debug.Log("Bullet Trigger Enter :: " + col.tag + "/" + col.gameObject.name);


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

    /*
    private void OnCollisionEnter2D(Collision2D collision) {

        if (collided)
            return;

        collided = true;


        // 히트 효과 처리
        if (HitPrefab != null) {
            GameObject hitVFX = Instantiate(HitPrefab, this.transform.position, Quaternion.identity);

            ParticleSystem ps = hitVFX.GetComponent<ParticleSystem>();
            if (ps == null) {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
            else
                Destroy(hitVFX, ps.main.duration);
        } // 히트 효과 처리 종료

        StartCoroutine(DestroyParticle(0f));

        if(collision.collider.tag == "Stair") {
            CameraShake.main.ShakeOnce(0.15f, 0.1f);
        }
        else if(collision.collider.tag == "Head") {
            hitEnemy = collision.gameObject.GetComponentInParent<Enemy>();
            hitEnemy.HitEnemy(GameManager.main.currentWeapon.Damage, true);
        }
        else if(collision.collider.tag == "Body") {
            hitEnemy = collision.gameObject.GetComponentInParent<Enemy>();
            hitEnemy.HitEnemy(GameManager.main.currentWeapon.Damage, false);
        }


    }
    */


    /// <summary>
    /// 
    /// </summary>
    /// <param name="waitTime"></param>
    /// <returns></returns>
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

}
