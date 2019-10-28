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
		public int _bid;
		public string _displayname;
		public string _spriteBody;
		public string _spriteLeg;
		public float _legX;
		public float _legY;
		public string _spriteHead;
		public float _headX;
		public float _headY;
		public float _boxoffsetx;
		public float _boxoffsety;
		public float _boxsizex;
		public float _boxsizey;
		public BossDataRow(string __ID, string __bid, string __displayname, string __spriteBody, string __spriteLeg, string __legX, string __legY, string __spriteHead, string __headX, string __headY, string __boxoffsetx, string __boxoffsety, string __boxsizex, string __boxsizey) 
		{
			{
			int res;
				if(int.TryParse(__bid, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_bid = res;
				else
					Debug.LogError("Failed To Convert _bid string: "+ __bid +" to int");
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
			_spriteHead = __spriteHead.Trim();
			{
			float res;
				if(float.TryParse(__headX, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_headX = res;
				else
					Debug.LogError("Failed To Convert _headX string: "+ __headX +" to float");
			}
			{
			float res;
				if(float.TryParse(__headY, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_headY = res;
				else
					Debug.LogError("Failed To Convert _headY string: "+ __headY +" to float");
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

		public int Length { get { return 13; } }

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
					ret = _bid.ToString();
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
					ret = _spriteHead.ToString();
					break;
				case 7:
					ret = _headX.ToString();
					break;
				case 8:
					ret = _headY.ToString();
					break;
				case 9:
					ret = _boxoffsetx.ToString();
					break;
				case 10:
					ret = _boxoffsety.ToString();
					break;
				case 11:
					ret = _boxsizex.ToString();
					break;
				case 12:
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
				case "bid":
					ret = _bid.ToString();
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
				case "spriteHead":
					ret = _spriteHead.ToString();
					break;
				case "headX":
					ret = _headX.ToString();
					break;
				case "headY":
					ret = _headY.ToString();
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
			ret += "{" + "bid" + " : " + _bid.ToString() + "} ";
			ret += "{" + "displayname" + " : " + _displayname.ToString() + "} ";
			ret += "{" + "spriteBody" + " : " + _spriteBody.ToString() + "} ";
			ret += "{" + "spriteLeg" + " : " + _spriteLeg.ToString() + "} ";
			ret += "{" + "legX" + " : " + _legX.ToString() + "} ";
			ret += "{" + "legY" + " : " + _legY.ToString() + "} ";
			ret += "{" + "spriteHead" + " : " + _spriteHead.ToString() + "} ";
			ret += "{" + "headX" + " : " + _headX.ToString() + "} ";
			ret += "{" + "headY" + " : " + _headY.ToString() + "} ";
			ret += "{" + "boxoffsetx" + " : " + _boxoffsetx.ToString() + "} ";
			ret += "{" + "boxoffsety" + " : " + _boxoffsety.ToString() + "} ";
			ret += "{" + "boxsizex" + " : " + _boxsizex.ToString() + "} ";
			ret += "{" + "boxsizey" + " : " + _boxsizey.ToString() + "} ";
			return ret;
		}
	}
	public sealed class BossData : IGoogle2uDB
	{
		public enum rowIds {
			boss1, boss2, boss3, boss4, boss5, boss6, boss7, boss8, boss9, boss10, boss11, boss12
		};
		public string [] rowNames = {
			"boss1", "boss2", "boss3", "boss4", "boss5", "boss6", "boss7", "boss8", "boss9", "boss10", "boss11", "boss12"
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
			Rows.Add( new BossDataRow("boss1", "1", "Boss-1", "1-body", "1-leg", "0.02", "-0.44", "1-head", "0.04", "0.65", "0.004303575", "-0.228092", "0.6298888", "0.5438161"));
			Rows.Add( new BossDataRow("boss2", "2", "Boss-2", "2-body", "2-leg", "0", "0", "2-head", "0", "0", "0.06455421", "-0.09898329", "0.5093875", "0.8020334"));
			Rows.Add( new BossDataRow("boss3", "3", "Boss-3", "3-body", "3-leg", "0", "0", "3-head", "0", "0", "-0.05164349", "-0.09898329", "0.6212814", "0.8020334"));
			Rows.Add( new BossDataRow("boss4", "4", "Boss-4", "4-body", "4-leg", "0", "0", "4-head", "0", "0", "-0.05164349", "-0.03012514", "0.6212814", "0.5093861"));
			Rows.Add( new BossDataRow("boss5", "5", "Boss-5", "5-body", "5-leg", "0", "0", "5-head", "0", "0", "-0.05164349", "-0.03012514", "0.6212814", "0.5093861"));
			Rows.Add( new BossDataRow("boss6", "6", "Boss-6", "6-body", "6-leg", "0", "0", "6-head", "0", "0", "-0.05164349", "-0.08607244", "0.6212814", "0.5524225"));
			Rows.Add( new BossDataRow("boss7", "7", "Boss-7", "7-body", "7-leg", "0", "0", "7-head", "0", "0", "-0.03873265", "-0.1893592", "0.7748187", "0.4622847"));
			Rows.Add( new BossDataRow("boss8", "8", "Boss-8", "8-body", "8-leg", "0", "0", "8-head", "0", "0", "0", "0", "0", "0"));
			Rows.Add( new BossDataRow("boss9", "9", "Boss-9", "9-body", "9-leg", "0", "0", "9-head", "0", "0", "0", "0", "0", "0"));
			Rows.Add( new BossDataRow("boss10", "10", "Boss-10", "10-body", "10-leg", "0", "0", "10-head", "0", "0", "0", "0", "0", "0"));
			Rows.Add( new BossDataRow("boss11", "11", "Boss-11", "11-body", "11-leg", "0", "0", "11-head", "0", "0", "0", "0", "0", "0"));
			Rows.Add( new BossDataRow("boss12", "12", "Boss-12", "12-body", "12-leg", "0", "0", "12-head", "0", "0", "0", "0", "0", "0"));
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
