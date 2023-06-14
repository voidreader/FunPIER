using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class BossVT_TwistVine : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private float _speed = 5.0f;
	[SerializeField]
	ParticleSystem _particleEffect = null;
	[SerializeField]
	private Animation _animation = null;
	[SerializeField]
	private string _animName_Start = null;
	[SerializeField]
	private string _animName_End = null;
	[SerializeField]
	private AudioClip _sounds = null;
	[SerializeField]
	private float _rotationTime = 0.5f;

	//---------------------------------------
	[SerializeField]
	private GameObject[] _pivots = null;

	//---------------------------------------
	public void Init(GameObject[] pPivots)
	{
		_pivots = pPivots;
		StartCoroutine(ProcessInternal());
	}

	private void PlaySound()
	{
		HT.HTSoundManager.PlaySound(_sounds);
	}

	//---------------------------------------
	private IEnumerator ProcessInternal()
	{
		HT.HTSoundManager.PlaySound(_sounds);
		Invoke("PlaySound", 10.0f);

		//-----
		transform.position = _pivots[0].transform.position;
		transform.forward = (_pivots[1].transform.position - _pivots[0].transform.position).normalized;

		_particleEffect.Play(true);
		_animation.Play(_animName_Start);

		//-----
		int nCurIndex = 1;
		while(true)
		{
			Vector3 vPos = transform.position;
			vPos = Vector3.MoveTowards(vPos, _pivots[nCurIndex].transform.position, _speed * HT.TimeUtils.GameTime);
			transform.position = vPos;

			if (Vector3.Distance(vPos, _pivots[nCurIndex].transform.position) < 0.1f)
			{
				if (nCurIndex >= _pivots.Length - 1)
					break;

				Vector3 vCurFoward = transform.forward;
				Vector3 vTargetForward = (_pivots[nCurIndex + 1].transform.position - _pivots[nCurIndex].transform.position).normalized;

				float fRotation = _rotationTime;
				while(fRotation >= 0.0f)
				{
					fRotation -= HT.TimeUtils.GameTime;
					transform.forward = Vector3.Lerp(vCurFoward, vTargetForward, 1.0f - (fRotation / _rotationTime));

					yield return new WaitForEndOfFrame();
				}

				++nCurIndex;
			}

			yield return new WaitForEndOfFrame();
		}

		//-----
		_particleEffect.Stop();
		_animation.Play(_animName_End);

		AnimationClip pEndClip = _animation.GetClip(_animName_End);
		HT.Utils.SafeDestroy(gameObject, pEndClip.length);
	}
}


/////////////////////////////////////////
//---------------------------------------