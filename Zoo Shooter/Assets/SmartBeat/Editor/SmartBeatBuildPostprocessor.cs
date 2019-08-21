using Ionic.Zip;
using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;

public class SmartBeatBuildPostprocessor
{
    static string libpath = Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "Temp" + Path.DirectorySeparatorChar + "StagingArea" + Path.DirectorySeparatorChar + "libs" + Path.DirectorySeparatorChar;
    static string sympath = Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "Temp" + Path.DirectorySeparatorChar + "StagingArea" + Path.DirectorySeparatorChar + "symbols" + Path.DirectorySeparatorChar;
    const string postPath = "https://api.smrtbeat.com/1/upload_symbols";
    const string progressbarTitle = "SmartBeat Symbol Upload";

    static string[] filePatterns = { "*.sym.so", "*.sym", "*.so.debug" };

    #if UNITY_5_4_OR_NEWER || UNITY_5_3_5 || UNITY_5_3_6 || UNITY_5_3_7 || UNITY_5_3_8
    [PostProcessBuildAttribute()]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.Android)
            return;

        bool enabled = false;
        string apiKey = "";
        string apiToken = "";

        SmartBeatBuildPreferences buildPrefs = (SmartBeatBuildPreferences)EditorGUIUtility.Load(SmartBeatPreferenceEditor.BUILD_ASSET_NAME);
        if (buildPrefs != null)
        {
            enabled = buildPrefs.androidSoUpload;
            apiKey = buildPrefs.androidApiKey;
            apiToken = buildPrefs.androidApiToken;
        }
        try
        {
            EditorUtility.DisplayProgressBar(progressbarTitle, "Finding android symbols", 0.0f);
            if (enabled)
            {
                string[] debugFiles = findSymbolFiles();
                if (debugFiles.Length > 0)
                {
                    string zFile = makeArchive(debugFiles);
                    EditorUtility.DisplayProgressBar(progressbarTitle, "Uploading android symbols", 0.5f);
                    uploadFile(zFile, apiKey, apiToken);
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }
    #endif

    private static string[] findSymbolFiles()
    {


        string fullLibPath = Application.dataPath + libpath;
        string fullSymPath = Application.dataPath + sympath;

        List<string> fileList = new List<string>();
        foreach (string filePattern in filePatterns)
        {
            fileList.AddRange(Directory.GetFiles(fullLibPath, filePattern, SearchOption.AllDirectories));
            try
            {
                fileList.AddRange(Directory.GetFiles(fullSymPath, filePattern, SearchOption.AllDirectories));
            }
            catch { }
        }

        return fileList.ToArray();
    }

    private static string makeArchive(string[] files) {
        string tempZipFile = Application.temporaryCachePath + Path.DirectorySeparatorChar + Path.GetRandomFileName() + ".zip";
        using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
        {
            zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
            foreach (string file in files)
            {
                string filePath = Path.GetFullPath(file);
                zip.AddFile(filePath);
            }
            zip.Save(tempZipFile);
        }

        return tempZipFile;
    }

    private static bool uploadFile(string fileName, string apiKey, string apiToken) {
        NameValueCollection nvc = new NameValueCollection();
        nvc.Add("api_key", apiKey);
        nvc.Add("api_token", apiToken);

        string resp = HttpUploadFile(postPath, @fileName, "so_file", "application/octet-stream", nvc);
        if (resp != null)
            return true;
        else
            return false;
    }

    private static string HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc)
    {
        string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
        byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
        byte[] boundarybytesF = System.Text.Encoding.ASCII.GetBytes("--" + boundary + "\r\n");  // the first time it itereates, you need to make sure it doesn't put too many new paragraphs down or it completely messes up poor webbrick.  


        HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
        wr.Method = "POST";
        wr.KeepAlive = true;
        wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
        wr.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        var nvc2 = new NameValueCollection();
        wr.Headers.Add(nvc2);
        wr.ContentType = "multipart/form-data; boundary=" + boundary;


        Stream rs = wr.GetRequestStream();

        bool firstLoop = true;
        string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
        foreach (string key in nvc.Keys)
        {
            if (firstLoop)
            {
                rs.Write(boundarybytesF, 0, boundarybytesF.Length);
                firstLoop = false;
            }
            else
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
            }
            string formitem = string.Format(formdataTemplate, key, nvc[key]);
            byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
            rs.Write(formitembytes, 0, formitembytes.Length);
        }
        rs.Write(boundarybytes, 0, boundarybytes.Length);

        string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
        string header = string.Format(headerTemplate, paramName, new FileInfo(file).Name, contentType);
        byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
        rs.Write(headerbytes, 0, headerbytes.Length);

        FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
        byte[] buffer = new byte[4096];
        int bytesRead = 0;
        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
        {
            rs.Write(buffer, 0, bytesRead);
        }
        fileStream.Close();

        byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
        rs.Write(trailer, 0, trailer.Length);
        rs.Close();

        WebResponse wresp = null;
        try
        {
            wresp = wr.GetResponse();
            Stream stream2 = wresp.GetResponseStream();
            StreamReader reader2 = new StreamReader(stream2);
            return reader2.ReadToEnd();
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Caught exception uploading symbols: " + ex);
            if (wresp != null)
            {
                wresp.Close();
                wresp = null;
            }
            return null;
        }
        finally
        {
            wr = null;
        }
    }
} 