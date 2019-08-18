using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(EnemySettingAssist))]
public class EnemySettingAssistEditor : Editor {
    EnemySettingAssist targetObject = null;
    string enemyID = string.Empty;
    Enemy enemy;

    public override void OnInspectorGUI() {
        if (!target) {
            EditorGUILayout.HelpBox("EquipAssist is missing", MessageType.Error);
            return;
        }

        targetObject = (EnemySettingAssist)target;
        enemy = targetObject.GetComponent<Enemy>();

        GUILayout.Label("Enter Enemy ID", EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandWidth(true));
        EditorGUILayout.BeginHorizontal();

        enemyID = EditorGUILayout.TextField(enemyID, GUILayout.Width(100));
        if (GUILayout.Button("NORMAL SET", GUILayout.Width(100))) {
            enemy.SetEnemy(EnemyType.Normal, enemyID);
        }

        if (GUILayout.Button("BOSS SET", GUILayout.Width(100))) {
            enemy.SetEnemy(EnemyType.Boss, enemyID);
        }

        EditorGUILayout.EndHorizontal();
    }

}
