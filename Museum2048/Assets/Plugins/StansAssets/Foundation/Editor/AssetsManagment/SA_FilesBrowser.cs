using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Foundation.Editor
{
    public class SA_FilesBrowser
    {
        public bool IsInMacOS => SystemInfo.operatingSystem.IndexOf("Mac OS") != -1;

        public bool IsInWinOS => SystemInfo.operatingSystem.IndexOf("Windows") != -1;

        public void Open(string path)
        {
            if (IsInWinOS)
            {
                OpenInWin(path);
            }
            else if (IsInMacOS)
            {
                OpenInMac(path);
            }
            else // couldn't determine OS
            {
                OpenInWin(path);
                OpenInMac(path);
            }
        }

        void OpenInMac(string path)
        {
            var openInsidesOfFolder = false;

            // try mac
            var macPath = path.Replace("\\", "/"); // mac finder doesn't like backward slashes

            if (System.IO.Directory.Exists(macPath)) // if path requested is a folder, automatically open insides of that folder
                openInsidesOfFolder = true;

            if (!macPath.StartsWith("\"")) macPath = "\"" + macPath;

            if (!macPath.EndsWith("\"")) macPath = macPath + "\"";

            var arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;

            try
            {
                System.Diagnostics.Process.Start("open", arguments);
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                // tried to open mac finder in windows
                // just silently skip error
                // we currently have no platform define for the current OS we are in, so we resort to this
                e.HelpLink = ""; // do anything with this variable to silence warning about not using it
            }
        }

        void OpenInWin(string path)
        {
            var openInsidesOfFolder = false;

            // try windows
            var winPath = path.Replace("/", "\\"); // windows explorer doesn't like forward slashes

            if (System.IO.Directory.Exists(winPath)) // if path requested is a folder, automatically open insides of that folder
                openInsidesOfFolder = true;

            try
            {
                System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                // tried to open win explorer in mac
                // just silently skip error
                // we currently have no platform define for the current OS we are in, so we resort to this
                e.HelpLink = ""; // do anything with this variable to silence warning about not using it
            }
        }
    }
}
