using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Manifest;

namespace SA.Android
{
    public class AN_CameraAndGalleryResolver : AN_APIResolver
    {
        public override bool IsSettingsEnabled {
            get { return AN_Settings.Instance.CameraAndGallery; }
            set { AN_Settings.Instance.CameraAndGallery = value; }
        }

        public override void AppendBuildRequirements(AN_AndroidBuildRequirements buildRequirements) {

            buildRequirements.AddPermission(AMM_ManifestPermission.READ_EXTERNAL_STORAGE);
            buildRequirements.AddPermission(AMM_ManifestPermission.WRITE_EXTERNAL_STORAGE);

            buildRequirements.AddInternalLib("an_gallery.aar");

            /*

            buildRequirements.AddBinaryDependency(AN_BinaryDependency.SupportV4CoreUtils);
            var provider = new AMM_PropertyTemplate("provider");
            provider.SetValue("android:name", "android.support.v4.content.FileProvider");
            provider.SetValue("android:authorities", "com.stansassets.android.fileprovider");
            provider.SetValue("android:exported", "false");
            provider.SetValue("android:grantUriPermissions", "true");

            var meta = new AMM_PropertyTemplate("meta-data");
            meta.SetValue("android:name", "android.support.FILE_PROVIDER_PATHS");
            meta.SetValue("android:resource", "@xml/file_paths");

            provider.AddProperty(meta);


            buildRequirements.AddApplicationProperty(provider);*/

        }

    }
}