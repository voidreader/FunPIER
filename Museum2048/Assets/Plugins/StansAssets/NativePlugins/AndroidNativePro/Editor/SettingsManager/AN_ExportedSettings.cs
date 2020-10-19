using UnityEngine;
using System;
using UnityEditor;

namespace SA.Android.Editor
{
    [Serializable]
    class AN_ExportedSettings
    {
        public string AndroidSettings => m_AndroidSettings;

        public AN_XMLSettings XmlSettings => m_XmlSettings;

        [SerializeField]
        string m_AndroidSettings;

        [SerializeField]
        AN_XMLSettings m_XmlSettings;

        public AN_ExportedSettings()
        {
            m_AndroidSettings = JsonUtility.ToJson(AN_Settings.Instance);
            m_XmlSettings = new AN_XMLSettings();
        }
    }
}
