using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using SA.Foundation.Config;
using SA.Foundation.UtilitiesEditor;

namespace SA.Foundation.Editor
{
    public static class SA_PluginsEditor
    {
        public const string DISABLED_LIB_EXTENSION = ".txt";

        public static void UninstallLibFolder(string path)
        {
            if (SA_AssetDatabase.IsDirectoryExists(path))
            {
                EditorUtility.DisplayProgressBar("Stan's Assets.", "Uninstalling: " + path, 1);
                SA_AssetDatabase.DeleteAsset(path);
                EditorUtility.ClearProgressBar();
            }
        }

        public static void InstallLibs(string source, string destination, List<string> libs)
        {
            for (var i = 0; i < libs.Count; i++)
            {
                var lib = libs[i];
                var disabledLib = lib + DISABLED_LIB_EXTENSION;
                var sourcePath = source + disabledLib;
                var destinationPath = destination + lib;

                if (!SA_AssetDatabase.IsFileExists(sourcePath))
                {
                    Debug.LogError("Can't find the source lib folder at path: " + sourcePath);
                    continue;
                }

                var progress = (float)(i + 1) / (float)libs.Count;
                EditorUtility.DisplayProgressBar("Stan's Assets.", "Installing: " + lib, progress);

                SA_AssetDatabase.CopyAsset(sourcePath, destinationPath);
            }

            EditorUtility.ClearProgressBar();
        }

        public static void UninstallLibs(string path, List<string> libs)
        {
            for (var i = 0; i < libs.Count; i++)
            {
                var lib = libs[i];
                var progress = (float)(i + 1) / (float)libs.Count;
                EditorUtility.DisplayProgressBar("Stan's Assets.", "Uninstalling: " + lib, progress);

                var libPath = path + lib;
                if (SA_AssetDatabase.IsFileExists(libPath) || SA_AssetDatabase.IsDirectoryExists(libPath))
                    SA_AssetDatabase.DeleteAsset(path + lib);
                else
                    Debug.LogWarning("There is no file to deleted at: " + libPath);
            }

            EditorUtility.ClearProgressBar();
        }
    }
}
