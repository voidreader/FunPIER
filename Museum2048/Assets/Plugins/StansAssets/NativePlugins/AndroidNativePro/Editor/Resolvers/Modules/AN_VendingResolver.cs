using SA.Android.Manifest;

namespace SA.Android.Editor
{
    class AN_VendingResolver : AN_APIResolver
    {
        public override bool IsSettingsEnabled
        {
            get => AN_Settings.Instance.Vending;
            set => AN_Settings.Instance.Vending = value;
        }

        protected override void AppendBuildRequirements(AN_AndroidBuildRequirements buildRequirements)
        {
            if (AN_Settings.Instance.GooglePlayBilling)
            {
                buildRequirements.AddBinaryDependency(AN_BinaryDependency.BillingClient);
                buildRequirements.AddPermission(AMM_ManifestPermission.BILLING);
            }

            if (AN_Settings.Instance.Licensing) buildRequirements.AddPermission(AMM_ManifestPermission.CHECK_LICENSE);

            if (AN_Settings.Instance.GooglePlayBilling || AN_Settings.Instance.Licensing) buildRequirements.AddInternalLib("an_vending.aar");
        }
    }
}
