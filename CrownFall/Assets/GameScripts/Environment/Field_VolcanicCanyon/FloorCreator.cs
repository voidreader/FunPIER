using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class FloorCreator : MonoBehaviour {
	/////////////////////////////////////////
	//---------------------------------------
	static public FloorCreator _Instance;

	/////////////////////////////////////////
	//---------------------------------------
	public VolcanicCanyon_Floor m_pInst_Floor;

	public int m_nCountX;
	public int m_nCountY;
	public float m_fFloorSize;

	public Vector3 m_vCenterPos;


	/////////////////////////////////////////
	//---------------------------------------
	public VolcanicCanyon_Floor [] m_pFloors;


	/////////////////////////////////////////
	//---------------------------------------
	// Use this for initialization
	void Start () {
		_Instance = this;
		m_pFloors = new VolcanicCanyon_Floor [m_nCountX * m_nCountY];

		for (int nY = 0; nY < m_nCountX; ++nY) {
			for (int nX = 0; nX < m_nCountY; ++nX) {
				VolcanicCanyon_Floor pObj = Instantiate (m_pInst_Floor);

				Vector3 vPos = m_vCenterPos;
				vPos.x = m_vCenterPos.x + (nX * m_fFloorSize) - ((m_nCountX * m_fFloorSize) * 0.5f);
				vPos.z = m_vCenterPos.z + (nY * m_fFloorSize) - ((m_nCountY * m_fFloorSize) * 0.5f);
				vPos.y = m_vCenterPos.y;

				pObj.transform.position = vPos;
				pObj.transform.SetParent (gameObject.transform);

				m_pFloors [(nY * m_nCountX) + nX] = pObj;
			}
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	public VolcanicCanyon_Floor GetFloor (int nX, int nY) {
		if (nX >= 0 && nX < m_nCountX) {
			if (nY >= 0 && nY < m_nCountY) {
				return m_pFloors [(nY * m_nCountX) + nX];
			}
		}

		return null;
	}

	public VolcanicCanyon_Floor GetFloor_ByAxis (float fX, float fZ) {
		VolcanicCanyon_Floor pFirstFloor = GetFloor (0, 0);
		Vector3 vStartPos = pFirstFloor.transform.position;

		float fDistX = fX - vStartPos.x;
		float fDistZ = fZ - vStartPos.z;

		int nPosX = (int) (fDistX / m_fFloorSize);
		int nPosZ = (int) (fDistZ / m_fFloorSize);

		return GetFloor (nPosX, nPosZ);
	}
}
