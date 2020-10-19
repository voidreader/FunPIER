using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Android
{
    static class AN_RuntimeConfig
    {
        static AndroidJavaClass s_ImageWrapper;
        static bool s_IsInitialized = false;

        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            if (Application.platform != RuntimePlatform.Android) return;

            if (s_IsInitialized) return;

            s_IsInitialized = true;
            s_ImageWrapper = new AndroidJavaClass("com.stansassets.core.utility.AN_ImageWrapper");
            s_ImageWrapper.CallStatic("SetStorageOptions", AN_Settings.Instance.PreferredImagesStorage == AN_Settings.StorageType.Internal);
        }
    }
}
