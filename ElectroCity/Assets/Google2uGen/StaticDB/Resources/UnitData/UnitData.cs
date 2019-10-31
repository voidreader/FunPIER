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
		public string _weaponid;
		public string _earning;
		public string _attackfactor;
		public string _spriteUI;
		public string _spriteBody;
		public string _spriteLeg;
		public float _legX;
		public float _legY;
		public string _spriteFaceIdle;
		public string _spriteFaceShoot;
		public float _faceX;
		public float _faceY;
		public float _weaponX;
		public float _weaponY;
		public float _weaponscale;
		public UnitDataRow(string __ID, string __uid, string __level, string __displayname, string __price, string __damage, string __weaponid, string __earning, string __attackfactor, string __spriteUI, string __spriteBody, string __spriteLeg, string __legX, string __legY, string __spriteFaceIdle, string __spriteFaceShoot, string __faceX, string __faceY, string __weaponX, string __weaponY, string __weaponscale) 
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
			_weaponid = __weaponid.Trim();
			_earning = __earning.Trim();
			_attackfactor = __attackfactor.Trim();
			_spriteUI = __spriteUI.Trim();
			_spriteBody = __spriteBody.Trim();
			_spriteLeg = __spriteLeg.Trim();
			{
			float res;
				if(float.TryParse(__legX, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_legX = res;
				else
					Debug.LogError("Failed To Convert _legX string: "+ __legX +" to float");
			}
			{
			float res;
				if(float.TryParse(__legY, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_legY = res;
				else
					Debug.LogError("Failed To Convert _legY string: "+ __legY +" to float");
			}
			_spriteFaceIdle = __spriteFaceIdle.Trim();
			_spriteFaceShoot = __spriteFaceShoot.Trim();
			{
			float res;
				if(float.TryParse(__faceX, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_faceX = res;
				else
					Debug.LogError("Failed To Convert _faceX string: "+ __faceX +" to float");
			}
			{
			float res;
				if(float.TryParse(__faceY, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_faceY = res;
				else
					Debug.LogError("Failed To Convert _faceY string: "+ __faceY +" to float");
			}
			{
			float res;
				if(float.TryParse(__weaponX, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_weaponX = res;
				else
					Debug.LogError("Failed To Convert _weaponX string: "+ __weaponX +" to float");
			}
			{
			float res;
				if(float.TryParse(__weaponY, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_weaponY = res;
				else
					Debug.LogError("Failed To Convert _weaponY string: "+ __weaponY +" to float");
			}
			{
			float res;
				if(float.TryParse(__weaponscale, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_weaponscale = res;
				else
					Debug.LogError("Failed To Convert _weaponscale string: "+ __weaponscale +" to float");
			}
		}

		public int Length { get { return 20; } }

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
					ret = _weaponid.ToString();
					break;
				case 6:
					ret = _earning.ToString();
					break;
				case 7:
					ret = _attackfactor.ToString();
					break;
				case 8:
					ret = _spriteUI.ToString();
					break;
				case 9:
					ret = _spriteBody.ToString();
					break;
				case 10:
					ret = _spriteLeg.ToString();
					break;
				case 11:
					ret = _legX.ToString();
					break;
				case 12:
					ret = _legY.ToString();
					break;
				case 13:
					ret = _spriteFaceIdle.ToString();
					break;
				case 14:
					ret = _spriteFaceShoot.ToString();
					break;
				case 15:
					ret = _faceX.ToString();
					break;
				case 16:
					ret = _faceY.ToString();
					break;
				case 17:
					ret = _weaponX.ToString();
					break;
				case 18:
					ret = _weaponY.ToString();
					break;
				case 19:
					ret = _weaponscale.ToString();
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
				case "weaponid":
					ret = _weaponid.ToString();
					break;
				case "earning":
					ret = _earning.ToString();
					break;
				case "attackfactor":
					ret = _attackfactor.ToString();
					break;
				case "spriteUI":
					ret = _spriteUI.ToString();
					break;
				case "spriteBody":
					ret = _spriteBody.ToString();
					break;
				case "spriteLeg":
					ret = _spriteLeg.ToString();
					break;
				case "legX":
					ret = _legX.ToString();
					break;
				case "legY":
					ret = _legY.ToString();
					break;
				case "spriteFaceIdle":
					ret = _spriteFaceIdle.ToString();
					break;
				case "spriteFaceShoot":
					ret = _spriteFaceShoot.ToString();
					break;
				case "faceX":
					ret = _faceX.ToString();
					break;
				case "faceY":
					ret = _faceY.ToString();
					break;
				case "weaponX":
					ret = _weaponX.ToString();
					break;
				case "weaponY":
					ret = _weaponY.ToString();
					break;
				case "weaponscale":
					ret = _weaponscale.ToString();
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
			ret += "{" + "weaponid" + " : " + _weaponid.ToString() + "} ";
			ret += "{" + "earning" + " : " + _earning.ToString() + "} ";
			ret += "{" + "attackfactor" + " : " + _attackfactor.ToString() + "} ";
			ret += "{" + "spriteUI" + " : " + _spriteUI.ToString() + "} ";
			ret += "{" + "spriteBody" + " : " + _spriteBody.ToString() + "} ";
			ret += "{" + "spriteLeg" + " : " + _spriteLeg.ToString() + "} ";
			ret += "{" + "legX" + " : " + _legX.ToString() + "} ";
			ret += "{" + "legY" + " : " + _legY.ToString() + "} ";
			ret += "{" + "spriteFaceIdle" + " : " + _spriteFaceIdle.ToString() + "} ";
			ret += "{" + "spriteFaceShoot" + " : " + _spriteFaceShoot.ToString() + "} ";
			ret += "{" + "faceX" + " : " + _faceX.ToString() + "} ";
			ret += "{" + "faceY" + " : " + _faceY.ToString() + "} ";
			ret += "{" + "weaponX" + " : " + _weaponX.ToString() + "} ";
			ret += "{" + "weaponY" + " : " + _weaponY.ToString() + "} ";
			ret += "{" + "weaponscale" + " : " + _weaponscale.ToString() + "} ";
			return ret;
		}
	}
	public sealed class UnitData : IGoogle2uDB
	{
		public enum rowIds {
			U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, U16, U17, U18
			, U19, U20, U21, U22, U23, U24, U25, U26, U27, U28, U29, U30, U31, U32, U33, U34, U35, U36, U37, U38
			, U39, U40
		};
		public string [] rowNames = {
			"U1", "U2", "U3", "U4", "U5", "U6", "U7", "U8", "U9", "U10", "U11", "U12", "U13", "U14", "U15", "U16", "U17", "U18"
			, "U19", "U20", "U21", "U22", "U23", "U24", "U25", "U26", "U27", "U28", "U29", "U30", "U31", "U32", "U33", "U34", "U35", "U36", "U37", "U38"
			, "U39", "U40"
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
			Rows.Add( new UnitDataRow("U1", "U1", "1", "Transistor", "120", "15", "UnitWeapon4", "5", "5", "01", "01_body", "01_leg", "0.02", "-0.46", "01_face_idle", "01_face_shoot", "0.015", "-0.074", "-0.124", "0.008", "1"));
			Rows.Add( new UnitDataRow("U2", "U2", "2", "Bulb", "1800", "22", "UnitWeapon7", "11", "11", "02", "02_body", "02_leg", "0.02", "-0.46", "02_face_idle", "02_face_shoot", "0.048", "0.27", "-0.149", "0.342", "1"));
			Rows.Add( new UnitDataRow("U3", "U3", "3", "Floppy", "5700", "6", "UnitWeapon11", "24", "24", "03", "03_body", "03_leg", "0.02", "-0.36", "03_face_idle", "03_face_shoot", "0.029", "0.01", "-0.238", "0.358", "0.8"));
			Rows.Add( new UnitDataRow("U4", "U4", "4", "Videotape", "16000", "30", "UnitWeapon1", "53", "53", "04", "04_body", "04_leg", "0.02", "-0.445", "04_face_idle", "04_face_shoot", "0.034", "0.066", "-0.255", "0.331", "1"));
			Rows.Add( new UnitDataRow("U5", "U5", "5", "CD", "55000", "8", "UnitWeapon2", "117", "117", "05", "05_body", "05_leg", "0.02", "-0.404", "05_face_idle", "05_face_shoot", "0.015", "-0.126", "-0.119", "0.287", "1"));
			Rows.Add( new UnitDataRow("U6", "U6", "6", "USB", "171600", "6", "UnitWeapon11", "257", "257", "06", "06_body", "06_leg", "0.02", "-0.526", "06_face_idle", "06_face_shoot", "0.062", "-0.112", "-0.087", "0.133", "0.8"));
			Rows.Add( new UnitDataRow("U7", "U7", "7", "Remote", "530000", "88", "UnitWeapon8", "566", "566", "07", "07_body", "07_leg", "0.02", "-0.684", "07_face_idle", "07_face_shoot", "0.048", "-0.022", "-0.101", "0.597", "0.8"));
			Rows.Add( new UnitDataRow("U8", "U8", "8", "unit8", "1650000", "60", "UnitWeapon6", "1250", "1250", "08", "08_body", "08_leg", "0.02", "-0.438", "08_face_idle", "08_face_shoot", "0.088", "0.263", "-0.345", "0.334", "0.8"));
			Rows.Add( new UnitDataRow("U9", "U9", "9", "unit9", "5115000", "25", "UnitWeapon3", "2750", "2750", "09", "09_body", "09_leg", "0.099", "-0.548", "09_face_idle", "09_face_shoot", "0.133", "-0.112", "-0.021", "0.278", "1"));
			Rows.Add( new UnitDataRow("U10", "U10", "10", "unit10", "15856500", "33", "UnitWeapon9", "6000", "6000", "10", "10_body", "10_leg", "0.02", "-0.592", "10_face_idle", "10_face_shoot", "0.154", "-0.037", "-0.162", "0.156", "1"));
			Rows.Add( new UnitDataRow("U11", "U11", "11", "unit11", "49155150", "44", "UnitWeapon7", "13280", "13280", "11", "11_body", "11_leg", "0.02", "-0.567", "11_face_idle", "11_face_shoot", "0.064", "0.171", "-0.233", "0.449", "1"));
			Rows.Add( new UnitDataRow("U12", "U12", "12", "unit12", "49155150", "66", "UnitWeapon5", "29000", "29000", "12", "12_body", "12_leg", "0.02", "-0.46", "12_face_idle", "12_face_shoot", "0.184", "-0.057", "-0.267", "0.29", "0.8"));
			Rows.Add( new UnitDataRow("U13", "U13", "13", "unit13", "49155150", "70", "UnitWeapon1", "64200", "64200", "13", "13_body", "13_leg", "0.061", "-0.46", "13_face_idle", "13_face_shoot", "0.323", "-0.086", "-0.273", "0.067", "1"));
			Rows.Add( new UnitDataRow("U14", "U14", "14", "unit14", "49155150", "80", "UnitWeapon4", "141400", "141400", "14", "14_body", "14_leg", "0.02", "-0.393", "14_face_idle", "14_face_shoot", "0.015", "0.049", "-0.554", "0.279", "1"));
			Rows.Add( new UnitDataRow("U15", "U15", "15", "unit15", "49155150", "90", "UnitWeapon10", "311000", "311000", "15", "15_body", "15_leg", "0.02", "-0.46", "15_face_idle", "15_face_shoot", "0.015", "0.457", "-0.187", "-0.107", "0.8"));
			Rows.Add( new UnitDataRow("U16", "U16", "16", "unit16", "49155150", "100", "UnitWeapon2", "684400", "684400", "16", "16_body", "16_leg", "0.02", "-0.741", "16_face_idle", "16_face_shoot", "0.095", "-0.146", "0.106", "0.095", "0.8"));
			Rows.Add( new UnitDataRow("U17", "U17", "17", "unit17", "49155150", "100", "UnitWeapon9", "1506000", "1506000", "17", "17_body", "17_leg", "-0.14", "-0.46", "17_face_idle", "17_face_shoot", "-0.145", "0.038", "-0.679", "0.435", "1"));
			Rows.Add( new UnitDataRow("U18", "U18", "18", "unit18", "49155150", "100", "UnitWeapon6", "3312000", "3312000", "18", "18_body", "18_leg", "0.02", "-0.46", "18_face_idle", "18_face_shoot", "0.127", "-0.138", "-0.166", "0.387", "0.8"));
			Rows.Add( new UnitDataRow("U19", "U19", "19", "unit19", "49155150", "100", "UnitWeapon3", "7290000", "7290000", "19", "19_body", "19_leg", "0.02", "-0.42", "19_face_idle", "19_face_shoot", "0.095", "0.383", "-0.343", "0.32", "1"));
			Rows.Add( new UnitDataRow("U20", "U20", "20", "unit20", "49155150", "100", "UnitWeapon11", "16032480", "16032480", "20", "20_body", "20_leg", "-0.096", "-0.492", "20_face_idle", "20_face_shoot", "-0.029", "0.421", "-0.488", "0.437", "0.9"));
			Rows.Add( new UnitDataRow("U21", "U21", "21", "unit21", "49155150", "100", "UnitWeapon5", "35270000", "35270000", "21", "21_body", "21_leg", "0.023", "-0.525", "21_face_idle", "21_face_shoot", "0.086", "0.489", "-0.231", "0.423", "0.8"));
			Rows.Add( new UnitDataRow("U22", "U22", "22", "unit22", "49155150", "100", "UnitWeapon8", "77600000", "77600000", "22", "22_body", "22_leg", "-0.235", "-0.486", "22_face_idle", "22_face_shoot", "0.517", "0.284", "-0.351", "0.417", "1"));
			Rows.Add( new UnitDataRow("U23", "U23", "23", "unit23", "49155150", "100", "UnitWeapon7", "170700000", "170700000", "23", "23_body", "23_leg", "0.348", "-0.399", "23_face_idle", "23_face_shoot", "0.373", "0.292", "-0.608", "0.134", "1"));
			Rows.Add( new UnitDataRow("U24", "U24", "24", "unit24", "49155150", "100", "UnitWeapon6", "375600000", "375600000", "24", "24_body", "24_leg", "0.02", "-0.46", "24_face_idle", "24_face_shoot", "0.206", "0.002", "-0.48", "0.302", "0.9"));
			Rows.Add( new UnitDataRow("U25", "U25", "25", "unit25", "49155150", "100", "UnitWeapon10", "826200000", "826200000", "25", "25_body", "25_leg", "0.02", "-0.46", "25_face_idle", "25_face_shoot", "0.16", "0.186", "-0.134", "0.438", "0.9"));
			Rows.Add( new UnitDataRow("U26", "U26", "26", "unit26", "49155150", "100", "UnitWeapon1", "1820000000", "1820000000", "26", "26_body", "26_leg", "0.028", "-0.567", "26_face_idle", "26_face_shoot", "0.069", "0.155", "-0.224", "0.495", "1"));
			Rows.Add( new UnitDataRow("U27", "U27", "27", "unit27", "49155150", "100", "UnitWeapon4", "4000000000", "4000000000", "27", "27_body", "27_leg", "0.02", "-0.539", "27_face_idle", "27_face_shoot", "0.252", "0.182", "-0.228", "0.461", "1.2"));
			Rows.Add( new UnitDataRow("U28", "U28", "28", "unit28", "49155150", "100", "UnitWeapon2", "8780000000", "8780000000", "28", "28_body", "28_leg", "0.02", "-0.479", "28_face_idle", "28_face_shoot", "0.158", "-0.074", "-0.207", "0.431", "1.2"));
			Rows.Add( new UnitDataRow("U29", "U29", "29", "unit29", "49155150", "100", "UnitWeapon11", "19360000000", "19360000000", "29", "29_body", "29_leg", "0.02", "-0.613", "29_face_idle", "29_face_shoot", "0.126", "-0.13", "-0.434", "0.12", "1"));
			Rows.Add( new UnitDataRow("U30", "U30", "30", "unit30", "49155150", "100", "UnitWeapon3", "42592000000", "42592000000", "30", "30_body", "30_leg", "0.02", "-0.46", "30_face_idle", "30_face_shoot", "0.17", "0.228", "-0.358", "0.388", "1.2"));
			Rows.Add( new UnitDataRow("U31", "U31", "31", "unit31", "49155150", "100", "UnitWeapon9", "93702400000", "93702400000", "31", "31_body", "31_leg", "-0.013", "-0.689", "31_face_idle", "31_face_shoot", "0.015", "0.188", "-0.171", "0.536", "1.2"));
			Rows.Add( new UnitDataRow("U32", "U32", "32", "unit32", "49155150", "100", "UnitWeapon7", "206145280000", "206145280000", "32", "32_body", "32_leg", "0.02", "-0.333", "32_face_idle", "32_face_shoot", "0.075", "0.264", "-0.684", "0.279", "1"));
			Rows.Add( new UnitDataRow("U33", "U33", "33", "unit33", "49155150", "100", "UnitWeapon5", "453000000000", "453000000000", "33", "33_body", "33_leg", "0.02", "-0.494", "33_face_idle", "33_face_shoot", "0.313", "0.005", "-0.646", "0.304", "1"));
			Rows.Add( new UnitDataRow("U34", "U34", "34", "unit34", "49155150", "100", "UnitWeapon10", "996600000000", "996600000000", "34", "34_body", "34_leg", "0.02", "-0.521", "34_face_idle", "34_face_shoot", "0.478", "0.341", "-0.247", "0.468", "1"));
			Rows.Add( new UnitDataRow("U35", "U35", "35", "unit35", "49155150", "100", "UnitWeapon3", "2192000000000", "2192000000000", "35", "35_body", "35_leg", "-0.203", "-0.533", "35_face_idle", "35_face_shoot", "0.015", "0.355", "-0.465", "0.522", "1.3"));
			Rows.Add( new UnitDataRow("U36", "U36", "36", "unit36", "49155150", "100", "UnitWeapon8", "4822400000000", "4822400000000", "36", "36_body", "36_leg", "0.02", "-0.46", "36_face_idle", "36_face_shoot", "0.184", "0.518", "-0.352", "0.522", "1.2"));
			Rows.Add( new UnitDataRow("U37", "U37", "37", "unit37", "49155150", "100", "UnitWeapon2", "10609000000000", "10609000000000", "37", "37_body", "37_leg", "0.02", "-0.6", "37_face_idle", "37_face_shoot", "0.172", "0.497", "-0.293", "0.582", "1.2"));
			Rows.Add( new UnitDataRow("U38", "U38", "38", "unit38", "49155150", "100", "UnitWeapon11", "23340000000000", "23340000000000", "38", "38_body", "38_leg", "0.057", "-0.672", "38_face_idle", "38_face_shoot", "0.256", "-0.359", "-0.273", "0.793", "1"));
			Rows.Add( new UnitDataRow("U39", "U39", "39", "unit39", "49155150", "100", "UnitWeapon4", "51348000000000", "51348000000000", "39", "39_body", "39_leg", "0.02", "-0.46", "39_face_idle", "39_face_shoot", "0.472", "-0.181", "0.096", "0.54", "1.2"));
			Rows.Add( new UnitDataRow("U40", "U40", "40", "unit40", "49155150", "100", "UnitWeapon1", "113000000000000", "113000000000000", "40", "40_body", "40_leg", "0.02", "-0.805", "40_face_idle", "40_face_shoot", "-0.056", "-0.074", "-0.468", "0.773", "1.4"));
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
