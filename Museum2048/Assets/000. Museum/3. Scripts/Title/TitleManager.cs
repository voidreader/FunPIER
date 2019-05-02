using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        StartCoroutine(TitleRoutine());
    }

    IEnumerator TitleRoutine() {

        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Main");
    }
}
