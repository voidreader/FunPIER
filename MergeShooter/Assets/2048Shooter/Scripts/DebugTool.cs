using UnityEngine;

public static class DebugTool
{
  private static bool enable = true;

  public static void SetDebugEnable(bool enable)
  {
    DebugTool.enable = enable;
  }

  public static void Log(string info)
  {
    if (!DebugTool.enable)
      return;
    Debug.Log((object) info);
  }

  public static void LogFormat(string info, params object[] objs)
  {
    if (!DebugTool.enable)
      return;
    Debug.LogFormat(info, objs);
  }

  public static void Error(string info)
  {
    if (!DebugTool.enable)
      return;
    Debug.LogError((object) info);
  }

  public static void ErrorFormat(string info, params object[] objs)
  {
    if (!DebugTool.enable)
      return;
    Debug.LogErrorFormat(info, objs);
  }

  public static void Warn(string info)
  {
    if (!DebugTool.enable)
      return;
    Debug.LogWarning((object) info);
  }

  public static void WarnFormat(string info, params object[] objs)
  {
    if (!DebugTool.enable)
      return;
    Debug.LogWarningFormat(info, objs);
  }
}