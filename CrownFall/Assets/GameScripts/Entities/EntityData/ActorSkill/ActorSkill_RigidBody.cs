using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public sealed class ActorSkill_RigidBody : ActorSkill
{
	//---------------------------------------
	[Header("SKILL SET")]
	[SerializeField]
	private Rigidbody _rigidBody = null;
	[SerializeField]
	private float _spreadXDegree = 0.0f;
	[SerializeField]
	private float _minSpeed = 0.0f;
	[SerializeField]
	private float _maxSpeed = 0.0f;
	[SerializeField]
	private float _posOffsetDistance = 1.0f;
	[SerializeField]
	private ForceMode _forceMode = ForceMode.VelocityChange;

	//---------------------------------------


	/////////////////////////////////////////
	//---------------------------------------
	public override bool SkillCastReady_Child()
	{
		return true;
	}

	public override void SkillThrow_Child()
	{
		GameObject pInstance = HT.Utils.Instantiate(_rigidBody.gameObject);

		//-----
		Vector3 vViewVec = m_pCaster.m_vViewVector;
		float fXRotationRange = _spreadXDegree * 0.5f;
		vViewVec = Quaternion.Euler(0.0f, HT.RandomUtils.Range(-fXRotationRange, fXRotationRange), 0.0f) * vViewVec;

		Rigidbody pRigid = pInstance.GetComponent<Rigidbody>();
		pRigid.AddForce(vViewVec * HT.RandomUtils.Range(_minSpeed, _maxSpeed), _forceMode);

		//-----
		Vector3 vPos = m_pCaster.transform.position;
		if (string.IsNullOrEmpty(_castDummyName) == false)
		{
			GameObject pDummy = m_pCaster.FindDummyPivot(_castDummyName);
			vPos = pDummy.transform.position;
		}

		vPos += (vViewVec * _posOffsetDistance);
		pInstance.transform.position = vPos;
	}
}


/////////////////////////////////////////
//---------------------------------------