using System;
using System.Collections;
using System.Collections.Generic;
using SA.Foundation.Templates;
using UnityEngine;

using SA.Android.GMS.Games;
using SA.Android.Utilities;

namespace SA.Android.GMS.Internal
{

    internal class AN_GMS_Native_AchievementAPI : AN_iGMS_AchievementsAPI
    {

        const string AN_AchievementsClient = "com.stansassets.gms.games.achievements.AN_AchievementsClient";


        public void Unlock(AN_AchievementsClient client, String achievementId) {
            AN_Java.Bridge.CallStatic(AN_AchievementsClient, "Unlock", client.HashCode, achievementId);
        }

        public void UnlockImmediate(AN_AchievementsClient client, String achievementId, Action<SA_Result> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_AchievementsClient, "UnlockImmediate", callback, client.HashCode, achievementId);
        }

        public void Increment(AN_AchievementsClient client, String achievementId, int step) {
            AN_Java.Bridge.CallStatic(AN_AchievementsClient, "Increment", client.HashCode, achievementId, step);
        }

        public void IncrementImmediate(AN_AchievementsClient client, String achievementId, int step, Action<AN_AchievementIncrementResult> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_AchievementsClient, "IncrementImmediate", callback,  client.HashCode, achievementId, step);
        }

        public void Reveal(AN_AchievementsClient client, String achievementId) {
            AN_Java.Bridge.CallStatic(AN_AchievementsClient, "Reveal", client.HashCode, achievementId);
        }

        public void RevealImmediate(AN_AchievementsClient client, String achievementId, Action<SA_Result> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_AchievementsClient, "RevealImmediate", callback, client.HashCode, achievementId);
        }


        public void GetAchievementsIntent(AN_AchievementsClient client, Action<AN_IntentResult> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_AchievementsClient, "GetAchievementsIntent", callback, client.HashCode);
        }

        public void Load(AN_AchievementsClient client, bool forceReload, Action<AN_AchievementsLoadResult> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_AchievementsClient, "Load", callback, client.HashCode, forceReload);
        }

    }
}