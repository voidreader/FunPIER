using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Utility;


namespace SA.Android.App
{
    /// <summary>
    /// Base class for Dialogs.
    /// </summary>
    [Serializable]
    public abstract class AN_Dialog 
    {

        [SerializeField] string m_id;
        [SerializeField] string m_title;
        [SerializeField] string m_message;
        [SerializeField] bool m_cancelable;
        [SerializeField] int m_themeId;

        /// <summary>
        /// Creates a dialog window that uses the default dialog theme.
        /// </summary>
        public AN_Dialog():this(AN_DialogTheme.Default) {
         
        }

        /// <summary>
        /// Creates a dialog window that uses a custom dialog style.
        /// </summary>
        /// <param name="theme">Dialog theme</param>
        public AN_Dialog(AN_DialogTheme theme) {
            m_id = SA_IdFactory.RandomString;
            m_themeId = (int)theme;
        }


        /// <summary>
        /// Start the dialog and display it on screen.
        /// </summary>
        public abstract void Show();


        /// <summary>
        /// Closes the dialog.
        /// </summary>
        public abstract void Hide();


        /// <summary>
        /// Internal dialog id
        /// </summary>
        public string Id {
            get {
                return m_id;
            }
        }

        /// <summary>
        /// Set the title text for this dialog's window.
        /// </summary>
        public string Title {
            get {
                return m_title;
            }

            set {
                m_title = value;
            }
        }

        /// <summary>
        /// Set the message to display.
        /// </summary>
        public string Message {
            get {
                return m_message;
            }

            set {
                m_message = value;
            }
        }

        /// <summary>
        /// Sets whether this dialog is cancelable with the BACK key.
        /// </summary>
        public bool Cancelable {
            get {
                return m_cancelable;
            }

            set {
                m_cancelable = value;
            }
        }


        /// <summary>
        /// Dialog theme
        /// </summary>
        public AN_DialogTheme ThemeId {
            get {
                return (AN_DialogTheme)m_themeId;
            }
        }
    }
}