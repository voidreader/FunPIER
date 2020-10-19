using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;

namespace SA.Android.Manifest
{
    public class AMM_PermissionsTab : SA_GUILayoutElement
    {
        List<string> m_permissionsStrings = null;
        List<AMM_ManifestPermission> m_permissionsArray = null;

        public override void OnGUI()
        {
            if (AMM_Manager.GetManifest().Permissions.Count > 0)
                using (new SA_WindowBlockWithSpace(new GUIContent("Permissions")))
                {
                    PermissionsList();
                    EditorGUILayout.Space();
                }

            using (new SA_WindowBlockWithSpace(new GUIContent("Actions")))
            {
                Actions();
                EditorGUILayout.Space();
            }
        }

        void Actions()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Android Permission"))
            {
                var permissionsMenu = new GenericMenu();
                foreach (var pStr in PermissionsStrings) permissionsMenu.AddItem(new GUIContent(pStr), false, SelectPermission, pStr);
                permissionsMenu.ShowAsContext();
            }

            if (GUILayout.Button("Add Custom Permission"))
            {
                var dlg = CreateInstance<AMM_AddPermissionDialog>();
                dlg.onClose += OnPermissionDlgClose;
                dlg.onAddClick += OnAddPermissionClick;

                dlg.titleContent.text = "Add Permission";

                dlg.ShowAuxWindow();
            }

            EditorGUILayout.EndHorizontal();
        }

        void PermissionsList()
        {
            foreach (var permission in AMM_Manager.GetManifest().Permissions)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUILayout.LabelField(permission.Values["android:name"]);
                if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20.0f)))
                {
                    AMM_Manager.GetManifest().RemovePermission(permission);
                    return;
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        List<string> PermissionsStrings
        {
            get
            {
                m_permissionsStrings = new List<string>();
                foreach (var p in PermissionsArray) m_permissionsStrings.Add(p.GetFullName());
                return m_permissionsStrings;
            }
        }

        List<AMM_ManifestPermission> PermissionsArray
        {
            get
            {
                var permissions = (AMM_ManifestPermission[])Enum.GetValues(typeof(AMM_ManifestPermission));
                m_permissionsArray = new List<AMM_ManifestPermission>(permissions);

                return m_permissionsArray;
            }
        }

        void OnAddPermissionClick(string permission)
        {
            var property = new AMM_PropertyTemplate("uses-permission");
            property.SetValue("android:name", permission);
            AMM_Manager.GetManifest().AddPermission(property);
        }

        void OnPermissionDlgClose(object dialog)
        {
            var dlg = dialog as AMM_AddPermissionDialog;
            dlg.onClose -= OnPermissionDlgClose;
            dlg.onAddClick -= OnAddPermissionClick;
        }

        void SelectPermission(object data)
        {
            var newPermission = new AMM_PropertyTemplate("uses-permission");
            newPermission.SetValue("android:name", data.ToString());
            AMM_Manager.GetManifest().AddPermission(newPermission);
        }
    }
}
