using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AudienceNetwork
{

    public class AdChoices : MonoBehaviour
    {
        [Header("Ad Choices:")]
        [SerializeField]
        public Image image;
        public Text text;
        public CanvasGroup canvasGroup;
        private string imageUrl;
        private string linkURL;

        void Awake()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
        }

        public void SetAd(NativeAdBase nativeAd)
        {
            text.text = nativeAd.AdChoicesText;
            linkURL = nativeAd.AdChoicesLinkURL;
            imageUrl = nativeAd.AdChoicesImageURL;
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            StartCoroutine(LoadAdChoicesImage());
        }

        public IEnumerator LoadAdChoicesImage()
        {
            Texture2D texture = new Texture2D(4, 4, TextureFormat.RGBA32, false);
            WWW www = new WWW(imageUrl);
            yield return www;
            www.LoadImageIntoTexture(texture);

            if (texture) {
                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
        }

        public void AdChoicesTapped()
        {
            Application.OpenURL(linkURL);
        }
    }
}
