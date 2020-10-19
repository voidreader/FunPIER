using SA.Android.Manifest;

namespace SA.Android.Editor
{
    class AN_SocialResolver : AN_APIResolver
    {
        public override bool IsSettingsEnabled
        {
            get => AN_Settings.Instance.Social;
            set => AN_Settings.Instance.Social = value;
        }

        protected override void AppendBuildRequirements(AN_AndroidBuildRequirements buildRequirements)
        {
            if (AN_Settings.Instance.PreferredImagesStorage != AN_Settings.StorageType.ForceInternal)
            {
                buildRequirements.AddPermission(AMM_ManifestPermission.WRITE_EXTERNAL_STORAGE);
                buildRequirements.AddPermission(AMM_ManifestPermission.READ_EXTERNAL_STORAGE);
            }

            buildRequirements.AddBinaryDependency(AN_BinaryDependency.AndroidX);
        }
    }
}
