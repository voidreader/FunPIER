using System;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Foundation.Utility;


namespace SA.Android
{
    [Serializable]
    public class AN_GamesIds
    {
        [SerializeField] string m_rawData;

        //Parsed from games-ids.xml
        [SerializeField] string app_id;
        [SerializeField] string package_name;
        private List<KeyValuePair<string, string>> m_leaderboards = new List<KeyValuePair<string, string>>();
        private List<KeyValuePair<string, string>> m_achievements = new List<KeyValuePair<string, string>>();

        public AN_GamesIds(string rawData) {
            m_rawData = rawData;

            try {
                
                XmlDocument doc = new XmlDocument();
                doc.Load(SA_PathUtil.ConvertRelativeToAbsolutePath(AN_Settings.ANDROID_GAMES_IDS_FILE_PATH));
                XmlNodeList xnList = doc.SelectNodes("resources/string");

                foreach (XmlNode node in xnList) {
                    string name = node.Attributes["name"].Value;
                    if(name.Equals("app_id")) {
                        app_id = node.InnerText;
                    }

                    if (name.Equals("package_name")) {
                        package_name = node.InnerText;
                    }

                    if (name.StartsWith("achievement") && name.Contains("_")) {
                        var key = name.Split('_')[1];
                        var value = node.InnerText;
                        m_achievements.Add(new KeyValuePair<string, string>(key, value));
                    }
                    
                    if (name.StartsWith("leaderboard") && name.Contains("_")) {
                        var key = name.Split('_')[1];
                        var value = node.InnerText;
                        m_leaderboards.Add(new KeyValuePair<string, string>(key, value));
                    }
                }

            } catch (Exception ex) {
                AN_Logger.LogError("Error reading AN_GamesIds");
                AN_Logger.LogError(AN_Settings.ANDROID_GAMES_IDS_FILE_PATH + " filed: " + ex.Message);
            }
        }


        public string RawData {
            get {
                return m_rawData;
            }
        }


        public string AppId {
            get {
                return app_id;
            }
        }

        public string PackageName {
            get {
                return package_name;
            }

        }

        public List<KeyValuePair<string, string>> Leaderboards {
            get {
                return m_leaderboards;
            }
        }

        public List<KeyValuePair<string, string>> Achievements {
            get {
                return m_achievements;
            }
        }
    }
}