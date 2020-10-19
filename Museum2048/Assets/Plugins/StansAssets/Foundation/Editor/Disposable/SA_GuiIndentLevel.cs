using UnityEngine;
using UnityEditor;
using System;

namespace SA.Foundation.Editor
{
    public class SA_GuiIndentLevel : IDisposable
    {
        readonly int m_indent = 1;

        public SA_GuiIndentLevel(int indent)
        {
            m_indent = indent;
            EditorGUI.indentLevel += m_indent;
        }

        public void Dispose()
        {
            EditorGUI.indentLevel -= m_indent;
        }
    }
}
