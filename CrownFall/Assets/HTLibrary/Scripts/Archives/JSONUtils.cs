using UnityEngine;
using System;
using System.Collections.Generic;

namespace HT
{
	public enum JSONSerializationMode
	{
		Normal,
	}
	public interface IJSONSerialization
	{
		bool ParseJSON(JSONNode node, JSONSerializationMode mode = JSONSerializationMode.Normal);
		JSONNode ToJSON(JSONSerializationMode mode = JSONSerializationMode.Normal);
	}


	public class JSONUtils
	{
		public static void AddValue<T>(JSONNode node, string key, T value)
		{
			node[key] = value.ToString();
		}

		public static void AddValue<T>(JSONNode node, string key, List<T> values)
		{
			JSONArray arrayNodes = new JSONArray();
			for (int i = 0; i < values.Count; ++i)
			{
				AddValue(arrayNodes, "", values[i]);
			}
			AddArrayNode(node, key, arrayNodes);
		}

		public static void AddValue<T>(JSONNode node, string key, T[] values)
		{
			JSONArray arrayNodes = new JSONArray();
			for (int i = 0; i < values.Length; ++i)
			{
				AddValue(arrayNodes, "", values[i]);
			}
			AddArrayNode(node, key, arrayNodes);
		}

		public static void AddValue(JSONNode node, string key, Vector2 value)
		{
			node[key] = string.Format("{0},{1}", value.x, value.y);
		}

		public static void AddValue(JSONNode node, string key, Vector3 value)
		{
			node[key] = string.Format("{0},{1},{2}", value.x, value.y, value.z);
		}

		public static void AddValue(JSONNode node, string key, Quaternion value)
		{
			node[key] = string.Format("{0},{1},{2},{3}", value.x, value.y, value.z, value.w);
		}

		public static void AddValue(JSONNode node, string key, CFloat value)
		{
			node[key].AsFloat = value.val;
		}

		public static void AddValue(JSONNode node, string key, CInt value)
		{
			node[key].AsInt = value.val;
		}

		public static void AddValue(JSONNode node, string key, CByte value)
		{
			node[key].AsByte = value.val;
		}

		public static void AddValue(JSONNode node, string key, Color value)
		{
			node[key] = string.Format("{0},{1},{2},{3}", (byte)(value.r * 255), (byte)(value.g * 255), (byte)(value.b * 255), (byte)(value.a * 255));
		}

		public static void AddValue(JSONNode node, string key, DateTime value)
		{
			node[key] = value.ToString("yyyy-MM-dd HH:mm:ss");
		}

		public static void AddValue(JSONArray node, JSONNode childNode)
		{
			node.Add(childNode);
		}

		public static void AddValueDefaultNoAdd<T>(JSONNode node, string key, T value, T defaultValue)
		{
			if (value.Equals(defaultValue))
				return;

			node[key] = value.ToString();
		}

		public static void AddValueDefaultNoAdd(JSONNode node, string key, Vector2 value, Vector2 defaultValue)
		{
			if (value == defaultValue)
				return;

			node[key] = string.Format("{0},{1}", value.x, value.y);
		}

		public static void AddValueDefaultNoAdd(JSONNode node, string key, Vector3 value, Vector3 defaultValue)
		{
			if (value == defaultValue)
				return;

			node[key] = string.Format("{0},{1},{2}", value.x, value.y, value.z);
		}

		public static void AddValueDefaultNoAdd(JSONNode node, string key, Quaternion value, Quaternion defaultValue)
		{
			if (value == defaultValue)
				return;

			node[key] = string.Format("{0},{1},{2},{3}", value.x, value.y, value.z, value.w);
		}

		public static void AddValueDefaultNoAdd(JSONNode node, string key, DateTime value, DateTime defaultValue)
		{
			if (value == defaultValue)
				return;

			node[key] = value.ToString("yyyy-MM-dd HH:mm:ss");
		}

		public static void AddNode(JSONNode node, string key, JSONNode childNode)
		{
			if (childNode == null)
				return;

			node[key] = childNode;
		}
		public static void AddClassNode(JSONNode node, string key, JSONClass childNode)
		{
			if (childNode == null)
				return;

			node[key] = childNode;
		}
		public static void AddArrayNode(JSONNode node, string key, JSONArray childNode)
		{
			if (childNode == null)
				return;

			node[key] = childNode;
		}

		public static void AddValueSerialize<T>(JSONNode node, string key, ref T target
			, JSONSerializationMode mode = JSONSerializationMode.Normal) where T : IJSONSerialization
		{
			node[key] = target.ToJSON(mode);
		}

		public static void AddValueSerialize<T>(JSONNode node, string key, List<T> values
			, JSONSerializationMode mode = JSONSerializationMode.Normal) where T : IJSONSerialization
		{
			JSONArray arrayNodes = new JSONArray();
			for (int i = 0; i < values.Count; ++i)
			{
				arrayNodes.Add(values[i].ToJSON(mode));
			}
			AddArrayNode(node, key, arrayNodes);
		}

		public static void AddValueSerialize<T>(JSONNode node, string key, T[] values
			, JSONSerializationMode mode = JSONSerializationMode.Normal) where T : IJSONSerialization
		{
			JSONArray arrayNodes = new JSONArray();
			for (int i = 0; i < values.Length; ++i)
			{
				arrayNodes.Add(values[i].ToJSON(mode));
			}
			AddArrayNode(node, key, arrayNodes);
		}

		#region GetValue
		// IJSONSerialization
		public static bool GetValueSerialize<T>(JSONNode node, int index, ref T target
			, JSONSerializationMode mode = JSONSerializationMode.Normal) where T : IJSONSerialization
		{
			if (node.Count <= index)
			{
				return false;
			}

			return target.ParseJSON(node[index], mode);
		}

		public static bool GetValueSerialize<T>(JSONNode node, string key, ref T target
			, JSONSerializationMode mode = JSONSerializationMode.Normal) where T : IJSONSerialization
		{
			if (node.Contains(key) == false)
			{
				return false;
			}

			return target.ParseJSON(node[key], mode);
		}

		public static bool GetValueSerialize<T>(JSONNode node, string key, out T[] target
			, JSONSerializationMode mode = JSONSerializationMode.Normal) where T : class, IJSONSerialization, new()
		{
			List<T> items = new List<T>();
			if (GetValueSerialize<T>(node, key, ref items, false, mode))
			{
				target = items.ToArray();
				return true;
			}
			target = new T[0];
			return false;
		}

		public static bool GetValueSerialize<T>(JSONNode node, string key, ref List<T> target, bool clear = false
			, JSONSerializationMode mode = JSONSerializationMode.Normal) where T : IJSONSerialization, new()
		{
			if (clear)
			{
				target.Clear();
			}
			JSONArray arrayNode;
			if (GetNode(node, key, out arrayNode))
			{
				for (int i = 0; i < arrayNode.Count; ++i)
				{
					T t = new T();
					GetValueSerialize(arrayNode, i, ref t, mode);
					target.Add(t);
				}
				return true;
			}
			return false;
		}

		// Array
		public static bool GetValueArray<T>(JSONNode node, string key, out T[] outValues, T defaultValue = default(T))
		{

			JSONArray arrayNode;
			if (GetNode(node, key, out arrayNode))
			{
				outValues = new T[arrayNode.Count];
				for (int i = 0; i < arrayNode.Count; ++i)
				{
					T value = NEnum.Parse(arrayNode[i], true, defaultValue);
					outValues[i] = value;
				}
				return true;
			}
			outValues = new T[0];
			return false;
		}

		// List
		public static bool GetValueEnumList<T>(JSONNode node, string key, ref List<T> outValues, T defaultValue = default(T), bool clear = false)
		{
			if (clear)
			{
				outValues.Clear();
			}
			JSONArray arrayNode;
			if (GetNode(node, key, out arrayNode))
			{
				for (int i = 0; i < arrayNode.Count; ++i)
				{
					T value = NEnum.Parse(arrayNode[i], true, defaultValue);
					outValues.Add(value);
				}
				return true;
			}
			return false;
		}

		// JSONNode
		public static bool GetNode<T>(JSONNode node, string key, out T outValue, T defaultValue = null) where T : JSONNode
		{
			if (node.Contains(key) == false)
			{
				outValue = defaultValue;
				return false;
			}
			outValue = node[key] as T;
			return true;
		}

		#region GetValueList
		// list - string
		public static bool GetValueList(JSONNode node, string key, ref List<string> outValues, string defaultValue = default(string), bool clear = false)
		{
			if (clear)
				outValues.Clear();

			JSONArray arrayNode;
			if (GetNode(node, key, out arrayNode) == false)
				return false;

			int nodeCount = arrayNode.Count;
			for (int i = 0; i < nodeCount; ++i)
			{
				outValues.Add(arrayNode[i]);
			}
			return true;
		}

		// list - int
		public static bool GetValueList(JSONNode node, string key, ref List<int> outValues, int defaultValue = default(int), bool clear = false)
		{
			if (clear)
				outValues.Clear();

			JSONArray arrayNode;
			if (GetNode(node, key, out arrayNode) == false)
				return false;

			int nodeCount = arrayNode.Count;
			for (int i = 0; i < nodeCount; ++i)
			{
				outValues.Add(arrayNode[i].AsInt);
			}
			return true;
		}
		// list - short
		public static bool GetValueList(JSONNode node, string key, ref List<short> outValues, short defaultValue = default(short), bool clear = false)
		{
			if (clear)
				outValues.Clear();

			JSONArray arrayNode;
			if (GetNode(node, key, out arrayNode) == false)
				return false;

			int nodeCount = arrayNode.Count;
			for (int i = 0; i < nodeCount; ++i)
			{
				outValues.Add(arrayNode[i].AsShort);
			}
			return true;
		}
		// list - long
		public static bool GetValueList(JSONNode node, string key, ref List<long> outValues, long defaultValue = default(long), bool clear = false)
		{
			if (clear)
				outValues.Clear();

			JSONArray arrayNode;
			if (GetNode(node, key, out arrayNode) == false)
				return false;

			int nodeCount = arrayNode.Count;
			for (int i = 0; i < nodeCount; ++i)
			{
				outValues.Add(arrayNode[i].AsLong);
			}
			return true;
		}
		// list - byte
		public static bool GetValueList(JSONNode node, string key, ref List<byte> outValues, byte defaultValue = default(byte), bool clear = false)
		{
			if (clear)
				outValues.Clear();

			JSONArray arrayNode;
			if (GetNode(node, key, out arrayNode) == false)
				return false;

			int nodeCount = arrayNode.Count;
			for (int i = 0; i < nodeCount; ++i)
			{
				outValues.Add(arrayNode[i].AsByte);
			}
			return true;
		}
		// list - bool
		public static bool GetValueList(JSONNode node, string key, ref List<bool> outValues, bool defaultValue = default(bool), bool clear = false)
		{
			if (clear)
				outValues.Clear();

			JSONArray arrayNode;
			if (GetNode(node, key, out arrayNode) == false)
				return false;

			int nodeCount = arrayNode.Count;
			for (int i = 0; i < nodeCount; ++i)
			{
				outValues.Add(arrayNode[i].AsBool);
			}
			return true;
		}
		#endregion // GetValueList

		// string
		public static bool GetValue(JSONNode node, string key, out string outValue, string defaultValue = "")
		{
			if (node.Contains(key) == false)
			{
				outValue = defaultValue;
				return false;
			}
			outValue = node[key];
			return true;
		}

		// Vector2
		public static bool GetValue(JSONNode node, string key, out Vector2 outValue, Vector2 defaultValue = default(Vector2))
		{
			outValue = defaultValue;
			if (node.Contains(key) == false)
				return false;


			string v = node[key];
			string[] values = v.Trim().Split(',');
			if (values.Length > 0)
			{
				float.TryParse(values[0].Trim(), out outValue.x);
			}

			if (values.Length > 1)
			{
				float.TryParse(values[1].Trim(), out outValue.y);
			}
			return true;
		}

		// Vector3
		public static bool GetValue(JSONNode node, string key, out Vector3 outValue, Vector3 defaultValue = default(Vector3))
		{
			outValue = defaultValue;
			if (node.Contains(key) == false)
				return false;


			string v = node[key];
			string[] values = v.Trim().Split(',');
			if (values.Length > 0)
			{
				float.TryParse(values[0].Trim(), out outValue.x);
			}

			if (values.Length > 1)
			{
				float.TryParse(values[1].Trim(), out outValue.y);
			}

			if (values.Length > 2)
			{
				float.TryParse(values[2].Trim(), out outValue.z);
			}
			return true;
		}

		// Quaternion
		public static bool GetValue(JSONNode node, string key, out Quaternion outValue, Quaternion defaultValue = default(Quaternion))
		{
			outValue = defaultValue;
			if (node.Contains(key) == false)
				return false;


			string v = node[key];
			string[] values = v.Trim().Split(',');
			if (values.Length > 0)
			{
				float.TryParse(values[0].Trim(), out outValue.x);
			}

			if (values.Length > 1)
			{
				float.TryParse(values[1].Trim(), out outValue.y);
			}

			if (values.Length > 2)
			{
				float.TryParse(values[2].Trim(), out outValue.z);
			}

			if (values.Length > 3)
			{
				float.TryParse(values[3].Trim(), out outValue.w);
			}
			return true;
		}

		// byte
		public static bool GetValue(JSONNode node, string key, out byte outValue, byte defaultValue = 0)
		{
			if (node.Contains(key) == false)
			{
				outValue = defaultValue;
				return false;
			}
			outValue = node[key].AsByte;
			return true;
		}

		// short
		public static bool GetValue(JSONNode node, string key, out short outValue, short defaultValue = 0)
		{
			if (node.Contains(key) == false)
			{
				outValue = defaultValue;
				return false;
			}
			outValue = node[key].AsShort;
			return true;
		}

		// ushort
		public static bool GetValue(JSONNode node, string key, out ushort outValue, ushort defaultValue = 0)
		{
			if (node.Contains(key) == false)
			{
				outValue = defaultValue;
				return false;
			}
			outValue = node[key].AsUShort;
			return true;
		}

		// int
		public static bool GetValue(JSONNode node, string key, out int outValue, int defaultValue = 0)
		{
			if (node.Contains(key) == false)
			{
				outValue = defaultValue;
				return false;
			}
			outValue = node[key].AsInt;
			return true;
		}

		// uint
		public static bool GetValue(JSONNode node, string key, out uint outValue, uint defaultValue = 0)
		{
			if (node.Contains(key) == false)
			{
				outValue = defaultValue;
				return false;
			}
			outValue = node[key].AsUInt;
			return true;
		}

		// long
		public static bool GetValue(JSONNode node, string key, out long outValue, long defaultValue = 0)
		{
			if (node.Contains(key) == false)
			{
				outValue = defaultValue;
				return false;
			}
			outValue = node[key].AsLong;
			return true;
		}

		// ulong
		public static bool GetValue(JSONNode node, string key, out ulong outValue, ulong defaultValue = 0)
		{
			if (node.Contains(key) == false)
			{
				outValue = defaultValue;
				return false;
			}
			outValue = node[key].AsULong;
			return true;
		}

		// float
		public static bool GetValue(JSONNode node, string key, out float outValue, float defaultValue = 0)
		{
			if (node.Contains(key) == false)
			{
				outValue = defaultValue;
				return false;
			}
			outValue = node[key].AsFloat;
			return true;
		}

		// double
		public static bool GetValue(JSONNode node, string key, out double outValue, double defaultValue = 0)
		{
			if (node.Contains(key) == false)
			{
				outValue = defaultValue;
				return false;
			}
			outValue = node[key].AsDouble;
			return true;
		}

		// bool
		public static bool GetValue(JSONNode node, string key, out bool outValue, bool defaultValue = false)
		{
			if (node.Contains(key) == false)
			{
				outValue = defaultValue;
				return false;
			}
			outValue = node[key].AsBool;
			return true;
		}

		// enum
		public static bool GetValueEnum<T>(JSONNode node, string key, out T outValue, T defaultValue = default(T)) where T : struct
		{
			if (node.Contains(key) == false)
			{
				outValue = defaultValue;
				return false;
			}

			try
			{
				outValue = (T)Enum.Parse(typeof(T), node[key], true);
			}
			catch (Exception)
			{
				outValue = defaultValue;
				return false;
			}

			return true;
		}

		// crypted float
		public static bool GetValue(JSONNode node, string key, ref CFloat outValue, float defaultValue = 0)
		{
			if (node.Contains(key) == false)
			{
				outValue.val = defaultValue;
				return false;
			}
			outValue.val = node[key].AsFloat;
			return true;
		}

		// crypted int
		public static bool GetValue(JSONNode node, string key, ref CInt outValue, int defaultValue = 0)
		{
			if (node.Contains(key) == false)
			{
				outValue.val = defaultValue;
				return false;
			}
			outValue.val = node[key].AsInt;
			return true;
		}

		// crypted byte
		public static bool GetValue(JSONNode node, string key, ref CByte outValue, byte defaultValue = 0)
		{
			if (node.Contains(key) == false)
			{
				outValue.val = defaultValue;
				return false;
			}
			outValue.val = node[key].AsByte;
			return true;
		}

		// color
		public static bool GetValue(JSONNode node, string key, out Color outValue)
		{
			return GetValue(node, key, out outValue, Color.white);
		}

		// color
		public static bool GetValue(JSONNode node, string key, out Color outValue, Color defaultValue)
		{
			if (node.Contains(key) == false)
			{
				outValue = defaultValue;
				return false;
			}

			string v = node[key];
			int nIndex = -1;
			int prevIndex = 0;

			byte[] colorByte = new byte[4];
			for (int nInd = 0; nInd < colorByte.Length; ++nInd)
			{
				prevIndex = nIndex + 1;
				nIndex = v.IndexOf(",", prevIndex);

				if (nIndex < 0)
					nIndex = v.Length;

				string szSubStr = v.Substring(prevIndex, nIndex - prevIndex);
				byte.TryParse(szSubStr, out colorByte[nInd]);
			}

			outValue = new Color(colorByte[0] / 255.0f, colorByte[1] / 255.0f, colorByte[2] / 255.0f, colorByte[3] / 255.0f);
			return true;
		}

		// DateTime
		public static bool GetValue(JSONNode node, string key, out DateTime outValue)
		{
			return GetValue(node, key, out outValue, DateTime.MinValue);
		}

		public static bool GetValue(JSONNode node, string key, out DateTime outValue, DateTime defaultValue)
		{
			if (node.Contains(key) == false)
			{
				outValue = defaultValue;
				return false;
			}

			string v = node[key];
			return DateTime.TryParse(v, out outValue);
		}
		#endregion
	}
}
