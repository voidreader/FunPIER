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
	public class StageDataRow : IGoogle2uRow
	{
		public int _level;
		public int _enemycount;
		public int _bosshp;
		public float _range;
		public StageDataRow(string __ID, string __level, string __enemycount, string __bosshp, string __range) 
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
				if(int.TryParse(__enemycount, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_enemycount = res;
				else
					Debug.LogError("Failed To Convert _enemycount string: "+ __enemycount +" to int");
			}
			{
			int res;
				if(int.TryParse(__bosshp, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_bosshp = res;
				else
					Debug.LogError("Failed To Convert _bosshp string: "+ __bosshp +" to int");
			}
			{
			float res;
				if(float.TryParse(__range, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_range = res;
				else
					Debug.LogError("Failed To Convert _range string: "+ __range +" to float");
			}
		}

		public int Length { get { return 4; } }

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
					ret = _enemycount.ToString();
					break;
				case 2:
					ret = _bosshp.ToString();
					break;
				case 3:
					ret = _range.ToString();
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
				case "enemycount":
					ret = _enemycount.ToString();
					break;
				case "bosshp":
					ret = _bosshp.ToString();
					break;
				case "range":
					ret = _range.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "level" + " : " + _level.ToString() + "} ";
			ret += "{" + "enemycount" + " : " + _enemycount.ToString() + "} ";
			ret += "{" + "bosshp" + " : " + _bosshp.ToString() + "} ";
			ret += "{" + "range" + " : " + _range.ToString() + "} ";
			return ret;
		}
	}
	public sealed class StageData : IGoogle2uDB
	{
		public enum rowIds {
			level0, level1, level2, level3, level4, level5, level6, level7, level8, level9, level10, level11, level12, level13, level14, level15, level16, level17
			, level18, level19, level20, level21, level22, level23, level24, level25, level26, level27, level28, level29, level30, level31, level32, level33, level34, level35, level36, level37
			, level38, level39, level40, level41, level42, level43, level44, level45, level46, level47, level48, level49, level50, level51, level52, level53, level54, level55, level56, level57
			, level58, level59, level60, level61, level62, level63, level64, level65, level66, level67, level68, level69, level70, level71, level72, level73, level74, level75, level76, level77
			, level78, level79, level80, level81, level82, level83, level84, level85
		};
		public string [] rowNames = {
			"level0", "level1", "level2", "level3", "level4", "level5", "level6", "level7", "level8", "level9", "level10", "level11", "level12", "level13", "level14", "level15", "level16", "level17"
			, "level18", "level19", "level20", "level21", "level22", "level23", "level24", "level25", "level26", "level27", "level28", "level29", "level30", "level31", "level32", "level33", "level34", "level35", "level36", "level37"
			, "level38", "level39", "level40", "level41", "level42", "level43", "level44", "level45", "level46", "level47", "level48", "level49", "level50", "level51", "level52", "level53", "level54", "level55", "level56", "level57"
			, "level58", "level59", "level60", "level61", "level62", "level63", "level64", "level65", "level66", "level67", "level68", "level69", "level70", "level71", "level72", "level73", "level74", "level75", "level76", "level77"
			, "level78", "level79", "level80", "level81", "level82", "level83", "level84", "level85"
		};
		public System.Collections.Generic.List<StageDataRow> Rows = new System.Collections.Generic.List<StageDataRow>();

		public static StageData Instance
		{
			get { return NestedStageData.instance; }
		}

		private class NestedStageData
		{
			static NestedStageData() { }
			internal static readonly StageData instance = new StageData();
		}

		private StageData()
		{
			Rows.Add( new StageDataRow("level0", "0", "4", "12", "0"));
			Rows.Add( new StageDataRow("level1", "1", "4", "12", "0"));
			Rows.Add( new StageDataRow("level2", "2", "5", "15", "0"));
			Rows.Add( new StageDataRow("level3", "3", "5", "15", "0"));
			Rows.Add( new StageDataRow("level4", "4", "5", "18", "0"));
			Rows.Add( new StageDataRow("level5", "5", "5", "12", "0"));
			Rows.Add( new StageDataRow("level6", "6", "6", "20", "0"));
			Rows.Add( new StageDataRow("level7", "7", "7", "21", "0"));
			Rows.Add( new StageDataRow("level8", "8", "8", "20", "0"));
			Rows.Add( new StageDataRow("level9", "9", "9", "23", "0"));
			Rows.Add( new StageDataRow("level10", "10", "9", "24", "0"));
			Rows.Add( new StageDataRow("level11", "11", "10", "26", "0"));
			Rows.Add( new StageDataRow("level12", "12", "10", "28", "0"));
			Rows.Add( new StageDataRow("level13", "13", "9", "32", "0"));
			Rows.Add( new StageDataRow("level14", "14", "10", "30", "0"));
			Rows.Add( new StageDataRow("level15", "15", "11", "32", "0"));
			Rows.Add( new StageDataRow("level16", "16", "8", "25", "0"));
			Rows.Add( new StageDataRow("level17", "17", "10", "35", "0"));
			Rows.Add( new StageDataRow("level18", "18", "11", "45", "0"));
			Rows.Add( new StageDataRow("level19", "19", "9", "46", "0"));
			Rows.Add( new StageDataRow("level20", "20", "11", "48", "0"));
			Rows.Add( new StageDataRow("level21", "21", "12", "40", "0"));
			Rows.Add( new StageDataRow("level22", "22", "9", "60", "0"));
			Rows.Add( new StageDataRow("level23", "23", "11", "55", "0"));
			Rows.Add( new StageDataRow("level24", "24", "13", "70", "0"));
			Rows.Add( new StageDataRow("level25", "25", "13", "80", "0"));
			Rows.Add( new StageDataRow("level26", "26", "9", "75", "0"));
			Rows.Add( new StageDataRow("level27", "27", "11", "75", "0"));
			Rows.Add( new StageDataRow("level28", "28", "12", "80", "0"));
			Rows.Add( new StageDataRow("level29", "29", "12", "75", "0"));
			Rows.Add( new StageDataRow("level30", "30", "10", "68", "0"));
			Rows.Add( new StageDataRow("level31", "31", "11", "80", "0"));
			Rows.Add( new StageDataRow("level32", "32", "13", "80", "0"));
			Rows.Add( new StageDataRow("level33", "33", "13", "85", "0"));
			Rows.Add( new StageDataRow("level34", "34", "12", "68", "0"));
			Rows.Add( new StageDataRow("level35", "35", "13", "70", "0"));
			Rows.Add( new StageDataRow("level36", "36", "11", "80", "0"));
			Rows.Add( new StageDataRow("level37", "37", "14", "88", "0"));
			Rows.Add( new StageDataRow("level38", "38", "15", "65", "0"));
			Rows.Add( new StageDataRow("level39", "39", "14", "70", "0"));
			Rows.Add( new StageDataRow("level40", "40", "12", "80", "0"));
			Rows.Add( new StageDataRow("level41", "41", "11", "90", "0"));
			Rows.Add( new StageDataRow("level42", "42", "9", "80", "0"));
			Rows.Add( new StageDataRow("level43", "43", "10", "80", "0"));
			Rows.Add( new StageDataRow("level44", "44", "12", "90", "0"));
			Rows.Add( new StageDataRow("level45", "45", "13", "95", "0"));
			Rows.Add( new StageDataRow("level46", "46", "9", "80", "0"));
			Rows.Add( new StageDataRow("level47", "47", "11", "90", "0"));
			Rows.Add( new StageDataRow("level48", "48", "14", "100", "0"));
			Rows.Add( new StageDataRow("level49", "49", "14", "90", "0"));
			Rows.Add( new StageDataRow("level50", "50", "12", "100", "0"));
			Rows.Add( new StageDataRow("level51", "51", "13", "90", "0"));
			Rows.Add( new StageDataRow("level52", "52", "14", "105", "0"));
			Rows.Add( new StageDataRow("level53", "53", "14", "102", "0"));
			Rows.Add( new StageDataRow("level54", "54", "15", "110", "0"));
			Rows.Add( new StageDataRow("level55", "55", "10", "70", "0"));
			Rows.Add( new StageDataRow("level56", "56", "12", "80", "0"));
			Rows.Add( new StageDataRow("level57", "57", "13", "90", "0"));
			Rows.Add( new StageDataRow("level58", "58", "13", "110", "0"));
			Rows.Add( new StageDataRow("level59", "59", "9", "88", "0"));
			Rows.Add( new StageDataRow("level60", "60", "11", "92", "0"));
			Rows.Add( new StageDataRow("level61", "61", "12", "100", "0"));
			Rows.Add( new StageDataRow("level62", "62", "12", "80", "0"));
			Rows.Add( new StageDataRow("level63", "63", "13", "99", "0"));
			Rows.Add( new StageDataRow("level64", "64", "13", "104", "0"));
			Rows.Add( new StageDataRow("level65", "65", "10", "88", "0"));
			Rows.Add( new StageDataRow("level66", "66", "12", "96", "0"));
			Rows.Add( new StageDataRow("level67", "67", "13", "99", "0"));
			Rows.Add( new StageDataRow("level68", "68", "13", "108", "0"));
			Rows.Add( new StageDataRow("level69", "69", "15", "115", "0"));
			Rows.Add( new StageDataRow("level70", "70", "10", "92", "0"));
			Rows.Add( new StageDataRow("level71", "71", "11", "96", "0"));
			Rows.Add( new StageDataRow("level72", "72", "13", "100", "0"));
			Rows.Add( new StageDataRow("level73", "73", "13", "110", "0"));
			Rows.Add( new StageDataRow("level74", "74", "14", "106", "0"));
			Rows.Add( new StageDataRow("level75", "75", "15", "114", "0"));
			Rows.Add( new StageDataRow("level76", "76", "12", "82", "0"));
			Rows.Add( new StageDataRow("level77", "77", "12", "88", "0"));
			Rows.Add( new StageDataRow("level78", "78", "13", "94", "0"));
			Rows.Add( new StageDataRow("level79", "79", "14", "100", "0"));
			Rows.Add( new StageDataRow("level80", "80", "9", "88", "0"));
			Rows.Add( new StageDataRow("level81", "81", "11", "92", "0"));
			Rows.Add( new StageDataRow("level82", "82", "13", "96", "0"));
			Rows.Add( new StageDataRow("level83", "83", "15", "100", "0"));
			Rows.Add( new StageDataRow("level84", "84", "16", "110", "0"));
			Rows.Add( new StageDataRow("level85", "85", "16", "120", "0"));
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
		public StageDataRow GetRow(rowIds in_RowID)
		{
			StageDataRow ret = null;
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
		public StageDataRow GetRow(string in_RowString)
		{
			StageDataRow ret = null;
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
