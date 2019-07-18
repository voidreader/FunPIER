using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 5;


    public GameObject BulletPrefab;
    public GameObject MuzzlePrefab;


    void Start() {
        GameObject b = Instantiate(BulletPrefab, this.transform, true);
        b.transform.localPosition = Vector3.zero;
        b.transform.localEulerAngles = new Vector3(90, 0, 0);
        


    }

    private void FixedUpdate() {
        // rb.position += transform.forward * speed * Time.deltaTime);
        rb.transform.Translate(transform.right * speed * Time.deltaTime);
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        switch (collision.collider.tag) {
        }
    }
}
