using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScaleEffectLoop : MonoBehaviour
{
    void OnEnable() {

        this.transform.DOKill();
        this.transform.localScale = Vector3.one;
        this.transform.DOScale(1.1f, 1).SetLoops(-1, LoopType.Yoyo);

    }
}
