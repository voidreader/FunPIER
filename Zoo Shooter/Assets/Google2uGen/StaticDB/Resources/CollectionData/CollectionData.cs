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
	public class CollectionDataRow : IGoogle2uRow
	{
		public int _list;
		public string _sprite;
		public CollectionDataRow(string __ID, string __list, string __sprite) 
		{
			{
			int res;
				if(int.TryParse(__list, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_list = res;
				else
					Debug.LogError("Failed To Convert _list string: "+ __list +" to int");
			}
			_sprite = __sprite.Trim();
		}

		public int Length { get { return 2; } }

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
					ret = _sprite.ToString();
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
				case "sprite":
					ret = _sprite.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "list" + " : " + _list.ToString() + "} ";
			ret += "{" + "sprite" + " : " + _sprite.ToString() + "} ";
			return ret;
		}
	}
	public sealed class CollectionData : IGoogle2uDB
	{
		public enum rowIds {
			collection0, collection1, collection2, collection3, collection4, collection5, collection6, collection7, collection8, collection9
		};
		public string [] rowNames = {
			"collection0", "collection1", "collection2", "collection3", "collection4", "collection5", "collection6", "collection7", "collection8", "collection9"
		};
		public System.Collections.Generic.List<CollectionDataRow> Rows = new System.Collections.Generic.List<CollectionDataRow>();

		public static CollectionData Instance
		{
			get { return NestedCollectionData.instance; }
		}

		private class NestedCollectionData
		{
			static NestedCollectionData() { }
			internal static readonly CollectionData instance = new CollectionData();
		}

		private CollectionData()
		{
			Rows.Add( new CollectionDataRow("collection0", "0", "THEM"));
			Rows.Add( new CollectionDataRow("collection1", "1", "HALLOWEEN"));
			Rows.Add( new CollectionDataRow("collection2", "2", "BADMEN"));
			Rows.Add( new CollectionDataRow("collection3", "3", "STARFAMILY"));
			Rows.Add( new CollectionDataRow("collection4", "4", "HOMETOGETHER"));
			Rows.Add( new CollectionDataRow("collection5", "5", "SILENTMTN"));
			Rows.Add( new CollectionDataRow("collection6", "6", "neighborhood"));
			Rows.Add( new CollectionDataRow("collection7", "7", "SUICIDEANIMAL"));
			Rows.Add( new CollectionDataRow("collection8", "8", "PIRATESOFSEA"));
			Rows.Add( new CollectionDataRow("collection9", "9", "TOILETFIGHTER2"));
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
		public CollectionDataRow GetRow(rowIds in_RowID)
		{
			CollectionDataRow ret = null;
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
		public CollectionDataRow GetRow(string in_RowString)
		{
			CollectionDataRow ret = null;
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
