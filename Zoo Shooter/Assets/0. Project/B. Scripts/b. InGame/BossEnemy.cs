using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google2u;
using PathologicalGames;
using DG.Tweening;

public class BossEnemy : Enemy {

    public override void SetEnemy(EnemyType t, string pID) {
        base.SetEnemy(t, pID);
    }

    public override void KillEnemy() {

        
        base.KillEnemy();

        Debug.Log("BossEnemy Killed");

        this.rigid.AddForce(new Vector2(Random.Range(-100f, 0f), 450));
        // this.transform.DOLocalRotate(new Vector3(0, 0, 720), 2, RotateMode.FastBeyond360).SetEase(Ease.Linear);
        // this.rigid.isKinematic = true;
        

    }

}
