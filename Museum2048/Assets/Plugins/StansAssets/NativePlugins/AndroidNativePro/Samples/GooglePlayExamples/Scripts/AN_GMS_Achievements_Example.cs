using UnityEngine;
using UnityEngine.UI;
using SA.Android.GMS.Games;
using SA.Android.App;

public class AN_GMS_Achievements_Example : MonoBehaviour
{
    [SerializeField]
    Button m_nativeUI = null;
    [SerializeField]
    Button m_load = null;

    void Start()
    {
        m_nativeUI.onClick.AddListener(() =>
        {
            var client = AN_Games.GetAchievementsClient();
            client.GetAchievementsIntent((result) =>
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
                    Debug.LogError($"Failed to Get Achievements Intent {result.Error.FullMessage}");
                }
            });
        });
        m_load.onClick.AddListener(() =>
        {
            var client = AN_Games.GetAchievementsClient();
            client.Load(false, (result) =>
            {
                if (result.IsSucceeded)
                {
                    Debug.Log($"Load Achievements Succeeded, count: {result.Achievements.Count}");
                    foreach (var achievement in result.Achievements)
                    {
                        Debug.Log("------------------------------------------------");
                        Debug.Log($"achievement.AchievementId: {achievement.AchievementId}");
                        Debug.Log($"achievement.Description: {achievement.Description}");
                        Debug.Log($"achievement.Name: {achievement.Name}");
                        Debug.Log($"achievement.UnlockedImageUri: {achievement.UnlockedImageUri}");
                        Debug.Log($"achievement.CurrentSteps: {achievement.CurrentSteps}");
                        Debug.Log($"achievement.TotalSteps: {achievement.TotalSteps}");
                        Debug.Log($"achievement.Type: {achievement.Type}");
                        Debug.Log($"achievement.Sate: {achievement.State}");
                    }

                    Debug.Log("------------------------------------------------");
                }
                else
                {
                    Debug.LogError($"Load Achievements Failed: {result.Error.FullMessage}");
                }

                foreach (var achievement in result.Achievements)
                {
                    if (achievement.Type == AN_Achievement.AchievementType.INCREMENTAL)
                    {
                        client.Increment(achievement.AchievementId, 1);
                        continue;
                    }

                    if (achievement.State == AN_Achievement.AchievementState.HIDDEN)
                    {
                        client.Reveal(achievement.AchievementId);
                        continue;
                    }

                    if (achievement.State != AN_Achievement.AchievementState.UNLOCKED) client.Unlock(achievement.AchievementId);
                }
            });
        });
    }
}
