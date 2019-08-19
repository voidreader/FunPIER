using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Android.GMS.Internal;

using SA.Foundation.Templates;

namespace SA.Android.GMS.Games
{
    [Serializable]
    public class AN_AchievementsClient : AN_LinkedObject
    {


        /// <summary>
        /// Unlocks an achievement for the currently signed in player. 
        /// If the achievement is hidden this will reveal it to the player.
        /// 
        /// This is the fire-and-forget form of the API.
        /// Use this form if you don't need to know the status of the operation immediately. 
        /// For most applications, this will be the preferred API to use, 
        /// though note that the update may not be sent to the server until the next sync. 
        /// Ask about RevealImmediate(String, int) if you need the operation to attempt to communicate to the server immediately 
        /// or need to have the status code delivered to your application.
        /// 
        /// Required Scopes: SCOPE_GAMES_LITE
        /// </summary>
        /// <param name="achievementId">The achievement ID to unlock.</param
        public void Unlock(string achievementId) {
            AN_GMS_Lib.Achievements.Unlock(this, achievementId);
        }


        /// <summary>
        /// Asynchronously unlocks an achievement for the currently signed in player. 
        /// If the achievement is hidden this will reveal it to the player.
        /// 
        /// This form of the API will attempt to update the user's achievement on the server immediately. 
        /// The callback will complete successfully when the server has been updated.
        /// </summary>
        /// <param name="achievementId">The achievement ID to unlock.</param>
        /// <param name="callback">Result callback</param>
        public void UnlockImmediate(string achievementId, Action<SA_Result> callback) {
            AN_GMS_Lib.Achievements.UnlockImmediate(this, achievementId, callback);
        }


        /// <summary>
        /// Increments an achievement by the given number of steps. 
        /// The achievement must be an incremental achievement. 
        /// Once an achievement reaches at least the maximum number of steps, it will be unlocked automatically. 
        /// Any further increments will be ignored.
        /// 
        /// This is the fire-and-forget form of the API.
        /// Use this form if you don't need to know the status of the operation immediately. 
        /// For most applications, this will be the preferred API to use, 
        /// though note that the update may not be sent to the server until the next sync. 
        /// Ask about incrementImmediate(String, int) if you need the operation to attempt to communicate to the server immediately 
        /// or need to have the status code delivered to your application.
        /// 
        /// Required Scopes: SCOPE_GAMES_LITE
        /// </summary>
        /// <param name="achievementId">The achievement ID to increment.</param>
        /// <param name="numSteps">The number of steps to increment by. Must be greater than 0.</param>
        public void Increment(string achievementId, int numSteps) {
            AN_GMS_Lib.Achievements.Increment(this, achievementId, numSteps);
        }

        /// <summary>
        /// Asynchronously increments an achievement by the given number of steps. 
        /// The achievement must be an incremental achievement. 
        /// Once an achievement reaches at least the maximum number of steps, it will be unlocked automatically. 
        /// Any further increments will be ignored.
        ///
        /// This form of the API will attempt to update the user's achievement on the server immediately. 
        /// The result successful response indicates whether the achievement is now unlocked.
        /// </summary>
        /// <param name="achievementId">The ID of the achievement to increment.</param>
        /// <param name="numSteps">The number of steps to increment by. Must be greater than 0.</param>
        /// <param name="callback">Result callback</param>
        public void IncrementImmediate(string achievementId, int numSteps, Action<AN_AchievementIncrementResult> callback) {
            AN_GMS_Lib.Achievements.IncrementImmediate(this, achievementId, numSteps, callback);
        }


        /// <summary>
        /// Reveals a hidden achievement to the currently signed-in player. 
        /// If the achievement has already been unlocked, this will have no effect.
        /// 
        /// This is the fire-and-forget form of the API.
        /// Use this form if you don't need to know the status of the operation immediately. 
        /// For most applications, this will be the preferred API to use, 
        /// though note that the update may not be sent to the server until the next sync. 
        /// Ask about RevealImmediate(String, int) if you need the operation to attempt to communicate to the server immediately 
        /// or need to have the status code delivered to your application.
        /// 
        /// Required Scopes: SCOPE_GAMES_LITE
        /// </summary>
        /// <param name="achievementId">The achievement ID to reveal.</param>
        public void Reveal(String achievementId) {
            AN_GMS_Lib.Achievements.Reveal(this, achievementId);
        }


        /// <summary>
        /// Asynchronously reveals a hidden achievement to the currently signed in player. 
        /// If the achievement is already visible, this will have no effect.
        /// 
        /// This form of the API will attempt to update the user's achievement on the server immediately. 
        /// If the Result callback is successful the server has been updated.
        /// </summary>
        /// <param name="achievementId">The achievement ID to reveal.</param>
        /// <param name="callback">Result callback</param>
        public void RevealImmediate(string achievementId, Action<SA_Result> callback) {
            AN_GMS_Lib.Achievements.RevealImmediate(this, achievementId, callback);
        }


        /// <summary>
        /// Returns a callback which asynchronously loads an <see cref="SA.Android.Content.AN_Intent"/> 
        /// to show the list of achievements for a game. 
        /// Note that the <see cref="SA.Android.Content.AN_Intent"/> returned from the callback must be invoked with startActivityForResult, 
        /// so that the identity of the calling package can be established.
        /// </summary>
        /// <param name="callback">Callback.</param>
        public void GetAchievementsIntent(Action<AN_IntentResult> callback) {
            AN_GMS_Lib.Achievements.GetAchievementsIntent(this, callback);
        }


        /// <summary>
        /// Asynchronously loads an <see cref="AN_AchievementsLoadResult"/> that represents the achievement data for the currently signed-in player.
        ///
        /// Required Scopes: SCOPE_GAMES_LITE
        /// </summary>
        /// <param name="forceReload">
        /// If <c>>true</c>, this call will clear any locally cached data and attempt to fetch the latest data from the server. 
        /// This would commonly be used for something like a user-initiated refresh. 
        /// Normally, this should be set to <c>false</c> to gain advantages of data caching.
        /// </param>
        /// <param name="callback">Load callback.</param>
        public void Load(bool forceReload, Action<AN_AchievementsLoadResult> callback) {
            AN_GMS_Lib.Achievements.Load(this, forceReload, callback);
        }
       
    }
}