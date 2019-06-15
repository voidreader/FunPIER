using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class StageMaker : EditorWindow {

    public List<Object> StageList = new List<Object>();

    bool IsEditor = false;

    [MenuItem("Stage/Stage Maker")]
    static void Init()
    {
        StageMaker window = EditorWindow.GetWindow<StageMaker>(false, "Stage Maker");
        window.Show();
    }

    void Awake()
    {
        Load();
    }

    void Load()
    {
        StageList.Clear();

        TextAsset stage = AssetDatabase.LoadMainAssetAtPath(GameConfig._SaveFileAssetPath + GameConfig._StageFilePath + GameConfig._Extention) as TextAsset;
        // 데이터 복호화.
        string compressData = RPGAesCrypt.Decrypt(stage.text);
        // 데이터 압축해제.
        string stageData = Zipper.UnzipString(compressData);
        // json 변환.
        JSONObject json = new JSONObject(stageData);
        // dictionary 변환.
        ArrayList list = json.ToArray();

        foreach (string str in list)
        {
            Object obj = AssetDatabase.LoadMainAssetAtPath(GameConfig._SaveFileFullPath + str + GameConfig._Extention);
            if (obj)
                StageList.Add(obj);
        }
        Repaint();

        //TextAsset stage = AssetDatabase.LoadMainAssetAtPath(GameConfig._SaveFileAssetPath + GameConfig._StageFilePath) as TextAsset;
        /*
        Object obj = AssetDatabase.LoadMainAssetAtPath(GameConfig._SaveFileFullPath + "1" + GameConfig._Extention);
        StageList.Add(obj);
        */
    }

    void Save()
    {
        JSONObject json = new JSONObject();
        foreach (Object obj in StageList)
        {
            json.Add(obj.name);
        }
        string stageData = json.print();
        // 데이터 압축.
        string compressData = Zipper.ZipString(stageData);
        // 데이터 암호화.
        string cryptData = RPGAesCrypt.Encrypt(compressData);

        RPGDefine.writeStringToFile(cryptData, GameConfig._SaveFileAssetPath + GameConfig._StageFilePath + GameConfig._Extention);
        AssetDatabase.Refresh();

        IsEditor = false;
    }

    void OnDestroy()
    {
        //Debug.Log("OnDestroy");
        if (IsEditor)
        {
            bool result = EditorUtility.DisplayDialog(
                        "What do you want to do?",
                        "Please choose one of the following options.",
                        "Save and Quit",
                        "Quit without saving");
            if (result)
            {
                Save();
            }
        }
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Load", GUI.skin.button, GUILayout.Width(150), GUILayout.Height(30)))
        {
            Load();
        }
        if (GUILayout.Button("Save", GUI.skin.button, GUILayout.Width(150), GUILayout.Height(30)))
        {
            Save();
        }

        EditorGUILayout.EndHorizontal();

        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty objectsProperty = so.FindProperty("StageList");

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(objectsProperty, true); // True means show children
        if (EditorGUI.EndChangeCheck())
            IsEditor = true;
        
        so.ApplyModifiedProperties();


    }

}
