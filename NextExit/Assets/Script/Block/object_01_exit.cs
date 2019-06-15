using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BlockBase))]
public class object_01_exit : MonoBehaviour {
    public BlockBase Base { get; private set; }

    void Awake()
    {
        Base = GetComponent<BlockBase>();

        Base.DelegateReset = () =>
        {
            if (m_IsOpen)
                Base.Animated.Play("exit_close");
        };
    }

    [Header("문이 열리는 거리(플레이어와의 간격)")]
    public float ShowDistance = 8.0f;
    bool m_IsOpen = false;

    void Update()
    {
        if (GameManager.Instance.IsPlaying)
        {
            float distance = Vector2.Distance(transform.position, GameManager.Instance.player.transform.position);
            //Debug.Log("distance = " + distance);
            if (distance > ShowDistance && m_IsOpen && !Base.Animated.Playing)
            {
                m_IsOpen = false;
                Base.Animated.Play("exit_close");
            }
            else if (distance < ShowDistance && !m_IsOpen)
            {
				RPGSoundManager.Instance.PlayEffectSound( 7 );
                m_IsOpen = true;
                Base.Animated.Play("exit_open");
            }
        }
    }

}
