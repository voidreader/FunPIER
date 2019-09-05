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
	public class PassiveSkillRow : IGoogle2uRow
	{
		public string _price;
		public PassiveSkillRow(string __ID, string __price) 
		{
			_price = __price.Trim();
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
					ret = _price.ToString();
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
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "price" + " : " + _price.ToString() + "} ";
			return ret;
		}
	}
	public sealed class PassiveSkill : IGoogle2uDB
	{
		public enum rowIds {
			TYPE, DAMAGE1, DAMAGE2, DAMAGE3, DAMAGE4, DAMAGE5, DAMAGE6, DAMAGE7, DAMAGE8, DAMAGE9, DAMAGE10, DISCOUNT1, DISCOUNT2, DISCOUNT3, DISCOUNT4, DISCOUNT5, DISCOUNT6, DISCOUNT7, DISCOUNT8
			, DISCOUNT9, DISCOUNT10
		};
		public string [] rowNames = {
			"TYPE", "DAMAGE1", "DAMAGE2", "DAMAGE3", "DAMAGE4", "DAMAGE5", "DAMAGE6", "DAMAGE7", "DAMAGE8", "DAMAGE9", "DAMAGE10", "DISCOUNT1", "DISCOUNT2", "DISCOUNT3", "DISCOUNT4", "DISCOUNT5", "DISCOUNT6", "DISCOUNT7", "DISCOUNT8"
			, "DISCOUNT9", "DISCOUNT10"
		};
		public System.Collections.Generic.List<PassiveSkillRow> Rows = new System.Collections.Generic.List<PassiveSkillRow>();

		public static PassiveSkill Instance
		{
			get { return NestedPassiveSkill.instance; }
		}

		private class NestedPassiveSkill
		{
			static NestedPassiveSkill() { }
			internal static readonly PassiveSkill instance = new PassiveSkill();
		}

		private PassiveSkill()
		{
			Rows.Add( new PassiveSkillRow("TYPE", "long"));
			Rows.Add( new PassiveSkillRow("DAMAGE1", "50000"));
			Rows.Add( new PassiveSkillRow("DAMAGE2", "700000"));
			Rows.Add( new PassiveSkillRow("DAMAGE3", "7000000"));
			Rows.Add( new PassiveSkillRow("DAMAGE4", "350000000"));
			Rows.Add( new PassiveSkillRow("DAMAGE5", "10000000000"));
			Rows.Add( new PassiveSkillRow("DAMAGE6", "95000000000"));
			Rows.Add( new PassiveSkillRow("DAMAGE7", "100"));
			Rows.Add( new PassiveSkillRow("DAMAGE8", "100"));
			Rows.Add( new PassiveSkillRow("DAMAGE9", "100"));
			Rows.Add( new PassiveSkillRow("DAMAGE10", "100"));
			Rows.Add( new PassiveSkillRow("DISCOUNT1", "75000"));
			Rows.Add( new PassiveSkillRow("DISCOUNT2", "1000000"));
			Rows.Add( new PassiveSkillRow("DISCOUNT3", "12000000"));
			Rows.Add( new PassiveSkillRow("DISCOUNT4", "520000000"));
			Rows.Add( new PassiveSkillRow("DISCOUNT5", "14000000000"));
			Rows.Add( new PassiveSkillRow("DISCOUNT6", "140000000000"));
			Rows.Add( new PassiveSkillRow("DISCOUNT7", "100"));
			Rows.Add( new PassiveSkillRow("DISCOUNT8", "100"));
			Rows.Add( new PassiveSkillRow("DISCOUNT9", "100"));
			Rows.Add( new PassiveSkillRow("DISCOUNT10", "100"));
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
		public PassiveSkillRow GetRow(rowIds in_RowID)
		{
			PassiveSkillRow ret = null;
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
		public PassiveSkillRow GetRow(string in_RowString)
		{
			PassiveSkillRow ret = null;
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
