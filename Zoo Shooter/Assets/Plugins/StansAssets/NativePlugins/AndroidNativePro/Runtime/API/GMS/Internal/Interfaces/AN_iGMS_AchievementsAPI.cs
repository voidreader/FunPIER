using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.GMS.Games;
using SA.Foundation.Templates;

namespace SA.Android.GMS.Internal
{
    internal interface AN_iGMS_AchievementsAPI
    {
        void Unlock(AN_AchievementsClient client, string achievementId);
        void UnlockImmediate(AN_AchievementsClient client, String achievementId, Action<SA_Result> callback);

        void Increment(AN_AchievementsClient client, string achievementId, int step);
        void IncrementImmediate(AN_AchievementsClient client, String achievementId, int step, Action<AN_AchievementIncrementResult> callback);

        void Reveal(AN_AchievementsClient client, String achievementId);
        void RevealImmediate(AN_AchievementsClient client, String achievementId, Action<SA_Result> callback);

        void GetAchievementsIntent(AN_AchievementsClient client, Action<AN_IntentResult> callback);
        void Load(AN_AchievementsClient client, bool forceReload, Action<AN_AchievementsLoadResult> callback);
    }
}