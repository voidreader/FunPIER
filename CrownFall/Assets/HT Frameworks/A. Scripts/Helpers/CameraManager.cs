using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
// CameraManager
public class CameraManager : MonoBehaviour {
	/////////////////////////////////////////
	//---------------------------------------
	static public CameraManager _Instance;

	/////////////////////////////////////////
	//---------------------------------------
	public Camera m_pMainCamera;
    public Vector3 m_vCamOffset;
	public GameObject m_pTargetEntity;

	//---------------------------------------
	public bool m_bCamFollowing = true;
	public Vector3 m_vCamFollowingOffset;

	//---------------------------------------
	public GameObject m_pFocusedEntity;
	public float m_fCameraZoomRatio = 0.25f;

    //---------------------------------------
    float m_fCameraShake;
	Vector3 m_vCameraShake;
	
	
	/////////////////////////////////////////
	//---------------------------------------
	void Awake () {
		_Instance = this;
	}
	
	private void Update () {
		if (m_pMainCamera == null)
			return;
		
		if (m_pTargetEntity == null)
			return;

		//-----
		Vector3 vCamPos;

		//-----
		if (m_bCamFollowing) {
			vCamPos = m_pTargetEntity.transform.position + m_vCamFollowingOffset;

		} else {
			vCamPos = m_pMainCamera.transform.position;

		}

		//-----
		if (m_fCameraShake > 0.0f) {
			float fNormX = Random.Range (0.0f, 1.0f);
			m_vCameraShake.x = m_fCameraShake * fNormX;
			m_vCameraShake.y = m_fCameraShake * (1.0f - fNormX);

			m_fCameraShake = m_fCameraShake * 0.75f;
            if (m_fCameraShake < 0.01f)
                m_fCameraShake = 0.0f;

		} else {
			m_fCameraShake = 0.0f;

			m_vCameraShake.x = 0.0f;
			m_vCameraShake.y = 0.0f;
		}
		
		//-----
		vCamPos += m_vCameraShake;
        vCamPos += m_vCamOffset;
		m_pMainCamera.transform.position = vCamPos;

		//-----
		if (m_pFocusedEntity != null) {
			Vector3 vPlayerPos = m_pTargetEntity.transform.position;
			Vector3 vEnemyPos = m_pFocusedEntity.transform.position;
			float fEnemyDist = Vector3.Distance (vEnemyPos, vPlayerPos);

			if (m_pMainCamera.orthographic) {
				float fDefaultSize = 5.0f;
				if (fEnemyDist > 0.1f) {
					m_pMainCamera.orthographicSize = fDefaultSize + (fEnemyDist * m_fCameraZoomRatio);
				} else {
					m_pMainCamera.orthographicSize = fDefaultSize;
				}

			} else {
			}
		}
	}

	private void OnDestroy () {
	}
	
	/////////////////////////////////////////
	//---------------------------------------
	public void SetCameraShake (float fPow)
	{
		m_fCameraShake += fPow;
	}

	public void ClearCameraShake()
	{
		m_fCameraShake = 0.0f;
	}
}
