using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
[CustomEditor(typeof(GameManager))]
public class testx : Editor
{
    SerializedProperty starGenerationSizeProp,starGenerationPointProp;
    void OnEnable()
    {
        starGenerationSizeProp = serializedObject.FindProperty("StarGenerationSize");
        starGenerationPointProp = serializedObject.FindProperty("StarGenerationPoint");
    }
    void OnSceneGUI()
    {
        Handles.Label(starGenerationPointProp.vector3Value+new Vector3(0, starGenerationSizeProp.vector3Value.y/2-0.2f, 0),"Star Random Generation Box");
        Handles.color = Color.red;
        Handles.DrawWireCube(starGenerationPointProp.vector3Value, starGenerationSizeProp.vector3Value);
    }
}