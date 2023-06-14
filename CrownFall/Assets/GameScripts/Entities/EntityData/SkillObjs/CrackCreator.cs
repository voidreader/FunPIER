using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HT;

/////////////////////////////////////////
//---------------------------------------
public sealed class CrackCreator : ISkillObject
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("CRACK CREATOR INFO")]
	public CrackCreator_Cracks m_vInst_Crack;
	public CrackCreator_Cracks[] m_vInst_Crack_Branch;
	public bool _randomRotation = true;
	
	public AudioClip _branceCreateSound = null;
	public float m_fBranchCreateCamShake;

	//---------------------------------------
	public int m_nCrackBranchCountMin;
	public int m_nCrackBranchCountMax;

	public int m_nCrackBranchLengthMin;
	public int m_nCrackBranchLengthMax;

	public int m_nCrackBranchChildCountMin;
	public int m_nCrackBranchChildCountMax;

	public float m_nCrackSpreadDegreeMax;

	//---------------------------------------
	public float m_fCrackBranchWaitTime;
	public float m_fCrackBranchSize;

	//---------------------------------------
	public float m_fDamageEnablingLimitTime;
	public float m_fLifeTime;


	/////////////////////////////////////////
	//---------------------------------------
	public List<CrackCreator_Cracks> m_vCracks;

	//---------------------------------------
	public float m_fLeastLifeTime;

	//---------------------------------------
	bool m_bCallCreateEffect = false;


	/////////////////////////////////////////
	//---------------------------------------
	public override void Init()
	{
		if (_randomRotation)
			gameObject.transform.Rotate(gameObject.transform.up, Random.Range(0.0f, 360.0f));

		m_vCracks = new List<CrackCreator_Cracks>();

		//-----
		CrackCreator_Cracks pMainCrack = HT.Utils.Instantiate(m_vInst_Crack);
		pMainCrack.transform.position = gameObject.transform.position;
		pMainCrack.transform.rotation = gameObject.transform.rotation;
		pMainCrack.transform.SetParent(gameObject.transform);

		m_vCracks.Add(pMainCrack);

		pMainCrack.m_nBranchCount = Random.Range(m_nCrackBranchCountMin, m_nCrackBranchCountMax);
		pMainCrack.m_nBranchLength = Random.Range(m_nCrackBranchLengthMin, m_nCrackBranchLengthMax);

		//-----
		m_fLeastLifeTime = m_fLifeTime;

		//-----
		pMainCrack.m_pParent = this;
		pMainCrack.Init();
	}

	public override void Frame()
	{
		m_fLeastLifeTime -= HT.TimeUtils.GameTime;
		m_bCallCreateEffect = false;

		//-----
		for (int nInd = 0; nInd < m_vCracks.Count; ++nInd)
			m_vCracks[nInd].Frame();

		if (m_fLeastLifeTime <= 0.0f)
			HT.Utils.SafeDestroy(gameObject);
	}

	public override void Release()
	{
		for (int nInd = 0; nInd < m_vCracks.Count; ++nInd)
		{
			HT.Utils.SafeDestroy(m_vCracks[nInd].gameObject);
		}

		m_vCracks = null;
	}


	/////////////////////////////////////////
	//---------------------------------------
	//void CreateBranches (CrackCreator_Cracks pBranch, GameObject pNode, int nLength) {
	//	nLength -= 1;
	//	if (nLength <= 0)
	//		return;
	//
	//	//-----
	//	if (pNode != null) {
	//		Vector3 vFoward = GEnv.GetRandomVector (pNode.transform.forward, m_nCrackSpreadDegreeMax);
	//
	//		pBranch.transform.rotation.SetLookRotation (vFoward);
	//		pBranch.transform.SetParent (pNode);
	//	}
	//
	//	//-----
	//	m_vCracks.Add (pBranch);
	//
	//	pBranch.m_nBranchLength = nLength;
	//	pBranch.m_pParent = this;
	//
	//	//-----
	//	m_fLeastLifeTime = m_fLifeTime;
	//}

	public void CallCreateEffect()
	{
		if (m_bCallCreateEffect)
			return;

		m_bCallCreateEffect = true;

		HT.HTSoundManager.PlaySound(_branceCreateSound);

		if (m_fBranchCreateCamShake > 0.0f)
			CameraManager._Instance.SetCameraShake(m_fBranchCreateCamShake);
	}
}
