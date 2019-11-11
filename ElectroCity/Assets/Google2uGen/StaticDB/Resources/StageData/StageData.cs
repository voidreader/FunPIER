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
		public string _displayname;
		public string _needcoin;
		public int _coinfactor;
		public StageDataRow(string __ID, string __stage, string __factor, string __displayname, string __needcoin, string __coinfactor) 
		{
			_stage = __stage.Trim();
			{
			int res;
				if(int.TryParse(__factor, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_factor = res;
				else
					Debug.LogError("Failed To Convert _factor string: "+ __factor +" to int");
			}
			_displayname = __displayname.Trim();
			_needcoin = __needcoin.Trim();
			{
			int res;
				if(int.TryParse(__coinfactor, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_coinfactor = res;
				else
					Debug.LogError("Failed To Convert _coinfactor string: "+ __coinfactor +" to int");
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
					ret = _stage.ToString();
					break;
				case 1:
					ret = _factor.ToString();
					break;
				case 2:
					ret = _displayname.ToString();
					break;
				case 3:
					ret = _needcoin.ToString();
					break;
				case 4:
					ret = _coinfactor.ToString();
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
				case "displayname":
					ret = _displayname.ToString();
					break;
				case "needcoin":
					ret = _needcoin.ToString();
					break;
				case "coinfactor":
					ret = _coinfactor.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "stage" + " : " + _stage.ToString() + "} ";
			ret += "{" + "factor" + " : " + _factor.ToString() + "} ";
			ret += "{" + "displayname" + " : " + _displayname.ToString() + "} ";
			ret += "{" + "needcoin" + " : " + _needcoin.ToString() + "} ";
			ret += "{" + "coinfactor" + " : " + _coinfactor.ToString() + "} ";
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
			Rows.Add( new StageDataRow("Stage1", "1", "1", "AREA #1", "50000", "2"));
			Rows.Add( new StageDataRow("Stage2", "2", "5", "AREA #2", "250000", "4"));
			Rows.Add( new StageDataRow("Stage3", "3", "20", "AREA #3", "800000", "8"));
			Rows.Add( new StageDataRow("Stage4", "4", "60", "AREA #4", "2500000", "16"));
			Rows.Add( new StageDataRow("Stage5", "5", "150", "AREA #5", "12000000", "32"));
			Rows.Add( new StageDataRow("Stage6", "6", "500", "AREA #6", "80000000", "64"));
			Rows.Add( new StageDataRow("Stage7", "7", "1000", "AREA #7", "500000000", "128"));
			Rows.Add( new StageDataRow("Stage8", "8", "1800", "AREA #8", "5000000000", "256"));
			Rows.Add( new StageDataRow("Stage9", "9", "5000", "AREA #9", "50000000000", "512"));
			Rows.Add( new StageDataRow("Stage10", "10", "12000", "AREA #10", "120000000000", "1024"));
			Rows.Add( new StageDataRow("Stage11", "11", "40000", "AREA #11", "250000000000", "2048"));
			Rows.Add( new StageDataRow("Stage12", "12", "150000", "AREA #12", "950000000000", "4196"));
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
