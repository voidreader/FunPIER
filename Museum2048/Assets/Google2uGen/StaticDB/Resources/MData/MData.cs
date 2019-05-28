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
	public class MDataRow : IGoogle2uRow
	{
		public int _Step;
		public MDataRow(string __ID, string __Step) 
		{
			{
			int res;
				if(int.TryParse(__Step, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_Step = res;
				else
					Debug.LogError("Failed To Convert _Step string: "+ __Step +" to int");
			}
		}

		public int Length { get { return 1; } }

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
					ret = _Step.ToString();
					break;
			}

			return ret;
		}

		public string GetStringData( string colID )
		{
			var ret = System.String.Empty;
			switch( colID )
			{
				case "Step":
					ret = _Step.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "Step" + " : " + _Step.ToString() + "} ";
			return ret;
		}
	}
	public sealed class MData : IGoogle2uDB
	{
		public enum rowIds {
			CAR_MUSEUM, WINE_MUSEUM, VIKING_MUSEUM, ICE_MUSEUM
		};
		public string [] rowNames = {
			"CAR_MUSEUM", "WINE_MUSEUM", "VIKING_MUSEUM", "ICE_MUSEUM"
		};
		public System.Collections.Generic.List<MDataRow> Rows = new System.Collections.Generic.List<MDataRow>();

		public static MData Instance
		{
			get { return NestedMData.instance; }
		}

		private class NestedMData
		{
			static NestedMData() { }
			internal static readonly MData instance = new MData();
		}

		private MData()
		{
			Rows.Add( new MDataRow("CAR_MUSEUM", "9"));
			Rows.Add( new MDataRow("WINE_MUSEUM", "11"));
			Rows.Add( new MDataRow("VIKING_MUSEUM", "12"));
			Rows.Add( new MDataRow("ICE_MUSEUM", "14"));
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
		public MDataRow GetRow(rowIds in_RowID)
		{
			MDataRow ret = null;
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
		public MDataRow GetRow(string in_RowString)
		{
			MDataRow ret = null;
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
