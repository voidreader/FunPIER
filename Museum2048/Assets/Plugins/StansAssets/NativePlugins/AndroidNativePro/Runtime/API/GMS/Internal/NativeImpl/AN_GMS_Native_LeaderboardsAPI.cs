using System;
using System.Collections;
using System.Collections.Generic;
using SA.Foundation.Templates;
using UnityEngine;

using SA.Android.GMS.Games;
using SA.Android.GMS.Common;
using SA.Android.Utilities;



namespace SA.Android.GMS.Internal
{

    internal class AN_GMS_Native_LeaderboardsAPI : AN_iGMS_LeaderboardsAPI
    {


        const string JAVA_PACKAGE = "com.stansassets.gms.games.leaderboards.";


        //--------------------------------------
        // AN_LeaderboardsClient
        //--------------------------------------

        const string AN_LeaderboardsClient = JAVA_PACKAGE + "AN_LeaderboardsClient";


        public void GetAllLeaderboardsIntent(AN_LeaderboardsClient client, Action<AN_IntentResult> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_LeaderboardsClient, "GetAllLeaderboardsIntent", callback, client.HashCode);
        }

        public void GetLeaderboardIntent(AN_LeaderboardsClient client, string leaderboardId, int timeSpan, int collection, Action<AN_IntentResult> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_LeaderboardsClient, "GetLeaderboardIntent", callback, client.HashCode, leaderboardId, timeSpan, collection);
        }

        public void LoadCurrentPlayerLeaderboardScore(AN_LeaderboardsClient client, string leaderboardId, int span, int leaderboardCollection, Action<AN_LinkedObjectResult<AN_LeaderboardScore>> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_LeaderboardsClient, "LoadCurrentPlayerLeaderboardScore", callback, client.HashCode, leaderboardId, span, leaderboardCollection);
        }


        public void LoadLeaderboardMetadata(AN_LeaderboardsClient client, bool forceReload, Action<AN_LeaderboardsLoadResult> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_LeaderboardsClient, "LoadLeaderboardMetadata", callback, client.HashCode, forceReload);
        }

        public void LoadLeaderboardMetadata(AN_LeaderboardsClient client, string leaderboardId, bool forceReload, Action<AN_LeaderboardLoadResult> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_LeaderboardsClient, "LoadLeaderboardMetadata", callback, client.HashCode, leaderboardId, forceReload);
        }



        public void LoadMoreScores(AN_LeaderboardsClient client, AN_LeaderboardScoreBuffer buffer, int maxResults, int pageDirection, Action<AN_LinkedObjectResult<AN_LeaderboardScores>> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_LeaderboardsClient, "LoadMoreScores", callback, client.HashCode, buffer.HashCode, maxResults, pageDirection);
        }

        public void LoadPlayerCenteredScores(AN_LeaderboardsClient client, string leaderboardId, int span, int leaderboardCollection, int maxResults, bool forceReload, Action<AN_LinkedObjectResult<AN_LeaderboardScores>> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_LeaderboardsClient, "LoadPlayerCenteredScores", callback, client.HashCode, leaderboardId, span, leaderboardCollection,  maxResults, forceReload);
        }

        public void LoadTopScores(AN_LeaderboardsClient client, string leaderboardId, int span, int leaderboardCollection, int maxResults, bool forceReload, Action<AN_LinkedObjectResult<AN_LeaderboardScores>> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_LeaderboardsClient, "LoadTopScores", callback, client.HashCode, leaderboardId, span, leaderboardCollection, maxResults, forceReload);
        }





        public void SubmitScore(AN_LeaderboardsClient client, string leaderboardId, long score, string scoreTag) {
            AN_Java.Bridge.CallStatic(AN_LeaderboardsClient, "SubmitScore", client.HashCode, leaderboardId, score, scoreTag);
        }

        public void SubmitScoreImmediate(AN_LeaderboardsClient client, string leaderboardId, long score, string scoreTag, Action<AN_LinkedObjectResult<AN_ScoreSubmissionData>> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_LeaderboardsClient, "SubmitScoreImmediate", callback, client.HashCode, leaderboardId, score, scoreTag);
        }


        //--------------------------------------
        // AN_LeaderboardScores
        //--------------------------------------

        const string AN_LeaderboardScores = JAVA_PACKAGE + "AN_LeaderboardScores";

        public AN_Leaderboard LeaderboardScores_GetLeaderboard(AN_LeaderboardScores scores) {
            var json = AN_Java.Bridge.CallStatic<string>(AN_LeaderboardScores, "GetLeaderboard", scores.HashCode);
            if(string.IsNullOrEmpty(json)) {
                return null;
            } else {
                return JsonUtility.FromJson<AN_Leaderboard>(json);
            }
        }

        public AN_LeaderboardScoreBuffer LeaderboardScores_GetScores(AN_LeaderboardScores scores)  {
            var json = AN_Java.Bridge.CallStatic<string>(AN_LeaderboardScores, "GetScores", scores.HashCode);
            return JsonUtility.FromJson<AN_LeaderboardScoreBuffer>(json);
        }


        //--------------------------------------
        // AN_LeaderboardScore
        //--------------------------------------

        const string AN_LeaderboardScore = JAVA_PACKAGE + "AN_LeaderboardScore";


        public string LeaderboardScore_GetDisplayRank(AN_LeaderboardScore score) {
            return AN_Java.Bridge.CallStatic<string>(AN_LeaderboardScore, "GetDisplayRank", score.HashCode);
        }

        public AN_Player LeaderboardScore_GetScoreHolder(AN_LeaderboardScore score) {
            var json = AN_Java.Bridge.CallStatic<string>(AN_LeaderboardScore, "GetScoreHolder", score.HashCode);
            return JsonUtility.FromJson<AN_Player>(json);
        }

        public string LeaderboardScore_GetDisplayScore(AN_LeaderboardScore score) {
            return AN_Java.Bridge.CallStatic<string>(AN_LeaderboardScore, "GetDisplayScore", score.HashCode);
        }

        public long LeaderboardScore_GetRawScore(AN_LeaderboardScore score) {
            return AN_Java.Bridge.CallStatic<long>(AN_LeaderboardScore, "GetRawScore", score.HashCode);
        }

        public long LeaderboardScore_GetRank(AN_LeaderboardScore score) {
            return AN_Java.Bridge.CallStatic<long>(AN_LeaderboardScore, "GetRank", score.HashCode);
        }
    

        public string LeaderboardScore_GetScoreHolderDisplayName(AN_LeaderboardScore score) {
            return AN_Java.Bridge.CallStatic<string>(AN_LeaderboardScore, "GetScoreHolderDisplayName", score.HashCode);
        }

        public string LeaderboardScore_GetScoreHolderIconImageUri(AN_LeaderboardScore score) {
            return AN_Java.Bridge.CallStatic<string>(AN_LeaderboardScore, "GetScoreHolderIconImageUri", score.HashCode);
        }

        public string LeaderboardScore_GetScoreHolderHiResImageUri(AN_LeaderboardScore score) {
            return AN_Java.Bridge.CallStatic<string>(AN_LeaderboardScore, "GetScoreHolderHiResImageUri", score.HashCode);
        }

        public string LeaderboardScore_GetScoreTag(AN_LeaderboardScore score) {
            return AN_Java.Bridge.CallStatic<string>(AN_LeaderboardScore, "GetScoreTag", score.HashCode);
        }

        public long LeaderboardScore_GetTimestampMillis(AN_LeaderboardScore score) {
            return AN_Java.Bridge.CallStatic<long>(AN_LeaderboardScore, "GetTimestampMillis", score.HashCode);
        }



        //--------------------------------------
        // AN_LeaderboardScoreBuffer
        //--------------------------------------


        const string AN_LeaderboardScoreBuffer = JAVA_PACKAGE + "AN_LeaderboardScoreBuffer";

        public AN_LeaderboardScoreBuffer.ScoresList LeaderboardScoreBuffer_GetScores(AN_LeaderboardScoreBuffer buffer) {
            var json = AN_Java.Bridge.CallStatic<string>(AN_LeaderboardScoreBuffer, "GetScores", buffer.HashCode);
            return JsonUtility.FromJson<AN_LeaderboardScoreBuffer.ScoresList>(json);
        }


        //--------------------------------------
        // AN_ScoreSubmissionData
        //--------------------------------------

        const string AN_ScoreSubmissionData = JAVA_PACKAGE + "AN_ScoreSubmissionData";

        public string ScoreSubmissionData_GetLeaderboardId(AN_ScoreSubmissionData data) {
            return AN_Java.Bridge.CallStatic<string>(AN_ScoreSubmissionData, "GetLeaderboardId", data.HashCode);
        }

        public string ScoreSubmissionData_GetPlayerId(AN_ScoreSubmissionData data) {
            return AN_Java.Bridge.CallStatic<string>(AN_ScoreSubmissionData, "GetPlayerId", data.HashCode);
        }

        public AN_ScoreSubmissionData.Result ScoreSubmissionData_GetScoreResult(AN_ScoreSubmissionData data, int timeSpan) {
            var json = AN_Java.Bridge.CallStatic<string>(AN_ScoreSubmissionData, "GetScoreResult", data.HashCode, timeSpan);
            if(string.IsNullOrEmpty(json)) {
                return null;
            }
            return JsonUtility.FromJson<AN_ScoreSubmissionData.Result>(json);
        }
        


        //--------------------------------------
        // AN_ScoreSubmissionDataResult
        //--------------------------------------

        const string AN_ScoreSubmissionDataResult = JAVA_PACKAGE + "AN_ScoreSubmissionDataResult";

        public string ScoreSubmissionDataResult_GetFormattedScore(AN_ScoreSubmissionData.Result data) {
            return AN_Java.Bridge.CallStatic<string>(AN_ScoreSubmissionDataResult, "GetFormattedScore", data.HashCode);
        }

        public bool ScoreSubmissionDataResult_GetNewBest(AN_ScoreSubmissionData.Result data) {
            return AN_Java.Bridge.CallStatic<bool>(AN_ScoreSubmissionDataResult, "GetNewBest", data.HashCode);
        }

        public long ScoreSubmissionDataResult_GetRawScore(AN_ScoreSubmissionData.Result data) {
            return AN_Java.Bridge.CallStatic<long>(AN_ScoreSubmissionDataResult, "GetRawScore", data.HashCode);
        }

        public string ScoreSubmissionDataResult_GetScoreTag(AN_ScoreSubmissionData.Result data) {
            return AN_Java.Bridge.CallStatic<string>(AN_ScoreSubmissionDataResult, "GetScoreTag", data.HashCode);
        }


    }

}