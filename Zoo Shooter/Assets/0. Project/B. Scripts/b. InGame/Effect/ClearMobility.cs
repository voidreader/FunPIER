using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ClearMobility : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public void CallMobility() {
        this.transform.position = new Vector3(7.6f, 5.34f, 0);
        this.gameObject.SetActive(true);
        this.transform.DOMove(new Vector3(2.11f, 3.87f, 0), 0.4f);
        // 2.11, 3.87

        AudioAssistant.main.PlayLoopingSFX("Helicorpter");
    }

    public void OffMobility() {
        this.gameObject.SetActive(false);
        AudioAssistant.main.StopLoopingSFX();
    }
}
