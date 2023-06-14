using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class GameObj_ShadowPlane : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private Transform _parent = null;
	[SerializeField]
	private GameObject _shadowPlane_Root = null;

	//---------------------------------------
	//private void OnEnable()
	//{
	//	HT.HTAfxPref.onChangeQuality += OnChangeQuality;
	//}
	//
	//private void OnDisable()
	//{
	//	HT.HTAfxPref.onChangeQuality -= OnChangeQuality;
	//}

	//---------------------------------------
	//void Awake()
	//{
	//	OnChangeQuality(HT.HTAfxPref.Quality);
	//}

	private void FixedUpdate()
	{
		//if (_shadowPlane_Root.activeInHierarchy)
		//{
			_shadowPlane_Root.transform.rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);

			Vector3 vCurPos = _parent.position;
			vCurPos.y = GameFramework._Instance.GetPositionByPhysic(vCurPos).y;

			_shadowPlane_Root.transform.position = vCurPos;
		//}
	}

	//---------------------------------------
	//private void OnChangeQuality(int nLevel)
	//{
	//	bool bEnable = false;
	//	if (nLevel == 0)
	//		bEnable = true;
	//
	//	_shadowPlane_Root.SetActive(bEnable);
	//}
}


/////////////////////////////////////////
//---------------------------------------