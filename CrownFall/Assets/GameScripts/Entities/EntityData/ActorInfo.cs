using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class ActorInfo : MonoBehaviour {
	/////////////////////////////////////////
	//---------------------------------------
	public string m_szActorName;
	public eBossType _bossTYpe = eBossType.eCommon;


	/////////////////////////////////////////
	//---------------------------------------
	public int m_nBaseHP = 1;
	public int m_nCalculatedHP = 1;

	//---------------------------------------
	public float m_fBaseMoveSpeed = 100.0f;
	public float m_fCalculatedMoveSpeed = -1.0f;


	/////////////////////////////////////////
	//---------------------------------------
	public HT.CInt m_cnMaxHP = new HT.CInt();
	public HT.CInt m_cnNowHP = new HT.CInt();


	/////////////////////////////////////////
	//---------------------------------------
}
