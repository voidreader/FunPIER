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
			Rows.Add( new EnemyDataRow("alligator", "alligator", "alligator", "alligator", "-0.1998554", "-0.258684", "1.206346", "0.8023167", "0.2868674", "0.4918259", "1.095168", "0.5437449", "1"));
			Rows.Add( new EnemyDataRow("alpaca", "alpaca", "alpaca", "alpaca", "-0.04292327", "-0.3871344", "1.008498", "0.6759346", "0.06168884", "0.3097668", "0.7693776", "0.668312", "1"));
			Rows.Add( new EnemyDataRow("bear", "bear", "bear", "bear", "-0.02336353", "-0.3643147", "1.047617", "1.008451", "0.1301482", "0.5281846", "0.6324589", "0.6878716", "1"));
			Rows.Add( new EnemyDataRow("cat", "cat", "cat", "cat", "0.07443547", "-0.2795554", "0.8520193", "0.6954941", "0.2638068", "0.3488864", "0.4694605", "0.4987931", "1"));
			Rows.Add( new EnemyDataRow("deer", "deer", "deer", "deer", "-0.03640345", "-0.3056351", "0.8780989", "0.8519724", "0.2344671", "0.3749661", "0.4759805", "0.4466337", "1"));
			Rows.Add( new EnemyDataRow("elephant", "elephant", "elephant", "elephant", "-0.02988353", "-0.2860752", "0.9432982", "0.7085338", "0.1203682", "0.384746", "0.7432979", "0.6096321", "1"));
			Rows.Add( new EnemyDataRow("giraffe", "giraffe", "giraffe", "giraffe", "-0.08779693", "-0.439375", "1.140885", "1.096894", "0.2181673", "0.5999039", "0.5607398", "0.9225887", "1"));
			Rows.Add( new EnemyDataRow("Hippo", "Hippo", "Hippo", "Hippo", "-0.108237", "-0.3133284", "1.100005", "0.6131476", "0.1466274", "0.3444041", "0.6765664", "0.6568694", "1"));
			Rows.Add( new EnemyDataRow("octopus", "octopus", "octopus", "octopus", "-0.002630472", "-0.3337683", "0.9705513", "0.6540275", "-0.006672591", "0.3512175", "0.6016197", "0.6704962", "1"));
			Rows.Add( new EnemyDataRow("Penguin", "Penguin", "Penguin", "Penguin", "-0.02307048", "-0.2656351", "0.9296712", "0.9538144", "-0.006672591", "0.5249574", "0.6016197", "0.568296", "1"));
			Rows.Add( new EnemyDataRow("pig", "pig", "pig", "pig", "0.01780951", "-0.2792618", "1.011431", "0.7766679", "0.1772873", "0.4329778", "0.6970063", "0.5751091", "1"));
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
