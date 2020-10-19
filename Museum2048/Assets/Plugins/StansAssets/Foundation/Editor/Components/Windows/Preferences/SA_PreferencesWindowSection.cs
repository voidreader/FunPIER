using System;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Foundation.Editor
{
    [Serializable]
    public class SA_PreferencesWindowSection
    {
        [SerializeField]
        GUIContent m_content;
        [SerializeField]
        SA_GUILayoutElement m_layout;

        public SA_PreferencesWindowSection(string name, SA_GUILayoutElement layout)
        {
            m_content = new GUIContent(name);
            m_layout = layout;
        }

        public string Name => m_content.text;

        public SA_GUILayoutElement Layout => m_layout;

        public GUIContent Content => m_content;
    }
}
