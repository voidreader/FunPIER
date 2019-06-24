using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{




    public GameObject ItemEffectPrefab;
    public GameObject DeadEffectPrefab;
    public GameObject shieldObj;
    public GameObject shieldFadeOutObj;

    public ParticleSystem engine;
    public ParticleSystem powerJumpParticle;

    [Space(10)]
    public float rotationSpeed = 80;
    public int angleLimit = 120;
    public float normalJumpPower = 30f;
    public float specialJumpPower = 2f;


    [HideInInspector]
    public bool isWaiting = true;
    bool isJumping = false;
    float angle = 0;
    float hueValue = 0;

    Rigidbody2D rigid;




    void Start()
    {
        powerJumpParticle.Stop();


        hueValue = Random.Range(0, 1f);
        ChangeBackgroundColor();

        rigid = GetComponent<Rigidbody2D>();
        rigid.isKinematic = true;
    }

    void Update()
    {
        RotatePlayer();

        if (Input.GetMouseButtonDown(0) )
        {
            if (isWaiting)
            {
                isWaiting = false;
                rigid.isKinematic = false;
            }

            AcceleratePlayer(angle, normalJumpPower);
            ChangeBackgroundColor();
        }


        DeceleratePlayer();


        if (transform.position.y < Camera.main.transform.position.y - DisplayManager.DISPLAY_HEIGHT / 2)
        {
            GameOver();
        }
    }



    void RotatePlayer()
    {
        if (isWaiting == true) return;

        if (isJumping == false)
        {
            angle += rotationSpeed * Time.deltaTime;
            if (angle > angleLimit)
            {
                angle = angleLimit;
                rotationSpeed *= -1;
            }
            if (angle < -angleLimit)
            {
                angle = -angleLimit;
                rotationSpeed *= -1;
            }
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        }


    }


    void AcceleratePlayer(float ang, float power)
    {
        Vector3 direction = Quaternion.AngleAxis(ang, Vector3.forward) * Vector3.up;
        rigid.velocity = direction * power;

        engine.Play();
    }




    void ChangeBackgroundColor()
    {
        hueValue += 0.002f;
        if (hueValue >= 1)
        {
            hueValue = 0;
        }
        Camera.main.backgroundColor = Color.HSVToRGB(hueValue, 0.5f, 0.7f);
    }


    void DeceleratePlayer()
    {
        if (rigid.velocity.magnitude > 0.03f)
        {
            rigid.velocity = rigid.velocity * 0.93f;
        }
        else
        {
            rigid.velocity = Vector2.zero;
        }
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
            GameOver();
        }

        if (other.gameObject.tag == "Item")
        {
            StartCoroutine(PowerJump());
            Destroy(Instantiate(ItemEffectPrefab, transform.position, Quaternion.identity), 1.0f);
            Destroy(other.gameObject.transform.parent.gameObject);
        }

    }

    void GameOver()
    {
        gameObject.SetActive(false);

        Destroy(Instantiate(DeadEffectPrefab, transform.position, Quaternion.identity), 1.5f);

        GameObject.Find("Manager").GetComponent<PlayManager>().GameOver();
    }



    IEnumerator PowerJump()
    {
        if(gameObject.activeSelf == false) yield break;

        isJumping = true;

        angle = 0;
        GetComponent<BoxCollider2D>().enabled = false;

        AcceleratePlayer(0, specialJumpPower);
        
        powerJumpParticle.Play();
        shieldObj.SetActive(true);




        yield return new WaitForSecondsRealtime(0.5f);


        isJumping = false;


        yield return new WaitForSecondsRealtime(1.0f);


        GetComponent<BoxCollider2D>().enabled = true;
        shieldObj.SetActive(false);

        shieldFadeOutObj.SetActive(true);
        yield return new WaitForSecondsRealtime(0.5f);
        shieldFadeOutObj.SetActive(false);

        yield break;



    }




}
