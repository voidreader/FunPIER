using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class FieldEvent : MonoBehaviour {
	/////////////////////////////////////////
	//---------------------------------------
	[Header("SETTINGS")]
	public float m_fEventTime;

	//---------------------------------------
	protected float m_fTimeLeast;


	/////////////////////////////////////////
	//---------------------------------------
	public void Init () {
		m_fTimeLeast = m_fEventTime;

		Init_Child ();
	}

	public bool Frame () {
		m_fTimeLeast -= HT.TimeUtils.GameTime;

		Frame_Child ();

		return ((m_fTimeLeast > 0.0f) ? true : false);
	}

	public void Release () {

		Release_Child ();
	}

	//---------------------------------------
	public virtual void Init_Child () {
	}

	public virtual void Frame_Child () {
	}

	public virtual void Release_Child () {
	}


	/////////////////////////////////////////
	//---------------------------------------
}
