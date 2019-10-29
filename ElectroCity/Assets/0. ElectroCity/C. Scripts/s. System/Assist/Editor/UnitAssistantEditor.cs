using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(UnitAssistant))]
public class UnitAssistantEditor : Editor
{
    UnitAssistant targetUnit = null;
    Unit unit = null;
    Minion minion = null;
    Boss boss = null;
    int unitLevel = 0;

    public override void OnInspectorGUI() {
        if (!target) {
            EditorGUILayout.HelpBox("EquipAssist is missing", MessageType.Error);
            return;
        }

        targetUnit = (UnitAssistant)target;
        

        GUILayout.Label("INPUT UNIT LEVEL", EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandWidth(true));
        EditorGUILayout.BeginHorizontal();

        unitLevel = EditorGUILayout.IntField(unitLevel, GUILayout.Width(100));

        if (GUILayout.Button("FRIENDLY UNIT", GUILayout.Width(100))) {
            unit = targetUnit.GetComponent<Unit>();
            unit.SetUnit(unitLevel);
        }

        if (GUILayout.Button("MINION UNIT", GUILayout.Width(100))) {
            minion = targetUnit.GetComponent<Minion>();
            minion.InitMinion(unitLevel, 100);
        }

        if (GUILayout.Button("BOSS", GUILayout.Width(100))) {
            boss = targetUnit.GetComponent<Boss>();
            boss.InitBoss(unitLevel, 100);
            // minion.InitMinion(unitLevel, 100);
        }


        EditorGUILayout.EndHorizontal();

    }
}
