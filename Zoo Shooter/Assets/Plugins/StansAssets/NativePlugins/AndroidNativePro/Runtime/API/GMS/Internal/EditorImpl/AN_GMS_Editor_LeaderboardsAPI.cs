using System;
using System.Collections;
using System.Collections.Generic;
using SA.Foundation.Templates;
using UnityEngine;

using SA.Android.GMS.Games;
using SA.Android.GMS.Common;
using SA.Android.Utilities;
using SA.Android.Content;

using SA.Foundation.Async;
using SA.Foundation.Time;


namespace SA.Android.GMS.Internal
{

    internal class AN_GMS_Editor_LeaderboardsAPI : AN_iGMS_LeaderboardsAPI
    {


    


        //--------------------------------------
        // AN_LeaderboardsClient
        //--------------------------------------

       


        public void GetAllLeaderboardsIntent(AN_LeaderboardsClient client, Action<AN_IntentResult> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                callback.Invoke(new AN_IntentResult(new AN_Intent()));
            });
        }

        public void GetLeaderboardIntent(AN_LeaderboardsClient client, string leaderboardId, int timeSpan, int collection, Action<AN_IntentResult> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                callback.Invoke(new AN_IntentResult(new AN_Intent()));
            });
        }

        public void LoadCurrentPlayerLeaderboardScore(AN_LeaderboardsClient client, string leaderboardId, int span, int leaderboardCollection, Action<AN_LinkedObjectResult<AN_LeaderboardScore>> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                var result = new AN_LinkedObjectResult<AN_LeaderboardScore>(new AN_LeaderboardScore());
                callback.Invoke(result);
            });
        }


        public void LoadLeaderboardMetadata(AN_LeaderboardsClient client, bool forceReload, Action<AN_LeaderboardsLoadResult> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                var result = new AN_LeaderboardsLoadResult();
                callback.Invoke(result);
            });
        }

        public void LoadLeaderboardMetadata(AN_LeaderboardsClient client, string leaderboardId, bool forceReload, Action<AN_LeaderboardLoadResult> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                var result = new AN_LeaderboardLoadResult();
                callback.Invoke(result);
            });
        }



        public void LoadMoreScores(AN_LeaderboardsClient client, AN_LeaderboardScoreBuffer buffer, int maxResults, int pageDirection, Action<AN_LinkedObjectResult<AN_LeaderboardScores>> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                var result = new AN_LinkedObjectResult<AN_LeaderboardScores>(new AN_LeaderboardScores());
                callback.Invoke(result);
            });
        }

        public void LoadPlayerCenteredScores(AN_LeaderboardsClient client, string leaderboardId, int span, int leaderboardCollection, int maxResults, bool forceReload, Action<AN_LinkedObjectResult<AN_LeaderboardScores>> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                var result = new AN_LinkedObjectResult<AN_LeaderboardScores>(new AN_LeaderboardScores());
                callback.Invoke(result);
            });
        }

        public void LoadTopScores(AN_LeaderboardsClient client, string leaderboardId, int span, int leaderboardCollection, int maxResults, bool forceReload, Action<AN_LinkedObjectResult<AN_LeaderboardScores>> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                var result = new AN_LinkedObjectResult<AN_LeaderboardScores>(new AN_LeaderboardScores());
                callback.Invoke(result);
            });
        }





        public void SubmitScore(AN_LeaderboardsClient client, string leaderboardId, long score, string scoreTag) {
            
        }

        public void SubmitScoreImmediate(AN_LeaderboardsClient client, string leaderboardId, long score, string scoreTag, Action<AN_LinkedObjectResult<AN_ScoreSubmissionData>> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                var result = new AN_LinkedObjectResult<AN_ScoreSubmissionData>(new AN_ScoreSubmissionData());
                callback.Invoke(result);
            });
        }


        //--------------------------------------
        // AN_LeaderboardScores
        //--------------------------------------

       

        public AN_Leaderboard LeaderboardScores_GetLeaderboard(AN_LeaderboardScores scores) {
            return new AN_Leaderboard();
        }

        public AN_LeaderboardScoreBuffer LeaderboardScores_GetScores(AN_LeaderboardScores scores)  {
            return new AN_LeaderboardScoreBuffer();
        }


        //--------------------------------------
        // AN_LeaderboardScore
        //--------------------------------------


        public string LeaderboardScore_GetDisplayRank(AN_LeaderboardScore score) {
            return "Top 10";
        }

        public AN_Player LeaderboardScore_GetScoreHolder(AN_LeaderboardScore score) {
            return new AN_Player();
        }

        public string LeaderboardScore_GetDisplayScore(AN_LeaderboardScore score) {
            return "Socre: 100";
        }

        public long LeaderboardScore_GetRawScore(AN_LeaderboardScore score) {
            return 100;
        }

        public long LeaderboardScore_GetRank(AN_LeaderboardScore score) {
            return 10;
        }
    

        public string LeaderboardScore_GetScoreHolderDisplayName(AN_LeaderboardScore score) {
            return "Score Holder Display Name";
        }

        public string LeaderboardScore_GetScoreHolderIconImageUri(AN_LeaderboardScore score) {
            return string.Empty;
        }

        public string LeaderboardScore_GetScoreHolderHiResImageUri(AN_LeaderboardScore score) {
            return string.Empty;
        }

        public string LeaderboardScore_GetScoreTag(AN_LeaderboardScore score) {
            return string.Empty;
        }

        public long LeaderboardScore_GetTimestampMillis(AN_LeaderboardScore score) {
            return SA_Unix_Time.ToUnixTime(DateTime.Now);
        }



        //--------------------------------------
        // AN_LeaderboardScoreBuffer
        //--------------------------------------


        public AN_LeaderboardScoreBuffer.ScoresList LeaderboardScoreBuffer_GetScores(AN_LeaderboardScoreBuffer buffer) {
            return new AN_LeaderboardScoreBuffer.ScoresList();
        }


        //--------------------------------------
        // AN_ScoreSubmissionData
        //--------------------------------------

        public string ScoreSubmissionData_GetLeaderboardId(AN_ScoreSubmissionData data) {
            return "leaderboard.id";
        }

        public string ScoreSubmissionData_GetPlayerId(AN_ScoreSubmissionData data) {
            return "player.id";
        }

        public AN_ScoreSubmissionData.Result ScoreSubmissionData_GetScoreResult(AN_ScoreSubmissionData data, int timeSpan) {
            return new AN_ScoreSubmissionData.Result();
        }


        //--------------------------------------
        // AN_ScoreSubmissionDataResult
        //--------------------------------------

        public string ScoreSubmissionDataResult_GetFormattedScore(AN_ScoreSubmissionData.Result data) {
            return "Socre: 100";
        }

        public bool ScoreSubmissionDataResult_GetNewBest(AN_ScoreSubmissionData.Result data) {
            return true;
        }

        public long ScoreSubmissionDataResult_GetRawScore(AN_ScoreSubmissionData.Result data) {
            return 100;
        }

        public string ScoreSubmissionDataResult_GetScoreTag(AN_ScoreSubmissionData.Result data) {
            return string.Empty;
        }
    }

}