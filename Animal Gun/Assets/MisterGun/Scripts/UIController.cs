using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Animator CameraAnim;

    public void SoundButton(Image currentImage)
    {
        AudioManager.Instance.SoundButton(currentImage);
    }
    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    public void EnableCameraAnim(string value)
    {
        CameraAnim.SetTrigger(value);
    }

    public void PanelAnim(Animator anim)
    {
        anim.SetTrigger("Up");
        StartCoroutine(ClosePanel(anim.gameObject));
    }

    private IEnumerator ClosePanel(GameObject panel)
    {
        yield return new WaitForSeconds(0.5f);
        panel.SetActive(false);
    }
}
