using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class BossBC_CopiedActor_VT : BossBC_CopiedActor
{
    //---------------------------------------
    [Header("OBJECT INFO")]
    [SerializeField]
    private GameObject _head = null;
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

    [Header("BONES INFO")]
    [SerializeField]
    private GameObject _root = null;
    [SerializeField]
    private GameObject[] _bodyBones = null;
    [SerializeField]
    private float _maxHeight = 5.0f;

    //---------------------------------------
    [Header("SPREAD SEED")]
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
    private int _seedSprd_LeastCount = 0;

	//---------------------------------------
	[Header("BITE")]
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

	//---------------------------------------
	[Header("BUD - EXPLOSION SEED")]
    [SerializeField]
    private BossVT_FlowerBud_Seed _bud = null;
    private Animation _budAnimation = null;

    //---------------------------------------
    [Header("SKILL INFO")]
    private float _waitForNextSkill = 1.0f;

    //---------------------------------------
    private Coroutine _vtProc = null;
    private bool _procCompleted = false;

    private bool _headOpened = true;

    //---------------------------------------
    public override void CopyActor_Init(bool bUseOnlyOneSkill)
    {
        base.CopyActor_Init(bUseOnlyOneSkill);

        //-----
        if (_headAnimation == null)
            _headAnimation = _head.GetComponent<Animation>();

        if (_budAnimation == null)
            _budAnimation = _bud.GetComponent<Animation>();

        _bud.SetSpreadSeedAnimCallback(OnSeedSpreadAnim);

        _seedSprd_budPosDummy = FindDummyPivot(_seedSprd_BudPosDummyName);

        _head.transform.localPosition = Vector3.zero;

        //-----
        _procCompleted = false;
    }

    public override bool CopyActor_Frame()
    {
        if(base.CopyActor_Frame() == false)
        {
            if (_vtProc == null)
            {
                if (_procCompleted == false)
                    _vtProc = StartCoroutine(VT_Proc());

                else
                    return false;
            }
        }

        //-----
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

        //-----

        return true;
    }

    public override void CopyActor_Release()
    {
        base.CopyActor_Release();
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
		if (_vtProc != null)
		{
			BattleFramework._Instance._ui_padArrowShake.SetActive(false);

			HT.Utils.SafeStopCorutine(this, ref _vtProc);

			pPlayer.RemoveActorBuff(_bite_DmgBuff, false);
			pPlayer.m_pRigidBody.useGravity = true;
			pPlayer.SetControlEnable(true);

			if (bPlayerWin == false)
				pPlayer.SetAction(pPlayer.m_szDEATHAnimName);
		}
	}

	//---------------------------------------
	private IEnumerator VT_Proc()
    {
        int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;
        string budAnimation = _seedSprd_BudAnim_Shoot[nDiff];
        _seedSprd_LeastCount = _seedSprd_SeedCount[nDiff];

        _budAnimation.Play(budAnimation);

        //-----
        yield return new WaitForSeconds(_waitForNextSkill);

        //-----
		if (_useOnlyOneSkill == false)
		{
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
			while (fHeadRotateTime > 0.0f)
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
				pBiteAttachDummy = FindDummyPivot(_bite_attachDummyName);

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
				_headRoot.transform.forward = Vector3.Lerp(vTargetHeadRot, (bCatchSuccessed) ? Vector3.up : vStartHeadRot, fMoveBackRatio);

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
				while (fHeadRotTime > 0.0f)
				{
					fHeadRotTime -= HT.TimeUtils.GameTime;
					_headRoot.transform.forward = Vector3.Lerp(Vector3.up, vStartHeadRot, 1.0f - fHeadRotTime);

					yield return new WaitForEndOfFrame();
				}
			}
		}

        //-----
        _procCompleted = true;
        _vtProc = null;
    }

    //---------------------------------------
    private void OnSeedSpreadAnim()
    {
        int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;
        Rigidbody pSeed = Instantiate(_seedSprd_SeedInstance[nDiff]);
        pSeed.transform.position = _seedSprd_budPosDummy.transform.position;

        Vector3 vVector = Vector3.forward;
        vVector.y = UnityEngine.Random.Range(_seedSprd_YVecRange.x, _seedSprd_YVecRange.y);
        vVector.Normalize();

        vVector = Quaternion.Euler(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f) * vVector;
        float fSpeed = UnityEngine.Random.Range(_seedSprd_SpeedRange.x, _seedSprd_SpeedRange.y);

        pSeed.AddForce(vVector * fSpeed, ForceMode.VelocityChange);

        //-----
        --_seedSprd_LeastCount;
        if (_seedSprd_LeastCount <= 0)
            _budAnimation.Play(_seedSprd_BudAnim_Idle);
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

    //---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------