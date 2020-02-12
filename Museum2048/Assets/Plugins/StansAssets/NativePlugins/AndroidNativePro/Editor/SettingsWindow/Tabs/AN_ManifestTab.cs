using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;
using SA.Foundation.Utility;
using SA.Foundation.UtilitiesEditor;

using SA.Android.Manifest;

namespace SA.Android
{
    [Serializable]
    public class AN_ManifestTab : SA_GUILayoutElement
    {
        [SerializeField] SA_HyperToolbar m_menuToolbar;
        [SerializeField] List<SA_GUILayoutElement> m_tabsLayout = new List<SA_GUILayoutElement>();

        public override void OnAwake() {
            m_tabsLayout = new List<SA_GUILayoutElement>();
            m_menuToolbar = new SA_HyperToolbar();

          

            AddMenuItem("MANIFEST", CreateInstance<AMM_ManifestTab>());
            AddMenuItem("APPLICATION", CreateInstance<AMM_ApplicationTab>());
            AddMenuItem("PRMISSIONS", CreateInstance<AMM_PermissionsTab>());
        }


        public override void OnLayoutEnable() {
            foreach (var tab in m_tabsLayout) {
                tab.OnLayoutEnable();
            }
        }

        private void AddMenuItem(string itemName, SA_GUILayoutElement layout) {
            var button = new SA_HyperLabel(new GUIContent(itemName), EditorStyles.boldLabel);
            button.SetMouseOverColor(SA_PluginSettingsWindowStyles.SelectedElementColor);
            m_menuToolbar.AddButtons(button);

            m_tabsLayout.Add(layout);
        }

        public override void OnGUI() {
            GUILayout.Space(2);
            int index = m_menuToolbar.Draw();
            GUILayout.Space(4);
            EditorGUILayout.BeginVertical(SA_PluginSettingsWindowStyles.SeparationStyle);
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();

            AMM_Template manifest = AMM_Manager.GetManifest();
            if (manifest == null) {
                EditorGUILayout.HelpBox("The selected build platform DOESN'T support AndroidManifest.xml file", MessageType.Info);
                return;
            }

            m_tabsLayout[index].OnGUI();

            EditorGUILayout.Space();
            if (GUILayout.Button("Save Manifest", GUILayout.Height(22.0f))) {
                AMM_Manager.SaveManifest();
            }
        }

    }


}