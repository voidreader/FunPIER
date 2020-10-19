using UnityEngine;
using SA.Android.Utilities;
using System;

namespace SA.Android.App
{
    class AN_AppNativeAPI : AN_iAppAPI
    {
        const string AN_ALERT_DIALOG_CLASS = "com.stansassets.android.app.dialogs.AN_AlertDialog";
        const string AN_PROGRESSS_DIALOG_CLASS = "com.stansassets.android.app.dialogs.AN_ProgressDialog";

        //--------------------------------------
        //  Progres Dialog
        //--------------------------------------

        public void LockScreen(string message)
        {
            AN_Java.Bridge.CallStatic(AN_PROGRESSS_DIALOG_CLASS, "LockScreen", message);
        }

        public void UnlockScreen()
        {
            AN_Java.Bridge.CallStatic(AN_PROGRESSS_DIALOG_CLASS, "UnlockScreen");
        }

        //--------------------------------------
        //  Alert Dialog
        //--------------------------------------

        public void AlertDialogHide(AN_AlertDialog dialog)
        {
            var json = JsonUtility.ToJson(dialog);
            AN_Java.Bridge.CallStatic(AN_ALERT_DIALOG_CLASS, "Hide", json);
        }

        public void AlertDialogShow(AN_AlertDialog dialog, Action<AN_AlertDialog.AN_AlertDialogCloseInfo> callback)
        {
            var json = JsonUtility.ToJson(dialog);

            AN_Java.Bridge.CallStaticWithCallback(AN_ALERT_DIALOG_CLASS, "Show", callback, json);
        }
    }
}
