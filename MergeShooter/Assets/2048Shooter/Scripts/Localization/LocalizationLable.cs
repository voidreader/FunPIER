using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizationLable : MonoBehaviour
{
	[SerializeField]
	private string key;

	[SerializeField]
	private string value;

	[SerializeField]
	private int sheetIndex;

	[SerializeField]
	private bool isFontModefy;

	[SerializeField]
	private int EnFontSize = 57;

	private void OnEnable()
	{
		Singleton<Localization>.instance.OnChangeLanguage += new Action(this.OnChangeLanguage);
		this.OnChangeLanguage();
	}

	private void OnDisable()
	{
		Singleton<Localization>.instance.OnChangeLanguage -= new Action(this.OnChangeLanguage);
	}

	private void OnDestroy()
	{
		Singleton<Localization>.instance.OnChangeLanguage -= new Action(this.OnChangeLanguage);
	}

	private void OnChangeLanguage()
	{
		Text component = base.GetComponent<Text>();
		if (component != null)
		{
			component.text = Singleton<Localization>.instance.GetLable(this.key, this.value, this.sheetIndex).Replace("\\n", "\n");
			if (this.isFontModefy)
			{
				if (Application.systemLanguage != SystemLanguage.Chinese && Application.systemLanguage != SystemLanguage.ChineseSimplified)
				{
					component.fontSize = this.EnFontSize;
				}
			}
		}
	}
}
