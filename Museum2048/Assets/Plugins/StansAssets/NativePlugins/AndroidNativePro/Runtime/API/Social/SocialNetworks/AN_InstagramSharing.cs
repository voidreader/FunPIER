using System;
using UnityEngine;
using System.Collections.Generic;

using SA.Android.Utilities;
using SA.Android.App;
using SA.Android.Content;
using SA.Android.Content.Pm;

namespace SA.Android.Social
{
    public class AN_InstagramSharing : AN_SocialImageShareBuilders
    {

        private const string APP_PACKAGE = "com.instagram.android";

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


        public override void AddImage(Texture2D image) {
            m_images = new List<Texture2D>();
            m_images.Add(image);
        }
    }
}

