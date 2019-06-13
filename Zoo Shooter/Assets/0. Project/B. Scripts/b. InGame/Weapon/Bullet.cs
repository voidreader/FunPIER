using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody2D BulletRB;

    public float BulletSpeed;

    public void AddBulletForce(Transform weaponTransform, float direcrion) {
        BulletRB.velocity = (weaponTransform.right * BulletSpeed) * direcrion;
        Destroy(gameObject, 1.5f);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        switch (collision.collider.tag) {
            case "Head":
                /*
                AudioManager.Instance.PlayAudio(AudioManager.Instance.Headshot);
                PlayFX.EnableFxAction(collision, 1);
                PlayFX.EnableFxAction(collision, 2);
                PlayerLevelManager.ChangeKillsAction();
                ScoreManager.Instance.AddScore(10);
                MoneyManager.Instance.AddMoney(10);
                */
                Destroy(collision.gameObject.GetComponentInParent<Enemy>().gameObject);
                Destroy(gameObject);
                break;
            case "Body":
                /*
                PlayFX.EnableFxAction(collision, 0);
                PlayFX.EnableFxAction(collision, 2);
                PlayerLevelManager.ChangeKillsAction();
                ScoreManager.Instance.AddScore(1);
                MoneyManager.Instance.AddMoney(1);
                */
                Destroy(collision.gameObject);
                Destroy(gameObject);
                break;
            case "Player":
                /*
                PlayFX.EnableFxAction(collision, 2);
                GameManager.GameOverAction();
                */
                // Destroy(collision.gameObject);
                Destroy(gameObject);
                break;
            default:
                // BulletRB.gravityScale = 1f;
                // BulletRB.gameObject.layer = 8;
                break;
        }
    }
}
