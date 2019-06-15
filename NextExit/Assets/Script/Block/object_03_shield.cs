using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BlockBase))]
public class object_03_shield : MonoBehaviour
{
    public BlockBase Base { get; private set; }
    [Header("충돌 체크 오브젝트")]
    public GameObject ObjectBullet;
    [Header("끝지점에서 멈춰있는 시간")]
    public float HideTimer = 2.0f;

    void Awake()
    {
        Base = GetComponent<BlockBase>();

        // 블럭이 리셋되거나 새로운 게임을 시작될때 호출된다.
        Base.DelegateReset = () =>
        {
            Base.MainCollider.enabled = false;
            Base.Animated.playAutomatically = false;
            if (Base.Animated.CurrentClip != null)
                Base.Animated.CurrentClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            Base.Animated.StopAndResetFrame();
            StopCoroutine("cShot");
            if (GameManager.Instance.IsPlaying)
                StartCoroutine("cShot");
        };
    }

    IEnumerator cShot()
    {
        Base.MainCollider.enabled = false;

        yield return new WaitForSeconds(HideTimer);
        
        Base.Animated.Play();
        Base.Animated.AnimationEventTriggered = (animator, clip, trigger) =>
        {
            Base.MainCollider.enabled = (clip.GetFrame(trigger).eventInt == 1);
        };
        Base.Animated.AnimationCompleted = (animator, clip) =>
        {
            Base.Animated.AnimationCompleted = null;
            Base.Animated.StopAndResetFrame();
            StartCoroutine("cShot");
        };

    }

}
