using UnityEngine;
using UnityEditor;
using System;

namespace SA.Foundation.Editor
{
    public class SA_GuiChangeColor : IDisposable
    {
        Color m_previousColor { get; set; }

        public SA_GuiChangeColor(string htmlColor)
        {
            m_previousColor = GUI.color;

            var color = m_previousColor;
            ColorUtility.TryParseHtmlString(htmlColor, out color);
            GUI.color = color;
        }

        public SA_GuiChangeColor(Color newColor)
        {
            m_previousColor = GUI.color;
            GUI.color = newColor;
        }

        public void Dispose()
        {
            GUI.color = m_previousColor;
        }
    }
}
