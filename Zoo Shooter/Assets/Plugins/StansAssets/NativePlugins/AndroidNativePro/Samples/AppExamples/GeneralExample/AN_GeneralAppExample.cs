using System.Collections;
using System.Collections.Generic;
using SA.Android.App.Utils;
using UnityEngine;

public class AN_GeneralAppExample : MonoBehaviour {



	private void PrintLocaleInfo()
	{
		var currentLocale = AN_Locale.GetDefault();
		Debug.Log("currentLocale.CountryCode: " + currentLocale.CountryCode);
		Debug.Log("currentLocale.CurrencyCode: " + currentLocale.CurrencyCode);
		Debug.Log("currentLocale.LanguageCode: " + currentLocale.LanguageCode);
		Debug.Log("currentLocale.CurrencySymbol: " + currentLocale.CurrencySymbol); 
	}
	
}
