using System;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public sealed class SpawnActor_Extend_Ghost_Burst : SpawnActor_Extend_Ghost
{
    //---------------------------------------
    [Header("BURST INFO")]
    [SerializeField]
    private float _rotatePerSecond = 120.0f;
    [SerializeField]
    private float _lifeTime = 8.0f;

    private float _leastLifeTime = 0.0f;

    //---------------------------------------
    public override void Init()
    {
        base.Init();

        _leastLifeTime = _lifeTime;
    }

    public override void Frame()
    {
        base.Frame();

        _leastLifeTime -= HT.TimeUtils.GameTime;
        if (_leastLifeTime < 0.0f)
            HT.Utils.SafeDestroy(gameObject);
    }

    public override void Release()
    {
        base.Release();
    }

    //---------------------------------------
    public override Vector3 GetMoveTargetPos()
    {
        Vector3 vCurView = Root.transform.forward;

        float fRotateEuler = _rotatePerSecond * ((_leastLifeTime / _lifeTime) * 0.5f);
        Vector3 vNextView = Quaternion.Euler(0.0f, fRotateEuler * HT.TimeUtils.GameTime, 0.0f) * vCurView;
        return Root.transform.position + (vNextView * 5.0f);
    }

    //---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------
