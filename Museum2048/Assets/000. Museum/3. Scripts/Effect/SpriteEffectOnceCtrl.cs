using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class SpriteEffectOnceCtrl : MonoBehaviour {
    public tk2dSpriteAnimator _anim = null;
    public bool _isOnBG;

    private void Start() {
        _anim.AnimationCompleted += AnimationCompleted;
    }

    void AnimationCompleted (tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip) {
        PoolManager.Pools[ConstBox.poolSpriteEffect].Despawn(this.transform);
    }


    void OnSpawned() {
        if (_anim == null)
            _anim = this.GetComponent<tk2dSpriteAnimator>();


        if(_isOnBG)
            this.transform.position = PierSystem.GetBGSpriteEffectRandomPosition();


        _anim.Play();
    }


}
