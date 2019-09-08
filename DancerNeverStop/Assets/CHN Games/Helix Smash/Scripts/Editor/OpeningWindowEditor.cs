using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Globalization;
using UnityEngine;
[InitializeOnLoad]
public class OpeningWindowEditor : Editor
{
    private static string version = "1.6"; 
    private static string message = "Thank you for choosing us";
    private static char dot = '\u2022';
    private static string whatsnew = dot+ " Haptic/Vibration feedback is fixed for some bugs.\n" + dot + " Burning effect credit link is corrected in credits UI and document.\n";
    private static string assetLink = "https://assetstore.unity.com/packages/slug/147944";
    static OpeningWindowEditor()
    {  
        if(!EditorApplication.isPlayingOrWillChangePlaymode)EditorApplication.delayCall += OpenWindow;
    }
    static void OpenWindow()
    {
        var oldVersion = EditorPrefs.GetString("asset_version", "0");
        if (float.Parse(oldVersion, CultureInfo.InvariantCulture) < float.Parse(version, CultureInfo.InvariantCulture))
        {
            var window = EditorWindow.GetWindow<OpeningWindow>(true,"Update Notifier",true);
        } 
    }
    class OpeningWindow : EditorWindow
    {
        private Texture2D logo;
        private GUIStyle messageStyle,whatsnewHeaderStyle,rateStyle,linkHeaderStyle,linkStyle;
        void Awake()
        {
            var windowRect = position;
            windowRect.width = 500;
            windowRect.height = 500;
            windowRect.position = new Vector2(Screen.currentResolution.width/2 - 250, Screen.currentResolution.height / 2 - 250);
            position = windowRect;
            logo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CHN Games/Helix Smash/Sprites/logo.png");
        }
        void OnGUI()
        {
            if (messageStyle == null)
            {
                messageStyle = new GUIStyle("Label");
                messageStyle.fontStyle = FontStyle.BoldAndItalic;
                messageStyle.fontSize = 30;
                whatsnewHeaderStyle = new GUIStyle("Label");
                whatsnewHeaderStyle.fontStyle = FontStyle.Italic;
                whatsnewHeaderStyle.fontSize = 15;
                rateStyle = new GUIStyle("Label");
                rateStyle.fontStyle = FontStyle.BoldAndItalic;
                rateStyle.fontSize = 18;
                linkHeaderStyle = new GUIStyle("Label");
                linkHeaderStyle.fontStyle = FontStyle.Bold;
                linkHeaderStyle.fontSize = 12; 
                linkStyle = new GUIStyle("Label");
                linkStyle.fontStyle = FontStyle.Italic;
                linkStyle.fontSize = 14;
            }
            GUI.DrawTexture(new Rect(50, 20, 400, 200), logo);
            GUI.Label(new Rect(25,230,500,60),message,messageStyle);
            GUI.Label(new Rect(25,290,300,60), "What's new in this v"+ version + " update?",whatsnewHeaderStyle);
            GUI.Label(new Rect(30,315,400,200),whatsnew);
            GUI.Label(new Rect(30,390, 400, 200), "Please rate this asset on asset store!",rateStyle);
            GUI.Label(new Rect(30, 420, 400, 200), "Rating in asset store is helping us so much. Thanks in advance.");
            GUI.Label(new Rect(30, 440, 400, 200), "Asset Link:",linkHeaderStyle);
            var oldColor = GUI.color;
            GUI.color = Color.cyan;
            if (GUI.Button(new Rect(105,438,400,25),assetLink, linkStyle))
            {
                Application.OpenURL(assetLink);
            }
            GUI.color = oldColor;
            GUILayout.Space(480);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Remind This Update Next Load"))
            {
                Close();
            }
            if (GUILayout.Button("Close"))
            {
                EditorPrefs.SetString("asset_version", version);
                Close();
            }
            GUILayout.EndHorizontal();
        } 
    }
}
