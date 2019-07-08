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
	public class WantedRewardDataRow : IGoogle2uRow
	{
		public int _list;
		public string _weaponid;
		public WantedRewardDataRow(string __ID, string __list, string __weaponid) 
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
	public sealed class WantedRewardData : IGoogle2uDB
	{
		public enum rowIds {
			reward0, reward1, reward2, reward3, reward4, reward5, reward6, reward7, reward8
		};
		public string [] rowNames = {
			"reward0", "reward1", "reward2", "reward3", "reward4", "reward5", "reward6", "reward7", "reward8"
		};
		public System.Collections.Generic.List<WantedRewardDataRow> Rows = new System.Collections.Generic.List<WantedRewardDataRow>();

		public static WantedRewardData Instance
		{
			get { return NestedWantedRewardData.instance; }
		}

		private class NestedWantedRewardData
		{
			static NestedWantedRewardData() { }
			internal static readonly WantedRewardData instance = new WantedRewardData();
		}

		private WantedRewardData()
		{
			Rows.Add( new WantedRewardDataRow("reward0", "0", "broom"));
			Rows.Add( new WantedRewardDataRow("reward1", "1", "bubble"));
			Rows.Add( new WantedRewardDataRow("reward2", "2", "bluedragon"));
			Rows.Add( new WantedRewardDataRow("reward3", "3", "bat"));
			Rows.Add( new WantedRewardDataRow("reward4", "4", "chicken"));
			Rows.Add( new WantedRewardDataRow("reward5", "5", "punch"));
			Rows.Add( new WantedRewardDataRow("reward6", "6", "dryer"));
			Rows.Add( new WantedRewardDataRow("reward7", "7", "ozone"));
			Rows.Add( new WantedRewardDataRow("reward8", "8", "kitchenglove"));
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
		public WantedRewardDataRow GetRow(rowIds in_RowID)
		{
			WantedRewardDataRow ret = null;
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
		public WantedRewardDataRow GetRow(string in_RowString)
		{
			WantedRewardDataRow ret = null;
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
