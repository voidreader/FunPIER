namespace SA.Android.Editor
{
    public class AN_FirebaseResolver : AN_APIResolver
    {
        public override bool IsSettingsEnabled {

            get => AN_Packages.IsAnalyticsSdkInstalled || AN_Packages.IsMessagingSdkInstalled;
            set {  }
        }

        protected override void AppendBuildRequirements(AN_AndroidBuildRequirements buildRequirements) {
           
        }
    }
}
