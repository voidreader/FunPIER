using System;
using SA.Android.Utilities;
using UnityEngine;

namespace SA.Android.App.Utils
{
	/// <summary>
	/// A Locale object represents a specific geographical, political, or cultural region.
	/// An operation that requires a Locale to perform its task is called locale-sensitive
	/// and uses the Locale to tailor information for the user.
	/// </summary>
	[Serializable]
	public class AN_Locale 
	{
		const string JAVA_CLASS = "com.stansassets.android.util.AN_Locale";
		
		[SerializeField] protected string m_CountryCode;
		[SerializeField] protected string m_LanguageCode;
		[SerializeField] protected string m_CurrencySymbol;
		[SerializeField] protected string m_CurrencyCode;


		/// <summary>
		/// Gets the current value of the default locale for this instance of the Java Virtual Machine.
		/// 
		/// The Java Virtual Machine sets the default locale during startup based on the host environment.
		/// It is used by many locale-sensitive methods if no locale is explicitly specified. 
		/// </summary>
		public static AN_Locale GetDefault()
		{
			var json = AN_Java.Bridge.CallStatic<string>(JAVA_CLASS, "GetDefault");
			return JsonUtility.FromJson<AN_Locale>(json);
		}

		/// <summary>
		/// Returns a three-letter abbreviation for this locale's country.
		/// If the country matches an ISO 3166-1 alpha-2 code,
		/// the corresponding ISO 3166-1 alpha-3 uppercase code is returned.
		/// If the locale doesn't specify a country, this will be the empty string.
		/// </summary>
		public string CountryCode
		{
			get { return m_CountryCode; }
		}

		/// <summary>
		/// Returns a three-letter abbreviation of this locale's language.
		/// If the language matches an ISO 639-1 two-letter code,
		/// the corresponding ISO 639-2/T three-letter lowercase code is returned.
		/// The ISO 639-2 language codes can be found on-line,
		/// see "Codes for the Representation of Names of Languages Part 2: Alpha-3 Code".
		/// If the locale specifies a three-letter language, the language is returned as is.
		/// If the locale does not specify a language the empty string is returned.
		/// </summary>
		public string LanguageCode
		{
			get { return m_LanguageCode; }
		}

		/// <summary>
		/// Gets the symbol of this currency for the specified locale.
		/// For example, for the US Dollar, the symbol is "$" if the specified locale is the US,
		/// while for other locales it may be "US$".
		/// If no symbol can be determined, the ISO 4217 currency code is returned.
		/// </summary>
		public string CurrencySymbol
		{
			get { return m_CurrencySymbol; }
		}

		/// <summary>
		/// Gets the ISO 4217 currency code of this currency.
		/// </summary>
		public string CurrencyCode
		{
			get { return m_CurrencyCode; }
		}
	}
}