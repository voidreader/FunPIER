using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using PathologicalGames;

public class KillEffect : MonoBehaviour
{
    public Animator _anim;
    public SpriteRenderer _sprite;

    public void SetKillEffect(Vector3 pos) {

        this.transform.position = pos;
        this.gameObject.SetActive(true);
        _anim.Play("KillEffect");
        this.transform.DOKill();
            


        _sprite.color = Color.white;
        this.transform.DOLocalMoveY(this.transform.localPosition.y + 0.4f, 0.8f);
    }
    
    public void OnAnim() {
        _sprite.DOColor(new Color(1, 1, 1, 0), 0.5f).OnComplete(OnFade);
    }

    void OnFade() {

        if(PoolManager.Pools[ConstBox.poolGame].IsSpawned(this.transform))
            PoolManager.Pools[ConstBox.poolGame].Despawn(this.transform);


    }
}
