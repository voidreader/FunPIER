using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FloatingMessage : MonoBehaviour
{
    public static FloatingMessage main = null;
    public Text message;

    private void Awake() {
        main = this;
        main.gameObject.SetActive(false);
    }

    public static void ShowMessage(string text) {
        main.gameObject.SetActive(true);
        main.message.text = text;

        main.transform.DOKill();
        main.transform.localPosition = new Vector3(0, -200, 0);
        main.transform.DOLocalMoveY(-100, 1).OnComplete(main.OnCompleteMove);
    }

    public void OnCompleteMove() {
        main.gameObject.SetActive(false);
    }

}
