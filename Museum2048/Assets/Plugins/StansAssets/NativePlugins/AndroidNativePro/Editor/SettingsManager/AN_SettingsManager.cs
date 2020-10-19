using UnityEngine;
using System.IO;
using SA.Foundation.Utility;

namespace SA.Android.Editor
{
    static class AN_SettingsManager
    {
        public static void Export(string filepath)
        {
            if (filepath.Length != 0)
            {
                var exportedSettings = new AN_ExportedSettings();
                var dataJson = JsonUtility.ToJson(exportedSettings);
                if (dataJson != null)
                    File.WriteAllBytes(filepath, System.Text.Encoding.UTF8.GetBytes(dataJson));
            }
        }

        public static void Import(string filepath)
        {
            if (filepath.Length != 0)
            {
                var fileContent = File.ReadAllText(filepath);
                if (fileContent != null)
                {
                    var importedSettings = JsonUtility.FromJson<AN_ExportedSettings>(fileContent);
                    JsonUtility.FromJsonOverwrite(importedSettings.AndroidSettings, AN_Settings.Instance);
                    SA_FilesUtil.Write(AN_Settings.ANDROID_GAMES_IDS_FILE_PATH, importedSettings.XmlSettings.GamesIds);
                }
            }
        }

        public static AN_ExportedSettings GetExportedSettings()
        {
            return new AN_ExportedSettings();
        }
    }
}
