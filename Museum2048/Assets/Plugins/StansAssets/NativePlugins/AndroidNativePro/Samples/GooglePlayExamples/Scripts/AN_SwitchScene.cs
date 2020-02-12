using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Button))]
public class AN_SwitchScene : MonoBehaviour {

#pragma warning disable 649
    [SerializeField] Object asset;
#pragma warning restore 649

    private void Start() {
        GetComponent<Button>().onClick.AddListener(() => {
            SceneManager.LoadScene(asset.name);
        });
    }

}
