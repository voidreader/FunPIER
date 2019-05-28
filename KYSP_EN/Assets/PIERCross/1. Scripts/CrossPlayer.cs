using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OldMoatGames;
using SimpleJSON;

public class CrossPlayer : MonoBehaviour {

    public static CrossPlayer main = null;
    public int OpenCounter = 3;
    public int CurrentOpenCounter = 0;

    public Vector3 targetPos = Vector3.zero;
    [SerializeField] AnimatedGifPlayer _animatedGifPlayer;
    [SerializeField] Text _lblName;

    [SerializeField] string _fileurl = string.Empty;
    [SerializeField] string _name = string.Empty;
    [SerializeField] string _onelink = string.Empty;


    private void OnEnable() {
        Debug.Log("OnEnable in Cross");
        SetInitPos();
    }

    void Awake() {
        
    }

    void SetInitPos() {
        main = this;
        this.transform.localPosition = new Vector3(-2000, 0, 0);
        StartCoroutine(OnDelayPost());
    }

    // Start is called before the first frame update
    void Start() {
        // InitPlayer();
        
    }

    public void OpenCrossPlayer() {
        if(CurrentOpenCounter != 0) {
            CurrentOpenCounter++;

            if (CurrentOpenCounter >= OpenCounter)
                CurrentOpenCounter = 0;

            return;
        }


        this.gameObject.SetActive(true);

    }

    IEnumerator OnDelayPost() {
        yield return new WaitForSeconds(0.2f);
        GetCrossMarketingInfo();
    }

    public void GetCrossMarketingInfo() {

        Debug.Log("GetCrossMarketingInfo");
        WWWHelper.main.Post(RequestID.request_crossinfo, CallbackDownload, null);
    }

    void CallbackDownload(JSONNode node) {
        Debug.Log("Callback Download :: " + node.ToString());
        _onelink = node["onelink"];
        _fileurl = node["gif"];
        _name = node["name"];

        _lblName.text = _name;

        InitPlayer();

    }


    public void InitPlayer() {

        // Set the file to use. File has to be in StreamingAssets folder or a remote url (For example: http://www.example.com/example.gif).
        // AnimatedGifPlayer.FileName = "AnimatedGIFPlayerExampe 3.gif";
        _animatedGifPlayer.FileName = _fileurl;

        // Disable autoplay
        _animatedGifPlayer.AutoPlay = false;

        // Add ready event to start play when GIF is ready to play
        _animatedGifPlayer.OnReady += OnGifLoaded;

        // Add ready event for when loading has failed
        _animatedGifPlayer.OnLoadError += OnGifLoadError;

        // Init the GIF player
        _animatedGifPlayer.Init();
    }

    private void OnGifLoaded() {
        Debug.Log("GIF size: width: " + _animatedGifPlayer.Width + "px, height: " + _animatedGifPlayer.Height + " px");
        // this.gameObject.SetActive(true);
        _animatedGifPlayer.Play();
        this.transform.localPosition = targetPos;
        CurrentOpenCounter++;

        _animatedGifPlayer.OnReady -= OnGifLoaded;
    }

    private void OnGifLoadError() {
        Debug.Log("Error Loading GIF");
        _animatedGifPlayer.OnLoadError -= OnGifLoadError;
    }

    public void OnClickClose() {
        _animatedGifPlayer.Pause();
        this.gameObject.SetActive(false);

    }

    public void OnClickPlay() {


        Application.OpenURL(_onelink);
        OnClickClose();

    }


}
