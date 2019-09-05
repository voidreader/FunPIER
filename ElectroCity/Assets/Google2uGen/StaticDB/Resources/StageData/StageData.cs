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
		public int _factor;
		public int _bossindex;
		public StageDataRow(string __ID, string __factor, string __bossindex) 
		{
			{
			int res;
				if(int.TryParse(__factor, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_factor = res;
				else
					Debug.LogError("Failed To Convert _factor string: "+ __factor +" to int");
			}
			{
			int res;
				if(int.TryParse(__bossindex, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_bossindex = res;
				else
					Debug.LogError("Failed To Convert _bossindex string: "+ __bossindex +" to int");
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
					ret = _factor.ToString();
					break;
				case 1:
					ret = _bossindex.ToString();
					break;
			}

			return ret;
		}

		public string GetStringData( string colID )
		{
			var ret = System.String.Empty;
			switch( colID )
			{
				case "factor":
					ret = _factor.ToString();
					break;
				case "bossindex":
					ret = _bossindex.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "factor" + " : " + _factor.ToString() + "} ";
			ret += "{" + "bossindex" + " : " + _bossindex.ToString() + "} ";
			return ret;
		}
	}
	public sealed class StageData : IGoogle2uDB
	{
		public enum rowIds {
			Stage1, Stage2, Stage3, Stage4, Stage5, Stage6, Stage7, Stage8, Stage9, Stage10
		};
		public string [] rowNames = {
			"Stage1", "Stage2", "Stage3", "Stage4", "Stage5", "Stage6", "Stage7", "Stage8", "Stage9", "Stage10"
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
			Rows.Add( new StageDataRow("Stage1", "1", "0"));
			Rows.Add( new StageDataRow("Stage2", "3", "1"));
			Rows.Add( new StageDataRow("Stage3", "5", "2"));
			Rows.Add( new StageDataRow("Stage4", "10", "3"));
			Rows.Add( new StageDataRow("Stage5", "15", "4"));
			Rows.Add( new StageDataRow("Stage6", "21", "5"));
			Rows.Add( new StageDataRow("Stage7", "25", "6"));
			Rows.Add( new StageDataRow("Stage8", "30", "7"));
			Rows.Add( new StageDataRow("Stage9", "45", "8"));
			Rows.Add( new StageDataRow("Stage10", "50", "9"));
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
