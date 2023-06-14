using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class ActorSkill_DynamicPolys_Instance : MonoBehaviour {
	/////////////////////////////////////////
	//---------------------------------------
	public MeshFilter m_pMeshFilter;

	//---------------------------------------
	public ActorSkill_DynamicPolys m_pSkill_DynPolys;

	public IActorBase m_pParent;
	public IActorBase m_pTarget;

	//---------------------------------------
	public ActorSkill_DynamicPolys_Collider m_pCollider;


	/////////////////////////////////////////
	//---------------------------------------
	public float m_fLifeTime = 1.0f;
	public Color m_pNowColor;

	//---------------------------------------
	float m_fTotalLifeTime;
	float m_fFirstAlpha;

	//---------------------------------------
	MeshRenderer m_pRenderer;


	/////////////////////////////////////////
	//---------------------------------------
	// Use this for initialization
	void Start () {
		MeshRenderer pRenderer = GetComponent<MeshRenderer> ();
		pRenderer.material.SetColor ("_TintColor", m_pNowColor);

		m_fTotalLifeTime = m_fLifeTime;
		m_fFirstAlpha = m_pNowColor.a;

		//-----
		m_pRenderer = GetComponent<MeshRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		m_fLifeTime -= HT.TimeUtils.GameTime;

		//-----
		m_pNowColor.a = m_fFirstAlpha * (m_fLifeTime / m_fTotalLifeTime);
		m_pRenderer.material.SetColor ("_TintColor", m_pNowColor);

		//-----
		if (m_fLifeTime <= 0.0f) {
			HT.Utils.SafeDestroy(gameObject);

			if (m_pCollider != null)
				HT.Utils.SafeDestroy(m_pCollider.gameObject);
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
}
