using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;


namespace SA.Android.OS {

    /// <summary>
    /// Information about the current build, extracted from system properties.
    /// </summary>
    public class AN_Build {

        private static string ANDROID_CLASS = "com.stansassets.android.os.AN_Build";


        /// <summary>
        /// Enumeration of the currently known SDK version codes. 
        /// https://developer.android.com/reference/android/os/Build.VERSION_CODES
        /// </summary>
        public static class VERSION_CODES
        {
            public const int BASE = 1;
            public const int BASE_1_1 = 2;
            public const int CUPCAKE = 3;
            public const int CUR_DEVELOPMENT = 10000;
            public const int DONUT = 4;
            public const int ECLAIR = 5;
            public const int ECLAIR_0_1 = 6;
            public const int ECLAIR_MR1 = 7;
            public const int FROYO = 8;
            public const int GINGERBREAD = 9;
            public const int GINGERBREAD_MR1 = 10;
            public const int HONEYCOMB = 11;
            public const int HONEYCOMB_MR1 = 12;
            public const int HONEYCOMB_MR2 = 13;
            public const int ICE_CREAM_SANDWICH = 14;
            public const int ICE_CREAM_SANDWICH_MR1 = 15;
            public const int JELLY_BEAN = 16;
            public const int JELLY_BEAN_MR1 = 17;
            public const int JELLY_BEAN_MR2 = 18;
            public const int KITKAT = 19;
            public const int KITKAT_WATCH = 20;
            public const int LOLLIPOP = 21;
            public const int LOLLIPOP_MR1 = 22;
            public const int M = 23;
            public const int N = 24;
            public const int N_MR1 = 25;
            public const int O = 26;
            public const int O_MR1 = 27;
            public const int P = 28;
        }


        /// <summary>
        /// Various version strings.
        /// </summary>
        [Serializable]
        public class AN_BuildVersion
        {
            /// <summary>
            /// The base OS build the product is based on
            /// </summary>
            public string BASE_OS = "";

            /// <summary>
            /// The current development codename, or the string "REL" if this is a release build.
            /// </summary>
            public string CODENAME = "";

            /// <summary>
            /// The internal value used by the underlying source control to represent this build.
            /// </summary>
            public string INCREMENTAL = "";

            /// <summary>
            /// The developer preview revision of a prerelease SDK.
            /// </summary>
            public int PREVIEW_SDK_INT = 0;

            /// <summary>
            /// The user-visible version string.
            /// </summary>
            public string RELEASE = "";

            /// <summary>
            /// The SDK version of the software currently running on this hardware device.
            /// </summary>
            public int SDK_INT = 0;

            /// <summary>
            /// The user-visible security patch level.
            /// </summary>
            public string SECURITY_PATCH = "";
        }


        private static AN_BuildVersion s_version;
        public static AN_BuildVersion VERSION {
            get {
                if (s_version == null) {
                    string json = AN_Java.Bridge.CallStatic<string>(ANDROID_CLASS, "GetBuildVerion");
                    s_version = JsonUtility.FromJson<AN_BuildVersion>(json);
                }

                return s_version;
            }
        }

    }
}