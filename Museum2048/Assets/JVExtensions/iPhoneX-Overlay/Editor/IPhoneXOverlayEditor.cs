using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(IPhoneXOverlay))]
public class IPhoneXOverlayEditor : Editor {

	public Texture landscapeActive, landscapeInactive;
	public Texture portraitActive, 	portraitInactive;

	public override void OnInspectorGUI() {
//		DrawDefaultInspector();

		IPhoneXOverlay overlayScript = (IPhoneXOverlay)target;

		EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

		if (overlayScript.landscapeOverlay) {
			EditorGUILayout.HelpBox("Remember to change your game view resolution to 2436x1125 for landscape.", MessageType.Warning);
		}else if (overlayScript.portraitOvelay) {
			EditorGUILayout.HelpBox("Remember to change your game view resolution to 1125x2436 for portrait.", MessageType.Warning);
		}

		GUILayout.BeginHorizontal("box");

		if (GUILayout.Button(overlayScript.landscapeOverlay ? landscapeActive : landscapeInactive)) {
			overlayScript.landscapeOverlay = true;
			overlayScript.portraitOvelay = false;
		}


		if (GUILayout.Button(overlayScript.portraitOvelay ? portraitActive : portraitInactive)) {
			overlayScript.portraitOvelay = true;
			overlayScript.landscapeOverlay = false;
		}

		GUILayout.EndHorizontal();

		EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

		overlayScript.showFrame 		= EditorGUILayout.Toggle("Show Frame", overlayScript.showFrame);
		overlayScript.showSafeArea 		= EditorGUILayout.Toggle("Show Safe Area", overlayScript.showSafeArea);
		overlayScript.showDangerArea 	= EditorGUILayout.Toggle("Show Danger Area", overlayScript.showDangerArea);

		EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

		overlayScript.frameColor 		= EditorGUILayout.ColorField("Frame area", overlayScript.frameColor);
		overlayScript.safeColor 		= EditorGUILayout.ColorField("Safe area", overlayScript.safeColor);
		overlayScript.dangerColor 		= EditorGUILayout.ColorField("Danger area", overlayScript.dangerColor);

		EditorUtility.SetDirty (target);

	}

}