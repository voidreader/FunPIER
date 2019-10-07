using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(UnitAssistant))]
public class UnitAssistantEditor : Editor
{
    UnitAssistant targetUnit = null;
    Unit unit = null;
    int unitLevel = 0;

    public override void OnInspectorGUI() {
        if (!target) {
            EditorGUILayout.HelpBox("EquipAssist is missing", MessageType.Error);
            return;
        }

        targetUnit = (UnitAssistant)target;
        unit = targetUnit.GetComponent<Unit>();

        GUILayout.Label("INPUT UNIT LEVEL", EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandWidth(true));
        EditorGUILayout.BeginHorizontal();

        unitLevel = EditorGUILayout.IntField(unitLevel, GUILayout.Width(100));

        if (GUILayout.Button("FRIENDLY UNIT", GUILayout.Width(100))) {
            // enemy.SetEnemy(EnemyType.Normal, enemyID);
            unit.SetUnit(unitLevel);
        }

        if (GUILayout.Button("MINION UNIT", GUILayout.Width(100))) {
            // enemy.SetEnemy(EnemyType.Normal, enemyID);
        }

        EditorGUILayout.EndHorizontal();

    }
}
