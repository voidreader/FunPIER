using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class BossHAL_Beam : MonoBehaviour
{
	//---------------------------------------
	[Header("BEAM OBJECTS")]
	[SerializeField]
	private Animation _anim_Beam = null;
	[SerializeField]
	private string _anim_Beam_Name = null;
	[SerializeField]
	private Animation _anim_BeamTarget = null;
	[SerializeField]
	private string _anim_BeamTarget_Name = null;
	[SerializeField]
	private AudioClip _readySound = null;
	[SerializeField]
	private AudioClip[] _strikeSound = null;

	[Header("SETTINGS")]
	[SerializeField]
	private int _damage = 1;
	[SerializeField]
	private float _damageRadius = 8.0f;
	[SerializeField]
	private float _cameraShake = 2.0f;

	[Header("ARCHIVEMENT")]
	public string _archiveWhenHit = null;

	//---------------------------------------
	void Start()
	{
		HT.HTSoundManager.PlaySound(_readySound);

		float fTime = _anim_BeamTarget.GetClip(_anim_BeamTarget_Name).length;
		_anim_BeamTarget.Play(_anim_BeamTarget_Name);

		BattleFramework._Instance.CreateAreaAlert(transform.position, _damageRadius, fTime);

		Invoke("Strike", fTime);
	}

	//---------------------------------------
	void Strike()
	{
		if (_strikeSound != null && _strikeSound.Length > 0)
		{
			for (int nInd = 0; nInd < _strikeSound.Length; ++nInd)
				HT.HTSoundManager.PlaySound(_strikeSound[nInd]);
		}

		if (_cameraShake > 0.0f)
			CameraManager._Instance.SetCameraShake(_cameraShake);

		Vector3 vPlayer = BattleFramework._Instance.m_pPlayerActor.transform.position;
		if (Vector3.Distance(vPlayer, gameObject.transform.position) <= _damageRadius)
		{
			BattleFramework._Instance.m_pPlayerActor.OnDamaged(_damage);

			if (string.IsNullOrEmpty(_archiveWhenHit) == false)
			{
				Archives pArchives = ArchivementManager.Instance.FindArchive(_archiveWhenHit);
				pArchives.Archive.OnArchiveCount(1);
			}
		}

		float fTime = _anim_Beam.GetClip(_anim_Beam_Name).length;
		_anim_Beam.Play(_anim_Beam_Name);

		HT.Utils.SafeDestroy(gameObject, fTime + 0.1f);
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------