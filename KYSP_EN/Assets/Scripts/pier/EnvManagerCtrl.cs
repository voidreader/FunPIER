using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnvManagerCtrl : MonoBehaviour {

    public static EnvManagerCtrl Instance = null;

    public string AOS_appID = string.Empty;
    public string AOS_bannerID = string.Empty;
    public string AOS_InterstitialID = string.Empty;
    public string AOS_RewardID = string.Empty;

    public string IOS_appID = string.Empty;
    public string IOS_bannerID = string.Empty;
    public string IOS_InterstitialID = string.Empty;
    public string IOS_RewardID = string.Empty;



    public string AOS_UnityADS = string.Empty;
    public string IOS_UnityADS = string.Empty;


    void Awake() {
        if (Instance == null)
            Instance = this;

        DontDestroyOnLoad(this);

        
    }

    // Use this for initialization
    void Start() {


    }




}
