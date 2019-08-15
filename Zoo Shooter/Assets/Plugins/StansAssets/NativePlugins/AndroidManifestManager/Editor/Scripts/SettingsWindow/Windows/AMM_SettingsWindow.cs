using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;

namespace SA.Android.Manifest
{

    public class AMM_SettingsWindow : SA_PluginSettingsWindow<AMM_SettingsWindow>
    {

        private static AMM_BaseTemplate m_parentTemplate = null;
   
        protected override void OnAwake() {
            SetHeaderTitle(AMM_Settings.PLUGIN_NAME);
            SetHeaderDescription("This editor extension allows you to manage android manifest files" +
                                 "located at Assets/Plugins/Android folder of your project. Plugin provides" +
                                 "visual representation of AndroidManifest.xml and allows you do manage the content" +
                                 "with the help of Editor UI tools.");
            SetHeaderVersion(AMM_Settings.FormattedVersion);
            SetDocumentationUrl("https://unionassets.com/android-manifest-manager");

            
            AddMenuItem("MANIFEST", CreateInstance<AMM_ManifestTab>()  );
            AddMenuItem("APPLICATION", CreateInstance<AMM_ApplicationTab>());
            AddMenuItem("PRMISSIONS", CreateInstance<AMM_PermissionsTab>());
            AddMenuItem("ABOUT", CreateInstance<SA_PluginAboutLayout>());

        }


        protected override void OnTabsGUI(int tabIndex) {

            if(tabIndex == 3) {
                base.OnTabsGUI(tabIndex);
                return;
            }

            AMM_Template manifest = AMM_Manager.GetManifest();
            if (manifest == null) {
                EditorGUILayout.HelpBox("The selected build platform DOESN'T support AndroidManifest.xml file", MessageType.Info);
                return;
            }

            base.OnTabsGUI(tabIndex);
            EditorGUILayout.Space();
            if (GUILayout.Button("Save Manifest", GUILayout.Height(22.0f))) {
                AMM_Manager.SaveManifest();
            }

        }


        public static void AddValueDialog(AMM_BaseTemplate parent) {
            m_parentTemplate = parent;

            AMM_AddValueDialog dialog = EditorWindow.CreateInstance<AMM_AddValueDialog>();
            dialog.onAddClick += OnAddValueClick;
            dialog.onClose += OnValueDlgClose;

            dialog.titleContent.text = "Add Value";
            dialog.ShowAuxWindow();
        }


        private static void OnAddValueClick(string key, string value) {
            if (m_parentTemplate is AMM_ActivityTemplate) {
                if (key.Equals("android:name")) {
                    return;
                }
            }
            m_parentTemplate.SetValue(key, value);
        }

        private static void OnValueDlgClose(object dialog) {
            m_parentTemplate = null;

            AMM_AddValueDialog dlg = dialog as AMM_AddValueDialog;
            dlg.onAddClick -= OnAddValueClick;
            dlg.onClose -= OnValueDlgClose;
        }


        public static void AddPropertyDialog(AMM_BaseTemplate parent) {
            m_parentTemplate = parent;

            AMM_AddPropertyDialog dialog = EditorWindow.CreateInstance<AMM_AddPropertyDialog>();
            dialog.onAddClick += OnAddPropertyClick;
            dialog.onClose += OnPropertyDlgClose;

            dialog.titleContent.text = "Add Property";

            dialog.ShowAuxWindow();
        }


        private static void OnAddPropertyClick(string tag) {
            AMM_PropertyTemplate property = new AMM_PropertyTemplate(tag);
            m_parentTemplate.AddProperty(tag, property);
        }

        private static void OnPropertyDlgClose(object dialog) {
            m_parentTemplate = null;

            AMM_AddPropertyDialog dlg = dialog as AMM_AddPropertyDialog;
            dlg.onAddClick -= OnAddPropertyClick;
            dlg.onClose -= OnValueDlgClose;

        }


        public static void DrawProperties(AMM_BaseTemplate parent) {


            using(new SA_GuiHorizontalSpaceIgnoreIndent(15)) {
                DrawPropertiesInternal(parent);
            }   
        }

        public static void DrawPropertiesInternal(AMM_BaseTemplate parent) {
            foreach (string key in parent.Properties.Keys) {
                foreach (AMM_PropertyTemplate property in parent.Properties[key]) {

                    if (parent is AMM_ActivityTemplate) {
                        AMM_ActivityTemplate activity = parent as AMM_ActivityTemplate;
                        if (activity.IsLauncherProperty(property)) {
                            continue;
                        }
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(27.0f));
                    EditorGUILayout.BeginHorizontal();

                    if (property.Values.ContainsKey("android:name")) {
                        property.IsOpen = EditorGUILayout.Foldout(property.IsOpen, "[" + property.Tag + "] " + property.Values["android:name"]);
                    } else {
                        if (key.Equals("intent-filter")) {
                            property.IsOpen = EditorGUILayout.Foldout(property.IsOpen, "[" + property.Tag + "] " + property.GetIntentFilterName(property));
                        } else {
                            property.IsOpen = EditorGUILayout.Foldout(property.IsOpen, "[" + property.Tag + "]");
                        }
                    }

                    if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20.0f))) {
                        parent.RemoveProperty(property);
                        return;
                    }
                    EditorGUILayout.EndHorizontal();

                    if (property.IsOpen) {
                        EditorGUILayout.BeginVertical();

                        foreach (string k in property.Values.Keys) {
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.LabelField(k);


                            string input = property.Values[k];
                            EditorGUI.BeginChangeCheck();
                            if (k.Equals("android:name")) {
                                input = GUILayout.TextField(property.Values[k]);
                            } else {
                                input = GUILayout.TextField(property.Values[k]);
                            }
                            if (EditorGUI.EndChangeCheck()) {
                                property.SetValue(k, input);
                                return;
                            }

                            if (GUILayout.Button("X", EditorStyles.miniButton,GUILayout. Width(20.0f))) {
                                property.RemoveValue(k);
                                return;
                            }

                            EditorGUILayout.EndHorizontal();
                        }

                        DrawProperties(property);
                        EditorGUILayout.Space();
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space();
                        if (GUILayout.Button("Add Value", GUILayout.Width(100.0f))) {
                            AddValueDialog(property);
                        }
                        if (GUILayout.Button("Add Property", GUILayout.Width(100.0f))) {
                            AddPropertyDialog(property);
                        }
                        EditorGUILayout.Space();
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space();
                    }
                    EditorGUILayout.EndVertical();
                }
            }
        }
    }
}