using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Controlling the shooter object
/// </summary>
public class Player : MonoBehaviour{
    public static Player _instance;

    [Header("Shooting")] public GameObject ballPrefab; //Ball to instantiate
    public Transform fireTransform;

    [HideInInspector] public int ballsInGame;

    float shooterTimer = 0f;

    Vector2 difference; //for calculating angle

    public GameObject trajectories;
    float trajectoryTimer = 0;
    public GameObject[] trajectoryDots;

    public bool isBlockTapped = false; //if a block is being tapped the touch will not affect the rotation

    float ballReturnTimer; //To avoid rapidly clicking the "call back balls" button

    [HideInInspector]
    public List<GameObject> ballPool; //for object pooling -> better fps

    private void Awake(){
        _instance = this;
    }

    // Use this for initialization
    void Start(){
        ballPool = new List<GameObject>();

        difference = Vector3.up;
        difference.Normalize();
    }

    // Update is called once per frame
    void Update(){
        ballReturnTimer += Time.deltaTime;

        Rotate();

        Shoot();
    }

    /// <summary>
    /// Shhoting balls, called in every frame
    /// </summary>
    void Shoot(){
        if (GameManager._instance.isPaused) return;
        shooterTimer += Time.deltaTime;

        //If you cant shoot return
        if (shooterTimer < 1f / Statistics._instance.fireRate || ballsInGame >= Statistics._instance.maxBallNum) return;

        shooterTimer = 0;

        //Getting ball from pool
        var g = GetBallFromPool();
        g.SetActive(true);
        g.transform.position = fireTransform.position;
        g.transform.eulerAngles = transform.eulerAngles;
        g.transform.localScale = Vector3.one / 6.5f;
        g.GetComponent<Ball>().damage = Statistics._instance.ballDamage;
        g.GetComponent<Rigidbody2D>().velocity = difference * Statistics._instance.ballSpeed;

        ballsInGame++;
    }

    /// <summary>
    /// if you have a free ball returns it, and if all balls are in usage instantiate one
    /// </summary>
    /// <returns></returns>
    GameObject GetBallFromPool(){
        foreach (GameObject g in ballPool)
            if (g.activeSelf == false)
                return g;

        var newBall = Instantiate(ballPrefab);
        ballPool.Add(newBall);

        return newBall;
    }

    //Rotating the shooter object
    void Rotate(){
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() && Input.mousePosition.y > Screen.height / 10 * 2){ //ignoring rotation if touching bottom holder panel
            // Check if there is a touch
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began){
                // Check if finger is over a UI element
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)){
                    return;
                }
            }
            else
                trajectoryTimer += Time.deltaTime;

            if (trajectoryTimer > 0.1f && !isBlockTapped){
                trajectories.SetActive(true);
                
                difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                difference.Normalize();
                float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Clamp(rotation_z - 90, -75, 75));
            }
        }
        else{
            trajectories.SetActive(false);
            trajectoryTimer = 0;
        }
    }

    /// <summary>
    /// Call back balls, On Click Listener on Call Back Balls button 
    /// </summary>
    public void BallsReturn(){
        if (ballReturnTimer < 1) return;

        ballReturnTimer = 0;

        var c = FindObjectsOfType<Ball>();

        foreach (Ball b in c){
            b.GetComponent<CircleCollider2D>().isTrigger = true;
            b.GetComponent<Rigidbody2D>().gravityScale = 1;

            Vector2 dif = b.transform.position - fireTransform.position;
            dif.Normalize();
            b.GetComponent<Rigidbody2D>().velocity = -dif * 7;

            b.trail.SetActive(true);
        }
    }
}