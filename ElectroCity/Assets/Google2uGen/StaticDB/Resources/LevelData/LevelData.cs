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
		public LevelDataRow(string __ID, string __level, string __needexp) 
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
		}

		public int Length { get { return 2; } }

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
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "level" + " : " + _level.ToString() + "} ";
			ret += "{" + "needexp" + " : " + _needexp.ToString() + "} ";
			return ret;
		}
	}
	public sealed class LevelData : IGoogle2uDB
	{
		public enum rowIds {
			Level1, Level2, Level3, Level4, Level5, Level6, Level7, Level8, Level9, Level10, Level11, Level12, Level13, Level14, Level15, Level16, Level17, Level18
			, Level19, Level20, Level21, Level22, Level23, Level24, Level25, Level26, Level27, Level28, Level29, Level30
		};
		public string [] rowNames = {
			"Level1", "Level2", "Level3", "Level4", "Level5", "Level6", "Level7", "Level8", "Level9", "Level10", "Level11", "Level12", "Level13", "Level14", "Level15", "Level16", "Level17", "Level18"
			, "Level19", "Level20", "Level21", "Level22", "Level23", "Level24", "Level25", "Level26", "Level27", "Level28", "Level29", "Level30"
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
			Rows.Add( new LevelDataRow("Level1", "1", "15"));
			Rows.Add( new LevelDataRow("Level2", "2", "30"));
			Rows.Add( new LevelDataRow("Level3", "3", "50"));
			Rows.Add( new LevelDataRow("Level4", "4", "100"));
			Rows.Add( new LevelDataRow("Level5", "5", "120"));
			Rows.Add( new LevelDataRow("Level6", "6", "150"));
			Rows.Add( new LevelDataRow("Level7", "7", "180"));
			Rows.Add( new LevelDataRow("Level8", "8", "240"));
			Rows.Add( new LevelDataRow("Level9", "9", "300"));
			Rows.Add( new LevelDataRow("Level10", "10", "330"));
			Rows.Add( new LevelDataRow("Level11", "11", "400"));
			Rows.Add( new LevelDataRow("Level12", "12", "450"));
			Rows.Add( new LevelDataRow("Level13", "13", "500"));
			Rows.Add( new LevelDataRow("Level14", "14", "550"));
			Rows.Add( new LevelDataRow("Level15", "15", "600"));
			Rows.Add( new LevelDataRow("Level16", "16", "650"));
			Rows.Add( new LevelDataRow("Level17", "17", "700"));
			Rows.Add( new LevelDataRow("Level18", "18", "720"));
			Rows.Add( new LevelDataRow("Level19", "19", "780"));
			Rows.Add( new LevelDataRow("Level20", "20", "850"));
			Rows.Add( new LevelDataRow("Level21", "21", "900"));
			Rows.Add( new LevelDataRow("Level22", "22", "920"));
			Rows.Add( new LevelDataRow("Level23", "23", "940"));
			Rows.Add( new LevelDataRow("Level24", "24", "960"));
			Rows.Add( new LevelDataRow("Level25", "25", "980"));
			Rows.Add( new LevelDataRow("Level26", "26", "1000"));
			Rows.Add( new LevelDataRow("Level27", "27", "1050"));
			Rows.Add( new LevelDataRow("Level28", "28", "1100"));
			Rows.Add( new LevelDataRow("Level29", "29", "1150"));
			Rows.Add( new LevelDataRow("Level30", "30", "1200"));
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
