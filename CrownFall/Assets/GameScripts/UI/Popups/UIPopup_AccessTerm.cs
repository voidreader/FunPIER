using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class UIPopup_AccessTerm : UIPopup
	{
		//---------------------------------------
		[Header("GAME ACCESS TERM")]
		[SerializeField]
		private ListScrollRect _gameTerm_List = null;
		[SerializeField]
		private RectTransform _gameTerm_Content = null;
		[SerializeField]
		private UIListFilter_AccessTerm _gameTerm_listFilter = null;
		[SerializeField]
		private Toggle _gameTerm_Button = null;

		//---------------------------------------
		private bool _checkedGameTerm = false;
		private bool _checkedInfoTerm = true;

		//---------------------------------------
		protected override void OnAwake()
		{
			base.OnAwake();
		}

		IEnumerator Start()
		{
			//-----
			_gameTerm_Button.onValueChanged.AddListener(OnCheckGameTerm);

			//-----
			//string[] vGameTermLocale = new string[20]
			//{
			//	HT.HTLocaleTable.GetLocalstring("term_access_1"),
			//	HT.HTLocaleTable.GetLocalstring("term_access_2"),
			//	HT.HTLocaleTable.GetLocalstring("term_access_3"),
			//	HT.HTLocaleTable.GetLocalstring("term_access_4"),
			//	HT.HTLocaleTable.GetLocalstring("term_access_5"),
			//	HT.HTLocaleTable.GetLocalstring("term_access_6"),
			//	HT.HTLocaleTable.GetLocalstring("term_access_7"),
			//	HT.HTLocaleTable.GetLocalstring("term_access_8"),
			//	HT.HTLocaleTable.GetLocalstring("term_access_9"),
			//	HT.HTLocaleTable.GetLocalstring("term_access_10"),
			//	HT.HTLocaleTable.GetLocalstring("term_access_11"),
			//	HT.HTLocaleTable.GetLocalstring("term_access_12"),
			//	HT.HTLocaleTable.GetLocalstring("term_access_13"),
			//	HT.HTLocaleTable.GetLocalstring("term_access_14"),
			//	HT.HTLocaleTable.GetLocalstring("term_access_15"),
			//	HT.HTLocaleTable.GetLocalstring("term_access_16"),
			//	HT.HTLocaleTable.GetLocalstring("term_access_17"),
			//	HT.HTLocaleTable.GetLocalstring("term_access_18"),
			//	HT.HTLocaleTable.GetLocalstring("term_access_19"),
			//	HT.HTLocaleTable.GetLocalstring("term_access_20")
			//};

			TextAsset pTerm = Resources.Load("Texts/PrivacyTerms") as TextAsset;
			string szGamePrivacyTerms = pTerm.text;

			List<string> vGameTermLocales = new List<string>();
			//for (int nInd = 0; nInd < vGameTermLocale.Length; ++nInd)
			//{
				//string[] vArray = vGameTermLocale[nInd].Split('\n');
				string[] vArray = szGamePrivacyTerms.Split('\n');
				for (int nArray = 0; nArray < vArray.Length; ++nArray)
				{
					//if (vArray[nArray].Length > 1)
						vGameTermLocales.Add(vArray[nArray]);
				}
			//}
			
			_gameTerm_listFilter.Initialize(vGameTermLocales);
			_gameTerm_listFilter.BuildList();

			yield return new WaitForEndOfFrame();

			//-----
			yield break;
		}

		//---------------------------------------
		void OnCheckGameTerm(bool bOn)
		{
			_checkedGameTerm = bOn;
			RefreshButton();
		}

		void RefreshButton()
		{
			CloseButton.interactable = (_checkedGameTerm && _checkedInfoTerm) ? true : false;
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}