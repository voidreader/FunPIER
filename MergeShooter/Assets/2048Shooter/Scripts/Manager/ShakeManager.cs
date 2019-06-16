using DG.Tweening;
using System;
using Toolkit;
using UnityEngine;

public class ShakeManager : MonoSingleton<ShakeManager>
{
    [SerializeField]
    private float m_ShakeTime;

    [SerializeField]
    private Vector3 m_ShakeValue = Vector3.zero;

    private Vector3 m_DefPos = Vector3.zero;

    protected override void Awake()
    {
        base.Awake();
        this.m_DefPos = base.transform.localPosition;
    }

    public void DoShake()
    {
        this.ScreenShake();

        if(GameDataManager.Instance.PhoneShake) Handheld.Vibrate();
    }


    public void ScreenShake()
    {
        base.transform.DOShakePosition(this.m_ShakeTime, this.m_ShakeValue, 10, 90f, false, true).OnComplete(delegate
        {
            base.transform.localPosition = this.m_DefPos;
        });
    }
}
