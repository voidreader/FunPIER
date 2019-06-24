using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFadeIn : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(DeactiveFadeinPanel());
    }

    IEnumerator DeactiveFadeinPanel()
    {
        yield return new WaitForSeconds(1f);
		gameObject.SetActive(false);
        yield break;
    }
}
