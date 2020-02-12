namespace SA.Android
{
    public class AN_FirebaseResolver : AN_APIResolver
    {
        public override bool IsSettingsEnabled 
        {
            get
            {
                return AN_FirebaseDefinesResolver.IsAnalyticsSDKInstalled ||
                       AN_FirebaseDefinesResolver.IsMessagingSDKInstalled;
            }
            set {  }
        }

        protected override void AppendBuildRequirements(AN_AndroidBuildRequirements buildRequirements) { }
    }
}