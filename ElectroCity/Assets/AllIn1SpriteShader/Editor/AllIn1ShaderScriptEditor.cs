using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AllIn1Shader)), CanEditMultipleObjects]
public class AllIn1ShaderScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Texture2D imageInspector = Resources.Load("CustomEditorImage") as Texture2D;
        if (imageInspector)
        {
            Rect rect;
            float imageHeight = imageInspector.height;
            float imageWidth = imageInspector.width;
            float aspectRatio = imageHeight / imageWidth;
            rect = GUILayoutUtility.GetRect(imageHeight, aspectRatio * Screen.width);
            EditorGUI.DrawTextureTransparent(rect, imageInspector);
        }

        AllIn1Shader myScript = (AllIn1Shader)target;

        if (GUILayout.Button("Deactivate All Effects"))
        {
            for (int i = 0; i < targets.Length; i++)
            {
                (targets[i] as AllIn1Shader).ClearAllKeywords();
            }
        }


        if (GUILayout.Button("New Clean Material"))
        {
            for (int i = 0; i < targets.Length; i++)
            {
                (targets[i] as AllIn1Shader).TryCreateNew();
            }
        }


        if (GUILayout.Button("Create New Material With Same Properties (SEE DOC)"))
        {
            for (int i = 0; i < targets.Length; i++)
            {
                (targets[i] as AllIn1Shader).MakeCopy();
            }
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Save Material To Folder (SEE DOC)"))
        {
            for (int i = 0; i < targets.Length; i++)
            {
                (targets[i] as AllIn1Shader).SaveMaterial();
            }
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("REMOVE COMPONENT AND MATERIAL"))
        {
            for (int i = 0; i < targets.Length; i++)
            {
                (targets[i] as AllIn1Shader).CleanMaterial();
            }
            for (int i = targets.Length - 1; i >= 0; i--)
            {
                DestroyImmediate(targets[i] as AllIn1Shader);
            }
        }
    }
}