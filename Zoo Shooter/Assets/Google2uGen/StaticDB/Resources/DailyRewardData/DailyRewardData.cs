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
	public class DailyRewardDataRow : IGoogle2uRow
	{
		public int _list;
		public string _weaponid;
		public DailyRewardDataRow(string __ID, string __list, string __weaponid) 
		{
			{
			int res;
				if(int.TryParse(__list, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_list = res;
				else
					Debug.LogError("Failed To Convert _list string: "+ __list +" to int");
			}
			_weaponid = __weaponid.Trim();
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
					ret = _list.ToString();
					break;
				case 1:
					ret = _weaponid.ToString();
					break;
			}

			return ret;
		}

		public string GetStringData( string colID )
		{
			var ret = System.String.Empty;
			switch( colID )
			{
				case "list":
					ret = _list.ToString();
					break;
				case "weaponid":
					ret = _weaponid.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "list" + " : " + _list.ToString() + "} ";
			ret += "{" + "weaponid" + " : " + _weaponid.ToString() + "} ";
			return ret;
		}
	}
	public sealed class DailyRewardData : IGoogle2uDB
	{
		public enum rowIds {
			reward0, reward1, reward2, reward3
		};
		public string [] rowNames = {
			"reward0", "reward1", "reward2", "reward3"
		};
		public System.Collections.Generic.List<DailyRewardDataRow> Rows = new System.Collections.Generic.List<DailyRewardDataRow>();

		public static DailyRewardData Instance
		{
			get { return NestedDailyRewardData.instance; }
		}

		private class NestedDailyRewardData
		{
			static NestedDailyRewardData() { }
			internal static readonly DailyRewardData instance = new DailyRewardData();
		}

		private DailyRewardData()
		{
			Rows.Add( new DailyRewardDataRow("reward0", "0", "cluck"));
			Rows.Add( new DailyRewardDataRow("reward1", "1", "kitchenknife"));
			Rows.Add( new DailyRewardDataRow("reward2", "2", "musket"));
			Rows.Add( new DailyRewardDataRow("reward3", "3", "Airpod"));
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
		public DailyRewardDataRow GetRow(rowIds in_RowID)
		{
			DailyRewardDataRow ret = null;
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
		public DailyRewardDataRow GetRow(string in_RowString)
		{
			DailyRewardDataRow ret = null;
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
