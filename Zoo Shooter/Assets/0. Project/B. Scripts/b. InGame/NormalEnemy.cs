using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google2u;
using PathologicalGames;
using DG.Tweening;

public class NormalEnemy : Enemy {

    public override void SetEnemy(EnemyType t, string pID) {
        base.SetEnemy(t, pID);
    }

    public override void KillEnemy(bool isHeadShot = false) {

        Debug.Log("NormalEnemy Killed");
        base.KillEnemy(isHeadShot);


    }




}
