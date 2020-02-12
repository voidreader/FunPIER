using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Android.GMS.Common
{
    /// <summary>
    /// OAuth 2.0 scopes for use with Google Play services. 
    /// See the specific client methods for details on which scopes are required.
    /// </summary>
    public static class AN_Scopes {

        public static string PROFILE = "profile";
        public static string EMAIL = "email";
        public static string OPEN_ID = "openid";
        public static string LEGACY_USERINFO_PROFILE = "https://www.googleapis.com/auth/userinfo.profile";
        public static string LEGACY_USERINFO_EMAIL = "https://www.googleapis.com/auth/userinfo.email";
   
        public static string PLUS_ME = "https://www.googleapis.com/auth/plus.me";
        public static string GAMES = "https://www.googleapis.com/auth/games";
        public static string GAMES_LITE = "https://www.googleapis.com/auth/games_lite";
        public static string CLOUD_SAVE = "https://www.googleapis.com/auth/datastoremobile";
        public static string APP_STATE = "https://www.googleapis.com/auth/appstate";
        public static string DRIVE_FILE = "https://www.googleapis.com/auth/drive.file";
        public static string DRIVE_APPFOLDER = "https://www.googleapis.com/auth/drive.appdata";
        public static string DRIVE_FULL = "https://www.googleapis.com/auth/drive";
        public static string DRIVE_APPS = "https://www.googleapis.com/auth/drive.apps";
        public static string CONNECTIONS_READ = "https://www.googleapis.com/auth/connections.read";
        public static string FITNESS_ACTIVITY_READ = "https://www.googleapis.com/auth/fitness.activity.read";
        public static string FITNESS_ACTIVITY_READ_WRITE = "https://www.googleapis.com/auth/fitness.activity.write";
        public static string FITNESS_LOCATION_READ = "https://www.googleapis.com/auth/fitness.location.read";
        public static string FITNESS_LOCATION_READ_WRITE = "https://www.googleapis.com/auth/fitness.location.write";
        public static string FITNESS_BODY_READ = "https://www.googleapis.com/auth/fitness.body.read";
        public static string FITNESS_BODY_READ_WRITE = "https://www.googleapis.com/auth/fitness.body.write";
        public static string FITNESS_NUTRITION_READ = "https://www.googleapis.com/auth/fitness.nutrition.read";
        public static string FITNESS_NUTRITION_READ_WRITE = "https://www.googleapis.com/auth/fitness.nutrition.write";
        public static string FITNESS_BLOOD_PRESSURE_READ = "https://www.googleapis.com/auth/fitness.blood_pressure.read";
        public static string FITNESS_BLOOD_PRESSURE_READ_WRITE = "https://www.googleapis.com/auth/fitness.blood_pressure.write";
        public static string FITNESS_BLOOD_GLUCOSE_READ = "https://www.googleapis.com/auth/fitness.blood_glucose.read";
        public static string FITNESS_BLOOD_GLUCOSE_READ_WRITE = "https://www.googleapis.com/auth/fitness.blood_glucose.write";
        public static string FITNESS_OXYGEN_SATURATION_READ = "https://www.googleapis.com/auth/fitness.oxygen_saturation.read";
        public static string FITNESS_OXYGEN_SATURATION_READ_WRITE = "https://www.googleapis.com/auth/fitness.oxygen_saturation.write";
        public static string FITNESS_BODY_TEMPERATURE_READ = "https://www.googleapis.com/auth/fitness.body_temperature.read";
        public static string FITNESS_BODY_TEMPERATURE_READ_WRITE = "https://www.googleapis.com/auth/fitness.body_temperature.write";
        public static string FITNESS_REPRODUCTIVE_HEALTH_READ = "https://www.googleapis.com/auth/fitness.reproductive_health.read";
        public static string FITNESS_REPRODUCTIVE_HEALTH_READ_WRITE = "https://www.googleapis.com/auth/fitness.reproductive_health.write";
        public static string DISPLAY_ADS = "https://www.googleapis.com/auth/display_ads";
        public static string YOUTUBE_DATA_API = "https://www.googleapis.com/auth/youtube";
    }
}