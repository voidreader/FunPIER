using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour {

    public GameObject copyright;
    bool isTallScreen = false;

    // Start is called before the first frame update
    void Start() {


        float screenw = (float)Screen.width;
        float screenh = (float)Screen.height;

        float ratio = screenw / screenh;

        isTallScreen = false;
        if (ratio < 0.56f)
            isTallScreen = true;


        if (isTallScreen)
            copyright.transform.localPosition = new Vector3(0, -710, 0);
        else
            copyright.transform.localPosition = new Vector3(0, -620, 0);

        StartCoroutine(TitleRoutine());

        ScissorCtrl.Instance.UpdateResolution();
    }

    IEnumerator TitleRoutine() {

        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Main");
    }
}
