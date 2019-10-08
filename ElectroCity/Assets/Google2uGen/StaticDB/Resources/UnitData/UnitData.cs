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
		public float _attackfactor;
		public string _spriteUI;
		public string _spriteBody;
		public string _spriteLeg;
		public float _legX;
		public float _legY;
		public string _spriteFaceIdle;
		public string _spriteFaceShoot;
		public float _faceX;
		public float _faceY;
		public string _weaponSprite;
		public float _weaponX;
		public float _weaponY;
		public UnitDataRow(string __ID, string __uid, string __level, string __displayname, string __price, string __damage, string __firerate, string __earning, string __attackfactor, string __spriteUI, string __spriteBody, string __spriteLeg, string __legX, string __legY, string __spriteFaceIdle, string __spriteFaceShoot, string __faceX, string __faceY, string __weaponSprite, string __weaponX, string __weaponY) 
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
				if(float.TryParse(__attackfactor, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_attackfactor = res;
				else
					Debug.LogError("Failed To Convert _attackfactor string: "+ __attackfactor +" to float");
			}
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
			_weaponSprite = __weaponSprite.Trim();
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
					ret = _firerate.ToString();
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
					ret = _weaponSprite.ToString();
					break;
				case 18:
					ret = _weaponX.ToString();
					break;
				case 19:
					ret = _weaponY.ToString();
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
				case "weaponSprite":
					ret = _weaponSprite.ToString();
					break;
				case "weaponX":
					ret = _weaponX.ToString();
					break;
				case "weaponY":
					ret = _weaponY.ToString();
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
			ret += "{" + "weaponSprite" + " : " + _weaponSprite.ToString() + "} ";
			ret += "{" + "weaponX" + " : " + _weaponX.ToString() + "} ";
			ret += "{" + "weaponY" + " : " + _weaponY.ToString() + "} ";
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
			Rows.Add( new UnitDataRow("U1", "U1", "1", "unit1", "120", "15", "1", "5", "1", "01", "01_body", "01_leg", "0.02", "-0.46", "01_face_idle", "01_face_shoot", "0.015", "-0.074", "gunParts4", "-0.124", "0.008"));
			Rows.Add( new UnitDataRow("U2", "U2", "2", "unit2", "1800", "22", "2", "11", "2", "02", "02_body", "02_leg", "0.02", "-0.46", "02_face_idle", "02_face_shoot", "0.015", "0.27", "gunParts3", "-0.064", "0.382"));
			Rows.Add( new UnitDataRow("U3", "U3", "3", "unit3", "5700", "6", "3", "24", "3", "03", "03_body", "03_leg", "0.02", "-0.36", "03_face_idle", "03_face_shoot", "0.024", "-0.041", "gunParts2", "-0.085", "0.234"));
			Rows.Add( new UnitDataRow("U4", "U4", "4", "unit4", "16000", "30", "4", "53", "4", "04", "04_body", "04_leg", "0.02", "-0.445", "04_face_idle", "04_face_shoot", "0.034", "0.066", "gunParts1", "-0.229", "0.414"));
			Rows.Add( new UnitDataRow("U5", "U5", "5", "unit5", "55000", "8", "5", "117", "5", "05", "05_body", "05_leg", "0.02", "-0.404", "05_face_idle", "05_face_shoot", "0.015", "-0.126", "gunParts2", "-0.068", "0.247"));
			Rows.Add( new UnitDataRow("U6", "U6", "6", "unit6", "171600", "6", "6", "257", "6", "06", "06_body", "06_leg", "0.02", "-0.526", "06_face_idle", "06_face_shoot", "0.062", "-0.112", "gunParts4", "-0.087", "0.163"));
			Rows.Add( new UnitDataRow("U7", "U7", "7", "unit7", "530000", "88", "7", "566", "7", "07", "07_body", "07_leg", "0.02", "-0.684", "07_face_idle", "07_face_shoot", "0.048", "-0.022", "gunParts3", "-0.105", "0.264"));
			Rows.Add( new UnitDataRow("U8", "U8", "8", "unit8", "1650000", "60", "8", "1247", "8", "08", "08_body", "08_leg", "0.02", "-0.438", "08_face_idle", "08_face_shoot", "0.088", "0.263", "gunParts2", "0.158", "0.547"));
			Rows.Add( new UnitDataRow("U9", "U9", "9", "unit9", "5115000", "25", "9", "2743", "9", "09", "09_body", "09_leg", "0.099", "-0.548", "09_face_idle", "09_face_shoot", "0.133", "-0.112", "gunParts4", "-0.057", "0.197"));
			Rows.Add( new UnitDataRow("U10", "U10", "10", "unit10", "15856500", "33", "10", "6036", "10", "10", "10_body", "10_leg", "0.02", "-0.592", "10_face_idle", "10_face_shoot", "0.154", "-0.037", "gunParts1", "-0.259", "0.008"));
			Rows.Add( new UnitDataRow("U11", "U11", "11", "unit11", "49155150", "44", "11", "13279", "11", "11", "11_body", "11_leg", "0.02", "-0.567", "11_face_idle", "11_face_shoot", "0.064", "0.171", "gunParts3", "0.044", "0.631"));
			Rows.Add( new UnitDataRow("U12", "U12", "12", "unit12", "152,380,965", "66", "12", "29215", "12", "12", "12_body", "12_leg", "0.02", "-0.46", "12_face_idle", "12_face_shoot", "0.184", "-0.057", "gunParts4", "0.03", "0.554"));
			Rows.Add( new UnitDataRow("U13", "U13", "13", "unit13", "472,380,992", "70", "13", "64275", "13", "13", "13_body", "13_leg", "0.061", "-0.46", "13_face_idle", "13_face_shoot", "0.323", "-0.086", "gunParts1", "-0.031", "0.503"));
			Rows.Add( new UnitDataRow("U14", "U14", "14", "unit14", "1,464,381,073", "80", "14", "141405", "14", "14", "14_body", "14_leg", "0.02", "-0.393", "14_face_idle", "14_face_shoot", "0.015", "0.049", "gunParts3", "0.108", "0.419"));
			Rows.Add( new UnitDataRow("U15", "U15", "15", "unit15", "4,539,581,328", "90", "15", "311091", "15", "15", "15_body", "15_leg", "0.02", "-0.46", "15_face_idle", "15_face_shoot", "0.015", "0.457", "gunParts1", "0.061", "0.567"));
			Rows.Add( new UnitDataRow("U16", "U16", "16", "unit16", "14,072,702,117", "100", "16", "684400", "16", "16", "16_body", "16_leg", "0.02", "-0.741", "16_face_idle", "16_face_shoot", "0.095", "-0.146", "gunParts2", "0.205", "-0.016"));
			Rows.Add( new UnitDataRow("U17", "U17", "17", "unit17", "14,072,702,117", "100", "16", "684400", "17", "17", "17_body", "17_leg", "-0.14", "-0.46", "17_face_idle", "17_face_shoot", "-0.145", "0.038", "gunParts5", "-0.284", "0.713"));
			Rows.Add( new UnitDataRow("U18", "U18", "18", "unit18", "14,072,702,117", "100", "16", "684400", "18", "18", "18_body", "18_leg", "0.02", "-0.46", "18_face_idle", "18_face_shoot", "0.127", "-0.138", "gunParts4", "0.149", "0.577"));
			Rows.Add( new UnitDataRow("U19", "U19", "19", "unit19", "14,072,702,117", "100", "16", "684400", "19", "19", "19_body", "19_leg", "0.02", "-0.42", "19_face_idle", "19_face_shoot", "0.095", "0.383", "gunParts5", "0.1", "0.585"));
			Rows.Add( new UnitDataRow("U20", "U20", "20", "unit20", "14,072,702,117", "100", "16", "684400", "20", "20", "20_body", "20_leg", "-0.096", "-0.492", "20_face_idle", "20_face_shoot", "-0.029", "0.421", "gunParts3", "-0.285", "0.449"));
			Rows.Add( new UnitDataRow("U21", "U21", "21", "unit21", "14,072,702,117", "100", "16", "684400", "21", "21", "21_body", "21_leg", "0.023", "-0.525", "21_face_idle", "21_face_shoot", "0.086", "0.489", "gunParts1", "-0.124", "0.53"));
			Rows.Add( new UnitDataRow("U22", "U22", "22", "unit22", "14,072,702,117", "100", "16", "684400", "22", "22", "22_body", "22_leg", "-0.235", "-0.486", "22_face_idle", "22_face_shoot", "0.517", "0.284", "gunParts5", "-0.32", "0.663"));
			Rows.Add( new UnitDataRow("U23", "U23", "23", "unit23", "14,072,702,117", "100", "16", "684400", "23", "23", "23_body", "23_leg", "0.348", "-0.399", "23_face_idle", "23_face_shoot", "0.373", "0.292", "gunParts6", "-0.376", "0.29"));
			Rows.Add( new UnitDataRow("U24", "U24", "24", "unit24", "14,072,702,117", "100", "16", "684400", "24", "24", "24_body", "24_leg", "0.02", "-0.46", "24_face_idle", "24_face_shoot", "0.206", "0.002", "gunParts5", "-0.033", "0.649"));
			Rows.Add( new UnitDataRow("U25", "U25", "25", "unit25", "14,072,702,117", "100", "16", "684400", "25", "25", "25_body", "25_leg", "0.02", "-0.46", "25_face_idle", "25_face_shoot", "0.16", "0.186", "gunParts7", "0.097", "0.686"));
			Rows.Add( new UnitDataRow("U26", "U26", "26", "unit26", "14,072,702,117", "100", "16", "684400", "26", "26", "26_body", "26_leg", "0.028", "-0.567", "26_face_idle", "26_face_shoot", "0.069", "0.155", "gunParts8", "-0.071", "0.603"));
			Rows.Add( new UnitDataRow("U27", "U27", "27", "unit27", "14,072,702,117", "100", "16", "684400", "27", "27", "27_body", "27_leg", "0.02", "-0.539", "27_face_idle", "27_face_shoot", "0.252", "0.182", "gunParts5", "-0.004", "0.701"));
			Rows.Add( new UnitDataRow("U28", "U28", "28", "unit28", "14,072,702,117", "100", "16", "684400", "28", "28", "28_body", "28_leg", "0.02", "-0.479", "28_face_idle", "28_face_shoot", "0.158", "-0.074", "gunParts7", "-0.207", "0.468"));
			Rows.Add( new UnitDataRow("U29", "U29", "29", "unit29", "14,072,702,117", "100", "16", "684400", "29", "29", "29_body", "29_leg", "0.02", "-0.613", "29_face_idle", "29_face_shoot", "0.126", "-0.13", "gunParts8", "-0.223", "0.31"));
			Rows.Add( new UnitDataRow("U30", "U30", "30", "unit30", "14,072,702,117", "100", "16", "684400", "30", "30", "30_body", "30_leg", "0.02", "-0.46", "30_face_idle", "30_face_shoot", "0.17", "0.228", "gunParts9", "0.018", "0.562"));
			Rows.Add( new UnitDataRow("U31", "U31", "31", "unit31", "14,072,702,117", "100", "16", "684400", "31", "31", "31_body", "31_leg", "-0.013", "-0.689", "31_face_idle", "31_face_shoot", "0.015", "0.188", "gunParts7", "-0.171", "0.536"));
			Rows.Add( new UnitDataRow("U32", "U32", "32", "unit32", "14,072,702,117", "100", "16", "684400", "32", "32", "32_body", "32_leg", "0.02", "-0.333", "32_face_idle", "32_face_shoot", "0.075", "0.264", "gunParts10", "-0.382", "0.445"));
			Rows.Add( new UnitDataRow("U33", "U33", "33", "unit33", "14,072,702,117", "100", "16", "684400", "33", "33", "33_body", "33_leg", "0.02", "-0.494", "33_face_idle", "33_face_shoot", "0.313", "0.005", "gunParts8", "-0.212", "0.609"));
			Rows.Add( new UnitDataRow("U34", "U34", "34", "unit34", "14,072,702,117", "100", "16", "684400", "34", "34", "34_body", "34_leg", "0.02", "-0.521", "34_face_idle", "34_face_shoot", "0.478", "0.341", "gunParts5", "-0.458", "0.41"));
			Rows.Add( new UnitDataRow("U35", "U35", "35", "unit35", "14,072,702,117", "100", "16", "684400", "35", "35", "35_body", "35_leg", "-0.203", "-0.533", "35_face_idle", "35_face_shoot", "0.015", "0.355", "gunParts8", "-0.37", "0.563"));
			Rows.Add( new UnitDataRow("U36", "U36", "36", "unit36", "14,072,702,117", "100", "16", "684400", "36", "36", "36_body", "36_leg", "0.02", "-0.46", "36_face_idle", "36_face_shoot", "0.184", "0.518", "gunParts11", "-0.414", "0.512"));
			Rows.Add( new UnitDataRow("U37", "U37", "37", "unit37", "14,072,702,117", "100", "16", "684400", "37", "37", "37_body", "37_leg", "0.02", "-0.6", "37_face_idle", "37_face_shoot", "0.172", "0.497", "gunParts7", "-0.338", "0.544"));
			Rows.Add( new UnitDataRow("U38", "U38", "38", "unit38", "14,072,702,117", "100", "16", "684400", "38", "38", "38_body", "38_leg", "0.057", "-0.672", "38_face_idle", "38_face_shoot", "0.256", "-0.359", "gunParts9", "-0.124", "0.673"));
			Rows.Add( new UnitDataRow("U39", "U39", "39", "unit39", "14,072,702,117", "100", "16", "684400", "39", "39", "39_body", "39_leg", "0.02", "-0.46", "39_face_idle", "39_face_shoot", "0.472", "-0.181", "gunParts6", "-0.099", "0.54"));
			Rows.Add( new UnitDataRow("U40", "U40", "40", "unit40", "14,072,702,117", "100", "16", "684400", "40", "40", "40_body", "40_leg", "0.02", "-0.805", "40_face_idle", "40_face_shoot", "-0.056", "-0.074", "gunParts11", "-0.327", "0.929"));
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
