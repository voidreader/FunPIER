using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class SpawnActor_Extend_Ghost : SpawnActor_Extend
{
	//---------------------------------------
	private static int _spawnedGhostCount = 0;
	public static int SpawnedGhostCount { get { return _spawnedGhostCount; } }

	private static int _killedGhostCount = 0;
	public static int KilledGhostCount
	{
		get { return _killedGhostCount; }
		set { _killedGhostCount = value; }
	}

	//---------------------------------------
	[Header("GHOST INFO")]
    [SerializeField]
    private float _maxRotationRatio = 3.0f;
	[SerializeField]
	private float _maxYMoveRatio = 0.5f;
	[SerializeField]
    private Rigidbody _root = null;
    [SerializeField]
    private GameObject[] _tails = null;
    [SerializeField]
    private float _tailMaxDistance = 2.5f;
    [SerializeField]
    private float _moveSpeed = 2.0f;
	[SerializeField]
	private float _moveSpeed_OnMobile = 1.5f;
	[SerializeField]
	private float _offsetY = 0.0f;

	[Header("EFFECT INFO")]
	[SerializeField]
	private ParticleSystem _destroyEffect = null;
	[SerializeField]
	private ISkillObject _spawnObjectWhenExplode = null;

	//---------------------------------------
	public Rigidbody Root { get { return _root; } }

	//---------------------------------------
	private void OnEnable()
    {
		++_spawnedGhostCount;

	}

    private void OnDisable()
	{
		--_spawnedGhostCount;
	}

    //---------------------------------------
    public override void Init()
	{
		Vector3 vCurPos = gameObject.transform.position;
		Vector3 vFoward = gameObject.transform.forward;

		gameObject.transform.position = Vector3.zero;
		gameObject.transform.forward = Vector3.forward;

		//-----
		_root.transform.position = vCurPos;
		_root.transform.forward = vFoward;

		float fDefaultRotateY = _root.transform.rotation.eulerAngles.y;
		for (int nInd = 0; nInd < _tails.Length; ++nInd)
		{
			_tails[nInd].transform.position = vCurPos;
			//_tails[nInd].transform.forward = vFoward;
			_tails[nInd].transform.rotation = Quaternion.Euler(90.0f + fDefaultRotateY, 0.0f, 90.0f);
		}
	}

    public override void Frame()
    {
        float fViewLerpRate = _maxRotationRatio * HT.TimeUtils.GameTime;

        //-----
        Vector3 vPlayerPos = GetMoveTargetPos();

        Vector3 vCurForward = _root.transform.forward;
		vCurForward.y = 0.0f;
		vCurForward.Normalize();

		Vector3 vNextForward = vPlayerPos - _root.transform.position;
		vNextForward.y = 0.0f;
        vNextForward.Normalize();

        Vector3 vFoward = Vector3.Lerp(vCurForward, vNextForward, fViewLerpRate);
		_root.transform.forward = vFoward;

        //-----
        Vector3 vPrevRootPos = _root.transform.position;
		float fYOffset = Mathf.Lerp(vPrevRootPos.y, vPlayerPos.y + _offsetY, _maxYMoveRatio * HT.TimeUtils.GameTime);
		vPrevRootPos.y = fYOffset;

		float fSpeed = (HT.HTAfxPref.IsMobilePlatform) ? _moveSpeed_OnMobile : _moveSpeed;
		_root.transform.position = vPrevRootPos + (vFoward * (fSpeed * HT.TimeUtils.GameTime));

        //-----
        GameObject pPrev = _root.gameObject;

		for (int nInd = 0; nInd < _tails.Length; ++nInd)
        {
            Vector3 vPrevPos = pPrev.transform.position;
            Vector3 vNowPos = _tails[nInd].transform.position;

			_tails[nInd].transform.position = vNowPos;

			if (Vector3.Distance(vPrevPos, vNowPos) >= _tailMaxDistance)
            {
                Vector3 vMoveVec = (vNowPos - vPrevPos).normalized;
                Vector3 vIgnoreYPos = vPrevPos + (vMoveVec * _tailMaxDistance);

				vIgnoreYPos.y = fYOffset;

				_tails[nInd].transform.position = vIgnoreYPos;

				//-----
				Vector3 vPrevView = _tails[nInd].transform.right;
				Vector3 vCurView = Vector3.Lerp((vPrevPos - vNowPos).normalized, vPrevView, fViewLerpRate);

				_tails[nInd].transform.right = vCurView;

				Vector3 vEuler = _tails[nInd].transform.rotation.eulerAngles;
				vEuler.x = 90.0f;
				_tails[nInd].transform.rotation = Quaternion.Euler(vEuler);
			}

			pPrev = _tails[nInd];
		}
    }

	//---------------------------------------
	public override void Release()
	{
		if (_destroyEffect != null && BattleFramework._Instance.GameEnd == false)
		{
			ParticleSystem pExpPart = HT.Utils.InstantiateFromPool(_destroyEffect);
			if (pExpPart != null && _root != null)
			{
				pExpPart.transform.position = _root.transform.position;
				pExpPart.transform.localScale = _root.transform.localScale;

				HT.Utils.SafeDestroy(pExpPart.gameObject, pExpPart.TotalSimulationTime());
			}
		}
		
		if (_spawnObjectWhenExplode != null && BattleFramework._Instance.GameEnd == false)
		{
			ISkillObject pNewObj = HT.Utils.Instantiate(_spawnObjectWhenExplode);
			if (pNewObj != null && _root != null)
			{
				Vector3 vPos = _root.transform.position;
				pNewObj.transform.position = vPos;

				if (pNewObj.m_bInheritRotation)
					pNewObj.transform.rotation = _root.transform.rotation;

				pNewObj.m_pCaster = BattleFramework._Instance.m_pEnemyActor;
			}
		}
	}

    //---------------------------------------
    public virtual Vector3 GetMoveTargetPos()
    {
        return BattleFramework._Instance.m_pPlayerActor.transform.position;
	}

	//---------------------------------------
	public override void OnEvent_Damage(int nDamage)
	{
		++_killedGhostCount;
	}
}


/////////////////////////////////////////
//---------------------------------------