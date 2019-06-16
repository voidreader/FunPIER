using System;
using UnityEngine;

public static class tool
{
	public static Color HexcolorTofloat(string hexStr)
	{
		Color result;
		if (ColorUtility.TryParseHtmlString(hexStr, out result))
		{
			return result;
		}
		SingleInstance<DebugManager>.Instance.LogError(" error color translate faild");
		return Color.white;
	}

	public static ShootLevel getLevelFromJson(int level)
	{
		ShootLevelJson t = ReadJson.GetT<ShootLevelJson>("Level");
		if (t == null)
		{
			SingleInstance<DebugManager>.Instance.LogError("getLevelFromJson get data error");
		}
		return t.Data[level];
	}
}
