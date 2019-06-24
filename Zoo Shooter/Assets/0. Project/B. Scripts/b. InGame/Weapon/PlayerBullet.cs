using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{

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

    // Start is called before the first frame update
    void Start() {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();

        //used to create a radius for the accuracy and have a very unique randomness
        /*
        if (accuracy != 100) {
            accuracy = 1 - (accuracy / 100);

            for (int i = 0; i < 2; i++) {
                var val = 1 * Random.Range(-accuracy, accuracy);
                var index = Random.Range(0, 2);
                if (i == 0) {
                    if (index == 0)
                        offset = new Vector3(0, -val, 0);
                    else
                        offset = new Vector3(0, val, 0);
                }
                else {
                    if (index == 0)
                        offset = new Vector3(0, offset.y, -val);
                    else
                        offset = new Vector3(0, offset.y, val);
                }
            }
        }
        */

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

    /*
    void FixedUpdate() {
        if (speed != 0 && rb != null) {
            // rb.position += new Vector2(transform.forward.x, transform.forward.y) * (speed * Time.deltaTime);

            this.transform.Translate(transform.right * speed * Time.deltaTime);

        }
    }
    */


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

    private void OnTriggerEnter2D(Collider2D co) {

        Debug.Log("OnTriggerEnter2D");
            

        if (collided)
            return;

        if (co.tag == "Player")
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


        // 적 죽이기
        if(co.tag == "Body") {
            co.GetComponent<Enemy>().KillEnemy();
            Debug.Log("Body Shot!!");
        }
        else if(co.tag == "Head") {
            // co.GetComponent<Enemy>().KillEnemy();
            co.GetComponentInParent<Enemy>().KillEnemy();
            Debug.Log("Head Shot!!");
        }
        else if (co.tag == "Stair") {
            Debug.Log("Hit Stair");
        }
        
    }


    private void OnCollisionEnter2D(Collision2D co) {
        Debug.Log("OnCollisionEnter2D");

    }

}
