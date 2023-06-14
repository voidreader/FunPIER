using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class Object_Follower : MonoBehaviour
{
	//---------------------------------------
	[Header("OBJECTS")]
	public IActorBase _owner = null;
	public GameObject _targetObject = null;

	[Header("OFFSET")]
	[SerializeField]
	private Vector3 _positionOffset = Vector3.zero;
	[SerializeField]
	private Vector3 _rotationOffset = Vector3.zero;

	[Header("INHERITS")]
	[SerializeField]
	private bool _inheritTransform = true;
	[SerializeField]
	private bool _inheritRotation = true;
	[SerializeField]
	private bool _inheritScale = false;

	//---------------------------------------
	private void Update()
	{
		if (_targetObject != null)
		{
			Transform pParent = _targetObject.transform;
			if (_inheritTransform)
				gameObject.transform.position = (pParent.position + _positionOffset);

			if (_inheritRotation)
				gameObject.transform.rotation = (pParent.rotation * Quaternion.Euler(_rotationOffset));

			if (_inheritScale)
				gameObject.transform.localScale = pParent.localScale;
		}
	}
}


/////////////////////////////////////////
//---------------------------------------