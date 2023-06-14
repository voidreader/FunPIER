using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public sealed class AIActor_Extend_PD : AIActor_Extend
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("MAIN SETTINGS")]
	public GameObject _syringe_Handle = null;
	public Vector3 _syringe_Handle_PosZero = Vector3.zero;
	public Vector3 _syringe_Handle_PosOne = Vector3.zero;

	public GameObject _syringe_Guage = null;
	public Vector3 _syringe_Gauge_ScaleZero = Vector3.zero;
	public Vector3 _syringe_Gauge_ScaleOne = Vector3.zero;

	public float _syringe_Charged = 1.0f;
	public float _syringe_ConsumeTime = 0.3f;
	public float _syringe_ChargeTime = 10.0f;


	/////////////////////////////////////////
	//---------------------------------------
	[Header("OPEN VALVE")]
	[SerializeField]
	private float _openValve_FirstDelay = 10.0f;
	[SerializeField]
	private float[] _openValve_RepeatTime = null;
	[SerializeField]
	private string _openValve_StartAnimation = null;
	[SerializeField]
	private string _openValve_EndAnimation = null;
	[SerializeField]
	private float _openValve_SkillTime = 45.0f;
	[SerializeField]
	private float _openValve_PipeOpenDelay = 13.0f;
	[SerializeField]
	private ActorBuff _openValve_WaitSummonSlime = null;


	//---------------------------------------
	private Coroutine _syringeProc = null;


	/////////////////////////////////////////
	//---------------------------------------
	public override void Extended_Init()
	{

	}
	
	public override void Extended_PostInit()
	{

	}

	public override void Extended_Frame()
	{
		if (_syringeProc == null)
		{
			float fChargeRatio = 1.0f / _syringe_ChargeTime;
			_syringe_Charged += HT.TimeUtils.GameTime * fChargeRatio;
		}

		_syringe_Handle.transform.localPosition = Vector3.Lerp(_syringe_Handle_PosZero, _syringe_Handle_PosOne, _syringe_Charged);
		_syringe_Guage.transform.localScale = Vector3.Lerp(_syringe_Gauge_ScaleZero, _syringe_Gauge_ScaleOne, _syringe_Charged);
	}

	public override void Extended_Release()
	{
	}


	/////////////////////////////////////////
	//---------------------------------------
	public override void OnBattleStart()
	{
		base.OnBattleStart();
		StartCoroutine(OpenValve_Internal());
	}

	public override void OnBattleEnd(bool bPlayerWin)
	{
		base.OnBattleEnd(bPlayerWin);
		StopAllCoroutines();
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void Syringe_GaugeZero()
	{
		if (_syringeProc != null)
			StopCoroutine(_syringeProc);

		_syringeProc = StartCoroutine(Syringe_GaugeZero_Internal());
	}

	private IEnumerator Syringe_GaugeZero_Internal()
	{
		float fConsumeRatio = 1.0f / _syringe_ConsumeTime;
		while (_syringe_Charged > 0.0f)
		{
			_syringe_Charged -= HT.TimeUtils.GameTime * fConsumeRatio;
			if (_syringe_Charged < 0.0f)
				_syringe_Charged = 0.0f;

			yield return new WaitForEndOfFrame();
		}

		_syringeProc = null;
	}


	/////////////////////////////////////////
	//---------------------------------------
	private IEnumerator OpenValve_Internal()
	{
		yield return new WaitForSeconds(_openValve_FirstDelay + 10.0f);

		//-----
		Field_PoisonFactory pField = BattleFramework._Instance.m_pField as Field_PoisonFactory;
		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;

		//-----
		while (true)
		{
			int nLastPipeIndex = -1;
			bool bSkillInitialized = false;

			//-----
			float fSkillTime = 0.0f;
			while (true)
			{
				float fDelay = 0.1f;
				while (true)
				{
					if (m_pActorBase.GetActorState() != IActorBase.eActorState.eAction)
					{
						fDelay -= HT.TimeUtils.GameTime;
						if (fDelay <= 0.0f)
							break;
					}
					else
						fDelay = 0.1f;

					fSkillTime -= HT.TimeUtils.GameTime;
					yield return new WaitForEndOfFrame();
				}

				//-----
				if (bSkillInitialized == false)
				{
					bSkillInitialized = true;
					fSkillTime = _openValve_SkillTime;
				}

				if (fSkillTime <= 0.0f)
					break;

				//-----
				if (pField.PoisonFillRatio < 1.0f)
				{
					float fAnimTime = m_pActorBase.SetAction(_openValve_StartAnimation);
					m_pActorBase.SetActionReadyTime(float.MaxValue);

					fSkillTime -= fAnimTime;
					yield return new WaitForSeconds(fAnimTime);

					//-----
					m_pActorBase.SetAction(_openValve_EndAnimation);
					pField.CloseAllPipe();

					int nPipeCount = pField.GetPipeCount();
					for (;;)
					{
						int nPipeIndex = HT.RandomUtils.Range(0, nPipeCount);
						if (nPipeIndex != nLastPipeIndex)
						{
							nLastPipeIndex = nPipeIndex;
							break;
						}
					}

					pField.SetOnPipesOpen(nLastPipeIndex);
				}

				//-----
				if (nLastPipeIndex < 0 || nLastPipeIndex >= pField.GetPipeCount())
					nLastPipeIndex = 0;

				float fWaitTime = _openValve_PipeOpenDelay;
				while(fWaitTime > 0.0f)
				{
					if (pField.IsPipeOpened(nLastPipeIndex))
						pField.PoisonFillRatio += GameDefine.fBoss_PG_CenterPoison_IncRatio * HT.TimeUtils.GameTime;

					//-----
					fWaitTime -= HT.TimeUtils.GameTime;
					fSkillTime -= HT.TimeUtils.GameTime;

					if (fSkillTime <= 0.0f)
						break;

					yield return new WaitForEndOfFrame();
				}
			}

			//-----
			pField.CloseAllPipe();

			//-----
			m_pActorBase.AddActorBuff(_openValve_WaitSummonSlime);

			while (m_pActorBase.FindEnabledActorBuff(_openValve_WaitSummonSlime) != null)
				yield return new WaitForEndOfFrame();

			//-----
			while(pField.PoisonFillRatio > 0.0f)
			{
				pField.PoisonFillRatio -= GameDefine.fBoss_PG_CenterPoison_DecRatio * HT.TimeUtils.GameTime;
				yield return new WaitForEndOfFrame();
			}

			yield return new WaitForSeconds(_openValve_RepeatTime[nDiff]);
		}
	}
}


/////////////////////////////////////////
//---------------------------------------