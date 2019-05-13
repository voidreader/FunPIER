using UnityEngine;
using System.Collections;

public class ScissorCtrl : MonoBehaviour {

    public static ScissorCtrl _instance = null;

    public GameObject m_objBackScissor;

    [SerializeField]
    bool _needChange = true;

    public float _wantedAspectRatio = 0.5625f;


    public static ScissorCtrl Instance {

        get {
            if (_instance == null) {
                _instance = FindObjectOfType(typeof(ScissorCtrl)) as ScissorCtrl;

                if (_instance == null) {
                    return null;
                }
            }

            return _instance;
        }

    }

    // Use this for initialization
    void Awake() {


        DontDestroyOnLoad(this.gameObject);
        float currentAspectRatio = (float)Screen.width / (float)Screen.height;

        Debug.Log("★★★ ScissorCtrl currentAspectRatio :: " + currentAspectRatio);

        /*
        if (currentAspectRatio - _wantedAspectRatio <= 0.01f && currentAspectRatio - _wantedAspectRatio >= -0.01f) {
            _needChange = false;
        }
        */


        if (currentAspectRatio > 0.6f) 
            _needChange = true;
        else
            _needChange = false;


        // 안드로이드 시저링 하지 않음
#if UNITY_ANDROID
        // _needChange = false;
#endif
    }

    // Use this for initialization
    void Start() {

    }


    public void UpdateResolution() {

        if (!_needChange)
            return;

        Camera[] objCameras = Camera.allCameras;

        //width 2, height 3
        float fResolutionX = Screen.width / 9.0f;
        float fResolutionY = Screen.height / 16.0f;

        if (fResolutionX > fResolutionY) {
            float fValue = (fResolutionX - fResolutionY) * 0.5f;
            fValue = fValue / fResolutionX;

            //fResolutionX fix, left & right Scissor (Viewport Re Setting)
            foreach (Camera obj in objCameras) {
                obj.rect = new Rect(Screen.width * fValue / Screen.width + obj.rect.x * (1.0f - 2.0f * fValue), obj.rect.y
                    , obj.rect.width * (1.0f - 2.0f * fValue), obj.rect.height);
            }

            GameObject objLeftScissor = (GameObject)Instantiate(m_objBackScissor);
            objLeftScissor.GetComponent<Camera>().rect = new Rect(0, 0, Screen.width * fValue / Screen.width, 1.0f);

            GameObject objRightScissor = (GameObject)Instantiate(m_objBackScissor);
            objRightScissor.GetComponent<Camera>().rect = new Rect((Screen.width - Screen.width * fValue) / Screen.width, 0
                , Screen.width * fValue / Screen.width, 1.0f);
        }
        else if (fResolutionX < fResolutionY) {

            float fValue = (fResolutionY - fResolutionX) * 0.5f;
            fValue = fValue / fResolutionY;

            //fResolutionY fix, Top & Bottom Scissor (Viewport Re Setting)
            foreach (Camera obj in objCameras) {
                obj.rect = new Rect(obj.rect.x, Screen.height * fValue / Screen.height + obj.rect.y * (1.0f - 2.0f * fValue)
                       , obj.rect.width, obj.rect.height * (1.0f - 2.0f * fValue));

                //obj.rect = new Rect( obj.rect.x , obj.rect.y + obj.rect.y * fValue, obj.rect.width, obj.rect.height - obj.rect.height * fValue );
            }

            GameObject objTopScissor = (GameObject)Instantiate(m_objBackScissor);
            objTopScissor.GetComponent<Camera>().rect = new Rect(0, 0, 1.0f, Screen.height * fValue / Screen.height);

            GameObject objBottomScissor = (GameObject)Instantiate(m_objBackScissor);
            objBottomScissor.GetComponent<Camera>().rect = new Rect(0, (Screen.height - Screen.height * fValue) / Screen.height
                , 1.0f, Screen.height * fValue / Screen.height);
        }
        else {
            // Do Not Setting Camera
        }
    }

}
