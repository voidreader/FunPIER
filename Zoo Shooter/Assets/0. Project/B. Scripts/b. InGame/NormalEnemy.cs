using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google2u;
using PathologicalGames;

public class NormalEnemy : Enemy {

    public override void SetEnemy(EnemyType t, string pID) {
        base.SetEnemy(t, pID);
    }

    public override void KillEnemy() {

        Debug.Log("NormalEnemy Killed");

        base.KillEnemy();
    }

}
