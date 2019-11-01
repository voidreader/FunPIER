using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BigAura : MonoBehaviour
{
    public Transform BigFan, SmallFan;


    public void PlayAura() {
        BigFan.DOKill();
        SmallFan.DOKill();

        BigFan.localScale = Vector3.zero;
        SmallFan.localScale = Vector3.zero;

        BigFan.DOScale(1, 0.45f).SetEase(Ease.OutBack);
        SmallFan.DOScale(1, 0.4f).SetEase(Ease.OutBack).SetDelay(1);

        // 회전 
        BigFan.DOLocalRotate(new Vector3(0, 0, 720), 4, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        SmallFan.DOLocalRotate(new Vector3(0, 0, -720), 4, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);

    }
}
