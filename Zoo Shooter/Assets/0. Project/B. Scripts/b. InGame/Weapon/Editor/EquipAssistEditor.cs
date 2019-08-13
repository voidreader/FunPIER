using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EquipAssist))]
public class EquipAssistEditor : Editor {

    EquipAssist targetObject = null;
    Player player;
    WeaponManager weaponManager;
    Weapon weapon;


    public override void OnInspectorGUI() {
        if(!target) {
            EditorGUILayout.HelpBox("EquipAssist is missing", MessageType.Error);
            return;
        }

        targetObject = (EquipAssist)target;

        player = targetObject.GetComponent<Player>();
        weaponManager = player.weapon;


        GUILayout.Label("Player Equip Management", EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandWidth(true));

        EditorGUILayout.BeginHorizontal();
        weapon = (Weapon)EditorGUILayout.ObjectField(weapon, typeof(Weapon), false, GUILayout.Width(150));
        if(GUILayout.Button("Equip", GUILayout.Width(100))) {
            if (weapon == null)
                return;

            // WeaponManager.s
            //weaponManager.CurentWeaponRenderer.sprite = weapon.WeaponSprite;
            weaponManager.SetWeaponByEquipManager(weapon);

        }

        EditorGUILayout.EndHorizontal();
    }
}
