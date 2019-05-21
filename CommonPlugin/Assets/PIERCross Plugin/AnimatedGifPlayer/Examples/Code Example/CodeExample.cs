using UnityEngine;
using UnityEngine.UI;
using OldMoatGames;

public class CodeExample : MonoBehaviour {
    [SerializeField] AnimatedGifPlayer _animatedGifPlayer;

    
    public void Awake() {
        // Get the GIF player component
        // _animatedGifPlayer = GetComponent<AnimatedGifPlayer>();

        // Set the file to use. File has to be in StreamingAssets folder or a remote url (For example: http://www.example.com/example.gif).
        // AnimatedGifPlayer.FileName = "AnimatedGIFPlayerExampe 3.gif";
        _animatedGifPlayer.FileName = "http://www.pier-showcase.com/gifs/test.gif";

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
        this.gameObject.SetActive(true);
        _animatedGifPlayer.Play();

    }

    private void OnGifLoadError() {
        Debug.Log("Error Loading GIF");
    }

    public void OnClickClose() {
        _animatedGifPlayer.Pause();
        this.gameObject.SetActive(false);

    }

    public void OnClickPlay() {

    }

    /*
    public void Play() {
        // Start playing the GIF
        AnimatedGifPlayer.Play();

        // Disable the play button
        PlayButton.interactable = false;

        // Enable the pause button
        PauseButton.interactable = true;
    }
    */

    /*
    public void Pause() {
        // Stop playing the GIF
        AnimatedGifPlayer.Pause();

        // Enable the play button
        PlayButton.interactable = true;

        // Disable the pause button
        PauseButton.interactable = false;
    }
    */


}
