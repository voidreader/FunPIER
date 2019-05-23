using SA.Android.Manifest;


namespace SA.Android
{
    public class AN_VendingResolver : AN_APIResolver
    {
        
        private const string AN_BILLING_PROXY_ACTIVITY_CLASS = "com.stansassets.billing.core.AN_BillingProxyActivity";


        public override bool IsSettingsEnabled {
            get { return AN_Settings.Instance.Vending; }
            set { AN_Settings.Instance.Vending = value; }
        }

        public override void AppendBuildRequirements(AN_AndroidBuildRequirements buildRequirements) {
            var proxyActivity = new AMM_ActivityTemplate(false, AN_BILLING_PROXY_ACTIVITY_CLASS);
            proxyActivity.SetValue("android:launchMode", "singleTask");
            proxyActivity.SetValue("android:label", "@string/app_name");
            proxyActivity.SetValue("android:configChanges", "fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen");
            proxyActivity.SetValue("android:theme", "@android:style/Theme.Translucent.NoTitleBar");

            buildRequirements.AddActivity(proxyActivity);
            buildRequirements.AddPermission(AMM_ManifestPermission.BILLING);

            if (AN_Settings.Instance.Licensing) 
            {
                buildRequirements.AddPermission(AMM_ManifestPermission.CHECK_LICENSE);
            }


            buildRequirements.AddInternalLib("an_vending.aar");
        }

       
    }
}