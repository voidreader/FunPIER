using UnityEngine;
using System;

namespace SA.Foundation.Editor
{
    public class SA_GuiChangeContentColor : IDisposable
    {
        Color PreviousColor { get; set; }

        public SA_GuiChangeContentColor(string htmlColor)
        {
            PreviousColor = GUI.contentColor;

            var color = PreviousColor;
            ColorUtility.TryParseHtmlString(htmlColor, out color);
            GUI.contentColor = color;
        }

        public SA_GuiChangeContentColor(Color newColor)
        {
            PreviousColor = GUI.contentColor;
            GUI.contentColor = newColor;
        }

        public void Dispose()
        {
            GUI.contentColor = PreviousColor;
        }
    }
}
