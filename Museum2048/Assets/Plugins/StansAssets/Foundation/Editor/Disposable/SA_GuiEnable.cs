using UnityEngine;
using System;

namespace SA.Foundation.Editor
{
    public class SA_GuiEnable : IDisposable
    {
        bool PreviousState { get; set; }

        public SA_GuiEnable(bool newState)
        {
            PreviousState = GUI.enabled;
            GUI.enabled = newState;
        }

        public void Dispose()
        {
            GUI.enabled = PreviousState;
        }
    }
}
