using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using SA.Android.GMS.Auth;
using SA.Android.GMS.Common;
using SA.Android.GMS.Games;
using SA.Android.App;

using SA.Android.Utilities;

using SA.Foundation.Network.Web;

public class AN_GMS_Achievements_Example : MonoBehaviour {


   
    [SerializeField] Button m_nativeUI = null;
    [SerializeField] Button m_load = null;



    private void Start() {


        m_nativeUI.onClick.AddListener(() => {

            var client = AN_Games.GetAchievementsClient();

            client.GetAchievementsIntent((result) => {
                if(result.IsSucceeded) {
                    var intent = result.Intent;
                    AN_ProxyActivity proxy = new AN_ProxyActivity();
                    proxy.StartActivityForResult(intent, (intentResult) => {
                        proxy.Finish();
                        //TODO you might want to check is user had sigend out with that UI
                    });

                } else {
                    Debug.Log("Failed to Get Achievements Intent " + result.Error.FullMessage);
                }
            });
        });



        m_load.onClick.AddListener(() => {
            var client = AN_Games.GetAchievementsClient();

            client.Load(false, (result) => {
                
                if (result.IsSucceeded) {
                    AN_Logger.Log("Load Achievements Succeeded, count: " + result.Achievements.Count);
                    foreach (var achievement in result.Achievements) {
                        AN_Logger.Log("------------------------------------------------");
                        AN_Logger.Log("achievement.AchievementId: " + achievement.AchievementId);
                        AN_Logger.Log("achievement.Description: " + achievement.Description);
                        AN_Logger.Log("achievement.Name: " + achievement.Name);
                        AN_Logger.Log("achievement.UnlockedImageUri: " + achievement.UnlockedImageUri);
                        AN_Logger.Log("achievement.CurrentSteps: " + achievement.CurrentSteps);
                        AN_Logger.Log("achievement.TotalSteps: " + achievement.TotalSteps);
                        AN_Logger.Log("achievement.Type: " + achievement.Type);
                        AN_Logger.Log("achievement.Sate: " + achievement.State);
                    }

                    AN_Logger.Log("------------------------------------------------");
                } else {
                    Debug.Log("Load Achievements Failed: " + result.Error.FullMessage);
                }

                

                foreach (var achievement in result.Achievements) {
                    if(achievement.Type == AN_Achievement.AchievementType.INCREMENTAL) {
                        client.Increment(achievement.AchievementId, 1);
                        continue;
                    }

                    if (achievement.State == AN_Achievement.AchievementState.HIDDEN) {
                        client.Reveal(achievement.AchievementId);
                        continue;
                    }

                    if (achievement.State != AN_Achievement.AchievementState.UNLOCKED) {
                        client.Unlock(achievement.AchievementId);
                    }
                }
            });

        });
    }
}
