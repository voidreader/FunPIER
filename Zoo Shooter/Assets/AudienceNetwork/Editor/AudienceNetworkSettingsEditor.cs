/**
 * Copyright (c) 2014-present, Facebook, Inc. All rights reserved.
 *
 * You are hereby granted a non-exclusive, worldwide, royalty-free license to use,
 * copy, modify, and distribute this software in source code or binary form for use
 * in connection with the web services and APIs provided by Facebook.
 *
 * As with any software that integrates with the Facebook platform, your use of
 * this software is subject to the Facebook Developer Principles and Policies
 * [http://developers.facebook.com/policy/]. This copyright notice shall be
 * included in all copies or substantial portions of the software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
namespace AudienceNetwork.Editor
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using UnityEditor;
    using UnityEngine;

    public class AudienceNetworkSettingsEditor : Editor
    {
        private static readonly string title = "Audience Network SDK";

        [MenuItem("Tools/Audience Network/About")]
        private static void AboutGUI()
        {
            string aboutString = string.Format("Facebook Audience Network Unity SDK Version {0}",
                                 SdkVersion.Build);
            EditorUtility.DisplayDialog(title,
                                        aboutString,
                                        "Okay");
        }

        [MenuItem("Tools/Audience Network/Regenerate Android Manifest")]
        private static void RegenerateManifest()
        {
            bool updateManifest = EditorUtility.DisplayDialog(title,
                                  "Are you sure you want to regenerate your Android Manifest.xml?",
                                  "Okay",
                                  "Cancel");

            if (updateManifest) {
                ManifestMod.GenerateManifest();
                EditorUtility.DisplayDialog(title, "Android Manifest updated. \n \n If interstitial ads still throw ActivityNotFoundException, " +
                                            "you may need to copy the generated manifest at " + ManifestMod.AndroidManifestPath + " to /Assets/Plugins/Android.", "Okay");
            }
        }
    }

    public class SDKVersionWindow : EditorWindow
    {

        public string version = "";
        public bool skipBuild;
        private bool building;
        private IEnumerator<SDKBuildStatus> buildStatusEnumerator;

        private const float maxHeight = float.MaxValue;

        void OnEnable()
        {
            titleContent.text = "Audience Network Unity SDK Build Generator";
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("SDK Base Version: (4.19.0)", version, GUILayout.MinWidth(600));
            version = GUILayout.TextField(version);
            skipBuild = GUILayout.Toggle(skipBuild, "Skip build?");

            GUILayout.FlexibleSpace();

            building |= GUILayout.Button("Generate Build") && version.Length > 0;

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Cancel")) {
                EndBuild();
            }
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        void Update()
        {
            if (building) {
                if (buildStatusEnumerator == null) {
                    AudienceNetworkBuild.SDKBuildCallback callback = delegate (bool success, string version, string message, string buildOutput, string buildError)
                    {
                        UnityEngine.Debug.Log("Build Complete for " + version + ".\nSuccess? " + success.ToString());
                        building = false;
                    };
                    IEnumerable<SDKBuildStatus> buildStatusEnumerable = AudienceNetworkBuild.RunSDKBuild(SdkVersion.Build, false,
                    callback);

                    buildStatusEnumerator = buildStatusEnumerable.GetEnumerator();
                }

                if (buildStatusEnumerator.MoveNext()) {
                    SDKBuildStatus buildStatus = buildStatusEnumerator.Current;
                    IList<string> logs = buildStatus.CurrentLogOutput;
                    if (logs.Count > 0) {
                        UnityEngine.Debug.Log(logs.Pop());
                    }

                    building &= buildStatus == null || buildStatus.BuildInProgress;
                }
            }
        }

        void OnDisable()
        {
            EndBuild();
        }

        private void EndBuild()
        {
            building = false;
            if (buildStatusEnumerator != null && buildStatusEnumerator.MoveNext()) {
                SDKBuildStatus buildStatus = buildStatusEnumerator.Current;
                if (buildStatus != null && buildStatus.process != null) {
                    KillProcess(buildStatus.process);
                }
                buildStatusEnumerator = null;
                UnityEngine.Debug.Log("Build cancelled.");
            }
        }

        private static void KillProcess(Process processToBeKilled)
        {
            if (processToBeKilled != null) {
                Process[] processes = Process.GetProcessesByName("xcodebuild");
                foreach (Process process in processes) {
                    process.Kill();
                }
                processToBeKilled.Kill();
            }
        }
    }
}
