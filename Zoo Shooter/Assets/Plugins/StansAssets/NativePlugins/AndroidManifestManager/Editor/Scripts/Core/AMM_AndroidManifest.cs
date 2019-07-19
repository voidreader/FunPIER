using UnityEngine;
using UnityEditor;

using System.IO;
#if !(UNITY_WP8 || UNITY_METRO)
using System.Xml;
#endif
using System.Text.RegularExpressions;

using SA.Foundation.UtilitiesEditor;
using SA.Foundation.Utility;


namespace SA.Android.Manifest
{


    /// <summary>
    /// The new AMM AndroidManifest implementation, 
    /// Meant to work with several manifests files, and keep old implementation untouched
    /// </summary>
    public class AMM_AndroidManifest
    {
        private string m_path;
        private  AMM_Template m_template;

        public AMM_Template Template {
            get {
                return m_template;
            }
        }

        public AMM_AndroidManifest(string path) {
            m_path = path;
            ReadManifest(path);
        }

        private void ReadManifest(string manifestPath) {

            m_template = new AMM_Template();

           
            if (!SA_AssetDatabase.IsFileExists(manifestPath)) {
                return;
            }

#if !(UNITY_WP8 || UNITY_METRO)
            //Read XML file
            XmlDocument doc = new XmlDocument();
            doc.Load(SA_PathUtil.ConvertRelativeToAbsolutePath(manifestPath));
            XmlNode rootNode = doc.DocumentElement;

            foreach (XmlAttribute attr in rootNode.Attributes) {
                m_template.SetValue(attr.Name, attr.Value);
            }

            foreach (XmlNode childNode in rootNode.ChildNodes) {
                if (!childNode.Name.Equals("application")
                    && !childNode.Name.Equals("uses-permission")
                    && !childNode.Name.Equals("#comment")) {
                    m_template.AddProperty(childNode.Name, AMM_Manager.ParseProperty(childNode));
                }
            }

            XmlNode applicationNode = null;
            foreach (XmlNode childNode in rootNode.ChildNodes) {
                if (childNode.Name.Equals("application")) {
                    applicationNode = childNode;
                    break;
                }
            }

            foreach (XmlAttribute attr in applicationNode.Attributes) {
                m_template.ApplicationTemplate.SetValue(attr.Name, attr.Value);
            }
            foreach (XmlNode childNode in applicationNode.ChildNodes) {
                if (!childNode.Name.Equals("#comment")
                   && !childNode.Name.Equals("activity")) {
                    m_template.ApplicationTemplate.AddProperty(childNode.Name, AMM_Manager.ParseProperty(childNode));
                }
            }

            foreach (XmlNode childNode in applicationNode.ChildNodes) {
                if (childNode.Name.Equals("activity")
                   && !childNode.Name.Equals("#comment")) {

                    string activityName = "";
                    if (childNode.Attributes["android:name"] != null) {
                        activityName = childNode.Attributes["android:name"].Value;
                    } else {
                        Debug.LogWarning("Android Manifest contains activity tag without android:name attribute.");
                    }

                    XmlNode launcher = null;
                    bool isLauncher = false;
                    foreach (XmlNode actNode in childNode.ChildNodes) {
                        if (actNode.Name.Equals("intent-filter")) {
                            foreach (XmlNode intentNode in actNode.ChildNodes) {
                                if (intentNode.Name.Equals("category")) {
                                    if (intentNode.Attributes["android:name"].Value.Equals("android.intent.category.LAUNCHER")) {
                                        isLauncher = true;
                                        launcher = actNode;
                                    }
                                }
                            }
                        }
                    }

                    AMM_ActivityTemplate activity = new AMM_ActivityTemplate(isLauncher, activityName);
                    foreach (XmlAttribute attr in childNode.Attributes) {
                        activity.SetValue(attr.Name, attr.Value);
                    }

                    foreach (XmlNode actNode in childNode.ChildNodes) {
                        if (!actNode.Name.Equals("#comment")) {
                            if (actNode != launcher) {
                                activity.AddProperty(actNode.Name, AMM_Manager.ParseProperty(actNode));
                            }
                        }
                    }

                    m_template.ApplicationTemplate.AddActivity(activity);
                }
            }

            //Load Manifest Permissions
            foreach (XmlNode node in rootNode.ChildNodes) {
                if (node.Name.Equals("uses-permission")) {
                    AMM_PropertyTemplate permission = new AMM_PropertyTemplate("uses-permission");
                    permission.SetValue("android:name", node.Attributes["android:name"].Value);
                    m_template.AddPermission(permission);
                }
            }
#endif

        }




        public void SaveManifest() {
#if !(UNITY_WP8 || UNITY_METRO)

            
     
            if (!SA_AssetDatabase.IsFileExists(m_path)) {
                string m_folderPath = SA_PathUtil.GetDirectoryPath(m_path);
                if (!SA_AssetDatabase.IsValidFolder(m_folderPath)) {
                    SA_AssetDatabase.CreateFolder(m_folderPath);
                }
            }

            XmlDocument newDoc = new XmlDocument();
            //Create XML header
            XmlNode docNode = newDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            newDoc.AppendChild(docNode);

            XmlElement child = newDoc.CreateElement("manifest");
            child.SetAttribute("xmlns:android", "http://schemas.android.com/apk/res/android");
            child.SetAttribute("xmlns:tools", "http://schemas.android.com/tools");
            child.SetAttribute("package", "com.stansassets.androidnative");
       
            m_template.ToXmlElement(newDoc, child);
            newDoc.AppendChild(child);


            newDoc.Save(SA_PathUtil.ConvertRelativeToAbsolutePath(m_path));

            //Replace 'android___' pattern with 'android:'
            TextReader reader = new StreamReader(SA_PathUtil.ConvertRelativeToAbsolutePath(m_path));
            string src = reader.ReadToEnd();
            string pattern = @"android___";
            string replacement = "android:";
            Regex regex = new Regex(pattern);
            src = regex.Replace(src, replacement);

            pattern = @"tools___";
            replacement = "tools:";
            regex = new Regex(pattern);
            src = regex.Replace(src, replacement);
            reader.Close();

            TextWriter writer = new StreamWriter(SA_PathUtil.ConvertRelativeToAbsolutePath(m_path));
            writer.Write(src);
            writer.Close();

            AssetDatabase.Refresh();
#endif
        }



    }
}