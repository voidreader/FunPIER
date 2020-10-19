namespace SA.Android.Editor
{
    public enum AN_BinaryDependency
    {
        AndroidX,
        PlayServicesAuth,
        PlayServicesGames,
        GSON,
        BillingClient
    }

    static class AN_RemoteBinaryDependencyMethods
    {
        public static string GetRemoteRepositoryName(this AN_BinaryDependency dependency)
        {
            switch (dependency)
            {
                case AN_BinaryDependency.AndroidX:
                    return "androidx.appcompat:appcompat:1.1.0";
                case AN_BinaryDependency.PlayServicesAuth:
                    return "com.google.android.gms:play-services-auth:17.0.0";
                case AN_BinaryDependency.PlayServicesGames:
                    return "com.google.android.gms:play-services-games:18.0.1";
                case AN_BinaryDependency.GSON:
                    return "com.google.code.gson:gson:2.8.5";
                case AN_BinaryDependency.BillingClient:
                    return "com.android.billingclient:billing:2.2.1";
            }

            return string.Empty;
        }
    }
}
