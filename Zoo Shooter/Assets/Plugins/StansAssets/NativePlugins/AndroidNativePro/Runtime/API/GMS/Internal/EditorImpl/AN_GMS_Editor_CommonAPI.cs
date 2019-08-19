using System;

using SA.Foundation.Templates;
using SA.Android.GMS.Common.Images;

namespace SA.Android.GMS.Internal
{

    public class AN_GMS_Editor_CommonAPI : AN_iGMS_Common
    {

        public void LoadImage(string url, Action<AN_ImageLoadResult> callback) {
            var error = new SA_Error(1, "Can only be used on a real device");
            var result = new AN_ImageLoadResult(error);

            callback.Invoke(result);
        }
    }
}