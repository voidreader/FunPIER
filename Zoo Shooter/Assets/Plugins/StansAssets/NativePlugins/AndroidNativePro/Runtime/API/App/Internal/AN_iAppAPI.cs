using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Templates;


namespace SA.Android.App.Internal
{
    public interface AN_iAppAPI
    {
        void LockScreen(string message);
        void UnlockScreen();

        void AlertDialogShow(AN_AlertDialog dialog, Action<AN_AlertDialog.AN_AlertDialogCloseInfo> callback);
        void AlertDialogHide(AN_AlertDialog dialog);

    }
}