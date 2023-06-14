using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HT;

public class GameInitializer : MonoBehaviour
{
	//---------------------------------------
	private static bool _initialized = false;

	[SerializeField]
	private GEnv _genv = null;

	//---------------------------------------
	private void Awake()
	{
		//-----
		HTLocaleTable.onLocaleTableLoadComplete += LoadAllExtraLocales;

		//-----
		if (_initialized == false)
		{
			_initialized = true;

			HTFramework.Instance.Initialize(false, _genv, null, null);
			HTGlobalUI.Instance.MaskFade(true);
		}

		//-----
		gameObject.SetActive(false);
	}

	//---------------------------------------
	private void LoadAllExtraLocales()
	{
#if UNITY_STANDALONE
		for (SystemLanguage eInd = 0; eInd < SystemLanguage.Unknown; ++eInd)
		{
			try
			{
				string szFileAddr = string.Format("CrownFall_Extra/{0}.csv", eInd.ToString());
				if (System.IO.File.Exists(szFileAddr))
				{
					List<Dictionary<string, object>> vExtraLocale = CSVReader.ReadFromFile(szFileAddr);
					if (vExtraLocale == null)
						continue;

					if (HTLocaleTable.IsLanguageAvailable(eInd))
						continue;

					//-----
					ExtraLanguageInfo pInfo = new ExtraLanguageInfo();
					pInfo.language = eInd;

					Dictionary<string, string> vLocaleDictionay = new Dictionary<string, string>();
					for (int nInd = 0; nInd < vExtraLocale.Count; ++nInd)
					{
						string szTid = (string)vExtraLocale[nInd]["[Tid]"];
						if (szTid.IndexOf("<!-") == 0 || string.IsNullOrEmpty(szTid))
							continue;

						string szText = (string)vExtraLocale[nInd]["[Text]"];
						if (szTid == "DISPLAY_TEXT")
						{
							pInfo.displayText = szText;
						}
						else
						{
							szText = szText.Replace("[n]", System.Environment.NewLine);
							vLocaleDictionay.Add(szTid, szText);
						}
					}

					//-----
					HTLocaleTable.AddExtraLocales(eInd, vLocaleDictionay);
					GameExtraLanguage.Instance.ExtraLanguages.Add(pInfo);
				}
			}
			catch (Exception pException)
			{
				HTDebug.PrintLog(eMessageType.Error, pException.Message);
			}
		}
#endif // UNITY_STANDALONE
	}

	//---------------------------------------
}
