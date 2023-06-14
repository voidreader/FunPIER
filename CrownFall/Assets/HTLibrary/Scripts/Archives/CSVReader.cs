using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HT
{
	public class CSVReader
	{
		//---------------------------------------
		static readonly string SPLIT_RE = @"/(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
		static readonly string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
		static readonly char[] TRIM_CHARS = { '\"' };

		//---------------------------------------
		public static List<Dictionary<string, object>> Read(string file)
		{
			TextAsset data = Resources.Load(file) as TextAsset;
			return ReadFromString(data.text);

		}

		//---------------------------------------
		public static List<Dictionary<string, object>> ReadFromFile(string file)
		{
			string szFileTexts = File.ReadAllText(file);
			return ReadFromString(szFileTexts);
		}

		//---------------------------------------
		public static List<Dictionary<string, object>> ReadFromString(string szText)
		{
			var list = new List<Dictionary<string, object>>();
			var lines = Regex.Split(szText, LINE_SPLIT_RE);

			if (lines.Length <= 1) return list;

			var header = Regex.Split(lines[0], SPLIT_RE);
			for (var i = 1; i < lines.Length; i++)
			{

				var values = Regex.Split(lines[i], SPLIT_RE);
				if (values.Length == 0 || values[0] == "") continue;

				var entry = new Dictionary<string, object>();
				for (var j = 0; j < header.Length && j < values.Length; j++)
				{
					string value = values[j];
					value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
					object finalvalue = value;
					int n;
					float f;
					if (int.TryParse(value, out n))
					{
						finalvalue = n;
					}
					else if (float.TryParse(value, out f))
					{
						finalvalue = f;
					}
					entry[header[j]] = finalvalue;
				}
				list.Add(entry);
			}
			return list;
		}
	}
}