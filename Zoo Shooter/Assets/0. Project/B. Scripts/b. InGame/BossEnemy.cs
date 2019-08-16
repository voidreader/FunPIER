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

    public override void HitEnemy(int d, bool isHeadShot) {

        this.transform.DOKill();

        // 보스의 경우 맞고 죽지 않는 경우 뒤로 살짝 밀리게 
        if(HP > d) {
            if(isLeft)
                this.transform.DOLocalMoveX(this.transform.localPosition.x - 0.1f, 0.05f).SetLoops(2, LoopType.Yoyo);
            else
                this.transform.DOLocalMoveX(this.transform.localPosition.x + 0.1f, 0.05f).SetLoops(2, LoopType.Yoyo);
        }

        base.HitEnemy(d, isHeadShot);
        

    }

    public override void KillEnemy(bool isHeadShot = false) {

        if (isKilled)
            return;

        Debug.Log("BossEnemy Killed");
        base.KillEnemy(isHeadShot);
        GameManager.main.ShowGetCoinTriple();  // 보스는 죽을때 코인 3개 떨군다. 

        // 인피니트 모드 로직 추가 
        if(PIER.main.InfiniteMode) {
            GameManager.main.SetNextInfiniteModeIndex();
            
        }
    }

}
