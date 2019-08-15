using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.App.Internal;
using SA.Foundation.Utility;
using SA.Foundation.Async;


namespace SA.Android.App
{

    /// <summary>
    /// A subclass of Dialog that can display one, two or three buttons. 
    /// </summary>
    [Serializable]
    public class AN_AlertDialog  : AN_Dialog
    {

        private const int NEUTRAL = 0;
        private const int POSITIVE = 1;
        private const int NEGATIVE = 2;


        [Serializable]
        public class AN_ButtonInfo 
        {
            public String m_id;
            public String m_text;
            public int m_type;
            public Action m_callback;
        }

        [Serializable]
        public class AN_AlertDialogCloseInfo
        {
            public String m_buttonid;
        }


        [SerializeField] List<AN_ButtonInfo> m_buttons;


        /// <summary>
        /// Creates a dialog window that uses the default dialog theme.
        /// </summary>
        public AN_AlertDialog() : base() { }


        /// <summary>
        /// Creates a dialog window that uses a custom dialog style.
        /// </summary>
        /// <param name="theme">Dialog theme</param>
        public AN_AlertDialog(AN_DialogTheme theme) : base(theme) {}



        /// <summary>
        /// Set a listener to be invoked when the neutral button of the dialog is pressed.
        /// </summary>
        /// <param name="text">button text</param>
        /// <param name="callback">click listner</param>
        public void SetNeutralButton(string text, Action callback) {
            AddButton(text, NEUTRAL, callback);
        }

        /// <summary>
        /// Set a listener to be invoked when the positive button of the dialog is pressed.
        /// </summary>
        /// <param name="text">button text</param>
        /// <param name="callback">click listner</param>
        public void SetPositiveButton(string text, Action callback) {
            AddButton(text, POSITIVE, callback);
        }

        /// <summary>
        /// Set a listener to be invoked when the negative button of the dialog is pressed.
        /// </summary>
        /// <param name="text">button text</param>
        /// <param name="callback">click listner</param>
        public void SetNegativeButton(string text, Action callback) {
            AddButton(text, NEGATIVE, callback);
        }

        /// <summary>
        /// Start the dialog and display it on screen.
        /// </summary>
        public override void Show() {

            if(Application.isEditor) {
                foreach(var button in m_buttons) {
                    if(button.m_type == POSITIVE) {
                        SA_Coroutine.WaitForSeconds(1, () => {
                            button.m_callback.Invoke();
                        });
                        return;
                    }
                }
                return;
            }

            AN_AppLib.API.AlertDialogShow(this, (closeResult) => {

                foreach(var button in m_buttons) {
                    if(button.m_id.Equals(closeResult.m_buttonid)) {
                        button.m_callback.Invoke();
                        return;
                    }
                }

                //looks like alert was dismissed wihtout user taping buttons
                //so far this is disabled, let's see if we gonna need it in guture
               // OnUserCanceled.Invoke();

            });
        }


        /// <summary>
        /// Closes the dialog.
        /// </summary>
        public override void Hide() {
            AN_AppLib.API.AlertDialogHide(this);
        }




        private void AddButton(string text, int type, Action callback) {
            var button = new AN_ButtonInfo();
            button.m_id = SA_IdFactory.RandomString;
            button.m_type = type;
            button.m_text = text;
            button.m_callback = callback;

            if(m_buttons == null) {
                m_buttons = new List<AN_ButtonInfo>();
            }

            m_buttons.Add(button);
        }
    }
}