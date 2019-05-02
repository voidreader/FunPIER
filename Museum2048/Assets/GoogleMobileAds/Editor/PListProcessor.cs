using System.IO;

using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

public static class PListProcessor
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
#if UNITY_IOS
          // Replace with your iOS AdMob app ID. Your AdMob App ID will look
          // similar to this sample ID: ca-app-pub-3940256099942544~1458002511
          string appId = "ca-app-pub-8118299571958162~8667218810";

          string plistPath = Path.Combine(path, "Info.plist");
          PlistDocument plist = new PlistDocument();

          plist.ReadFromFile(plistPath);
          plist.root.SetString("GADApplicationIdentifier", appId);
          File.WriteAllText(plistPath, plist.WriteToString());
#endif
    }
}
