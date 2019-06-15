using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParsingDictionary
{

    public static string ToString(object obj, string key, string defaultValue = "")
    {
        Dictionary<string, object> dic = obj as Dictionary<string, object>;

        if (!dic.ContainsKey(key))
            return defaultValue;
        if (dic[key] == null)
            return defaultValue;
        return dic[key].ToString();
    }

    public static int ToInt(object obj, string key)
    {
        Dictionary<string, object> dic = obj as Dictionary<string, object>;

        string str = ToString(dic, key);
        if (str.Length == 0)
            return 0;

        int ret = 0;
        bool parsed = int.TryParse(str, out ret);
        if (!parsed)
        {
            // int 파싱에 실패하면 float 파싱을 시도하고 int형으로 변환합니다.
            float f_ret;
            parsed = float.TryParse(str, out f_ret);
            ret = (int)f_ret;
        }
        return ret;
    }

    public static long ToLong(Dictionary<string, object> dic, string key)
    {
        string str = ToString(dic, key);
        if (str.Length == 0)
            return 0;

        long ret = 0;
        bool parsed = long.TryParse(str, out ret);
        if (!parsed)
        {
            // long 파싱에 실패하면 double 파싱을 시도하고 long형으로 변환합니다.
            double f_ret;
            parsed = double.TryParse(str, out f_ret);
            ret = (long)f_ret;
        }
        return ret;
    }

    public static float ToFloat(object obj, string key)
    {
        Dictionary<string, object> dic = obj as Dictionary<string, object>;

        string str = ToString(dic, key);
        if (str.Length == 0)
            return 0;

        float ret = float.Parse(str);
        return ret;
    }

    public static bool ToBool(Dictionary<string, object> dic, string key)
    {
        int i = ToInt(dic, key);

        return i == 0 ? false : true;
    }

    public static Dictionary<string, object> ToDictionary(Dictionary<string, object> dic, string key)
    {
        return dic[key] as Dictionary<string, object>;
    }

    public static ArrayList ToArrayList(Dictionary<string, object> dic, string key)
    {
        return dic[key] as ArrayList;
    }

}
