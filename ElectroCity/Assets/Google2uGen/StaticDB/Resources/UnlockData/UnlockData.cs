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
	public class UnlockDataRow : IGoogle2uRow
	{
		public int _level;
		public int _new;
		public int _coinlimit;
		public int _adlimit;
		public int _gemlimit;
		public int _quick;
		public int _boxlimit;
		public int _sbox;
		public UnlockDataRow(string __ID, string __level, string __new, string __coinlimit, string __adlimit, string __gemlimit, string __quick, string __boxlimit, string __sbox) 
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
				if(int.TryParse(__new, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_new = res;
				else
					Debug.LogError("Failed To Convert _new string: "+ __new +" to int");
			}
			{
			int res;
				if(int.TryParse(__coinlimit, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_coinlimit = res;
				else
					Debug.LogError("Failed To Convert _coinlimit string: "+ __coinlimit +" to int");
			}
			{
			int res;
				if(int.TryParse(__adlimit, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_adlimit = res;
				else
					Debug.LogError("Failed To Convert _adlimit string: "+ __adlimit +" to int");
			}
			{
			int res;
				if(int.TryParse(__gemlimit, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_gemlimit = res;
				else
					Debug.LogError("Failed To Convert _gemlimit string: "+ __gemlimit +" to int");
			}
			{
			int res;
				if(int.TryParse(__quick, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_quick = res;
				else
					Debug.LogError("Failed To Convert _quick string: "+ __quick +" to int");
			}
			{
			int res;
				if(int.TryParse(__boxlimit, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_boxlimit = res;
				else
					Debug.LogError("Failed To Convert _boxlimit string: "+ __boxlimit +" to int");
			}
			{
			int res;
				if(int.TryParse(__sbox, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_sbox = res;
				else
					Debug.LogError("Failed To Convert _sbox string: "+ __sbox +" to int");
			}
		}

		public int Length { get { return 8; } }

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
					ret = _new.ToString();
					break;
				case 2:
					ret = _coinlimit.ToString();
					break;
				case 3:
					ret = _adlimit.ToString();
					break;
				case 4:
					ret = _gemlimit.ToString();
					break;
				case 5:
					ret = _quick.ToString();
					break;
				case 6:
					ret = _boxlimit.ToString();
					break;
				case 7:
					ret = _sbox.ToString();
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
				case "new":
					ret = _new.ToString();
					break;
				case "coinlimit":
					ret = _coinlimit.ToString();
					break;
				case "adlimit":
					ret = _adlimit.ToString();
					break;
				case "gemlimit":
					ret = _gemlimit.ToString();
					break;
				case "quick":
					ret = _quick.ToString();
					break;
				case "boxlimit":
					ret = _boxlimit.ToString();
					break;
				case "sbox":
					ret = _sbox.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "level" + " : " + _level.ToString() + "} ";
			ret += "{" + "new" + " : " + _new.ToString() + "} ";
			ret += "{" + "coinlimit" + " : " + _coinlimit.ToString() + "} ";
			ret += "{" + "adlimit" + " : " + _adlimit.ToString() + "} ";
			ret += "{" + "gemlimit" + " : " + _gemlimit.ToString() + "} ";
			ret += "{" + "quick" + " : " + _quick.ToString() + "} ";
			ret += "{" + "boxlimit" + " : " + _boxlimit.ToString() + "} ";
			ret += "{" + "sbox" + " : " + _sbox.ToString() + "} ";
			return ret;
		}
	}
	public sealed class UnlockData : IGoogle2uDB
	{
		public enum rowIds {
			unlock1, unlock2, unlock3, unlock4, unlock5, unlock6, unlock7, unlock8, unlock9, unlock10, unlock11, unlock12, unlock13, unlock14, unlock15, unlock16, unlock17, unlock18
			, unlock19, unlock20, unlock21, unlock22, unlock23, unlock24, unlock25, unlock26, unlock27, unlock28, unlock29, unlock30, unlock31, unlock32, unlock33, unlock34, unlock35, unlock36, unlock37, unlock38
			, unlock39, unlock40
		};
		public string [] rowNames = {
			"unlock1", "unlock2", "unlock3", "unlock4", "unlock5", "unlock6", "unlock7", "unlock8", "unlock9", "unlock10", "unlock11", "unlock12", "unlock13", "unlock14", "unlock15", "unlock16", "unlock17", "unlock18"
			, "unlock19", "unlock20", "unlock21", "unlock22", "unlock23", "unlock24", "unlock25", "unlock26", "unlock27", "unlock28", "unlock29", "unlock30", "unlock31", "unlock32", "unlock33", "unlock34", "unlock35", "unlock36", "unlock37", "unlock38"
			, "unlock39", "unlock40"
		};
		public System.Collections.Generic.List<UnlockDataRow> Rows = new System.Collections.Generic.List<UnlockDataRow>();

		public static UnlockData Instance
		{
			get { return NestedUnlockData.instance; }
		}

		private class NestedUnlockData
		{
			static NestedUnlockData() { }
			internal static readonly UnlockData instance = new UnlockData();
		}

		private UnlockData()
		{
			Rows.Add( new UnlockDataRow("unlock1", "1", "0", "1", "0", "0", "1", "1", "0"));
			Rows.Add( new UnlockDataRow("unlock2", "2", "0", "2", "0", "0", "1", "1", "0"));
			Rows.Add( new UnlockDataRow("unlock3", "3", "0", "3", "0", "0", "1", "1", "0"));
			Rows.Add( new UnlockDataRow("unlock4", "4", "0", "3", "0", "0", "1", "1", "0"));
			Rows.Add( new UnlockDataRow("unlock5", "5", "0", "3", "0", "0", "2", "3", "0"));
			Rows.Add( new UnlockDataRow("unlock6", "6", "4", "3", "0", "4", "2", "3", "0"));
			Rows.Add( new UnlockDataRow("unlock7", "7", "5", "3", "0", "5", "3", "4", "0"));
			Rows.Add( new UnlockDataRow("unlock8", "8", "6", "4", "0", "6", "3", "5", "5"));
			Rows.Add( new UnlockDataRow("unlock9", "9", "7", "5", "5", "7", "4", "5", "6"));
			Rows.Add( new UnlockDataRow("unlock10", "10", "8", "6", "6", "8", "4", "6", "7"));
			Rows.Add( new UnlockDataRow("unlock11", "11", "9", "7", "7", "9", "5", "6", "8"));
			Rows.Add( new UnlockDataRow("unlock12", "12", "10", "8", "8", "10", "5", "7", "9"));
			Rows.Add( new UnlockDataRow("unlock13", "13", "11", "9", "9", "11", "6", "7", "10"));
			Rows.Add( new UnlockDataRow("unlock14", "14", "12", "10", "10", "12", "7", "8", "11"));
			Rows.Add( new UnlockDataRow("unlock15", "15", "13", "11", "11", "13", "8", "8", "12"));
			Rows.Add( new UnlockDataRow("unlock16", "16", "14", "12", "12", "14", "9", "9", "13"));
			Rows.Add( new UnlockDataRow("unlock17", "17", "15", "13", "13", "15", "10", "10", "14"));
			Rows.Add( new UnlockDataRow("unlock18", "18", "16", "14", "14", "16", "10", "11", "15"));
			Rows.Add( new UnlockDataRow("unlock19", "19", "17", "15", "15", "17", "11", "12", "16"));
			Rows.Add( new UnlockDataRow("unlock20", "20", "18", "16", "16", "18", "12", "13", "17"));
			Rows.Add( new UnlockDataRow("unlock21", "21", "19", "17", "17", "19", "13", "14", "18"));
			Rows.Add( new UnlockDataRow("unlock22", "22", "20", "18", "18", "20", "14", "14", "19"));
			Rows.Add( new UnlockDataRow("unlock23", "23", "21", "19", "19", "21", "14", "15", "20"));
			Rows.Add( new UnlockDataRow("unlock24", "24", "22", "20", "20", "22", "15", "15", "21"));
			Rows.Add( new UnlockDataRow("unlock25", "25", "23", "21", "21", "23", "15", "16", "22"));
			Rows.Add( new UnlockDataRow("unlock26", "26", "24", "22", "22", "24", "16", "17", "23"));
			Rows.Add( new UnlockDataRow("unlock27", "27", "25", "23", "23", "25", "17", "18", "24"));
			Rows.Add( new UnlockDataRow("unlock28", "28", "26", "24", "24", "26", "18", "19", "25"));
			Rows.Add( new UnlockDataRow("unlock29", "29", "27", "25", "25", "27", "19", "20", "26"));
			Rows.Add( new UnlockDataRow("unlock30", "30", "28", "26", "26", "28", "20", "21", "27"));
			Rows.Add( new UnlockDataRow("unlock31", "31", "29", "27", "27", "29", "21", "22", "28"));
			Rows.Add( new UnlockDataRow("unlock32", "32", "30", "28", "28", "30", "22", "23", "29"));
			Rows.Add( new UnlockDataRow("unlock33", "33", "31", "29", "29", "31", "23", "24", "30"));
			Rows.Add( new UnlockDataRow("unlock34", "34", "32", "30", "30", "32", "24", "25", "31"));
			Rows.Add( new UnlockDataRow("unlock35", "35", "33", "31", "31", "33", "25", "26", "32"));
			Rows.Add( new UnlockDataRow("unlock36", "36", "34", "32", "32", "34", "26", "27", "33"));
			Rows.Add( new UnlockDataRow("unlock37", "37", "35", "33", "33", "35", "27", "28", "34"));
			Rows.Add( new UnlockDataRow("unlock38", "38", "36", "34", "34", "36", "28", "29", "35"));
			Rows.Add( new UnlockDataRow("unlock39", "39", "37", "35", "35", "37", "29", "30", "36"));
			Rows.Add( new UnlockDataRow("unlock40", "40", "38", "36", "36", "38", "30", "31", "37"));
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
		public UnlockDataRow GetRow(rowIds in_RowID)
		{
			UnlockDataRow ret = null;
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
		public UnlockDataRow GetRow(string in_RowString)
		{
			UnlockDataRow ret = null;
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
