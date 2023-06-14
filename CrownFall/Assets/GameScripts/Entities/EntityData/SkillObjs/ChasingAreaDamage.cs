using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public sealed class ChasingAreaDamage : ISkillObject
{
	//---------------------------------------
	[Header("DAMAGE INFO")]
	[SerializeField]
	private int _damage = 1;
	[SerializeField]
	private float _range = 1.0f;
	[SerializeField]
	private float _chasingSpeed = 1.0f;
	[SerializeField]
	private float _chasingTime = 1.0f;
	[SerializeField]
	private float _chasingDistMin = 0.25f;
	[SerializeField]
	private float _damageTime = 1.0f;

	//---------------------------------------
	[Header("EFFECT INFO")]
	[SerializeField]
	private Animation _animation = null;
	[SerializeField]
	private string _anim_WhenDamage = null;

	//---------------------------------------
	private Object_AreaAlert _alertObject = null;
	private float _leastTime = 0.0f;
	private IActorBase _targetActor = null;

	//---------------------------------------
	private void OnEnable()
	{
		_leastTime = _chasingTime;

		//-----
		_alertObject = BattleFramework._Instance.CreateAreaAlert(gameObject.transform.position, _range, _chasingTime);

		//-----
		_targetActor = BattleFramework._Instance.m_pPlayerActor;

		//-----
		StartCoroutine(SkillObject_Internal());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	//---------------------------------------
	private IEnumerator SkillObject_Internal()
	{
		float fSecondDelay = _damageTime - _leastTime;
		while (_leastTime > 0.0f)
		{
			_leastTime -= HT.TimeUtils.GameTime;
			
			if (Vector3.Distance(gameObject.transform.position, _targetActor.transform.position) > _chasingDistMin)
			{
				Vector3 vCurPos = gameObject.transform.position;
				Vector3 vMoveDir = _targetActor.transform.position - vCurPos;
				vMoveDir.Normalize();

				vCurPos = vCurPos + (vMoveDir * (_chasingSpeed * HT.TimeUtils.GameTime));

				_alertObject.transform.position = vCurPos;
				gameObject.transform.position = vCurPos;
			}

			yield return new WaitForEndOfFrame();
		}

		//-----
		if (fSecondDelay > 0.0f)
			yield return new WaitForSeconds(fSecondDelay);

		//-----
		float fDist = Vector3.Distance(_targetActor.transform.position, gameObject.transform.position);
		if (fDist <= _range)
		{
			_targetActor.OnDamaged(_damage);
			if (string.IsNullOrEmpty(_archiveWhenHit) == false)
			{
				Archives pArchives = ArchivementManager.Instance.FindArchive(_archiveWhenHit);
				pArchives.Archive.OnArchiveCount(1);
			}
		}

		if (string.IsNullOrEmpty(_anim_WhenDamage) == false)
		{
			AnimationClip pClip = _animation.GetClip(_anim_WhenDamage);
			_animation.Play(_anim_WhenDamage);

			yield return new WaitForSeconds(pClip.length);
		}

		//-----
		HT.Utils.SafeDestroy(gameObject);
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------