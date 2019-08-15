using SA.Android.Manifest;

namespace SA.Android
{
    public class AN_GooglePlayResolver : AN_APIResolver
    {
        public override bool IsSettingsEnabled 
        {
            get { return AN_Settings.Instance.GooglePlay; }
            set { AN_Settings.Instance.GooglePlay = value; }
        }

        public override void AppendBuildRequirements(AN_AndroidBuildRequirements buildRequirements) {

            buildRequirements.AddBinaryDependency(AN_BinaryDependency.PlayServicesAuth);
            buildRequirements.AddBinaryDependency(AN_BinaryDependency.AndroidX);
            buildRequirements.AddInternalLib("an_gms.aar");

            var games_APP_ID = new AMM_PropertyTemplate("meta-data");
            games_APP_ID.SetValue("android:name", "com.google.android.gms.games.APP_ID");
            games_APP_ID.SetValue("android:value", "@string/app_id");
            buildRequirements.AddApplicationProperty(games_APP_ID);


            if (AN_Settings.Instance.GooglePlayGamesAPI) 
            {
                buildRequirements.AddBinaryDependency(AN_BinaryDependency.PlayServicesGames);
            }
        }
    }
}