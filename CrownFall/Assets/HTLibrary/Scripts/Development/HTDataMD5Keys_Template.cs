using UnityEngine;
using System.Collections;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class HTDataMD5Keys_Template
{
	//---------------------------------------
	private static bool _initialized = false;

	//---------------------------------------
	public const bool PARAM_USE_DATAPURITY = false;
	public static HTDataMD5Pair[] _dataKeys_Table = null;

	public static HTDataMD5Pair _dataKeys_SteamDLL = null;
	public static HTDataMD5Pair _dataKeys_SteamInfo = null;

	//---------------------------------------
	private static void Initialize()
	{
		if (_initialized)
			return;

		_initialized = true;

		/*VDATAKEY_TABLE_INIT*/        /*END*/

		/*VDATAKEY_TABLE_ARRAY*/        /*END*/
	}

	//---------------------------------------
	public static string FindMD5(string szName)
	{
		Initialize();
		for (int nInd = 0; nInd < _dataKeys_Table.Length; ++nInd)
			if (_dataKeys_Table[nInd].name == szName)
				return _dataKeys_Table[nInd].md5;

		return string.Empty;
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------
public class HTDataMD5Pair
{
	public string name = null;
	public string md5 = null;

	public HTDataMD5Pair Init(string szName, string szMD5)
	{
		name = szName;
		md5 = szMD5;
		return this;
	}
}

//---------------------------------------
public class HTDataMD5Keys_Token
{
	public const string FEATURE_DISABLED = "PARAM_USE_DATAPURITY = false";
	public const string FEATURE_ENABLED = "PARAM_USE_DATAPURITY = true";

	public const string KEYWORD_END = "        /*END*/";
	public const string FEATURE_TABLE_INIT = "/*VDATAKEY_TABLE_INIT*/";
	public const string KEYWORD_TABLE_INIT = "_dataKeys_Table = new HTDataMD5Pair[{0}];";

	public const string FEATURE_TABLE_ARRAY = "/*VDATAKEY_TABLE_ARRAY*/";
	public const string KEYWORD_TABLE_ARRAY = "        _dataKeys_Table[{0}] = new HTDataMD5Pair().Init(\"{1}\", \"{2}\");";
}

/////////////////////////////////////////
//---------------------------------------