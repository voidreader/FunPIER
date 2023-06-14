using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class MobileColliderHelper : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private SphereCollider _sphere = null;
	[SerializeField]
	private float _increaseRatio = 2.0f;

	//---------------------------------------
	private void Awake()
	{
		if (HT.HTAfxPref.IsMobilePlatform == false)
			return;

		if (_sphere != null)
			_sphere.radius = _sphere.radius * _increaseRatio;

		this.enabled = false;
	}
}


/////////////////////////////////////////
//---------------------------------------