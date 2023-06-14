using UnityEngine;
using System.Collections;
using HT;

/////////////////////////////////////////
//---------------------------------------
public class VolcanicCanyon_Floor : MonoBehaviour {
	/////////////////////////////////////////
	//---------------------------------------
	Animation m_pAnimation;
	public string m_szDiveAnim;
	public string m_szDiveAnim_Instantly;
	public string m_szUnDiveAnim;

	//---------------------------------------
	bool m_bReadyForDive;
	bool m_bDive = false;
	float m_fDiveTime = 0.0f;

	//---------------------------------------
	public GameObject m_pFloorTagPoint;

	//---------------------------------------
	public GameObject m_pDiveAlertEffect;

	//---------------------------------------
	public ActorBuff m_pDiveWhenCollisionOnBuff;
	AIActor_Extend_FL m_pAIExtend_FL;

	public float m_fDiveWhenCollisionTime = 5.0f;
	public float m_fDiveWhenCollisionCamShake = 0.5f;
	public AudioClip m_pDiveWhenCollisionSounds;
	public ParticleSystem m_pDiveWhenCollisionEffect;


	/////////////////////////////////////////
	//---------------------------------------
	// Use this for initialization
	void Start () {
		m_pAnimation = GetComponent<Animation> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (m_pAIExtend_FL == null && BattleFramework._Instance != null) {
			m_pAIExtend_FL = BattleFramework._Instance.m_pEnemyActor.GetComponent<AIActor_Extend_FL> ();
		}

		if (m_bDive) {
			m_fDiveTime -= HT.TimeUtils.GameTime;

			if (m_fDiveTime <= 0.0f) {
				m_bDive = false;

				m_pAnimation.Play (m_szUnDiveAnim);
			}
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void SetDive (float fTime, bool bInstantly) {
		m_bDive = true;
		m_fDiveTime = fTime;

		if (bInstantly)
			m_pAnimation.Play (m_szDiveAnim_Instantly);
		
		else {
			m_bReadyForDive = true;
			m_pAnimation.Play (m_szDiveAnim);

			if (m_pDiveAlertEffect != null) {
				GameObject pObj = HT.Utils.Instantiate(m_pDiveAlertEffect);
				pObj.transform.position = gameObject.transform.position;
			}
		}
	}

	public bool GetDive () {
		return m_bDive;
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void OnCollisionCallBack (Collision pCollision) {
		if (m_pAIExtend_FL == null || m_pDiveWhenCollisionOnBuff == null)
			return;

		if (GetDive () && m_bReadyForDive == false)
			return;

		if (pCollision.gameObject == m_pAIExtend_FL.gameObject) {
			m_bReadyForDive = false;

			IActorBase.ActorBuffEnable pBuff = m_pAIExtend_FL.m_pActorBase.FindEnabledActorBuff (m_pDiveWhenCollisionOnBuff);
			if (pBuff != null) {
				SetDive (m_fDiveWhenCollisionTime, true);

				if (m_fDiveWhenCollisionCamShake > 0.0f)
					CameraManager._Instance.SetCameraShake (m_fDiveWhenCollisionCamShake);

				HT.HTSoundManager.PlaySound(m_pDiveWhenCollisionSounds);

				if (m_pDiveWhenCollisionEffect != null) {
					ParticleSystem pEffect = HT.Utils.Instantiate(m_pDiveWhenCollisionEffect);
					pEffect.transform.position = pCollision.transform.position;

					Destroy (pEffect.gameObject, pEffect.TotalSimulationTime());
				}
			}
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void CallDiveComplete () {
		m_bReadyForDive = false;
	}
}
