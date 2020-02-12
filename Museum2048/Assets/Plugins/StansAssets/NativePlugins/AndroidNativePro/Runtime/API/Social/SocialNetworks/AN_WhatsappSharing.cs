
using SA.Android.App;
using SA.Android.Content;
using SA.Android.Content.Pm;

namespace SA.Android.Social
{
    public class AN_WhatsappSharing : AN_SocialFullShareBuilder
    {

        private const string APP_PACKAGE = "com.whatsapp";

        public static bool IsAppInstalled {
            get {
                var pm = AN_MainActivity.Instance.GetPackageManager();
                var info = pm.GetPackageInfo(APP_PACKAGE, AN_PackageManager.GET_ACTIVITIES);

                return info != null;
            }
        }

        protected override AN_Intent MakeSharingIntent() {
            SetPackage(APP_PACKAGE);
            return ShareIntent;
        }
    }
}

