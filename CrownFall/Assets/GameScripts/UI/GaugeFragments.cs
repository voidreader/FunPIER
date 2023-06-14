using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/////////////////////////////////////////
//---------------------------------------
public class GaugeFragments : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private Image _parentImage = null;
	[SerializeField]
	private float _gaugeSize = 647.0f;
	[SerializeField]
	private RectTransform _fragmentParent = null;

	[SerializeField]
	private GaugeFragments_Element _instance = null;

	private float _prevFillAmount = 1.0f;

	//---------------------------------------
	private void Update()
	{
		if (_parentImage.fillAmount < _prevFillAmount)
		{
			//-----
			float fRatioDiff = _prevFillAmount - _parentImage.fillAmount;
			float fWidth = _gaugeSize * fRatioDiff;

			float fGaugeCurSize = _gaugeSize * _prevFillAmount;
			Vector2 vAnchoredPos = (Vector2.right * (fGaugeCurSize - fWidth));

			//-----
			GaugeFragments_Element pNewElem = HT.Utils.InstantiateFromPool(_instance);
			pNewElem._rectTransform.SetParent(gameObject.transform);
			pNewElem._rectTransform.localScale = Vector3.one;
			pNewElem._rectTransform.anchoredPosition = vAnchoredPos;
			pNewElem.Init(fWidth);

			pNewElem._rectTransform.SetParent(_fragmentParent);

			HT.Utils.SafeDestroy(pNewElem.gameObject, 1.0f);

			//-----
			_prevFillAmount = _parentImage.fillAmount;
		}
	}
}


/////////////////////////////////////////
//---------------------------------------