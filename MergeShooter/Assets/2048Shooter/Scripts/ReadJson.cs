using UnityEngine;

public static class ReadJson
{
  public static T GetT<T>(string name)
  {
    TextAsset textAsset = (TextAsset) Resources.Load<TextAsset>("Json/" + name);
    if (textAsset==null)
      return default (T);
    return JsonUtility.FromJson<T>(textAsset.text);
  }
}
