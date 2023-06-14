using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class BossBC_BlackArrow : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private ParticleSystem _createEffect = null;
	[SerializeField]
	private ParticleSystem _destroyEffect = null;
	[SerializeField]
	private AudioClip _shootSound = null;
	[SerializeField]
	private GameObject _root = null;
	[SerializeField]
	private float _heightChangeRange = 1.0f;

	[Header("PROJECTILE INFO")]
	[SerializeField]
	private Projectile_Trigger _projecile = null;
	[SerializeField]
	private float _projectileSize = 0.5f;
	[SerializeField]
	private float _shootHeight = 0.5f;
	[SerializeField]
	private float _proj_Speed = 8.0f;

	//---------------------------------------
	private float _curSin = 0.0f;

	//---------------------------------------
	private void OnEnable()
	{
		if (_createEffect != null)
		{
			ParticleSystem pEffect = HT.Utils.InstantiateFromPool(_createEffect);
			pEffect.transform.position = gameObject.transform.position;
			pEffect.transform.forward = gameObject.transform.forward;

			HT.Utils.SafeDestroy(pEffect.gameObject, pEffect.TotalSimulationTime());
		}
	}

	private void Update()
	{
		_curSin += HT.TimeUtils.GameTime;
		_root.transform.localPosition = Vector3.up * ((Mathf.Sin(_curSin) * _heightChangeRange) - (_heightChangeRange * 0.5f));

		//-----
		Vector3 vScale = _root.transform.localScale;
		vScale.y = vScale.x;
		_root.transform.localScale = vScale;
	}

	//---------------------------------------
	public void CastSkill()
	{
		StartCoroutine(CastSkill_Internal());
	}

	private IEnumerator CastSkill_Internal()
	{
		Vector3 vPlayerPos = BattleFramework._Instance.m_pPlayerActor.transform.position;
		Vector3 vCurPos = gameObject.transform.position;

		Vector3 vDir = (vPlayerPos - vCurPos).normalized;
		vDir.y = 0.0f;
		vDir.Normalize();

		float fWaitTime = 1.0f;

		Vector3 vAreaStartPos = vCurPos;
		vAreaStartPos.y = 0.0f;

		Object_AreaAlert pAlert = BattleFramework._Instance.CreateAreaAlert_Simple(vAreaStartPos, _projectileSize, 50.0f, fWaitTime);
		pAlert.transform.forward = vDir;

		//-----
		float fCurY = vCurPos.y;
		float fTime = fWaitTime;
		while(fTime > 0.0f)
		{
			fTime -= HT.TimeUtils.GameTime;
			vCurPos.y = Mathf.Lerp(fCurY, _shootHeight, 1.0f - (fTime / fWaitTime));

			gameObject.transform.position = vCurPos;

			yield return new WaitForEndOfFrame();
		}

		//-----
		if (_destroyEffect != null)
		{
			ParticleSystem pEffect = HT.Utils.InstantiateFromPool(_destroyEffect);
			pEffect.transform.position = gameObject.transform.position;
			pEffect.transform.forward = gameObject.transform.forward;

			HT.Utils.SafeDestroy(pEffect.gameObject, pEffect.TotalSimulationTime());
		}

		if (_shootSound != null)
			HT.HTSoundManager.PlaySound(_shootSound);

		//-----
		Projectile_Trigger pProj = HT.Utils.Instantiate(_projecile);
		pProj.m_pParent = BattleFramework._Instance.m_pEnemyActor;

		pProj.AddIgnorePhysList(pProj.m_pParent.gameObject);

		pProj.m_vMoveVector = vDir;
		pProj.m_fSpeed = _proj_Speed;
		pProj.m_nProjectileDamage = 1;

		pProj.Init(vCurPos, 5.0f);

		//-----
		HT.Utils.SafeDestroy(gameObject);
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------