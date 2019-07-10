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
			, boss18, boss19, boss20, boss21, boss22, boss23, boss24, boss25, boss26, boss27, boss28, boss29, boss30, boss31, boss32, boss33, boss34, boss35, boss36, boss37
			, boss38, boss39, boss40, boss41, boss42
		};
		public string [] rowNames = {
			"boss0", "boss1", "boss2", "boss3", "boss4", "boss5", "boss6", "boss7", "boss8", "boss9", "boss10", "boss11", "boss12", "boss13", "boss14", "boss15", "boss16", "boss17"
			, "boss18", "boss19", "boss20", "boss21", "boss22", "boss23", "boss24", "boss25", "boss26", "boss27", "boss28", "boss29", "boss30", "boss31", "boss32", "boss33", "boss34", "boss35", "boss36", "boss37"
			, "boss38", "boss39", "boss40", "boss41", "boss42"
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
			Rows.Add( new BossDataRow("boss2", "0", "2", "S.Witch", "S.Witch", "15", "S.witch"));
			Rows.Add( new BossDataRow("boss3", "1", "3", "Surprise", "Surprise Man", "15", "Surprise"));
			Rows.Add( new BossDataRow("boss4", "1", "4", "Freddy", "Cute Freddy", "18", "Freddy"));
			Rows.Add( new BossDataRow("boss5", "1", "5", "Hotkey", "Hotkey", "12", "hotkey"));
			Rows.Add( new BossDataRow("boss6", "1", "6", "Texas", "Texas Man", "20", "Texas"));
			Rows.Add( new BossDataRow("boss7", "1", "7", "Trick", "Trick", "21", "Trick"));
			Rows.Add( new BossDataRow("boss8", "2", "8", "BadPenguin", "Bad Penguin", "20", "badpenguin"));
			Rows.Add( new BossDataRow("boss9", "2", "9", "CatAunt", "Cat Aunt", "23", "cataunt"));
			Rows.Add( new BossDataRow("boss10", "2", "10", "Halface", "Half Face", "24", "halface"));
			Rows.Add( new BossDataRow("boss11", "2", "11", "Pigin", "Pig In", "26", "pigin"));
			Rows.Add( new BossDataRow("boss12", "3", "12", "Alien-Yo", "Alien-Yo", "28", "Alien-Yo"));
			Rows.Add( new BossDataRow("boss13", "3", "13", "Princess-A", "Princess-A", "32", "Princess-A"));
			Rows.Add( new BossDataRow("boss14", "3", "14", "Hero-Je", "Hero-Je", "30", "Hero-Je"));
			Rows.Add( new BossDataRow("boss15", "3", "15", "Father", "Big Daddy", "32", "father"));
			Rows.Add( new BossDataRow("boss16", "4", "16", "R.Harry", "R.Harry", "25", "R.harry"));
			Rows.Add( new BossDataRow("boss17", "4", "17", "R.Marv", "R.Marv", "35", "R.marv"));
			Rows.Add( new BossDataRow("boss18", "4", "18", "Gevin", "Gevin Lonely", "45", "Gevin"));
			Rows.Add( new BossDataRow("boss19", "5", "19", "PinkRabbit", "Pink Rabbit", "46", "PinkRabbit"));
			Rows.Add( new BossDataRow("boss20", "5", "20", "HorrorNurse", "Horror Nurse", "48", "horrornurse"));
			Rows.Add( new BossDataRow("boss21", "5", "21", "Triangle", "Triangle", "40", "triangle"));
			Rows.Add( new BossDataRow("boss22", "6", "22", "Dr.Bald", "Dr.Bald", "60", "Dr.bald"));
			Rows.Add( new BossDataRow("boss23", "6", "23", "Greenman", "Greeny", "55", "greenman"));
			Rows.Add( new BossDataRow("boss24", "6", "24", "Chief-buggle", "Chief-Buggle", "70", "Chief-buggle"));
			Rows.Add( new BossDataRow("boss25", "6", "25", "Lizard", "Lizard", "80", "Lizard"));
			Rows.Add( new BossDataRow("boss26", "6", "26", "Sand", "Sandy", "75", "Sand"));
			Rows.Add( new BossDataRow("boss27", "7", "27", "MagicGirl", "Enchanter", "75", "MagicGirl"));
			Rows.Add( new BossDataRow("boss28", "7", "28", "Killer", "Ugly Croc", "80", "Killer"));
			Rows.Add( new BossDataRow("boss29", "7", "29", "ShooterMan", "Head Shot", "75", "ShooterMan"));
			Rows.Add( new BossDataRow("boss30", "7", "30", "Diablo", "Diablo", "68", "Diablo"));
			Rows.Add( new BossDataRow("boss31", "7", "31", "Hollyshit", "Holly Queen", "80", "hollyshit"));
			Rows.Add( new BossDataRow("boss32", "7", "32", "Pocker", "Poker", "80", "poker"));
			Rows.Add( new BossDataRow("boss33", "8", "33", "FishMan", "Walking Fish", "85", "FishMan"));
			Rows.Add( new BossDataRow("boss34", "8", "34", "StarFish", "Star Fish", "68", "StarFish"));
			Rows.Add( new BossDataRow("boss35", "8", "35", "Sparrow", "Sparrow", "70", "sparrow"));
			Rows.Add( new BossDataRow("boss36", "8", "36", "Octo-Johns", "Octo-Johns", "80", "Octo-Johns"));
			Rows.Add( new BossDataRow("boss37", "9", "37", "ryu", "White Fighter", "88", "ryu"));
			Rows.Add( new BossDataRow("boss38", "9", "38", "ken", "Red Fighter", "65", "ken"));
			Rows.Add( new BossDataRow("boss39", "9", "39", "Dhalim", "Darling", "70", "Dhalim"));
			Rows.Add( new BossDataRow("boss40", "9", "40", "Guile", "AlliGuile", "80", "Guile"));
			Rows.Add( new BossDataRow("boss41", "9", "41", "Chun-li", "Street Girl", "90", "Chun-li"));
			Rows.Add( new BossDataRow("boss42", "9", "42", "HeyGrandfa", "Tekken Papa", "80", "heygrandfa"));
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
