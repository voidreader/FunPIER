using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class Field_Hall_WallHand : MonoBehaviour
{
	//---------------------------------------
	[Header("HAND SETTING")]
	[SerializeField]
	private Animation _animation = null;
	[SerializeField]
	private string _anim_IDLE = null;
	[SerializeField]
	private string _anim_Attack_Ready = null;
	[SerializeField]
	private string _anim_Attack_Cast = null;
	[SerializeField]
	private AudioClip _anim_Attack_Sound = null;

	//---------------------------------------
	[Header("HAND ATTACK INFO")]
	[SerializeField]
	private float _readyTime_Min = 1.0f;
	[SerializeField]
	private float[] _readyTime_Max = null;
	[SerializeField]
	private Field_Hall_WallHand_Trigger _damagePivot = null;
	//[SerializeField]
	//private float _damageRange = 1.2f;
	[SerializeField]
	private int _damage = 1;

    //---------------------------------------
    private Bounds _aabbBound;

    //---------------------------------------
    private float _attackDelay_LeastTime = 0.0f;

	//---------------------------------------
	private void OnEnable()
	{
		StartCoroutine(HandProc_Internal());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	//---------------------------------------
	private IEnumerator HandProc_Internal()
	{
		AnimationClip pReadyClip = _animation.GetClip(_anim_Attack_Ready);
		AnimationClip pCastClip = _animation.GetClip(_anim_Attack_Cast);

		//IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;

		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;
		while (true)
		{
			_animation.Play(_anim_IDLE);
			_attackDelay_LeastTime = HT.RandomUtils.Range(_readyTime_Min, _readyTime_Max[nDiff]);

			while(_attackDelay_LeastTime > 0.0f)
			{
				_attackDelay_LeastTime -= HT.TimeUtils.GameTime;
				yield return new WaitForEndOfFrame();
			}

			//-----
			Object_AreaAlert pAlert = BattleFramework._Instance.CreateAreaAlert(_damagePivot.transform.position, _damagePivot.AreaX, _damagePivot.AreaZ, pReadyClip.length);
			pAlert.transform.forward = _damagePivot.transform.forward;

			_animation.Play(_anim_Attack_Ready);
			yield return new WaitForSeconds(pReadyClip.length);

			//-----
			//float fDistance = Vector3.Distance(pPlayer.transform.position, _damagePivot.transform.position);
			//if (fDistance <= _damageRange)
			//	pPlayer.OnDamaged(_damage);

			_damagePivot.OnDamage(_damage);

			HT.HTSoundManager.PlaySound(_anim_Attack_Sound);

			_animation.Play(_anim_Attack_Cast);
			yield return new WaitForSeconds(pCastClip.length);
		}
	}
}


/////////////////////////////////////////
//---------------------------------------