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
		public string _stage;
		public int _factor;
		public int _bid;
		public StageDataRow(string __ID, string __stage, string __factor, string __bid) 
		{
			_stage = __stage.Trim();
			{
			int res;
				if(int.TryParse(__factor, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_factor = res;
				else
					Debug.LogError("Failed To Convert _factor string: "+ __factor +" to int");
			}
			{
			int res;
				if(int.TryParse(__bid, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_bid = res;
				else
					Debug.LogError("Failed To Convert _bid string: "+ __bid +" to int");
			}
		}

		public int Length { get { return 3; } }

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
					ret = _stage.ToString();
					break;
				case 1:
					ret = _factor.ToString();
					break;
				case 2:
					ret = _bid.ToString();
					break;
			}

			return ret;
		}

		public string GetStringData( string colID )
		{
			var ret = System.String.Empty;
			switch( colID )
			{
				case "stage":
					ret = _stage.ToString();
					break;
				case "factor":
					ret = _factor.ToString();
					break;
				case "bid":
					ret = _bid.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "stage" + " : " + _stage.ToString() + "} ";
			ret += "{" + "factor" + " : " + _factor.ToString() + "} ";
			ret += "{" + "bid" + " : " + _bid.ToString() + "} ";
			return ret;
		}
	}
	public sealed class StageData : IGoogle2uDB
	{
		public enum rowIds {
			Stage1, Stage2, Stage3, Stage4, Stage5, Stage6, Stage7, Stage8, Stage9, Stage10, Stage11, Stage12
		};
		public string [] rowNames = {
			"Stage1", "Stage2", "Stage3", "Stage4", "Stage5", "Stage6", "Stage7", "Stage8", "Stage9", "Stage10", "Stage11", "Stage12"
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
			Rows.Add( new StageDataRow("Stage1", "1", "1", "1"));
			Rows.Add( new StageDataRow("Stage2", "2", "3", "2"));
			Rows.Add( new StageDataRow("Stage3", "3", "5", "3"));
			Rows.Add( new StageDataRow("Stage4", "4", "10", "4"));
			Rows.Add( new StageDataRow("Stage5", "5", "15", "5"));
			Rows.Add( new StageDataRow("Stage6", "6", "21", "6"));
			Rows.Add( new StageDataRow("Stage7", "7", "25", "7"));
			Rows.Add( new StageDataRow("Stage8", "8", "30", "8"));
			Rows.Add( new StageDataRow("Stage9", "9", "45", "9"));
			Rows.Add( new StageDataRow("Stage10", "10", "50", "10"));
			Rows.Add( new StageDataRow("Stage11", "11", "60", "11"));
			Rows.Add( new StageDataRow("Stage12", "12", "80", "12"));
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
