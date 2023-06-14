using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class BossVT_FlowerBud_Extend : MonoBehaviour
{
	//---------------------------------------
	[Header("DEFAULT INFO")]
	[SerializeField]
	BossVT_FlowerBud.eBudType _budConditionType = BossVT_FlowerBud.eBudType.MAX;
	[SerializeField]
	private SkinnedMeshRenderer _meshRenderer = null;
	[SerializeField]
	private int _flowerColorMatIndex = 0;
	[SerializeField]
	private Animation _branchAnimation = null;
	[SerializeField]
	private string _branchAnim_Bloom = null;

	[Header("BUD INFO")]
	[SerializeField]
	private Animation _budAnimation = null;
	[SerializeField]
	private string _budAnim_Close = null;
	[SerializeField]
	private string _budAnim_Open = null;
	[SerializeField]
	private string _budAnim_Shoot = null;

	[Header("SKILLS")]
	[SerializeField]
	private BossVT_FlowerBudSkillInfo _skillInfo = null;
	[SerializeField]
	private GameObject _dumyy_FlowerStart = null;
	[SerializeField]
	private GameObject _dumyy_FlowerEnd = null;

	//---------------------------------------
	[Header("STATE")]
	[SerializeField]
	float _leastTime = 0.0f;
	[SerializeField]
	private BossVT_FlowerBud.eBudType _budType = BossVT_FlowerBud.eBudType.MAX;
	[SerializeField]
	private bool _isExtended = false;

	//---------------------------------------
	public bool IsExtended { get { return _isExtended; } }

	public BossVT_FlowerBud.eBudType BudConditionType { get { return _budConditionType; } }

	//---------------------------------------
	private void Update()
	{
		if (BattleFramework._Instance == null || BattleFramework._Instance.m_eBattleState != BattleFramework.eBattleState.eBattle)
			return;

		if (_budType == BossVT_FlowerBud.eBudType.MAX)
			return;

		if (_leastTime > 0.0f)
			_leastTime -= Time.deltaTime;

		else
		{
			_leastTime = _skillInfo.GetRepeatTime(_budType);
			switch(_budType)
			{
				case BossVT_FlowerBud.eBudType.FlameWall:
					StartCoroutine(CastSkill_Firewall());
					break;
					
				case BossVT_FlowerBud.eBudType.Freezy:
					StartCoroutine(CastSkill_Freeze());
					break;
					
				case BossVT_FlowerBud.eBudType.Thorns:
					StartCoroutine(CastSkill_Throns());
					break;

				case BossVT_FlowerBud.eBudType.Frenzy:
					StartCoroutine(CastSkill_Frenzy());
					break;
			}
		}
	}

	//---------------------------------------
	private IEnumerator CastSkill_Firewall()
	{
		AnimationClip pClip = _budAnimation.GetClip(_budAnim_Shoot);
		_budAnimation.Play(_budAnim_Shoot);

		yield return new WaitForSeconds(pClip.length);

		//-----
		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;

		//-----
		BossVT_FlamewallTarget pTarget = HT.Utils.InstantiateFromPool(_skillInfo._flamewall_Target[nDiff]);
		Vector3 vCenterPos = Vector3.zero;
		vCenterPos.x = HT.RandomUtils.Range(_skillInfo._flamewall_RangeMin.x, _skillInfo._flamewall_RangeMax.x);
		vCenterPos.z = HT.RandomUtils.Range(_skillInfo._flamewall_RangeMin.z, _skillInfo._flamewall_RangeMax.z);
		pTarget.transform.position = vCenterPos;

		Vector3 vForward = (Vector3.zero - vCenterPos).normalized;
		pTarget.transform.forward = vForward;

		float fXScale = pTarget._collider.size.x * 0.5f;
		float fZScale = pTarget._collider.size.z * 0.5f;
		Object_AreaAlert pAlert = BattleFramework._Instance.CreateAreaAlert(vCenterPos, fXScale, fZScale, 1.0f);
		pAlert.transform.forward = vForward;

		HT.HTSoundManager.PlaySound(_skillInfo._flamewall_Sounds);

		if (_skillInfo._flamewall_ShootEffect != null)
		{
			ParticleSystem pEffect = HT.Utils.InstantiateFromPool(_skillInfo._flamewall_ShootEffect);
			pEffect.transform.position = _dumyy_FlowerEnd.transform.position;
			pEffect.transform.up = _dumyy_FlowerEnd.transform.forward;

			HT.Utils.SafeDestroy(pEffect.gameObject, pEffect.TotalSimulationTime());
		}

		//-----
		for (int nInd = 0; nInd < pTarget._targets.Length; ++nInd)
		{
			Projectile_Parabola pProj = HT.Utils.Instantiate(_skillInfo._flamewall_Projectile[nDiff]);

			pProj.m_pSkill_Proj = null;
			pProj.m_pParent = BattleFramework._Instance.m_pEnemyActor;

			pProj._damage = 1;
			pProj._speed = _skillInfo._flamewall_Speed;
			pProj._flyHeight = _skillInfo._flamewall_Height;

			GameObject pObj = pTarget._targets[nInd];
			pProj.Init(_dumyy_FlowerEnd.transform.position, pObj.transform.position);
		}
	}

	//---------------------------------------
	private IEnumerator CastSkill_Freeze()
	{
		AnimationClip pClip = _budAnimation.GetClip(_budAnim_Shoot);
		_budAnimation.Play(_budAnim_Shoot);

		yield return new WaitForSeconds(pClip.length);

		//-----
		eGameDifficulty eDiff = GameFramework._Instance.m_pPlayerData.m_eDifficulty;
		Projectile_Parabola pProj = HT.Utils.Instantiate(_skillInfo._freeze_Projectile[(int)eDiff]);

		pProj.m_pSkill_Proj = null;
		pProj.m_pParent = BattleFramework._Instance.m_pEnemyActor;

		pProj._damage = 0;
		pProj._speed = _skillInfo._freeze_Speed;
		pProj._flyHeight = _skillInfo._freeze_Height;

		//-----
		Vector3 vCenterPos = BattleFramework._Instance.m_pPlayerActor.transform.position; //Vector3.zero;
		vCenterPos.y = 0.0f;

		//vCenterPos.x = HT.RandomUtils.Range(_skillInfo._flamewall_RangeMin.x, _skillInfo._flamewall_RangeMax.x);
		//vCenterPos.z = HT.RandomUtils.Range(_skillInfo._flamewall_RangeMin.z, _skillInfo._flamewall_RangeMax.z);
		pProj.Init(_dumyy_FlowerEnd.transform.position, vCenterPos);
	}

	//---------------------------------------
	private IEnumerator CastSkill_Throns()
	{
		AnimationClip pClip = _budAnimation.GetClip(_budAnim_Open);
		_budAnimation.Play(_budAnim_Open);

		yield return new WaitForSeconds(pClip.length);

		//-----
		eGameDifficulty eDiff = GameFramework._Instance.m_pPlayerData.m_eDifficulty;
		int nCount = _skillInfo._thorn_Count[(int)eDiff];

		Vector3 vStartPos = _dumyy_FlowerStart.transform.position;
		float fStartRotate = RandomUtils.Range(0.0f, 360.0f);
		float fRotate = 360.0f / nCount;
		for (int nInd = 0; nInd < nCount; ++nInd)
		{
			Projectile_Trigger pThron = HT.Utils.Instantiate(_skillInfo._thorn_Instance);

			pThron.m_pParent = BattleFramework._Instance.m_pEnemyActor;
			pThron.m_nProjectileDamage = _skillInfo._thorn_Damage;
            pThron.m_fSpeed = _skillInfo._thorn_Speed[(int)eDiff] * 60.0f;

			pThron.m_vMoveVector = Quaternion.Euler(0.0f, fStartRotate + (fRotate * nInd), 0.0f) * Vector3.forward;
			pThron.UpdateRotate();

			pThron.AddIgnorePhysList(_skillInfo._ignoreList);
			pThron.Init(_dumyy_FlowerStart.transform.position, 10.0f);
		}

		//-----
		_budAnimation.Play(_budAnim_Close);
	}

	//---------------------------------------
	private IEnumerator CastSkill_Frenzy()
	{
		AnimationClip pClip = _budAnimation.GetClip(_budAnim_Shoot);
		_budAnimation.Play(_budAnim_Shoot);

		yield return new WaitForSeconds(pClip.length);

		//-----
		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;
		for (int nInd = 0; nInd < _skillInfo._frenzy_Count[nDiff]; ++nInd)
		{
			Projectile_Parabola pProj = HT.Utils.Instantiate(_skillInfo._frenze_Projectile[nDiff]);

			pProj.m_pSkill_Proj = null;
			pProj.m_pParent = BattleFramework._Instance.m_pEnemyActor;

			pProj._damage = 0;
			pProj._speed = _skillInfo._frenzy_Speed;
			pProj._flyHeight = _skillInfo._frenzy_Height;

			Vector3 vCenterPos = Vector3.zero;
			vCenterPos.x = HT.RandomUtils.Range(_skillInfo._flamewall_RangeMin.x, _skillInfo._flamewall_RangeMax.x);
			vCenterPos.z = HT.RandomUtils.Range(_skillInfo._flamewall_RangeMin.z, _skillInfo._flamewall_RangeMax.z);
			pProj.Init(_dumyy_FlowerEnd.transform.position, vCenterPos);
		}
	}

	//---------------------------------------
	public void SetExtend(BossVT_FlowerBud pBud)
	{
		_isExtended = true;

		_budType = pBud.BudType;

		Material[] vMats = _meshRenderer.materials;
		vMats[_flowerColorMatIndex] = pBud.ColorMateial;
		_meshRenderer.materials = vMats;

		_branchAnimation.Play(_branchAnim_Bloom);

		//-----
		_leastTime = _skillInfo.GetRepeatTime(_budType);
	}

	//---------------------------------------
	public void PlaySound(AudioClip pSound)
	{
		if (pSound == null)
			return;

		HT.HTSoundManager.PlaySound(pSound);
	}
}


/////////////////////////////////////////
//---------------------------------------