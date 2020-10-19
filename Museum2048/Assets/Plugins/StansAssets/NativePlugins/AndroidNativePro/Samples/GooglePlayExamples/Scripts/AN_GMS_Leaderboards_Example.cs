using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SA.Android.GMS.Games;
using SA.Android.App;

public class AN_GMS_Leaderboards_Example : MonoBehaviour
{
    // private string leaderboardsId = "CgkI0Y2kiIAVEAIQBw";
    readonly string leaderboardsId = "CgkI0Y2kiIAVEAIQCA";

    [SerializeField]
    Button m_nativeUI = null;
    [SerializeField]
    Button m_loadLeaberboards = null;
    [SerializeField]
    Button m_loadOneLeaberboard = null;
    [SerializeField]
    Button m_sbmit = null;
    [SerializeField]
    Button m_sbmitNoCB = null;
    [SerializeField]
    Button m_loadScores = null;

    void Start()
    {
        m_loadScores.onClick.AddListener(() =>
        {
            var leaderboards = AN_Games.GetLeaderboardsClient();
            var maxResults = 20;
            leaderboards.LoadTopScores(leaderboardsId, maxResults, (result) =>
            {
                if (result.IsSucceeded)
                {
                    var scores = result.Data;
                    var buffer = scores.Scores;
                    Debug.Log($"scores.Leaderboard.DisplayName: {scores.Leaderboard.DisplayName}");
                    Debug.Log($"Loaded scores Count: {buffer.Scores.Count}");
                    foreach (var score in buffer.Scores)
                    {
                        Debug.Log($"score.DisplayRank: {score.DisplayRank}");
                        Debug.Log($"score.DisplayScore: {score.DisplayScore}");
                        Debug.Log($"score.Rank: {score.Rank}");
                        Debug.Log($"score.RawScore: {score.RawScore}");
                        Debug.Log($"score.ScoreHolder: {score.ScoreHolder}");
                        Debug.Log($"score.ScoreHolderDisplayName: {score.ScoreHolderDisplayName}");
                        Debug.Log($"score.ScoreHolderIconImageUri: {score.ScoreHolderIconImageUri}");
                        Debug.Log($"score.ScoreHolderHiResImageUri: {score.ScoreHolderHiResImageUri}");
                        Debug.Log($"score.ScoreTag: {score.ScoreTag}");
                        Debug.Log($"score.TimestampMillis: {score.TimestampMillis}");
                        Debug.Log("------------------------------------------------");
                    }
                }
                else
                {
                    Debug.Log($"Failed to Load Top Scores {result.Error.FullMessage}");
                }
            });
        });
        m_sbmitNoCB.onClick.AddListener(() =>
        {
            var leaderboards = AN_Games.GetLeaderboardsClient();
            leaderboards.SubmitScore(leaderboardsId, 250);
        });
        m_sbmit.onClick.AddListener(() =>
        {
            var leaderboards = AN_Games.GetLeaderboardsClient();
            leaderboards.SubmitScoreImmediate(leaderboardsId, 200, "Tag", (result) =>
            {
                if (result.IsSucceeded)
                {
                    var scoreSubmissionData = result.Data;
                    Debug.Log("SubmitScoreImmediate completed");
                    Debug.Log($"scoreSubmissionData.PlayerId: {scoreSubmissionData.PlayerId}");
                    Debug.Log($"scoreSubmissionData.PlayerId: {scoreSubmissionData.LeaderboardId}");
                    foreach (var span in (AN_Leaderboard.TimeSpan[])System.Enum.GetValues(typeof(AN_Leaderboard.TimeSpan)))
                    {
                        var scoreSubmissionResult = scoreSubmissionData.GetScoreResult(span);
                        Debug.Log($"scoreSubmissionData.FormattedScore: {scoreSubmissionResult.FormattedScore}");
                        Debug.Log($"scoreSubmissionData.NewBest: {scoreSubmissionResult.NewBest}");
                        Debug.Log($"scoreSubmissionData.RawScore: {scoreSubmissionResult.RawScore}");
                        Debug.Log($"scoreSubmissionData.ScoreTag: {scoreSubmissionResult.ScoreTag}");
                    }
                }
                else
                {
                    Debug.Log($"Failed to Submit Score Immediate {result.Error.FullMessage}");
                }
            });
        });
        m_nativeUI.onClick.AddListener(() =>
        {
            var leaderboards = AN_Games.GetLeaderboardsClient();
            leaderboards.GetAllLeaderboardsIntent((result) =>
            {
                if (result.IsSucceeded)
                {
                    var intent = result.Intent;
                    var proxy = new AN_ProxyActivity();
                    proxy.StartActivityForResult(intent, (intentResult) =>
                    {
                        proxy.Finish();
                        //TODO you might want to check is user had sigend out with that UI
                    });
                }
                else
                {
                    Debug.Log($"Failed to Get leaderboards Intent {result.Error.FullMessage}");
                }
            });
        });
        m_loadOneLeaberboard.onClick.AddListener(() =>
        {
            var leaderboards = AN_Games.GetLeaderboardsClient();
            leaderboards.LoadLeaderboardMetadata(leaderboardsId, true, (result) =>
            {
                if (result.IsSucceeded)
                    PrintLeaderboardsInfo(new List<AN_Leaderboard> { result.Leaderboard });
                else
                    Debug.LogError($"Load Leaderboard Failed: {result.Error.FullMessage}");
            });
        });
        m_loadLeaberboards.onClick.AddListener(() =>
        {
            var leaderboards = AN_Games.GetLeaderboardsClient();
            leaderboards.LoadLeaderboardMetadata(false, (result) =>
            {
                if (result.IsSucceeded)
                {
                    Debug.Log($"Load Leaderboards Metadata Succeeded, count: {result.Leaderboards.Count}");
                    PrintLeaderboardsInfo(result.Leaderboards);
                }
                else
                {
                    Debug.Log($"Load Leaderboards Failed: {result.Error.FullMessage}");
                }
            });
        });
    }

    void PrintLeaderboardsInfo(List<AN_Leaderboard> leaderboards)
    {
        foreach (var leaderboard in leaderboards)
        {
            Debug.Log("------------------------------------------------");
            Debug.Log($"leaderboard.LeaderboardId: {leaderboard.LeaderboardId}");
            Debug.Log($"leaderboard.Description: {leaderboard.DisplayName}");
            Debug.Log($"leaderboard.Name: {leaderboard.IconImageUri}");
            Debug.Log($"leaderboard.UnlockedImageUri: {leaderboard.LeaderboardScoreOrder}");
            Debug.Log($"leaderboard.Variants.Count: {leaderboard.Variants.Count}");
            foreach (var variant in leaderboard.Variants)
            {
                Debug.Log("***************************");
                Debug.Log($"variant.Collection: {variant.Collection}");
                Debug.Log($"variant.DisplayPlayerRank: {variant.DisplayPlayerRank}");
                Debug.Log($"variant.DisplayPlayerScore: {variant.DisplayPlayerScore}");
                Debug.Log($"variant.NumScores: {variant.NumScores}");
                Debug.Log($"variant.PlayerRank: {variant.PlayerRank}");
                Debug.Log($"variant.PlayerScoreTag: {variant.PlayerScoreTag}");
                Debug.Log($"variant.RawPlayerScore: {variant.RawPlayerScore}");
                Debug.Log($"variant.TimeSpan: {variant.TimeSpan}");
                Debug.Log($"variant.HasPlayerInfo: {variant.HasPlayerInfo}");
            }
        }

        Debug.Log("------------------------------------------------");
    }
}
