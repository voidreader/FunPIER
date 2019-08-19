using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;

namespace SA.Android.Manifest
{

    public class AMM_ApplicationTab : SA_GUILayoutElement
    {
        public override void OnGUI() {


            using (new SA_WindowBlockWithIndent(new GUIContent("Values"))) {
                Values();
                EditorGUILayout.Space();
            }
           
            using (new SA_WindowBlockWithIndent(new GUIContent("Activities"))) {
                using(new SA_GuiHorizontalSpaceIgnoreIndent(15)) {
                    Activities();
                } 
                EditorGUILayout.Space();
            }


            using (new SA_WindowBlockWithIndent(new GUIContent("Properties"))) {
                Properties();
                EditorGUILayout.Space();
            }
                  
        }

       

        private void Values() {
            AMM_Template manifest = AMM_Manager.GetManifest();
            foreach (string key in manifest.ApplicationTemplate.Values.Keys) {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(key);

                string input = AMM_Manager.GetManifest().ApplicationTemplate.Values[key];
                EditorGUI.BeginChangeCheck();
                input = EditorGUILayout.TextField(AMM_Manager.GetManifest().ApplicationTemplate.Values[key]);
                if (EditorGUI.EndChangeCheck()) {
                    AMM_Manager.GetManifest().ApplicationTemplate.SetValue(key, input);
                    return;
                }

                if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20.0f))) {
                    AMM_Manager.GetManifest().ApplicationTemplate.RemoveValue(key);
                    return;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Add Value", GUILayout.Width(100.0f))) {
                AMM_SettingsWindow.AddValueDialog(AMM_Manager.GetManifest().ApplicationTemplate);
            }
            EditorGUILayout.EndHorizontal();
        }


        private void Activities() {
            int launcherActivities = 0;
            foreach (int id in AMM_Manager.GetManifest().ApplicationTemplate.Activities.Keys) {
                AMM_ActivityTemplate activity = AMM_Manager.GetManifest().ApplicationTemplate.Activities[id];

                if (activity.IsLauncher) {
                    launcherActivities++;
                }

                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.BeginHorizontal();
                activity.IsOpen = EditorGUILayout.Foldout(activity.IsOpen, activity.Name);
                if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20.0f))) {
                    AMM_Manager.GetManifest().ApplicationTemplate.RemoveActivity(activity);
                    return;
                }
                EditorGUILayout.EndHorizontal();

                if (activity.IsOpen) {
                    EditorGUILayout.BeginVertical();

                    bool isLauncher = activity.IsLauncher;
                    EditorGUI.BeginChangeCheck();
                    isLauncher = EditorGUILayout.Toggle("Is Launcher", activity.IsLauncher);
                    if (EditorGUI.EndChangeCheck()) {
                        activity.SetAsLauncher(isLauncher);
                    }

                    foreach (string k in activity.Values.Keys) {
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField(k);
                        EditorGUILayout.Space();

                        string input = activity.Values[k];
                        EditorGUI.BeginChangeCheck();

                        if (k.Equals("android:name")) {
                            input = EditorGUILayout.TextField(activity.Values[k]);
                        } else {
                            input = EditorGUILayout.TextField(activity.Values[k]);
                        }

                        if (EditorGUI.EndChangeCheck()) {
                            activity.SetValue(k, input);
                            return;
                        }

                        if (!k.Equals("android:name")) {
                            if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20.0f))) {
                                activity.RemoveValue(k);
                                return;
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space();
                    }

                    AMM_SettingsWindow.DrawProperties(activity);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Add Value", GUILayout.Width(100.0f))) {
                        AMM_SettingsWindow.AddValueDialog(activity);
                    }
                    if (GUILayout.Button("Add Property", GUILayout.Width(100.0f))) {
                        AMM_SettingsWindow.AddPropertyDialog(activity);
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Add Activity", GUILayout.Width(100.0f))) {
                AMM_AddPermissionDialog dlg = EditorWindow.CreateInstance<AMM_AddPermissionDialog>();
                dlg.onAddClick += OnAddActivityClick;

                dlg.titleContent.text = "Add Activity";
                dlg.ShowAuxWindow();
            }
            EditorGUILayout.EndHorizontal();

            if (launcherActivities > 1) {
                EditorGUILayout.HelpBox("There is MORE THAN ONE Launcher Activity in Manifest", MessageType.Warning);
            } else if (launcherActivities < 1) {
                EditorGUILayout.HelpBox("There is NO Launcher Activities in Manifest", MessageType.Warning);
            }
        }


        private void Properties() {
          
            AMM_SettingsWindow.DrawProperties(AMM_Manager.GetManifest().ApplicationTemplate);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Add Property", GUILayout.Width(100.0f))) {
                AMM_SettingsWindow.AddPropertyDialog(AMM_Manager.GetManifest().ApplicationTemplate);
            }
           
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        private void OnAddActivityClick(string activityName) {
            AMM_ActivityTemplate newActivity = new AMM_ActivityTemplate(false, activityName);
            newActivity.SetValue("android:name", activityName);
            newActivity.IsOpen = true;
            AMM_Manager.GetManifest().ApplicationTemplate.AddActivity(newActivity);
        }

    }
}