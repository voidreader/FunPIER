
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;

namespace SA.Android.Manifest
{

    public class AMM_PermissionsTab : SA_GUILayoutElement
    {

        private List<string> m_permissionsStrings = null;
        private List<AMM_ManifestPermission> m_permissionsArray = null;


        public override void OnGUI() {

            if(AMM_Manager.GetManifest().Permissions.Count > 0) {
                using (new SA_WindowBlockWithSpace(new GUIContent("Permissions"))) {
                    
                    PermissionsList();
                    EditorGUILayout.Space();
                }
            }

               
          
            using (new SA_WindowBlockWithSpace(new GUIContent("Actions"))) {
                Actions();
                EditorGUILayout.Space();
            }

              

               

        }


        private void Actions() {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Android Permission")) {
                GenericMenu permissionsMenu = new GenericMenu();
                foreach (string pStr in PermissionsStrings) {
                    permissionsMenu.AddItem(new GUIContent(pStr), false, SelectPermission, pStr);
                }
                permissionsMenu.ShowAsContext();
            }

            if (GUILayout.Button("Add Custom Permission")) {
                AMM_AddPermissionDialog dlg = EditorWindow.CreateInstance<AMM_AddPermissionDialog>();
                dlg.onClose += OnPermissionDlgClose;
                dlg.onAddClick += OnAddPermissionClick;

                dlg.titleContent.text = "Add Permission";



                dlg.ShowAuxWindow();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void PermissionsList() {
            foreach (AMM_PropertyTemplate permission in AMM_Manager.GetManifest().Permissions) {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUILayout.LabelField(permission.Values["android:name"]);
                if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20.0f))) {
                    AMM_Manager.GetManifest().RemovePermission(permission);
                    return;
                }
                EditorGUILayout.EndHorizontal();
            }
        }


        private List<string> PermissionsStrings {
            get {
                m_permissionsStrings = new List<string>();
                foreach (AMM_ManifestPermission p in PermissionsArray) {
                    m_permissionsStrings.Add(p.GetFullName());
                }
                return m_permissionsStrings;
            }
        }

        private List<AMM_ManifestPermission> PermissionsArray {
            get {
                AMM_ManifestPermission[] permissions = (AMM_ManifestPermission[])Enum.GetValues(typeof(AMM_ManifestPermission));
                m_permissionsArray = new List<AMM_ManifestPermission>(permissions);

                return m_permissionsArray;
            }
        }


        private void OnAddPermissionClick(string permission) {
            AMM_PropertyTemplate property = new AMM_PropertyTemplate("uses-permission");
            property.SetValue("android:name", permission);
            AMM_Manager.GetManifest().AddPermission(property);
        }


        private void OnPermissionDlgClose(object dialog) {
            AMM_AddPermissionDialog dlg = dialog as AMM_AddPermissionDialog;
            dlg.onClose -= OnPermissionDlgClose;
            dlg.onAddClick -= OnAddPermissionClick;
        }

        private void SelectPermission(object data) {
            AMM_PropertyTemplate newPermission = new AMM_PropertyTemplate("uses-permission");
            newPermission.SetValue("android:name", data.ToString());
            AMM_Manager.GetManifest().AddPermission(newPermission);
        }
    }
}