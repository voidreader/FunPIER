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
	public class PassiveDataRow : IGoogle2uRow
	{
		public string _price;
		public string _factor;
		public string _rid;
		public PassiveDataRow(string __ID, string __price, string __factor, string __rid) 
		{
			_price = __price.Trim();
			_factor = __factor.Trim();
			_rid = __rid.Trim();
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
					ret = _price.ToString();
					break;
				case 1:
					ret = _factor.ToString();
					break;
				case 2:
					ret = _rid.ToString();
					break;
			}

			return ret;
		}

		public string GetStringData( string colID )
		{
			var ret = System.String.Empty;
			switch( colID )
			{
				case "price":
					ret = _price.ToString();
					break;
				case "factor":
					ret = _factor.ToString();
					break;
				case "rid":
					ret = _rid.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "price" + " : " + _price.ToString() + "} ";
			ret += "{" + "factor" + " : " + _factor.ToString() + "} ";
			ret += "{" + "rid" + " : " + _rid.ToString() + "} ";
			return ret;
		}
	}
	public sealed class PassiveData : IGoogle2uDB
	{
		public enum rowIds {
			TYPE, DAMAGE1, DAMAGE2, DAMAGE3, DAMAGE4, DAMAGE5, DAMAGE6, DAMAGE7, DAMAGE8, DAMAGE9, DAMAGE10, DISCOUNT1, DISCOUNT2, DISCOUNT3, DISCOUNT4, DISCOUNT5, DISCOUNT6, DISCOUNT7, DISCOUNT8
			, DISCOUNT9, DISCOUNT10
		};
		public string [] rowNames = {
			"TYPE", "DAMAGE1", "DAMAGE2", "DAMAGE3", "DAMAGE4", "DAMAGE5", "DAMAGE6", "DAMAGE7", "DAMAGE8", "DAMAGE9", "DAMAGE10", "DISCOUNT1", "DISCOUNT2", "DISCOUNT3", "DISCOUNT4", "DISCOUNT5", "DISCOUNT6", "DISCOUNT7", "DISCOUNT8"
			, "DISCOUNT9", "DISCOUNT10"
		};
		public System.Collections.Generic.List<PassiveDataRow> Rows = new System.Collections.Generic.List<PassiveDataRow>();

		public static PassiveData Instance
		{
			get { return NestedPassiveData.instance; }
		}

		private class NestedPassiveData
		{
			static NestedPassiveData() { }
			internal static readonly PassiveData instance = new PassiveData();
		}

		private PassiveData()
		{
			Rows.Add( new PassiveDataRow("TYPE", "long", "int", "string"));
			Rows.Add( new PassiveDataRow("DAMAGE1", "50000", "5", "DAMAGE1"));
			Rows.Add( new PassiveDataRow("DAMAGE2", "700000", "10", "DAMAGE2"));
			Rows.Add( new PassiveDataRow("DAMAGE3", "7000000", "20", "DAMAGE3"));
			Rows.Add( new PassiveDataRow("DAMAGE4", "350000000", "40", "DAMAGE4"));
			Rows.Add( new PassiveDataRow("DAMAGE5", "10000000000", "55", "DAMAGE5"));
			Rows.Add( new PassiveDataRow("DAMAGE6", "95000000000", "70", "DAMAGE6"));
			Rows.Add( new PassiveDataRow("DAMAGE7", "100", "80", "DAMAGE7"));
			Rows.Add( new PassiveDataRow("DAMAGE8", "100", "90", "DAMAGE8"));
			Rows.Add( new PassiveDataRow("DAMAGE9", "100", "95", "DAMAGE9"));
			Rows.Add( new PassiveDataRow("DAMAGE10", "100", "100", "DAMAGE10"));
			Rows.Add( new PassiveDataRow("DISCOUNT1", "75000", "5", "DISCOUNT1"));
			Rows.Add( new PassiveDataRow("DISCOUNT2", "1000000", "10", "DISCOUNT2"));
			Rows.Add( new PassiveDataRow("DISCOUNT3", "12000000", "15", "DISCOUNT3"));
			Rows.Add( new PassiveDataRow("DISCOUNT4", "520000000", "20", "DISCOUNT4"));
			Rows.Add( new PassiveDataRow("DISCOUNT5", "14000000000", "25", "DISCOUNT5"));
			Rows.Add( new PassiveDataRow("DISCOUNT6", "140000000000", "30", "DISCOUNT6"));
			Rows.Add( new PassiveDataRow("DISCOUNT7", "100", "35", "DISCOUNT7"));
			Rows.Add( new PassiveDataRow("DISCOUNT8", "100", "40", "DISCOUNT8"));
			Rows.Add( new PassiveDataRow("DISCOUNT9", "100", "45", "DISCOUNT9"));
			Rows.Add( new PassiveDataRow("DISCOUNT10", "100", "50", "DISCOUNT10"));
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
		public PassiveDataRow GetRow(rowIds in_RowID)
		{
			PassiveDataRow ret = null;
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
		public PassiveDataRow GetRow(string in_RowString)
		{
			PassiveDataRow ret = null;
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
