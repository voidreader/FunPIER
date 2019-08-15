using System;
using UnityEngine;

using SA.Android.Content;
using SA.Android.Utilities;
using SA.Android.Content.Pm;

using SA.Foundation.Async;

namespace SA.Android.App {

    /// <summary>
    /// An activity is a single, focused thing that the user can do. 
    /// Almost all activities interact with the user, 
    /// so the Activity class takes care of creating a window for you in which you can place your UI with setContentView(View). 
    /// While activities are often presented to the user as full-screen windows, 
    /// they can also be used in other ways: as floating windows (via a theme with R.attr.windowIsFloating set) 
    /// or embedded inside of another activity (using ActivityGroup). 
    /// </summary>
    [Serializable]
    public class AN_Activity : AN_Context
    {
        public enum Result
        {
            Ok = -1,
            Canceled = 0
        }

#pragma warning disable 414

        [SerializeField] protected AN_ActivityId m_classId = AN_ActivityId.Undefined;
        [SerializeField] protected string m_instanceId = string.Empty;

#pragma warning restore 414


        private static string ANDROID_CLASS = "com.stansassets.android.app.AN_Activity";


        //--------------------------------------
        // Public Methods
        //--------------------------------------


        /// <summary>
        /// Launch a new activity. 
        /// You will not receive any information about when the activity exits. 
        /// This implementation overrides the base version, providing information about the activity performing the launch. 
        /// Because of this additional information, the Intent.FLAG_ACTIVITY_NEW_TASK launch flag is not required; 
        /// if not specified, the new activity will be added to the task of the caller.
        /// </summary>
        /// <param name="intent">The intent to start.</param>
        public bool StartActivity(AN_Intent intent) {
            return AN_Java.Bridge.CallStatic<bool>(ANDROID_CLASS, "StartActivity", this, intent);
        }


        /// <summary>
        /// Launch an activity for which you would like a result when it finished. 
        /// When this activity exits, your callback will be called.
        /// </summary>
        /// <param name="intent">The intent to start.</param>
        /// <param name="callback">Activity result callback</param>
        public bool StartActivityForResult(AN_Intent intent, Action<AN_ActivityResult> callback) {

            if (Application.isEditor) {
                SA_Coroutine.WaitForSeconds(1, () => {
                    callback.Invoke(new AN_ActivityResult());
                });
                return true;
            }

            return AN_Java.Bridge.CallStaticWithCallback<bool, AN_ActivityResult>(ANDROID_CLASS, "StartActivityForResult", callback, this, intent);
        }

        /// <summary>
        /// Call this when your activity is done and should be closed. 
        /// The ActivityResult is propagated back to whoever launched the activity.
        /// </summary>
        public bool Finish() {
            return AN_Java.Bridge.CallStatic<bool>(ANDROID_CLASS, "Finish", this);
        }

        /// <summary>
        /// Move the task containing this activity to the back of the activity stack. The activity's order within the task is unchanged
        /// </summary>
        /// <param name="nonRoot">If false then this only works if the activity is the root of a task; if true it will work for any activity in a task.</param>
        /// <returns>If the task was moved (or it was already at the back) true is returned, else false.</returns>
        public bool MoveTaskToBack(bool nonRoot)
        {
            return AN_Java.Bridge.CallStatic<bool>(ANDROID_CLASS, "MoveTaskToBack", this, nonRoot);
        }

        //--------------------------------------
        // Overriden Methods
        //--------------------------------------

        /// <summary>
        /// Return PackageManager instance to find global package information.
        /// </summary>
        public override AN_PackageManager GetPackageManager() {
            return new AN_PackageManager(this);
        }



        //--------------------------------------
        // Protected Methods
        //--------------------------------------


    }

}

