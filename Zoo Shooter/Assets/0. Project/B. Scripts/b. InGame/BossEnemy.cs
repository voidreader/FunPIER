using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google2u;
using PathologicalGames;
using DG.Tweening;

public class BossEnemy : Enemy {

    bool isHitMoving = false;

    public override void SetEnemy(EnemyType t, string pID) {
        base.SetEnemy(t, pID);
    }

    public override void HitEnemy(int d, bool isHeadShot) {

        if (isKilled)
            return;

        this.transform.DOKill();


        // 보스의 경우 맞고 죽지 않는 경우 뒤로 살짝 밀리게 
        if(HP > d && !isHitMoving) {
            if(isLeft)
                this.transform.DOLocalMoveX(this.transform.localPosition.x - 0.2f, 0.05f).SetLoops(2, LoopType.Yoyo).OnStart(OnStartHitMoving).OnComplete(OnFinishHitMoving);
            else
                this.transform.DOLocalMoveX(this.transform.localPosition.x + 0.2f, 0.05f).SetLoops(2, LoopType.Yoyo).OnStart(OnStartHitMoving).OnComplete(OnFinishHitMoving);
        }

        base.HitEnemy(d, isHeadShot);
    }

    public override void KillEnemy(bool isHeadShot = false) {

        if (isKilled)
            return;

        
        base.KillEnemy(isHeadShot);
        

        // 인피니트 모드 로직 추가 
        if(PIER.main.InfiniteMode) {
            GameManager.main.SetNextInfiniteModeIndex();
            GameManager.main.ShowGetCoin(); // 인피니트 모드는 보스 죽어도 코인 1개.
        }
        else {
            GameManager.main.ShowGetCoinTriple();  // 보스는 죽을때 코인 3개 떨군다. 
        }
    }

    void OnStartHitMoving() {
        isHitMoving = true;
    }

    void OnFinishHitMoving() {
        isHitMoving = false;
    }

}
