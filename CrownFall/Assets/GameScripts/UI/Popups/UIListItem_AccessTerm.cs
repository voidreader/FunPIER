using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace HT
{
	public class UIListItem_AccessTerm : MonoBehaviour
	{
		[SerializeField]
		private Text _termLocalizationElem = null;

		public RectTransform _rectTransform = null;

		public void Init(string szTermElem)
		{
			_termLocalizationElem.text = szTermElem;

			_rectTransform = GetComponent<RectTransform>();
			_rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _termLocalizationElem.preferredHeight);
		}
	}
}
