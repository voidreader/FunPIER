using SA.Android.Manifest;

namespace SA.Android.Editor
{
    class AN_ContactsResolver : AN_APIResolver
    {
        public override bool IsSettingsEnabled
        {
            get => AN_Settings.Instance.Contacts;
            set => AN_Settings.Instance.Contacts = value;
        }

        protected override void AppendBuildRequirements(AN_AndroidBuildRequirements buildRequirements)
        {
            buildRequirements.AddPermission(AMM_ManifestPermission.READ_CONTACTS);
        }
    }
}
