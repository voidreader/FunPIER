using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class BossHAL_Monitor : MonoBehaviour
{
	//---------------------------------------
	[Header("ROOTS")]
	[SerializeField]
	private GameObject _monitorSight_Root = null;

	[Header("DEFAULT FACES")]
	[SerializeField]
	private GameObject _monitor_Face = null;
	[SerializeField]
	private Vector3 _monitor_Face_MoveMin = Vector3.zero;
	[SerializeField]
	private Vector3 _monitor_Face_MoveMax = Vector3.zero;
	[SerializeField]
	private GameObject _monitor_Eye = null;
	[SerializeField]
	private Vector3 _monitor_Eye_MoveMin = Vector3.zero;
	[SerializeField]
	private Vector3 _monitor_Eye_MoveMax = Vector3.zero;
	[SerializeField]
	private bool _eyeChasing = true;
	[SerializeField]
	private float _fChasingSpeed = 0.25f;

	//---------------------------------------

	//---------------------------------------
	private Ray _castRay = new Ray();

	//---------------------------------------
	private void Awake()
	{

	}
	
	private void FixedUpdate()
	{
		if (_eyeChasing)
		{
			if (BattleFramework._Instance == null)
				return;

			IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;
			if (pPlayer == null)
				return;

			Vector3 vPosVec = pPlayer.transform.position - _monitorSight_Root.transform.position;
			vPosVec.Normalize();

			_castRay.origin = _monitorSight_Root.transform.position;
			_castRay.direction = vPosVec;

			RaycastHit pHit;
			if (UnityEngine.Physics.Raycast(_castRay, out pHit, 3.0f))
			{
				//-----
				Vector3 vFacePos = Vector3.MoveTowards(_monitor_Face.transform.position, pHit.point, _fChasingSpeed * HT.TimeUtils.GameTime);
				_monitor_Face.transform.position = vFacePos;

				Vector3 vLocFacePos = _monitor_Face.transform.localPosition;
				vLocFacePos.x = Mathf.Clamp(vLocFacePos.x, _monitor_Face_MoveMin.x, _monitor_Face_MoveMax.x);
				vLocFacePos.y = Mathf.Clamp(vLocFacePos.y, _monitor_Face_MoveMin.y, _monitor_Face_MoveMax.y);
				vLocFacePos.z = Mathf.Clamp(vLocFacePos.z, _monitor_Face_MoveMin.z, _monitor_Face_MoveMax.z);
				_monitor_Face.transform.localPosition = vLocFacePos;

				//-----
				Vector3 vEyePos = Vector3.MoveTowards(_monitor_Eye.transform.position, pHit.point, _fChasingSpeed * HT.TimeUtils.GameTime);
				_monitor_Eye.transform.position = vEyePos;

				Vector3 vLocEyePos = _monitor_Eye.transform.localPosition;
				vLocEyePos.x = Mathf.Clamp(vLocEyePos.x, _monitor_Eye_MoveMin.x, _monitor_Eye_MoveMax.x);
				vLocEyePos.y = Mathf.Clamp(vLocEyePos.y, _monitor_Eye_MoveMin.y, _monitor_Eye_MoveMax.y);
				vLocEyePos.z = Mathf.Clamp(vLocEyePos.z, _monitor_Eye_MoveMin.z, _monitor_Eye_MoveMax.z);
				_monitor_Eye.transform.localPosition = vLocEyePos;
			}
		}
	}

	//---------------------------------------
	public void SetEyeChasing(bool bSet)
	{
		_eyeChasing = bSet;
	}
}


/////////////////////////////////////////
//---------------------------------------