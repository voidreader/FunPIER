namespace SA.Android
{
    public enum AN_BinaryDependency
    {
        AndroidX,
        PlayServicesAuth,
        PlayServicesGames,
        GSON,
        BillingClient
    }

    public static class AN_RemoteBinaryDependencyMethods
    {
        public static string GetLocalRepositoryName(this AN_BinaryDependency dependency) 
        {
            switch (dependency) 
            {
                case AN_BinaryDependency.AndroidX:
                    return "androidx-1.0.0-alpha3";
                case AN_BinaryDependency.PlayServicesAuth:
                    return "play-services-auth-17.0.0";
                case AN_BinaryDependency.PlayServicesGames:
                    return "play-services-games-18.0.0";
                case AN_BinaryDependency.GSON:
                    return "gson-2.8.5";
                case AN_BinaryDependency.BillingClient:
                    return "billingclient-2.0.1";
            }
            return string.Empty;
        }

        public static string GetRemoteRepositoryName(this AN_BinaryDependency dependency) 
        {
            switch (dependency) 
            {
                case AN_BinaryDependency.AndroidX:
                    return "me.panpf:androidx:1.0.0-alpha3";
                case AN_BinaryDependency.PlayServicesAuth:
                    return "com.google.android.gms:play-services-auth:17.0.0";
                case AN_BinaryDependency.PlayServicesGames:
                    return "com.google.android.gms:play-services-games:18.0.0";
                case AN_BinaryDependency.GSON:
                    return "com.google.code.gson:gson:2.8.5";
                case AN_BinaryDependency.BillingClient:
                    return "com.android.billingclient:billing:2.0.1";
            }
            return string.Empty;
        }
    }
}