using System.IO;
using System.Text;
using System;
using System.Collections.Generic;
using System.Reflection;


namespace HT
{
	public class NEnum
	{
		public const int InvalidValue = GEnv.INVALID_TID;
		public const string InvalidKey = "";

		public static T Parse<T>(string value, bool ignoreCase, T defaultValue = default(T))
		{
			try
			{
				return (T)Enum.Parse(typeof(T), value, ignoreCase);
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}
	}

	public class NEnum<T> : Singleton<NEnum<T>>
	{
		private static Dictionary<object, string> _dicIS;
		private static Dictionary<int, int> _dicSI;


		public static int GetValue(string key)
		{
			if (_dicSI == null)
				_dicSI = Instance.ToDictionary_SI();

			int outValue;
			if (_dicSI.TryGetValue(key.ToLower().GetHashCode(), out outValue))
			{
				return outValue;
			}
			return NEnum.InvalidValue;
		}

		public static bool ContainsKey(int value)
		{
			if (_dicIS == null)
				_dicIS = Instance.ToDictionary_IS();

			return _dicIS.ContainsKey(value);
		}

		public static string GetKey(Type value)
		{
			if (_dicIS == null)
				_dicIS = Instance.ToDictionary_IS();

			string outValue;
			if (_dicIS.TryGetValue(value, out outValue))
			{
				return outValue;
			}
			return NEnum.InvalidKey;
		}

		protected Dictionary<object, string> ToDictionary_IS()
		{
			Dictionary<object, string> result = new Dictionary<object, string>();
			Type tp = typeof(T);
			FieldInfo[] members = tp.GetFields(BindingFlags.Static | BindingFlags.Public);

			int count = members.Length;
			for (int i = 0; i < count; ++i)
			{
				FieldInfo m = members[i];
				result.Add((int)m.GetValue(m), ((T)m.GetValue(m)).ToString());
			}
			return result;
		}

		protected Dictionary<int, int> ToDictionary_SI()
		{
			Dictionary<int, int> result = new Dictionary<int, int>();
			Type tp = typeof(T);
			FieldInfo[] members = tp.GetFields(BindingFlags.Static | BindingFlags.Public);
			string key;

			int count = members.Length;
			for (int i = 0; i < count; ++i)
			{
				FieldInfo m = members[i];
				key = ((T)m.GetValue(m)).ToString().ToLower();
				result.Add(key.GetHashCode(), (int)m.GetValue(m));
			}
			return result;
		}

		protected List<T> ToList()
		{
			List<T> result = new List<T>();
			Type tp = typeof(T);
			FieldInfo[] members = tp.GetFields(BindingFlags.Static | BindingFlags.Public);

			int count = members.Length;
			for (int i = 0; i < count; ++i)
			{
				FieldInfo m = members[i];
				result.Add(((T)m.GetValue(m)));
			}
			return result;
		}

		protected T[] ToArray()
		{
			List<T> result = new List<T>();
			Type tp = typeof(T);
			FieldInfo[] members = tp.GetFields(BindingFlags.Static | BindingFlags.Public);

			int count = members.Length;
			for (int i = 0; i < count; ++i)
			{
				FieldInfo m = members[i];
				result.Add(((T)m.GetValue(m)));
			}
			return result.ToArray();
		}
	}
}
