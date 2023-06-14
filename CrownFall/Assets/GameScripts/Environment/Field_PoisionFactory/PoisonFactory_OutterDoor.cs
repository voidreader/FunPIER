using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HT;

/////////////////////////////////////////
//---------------------------------------
public class PoisonFactory_OutterDoor : MonoBehaviour
{
	//---------------------------------------
	[Header("DOOR SETTING")]
	[SerializeField]
	private Animation _doorAnimator = null;
	[SerializeField]
	private string _doorAnim_Open = null;
	[SerializeField]
	private string _doorAnim_Close = null;
	[SerializeField]
	private string _doorAnim_IDLE = null;
	[SerializeField]
	private ParticleSystem _doorOpenParticles = null;

	[Header("DOOR STATE")]
	[SerializeField]
	private bool _isOpened = false;

	[Header("POISON SPRINKLER")]
	[SerializeField]
	private ParticleSystem[] _sprinklerEffects = null;
	[SerializeField]
	private PoisonFactory_Poison _sprinklerPoison = null;
	[SerializeField]
	private Transform _alertPosition = null;

	//---------------------------------------
	private float _openTimeRemain = 0.0f;

	public bool IsOpened { get { return _isOpened; } }

	//---------------------------------------
	private void Awake()
	{

	}

	private void Update()
	{
		if (_isOpened)
		{
			if (_openTimeRemain > 0.0f)
				_openTimeRemain -= HT.TimeUtils.GameTime;

			if (_openTimeRemain <= 0.0f)
			{
				_isOpened = false;
				EnableSprinklerEffect(false);
				_doorAnimator.Play(_doorAnim_Close);
			}
		}
	}

	//---------------------------------------
	public void DoorOpen(float fTime)
	{
		float fPartcleTime = _doorOpenParticles.TotalSimulationTime();
		_openTimeRemain = fTime + fPartcleTime;

		if (_isOpened)
			return;

		_isOpened = true;
		_doorOpenParticles.Play(true);

		Invoke("DoorOpen_Internal", fPartcleTime);
	}

	private void DoorOpen_Internal()
	{
		_doorAnimator.Play(_doorAnim_Open);

		AnimationClip pClip = _doorAnimator.GetClip(_doorAnim_Open);
		Invoke("OnDoorOpenAnimComplete", pClip.length);

		Object_AreaAlert pAlertObj = BattleFramework._Instance.CreateAreaAlert(_alertPosition.position, _sprinklerPoison.GetRadius(), _openTimeRemain);
		pAlertObj.transform.position = _alertPosition.position;
		pAlertObj.transform.rotation = _alertPosition.rotation;
	}

	//---------------------------------------
	private void OnDoorOpenAnimComplete()
	{
		EnableSprinklerEffect(true);
	}

	private void EnableSprinklerEffect(bool bSet)
	{
		_sprinklerPoison.SetEnable(bSet);
	}

	//---------------------------------------
	public void OnAnimEvent_StartIDLEAnim()
	{
		_doorAnimator.Play(_doorAnim_IDLE);
	}

	public void OnAnimEvent_SpreadPoison()
	{
		for (int nInd = 0; nInd < _sprinklerEffects.Length; ++nInd)
			_sprinklerEffects[nInd].Play(true);
	}

	public void OnAnimEvent_PlayMisc(AudioClip pClip)
	{
		if (pClip != null)
			HTSoundManager.PlaySound(pClip);
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------
