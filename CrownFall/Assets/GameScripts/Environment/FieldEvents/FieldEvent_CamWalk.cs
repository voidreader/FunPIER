using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public sealed class FieldEvent_CamWalk : FieldEvent
{
	/////////////////////////////////////////
	//---------------------------------------
	public Camera m_pCamera;

	public enum eCamWalkType
	{
		eMoveToPosition = 0,
		eMoveToCamMgrPos,
	}

	public eCamWalkType m_eCamWalkType;
	public Vector3 m_vMoveToPosition;

	[SerializeField]
	private bool _useLocalPosition = false;

	//---------------------------------------
	[Header("READ ONLY")]
	public Vector3 m_vStartPos;


	/////////////////////////////////////////
	//---------------------------------------
	public override void Init_Child()
	{
		if (_useLocalPosition)
			m_vStartPos = m_pCamera.transform.localPosition;
		else
			m_vStartPos = m_pCamera.transform.position;
	}


	public override void Frame_Child()
	{
		Vector3 vPos = Vector3.zero;

		if (m_fEventTime > 0.0f)
		{
			switch (m_eCamWalkType)
			{
				case eCamWalkType.eMoveToPosition:
					vPos = Vector3.Lerp(m_vMoveToPosition, m_vStartPos, m_fTimeLeast / m_fEventTime);
					break;

				case eCamWalkType.eMoveToCamMgrPos:
					GameObject pTargetEntity = CameraManager._Instance.m_pTargetEntity;
					Vector3 vOffset = CameraManager._Instance.m_vCamFollowingOffset;

					if (pTargetEntity != null)
						vOffset += pTargetEntity.transform.position;

					vPos = Vector3.Lerp(vOffset, m_vStartPos, m_fTimeLeast / m_fEventTime);
					break;
			}
		}
		else
		{
			switch (m_eCamWalkType)
			{
				case eCamWalkType.eMoveToPosition:
					vPos = m_vMoveToPosition;
					break;

				case eCamWalkType.eMoveToCamMgrPos:
					GameObject pTargetEntity = CameraManager._Instance.m_pTargetEntity;
					Vector3 vOffset = CameraManager._Instance.m_vCamFollowingOffset;

					if (pTargetEntity != null)
						vOffset += pTargetEntity.transform.position;

					vPos = vOffset;
					break;
			}
		}

		if (_useLocalPosition && m_eCamWalkType != eCamWalkType.eMoveToCamMgrPos)
			m_pCamera.transform.localPosition = vPos;
		else
			m_pCamera.transform.position = vPos;
	}


	public override void Release_Child()
	{
	}


	/////////////////////////////////////////
	//---------------------------------------
}
