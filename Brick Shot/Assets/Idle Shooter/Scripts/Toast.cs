using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class Toast : MonoBehaviour
{
    public RectTransform backgroundTransform;
    public RectTransform messageTransform;

    public static Toast instance;
    [HideInInspector]
    public bool isShowing = false;

    private Queue<AToast> queue = new Queue<AToast>();

    private class AToast
    {
        public string msg;
        public float duration;
        public float requestTime;
        public AToast(string msg, float duration)
        {
            this.msg = msg;
            this.duration = duration;
            requestTime = Time.time;
        }
    }

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public void SetMessage(string msg)
    {
        messageTransform.GetComponent<Text>().text = msg;
        //backgroundTransform.sizeDelta = new Vector2(messageTransform.GetComponent<Text>().preferredWidth + 30, backgroundTransform.sizeDelta.y);
    }

    private void Show(AToast aToast)
    {
        SetMessage(aToast.msg);
        gameObject.SetActive(true);
        Invoke("Hide", aToast.duration);
        isShowing = true;
    }

    public void ShowMessage(string msg, float duration = 2f)
    {
        AToast aToast = new AToast(msg, duration);
        queue.Enqueue(aToast);

        ShowOldestToast();
    }

    public void ShowMessage2(string msg) {
        AToast aToast = new AToast(msg, 1f);
        queue.Enqueue(aToast);

        ShowOldestToast();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
        Invoke("CompleteHiding", 0.3f);
    }

    private void CompleteHiding()
    {
        isShowing = false;
        ShowOldestToast();
    }

    private void ShowOldestToast()
    {
        if (queue.Count == 0) return;
        if (isShowing) return;

        AToast current = queue.Dequeue();
        if (current.requestTime < Time.time - 2)
        {
            ShowOldestToast();
            return;
        }
        Show(current);
    }
}
