using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class PowerPlant_Thing : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private GameObject _notPressed = null;
	[SerializeField]
	private GameObject _pressed = null;
	[SerializeField]
	private float _pressDist = 2.0f;

	//---------------------------------------
	private bool _isPressed = false;
	public PowerPlant_Wall _parent = null;

	//---------------------------------------
	public void Init()
	{
		_parent.AddBeltUpObject(gameObject);
	}

	private void OnDestroy()
	{
		_parent.RemoveBeltUpObject(gameObject);
	}

	private void Update()
	{
		if (_isPressed)
			return;

		if (_parent.Belt_Press.Presssed)
		{
			float fDist = Vector3.Distance(_parent.Belt_Press.transform.position, gameObject.transform.position);

			bool bPress = false;
			if (fDist < _pressDist)
				bPress = true;

			if (_parent.Belt_Press.transform.position.z > gameObject.transform.position.z)
				bPress = true;

			if (bPress)
			{
				_isPressed = true;

				_notPressed.SetActive(false);
				_pressed.SetActive(true);
			}
		}
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------