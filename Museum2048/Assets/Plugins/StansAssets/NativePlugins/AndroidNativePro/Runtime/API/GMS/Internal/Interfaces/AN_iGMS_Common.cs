using System;
using SA.Android.GMS.Common.Images;

namespace SA.Android.GMS.Internal
{
    interface AN_iGMS_Common
    {
        void LoadImage(string url, Action<AN_ImageLoadResult> callback);
    }
}
