using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ball object
/// </summary>
public class Ball : MonoBehaviour {

    public float speed;
    public decimal damage;

    public GameObject trail, trailBooster; //trail activated on boost
    Rigidbody2D rb;

    public AudioSource music; //sound of hit

    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable() {
        if (GameManager._instance.boostTime > 5f) trailBooster.SetActive(true); //checking if boost is enabled
    }

    // Update is called once per frame
    void Update() {
        //if the velocity is too low increase it
        if (Mathf.Abs(rb.velocity.y) < 4.5f && Mathf.Abs(rb.velocity.x) < 4.5f) {
            rb.velocity = Vector2.down * Statistics._instance.ballSpeed;
        }
    }

    /// <summary>
    /// Trigger collision event
    /// If the ball hits the bottom boundary, it deactivates itself and goes back to ball pool
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Boundary") {
            gameObject.SetActive(false);
            ResetBall();

            Player._instance.ballsInGame--;
        }
    }

    /// <summary>
    /// Collision event
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision) {
        if (rb.velocity.y < 1 && rb.velocity.y > -1) {
            rb.velocity += Vector2.down * 2f;
        }

        if (collision.gameObject.tag == "Block")

            if (SoundManager._instance.hasSound)
                music.Play();
    }

    /// <summary>
    /// Resetting ball to default options
    /// </summary>
    void ResetBall() {
        trail.SetActive(false);
        trailBooster.SetActive(false);
        GetComponent<CircleCollider2D>().isTrigger = false;
        GetComponent<Rigidbody2D>().gravityScale = 0;
    }
}
