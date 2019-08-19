using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;

namespace SA.Android.App.View {


    /// <summary>
    /// This class gives you the ability to controll Android full screen mode.
    /// https://developer.android.com/training/system-ui/immersive
    /// </summary>
    public class AN_ImmersiveMode  {

        const string AN_IMMERSIVE_MODE_CLASS = "com.stansassets.core.features.ImmersiveMode";

        /// <summary>
        /// Enables the Sticky immersive  mode
        /// </summary>
        public static void Enable() {
            AN_Java.Bridge.CallStatic(AN_IMMERSIVE_MODE_CLASS, "EnableImmersiveMode");
        }
    }
}