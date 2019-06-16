using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class Localization : SingleInstance<Localization>
{
	private const string Key = "Localization_Key";

	private Dictionary<int, Dictionary<string, string>> languageDic;

	private Action onChangeLanguage;

	//private event Action onChangeLanguage;

	public event Action OnChangeLanguage;

	public LanguageEnum CurrentLanguage
	{
		get
		{
			return (LanguageEnum)PlayerPrefs.GetInt("Localization_Key");
		}
	}

	public Localization()
	{
		this.languageDic = new Dictionary<int, Dictionary<string, string>>();
		LanguageEnum systemLanguage;
		// if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified)
		// {
		// 	systemLanguage = LanguageEnum.ChineseSimplified;
		// }
		// else if (Application.systemLanguage == SystemLanguage.ChineseTraditional)
		// {
		// 	systemLanguage = LanguageEnum.ChineseTraditional;
		// }
		// else if (Application.systemLanguage == SystemLanguage.English)
		// {
		// 	systemLanguage = LanguageEnum.English;
		// }
		// else
		// {
		// 	systemLanguage = LanguageEnum.Kor;
		// }

		if (Application.systemLanguage == SystemLanguage.English)
			systemLanguage = LanguageEnum.English;
		else
			systemLanguage = LanguageEnum.Kor;
	
		this.ResetLanguage(systemLanguage);
	}

	private void SetLanguage(LanguageEnum systemLanguage, int sheetIndex)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if (this.languageDic.ContainsKey(sheetIndex))
		{
			this.languageDic[sheetIndex] = dictionary;
		}
		else
		{
			this.languageDic.Add(sheetIndex, dictionary);
		}
		LocalizationDataJson t = ReadJson.GetT<LocalizationDataJson>("LocalizationData" + sheetIndex);
		if (systemLanguage == LanguageEnum.ChineseSimplified)
		{
			foreach (LocalizationData current in t.Data)
			{
				if (dictionary.ContainsKey(current.Key))
				{
					DebugTool.Error("Localization Same Key : " + current.Key);
				}
				else
				{
					dictionary.Add(current.Key, current.Zh);
				}
			}
		}
		else if (systemLanguage == LanguageEnum.ChineseTraditional)
		{
			foreach (LocalizationData current2 in t.Data)
			{
				if (dictionary.ContainsKey(current2.Key))
				{
					DebugTool.Error("Localization Same Key : " + current2.Key);
				}
				else
				{
					dictionary.Add(current2.Key, current2.Zh);
				}
			}
		}
		else if (systemLanguage == LanguageEnum.English)
		{
			foreach (LocalizationData current3 in t.Data)
			{
				if (dictionary.ContainsKey(current3.Key))
				{
					DebugTool.Error("Localization Same Key : " + current3.Key);
				}
				else
				{
					dictionary.Add(current3.Key, current3.En);
				}
			}
		}
		else
		{
			foreach (LocalizationData current4 in t.Data)
			{
				if (dictionary.ContainsKey(current4.Key))
				{
					DebugTool.Error("Localization Same Key : " + current4.Key);
				}
				else
				{
					dictionary.Add(current4.Key, current4.Ko);
				}
			}
		}
	}

	public void ReleaseLanguageSheet(int sheetIndex)
	{
		if (this.languageDic.ContainsKey(sheetIndex))
		{
			this.languageDic[sheetIndex].Clear();
			this.languageDic.Remove(sheetIndex);
		}
	}

	public void ResetLanguage(LanguageEnum systemLanguage)
	{
		PlayerPrefs.SetInt("Localization_Key", (int)systemLanguage);
		foreach (int current in this.languageDic.Keys)
		{
			this.SetLanguage(systemLanguage, current);
		}
		if (this.onChangeLanguage != null)
		{
			this.onChangeLanguage();
		}
	}

	public string GetLable(string key, int sheetIndex = 0)
	{
		this.CheckLanguageSheet(sheetIndex);
		string result;
		if (this.languageDic[sheetIndex].TryGetValue(key, out result))
		{
			return result;
		}
		return key;
	}

	public string GetLable(string key, string obj, int sheetIndex = 0)
	{
		this.CheckLanguageSheet(sheetIndex);
		string format;
		if (this.languageDic[sheetIndex].TryGetValue(key, out format))
		{
			return string.Format(format, obj);
		}
		return key;
	}

	private void CheckLanguageSheet(int sheetIndex)
	{
		if (!this.languageDic.ContainsKey(sheetIndex))
		{
			this.SetLanguage(this.CurrentLanguage, sheetIndex);
		}
	}
}
