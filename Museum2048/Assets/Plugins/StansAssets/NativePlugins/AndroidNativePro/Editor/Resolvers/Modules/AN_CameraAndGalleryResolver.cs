using SA.Android.Manifest;

namespace SA.Android.Editor
{
    class AN_CameraAndGalleryResolver : AN_APIResolver
    {
        public override bool IsSettingsEnabled
        {
            get => AN_Settings.Instance.CameraAndGallery;
            set => AN_Settings.Instance.CameraAndGallery = value;
        }

        protected override void AppendBuildRequirements(AN_AndroidBuildRequirements buildRequirements)
        {
            if (AN_Settings.Instance.PreferredImagesStorage != AN_Settings.StorageType.ForceInternal)
            {
                buildRequirements.AddPermission(AMM_ManifestPermission.READ_EXTERNAL_STORAGE);
                buildRequirements.AddPermission(AMM_ManifestPermission.WRITE_EXTERNAL_STORAGE);
            }

            buildRequirements.AddInternalLib("an_gallery.aar");
        }
    }
}
