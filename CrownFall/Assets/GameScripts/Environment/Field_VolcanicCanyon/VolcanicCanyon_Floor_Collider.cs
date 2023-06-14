using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class VolcanicCanyon_Floor_Collider : MonoBehaviour {
	/////////////////////////////////////////
	//---------------------------------------
	public VolcanicCanyon_Floor m_pListener;


	/////////////////////////////////////////
	//---------------------------------------
	void OnCollisionEnter (Collision pCollision) {
		m_pListener.OnCollisionCallBack (pCollision);
	}
}
