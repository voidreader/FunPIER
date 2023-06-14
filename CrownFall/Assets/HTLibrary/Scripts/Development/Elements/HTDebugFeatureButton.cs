using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class HTDebugFeatureButton : MonoBehaviour
	{
		//---------------------------------------
		[SerializeField]
		private Button _button = null;
		[SerializeField]
		private Text _subject = null;
		[SerializeField]
		private Text _order = null;

		//---------------------------------------
		private HTDebugFeatureOrder _featureOrder = null;

		//---------------------------------------
		private void Awake()
		{
			_button.onClick.AddListener(OnClickButton);
		}

		public void Init(HTDebugFeatureOrder pOrder)
		{
			_featureOrder = pOrder;
			_subject.text = pOrder.subject;
			_order.text = pOrder.order;
		}

		private void OnClickButton()
		{
			HTConsoleWindow.DoConsoleOrder(_featureOrder.order);
		}

		//---------------------------------------
	}

	/////////////////////////////////////////
	//---------------------------------------
}