using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    [SerializeField] bool isBoss = false;

    [SerializeField] Minion minion;
    [SerializeField] Boss boss;

    public bool IsBoss {
        get { return isBoss; }
        set {
            isBoss = value;

            if (isBoss) {
                boss = this.GetComponent<Boss>();
                minion = null;
            }
            else {
                minion = this.GetComponent<Minion>();
                boss = null;
            }

        }
    }


    public bool IsDestroy() {

        if(isBoss && boss.HP <= 0) {
            return true;
        }
        else if (!isBoss && minion.HP <= 0) {
            return true;
        }


        return false;
    }
    


    public Vector3 GetTargetPosition() {
        return this.GetComponent<Minion>().body.transform.position;
    }

    public void InitMinion(int id, long hp) {
        IsBoss = false;
        minion.InitMinion(id, hp);
    }

    public void InitBoss(int id, long hp) {
        isBoss = true;
        boss.InitBoss(id, hp);
    }

    /// <summary>
    /// 데미지 주기!
    /// </summary>
    /// <param name="d"></param>
    public void SetDamage(long d) {

        if (isBoss)
            boss.SetDamage(d);
        else
            minion.SetDamage(d);
    }

    public GameObject GetHitTarget() {
        if(isBoss)
            return boss.body.gameObject;
        else
            return minion.body.gameObject;
    }

    public void BreakImmediate() {
        this.SendMessage("BreakUnit");
    }

}
