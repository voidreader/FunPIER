//----------------------------------------------
//    Google2u: Google Doc Unity integration
//         Copyright © 2015 Litteratus
//
//        This file has been auto-generated
//              Do not manually edit
//----------------------------------------------

using UnityEngine;
using System.Globalization;

namespace Google2u
{
	[System.Serializable]
	public class LevelDataRow : IGoogle2uRow
	{
		public int _level;
		public int _needexp;
		public int _mergespot;
		public int _battlespot;
		public int _gem;
		public LevelDataRow(string __ID, string __level, string __needexp, string __mergespot, string __battlespot, string __gem) 
		{
			{
			int res;
				if(int.TryParse(__level, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_level = res;
				else
					Debug.LogError("Failed To Convert _level string: "+ __level +" to int");
			}
			{
			int res;
				if(int.TryParse(__needexp, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_needexp = res;
				else
					Debug.LogError("Failed To Convert _needexp string: "+ __needexp +" to int");
			}
			{
			int res;
				if(int.TryParse(__mergespot, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_mergespot = res;
				else
					Debug.LogError("Failed To Convert _mergespot string: "+ __mergespot +" to int");
			}
			{
			int res;
				if(int.TryParse(__battlespot, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_battlespot = res;
				else
					Debug.LogError("Failed To Convert _battlespot string: "+ __battlespot +" to int");
			}
			{
			int res;
				if(int.TryParse(__gem, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_gem = res;
				else
					Debug.LogError("Failed To Convert _gem string: "+ __gem +" to int");
			}
		}

		public int Length { get { return 5; } }

		public string this[int i]
		{
		    get
		    {
		        return GetStringDataByIndex(i);
		    }
		}

		public string GetStringDataByIndex( int index )
		{
			string ret = System.String.Empty;
			switch( index )
			{
				case 0:
					ret = _level.ToString();
					break;
				case 1:
					ret = _needexp.ToString();
					break;
				case 2:
					ret = _mergespot.ToString();
					break;
				case 3:
					ret = _battlespot.ToString();
					break;
				case 4:
					ret = _gem.ToString();
					break;
			}

			return ret;
		}

		public string GetStringData( string colID )
		{
			var ret = System.String.Empty;
			switch( colID )
			{
				case "level":
					ret = _level.ToString();
					break;
				case "needexp":
					ret = _needexp.ToString();
					break;
				case "mergespot":
					ret = _mergespot.ToString();
					break;
				case "battlespot":
					ret = _battlespot.ToString();
					break;
				case "gem":
					ret = _gem.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "level" + " : " + _level.ToString() + "} ";
			ret += "{" + "needexp" + " : " + _needexp.ToString() + "} ";
			ret += "{" + "mergespot" + " : " + _mergespot.ToString() + "} ";
			ret += "{" + "battlespot" + " : " + _battlespot.ToString() + "} ";
			ret += "{" + "gem" + " : " + _gem.ToString() + "} ";
			return ret;
		}
	}
	public sealed class LevelData : IGoogle2uDB
	{
		public enum rowIds {
			Level1, Level2, Level3, Level4, Level5, Level6, Level7, Level8, Level9, Level10, Level11, Level12, Level13, Level14, Level15, Level16, Level17, Level18
			, Level19, Level20, Level21, Level22, Level23, Level24, Level25, Level26, Level27, Level28, Level29, Level30, Level31, Level32, Level33, Level34, Level35, Level36, Level37, Level38
			, Level39, Level40, Level41, Level42, Level43, Level44, Level45, Level46, Level47, Level48, Level49, Level50, Level51, Level52, Level53, Level54, Level55, Level56, Level57, Level58
			, Level59, Level60, Level61, Level62, Level63, Level64, Level65, Level66, Level67, Level68, Level69, Level70, Level71, Level72, Level73, Level74, Level75, Level76, Level77, Level78
			, Level79, Level80, Level81, Level82, Level83, Level84, Level85, Level86, Level87, Level88, Level89, Level90, Level91, Level92, Level93, Level94, Level95, Level96, Level97, Level98
			, Level99
		};
		public string [] rowNames = {
			"Level1", "Level2", "Level3", "Level4", "Level5", "Level6", "Level7", "Level8", "Level9", "Level10", "Level11", "Level12", "Level13", "Level14", "Level15", "Level16", "Level17", "Level18"
			, "Level19", "Level20", "Level21", "Level22", "Level23", "Level24", "Level25", "Level26", "Level27", "Level28", "Level29", "Level30", "Level31", "Level32", "Level33", "Level34", "Level35", "Level36", "Level37", "Level38"
			, "Level39", "Level40", "Level41", "Level42", "Level43", "Level44", "Level45", "Level46", "Level47", "Level48", "Level49", "Level50", "Level51", "Level52", "Level53", "Level54", "Level55", "Level56", "Level57", "Level58"
			, "Level59", "Level60", "Level61", "Level62", "Level63", "Level64", "Level65", "Level66", "Level67", "Level68", "Level69", "Level70", "Level71", "Level72", "Level73", "Level74", "Level75", "Level76", "Level77", "Level78"
			, "Level79", "Level80", "Level81", "Level82", "Level83", "Level84", "Level85", "Level86", "Level87", "Level88", "Level89", "Level90", "Level91", "Level92", "Level93", "Level94", "Level95", "Level96", "Level97", "Level98"
			, "Level99"
		};
		public System.Collections.Generic.List<LevelDataRow> Rows = new System.Collections.Generic.List<LevelDataRow>();

		public static LevelData Instance
		{
			get { return NestedLevelData.instance; }
		}

		private class NestedLevelData
		{
			static NestedLevelData() { }
			internal static readonly LevelData instance = new LevelData();
		}

		private LevelData()
		{
			Rows.Add( new LevelDataRow("Level1", "1", "15", "4", "2", "0"));
			Rows.Add( new LevelDataRow("Level2", "2", "30", "1", "1", "10"));
			Rows.Add( new LevelDataRow("Level3", "3", "50", "1", "1", "10"));
			Rows.Add( new LevelDataRow("Level4", "4", "100", "1", "1", "10"));
			Rows.Add( new LevelDataRow("Level5", "5", "120", "1", "1", "10"));
			Rows.Add( new LevelDataRow("Level6", "6", "150", "1", "0", "10"));
			Rows.Add( new LevelDataRow("Level7", "7", "180", "1", "0", "10"));
			Rows.Add( new LevelDataRow("Level8", "8", "240", "1", "0", "10"));
			Rows.Add( new LevelDataRow("Level9", "9", "300", "1", "0", "10"));
			Rows.Add( new LevelDataRow("Level10", "10", "330", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level11", "11", "400", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level12", "12", "450", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level13", "13", "500", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level14", "14", "550", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level15", "15", "600", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level16", "16", "650", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level17", "17", "700", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level18", "18", "720", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level19", "19", "780", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level20", "20", "850", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level21", "21", "900", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level22", "22", "920", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level23", "23", "940", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level24", "24", "960", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level25", "25", "980", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level26", "26", "1000", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level27", "27", "1050", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level28", "28", "1100", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level29", "29", "1150", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level30", "30", "1200", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level31", "31", "1250", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level32", "32", "1300", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level33", "33", "1350", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level34", "34", "1400", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level35", "35", "1450", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level36", "36", "1500", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level37", "37", "1550", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level38", "38", "1600", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level39", "39", "1650", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level40", "40", "1700", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level41", "41", "1750", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level42", "42", "1800", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level43", "43", "1850", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level44", "44", "1900", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level45", "45", "1950", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level46", "46", "2000", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level47", "47", "2050", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level48", "48", "2100", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level49", "49", "2150", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level50", "50", "2200", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level51", "51", "2300", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level52", "52", "2400", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level53", "53", "2500", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level54", "54", "2600", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level55", "55", "2700", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level56", "56", "2800", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level57", "57", "2900", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level58", "58", "3000", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level59", "59", "3100", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level60", "60", "3200", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level61", "61", "3300", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level62", "62", "3400", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level63", "63", "3500", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level64", "64", "3600", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level65", "65", "3700", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level66", "66", "3800", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level67", "67", "3900", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level68", "68", "4000", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level69", "69", "4100", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level70", "70", "4200", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level71", "71", "4300", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level72", "72", "4400", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level73", "73", "4500", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level74", "74", "4600", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level75", "75", "4700", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level76", "76", "4800", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level77", "77", "4900", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level78", "78", "5000", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level79", "79", "5100", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level80", "80", "5200", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level81", "81", "5400", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level82", "82", "5600", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level83", "83", "5800", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level84", "84", "6000", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level85", "85", "6200", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level86", "86", "6400", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level87", "87", "6600", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level88", "88", "6800", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level89", "89", "7000", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level90", "90", "7300", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level91", "91", "7600", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level92", "92", "7900", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level93", "93", "8200", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level94", "94", "8500", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level95", "95", "8800", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level96", "96", "9100", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level97", "97", "9400", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level98", "98", "9700", "0", "0", "10"));
			Rows.Add( new LevelDataRow("Level99", "99", "10000", "0", "0", "10"));
		}
		public IGoogle2uRow GetGenRow(string in_RowString)
		{
			IGoogle2uRow ret = null;
			try
			{
				ret = Rows[(int)System.Enum.Parse(typeof(rowIds), in_RowString)];
			}
			catch(System.ArgumentException) {
				Debug.LogError( in_RowString + " is not a member of the rowIds enumeration.");
			}
			return ret;
		}
		public IGoogle2uRow GetGenRow(rowIds in_RowID)
		{
			IGoogle2uRow ret = null;
			try
			{
				ret = Rows[(int)in_RowID];
			}
			catch( System.Collections.Generic.KeyNotFoundException ex )
			{
				Debug.LogError( in_RowID + " not found: " + ex.Message );
			}
			return ret;
		}
		public LevelDataRow GetRow(rowIds in_RowID)
		{
			LevelDataRow ret = null;
			try
			{
				ret = Rows[(int)in_RowID];
			}
			catch( System.Collections.Generic.KeyNotFoundException ex )
			{
				Debug.LogError( in_RowID + " not found: " + ex.Message );
			}
			return ret;
		}
		public LevelDataRow GetRow(string in_RowString)
		{
			LevelDataRow ret = null;
			try
			{
				ret = Rows[(int)System.Enum.Parse(typeof(rowIds), in_RowString)];
			}
			catch(System.ArgumentException) {
				Debug.LogError( in_RowString + " is not a member of the rowIds enumeration.");
			}
			return ret;
		}

	}

}
