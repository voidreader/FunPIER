using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweEnemyStatConrol : MonoBehaviour
{
    public float enemyHP;
    public bool limitOneDamgerPerFrame = true;//when enabled the esc can only take damage once per frame

    private bool canTakeDamage = true;
    void Update()
    {
        if (enemyHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {
        canTakeDamage = true;
    }

    public void Hit(float damage)
    {
        if (limitOneDamgerPerFrame == true && canTakeDamage)
        {
            enemyHP -= damage;
        }
        else if (!limitOneDamgerPerFrame)
        {
            enemyHP -= damage;
        }
    }
}
