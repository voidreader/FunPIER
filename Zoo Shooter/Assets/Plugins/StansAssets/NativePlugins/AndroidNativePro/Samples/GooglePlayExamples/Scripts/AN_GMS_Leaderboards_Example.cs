using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SA.Android.GMS.Auth;
using SA.Android.GMS.Common;
using SA.Android.GMS.Games;
using SA.Android.App;
using SA.Android.Utilities;

public class AN_GMS_Leaderboards_Example : MonoBehaviour {

    
   // private string leaderboardsId = "CgkI0Y2kiIAVEAIQBw";
    private string leaderboardsId = "CgkI0Y2kiIAVEAIQCA";

    
    [SerializeField] Button m_nativeUI = null;
    [SerializeField] Button m_loadLeaberboards = null;
    [SerializeField] Button m_loadOneLeaberboard = null;

    [SerializeField] Button m_sbmit = null;
    [SerializeField] Button m_sbmitNoCB = null;

    [SerializeField] Button m_loadScores = null;


    private void Start() {


        m_loadScores.onClick.AddListener(() => {
            var leaderboards = AN_Games.GetLeaderboardsClient();
            int maxResults = 20;
            leaderboards.LoadTopScores(leaderboardsId, maxResults, (result) => {

                if(result.IsSucceeded) {
                    var scores = result.Data;
                    var buffer = scores.Scores;
                    AN_Logger.Log("scores.Leaderboard.DisplayName: " + scores.Leaderboard.DisplayName);
                    AN_Logger.Log("Loaded scores Count: " + buffer.Scores.Count);

                    foreach (var score in buffer.Scores) {
                        AN_Logger.Log("score.DisplayRank: " + score.DisplayRank);
                        AN_Logger.Log("score.DisplayScore: " + score.DisplayScore);
                        AN_Logger.Log("score.Rank: " + score.Rank);
                        AN_Logger.Log("score.RawScore: " + score.RawScore);
                        AN_Logger.Log("score.ScoreHolder: " + score.ScoreHolder);
                        AN_Logger.Log("score.ScoreHolderDisplayName: " + score.ScoreHolderDisplayName);
                        AN_Logger.Log("score.ScoreHolderIconImageUri: " + score.ScoreHolderIconImageUri);
                        AN_Logger.Log("score.ScoreHolderHiResImageUri: " + score.ScoreHolderHiResImageUri);
                        AN_Logger.Log("score.ScoreTag: " + score.ScoreTag);
                        AN_Logger.Log("score.TimestampMillis: " + score.TimestampMillis);
                        AN_Logger.Log("------------------------------------------------");
                    }
                } else {
                    AN_Logger.Log("Failed to Load Top Scores " + result.Error.FullMessage);
                }
            });

        });


        m_sbmitNoCB.onClick.AddListener(() => {
            var leaderboards = AN_Games.GetLeaderboardsClient();
            leaderboards.SubmitScore(leaderboardsId, 250);
        });


        m_sbmit.onClick.AddListener(() => {
            var leaderboards = AN_Games.GetLeaderboardsClient();
            leaderboards.SubmitScoreImmediate(leaderboardsId, 200, "Tag", (result) => {

                if(result.IsSucceeded) {
                    var scoreSubmissionData = result.Data;
                    AN_Logger.Log("SubmitScoreImmediate completed");
                    AN_Logger.Log("scoreSubmissionData.PlayerId: " + scoreSubmissionData.PlayerId);
                    AN_Logger.Log("scoreSubmissionData.PlayerId: " + scoreSubmissionData.LeaderboardId);

                    foreach (AN_Leaderboard.TimeSpan span in (AN_Leaderboard.TimeSpan[])System.Enum.GetValues(typeof(AN_Leaderboard.TimeSpan))) {
                        var scoreSubmissionResult = scoreSubmissionData.GetScoreResult(span);
                        AN_Logger.Log("scoreSubmissionData.FormattedScore: " + scoreSubmissionResult.FormattedScore);
                        AN_Logger.Log("scoreSubmissionData.NewBest: " + scoreSubmissionResult.NewBest);
                        AN_Logger.Log("scoreSubmissionData.RawScore: " + scoreSubmissionResult.RawScore);
                        AN_Logger.Log("scoreSubmissionData.ScoreTag: " + scoreSubmissionResult.ScoreTag);
                    }
                } else {
                    AN_Logger.Log("Failed to Submit Score Immediate " + result.Error.FullMessage);
                }

            });
        });



        m_nativeUI.onClick.AddListener(() => {
            var leaderboards = AN_Games.GetLeaderboardsClient();
            leaderboards.GetAllLeaderboardsIntent((result) => {
                if (result.IsSucceeded) {
                    var intent = result.Intent;
                    AN_ProxyActivity proxy = new AN_ProxyActivity();
                    proxy.StartActivityForResult(intent, (intentResult) => {
                        proxy.Finish();
                        //TODO you might want to check is user had sigend out with that UI
                    });

                } else {
                    Debug.Log("Failed to Get leaderboards Intent " + result.Error.FullMessage);
                }
            });

        });


        m_loadOneLeaberboard.onClick.AddListener(() => {
            var leaderboards = AN_Games.GetLeaderboardsClient();

            leaderboards.LoadLeaderboardMetadata(leaderboardsId, true, (result) => {
                if (result.IsSucceeded) {
                    PrintLeaderboardsInfo(new List<AN_Leaderboard>() { result.Leaderboard });
                } else {
                    AN_Logger.Log("Load Leaderboard Failed: " + result.Error.FullMessage);
                }
            });
        });

        m_loadLeaberboards.onClick.AddListener(() => {
            var leaderboards = AN_Games.GetLeaderboardsClient();

            
            leaderboards.LoadLeaderboardMetadata(false, (result) => {
                if (result.IsSucceeded) {
                    AN_Logger.Log("Load Leaderboards Metadata Succeeded, count: " + result.Leaderboards.Count);
                    PrintLeaderboardsInfo(result.Leaderboards);

                } else {
                    AN_Logger.Log("Load Leaderboards Failed: " + result.Error.FullMessage);
                }
            });
        });
    }


    private void PrintLeaderboardsInfo(List<AN_Leaderboard> leaderboards) {
        foreach (var leaderboard in leaderboards) {
            AN_Logger.Log("------------------------------------------------");
            AN_Logger.Log("leaderboard.LeaderboardId: " + leaderboard.LeaderboardId);
            AN_Logger.Log("leaderboard.Description: " + leaderboard.DisplayName);
            AN_Logger.Log("leaderboard.Name: " + leaderboard.IconImageUri);
            AN_Logger.Log("leaderboard.UnlockedImageUri: " + leaderboard.LeaderboardScoreOrder);

            AN_Logger.Log("leaderboard.Variants.Count: " + leaderboard.Variants.Count);

            foreach (var variant in leaderboard.Variants) {
                AN_Logger.Log("***************************");
                AN_Logger.Log("variant.Collection: " + variant.Collection);
                AN_Logger.Log("variant.DisplayPlayerRank: " + variant.DisplayPlayerRank);
                AN_Logger.Log("variant.DisplayPlayerScore: " + variant.DisplayPlayerScore);
                AN_Logger.Log("variant.NumScores: " + variant.NumScores);
                AN_Logger.Log("variant.PlayerRank: " + variant.PlayerRank);
                AN_Logger.Log("variant.PlayerScoreTag: " + variant.PlayerScoreTag);
                AN_Logger.Log("variant.RawPlayerScore: " + variant.RawPlayerScore);
                AN_Logger.Log("variant.TimeSpan: " + variant.TimeSpan);
                AN_Logger.Log("variant.HasPlayerInfo: " + variant.HasPlayerInfo);

            }
        }
        AN_Logger.Log("------------------------------------------------");
    }

}
