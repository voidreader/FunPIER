using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomExtraLangauge : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private int _extraLanguageIndex = 0;
	[SerializeField]
	private Text _languageName = null;

	//---------------------------------------
	private Toggle _button = null;
	private SystemLanguage _managedLanguage = SystemLanguage.English;

	//---------------------------------------
	private void Awake()
	{
		bool bEnabled = false;
		do
		{
			_button = GetComponent<Toggle>();
			if (_button == null)
				break;

			if (GameExtraLanguage.Instance.ExtraLanguages.Count < _extraLanguageIndex + 1)
				break;

			//-----
			bEnabled = true;

			_button.onValueChanged.AddListener(OnClickedButton);

			ExtraLanguageInfo pInfo = GameExtraLanguage.Instance.ExtraLanguages[_extraLanguageIndex];
			_managedLanguage = pInfo.language;
			_languageName.text = pInfo.displayText;

			if (HT.HTLocaleTable.CurrentLanguage == _managedLanguage)
				_button.isOn = true;
			else
				_button.isOn = false;
		}
		while (false);

		//-----
		gameObject.SetActive(bEnabled);
	}

	//---------------------------------------
	private void OnClickedButton(bool bOn)
	{
		if (bOn)
			HT.HTLocaleTable.ChangeLanguage(_managedLanguage, true);
	}

	//---------------------------------------
}
