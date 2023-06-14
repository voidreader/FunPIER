using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class CrackCreator_Cracks : MonoBehaviour {
	/////////////////////////////////////////
	//---------------------------------------
	public MeshRenderer m_pMeshRenderer;

	//---------------------------------------
	public CrackCreator m_pParent;
	public GameObject [] m_vNodes;
	public HT.PhysicEventer[] _physicEventers = null;

	//---------------------------------------
	public bool m_bIsShowRangeByDetail = true;
	public int m_nBranchCount = 0;
	public int m_nBranchLength = 0;


	/////////////////////////////////////////
	//---------------------------------------
	Material m_pMaterial;
	Color m_pMaterialColor;
	float m_fMaterialBaseAlpha;

	//---------------------------------------
	bool m_bCreatedBranch = false;
	float m_fLeastLifeTime = 0.0f;

	//---------------------------------------
	public GameObject [] m_vWillBrachCreateNode;


	/////////////////////////////////////////
	//---------------------------------------
	public void Init () {
		m_pParent.CallCreateEffect ();

		//-----
		if (_physicEventers != null && _physicEventers.Length > 0)
		{
			for (int nInd = 0; nInd < _physicEventers.Length; ++nInd)
				_physicEventers[nInd].Init(OnPhysicEventer_Enter, null);
		}

		//-----
		m_pMaterial = Instantiate (m_pMeshRenderer.material);

		m_pMaterialColor = m_pMaterial.color;
		m_fMaterialBaseAlpha = m_pMaterialColor.a;

		m_pMeshRenderer.material = m_pMaterial;

		//-----
		if (m_nBranchLength > 0) {
			m_vWillBrachCreateNode = new GameObject[m_nBranchCount];
			for (int nInd = 0; nInd < m_nBranchCount; ++nInd) {
				//-----
				bool bHasUnusedNode = false;
				for (int nCheckInd = 0; nCheckInd < m_vNodes.Length; ++nCheckInd) {
					if (m_vNodes [nCheckInd] != null) {
						bHasUnusedNode = true;
						break;
					}
				}

				if (bHasUnusedNode == false)
					break;

				//-----
				for (;;) {
					int nNodeInd = Random.Range (0, m_vNodes.Length);
					m_vWillBrachCreateNode [nInd] = m_vNodes [nNodeInd];

					if (m_vWillBrachCreateNode [nInd] != null) {
						m_vNodes [nNodeInd] = null;
						break;
					}
				}
			}

			//-----
			for (int nInd = 0; nInd < m_vWillBrachCreateNode.Length; ++nInd) {
				if (m_vWillBrachCreateNode [nInd] == null)
					continue;

				float fCrackSize = m_pParent.m_fCrackBranchSize;
				Vector3 vPos = m_vWillBrachCreateNode [nInd].transform.position;
				vPos += m_vWillBrachCreateNode [nInd].transform.forward * (fCrackSize * 0.5f);

				if (m_bIsShowRangeByDetail)
					BattleFramework._Instance.CreateAreaAlert(vPos, fCrackSize, m_pParent.m_fCrackBranchWaitTime);
				else
					BattleFramework._Instance.CreateAreaAlert(eAlertRingType.Angle360_Simple, vPos, fCrackSize, m_pParent.m_fCrackBranchWaitTime);
			}
		}
	}

	public void Frame () {
		m_fLeastLifeTime += HT.TimeUtils.GameTime;

		if (m_nBranchLength > 0 && m_bCreatedBranch == false && m_fLeastLifeTime >= m_pParent.m_fCrackBranchWaitTime) {
			m_bCreatedBranch = true;

			for (int nInd = 0; nInd < m_nBranchCount; ++nInd) {
				GameObject pBranchNode = m_vWillBrachCreateNode [nInd];
				if (pBranchNode == null)
					break;

				//-----
				int nBranchIndex = Random.Range (0, m_pParent.m_vInst_Crack_Branch.Length);
				CrackCreator_Cracks pBranch = Instantiate (m_pParent.m_vInst_Crack_Branch [nBranchIndex]);

				//-----
				m_pParent.m_vCracks.Add (pBranch);

				//Vector3 vFoward = pBranchNode.transform.forward;
				//if (m_pParent.m_nCrackSpreadDegreeMax > 0.0f)
				//	GEnv.GetRandomVector (vFoward, m_pParent.m_nCrackSpreadDegreeMax);
				//pBranch.transform.rotation.SetLookRotation (vFoward);

				//-----
				//int nBranchStartInd = Random.Range (0, pBranch.m_vNodes.Length);
				//GameObject pBranchStartNode = pBranch.m_vNodes [nBranchStartInd];
				//pBranch.m_vNodes [nBranchStartInd] = null;

				//Vector3 vPos = pBranchNode.transform.position - pBranchStartNode.transform.position;
				//pBranch.transform.position = pBranchStartNode.transform.position + vPos;

				pBranch.transform.position = pBranchNode.transform.position;
				pBranch.transform.rotation = pBranchNode.transform.rotation;
				pBranch.transform.SetParent (pBranchNode.transform);

				//-----
				pBranch.m_pParent = m_pParent;

				pBranch.m_nBranchCount = Random.Range (m_pParent.m_nCrackBranchChildCountMin, m_pParent.m_nCrackBranchChildCountMax);
				pBranch.m_nBranchLength = m_nBranchLength - 1;

				pBranch.Init ();
			}
		}

		//-----
		if (m_pParent.m_fLeastLifeTime < 0.5f) {
			m_pMaterialColor.a = m_fMaterialBaseAlpha * (m_pParent.m_fLeastLifeTime * 2.0f);
			m_pMaterial.color = m_pMaterialColor;

			m_pMeshRenderer.material = m_pMaterial;
		}
	}

	void OnDestroy () {
		m_vWillBrachCreateNode = null;
	}


	/////////////////////////////////////////
	//---------------------------------------
	void OnTriggerEnter (Collider pCollision)
	{
		if (m_pParent.m_fDamageEnablingLimitTime >= m_fLeastLifeTime)
			m_pParent.OnTriggerEvent (pCollision.gameObject);
	}

	void OnPhysicEventer_Enter(GameObject pObj)
	{
		if (m_pParent.m_fDamageEnablingLimitTime >= m_fLeastLifeTime)
			m_pParent.OnTriggerEvent(pObj);
	}
}
