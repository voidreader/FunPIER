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
    using System.IO;
    using UnityEditor;
    using UnityEditor.Callbacks;
#if UNITY_IOS
    using UnityEditor.iOS.Xcode;
#endif
    using UnityEngine;

    public static class XCodePostProcess
    {
        public static string AudienceNetworkFramework = "FBAudienceNetwork.framework";
        public static string AudienceNetworkAAR = "AudienceNetwork.aar";
        public static string FrameworkDependenciesKey = "FrameworkDependencies";
        public static string RequiredFrameworks = "AdSupport;StoreKit;WebKit";

        [PostProcessBuild(100)]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {

            #if UNITY_IOS
            if (target == BuildTarget.iOS) {
                string projectPath = PBXProject.GetPBXProjectPath(path);
                PBXProject project = new PBXProject();
                project.ReadFromString(File.ReadAllText(projectPath));
                string targetName = PBXProject.GetUnityTargetName();
                string targetGUID = project.TargetGuidByName(targetName);
                project.AddFrameworkToProject(targetGUID, "AdSupport.framework", false);
                project.AddFrameworkToProject(targetGUID, "StoreKit.framework", false);
                project.AddFrameworkToProject(targetGUID, "WebKit.framework", false);

                File.WriteAllText(projectPath, project.WriteToString());
            }
#endif  

            PluginImporter[] importers = PluginImporter.GetAllImporters();
            PluginImporter iOSPlugin = null;
            PluginImporter androidPlugin = null;
            foreach (PluginImporter importer in importers)
            {
                if (importer.assetPath.Contains(AudienceNetworkFramework))
                {
                    iOSPlugin = importer;
                    Debug.Log("Audience Network iOS plugin found at " + importer.assetPath + ".");
                }
                else if (importer.assetPath.Contains(AudienceNetworkAAR))
                {
                    androidPlugin = importer;
                    Debug.Log("Audience Network Android plugin found at " + importer.assetPath + ".");
                }
            }
            if (iOSPlugin != null)
            {
                iOSPlugin.SetCompatibleWithAnyPlatform(false);
                iOSPlugin.SetCompatibleWithEditor(false);
                iOSPlugin.SetCompatibleWithPlatform(BuildTarget.iOS, true);
                iOSPlugin.SetPlatformData(BuildTarget.iOS, FrameworkDependenciesKey, RequiredFrameworks);
                iOSPlugin.SaveAndReimport();
            }
            if (androidPlugin != null)
            {
                androidPlugin.SetCompatibleWithAnyPlatform(false);
                androidPlugin.SetCompatibleWithEditor(false);
                androidPlugin.SetCompatibleWithPlatform(BuildTarget.Android, true);
                androidPlugin.SaveAndReimport();
            }
        }
    }
}
