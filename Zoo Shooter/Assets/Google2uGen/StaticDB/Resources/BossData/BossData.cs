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
	public class BossDataRow : IGoogle2uRow
	{
		public int _list;
		public int _level;
		public string _sprite;
		public string _name;
		public int _hp;
		public string _portrait;
		public BossDataRow(string __ID, string __list, string __level, string __sprite, string __name, string __hp, string __portrait) 
		{
			{
			int res;
				if(int.TryParse(__list, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_list = res;
				else
					Debug.LogError("Failed To Convert _list string: "+ __list +" to int");
			}
			{
			int res;
				if(int.TryParse(__level, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_level = res;
				else
					Debug.LogError("Failed To Convert _level string: "+ __level +" to int");
			}
			_sprite = __sprite.Trim();
			_name = __name.Trim();
			{
			int res;
				if(int.TryParse(__hp, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_hp = res;
				else
					Debug.LogError("Failed To Convert _hp string: "+ __hp +" to int");
			}
			_portrait = __portrait.Trim();
		}

		public int Length { get { return 6; } }

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
					ret = _level.ToString();
					break;
				case 2:
					ret = _sprite.ToString();
					break;
				case 3:
					ret = _name.ToString();
					break;
				case 4:
					ret = _hp.ToString();
					break;
				case 5:
					ret = _portrait.ToString();
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
				case "level":
					ret = _level.ToString();
					break;
				case "sprite":
					ret = _sprite.ToString();
					break;
				case "name":
					ret = _name.ToString();
					break;
				case "hp":
					ret = _hp.ToString();
					break;
				case "portrait":
					ret = _portrait.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "list" + " : " + _list.ToString() + "} ";
			ret += "{" + "level" + " : " + _level.ToString() + "} ";
			ret += "{" + "sprite" + " : " + _sprite.ToString() + "} ";
			ret += "{" + "name" + " : " + _name.ToString() + "} ";
			ret += "{" + "hp" + " : " + _hp.ToString() + "} ";
			ret += "{" + "portrait" + " : " + _portrait.ToString() + "} ";
			return ret;
		}
	}
	public sealed class BossData : IGoogle2uDB
	{
		public enum rowIds {
			boss0, boss1, boss2, boss3, boss4, boss5, boss6, boss7, boss8, boss9, boss10, boss11, boss12, boss13, boss14, boss15, boss16, boss17
			, boss18
		};
		public string [] rowNames = {
			"boss0", "boss1", "boss2", "boss3", "boss4", "boss5", "boss6", "boss7", "boss8", "boss9", "boss10", "boss11", "boss12", "boss13", "boss14", "boss15", "boss16", "boss17"
			, "boss18"
		};
		public System.Collections.Generic.List<BossDataRow> Rows = new System.Collections.Generic.List<BossDataRow>();

		public static BossData Instance
		{
			get { return NestedBossData.instance; }
		}

		private class NestedBossData
		{
			static NestedBossData() { }
			internal static readonly BossData instance = new BossData();
		}

		private BossData()
		{
			Rows.Add( new BossDataRow("boss0", "0", "0", "GasMask", "Gas Mask", "12", "gasmask"));
			Rows.Add( new BossDataRow("boss1", "0", "1", "Clown", "Clown", "12", "clown"));
			Rows.Add( new BossDataRow("boss2", "0", "2", "S.Witch", "S.Witch", "18", "S.witch"));
			Rows.Add( new BossDataRow("boss3", "1", "3", "Surprise", "Surprise Man", "15", "Surprise"));
			Rows.Add( new BossDataRow("boss4", "1", "4", "Freddy", "Freddy", "17", "Freddy"));
			Rows.Add( new BossDataRow("boss5", "1", "5", "Hotkey", "Hotkey", "20", "hotkey"));
			Rows.Add( new BossDataRow("boss6", "1", "6", "Texas", "Texas", "20", "Texas"));
			Rows.Add( new BossDataRow("boss7", "1", "7", "Trick", "Trick", "22", "Trick"));
			Rows.Add( new BossDataRow("boss8", "2", "8", "BadPenguin", "Bad Penguin", "25", "badpenguin"));
			Rows.Add( new BossDataRow("boss9", "2", "9", "CatAunt", "Cat Aunt", "28", "cataunt"));
			Rows.Add( new BossDataRow("boss10", "2", "10", "Halface", "Half Face", "30", "halface"));
			Rows.Add( new BossDataRow("boss11", "2", "11", "Pigin", "Pig In", "35", "pigin"));
			Rows.Add( new BossDataRow("boss12", "3", "12", "Alien-Yo", "Alien-Yo", "35", "Alien-Yo"));
			Rows.Add( new BossDataRow("boss13", "3", "13", "Princess-A", "Princess-A", "35", "Princess-A"));
			Rows.Add( new BossDataRow("boss14", "3", "14", "Hero-Je", "Hero-Je", "35", "Hero-Je"));
			Rows.Add( new BossDataRow("boss15", "3", "15", "Father", "Big Daddy", "35", "father"));
			Rows.Add( new BossDataRow("boss16", "4", "16", "R.Harry", "R.Harry", "35", "R.harry"));
			Rows.Add( new BossDataRow("boss17", "4", "17", "R.Marv", "R.Marv", "35", "R.marv"));
			Rows.Add( new BossDataRow("boss18", "4", "18", "Gevin", "Gevin Lonely", "35", "Gevin"));
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
		public BossDataRow GetRow(rowIds in_RowID)
		{
			BossDataRow ret = null;
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
		public BossDataRow GetRow(string in_RowString)
		{
			BossDataRow ret = null;
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
