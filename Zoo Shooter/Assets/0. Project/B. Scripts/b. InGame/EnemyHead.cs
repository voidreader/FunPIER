using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHead : MonoBehaviour
{

    public Enemy avatar = null;

    
    public void OnRigidBody() {
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
     
        if(collision.collider.tag == "Bullet") {
            avatar.HitEnemy(GameManager.main.currentWeapon.Damage, true);
        }
    }
}
