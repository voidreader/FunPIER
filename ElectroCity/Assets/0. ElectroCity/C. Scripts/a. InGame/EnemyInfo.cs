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
        if (minion.HP <= 0)
            return true;

        return false;
    }
    


    public Vector3 GetTargetPosition() {
        return this.GetComponent<Minion>().body.transform.position;
    }

    public void InitMinion(int id, long hp) {
        IsBoss = false;
        minion.InitMinion(id, hp);
    }

    /// <summary>
    /// 데미지 주기!
    /// </summary>
    /// <param name="d"></param>
    public void SetDamage(long d) {
        minion.SetDamage(d);
    }

    public GameObject GetHitTarget() {
        return minion.body.gameObject;
    }

}
