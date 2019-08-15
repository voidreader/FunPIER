////////////////////////////////////////////////////////////////////////////////
//  
// @module Manifest Manager
// @author Alex Yaremenko (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using UnityEditor;

using System.IO;
using SA.Foundation.Config;

#if !(UNITY_WP8 || UNITY_METRO)
using System.Xml;
#endif

using System.Collections;
using System.Text.RegularExpressions;

using SA.Foundation.UtilitiesEditor;
using SA.Foundation.Utility;


namespace SA.Android.Manifest {


    static public class AMM_Manager {


        private static AMM_Template s_template = null;

    	public static bool ManifestExists {
    		get {
                return SA_AssetDatabase.IsFileExists(AMM_Settings.MANIFEST_FILE_PATH);
    		}
    	}

    	private static void ReadManifest(string manifestPath) {

    #if !(UNITY_WP8 || UNITY_METRO)
    		//Read XML file
    		s_template = new AMM_Template ();
    			
    		XmlDocument doc = new XmlDocument ();
            doc.Load(SA_PathUtil.ConvertRelativeToAbsolutePath(manifestPath));
    		XmlNode rootNode = doc.DocumentElement;
    			
    		foreach (XmlAttribute attr in rootNode.Attributes) {				
    			s_template.SetValue(attr.Name, attr.Value);
    		}
    			
    		foreach (XmlNode childNode in rootNode.ChildNodes) {
    			if (!childNode.Name.Equals("application")
    			    && !childNode.Name.Equals("uses-permission")
    			    && !childNode.Name.Equals("#comment")) {
    				s_template.AddProperty(childNode.Name, ParseProperty(childNode));
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
    			s_template.ApplicationTemplate.SetValue(attr.Name, attr.Value);
    		}
    		foreach (XmlNode childNode in applicationNode.ChildNodes) {
    			if(!childNode.Name.Equals("#comment")
    			   && !childNode.Name.Equals("activity")) {
    				s_template.ApplicationTemplate.AddProperty(childNode.Name, ParseProperty(childNode));
    			}
    		}
    			
    		foreach (XmlNode childNode in applicationNode.ChildNodes) {
    			if(childNode.Name.Equals("activity")
    			   && !childNode.Name.Equals("#comment")) {

    				string activityName = "";
    				if(childNode.Attributes["android:name"] != null) {
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
    							activity.AddProperty(actNode.Name, ParseProperty(actNode));
    						}
    					}
    				}
    					
    				s_template.ApplicationTemplate.AddActivity(activity);
    			}
    		}
    			
    		//Load Manifest Permissions
    		foreach (XmlNode node in rootNode.ChildNodes) {
    			if (node.Name.Equals("uses-permission")) {
    				AMM_PropertyTemplate permission = new AMM_PropertyTemplate("uses-permission");
    				permission.SetValue("android:name", node.Attributes["android:name"].Value);
    				s_template.AddPermission(permission);
    			}
    		}
    #endif

    	}

    	public static void CreateDefaultManifest() {
            ReadManifest (AMM_Settings.DEFAULT_MANIFEST_PATH);
    		SaveManifest ();
    	}

    	public static void SaveManifest() {
#if !(UNITY_WP8 || UNITY_METRO)

          
            if(!SA_AssetDatabase.IsFileExists(AMM_Settings.MANIFEST_FILE_PATH)) {
                //Make sure we have a folder
                if(!SA_AssetDatabase.IsValidFolder(AMM_Settings.MANIFEST_FOLDER_PATH)) {
                    SA_AssetDatabase.CreateFolder(AMM_Settings.MANIFEST_FOLDER_PATH);
                }
               
            }

    		XmlDocument newDoc = new XmlDocument ();
    		//Create XML header
    		XmlNode docNode = newDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
    		newDoc.AppendChild(docNode);

    		XmlElement child = newDoc.CreateElement ("manifest");
    		s_template.ToXmlElement (newDoc, child);
    		newDoc.AppendChild (child);


            newDoc.Save (SA_PathUtil.ConvertRelativeToAbsolutePath(AMM_Settings.MANIFEST_FILE_PATH));

    		//Replace 'android___' pattern with 'android:'
            TextReader reader = new StreamReader (SA_PathUtil.ConvertRelativeToAbsolutePath(AMM_Settings.MANIFEST_FILE_PATH));
    		string src = reader.ReadToEnd ();
    		string pattern = @"android___";
    		string replacement = "android:";
    		Regex regex = new Regex (pattern);
    		src = regex.Replace(src, replacement);

    		pattern = @"tools___";
    		replacement = "tools:";
    		regex = new Regex(pattern);
    		src = regex.Replace (src, replacement);
    		reader.Close ();

            TextWriter writer = new StreamWriter(SA_PathUtil.ConvertRelativeToAbsolutePath(AMM_Settings.MANIFEST_FILE_PATH));
    		writer.Write (src);
    		writer.Close ();

    		AssetDatabase.Refresh ();
    #endif
    	}

    	public static AMM_Template GetManifest() {
            if (s_template == null) {
                if (ManifestExists) {
                    ReadManifest(AMM_Settings.MANIFEST_FILE_PATH);
                } else {
                    CreateDefaultManifest();
                }
                    
    		}
    		return s_template;
    	}


    #if !(UNITY_WP8 || UNITY_METRO)
    	public static AMM_PropertyTemplate ParseProperty(XmlNode node) {
    		AMM_PropertyTemplate property = new AMM_PropertyTemplate (node.Name);
    		//Get Values
    		foreach (XmlAttribute attr in node.Attributes) {
    			property.SetValue(attr.Name, attr.Value);
    		}
    		//Get Properties
    		foreach (XmlNode n in node.ChildNodes) {
    			if (!n.Name.Equals("#comment")) {
    				property.AddProperty(n.Name, ParseProperty(n));
    			}
    		}

    		return property;
    	}
    #endif
    }

}
