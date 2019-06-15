using UnityEngine;
using System.Collections;
using UnityEditor;

public class RPGTextMeshChangeFont : Editor
{



	// Use this for initialization
    [MenuItem("RPG TextMesh/Change Font")]
	static void CreateEasyFontObject () {
        RPGTextMesh[] tm_list = Selection.activeTransform.GetComponentsInChildren<RPGTextMesh>(true);

        for (int i = 0; i < tm_list.Length; i++ )
        {
            RPGTextMesh tm = tm_list[i];
            tm.FontType = UnityEditor.AssetDatabase.LoadAssetAtPath<Font>("Assets/Font/arialbd.ttf");
        }
            
	}



}
