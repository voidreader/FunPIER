using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToMainScene : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(MainScene());
    }
    

    IEnumerator MainScene()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        yield break;
    }

}
