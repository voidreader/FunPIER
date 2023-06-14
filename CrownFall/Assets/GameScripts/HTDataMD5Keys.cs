using UnityEngine;
using System.Collections;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class HTDataMD5Keys
{
	//---------------------------------------
	private static bool _initialized = false;

	//---------------------------------------
	public const bool PARAM_USE_DATAPURITY = true;
	public static HTDataMD5Pair[] _dataKeys_Table = null;

	public static HTDataMD5Pair _dataKeys_SteamDLL = null;
	public static HTDataMD5Pair _dataKeys_SteamInfo = null;

	//---------------------------------------
	private static void Initialize()
	{
		if (_initialized)
			return;

		_initialized = true;

		/*VDATAKEY_TABLE_INIT*/_dataKeys_Table = new HTDataMD5Pair[1];        /*END*/

		/*VDATAKEY_TABLE_ARRAY*/
        _dataKeys_Table[0] = new HTDataMD5Pair().Init("LocalizationTable_Strings", "WL2I+6ZCODDxBO0/EiXLsgTsqR3JJHtMhUXu8vkx5RrsLjqNf3Gru+ym9cS+VGbR");
        /*END*/
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