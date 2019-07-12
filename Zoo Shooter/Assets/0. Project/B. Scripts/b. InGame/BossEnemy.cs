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

    public override void HitEnemy(int d) {

        this.transform.DOKill();

        // 보스의 경우 맞고 죽지 않는 경우 뒤로 살짝 밀리게 
        if(HP > d) {
            if(isLeft)
                this.transform.DOLocalMoveX(this.transform.localPosition.x - 0.1f, 0.05f).SetLoops(2, LoopType.Yoyo);
            else
                this.transform.DOLocalMoveX(this.transform.localPosition.x + 0.1f, 0.05f).SetLoops(2, LoopType.Yoyo);
        }

        base.HitEnemy(d);

        GameViewManager.main.CalcBossHP(d); // HP 게이지 연동 추가 
    }

    public override void KillEnemy() {

        
        base.KillEnemy();

        Debug.Log("BossEnemy Killed");

        this.rigid.AddForce(new Vector2(Random.Range(-100f, 0f), 450));
        // this.transform.DOLocalRotate(new Vector3(0, 0, 720), 2, RotateMode.FastBeyond360).SetEase(Ease.Linear);
        // this.rigid.isKinematic = true;
        

    }

}
