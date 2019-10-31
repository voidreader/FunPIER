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
	public class MinionDataRow : IGoogle2uRow
	{
		public int _mid;
		public string _displayname;
		public string _spriteBody;
		public string _spriteLeg;
		public float _legX;
		public float _legY;
		public float _boxoffsetx;
		public float _boxoffsety;
		public float _boxsizex;
		public float _boxsizey;
		public MinionDataRow(string __ID, string __mid, string __displayname, string __spriteBody, string __spriteLeg, string __legX, string __legY, string __boxoffsetx, string __boxoffsety, string __boxsizex, string __boxsizey) 
		{
			{
			int res;
				if(int.TryParse(__mid, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_mid = res;
				else
					Debug.LogError("Failed To Convert _mid string: "+ __mid +" to int");
			}
			_displayname = __displayname.Trim();
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
			{
			float res;
				if(float.TryParse(__boxoffsetx, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_boxoffsetx = res;
				else
					Debug.LogError("Failed To Convert _boxoffsetx string: "+ __boxoffsetx +" to float");
			}
			{
			float res;
				if(float.TryParse(__boxoffsety, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_boxoffsety = res;
				else
					Debug.LogError("Failed To Convert _boxoffsety string: "+ __boxoffsety +" to float");
			}
			{
			float res;
				if(float.TryParse(__boxsizex, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_boxsizex = res;
				else
					Debug.LogError("Failed To Convert _boxsizex string: "+ __boxsizex +" to float");
			}
			{
			float res;
				if(float.TryParse(__boxsizey, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_boxsizey = res;
				else
					Debug.LogError("Failed To Convert _boxsizey string: "+ __boxsizey +" to float");
			}
		}

		public int Length { get { return 10; } }

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
					ret = _mid.ToString();
					break;
				case 1:
					ret = _displayname.ToString();
					break;
				case 2:
					ret = _spriteBody.ToString();
					break;
				case 3:
					ret = _spriteLeg.ToString();
					break;
				case 4:
					ret = _legX.ToString();
					break;
				case 5:
					ret = _legY.ToString();
					break;
				case 6:
					ret = _boxoffsetx.ToString();
					break;
				case 7:
					ret = _boxoffsety.ToString();
					break;
				case 8:
					ret = _boxsizex.ToString();
					break;
				case 9:
					ret = _boxsizey.ToString();
					break;
			}

			return ret;
		}

		public string GetStringData( string colID )
		{
			var ret = System.String.Empty;
			switch( colID )
			{
				case "mid":
					ret = _mid.ToString();
					break;
				case "displayname":
					ret = _displayname.ToString();
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
				case "boxoffsetx":
					ret = _boxoffsetx.ToString();
					break;
				case "boxoffsety":
					ret = _boxoffsety.ToString();
					break;
				case "boxsizex":
					ret = _boxsizex.ToString();
					break;
				case "boxsizey":
					ret = _boxsizey.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "mid" + " : " + _mid.ToString() + "} ";
			ret += "{" + "displayname" + " : " + _displayname.ToString() + "} ";
			ret += "{" + "spriteBody" + " : " + _spriteBody.ToString() + "} ";
			ret += "{" + "spriteLeg" + " : " + _spriteLeg.ToString() + "} ";
			ret += "{" + "legX" + " : " + _legX.ToString() + "} ";
			ret += "{" + "legY" + " : " + _legY.ToString() + "} ";
			ret += "{" + "boxoffsetx" + " : " + _boxoffsetx.ToString() + "} ";
			ret += "{" + "boxoffsety" + " : " + _boxoffsety.ToString() + "} ";
			ret += "{" + "boxsizex" + " : " + _boxsizex.ToString() + "} ";
			ret += "{" + "boxsizey" + " : " + _boxsizey.ToString() + "} ";
			return ret;
		}
	}
	public sealed class MinionData : IGoogle2uDB
	{
		public enum rowIds {
			minion1, minion2, minion3, minion4, minion5, minion6, minion7
		};
		public string [] rowNames = {
			"minion1", "minion2", "minion3", "minion4", "minion5", "minion6", "minion7"
		};
		public System.Collections.Generic.List<MinionDataRow> Rows = new System.Collections.Generic.List<MinionDataRow>();

		public static MinionData Instance
		{
			get { return NestedMinionData.instance; }
		}

		private class NestedMinionData
		{
			static NestedMinionData() { }
			internal static readonly MinionData instance = new MinionData();
		}

		private MinionData()
		{
			Rows.Add( new MinionDataRow("minion1", "1", "Minion-1", "1-body", "1-leg", "0", "-0.8", "0.2365883", "-0.2517949", "0.8479521", "1.558284"));
			Rows.Add( new MinionDataRow("minion2", "2", "Minion-2", "2-body", "2-leg", "0", "-0.7", "0.4343138", "-0.226977", "1.258388", "1.361413"));
			Rows.Add( new MinionDataRow("minion3", "3", "Minion-3", "3-body", "3-leg", "-0.05", "-0.94", "0.1711604", "-0.2222365", "0.820383", "1.958717"));
			Rows.Add( new MinionDataRow("minion4", "4", "Minion-4", "4-body", "4-leg", "0", "-0.7", "0.2280464", "-0.3477389", "0.9910412", "1.39112"));
			Rows.Add( new MinionDataRow("minion5", "5", "Minion-5", "5-body", "5-leg", "-0.1", "-0.79", "0.2280464", "-0.4235873", "1.123775", "1.637627"));
			Rows.Add( new MinionDataRow("minion6", "6", "Minion-6", "6-body", "6-leg", "-0.1", "-0.64", "0.1237553", "-0.2188067", "0.8014208", "1.272979"));
			Rows.Add( new MinionDataRow("minion7", "7", "Minion-7", "7-body", "7-leg", "-0.1", "-0.69", "0.2646596", "-0.3315745", "1.154059", "1.201804"));
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
		public MinionDataRow GetRow(rowIds in_RowID)
		{
			MinionDataRow ret = null;
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
		public MinionDataRow GetRow(string in_RowString)
		{
			MinionDataRow ret = null;
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
