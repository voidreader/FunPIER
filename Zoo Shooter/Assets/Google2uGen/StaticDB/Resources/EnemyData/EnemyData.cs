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
	public class EnemyDataRow : IGoogle2uRow
	{
		public string _sprite;
		public string _identifier;
		public string _name;
		public float _offsetX;
		public float _offsetY;
		public float _sizeX;
		public float _sizeY;
		public float _hoffsetX;
		public float _hoffsetY;
		public float _hsizeX;
		public float _hsizeY;
		public int _hp;
		public EnemyDataRow(string __ID, string __sprite, string __identifier, string __name, string __offsetX, string __offsetY, string __sizeX, string __sizeY, string __hoffsetX, string __hoffsetY, string __hsizeX, string __hsizeY, string __hp) 
		{
			_sprite = __sprite.Trim();
			_identifier = __identifier.Trim();
			_name = __name.Trim();
			{
			float res;
				if(float.TryParse(__offsetX, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_offsetX = res;
				else
					Debug.LogError("Failed To Convert _offsetX string: "+ __offsetX +" to float");
			}
			{
			float res;
				if(float.TryParse(__offsetY, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_offsetY = res;
				else
					Debug.LogError("Failed To Convert _offsetY string: "+ __offsetY +" to float");
			}
			{
			float res;
				if(float.TryParse(__sizeX, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_sizeX = res;
				else
					Debug.LogError("Failed To Convert _sizeX string: "+ __sizeX +" to float");
			}
			{
			float res;
				if(float.TryParse(__sizeY, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_sizeY = res;
				else
					Debug.LogError("Failed To Convert _sizeY string: "+ __sizeY +" to float");
			}
			{
			float res;
				if(float.TryParse(__hoffsetX, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_hoffsetX = res;
				else
					Debug.LogError("Failed To Convert _hoffsetX string: "+ __hoffsetX +" to float");
			}
			{
			float res;
				if(float.TryParse(__hoffsetY, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_hoffsetY = res;
				else
					Debug.LogError("Failed To Convert _hoffsetY string: "+ __hoffsetY +" to float");
			}
			{
			float res;
				if(float.TryParse(__hsizeX, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_hsizeX = res;
				else
					Debug.LogError("Failed To Convert _hsizeX string: "+ __hsizeX +" to float");
			}
			{
			float res;
				if(float.TryParse(__hsizeY, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_hsizeY = res;
				else
					Debug.LogError("Failed To Convert _hsizeY string: "+ __hsizeY +" to float");
			}
			{
			int res;
				if(int.TryParse(__hp, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_hp = res;
				else
					Debug.LogError("Failed To Convert _hp string: "+ __hp +" to int");
			}
		}

		public int Length { get { return 12; } }

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
					ret = _sprite.ToString();
					break;
				case 1:
					ret = _identifier.ToString();
					break;
				case 2:
					ret = _name.ToString();
					break;
				case 3:
					ret = _offsetX.ToString();
					break;
				case 4:
					ret = _offsetY.ToString();
					break;
				case 5:
					ret = _sizeX.ToString();
					break;
				case 6:
					ret = _sizeY.ToString();
					break;
				case 7:
					ret = _hoffsetX.ToString();
					break;
				case 8:
					ret = _hoffsetY.ToString();
					break;
				case 9:
					ret = _hsizeX.ToString();
					break;
				case 10:
					ret = _hsizeY.ToString();
					break;
				case 11:
					ret = _hp.ToString();
					break;
			}

			return ret;
		}

		public string GetStringData( string colID )
		{
			var ret = System.String.Empty;
			switch( colID )
			{
				case "sprite":
					ret = _sprite.ToString();
					break;
				case "identifier":
					ret = _identifier.ToString();
					break;
				case "name":
					ret = _name.ToString();
					break;
				case "offsetX":
					ret = _offsetX.ToString();
					break;
				case "offsetY":
					ret = _offsetY.ToString();
					break;
				case "sizeX":
					ret = _sizeX.ToString();
					break;
				case "sizeY":
					ret = _sizeY.ToString();
					break;
				case "hoffsetX":
					ret = _hoffsetX.ToString();
					break;
				case "hoffsetY":
					ret = _hoffsetY.ToString();
					break;
				case "hsizeX":
					ret = _hsizeX.ToString();
					break;
				case "hsizeY":
					ret = _hsizeY.ToString();
					break;
				case "hp":
					ret = _hp.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "sprite" + " : " + _sprite.ToString() + "} ";
			ret += "{" + "identifier" + " : " + _identifier.ToString() + "} ";
			ret += "{" + "name" + " : " + _name.ToString() + "} ";
			ret += "{" + "offsetX" + " : " + _offsetX.ToString() + "} ";
			ret += "{" + "offsetY" + " : " + _offsetY.ToString() + "} ";
			ret += "{" + "sizeX" + " : " + _sizeX.ToString() + "} ";
			ret += "{" + "sizeY" + " : " + _sizeY.ToString() + "} ";
			ret += "{" + "hoffsetX" + " : " + _hoffsetX.ToString() + "} ";
			ret += "{" + "hoffsetY" + " : " + _hoffsetY.ToString() + "} ";
			ret += "{" + "hsizeX" + " : " + _hsizeX.ToString() + "} ";
			ret += "{" + "hsizeY" + " : " + _hsizeY.ToString() + "} ";
			ret += "{" + "hp" + " : " + _hp.ToString() + "} ";
			return ret;
		}
	}
	public sealed class EnemyData : IGoogle2uDB
	{
		public enum rowIds {
			alligator, alpaca, bear, cat, deer, elephant, giraffe, Hippo, octopus, Penguin, pig
		};
		public string [] rowNames = {
			"alligator", "alpaca", "bear", "cat", "deer", "elephant", "giraffe", "Hippo", "octopus", "Penguin", "pig"
		};
		public System.Collections.Generic.List<EnemyDataRow> Rows = new System.Collections.Generic.List<EnemyDataRow>();

		public static EnemyData Instance
		{
			get { return NestedEnemyData.instance; }
		}

		private class NestedEnemyData
		{
			static NestedEnemyData() { }
			internal static readonly EnemyData instance = new EnemyData();
		}

		private EnemyData()
		{
			Rows.Add( new EnemyDataRow("alligator", "alligator", "alligator", "alligator", "-0.1769063", "0.4905846", "1.183871", "0.849007", "0.3338149", "1.248173", "1.035276", "0.5865326", "1"));
			Rows.Add( new EnemyDataRow("alpaca", "alpaca", "alpaca", "alpaca", "-0.05134553", "0.3823424", "1.002024", "0.6325226", "0.06970397", "1.135601", "0.7841546", "0.811676", "1"));
			Rows.Add( new EnemyDataRow("bear", "bear", "bear", "bear", "0.01667953", "0.6353957", "1.072771", "1.073326", "0.137729", "1.557357", "0.6807566", "0.7300463", "1"));
			Rows.Add( new EnemyDataRow("cat", "cat", "cat", "cat", "0.07509351", "0.4126101", "0.8531339", "0.6277542", "0.2508899", "1.044596", "0.49687", "0.5956676", "1"));
			Rows.Add( new EnemyDataRow("deer", "deer", "deer", "deer", "-0.0436327", "0.5292891", "0.9104499", "0.8611124", "0.2427053", "1.22261", "0.4805007", "0.4933609", "1"));
			Rows.Add( new EnemyDataRow("elephant", "elephant", "elephant", "elephant", "-0.05483519", "0.3866564", "1.09033", "0.7733129", "0.05483508", "1.104537", "1.10967", "0.58", "1"));
			Rows.Add( new EnemyDataRow("giraffe", "giraffe", "giraffe", "giraffe", "-0.09783268", "0.7831731", "1.167186", "1.243364", "-0.09783268", "0.7831731", "1.167186", "1.243364", "1"));
			Rows.Add( new EnemyDataRow("Hippo", "Hippo", "Hippo", "Hippo", "-0.02366436", "0.4055996", "0.9732078", "0.5836091", "0.1713896", "1.040042", "0.7258267", "0.6302874", "1"));
			Rows.Add( new EnemyDataRow("octopus", "octopus", "octopus", "octopus", "-0.005778193", "0.4592575", "0.6393362", "0.6909249", "-0.02237521", "1.114567", "0.6483209", "0.5766296", "1"));
			Rows.Add( new EnemyDataRow("Penguin", "Penguin", "Penguin", "Penguin", "0.02225922", "0.5555298", "0.9067713", "0.9788975", "0.06537434", "1.343426", "0.4983955", "0.5518949", "1"));
			Rows.Add( new EnemyDataRow("pig", "pig", "pig", "pig", "0.02105077", "0.4831055", "1.026866", "0.7386208", "0.1922564", "1.168224", "0.6960168", "0.6004777", "1"));
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
		public EnemyDataRow GetRow(rowIds in_RowID)
		{
			EnemyDataRow ret = null;
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
		public EnemyDataRow GetRow(string in_RowString)
		{
			EnemyDataRow ret = null;
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
