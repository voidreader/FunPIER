using SA.Android.Manifest;

namespace SA.Android
{
    public class AN_VendingResolver : AN_APIResolver
    {
        public override bool IsSettingsEnabled {
            get { return AN_Settings.Instance.Vending; }
            set { AN_Settings.Instance.Vending = value; }
        }

        public override void AppendBuildRequirements(AN_AndroidBuildRequirements buildRequirements) {
            
            buildRequirements.AddBinaryDependency(AN_BinaryDependency.BillingClient);
            buildRequirements.AddPermission(AMM_ManifestPermission.BILLING);

            if (AN_Settings.Instance.Licensing) 
            {
                buildRequirements.AddPermission(AMM_ManifestPermission.CHECK_LICENSE);
            }


            buildRequirements.AddInternalLib("an_vending.aar");
        }
    }
}