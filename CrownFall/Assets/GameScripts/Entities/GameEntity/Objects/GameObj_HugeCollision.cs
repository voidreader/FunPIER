using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class GameObj_HugeCollision : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private float _playEffectSpeed = 1.0f;
	[SerializeField]
	private AudioClip _effectSound = null;
	[SerializeField]
	private float _effectCamShake = 0.0f;
	[SerializeField]
	private ParticleSystem _effectParticle = null;

	//---------------------------------------
	private void Start()
	{
	}

	//---------------------------------------
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.GetComponent<HT.MousePickableObject>() == null)
			return;

		if (collision.relativeVelocity.magnitude > _playEffectSpeed)
		{
			HT.HTSoundManager.PlaySound(_effectSound);

			if (_effectCamShake > 0.0f)
				CameraManager._Instance.SetCameraShake(_effectCamShake);

			if (_effectParticle != null)
			{
				ParticleSystem pParticle = HT.Utils.InstantiateFromPool(_effectParticle);
				pParticle.transform.position = collision.transform.position;

				HT.Utils.SafeDestroy(pParticle.gameObject, pParticle.TotalSimulationTime());
			}
		}
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------