using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SA.Android.Media;

public class AN_PlayVideoExample : MonoBehaviour
{

    private static string movieURL = "https://videocdn.bodybuilding.com/video/mp4/62000/62792m.mp4";

    [SerializeField] Button m_playButton  = null;

    void Start()  {
        m_playButton.onClick.AddListener(() => {
            AN_MediaPlayer.ShowRemoteVideo(movieURL, () => {
                Debug.Log("Video closed");
            });
        });
    }


}
