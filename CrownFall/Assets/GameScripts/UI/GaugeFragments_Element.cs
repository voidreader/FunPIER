using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/////////////////////////////////////////
//---------------------------------------
public class GaugeFragments_Element : MonoBehaviour
{
	//---------------------------------------
	public RectTransform _rectTransform = null;

	[SerializeField]
	private Image _fragImage = null;
	[SerializeField]
	private Animation _animation = null;

	//---------------------------------------
	public void Init(float fSize)
	{
		Vector2 vSize = _fragImage.rectTransform.sizeDelta;
		vSize.x = fSize;

		_fragImage.rectTransform.sizeDelta = vSize;

		//-----
		_animation.Play();
	}
}


/////////////////////////////////////////
//---------------------------------------