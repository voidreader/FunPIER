using System;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Utility;
using SA.Foundation.Editor;
using SA.Foundation.UtilitiesEditor;

using SA.Android.Utilities;


namespace SA.Android
{
    public enum AN_BinaryDependency
    {
        SupportV4CoreUtils,
        PlayServicesAuth,
     //   PlayServicesAuthBase,
        PlayServicesGames,
        GSON
    }

    public static class AN_RemoteBinaryDependencyMethods
    {

        public static string GetLocalRepositoryName(this AN_BinaryDependency dependency) {
            switch (dependency) {
                case AN_BinaryDependency.SupportV4CoreUtils:
                    return "support-v427.1.1";
                case AN_BinaryDependency.PlayServicesAuth:
                    return "play-services-auth-16.0.1";
                    /*
                case AN_BinaryDependency.PlayServicesAuthBase:
                    return "play-services-auth-16.0.1";*/
                case AN_BinaryDependency.PlayServicesGames:
                    return "play-services-games-16.0.1";
                case AN_BinaryDependency.GSON:
                    return "gson-2.8.5";

            }

            return string.Empty;
        }

        public static string GetRemoteRepositoryName(this AN_BinaryDependency dependency) {
            switch (dependency) {
                case AN_BinaryDependency.SupportV4CoreUtils:
                    return "com.android.support:support-v4:27.1.1";
                case AN_BinaryDependency.PlayServicesAuth:
                    return "com.google.android.gms:play-services-auth:16.0.1";
                /*case AN_BinaryDependency.PlayServicesAuthBase:
                    return "com.google.android.gms:play-services-auth-base:16.0.0";*/
                case AN_BinaryDependency.PlayServicesGames:
                    return "com.google.android.gms:play-services-games:16.0.0";
                case AN_BinaryDependency.GSON:
                    return "com.google.code.gson:gson:2.8.5";
            }

            return string.Empty;
        }


    }

}