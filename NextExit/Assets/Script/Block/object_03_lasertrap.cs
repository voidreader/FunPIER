using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BlockBase))]
public class object_03_lasertrap : MonoBehaviour
{
    public BlockBase Base { get; private set; }

    [Header("이동과 회전이되는 오브젝트.")]
    public Transform MoveTransform;
    [Header("줌인으로 적용되는 최소 크기")]
    public float ZoomInSize = 0.25f;
    [Header("오브젝트 회전 속도")]
    public float RotateSpeed = 100.0f;
    [Header("오브젝트 이동 속도")]
    public float MoveSpeed = 50.0f;
    [Header("줌인 속도")]
    public float ZoomInSpeed = 0.5f;
    [Header("줌아웃 속도")]
    public float ZoomOutSpeed = 0.5f;
    [Header("하단으로 이동했을때 딜레이")]
    public float DownDelay = 0.5f;
    [Header("상단으로 이동했을때 딜레이")]
    public float UpDelay = 0.5f;

    void Awake()
    {
        Base = GetComponent<BlockBase>();

        // 블럭이 리셋되거나 새로운 게임을 시작될때 호출된다.
        Base.DelegateReset = () =>
        {
            MoveTransform.localPosition = new Vector3(0, 0, 0);
            MoveTransform.localEulerAngles = new Vector3(0, 0, 0);
            MoveTransform.localScale = new Vector3(1, 1, 1);
            StopCoroutine("cRotate");
            StopCoroutine("cMove");
            if (GameManager.Instance.IsPlaying)
            {
                StartCoroutine("cRotate");
                StartCoroutine("cMove");
            }
        };
    }

    IEnumerator cRotate()
    {
        while (GameManager.Instance.IsPlaying)
        {
            MoveTransform.localEulerAngles += new Vector3(0, 0, -RotateSpeed * Time.deltaTime);
            yield return null;
        }
        MoveTransform.localEulerAngles = new Vector3(0, 0, 0);
    }

    enum eState
    {
        ZoomIn,
        MoveDown,
        DelayDown,
        MoveUp,
        DelayUp,
        MoveCenter,
        ZoomOut
    }

    IEnumerator cMove()
    {
        eState state = eState.ZoomIn;
        float delay = 0.0f;
        while (GameManager.Instance.IsPlaying)
        {
            switch (state)
            {
                case eState.ZoomIn:
                    {
                        float zoom = ZoomInSpeed * Time.deltaTime;
                        MoveTransform.localScale -= new Vector3(zoom, zoom, 0);
                        if (MoveTransform.localScale.x <= ZoomInSize)
                        {
                            MoveTransform.localScale = new Vector3(ZoomInSize, ZoomInSize, 0);
                            state = eState.MoveDown;
                        }
                    }
                    break;
                case eState.MoveDown:
                    {
                        MoveTransform.localPosition += new Vector3(0, -MoveSpeed / GameConfig.PixelsPerMeter * Time.deltaTime, 0);
                        if (MoveTransform.localPosition.y <= -18.0f / GameConfig.PixelsPerMeter)
                        {
                            MoveTransform.localPosition = new Vector3(0, -18.0f / GameConfig.PixelsPerMeter, 0);
                            state = eState.DelayDown;
                            delay = DownDelay;
                        }
                    }
                    break;
                case eState.DelayDown:
                    {
                        delay -= Time.deltaTime;
                        if (delay <= 0.0f)
                            state = eState.MoveUp;
                    }
                    break;
                case eState.MoveUp:
                    {
                        MoveTransform.localPosition += new Vector3(0, MoveSpeed / GameConfig.PixelsPerMeter * Time.deltaTime, 0);
                        if (MoveTransform.localPosition.y >= 18.0f / GameConfig.PixelsPerMeter)
                        {
                            MoveTransform.localPosition = new Vector3(0, 18.0f / GameConfig.PixelsPerMeter, 0);
                            state = eState.DelayUp;
                            delay = UpDelay;
                        }
                    }
                    break;
                case eState.DelayUp:
                    {
                        delay -= Time.deltaTime;
                        if (delay <= 0.0f)
                            state = eState.MoveCenter;
                    }
                    break;
                case eState.MoveCenter:
                    {
                        MoveTransform.localPosition += new Vector3(0, -MoveSpeed / GameConfig.PixelsPerMeter * Time.deltaTime, 0);
                        if (MoveTransform.localPosition.y <= 0)
                        {
                            MoveTransform.localPosition = new Vector3(0, 0, 0);
                            state = eState.ZoomOut;
                        }
                    }
                    break;
                case eState.ZoomOut:
                    {
                        float zoom = ZoomOutSpeed * Time.deltaTime;
                        MoveTransform.localScale += new Vector3(zoom, zoom, 0);
                        if (MoveTransform.localScale.x >= 1)
                        {
                            MoveTransform.localScale = new Vector3(1, 1, 0);
                            state = eState.ZoomIn;
                        }
                    }
                    break;
            }

            yield return null;
        }
        MoveTransform.localPosition = new Vector3(0, 0, 0);
        MoveTransform.localScale = new Vector3(1, 1, 1);
    }

}
