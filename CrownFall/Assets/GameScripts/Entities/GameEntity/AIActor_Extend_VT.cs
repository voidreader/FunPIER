using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
[Serializable]
public struct TransformRect
{
	public GameObject _minPos;
	public GameObject _maxPos;
}


/////////////////////////////////////////
//---------------------------------------
public sealed class AIActor_Extend_VT : AIActor_Extend
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("EXTEND SETTINGS")]
	[SerializeField]
	private GameObject _head = null;
	[SerializeField]
	private Rigidbody _headRigidBody = null;
	[SerializeField]
	private CapsuleCollider _headCollider = null;
	[SerializeField]
	private GameObject _headRoot = null;
	private Animation _headAnimation = null;
	[SerializeField]
	private string _headAnim_Open = null;
	[SerializeField]
	private string _headAnim_Opened = null;
	[SerializeField]
	private string _headAnim_Close = null;
	[SerializeField]
	private string _headAnim_Closed = null;

	[Header("BUD - EXPLOSION SEED")]
	[SerializeField]
	private BossVT_FlowerBud_Seed _bud = null;
	private Animation _budAnimation = null;

	[Header("ROOTS")]
	[SerializeField]
	private GameObject[] _roots = null;
	private Animation[] _rootsAnimation = null;

	[Header("BONES")]
	[SerializeField]
	private GameObject _root = null;
	[SerializeField]
	private GameObject[] _bodyBones = null;
	[SerializeField]
	private float _maxHeight = 5.0f;

	//---------------------------------------
	[Header("BITE")]
	[SerializeField]
	private float[] _bite_RepeatTime = null;
	[SerializeField]
	private float _bite_FirstCooltimeRatio = 0.5f;
	[SerializeField]
	private float _bite_TimeDelay = 1.5f;
	[SerializeField]
	private float[] _bite_Speed = null;
	[SerializeField]
	private float[] _bite_CatchRange = null;
	[SerializeField]
	private int[] _bite_JoystickCnt = null;
	[SerializeField]
	private int[] _bite_JoystickCnt_Mobile = null;
	[SerializeField]
	private int[] _bite_JoystickCnt_Joystick = null;
	[SerializeField]
	private string _bite_attachDummyName = "HEAD_BITE_POS";
	[SerializeField]
	private ActorBuff _bite_DmgBuff = null;
	[SerializeField]
	private AudioClip _bite_SuccessSounds = null;

	private float _bite_LeastTime = 0.0f;

	//---------------------------------------
	[Header("SEED SPREAD")]
	[SerializeField]
	private float[] _seedSprd_RepeatTime = null;
	[SerializeField]
	private float _seedSprd_FirstCooltimeRatio = 0.5f;
	[SerializeField]
	private int[] _seedSprd_SeedCount = null;
	[SerializeField]
	private Rigidbody[] _seedSprd_SeedInstance = null;
	[SerializeField]
	private Vector2 _seedSprd_YVecRange = Vector2.zero;
	[SerializeField]
	private Vector2 _seedSprd_SpeedRange = Vector2.zero;
	[SerializeField]
	private string _seedSprd_BudAnim_Idle = null;
	[SerializeField]
	private string[] _seedSprd_BudAnim_Shoot = null;
	[SerializeField]
	private string _seedSprd_BudPosDummyName = "SEED_SPREAD";

	private GameObject _seedSprd_budPosDummy = null;
	private float _seedSprd_LeastTime = 0.0f;
	private int _seedSprd_LeastCount = 0;

	//---------------------------------------
	[Header("TWIST VINE")]
	[SerializeField]
	private float[] _twistVine_RepeatTime = null;
	[SerializeField]
	private string _twistVine_Dmy_Start = null;
	[SerializeField]
	private string _twistVine_Dmy_MidName = null;
	[SerializeField]
	private string _twistVine_Dmy_CnrName = null;
	[SerializeField]
	private BossVT_TwistVine[] _twistVine_Instance = null;

	private float _twistVine_LeastTime = 0.0f;

	//---------------------------------------
	[Header("STRETCH ROOTS")]
	[SerializeField]
	private BossVT_StretchRoot[] _strRoots = null;
	[SerializeField]
	private float[] _strRoots_RepeatTime = null;
	[SerializeField]
	private BossVT_FlowerBud_Extend[] _strRoots_ExtendBuds = null;
	[SerializeField]
	private float _strRoots_DecreaseWhenExtend = 0.8f;
    [SerializeField]
	private float[] _strRoots_StretchRatioDecPerRoot_Lv1 = null;
	[SerializeField]
	private float[] _strRoots_StretchRatioDecPerRoot_Lv2 = null;
	[SerializeField]
	private float[] _strRoots_StretchRatioDecPerRoot_Lv3 = null;

	private float _strRoots_LeastTime = 0.0f;

    public int StretchRootCompleteCount()
    {
        int nCompleteCount = 0;
        for (int nInd = 0; nInd < _strRoots.Length; ++nInd)
            if (_strRoots[nInd].IsProcessEnd())
                ++nCompleteCount;

        return nCompleteCount;
    }

	public float[] GetStrRoots_StretchRatioDec()
	{
		switch(GameFramework._Instance.m_pPlayerData.m_eDifficulty)
		{
			case eGameDifficulty.eEasy:
				return _strRoots_StretchRatioDecPerRoot_Lv1;

			case eGameDifficulty.eNormal:
				return _strRoots_StretchRatioDecPerRoot_Lv2;
		}

		return _strRoots_StretchRatioDecPerRoot_Lv3;
	}

    //---------------------------------------
    [Header("STRETCH ROOTS - ARCHIVES")]
	[SerializeField]
	private string _acv_Bud_Name = null;
	[SerializeField]
	private int _acv_Bud_Count = 3;


	//---------------------------------------
	private Coroutine _headActionProc = null;
	private Coroutine _budActionProc = null;

	private bool _headOpened = true;

    //---------------------------------------
    private void Awake()
	{
		_headAnimation = _head.GetComponent<Animation>();

		_budAnimation = _bud.GetComponent<Animation>();

		_rootsAnimation = new Animation[_roots.Length];
		for (int nInd = 0; nInd < _roots.Length; ++nInd)
			_rootsAnimation[nInd] = _roots[nInd].GetComponent<Animation>();

		for (int nInd = 0; nInd < _strRoots.Length; ++nInd)
			_strRoots[nInd].onStretchComplete += OnExtendBud;

		//-----
		_head.transform.localPosition = Vector3.zero;
		_bud.SetSpreadSeedAnimCallback(OnSeedSpreadAnim);
	}

	private void Update()
	{
		Vector3 vViewDir = _head.transform.position - _root.transform.position;
		vViewDir.Normalize();

		float fTotalDistance = Vector3.Distance(_head.transform.position, _root.transform.position);
		float fBoneOffset = fTotalDistance / _bodyBones.Length;

		float fCurHeight = _head.transform.position.y;
		for (int nInd = 0; nInd < _bodyBones.Length; ++nInd)
		{
			GameObject pBone = _bodyBones[nInd];
			Vector3 vPos = _root.transform.position + (vViewDir * (fBoneOffset * nInd));

			float fHalfHeight = _maxHeight * 0.5f;
			float fHalfLength = _bodyBones.Length * 0.5f;
			if (nInd <= fHalfLength)
			{
				float fCurRatio = nInd / fHalfLength;
				vPos.y = Mathf.Lerp(_root.transform.position.y, fHalfHeight, fCurRatio);
			}
			else
			{
				float fCurRatio = (nInd - fHalfLength) / (fHalfLength + 1.0f);
				vPos.y = Mathf.Lerp(fHalfHeight, fCurHeight, fCurRatio);
			}

			pBone.transform.position = vPos;

			float fLerp = (1.0f / _bodyBones.Length) * nInd;
			pBone.transform.rotation = Quaternion.Lerp(_root.transform.rotation, _head.transform.rotation, fLerp);
		}
	}

	//---------------------------------------
	public override void Extended_Init()
	{
		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;

		_bite_LeastTime = _bite_RepeatTime[nDiff];
		_bite_LeastTime *= _bite_FirstCooltimeRatio;

		_seedSprd_LeastTime = _seedSprd_RepeatTime[nDiff];
		_seedSprd_LeastTime *= _seedSprd_FirstCooltimeRatio;

		_twistVine_LeastTime = _twistVine_RepeatTime[nDiff];

		_strRoots_LeastTime = _strRoots_RepeatTime[nDiff];

		//-----
		_seedSprd_budPosDummy = m_pActorBase.FindDummyPivot(_seedSprd_BudPosDummyName);
	}

	public override void Extended_PostInit()
	{
	}

	public override void Extended_Frame()
	{
		if (BattleFramework._Instance == null)
			return;

		//-----
		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;

		//-----
		if (_bite_LeastTime > 0.0f)
			_bite_LeastTime -= HT.TimeUtils.GameTime;

		if (_headActionProc == null)
		{
			HeadOpen();

			//-----
			//Vector3 vForward = _headRoot.transform.forward;
			//Vector3 vViewVec = BattleFramework._Instance.m_pPlayerActor.transform.position - _headRoot.transform.position;
			//_headRoot.transform.forward = Vector3.MoveTowards(vForward, vViewVec.normalized, 1.0f * HT.TimeUtils.GameTime);

			//Vector3 vRootPos = _root.transform.position;
			//vViewVec.y = 0.0f;
			//vRootPos.y = 0.0f;
			//Vector3 vHeadPosVec = (vViewVec - vRootPos).normalized;
			//Vector3 vHeadPos = vRootPos + (vHeadPosVec * 1.0f);
			//vHeadPos.y = _headRoot.transform.position.y;
			//_headRoot.transform.position = Vector3.MoveTowards(_headRoot.transform.position, vHeadPos, 1.0f * HT.TimeUtils.GameTime);

			//-----
			if (_bite_LeastTime < 0.0f)
			{
				_bite_LeastTime = _bite_RepeatTime[nDiff];
				_headActionProc = StartCoroutine(Bite_Internal());
			}
		}

		//-----
		if (_seedSprd_LeastTime > 0.0f)
			_seedSprd_LeastTime -= HT.TimeUtils.GameTime;

		if (_budActionProc == null)
		{
			if (_seedSprd_LeastTime < 0.0f)
			{
				_seedSprd_LeastTime = _seedSprd_RepeatTime[nDiff];

				_seedSprd_LeastCount = _seedSprd_SeedCount[nDiff];
				_budActionProc = StartCoroutine(SeedSpread_Internal());
			}
		}

		//-----
		if (_twistVine_LeastTime > 0.0f)
			_twistVine_LeastTime -= HT.TimeUtils.GameTime;

		else
		{
			_twistVine_LeastTime = _twistVine_RepeatTime[nDiff];
			CreateTwistVine();
		}

		//-----
		if (_strRoots_LeastTime > 0.0f)
			_strRoots_LeastTime -= HT.TimeUtils.GameTime;

		else
		{
			bool bLeastRoot = false;
			for (int nInd = 0; nInd < _strRoots.Length; ++nInd)
			{
				if (_strRoots[nInd].StretchProcessing() == false)
				{
					bLeastRoot = true;
					break;
				}
			}

			if (bLeastRoot)
			{
				_strRoots_LeastTime = _strRoots_RepeatTime[nDiff];
				for (int nInd = 0; nInd < 1000; ++nInd)
				{
					int nRandom = HT.RandomUtils.Range(0, _strRoots.Length);
					if (_strRoots[nRandom].StretchProcessing() == false)
					{
						_strRoots[nRandom].StretchStart();
						break;
					}
				}
			}
			else
			{
				_strRoots_LeastTime = float.PositiveInfinity;
			}
		}
	}

	public override void Extended_Release()
	{
	}


	//---------------------------------------
	public override void OnBattleStart()
	{
		base.OnBattleStart();
	}

	public override void OnBattleEnd(bool bPlayerWin)
	{
		base.OnBattleEnd(bPlayerWin);

		ControllableActor pPlayer = BattleFramework._Instance.m_pPlayerActor as ControllableActor;
		if (_headActionProc != null)
		{
			BattleFramework._Instance._ui_padArrowShake.SetActive(false);

			HT.Utils.SafeStopCorutine(this, ref _headActionProc);

			pPlayer.RemoveActorBuff(_bite_DmgBuff, false);
			pPlayer.m_pRigidBody.useGravity = true;
			pPlayer.SetControlEnable(true);

			if (bPlayerWin == false)
				pPlayer.SetAction(pPlayer.m_szDEATHAnimName);
		}

		if (_budActionProc != null)
		{
			_budAnimation.Play(_seedSprd_BudAnim_Idle);

			StopCoroutine(_budActionProc);
			_budActionProc = null;
		}

		//-----
		for (int nInd = 0; nInd < _strRoots.Length; ++nInd)
			_strRoots[nInd].PauseStretch();

		for (int nInd = 0; nInd < _strRoots_ExtendBuds.Length; ++nInd)
			_strRoots_ExtendBuds[nInd].StopAllCoroutines();
	}

	//---------------------------------------
	private IEnumerator Bite_Internal()
	{
		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;
		HeadOpen();

		//-----
		ControllableActor pPlayer = BattleFramework._Instance.m_pPlayerActor as ControllableActor;

		Vector3 vStartPos = _headRoot.transform.position;
		Vector3 vTargetPos = pPlayer.transform.position;
		float fHeadMoveTime = Vector3.Distance(vStartPos, vTargetPos) / _bite_Speed[nDiff];
		BattleFramework._Instance.CreateAreaAlert(vTargetPos, _bite_CatchRange[nDiff], _bite_TimeDelay + fHeadMoveTime);

		//-----
		Vector3 vStartHeadRot = _headRoot.transform.forward;
		Vector3 vTargetHeadRot = (vTargetPos - vStartPos).normalized;
		float fHeadRotateTime = _bite_TimeDelay;
		while(fHeadRotateTime > 0.0f)
		{
			fHeadRotateTime -= HT.TimeUtils.GameTime;
			_headRoot.transform.forward = Vector3.Lerp(vStartHeadRot, vTargetHeadRot, 1.0f - (fHeadRotateTime / _bite_TimeDelay));

			yield return new WaitForEndOfFrame();
		}

		//-----
		Vector3 vYZeroHeadPos = _headRoot.transform.position;
		float fHeadStartY = vYZeroHeadPos.y;

		vYZeroHeadPos.y = vTargetPos.y;
		float fMoveDistance = Vector3.Distance(vTargetPos, vYZeroHeadPos);

		while (true)
		{
			vYZeroHeadPos.y = vTargetPos.y;
			vYZeroHeadPos = Vector3.MoveTowards(vYZeroHeadPos, vTargetPos, _bite_Speed[nDiff] * HT.TimeUtils.GameTime);

			float fLeastDist = Vector3.Distance(vTargetPos, vYZeroHeadPos);
			if (fLeastDist > 0.1f)
			{
				float fDistRatio = fLeastDist / fMoveDistance;
				if (fDistRatio > 0.5f)
					vYZeroHeadPos.y = fHeadStartY;

				else
				{
					float fHeightOffset = Mathf.Sin(Mathf.PI * fDistRatio) * (fHeadStartY - vTargetPos.y);
					fHeightOffset += vTargetPos.y;

					vYZeroHeadPos.y = fHeightOffset;
				}
			}

			_headRoot.transform.position = vYZeroHeadPos;

			if (fLeastDist < 0.1f)
				break;

			yield return new WaitForEndOfFrame();
		}

		//-----
		bool bCatchSuccessed = false;
		GameObject pBiteAttachDummy = null;
		if (Vector3.Distance(pPlayer.transform.position, vTargetPos) < _bite_CatchRange[nDiff])
		{
			HT.HTSoundManager.PlaySound(_bite_SuccessSounds);

			bCatchSuccessed = true;
			pBiteAttachDummy = m_pActorBase.FindDummyPivot(_bite_attachDummyName);

			pPlayer.m_pRigidBody.useGravity = false;
			pPlayer.SetControlEnable(false);
		}

		//-----
		HeadClose(bCatchSuccessed);

		//-----
		float fBackTotalDistance = Vector3.Distance(vStartPos, _headRoot.transform.position);
		Vector3 vYZeroStartPos = vStartPos;
		vYZeroStartPos.y = vTargetPos.y;
		vYZeroHeadPos = vTargetPos;

		while (true)
		{
			vYZeroHeadPos.y = vTargetPos.y;
			vYZeroHeadPos = Vector3.MoveTowards(vYZeroHeadPos, vYZeroStartPos, _bite_Speed[nDiff] * HT.TimeUtils.GameTime);

			float fLeastDist = Vector3.Distance(vYZeroHeadPos, vYZeroStartPos);
			if (fLeastDist > 0.1f)
			{
				float fDistRatio = 1.0f - (fLeastDist / fBackTotalDistance);
				if (fDistRatio > 0.5f)
					vYZeroHeadPos.y = vStartPos.y;
				else
				{
					float fHeightOffset = Mathf.Sin(Mathf.PI * fDistRatio) * (vStartPos.y - vTargetPos.y);
					fHeightOffset += vTargetPos.y;

					vYZeroHeadPos.y = fHeightOffset;
				}
			}

			_headRoot.transform.position = vYZeroHeadPos;

			//-----
			float fMoveBackRatio = 1.0f - (fLeastDist / fBackTotalDistance);
			_headRoot.transform.forward = Vector3.Lerp(vTargetHeadRot, (bCatchSuccessed)? Vector3.up : vStartHeadRot, fMoveBackRatio);

			if (bCatchSuccessed)
				pPlayer.transform.position = pBiteAttachDummy.transform.position;

			if (fLeastDist < 0.1f)
				break;

			//-----
			yield return new WaitForEndOfFrame();
		}

		//-----
		_headRoot.transform.position = vStartPos;

		//-----
		if (bCatchSuccessed)
		{
			BattleFramework._Instance._ui_padArrowShake.SetActive(true);

			//-----
			pPlayer.AddActorBuff(_bite_DmgBuff);

			int nJoystickCnt = 0;
			if (HT.HTAfxPref.IsMobilePlatform)
				nJoystickCnt = _bite_JoystickCnt_Mobile[nDiff];
			else if (HT.HTInputManager.Instance.JoystickConnected)
				nJoystickCnt = _bite_JoystickCnt_Joystick[nDiff];
			else
				nJoystickCnt = _bite_JoystickCnt[nDiff];

			//-----
			float fHWeight = 0.0f;
			float fVWeight = 0.0f;

			bool bPressedA = false;
			bool bPressedD = false;
			bool bPressedS = false;
			bool bPressedW = false;

			while (nJoystickCnt > 0)
			{
				if (HT.TimeUtils.TimeScale > 0.0f)
				{
					if (HT.HTAfxPref.IsMobilePlatform || HT.HTInputManager.Instance.JoystickConnected)
					{
						float fCurHWeight = HT.HTInputManager.Instance.Horizontal;
						float fCurVWeight = HT.HTInputManager.Instance.Vertical;

						if (Mathf.Abs(fCurHWeight) > 0.1f)
						{
							if ((fHWeight >= 0.0f) && fHWeight > fCurHWeight)
							{
								fHWeight = fCurHWeight;
								--nJoystickCnt;
							}
							else if ((fHWeight <= 0.0f) && fHWeight < fCurHWeight)
							{
								fHWeight = fCurHWeight;
								--nJoystickCnt;
							}
						}

						if (Mathf.Abs(fCurVWeight) > 0.1f)
						{
							if ((fVWeight >= 0.0f) && fVWeight > fCurVWeight)
							{
								fVWeight = fCurVWeight;
								--nJoystickCnt;
							}
							else if ((fVWeight <= 0.0f) && fVWeight < fCurVWeight)
							{
								fVWeight = fCurVWeight;
								--nJoystickCnt;
							}
						}
					}
					else
					{
						if (Input.GetKey(KeyCode.A))
						{
							if (bPressedA == false)
								--nJoystickCnt;

							bPressedA = true;
						}
						else
							bPressedA = false;

						if (Input.GetKey(KeyCode.D))
						{
							if (bPressedD == false)
								--nJoystickCnt;

							bPressedD = true;
						}
						else
							bPressedD = false;

						if (Input.GetKey(KeyCode.W))
						{
							if (bPressedW == false)
								--nJoystickCnt;

							bPressedW = true;
						}
						else
							bPressedW = false;

						if (Input.GetKey(KeyCode.S))
						{
							if (bPressedS == false)
								--nJoystickCnt;

							bPressedS = true;
						}
						else
							bPressedS = false;
					}
				}

				//-----
				pPlayer.m_pRigidBody.velocity = Vector3.zero;
				pPlayer.transform.position = pBiteAttachDummy.transform.position;

				//-----
				yield return new WaitForEndOfFrame();
			}

			//-----
			pPlayer.RemoveActorBuff(_bite_DmgBuff, false);

			pPlayer.m_pRigidBody.useGravity = true;
			pPlayer.SetControlEnable(true);

			//-----
			BattleFramework._Instance._ui_padArrowShake.SetActive(false);
			HeadOpen();

			float fHeadRotTime = 1.0f;
			while(fHeadRotTime > 0.0f)
			{
				fHeadRotTime -= HT.TimeUtils.GameTime;
				_headRoot.transform.forward = Vector3.Lerp(Vector3.up, vStartHeadRot, 1.0f - fHeadRotTime);
				
				yield return new WaitForEndOfFrame();
			}
		}

        //-----
        //HeadOpen();
        _bite_LeastTime = _bite_RepeatTime[nDiff];
        _headActionProc = null;
    }


	//---------------------------------------
	private IEnumerator SeedSpread_Internal()
	{
		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;
		_budAnimation.Play(_seedSprd_BudAnim_Shoot[nDiff]);

		while (_seedSprd_LeastCount > 0.0f)
			yield return new WaitForEndOfFrame();

		//-----
		_budAnimation.Play(_seedSprd_BudAnim_Idle);
		_budActionProc = null;
	}

	private void OnSeedSpreadAnim()
	{
		if (_seedSprd_LeastCount > 0)
		{
			--_seedSprd_LeastCount;

			//-----
			int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;
			Rigidbody pSeed = Instantiate(_seedSprd_SeedInstance[nDiff]);
			pSeed.transform.position = _seedSprd_budPosDummy.transform.position;

			Vector3 vVector = Vector3.forward;
			vVector.y = UnityEngine.Random.Range(_seedSprd_YVecRange.x, _seedSprd_YVecRange.y);
			vVector.Normalize();

			vVector = Quaternion.Euler(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f) * vVector;
			float fSpeed = UnityEngine.Random.Range(_seedSprd_SpeedRange.x, _seedSprd_SpeedRange.y);

			pSeed.AddForce(vVector * fSpeed, ForceMode.VelocityChange);
		}
	}


	//---------------------------------------
	private void CreateTwistVine()
	{
		int nDirection = UnityEngine.Random.Range(0, 4);
		Field pField = BattleFramework._Instance.m_pField;

		GameObject pStartPos = pField.FindDummyPivot(_twistVine_Dmy_Start);
		GameObject pDirPos = pField.FindDummyPivot(string.Format(_twistVine_Dmy_MidName, nDirection));

		GameObject[] pPivots = new GameObject[7];
		pPivots[0] = pStartPos;
		pPivots[1] = pDirPos;

		bool bCW = (UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f) ? true : false;
		{
			int nCurPos = nDirection + ((bCW)? 0 : 1);
			if (nCurPos > 3)
				nCurPos = 0;

			for(int nInd = 2; nInd < 7; ++nInd)
			{
				pPivots[nInd] = pField.FindDummyPivot(string.Format(_twistVine_Dmy_CnrName, nCurPos));

				nCurPos += (bCW) ? -1 : 1;
				if (nCurPos > 3)
					nCurPos = 0;
				else if (nCurPos < 0)
					nCurPos = 3;
			}
		}

		//-----
		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;
		BossVT_TwistVine pVine = HT.Utils.Instantiate(_twistVine_Instance[nDiff]);
		pVine.Init(pPivots);
	}


	//---------------------------------------
	public void HeadOpen(bool bInstantly = false)
	{
		if (_headOpened)
			return;

		_headOpened = true;

		if (bInstantly)
			_headAnimation.Play(_headAnim_Opened);
		else
			_headAnimation.Play(_headAnim_Open);
	}

	public void HeadClose(bool bInstantly = false)
	{
		if (_headOpened == false)
			return;

		_headOpened = false;

		if (bInstantly)
			_headAnimation.Play(_headAnim_Closed);
		else
			_headAnimation.Play(_headAnim_Close);
	}

	public void OnDeathAnimation()
	{
		_headOpened = false;
		_headAnimation.Play(_headAnim_Close);

		//-----
		_headRigidBody.useGravity = true;
		_headRigidBody.constraints = RigidbodyConstraints.FreezeRotationX;

		_headCollider.enabled = true;
	}


	//---------------------------------------
	private void OnExtendBud(BossVT_FlowerBud pBud)
	{
		bool bFound = false;
		for (int nInd = 0; nInd < _strRoots_ExtendBuds.Length; ++nInd)
		{
			if (_strRoots_ExtendBuds[nInd].IsExtended == false)
			{
				if (_strRoots_ExtendBuds[nInd].BudConditionType == pBud.BudType)
				{
					bFound = true;
					_strRoots_ExtendBuds[nInd].SetExtend(pBud);

					break;
				}
			}
		}

		if (bFound == false)
		{
			for (int nInd = 0; nInd < _strRoots_ExtendBuds.Length; ++nInd)
			{
				if (_strRoots_ExtendBuds[nInd].IsExtended == false)
				{
					if (_strRoots_ExtendBuds[nInd].BudConditionType != BossVT_FlowerBud.eBudType.MAX)
					{
						if (_strRoots_ExtendBuds[nInd].BudConditionType != pBud.BudType)
							continue;
					}

					_strRoots_ExtendBuds[nInd].SetExtend(pBud);
					break;
				}
			}
		}

		for (int nInd = 0; nInd < _strRoots.Length; ++nInd)
			_strRoots[nInd].OnDecreaseStretch(_strRoots_DecreaseWhenExtend, true);
	}

	//---------------------------------------
	public override void Extended_EventCallback(AIActor.eActorEventCallback eEvent, GameObject pParam)
	{
		switch(eEvent)
		{
			case AIActor.eActorEventCallback.eDamaged:
				{
					if (m_pActorBase.GetCurrHP() <= 0)
					{
						OnRootsDEATH();

						//-----
						int nBudCount = 0;
						for (int nInd = 0; nInd < _strRoots_ExtendBuds.Length; ++nInd)
							if (_strRoots_ExtendBuds[nInd].IsExtended)
								++nBudCount;

						if (nBudCount < _acv_Bud_Count)
						{
							Archives pArchives = ArchivementManager.Instance.FindArchive(_acv_Bud_Name);
							pArchives.Archive.OnArchiveCount(1);
						}
					}
				}
				break;
		}
	}

	//---------------------------------------
	public void OnRootsBattleStart()
	{
		OnRootsAnimation("BATTLESTART");
	}

	public void OnRootsIDLE()
	{
		OnRootsAnimation("IDLE");
	}

	public void OnRootsDEATH()
	{
		OnRootsAnimation("DEATH");
	}

	private void OnRootsAnimation(string szName)
	{
		for (int nInd = 0; nInd < _rootsAnimation.Length; ++nInd)
			_rootsAnimation[nInd].Play(szName);
	}
}


/////////////////////////////////////////
//---------------------------------------