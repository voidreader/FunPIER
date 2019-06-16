using DG.Tweening;
using System;
using Toolkit;
using UnityEngine;

public class SettingBtns : MonoBehaviour
{
    private const int DistanceY = 95;

    //[SerializeField]
    // private GameObject m_NOShake;

    // [SerializeField]
    //  private GameObject m_Shake;

    [SerializeField]
    private GameObject m_Audio;

    [SerializeField]
    private GameObject m_NOAudio;

    [SerializeField]
    private GameObject m_Shake;

    [SerializeField]
    private GameObject m_NOShake;

    [SerializeField]
    private GameObject m_Restore;

    [SerializeField]
    private GameObject m_Setting;

    private bool m_OpenSetting;

    private bool m_Tweener;

    public bool OpenSetting
    {
        get
        {
            return this.m_OpenSetting;
        }
    }

    public void Refresh(bool init = false)
    {
        if (init || !this.m_OpenSetting)
        {
            this.m_OpenSetting = false;
            this.m_NOAudio.gameObject.SetActive(false);
            this.m_Audio.gameObject.SetActive(false);
            this.m_Shake.gameObject.SetActive(false);
            this.m_NOShake.gameObject.SetActive(false);
            this.m_Restore.gameObject.SetActive(false);
        }
        else
        {
            this.m_NOAudio.gameObject.SetActive(!MonoSingleton<GameDataManager>.Instance.Audio);
            this.m_Audio.gameObject.SetActive(MonoSingleton<GameDataManager>.Instance.Audio);
            this.m_Shake.gameObject.SetActive(MonoSingleton<GameDataManager>.Instance.PhoneShake);
            this.m_NOShake.gameObject.SetActive(!MonoSingleton<GameDataManager>.Instance.PhoneShake);
#if UNITY_IOS
            this.m_Restore.gameObject.SetActive(true);
#endif
        }
    }

    public void Setting()
    {
        if (this.m_Tweener)
        {
            return;
        }
        // MonoSingleton<GAEvent>.Instance.GameMenu();
        this.m_Tweener = true;
        this.m_OpenSetting = !this.m_OpenSetting;
        GameObject obj = (!MonoSingleton<GameDataManager>.Instance.Audio) ? this.m_NOAudio : this.m_Audio;
        GameObject obj2 = (!MonoSingleton<GameDataManager>.Instance.PhoneShake) ? this.m_NOShake : this.m_Shake;
        if (this.m_OpenSetting)
        {
            // this.unfold(obj2, 1);
            this.unfold(obj, 1);
            this.unfold(obj2, 2);
#if UNITY_IOS
            this.unfold(this.m_Restore, 3);
#endif
            this.m_Setting.transform.DOLocalRotate(new Vector3(0f, 0f, 45f), 0.2f, RotateMode.Fast).OnComplete(delegate
            {
                this.m_Tweener = false;
            });
        }
        else
        {
            this.fold(obj);
            this.fold(obj2);
#if UNITY_IOS
            this.fold(this.m_Restore);
#endif
            //    this.fold(obj2);
            this.m_Setting.transform.DOLocalRotate(Vector3.zero, 0.2f, RotateMode.Fast).OnComplete(delegate
            {
                this.m_Tweener = false;
            });
        }
    }

    private void unfold(GameObject obj, int pos)
    {
        obj.transform.localPosition = this.m_Setting.transform.localPosition;
        obj.transform.localScale = Vector3.zero;
        obj.gameObject.SetActive(true);
        obj.transform.DOScale(Vector3.one, 0.2f);
        obj.transform.DOLocalMove(this.m_Setting.transform.localPosition + Vector3.up * 110f * (float)pos, 0.4f, false);
    }

    private void fold(GameObject obj)
    {
        obj.transform.DOScale(Vector3.zero, 0.2f).SetDelay(0.1f);
        obj.transform.DOLocalMove(this.m_Setting.transform.localPosition, 0.4f, false);
    }
}
