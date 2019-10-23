using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Minion : MonoBehaviour
{
    public Transform tr;

    // Start is called before the first frame update
    void Start()
    {
        tr.DOLocalRotate(new Vector3(0, 0, -90), 0.5f, RotateMode.Fast).SetEase(Ease.InQuad).SetDelay(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
