using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BlockBase))]
public class object_03_electric : MonoBehaviour
{
    public BlockBase Base { get; private set; }
    [Header("충돌 체크 오브젝트")]
    public GameObject ObjectBullet;
    [Header("루프 진행 시간")]
    public float LoopTimer = 2.0f;
    [Header("안보이는 유지 시간")]
    public float HideTimer = 2.0f;

    void Awake()
    {
        Base = GetComponent<BlockBase>();

        // 블럭이 리셋되거나 새로운 게임을 시작될때 호출된다.
        Base.DelegateReset = () =>
        {
            ObjectBullet.SetActive(false);
            StopCoroutine("cShot");
            if (GameManager.Instance.IsPlaying)
            {
                Base.Animated.playAutomatically = false;
                StartCoroutine("cShot");
            }
        };
    }

    IEnumerator cShot()
    {
        ObjectBullet.SetActive(false);

        yield return new WaitForSeconds(HideTimer);

        ObjectBullet.SetActive(true);

        Base.Animated.Play("electric_start");
        Base.Animated.AnimationCompleted = (animator, clip) =>
        {
            Base.Animated.AnimationCompleted = null;                
            StartCoroutine("cEndShot");
        };

    }

    IEnumerator cEndShot()
    {
        Base.Animated.Play("electric_loop");
        yield return new WaitForSeconds(LoopTimer);
        Base.Animated.Play("electric_end");
        Base.Animated.AnimationCompleted = (animator, clip) =>
        {
            Base.Animated.AnimationCompleted = null;
            StartCoroutine("cShot");
        };
    }

}
