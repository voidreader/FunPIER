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
	public class UnitDataRow : IGoogle2uRow
	{
		public string _uid;
		public int _level;
		public string _displayname;
		public string _price;
		public float _damage;
		public float _firerate;
		public string _earning;
		public float _factor;
		public UnitDataRow(string __ID, string __uid, string __level, string __displayname, string __price, string __damage, string __firerate, string __earning, string __factor) 
		{
			_uid = __uid.Trim();
			{
			int res;
				if(int.TryParse(__level, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_level = res;
				else
					Debug.LogError("Failed To Convert _level string: "+ __level +" to int");
			}
			_displayname = __displayname.Trim();
			_price = __price.Trim();
			{
			float res;
				if(float.TryParse(__damage, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_damage = res;
				else
					Debug.LogError("Failed To Convert _damage string: "+ __damage +" to float");
			}
			{
			float res;
				if(float.TryParse(__firerate, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_firerate = res;
				else
					Debug.LogError("Failed To Convert _firerate string: "+ __firerate +" to float");
			}
			_earning = __earning.Trim();
			{
			float res;
				if(float.TryParse(__factor, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_factor = res;
				else
					Debug.LogError("Failed To Convert _factor string: "+ __factor +" to float");
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
					ret = _uid.ToString();
					break;
				case 1:
					ret = _level.ToString();
					break;
				case 2:
					ret = _displayname.ToString();
					break;
				case 3:
					ret = _price.ToString();
					break;
				case 4:
					ret = _damage.ToString();
					break;
				case 5:
					ret = _firerate.ToString();
					break;
				case 6:
					ret = _earning.ToString();
					break;
				case 7:
					ret = _factor.ToString();
					break;
			}

			return ret;
		}

		public string GetStringData( string colID )
		{
			var ret = System.String.Empty;
			switch( colID )
			{
				case "uid":
					ret = _uid.ToString();
					break;
				case "level":
					ret = _level.ToString();
					break;
				case "displayname":
					ret = _displayname.ToString();
					break;
				case "price":
					ret = _price.ToString();
					break;
				case "damage":
					ret = _damage.ToString();
					break;
				case "firerate":
					ret = _firerate.ToString();
					break;
				case "earning":
					ret = _earning.ToString();
					break;
				case "factor":
					ret = _factor.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "uid" + " : " + _uid.ToString() + "} ";
			ret += "{" + "level" + " : " + _level.ToString() + "} ";
			ret += "{" + "displayname" + " : " + _displayname.ToString() + "} ";
			ret += "{" + "price" + " : " + _price.ToString() + "} ";
			ret += "{" + "damage" + " : " + _damage.ToString() + "} ";
			ret += "{" + "firerate" + " : " + _firerate.ToString() + "} ";
			ret += "{" + "earning" + " : " + _earning.ToString() + "} ";
			ret += "{" + "factor" + " : " + _factor.ToString() + "} ";
			return ret;
		}
	}
	public sealed class UnitData : IGoogle2uDB
	{
		public enum rowIds {
			U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, U16
		};
		public string [] rowNames = {
			"U1", "U2", "U3", "U4", "U5", "U6", "U7", "U8", "U9", "U10", "U11", "U12", "U13", "U14", "U15", "U16"
		};
		public System.Collections.Generic.List<UnitDataRow> Rows = new System.Collections.Generic.List<UnitDataRow>();

		public static UnitData Instance
		{
			get { return NestedUnitData.instance; }
		}

		private class NestedUnitData
		{
			static NestedUnitData() { }
			internal static readonly UnitData instance = new UnitData();
		}

		private UnitData()
		{
			Rows.Add( new UnitDataRow("U1", "U1", "1", "unit1", "120", "15", "1", "5", "1"));
			Rows.Add( new UnitDataRow("U2", "U2", "2", "unit2", "1800", "22", "2", "11", "2"));
			Rows.Add( new UnitDataRow("U3", "U3", "3", "unit3", "5700", "6", "3", "24", "3"));
			Rows.Add( new UnitDataRow("U4", "U4", "4", "unit4", "16000", "30", "4", "53", "4"));
			Rows.Add( new UnitDataRow("U5", "U5", "5", "unit5", "55000", "8", "5", "117", "5"));
			Rows.Add( new UnitDataRow("U6", "U6", "6", "unit6", "171600", "6", "6", "257", "6"));
			Rows.Add( new UnitDataRow("U7", "U7", "7", "unit7", "530000", "88", "7", "566", "7"));
			Rows.Add( new UnitDataRow("U8", "U8", "8", "unit8", "1650000", "60", "8", "1247", "8"));
			Rows.Add( new UnitDataRow("U9", "U9", "9", "unit9", "5115000", "25", "9", "2743", "9"));
			Rows.Add( new UnitDataRow("U10", "U10", "10", "unit10", "15856500", "33", "10", "6036", "10"));
			Rows.Add( new UnitDataRow("U11", "U11", "11", "unit11", "49155150", "44", "11", "13279", "11"));
			Rows.Add( new UnitDataRow("U12", "U12", "12", "unit12", "152,380,965", "66", "12", "29215", "12"));
			Rows.Add( new UnitDataRow("U13", "U13", "13", "unit13", "472,380,992", "70", "13", "64275", "13"));
			Rows.Add( new UnitDataRow("U14", "U14", "14", "unit14", "1,464,381,073", "80", "14", "141405", "14"));
			Rows.Add( new UnitDataRow("U15", "U15", "15", "unit15", "4,539,581,328", "90", "15", "311091", "15"));
			Rows.Add( new UnitDataRow("U16", "U16", "16", "unit16", "14,072,702,117", "100", "16", "684400", "16"));
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
		public UnitDataRow GetRow(rowIds in_RowID)
		{
			UnitDataRow ret = null;
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
		public UnitDataRow GetRow(string in_RowString)
		{
			UnitDataRow ret = null;
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
