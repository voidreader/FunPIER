using UnityEngine;
using System.Collections;
using UnityEditor;

public class RPGtMenuItem : Editor {



	// Use this for initialization
    [MenuItem("GameObject/RPG TextMesh/RPG TextMesh")]
	static void CreateEasyFontObject () {
		GameObject tempGameObject = new GameObject();
		tempGameObject.name = "RPGTextMesh";
		tempGameObject.AddComponent<RPGTextMesh>();
        tempGameObject.GetComponent<RPGTextMesh>().RefreshMeshEditor();
        tempGameObject.transform.parent = Selection.activeTransform;

        tempGameObject.transform.localPosition = Vector3.zero;
        tempGameObject.transform.localRotation = Quaternion.identity;
        tempGameObject.transform.localScale = Vector3.one;	

		Selection.activeGameObject = tempGameObject;
	}



}
