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
			, level38, level39, level40, level41, level42, level43
		};
		public string [] rowNames = {
			"level0", "level1", "level2", "level3", "level4", "level5", "level6", "level7", "level8", "level9", "level10", "level11", "level12", "level13", "level14", "level15", "level16", "level17"
			, "level18", "level19", "level20", "level21", "level22", "level23", "level24", "level25", "level26", "level27", "level28", "level29", "level30", "level31", "level32", "level33", "level34", "level35", "level36", "level37"
			, "level38", "level39", "level40", "level41", "level42", "level43"
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
			Rows.Add( new StageDataRow("level13", "13", "9", "320", "0"));
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
			Rows.Add( new StageDataRow("level34", "34", "5", "68", "0"));
			Rows.Add( new StageDataRow("level35", "35", "5", "70", "0"));
			Rows.Add( new StageDataRow("level36", "36", "5", "15", "0"));
			Rows.Add( new StageDataRow("level37", "37", "5", "15", "0"));
			Rows.Add( new StageDataRow("level38", "38", "5", "15", "0"));
			Rows.Add( new StageDataRow("level39", "39", "5", "15", "0"));
			Rows.Add( new StageDataRow("level40", "40", "5", "15", "0"));
			Rows.Add( new StageDataRow("level41", "41", "5", "15", "0"));
			Rows.Add( new StageDataRow("level42", "42", "5", "15", "0"));
			Rows.Add( new StageDataRow("level43", "43", "5", "15", "0"));
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
