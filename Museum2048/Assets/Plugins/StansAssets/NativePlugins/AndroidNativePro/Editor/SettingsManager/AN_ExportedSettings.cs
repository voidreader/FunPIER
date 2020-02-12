using UnityEngine;
using System;
using UnityEditor;

namespace SA.Android.Editor
{
    [Serializable]
    internal class AN_ExportedSettings
    {
        public string AndroidSettings
        {
            get
            {
                return m_AndroidSettings;
            }
        }

        public AN_XMLSettings XmlSettings
        {
            get
            {
                return m_XmlSettings;
            }
        }

        [SerializeField]
        private string m_AndroidSettings;

        [SerializeField]
        private AN_XMLSettings m_XmlSettings;

        public AN_ExportedSettings()
        {
            m_AndroidSettings = JsonUtility.ToJson(AN_Settings.Instance);
            m_XmlSettings = new AN_XMLSettings();
        }
    }
}
