using System;

using SA.Android.Utilities;
using SA.Android.GMS.Common.Images;


namespace SA.Android.GMS.Internal
{
    public class AN_GMS_Native_CommonAPI : AN_iGMS_Common
    {
        const string AN_ImageManager = "com.stansassets.gms.common.images.AN_ImageManager";


        public void LoadImage(string url, Action<AN_ImageLoadResult> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_ImageManager, "LoadImage", callback, url);
        }
    }
}