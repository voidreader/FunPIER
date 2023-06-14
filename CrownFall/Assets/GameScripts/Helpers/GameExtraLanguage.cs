using System;
using System.Collections.Generic;
using UnityEngine;
using HT;


//---------------------------------------
public class ExtraLanguageInfo
{
	public SystemLanguage language;
	public string displayText = null;
}

//---------------------------------------
public class GameExtraLanguage : Singleton<GameExtraLanguage>
{
	//---------------------------------------
	private List<ExtraLanguageInfo> _extraLanguages = new List<ExtraLanguageInfo>();
	public List<ExtraLanguageInfo> ExtraLanguages { get { return _extraLanguages; } }

	//---------------------------------------
}
