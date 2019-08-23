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
using System.Diagnostics;
using System.Text;
using System.Threading;
using UnityEditor.Build.Reporting;

namespace AudienceNetwork.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public static class AudienceNetworkBuild
    {
        public const string AudienceNetworkPath = "Assets/AudienceNetwork/";
        public const string AudienceNetworkPluginsPath = AudienceNetworkPath + "Plugins/";
        public const string AudienceNetworkPluginiOSPath = AudienceNetworkPluginsPath + "iOS/libs/FBAudienceNetwork.framework/FBAudienceNetwork";
        public const string AudienceNetworkPluginAndroidPath = AudienceNetworkPluginsPath + "Android/libs/AudienceNetwork.aar";
        public static string PluginsPath = "Assets/Plugins/";

        public enum Target
        {
            DEBUG,
            RELEASE
        }

        private static string VersionName
        {
            get
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "audience-network-unity-sdk-{0}",
                    SdkVersion.Build);
            }
        }

        private static string UnityPackagePath
        {
            get
            {
                string unityPackageName = string.Format("{0}.unitypackage", VersionName);
                return Path.Combine(OutputDirectoryPath, unityPackageName);
            }
        }

        private static string AndroidAPKPath
        {
            get
            {
                string androidApkName = string.Format("{0}.apk", VersionName);
                return Path.Combine(OutputDirectoryPath, androidApkName);
            }
        }

        private static string IOSProjectPath
        {
            get
            {
                string iOSProjectName = string.Format("{0}-ios", VersionName);
                return Path.Combine(OutputDirectoryPath, iOSProjectName);
            }
        }

        private static string[] Scenes
        {
            get
            {
                return new string[] { "Assets/AudienceNetwork/Scenes/Banner/AdViewScene.unity",
                "Assets/AudienceNetwork/Scenes/Interstitial/InterstitialAdScene.unity",
                "Assets/AudienceNetwork/Scenes/NativeAd/NativeAdScene.unity",
                "Assets/AudienceNetwork/Scenes/NativeBannerAd/NativeBannerAdScene.unity",
                "Assets/AudienceNetwork/Scenes/RewardedVideo/RewardedVideoAdScene.unity" };
            }
        }

        private static string OutputDirectoryPath
        {
            get
            {
                DirectoryInfo outputDirectory = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "out"));
                outputDirectory.Create();
                DirectoryInfo versionOutputDirectory = new DirectoryInfo(Path.Combine(outputDirectory.FullName, VersionName));
                versionOutputDirectory.Create();
                return versionOutputDirectory.FullName;
            }
        }

        // Exporting the *.unityPackage for Asset store
        public static bool ExportPackage()
        {
            // Check that SDKs are built
            bool iOSFound = File.Exists(AudienceNetworkPluginiOSPath);
            bool androidFound = File.Exists(AudienceNetworkPluginAndroidPath);
            if (!iOSFound || !androidFound)
            {
                Debug.Log("Exporting failed, no AN SDK build found. Found SDKS - iOS: " + iOSFound + " Android: " + androidFound);
                return false;
            }


            try
            {
                AssetDatabase.DeleteAsset(PluginsPath + "Android/AndroidManifest.xml");
                AssetDatabase.DeleteAsset(PluginsPath + "Android/AndroidManifest.xml.meta");
                AssetDatabase.DeleteAsset(AudienceNetworkPluginsPath + "Android/AndroidManifest.xml");
                AssetDatabase.DeleteAsset(AudienceNetworkPluginsPath + "Android/AndroidManifest.xml.meta");

                string[] facebookFiles = Directory.GetFiles(AudienceNetworkPath, "*.*", SearchOption.AllDirectories);
                string[] pluginsFiles = Directory.GetFiles(AudienceNetworkPluginsPath, "*.*", SearchOption.AllDirectories);
                string[] files = new string[facebookFiles.Length + pluginsFiles.Length];

                facebookFiles.CopyTo(files, 0);
                pluginsFiles.CopyTo(files, facebookFiles.Length);

                AssetDatabase.ExportPackage(
                    files,
                    UnityPackagePath,
                    ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse);
            }
            finally
            {
                // regenerate the manifest
                ManifestMod.GenerateManifest();
            }
            return true;
        }

        private static BuildReport BuildAndroidAPK()
        {
            return BuildPipeline.BuildPlayer(Scenes, AndroidAPKPath, BuildTarget.Android, BuildOptions.None);
        }

        private static BuildReport BuildiOSProject()
        {
            return BuildPipeline.BuildPlayer(Scenes, IOSProjectPath, BuildTarget.iOS, BuildOptions.None);
        }

        private static bool ReportOnBuild(BuildReport report)
        {
            BuildSummary summary = report.summary;
            if (summary.result == BuildResult.Succeeded)
            {
                string message = string.Format("Success! File created at {0}", summary.outputPath);
                LogActionSuccess(message);
                return true;
            }
            LogActionFailure("Something went wrong!");
            return false;
        }

        public static void BuildRelease()
        {
            LogActionStart("Exporting unitypackage");
            if (ExportPackage())
            {
                string message = string.Format("unitypackage exported to {0}", UnityPackagePath);
                LogActionSuccess(message);
            }
            else
            {
                LogActionFailure("Failed exporting unitypackage");
                return;
            }

            LogActionStart("Building Android APK");
            BuildReport androidBuildReport = BuildAndroidAPK();
            if (!ReportOnBuild(androidBuildReport))
            {
                LogActionFailure("Android APK failed to build. Exiting");
                return;
            }

            LogActionStart("Building iOS Project");
            BuildReport iOSBuildReport = BuildiOSProject();
            if (!ReportOnBuild(iOSBuildReport))
            {
                LogActionFailure("iOS Project failed to build. Exiting.");
                return;
            }
        }

        private static void LogActionStart(string test)
        {
            PrintToConsole(test, ConsoleColor.Yellow, false);
        }

        private static void LogActionSuccess(string test)
        {
            PrintToConsole(test, ConsoleColor.Green);
        }

        private static void LogActionFailure(string test)
        {
            PrintToConsole(test, ConsoleColor.Red);
        }

        private static void PrintToConsole(string text, ConsoleColor color, bool revertToWhite = true)
        {
            Console.ForegroundColor = color;
            Debug.Log(text);
            if (revertToWhite)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public delegate void SDKBuildCallback(bool success, string version, string message, string buildOutput, string buildError);

        public static IEnumerable<SDKBuildStatus> RunSDKBuild(string version, bool skipBuild, SDKBuildCallback callback)
        {
            DirectoryInfo projectRoot = Directory.GetParent(Directory.GetCurrentDirectory());
            string workingDirectory = Path.Combine(projectRoot.FullName, "ads/scripts/");
            string script = Path.Combine(workingDirectory, "build_distribution.sh");
            string scriptPlusArgs = script + " -v " + version;
            if (skipBuild)
            {
                scriptPlusArgs = scriptPlusArgs + " -s";
            }

            ProcessStartInfo processStartInfo = new ProcessStartInfo("sh", scriptPlusArgs)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = workingDirectory
            };

            StringBuilder outputBuilder = new StringBuilder();
            StringBuilder errorBuilder = new StringBuilder();
            List<string> outputList = new List<string>();

            Process process = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };

            outputBuilder.Append("Build Starting...\n");
            yield return new SDKBuildStatus(true, outputList, process);

            using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
            using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
            {
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        outputWaitHandle.Set();
                    }
                    else
                    {
                        outputBuilder.AppendLine(e.Data);
                        outputList.Add(e.Data);
                    }
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        errorWaitHandle.Set();
                    }
                    else
                    {
                        errorBuilder.AppendLine(e.Data);
                    }
                };

                bool exited = false;

                process.Exited += (sender, e) =>
                {
                    string output = outputBuilder.ToString();
                    string error = errorBuilder.ToString();
                    bool success = process.ExitCode == 0;
                    string message = success ? "Completed successfully." : "Build script returned an error, check console log.";
                    exited = true;
                    callback(success, version, message, output, error);
                };

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                while (true)
                {
                    if (!exited)
                    {
                        yield return new SDKBuildStatus(true, outputList, process);
                    }
                    else
                    {
                        yield break;
                    }
                }
            }
        }
    }

    public class SDKBuildStatus
    {
        public SDKBuildStatus(bool BuildInProgress, List<string> CurrentLogOutput, Process process)
        {
            this.BuildInProgress = BuildInProgress;
            this.CurrentLogOutput = CurrentLogOutput;
            this.process = process;
        }

        public bool BuildInProgress;
        public List<string> CurrentLogOutput;
        public Process process;
    }
}
