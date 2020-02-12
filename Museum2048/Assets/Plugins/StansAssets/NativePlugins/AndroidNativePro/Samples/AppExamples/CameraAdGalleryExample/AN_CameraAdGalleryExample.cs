using UnityEngine;
using UnityEngine.UI;
using SA.Android.Camera;
using SA.Android.Gallery;
using SA.Android.Utilities;

using SA.Foundation.Utility;

public class AN_CameraAdGalleryExample : MonoBehaviour {

#pragma warning disable 649
    [SerializeField] RawImage m_image;
    [SerializeField] Image m_sprite;

    [Header("Camera")]
    [SerializeField] Button m_captureAdvanced;
     //[SerializeField] Button m_nativeCapture;
    [SerializeField] Button m_captureVideo;

    [Header("Gallery")]
    [SerializeField] Button m_getPictures;
    [SerializeField] Button m_getVideos;
    [SerializeField] Button m_getMixed;

    [SerializeField] Button m_saveScreenshot;
    
#pragma warning restore 649
   
   

    private void AddFitter(GameObject go) {
        var fitter = go.AddComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        fitter.aspectRatio = 1;
    }

    private void Awake() {

        AddFitter(m_image.gameObject);
        AddFitter(m_sprite.gameObject);
        
        m_captureAdvanced.onClick.AddListener(() => {
            var maxSize = 1024;
            AN_Camera.CaptureImage(maxSize, (result) => {
                PrintCaptureResult(result);
            });
        });
        

        m_captureVideo.onClick.AddListener(() => {
            var maxSize = 1024;

            AN_Camera.CaptureVideo(maxSize, (result) => {
                PrintCaptureResult(result);
            });
        });



        m_getVideos.onClick.AddListener(() => {

            var picker = new AN_MediaPicker(AN_MediaType.Video);
            picker.AllowMultiSelect = true;
            picker.MaxSize = 512;

            picker.Show((result) => {
                PrintPickerResult(result);
            });

        });


        m_getMixed.onClick.AddListener(() => {
            var picker = new AN_MediaPicker(AN_MediaType.Image, AN_MediaType.Video);
            picker.AllowMultiSelect = true;
            picker.MaxSize = 512;

            picker.Show((result) => {
                PrintPickerResult(result);
            });
        });

        m_getPictures.onClick.AddListener(() => {

            var picker = new AN_MediaPicker(AN_MediaType.Image);

            // Defines if multiple images picker is allowed.
            // The default value is < c > false </ c >
            picker.AllowMultiSelect = true;

            // Max thumbnail size that will be transferred to the Unity side.
            // The thumbnail will be resized before it sent.
            // The default value is 512.
            picker.MaxSize = 512;

            // Starts pick media from a gallery flow.
            picker.Show((result) => {
                PrintPickerResult(result);
            });
        });



        m_saveScreenshot.onClick.AddListener(() => {
            SA_ScreenUtil.TakeScreenshot(512, (screenshot) => {
                AN_Gallery.SaveImageToGallery(screenshot, "ExampleScreenshot", (result) => {
                    if (result.IsFailed) {
                        AN_Logger.Log("Filed:  " + result.Error.Message);
                        return;
                    }

                    AN_Logger.Log("Screenshot has been saved to:  " + result.Path);
                });
            });
        });
    }


    private void PrintPickerResult(AN_GalleryPickResult result) {
        if (result.IsFailed) {
            AN_Logger.Log("Picker Filed:  " + result.Error.Message);
            return;
        }

        AN_Logger.Log("Picked media count: " + result.Media.Count);
        foreach (var an_media in result.Media) {
            AN_Logger.Log("an_media.Type: " + an_media.Type);
            AN_Logger.Log("an_media.Path: " + an_media.Path);
            AN_Logger.Log("an_media.Thumbnail: " + an_media.Thumbnail);
        }

        ApplyImageToGUI(result.Media[0].Thumbnail);

    }


    private void PrintCaptureResult(AN_CameraCaptureResult result) {
        if (result.IsFailed) {
            AN_Logger.Log("Filed:  " + result.Error.Message);
            return;
        }

        AN_Logger.Log("result.Media.Type: " + result.Media.Type);
        AN_Logger.Log("result.Media.Path: " + result.Media.Path);
        AN_Logger.Log("result.Media.Thumbnail: " + result.Media.Thumbnail);


        ApplyImageToGUI(result.Media.Thumbnail);
        AN_Camera.DeleteCapturedMedia();
    }


    private void ApplyImageToGUI(Texture2D image) {

        var aspectRatio =  (float)image.width / (float)image.height;

        m_image.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;
        m_sprite.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;

        //m_image is a UnityEngine.UI.RawImage
        m_image.texture = image;

        //m_sprite is a UnityEngine.UI.Image
        m_sprite.sprite = image.ToSprite();
    }

}
