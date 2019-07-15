﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GetCoin : MonoBehaviour
{


    public Transform target;
    public SpriteRenderer sprite;

    
    public void SetCoin(Transform t) {
        target = t;
        Vector3 pos = target.transform.position;
        pos = new Vector3(pos.x + Random.Range(-0.5f, 0.5f), pos.y+0.7f, 0);

        this.transform.position = pos;
        sprite.color = Color.white;

        this.gameObject.SetActive(true);
        this.transform.DOKill();

        this.transform.DOScale(1.2f, 0.5f).SetLoops(2, LoopType.Yoyo);
        this.transform.DOLocalMoveY(this.transform.localPosition.y + 1, 1f);
        sprite.DOFade(0, 0.4f).SetDelay(0.61f).OnComplete(OnCompleteTween);

    }

    void OnCompleteTween() {
        this.gameObject.SetActive(false);

        PIER.main.AddCoin(1);
    }
}