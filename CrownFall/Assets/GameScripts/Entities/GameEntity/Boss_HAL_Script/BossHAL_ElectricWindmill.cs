using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class BossHAL_ElectricWindmill : MonoBehaviour
{
	//---------------------------------------
	[Header("OBJECTS")]
	[SerializeField]
	private GameObject _root = null;
	[SerializeField]
	private ISkillObject _skillObject = null;
	[SerializeField]
	private HT.HTLightning _lightning = null;
	[SerializeField]
	private GameObject _objectA = null;
	[SerializeField]
	private GameObject _objectB = null;

	//---------------------------------------
	[Header("ANIMATIONS")]
	[SerializeField]
	private Animation _animation = null;
	[SerializeField]
	private string _startAnim = null;
	[SerializeField]
	private string _endAnim = null;

	//---------------------------------------
	[Header("ROTATE INFO")]
	[SerializeField]
	private float _rotatePerSec = 0.0f;

	[Header("LIGHTNING INFO")]
	[SerializeField]
	private float _lightning_Repeat = 0.25f;
	[SerializeField]
	private float _lightning_Time = 0.2f;
	[SerializeField]
	private float _lightning_Delay = 0.1f;
	[SerializeField]
	private float _lightning_Width = 0.5f;
	[SerializeField]
	private float _lightning_Size = 0.25f;
	[SerializeField]
	private int _lightning_Step = 5;

	//---------------------------------------
	private bool _rotate_Positive = true;

	//---------------------------------------
	public void Init(Vector3 pos, float fLifeTime)
	{
		_skillObject.m_pCaster = BattleFramework._Instance.m_pEnemyActor;

		gameObject.transform.position = pos;
		gameObject.transform.Rotate(0.0f, Random.Range(0.0f, 360.0f), 0.0f);

		_rotate_Positive = (Random.Range(0.0f, 1.0f) < 0.5f) ? true : false;
		StartCoroutine(Proc_Internal(fLifeTime));
	}

	private IEnumerator Proc_Internal(float fLifeTime)
	{
		_lightning.SetLightningInfo(_lightning_Width, _lightning_Size, _lightning_Step, false);

		//-----
		AnimationClip pStartAnim = _animation.GetClip(_startAnim);
		float fAnimTime = pStartAnim.length + 0.1f;
		BattleFramework._Instance.CreateAreaAlert(_root.transform.position, 0.5f, fAnimTime);

		_animation.Play(_startAnim);
		yield return new WaitForSeconds(fAnimTime);

		//-----
		float fLightningTime = 0.0f;
		while(fLifeTime >= 0.0f)
		{
			float fDelta = HT.TimeUtils.GameTime;

			float fRotateDelta = (fDelta * _rotatePerSec) * ((_rotate_Positive) ? 1.0f : -1.0f);
			_root.transform.Rotate(0.0f, fRotateDelta, 0.0f);
			fLifeTime -= fDelta;

			if (fLightningTime > 0.0f)
				fLightningTime -= fDelta;

			else
			{
				fLightningTime = _lightning_Repeat;
				_lightning.Init(_objectA, _objectB, _lightning_Time, _lightning_Delay);
			}

			yield return new WaitForEndOfFrame();
		}

		//-----
		AnimationClip pEndAnim = _animation.GetClip(_endAnim);
		fAnimTime = pEndAnim.length + 0.1f;

		_animation.Play(_endAnim);
		HT.Utils.SafeDestroy(gameObject, fAnimTime);
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------