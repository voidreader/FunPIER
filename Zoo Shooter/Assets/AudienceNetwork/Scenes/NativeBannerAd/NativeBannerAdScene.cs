using UnityEngine;
using UnityEngine.UI;
using AudienceNetwork;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasRenderer))]
[RequireComponent(typeof(RectTransform))]
public class NativeBannerAdScene : MonoBehaviour
{
    private NativeBannerAd nativeBannerAd;

    // UI elements in scene
    [Header("Text:")]
    public Text advertiserName;
    public Text sponsored;
    public Text status; // Show ad status in the sample Unity scene
    [Header("Images:")]
    public GameObject iconImage;
    [Header("Buttons:")]
    // This doesn't be a button - it can also be an image
    public Button callToActionButton;
    [Header("Ad Choices:")]
    public AdChoices adChoices;

    void Awake()
    {
        Log("Native banner ad ready to load.");
    }

    void OnDestroy()
    {
        // Dispose of native ad when the scene is destroyed
        if (nativeBannerAd) {
            nativeBannerAd.Dispose();
        }
        Debug.Log("NativeBannerAdTest was destroyed!");
    }

    // Load Ad button
    public void LoadAd()
    {
        if (nativeBannerAd != null) {
            nativeBannerAd.Dispose();
        }

        // Create a native ad request with a unique placement ID (generate your own on the Facebook app settings).
        // Use different ID for each ad placement in your app.
        nativeBannerAd = new NativeBannerAd("YOUR_PLACEMENT_ID");

        // Wire up GameObject with the native banner ad. The game object should be a child of the canvas.
        nativeBannerAd.RegisterGameObject(gameObject);

        // Set delegates to get notified on changes or when the user interacts with the ad.
        nativeBannerAd.NativeAdDidLoad = delegate() {
            // Register game objects for interactions.
            // IconImage will be used for impression logging.
            // CallToActionButton will be used for click logging.
            nativeBannerAd.RegisterGameObjectsForInteraction((RectTransform)iconImage.transform,
                    (RectTransform)callToActionButton.transform);
            string isAdValid = nativeBannerAd.IsValid() ? "valid" : "invalid";
            Log("Native banner ad loaded and is " + isAdValid + ".");
            adChoices.SetAd(nativeBannerAd);
            advertiserName.text = nativeBannerAd.AdvertiserName;
            sponsored.text = nativeBannerAd.SponsoredTranslation;
            callToActionButton.GetComponentInChildren<Text>().text = nativeBannerAd.CallToAction;
        };
        nativeBannerAd.NativeAdDidDownloadMedia = delegate() {
            Log("Native banner ad media downloaded");
        };
        nativeBannerAd.NativeAdDidFailWithError = delegate(string error) {
            Log("Native banner ad failed to load with error: " + error);
        };
        nativeBannerAd.NativeAdWillLogImpression = delegate() {
            Log("Native banner ad logged impression.");
        };
        nativeBannerAd.NativeAdDidClick = delegate() {
            Log("Native banner ad clicked.");
        };

        // Initiate a request to load an ad.
        nativeBannerAd.LoadAd();

        Log("Native banner ad loading...");
    }

    private void Log(string s)
    {
        status.text = s;
        Debug.Log(s);
    }

    // Next button
    public void NextScene()
    {
        SceneManager.LoadScene("InterstitialAdScene");
    }
}
