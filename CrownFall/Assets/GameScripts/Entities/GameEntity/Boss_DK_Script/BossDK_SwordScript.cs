using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class BossDK_SwordScript : MonoBehaviour {
	/////////////////////////////////////////
	//---------------------------------------
	static public BossDK_SwordScript _Instance;


	/////////////////////////////////////////
	//---------------------------------------
	bool m_bLerpStart = false;
	float m_fLerpTime = 0.0f;

	public GameObject m_pBindTarget;
	public bool m_bFollowBindTarget = false;

	public GameObject m_pSword_Begin;
	public GameObject m_pSword_End;


	/////////////////////////////////////////
	//---------------------------------------
	public GameObject m_pChain_Dummy;
	public int m_nChainCount;
	public GameObject m_pInst_Chain;
	public GameObject m_pInst_Lantern;

	//---------------------------------------
	public BossDK_LanternScript m_pLantern;


	/////////////////////////////////////////
	//---------------------------------------
	public GameObject m_pDynamicMeshFilter;

	public float m_fDynamicPolyCreateTime;
	public Vector3 m_vPrevSword_Begin;
	public Vector3 m_vPrevSword_End;

	int m_nCreatePolysCount;

	int m_nPolyMaxCounts = 256;
	Vector3 [] m_vCreatePolys;
	Vector2 [] m_vCreateUVs;
	int [] m_vIndeces;

	//---------------------------------------
	float m_fCreationLimitTime = 0.0f;

	//---------------------------------------
	MeshFilter m_pMeshFilter;


	/////////////////////////////////////////
	//---------------------------------------
	// Use this for initialization
	void Start () {
		_Instance = this;

		//-----
		m_vCreateUVs = new Vector2 [m_nPolyMaxCounts];
		for (int nInd = 0; nInd < m_vCreateUVs.Length; ++nInd)
			m_vCreateUVs [nInd] = Vector2.zero;

		m_vIndeces = new int [(int) (m_nPolyMaxCounts * 1.5f)];

		int nVertIndex = 0;
		for (int nInd = 0; nInd < m_vIndeces.Length; nInd += 6) {
			m_vIndeces [nInd + 0] = nVertIndex + 0;
			m_vIndeces [nInd + 1] = nVertIndex + 1;
			m_vIndeces [nInd + 2] = nVertIndex + 2;

			m_vIndeces [nInd + 3] = nVertIndex + 2;
			m_vIndeces [nInd + 4] = nVertIndex + 0;
			m_vIndeces [nInd + 5] = nVertIndex + 3;

			nVertIndex += 4;
		}

		return;

		////-----
		//GameObject pLastChain = null;
		//Rigidbody pLastChain_Rigid = null;
		//Vector3 vChainPos = m_pChain_Dummy.transform.position;
		//
		//for (int nInd = 0; nInd < m_nChainCount; ++nInd) {
		//	GameObject pNewChain = (GameObject) Instantiate (m_pInst_Chain);
		//	pNewChain.transform.position = vChainPos;
		//
		//	//-----
		//	if (pLastChain == null) {
		//		Rigidbody pRigid = pNewChain.GetComponent<Rigidbody> ();
		//		pRigid.drag = 100.0f;
		//		pRigid.mass = 1.0f;
		//
		//		pLastChain = gameObject;
		//		pLastChain_Rigid = pLastChain.GetComponent<Rigidbody> ();
		//	}
		//
		//	HingeJoint pChainJoint = pNewChain.GetComponent<HingeJoint> ();
		//	pChainJoint.connectedBody = pLastChain_Rigid;
		//
		//	//-----
		//	pLastChain = pNewChain;
		//	pLastChain_Rigid = pLastChain.GetComponent<Rigidbody> ();
		//
		//	vChainPos.z += 0.35f;
		//}
		//
		//GameObject pLantern = (GameObject) Instantiate (m_pInst_Lantern);
		//pLantern.transform.position = vChainPos;
		//
		//HingeJoint pLanternJoint = pLantern.GetComponent<HingeJoint> ();
		//pLanternJoint.connectedBody = pLastChain_Rigid;
		//
		//m_pLantern = pLantern.GetComponent<BossDK_LanternScript> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (m_bFollowBindTarget) {
			gameObject.transform.position = m_pBindTarget.gameObject.transform.position;
			gameObject.transform.rotation = m_pBindTarget.gameObject.transform.rotation;

			//-----
			do {
				if (m_fDynamicPolyCreateTime > 0.0f && m_pDynamicMeshFilter != null) {
					m_fDynamicPolyCreateTime -= Time.fixedDeltaTime;

					m_fCreationLimitTime -= HT.TimeUtils.GameTime;
					if (m_fCreationLimitTime > 0.0f)
						break;	

					m_fCreationLimitTime = 0.01f;

					//-----
					if (m_pMeshFilter == null)
						m_pMeshFilter = m_pDynamicMeshFilter.GetComponent<MeshFilter> ();

					if (m_pMeshFilter.mesh == null)
						m_pMeshFilter.mesh = new Mesh ();

					Mesh pMesh = m_pMeshFilter.mesh;

					//-----
					m_vCreatePolys [m_nCreatePolysCount++] = m_vPrevSword_End;
					m_vCreatePolys [m_nCreatePolysCount++] = m_vPrevSword_Begin;
					m_vCreatePolys [m_nCreatePolysCount++] = m_pSword_Begin.transform.position;
					m_vCreatePolys [m_nCreatePolysCount++] = m_pSword_End.transform.position;

					pMesh.vertices = m_vCreatePolys;
					pMesh.uv = m_vCreateUVs;
					pMesh.triangles = m_vIndeces;

					//-----
					UpdatePrevSwordPosition ();

				} else
					m_pDynamicMeshFilter = null;

			} while (false);

			//-----

		} else {
			if (m_bLerpStart) {
				m_fLerpTime -= HT.TimeUtils.GameTime;

				Vector3 vPos = gameObject.transform.position;
				Quaternion qQuat = gameObject.transform.rotation;

				gameObject.transform.position = Vector3.Lerp (vPos, m_pBindTarget.gameObject.transform.position, m_fLerpTime);
				gameObject.transform.rotation = Quaternion.Lerp (qQuat, m_pBindTarget.gameObject.transform.rotation, m_fLerpTime);

				if (m_fLerpTime <= 0.0f) {
					m_bFollowBindTarget = true;

					m_bLerpStart = false;
					m_fLerpTime = 0.0f;
				}
			}
		}
	}

	//---------------------------------------
	void OnDestroy () {
		m_vCreatePolys = null;
		m_vCreateUVs = null;
	}

	/////////////////////////////////////////
	//---------------------------------------
	public void ResetDynamicPolys (float fTime) {
		m_fDynamicPolyCreateTime = fTime;

		m_nCreatePolysCount = 0;

		m_vCreatePolys = null;
		m_vCreateUVs = null;

		m_vCreatePolys = new Vector3[m_nPolyMaxCounts];

		//-----
		UpdatePrevSwordPosition ();
	}

	/////////////////////////////////////////
	//---------------------------------------
	public void SetSwordPosInit (float fLerpTime) {
		m_bFollowBindTarget = false;

		m_bLerpStart = true;
		m_fLerpTime = fLerpTime;
	}

	void UpdatePrevSwordPosition () {
		m_vPrevSword_Begin = m_pSword_Begin.transform.position;
		m_vPrevSword_End = m_pSword_End.transform.position;
	}
}
