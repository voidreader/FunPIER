using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class HeadShot : MonoBehaviour
{

    public Animator anim;

    bool isSpawned = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnPlayed() {
        PoolManager.Pools[ConstBox.poolGame].Despawn(this.transform);
    }

    void OnSpawned() {
        anim.Play("HeadShot");
        isSpawned = true;

    }

    void OnDespawned() {
        isSpawned = false;
    }
}
