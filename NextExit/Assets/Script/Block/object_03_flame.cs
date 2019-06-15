using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BlockBase))]
public class object_03_flame : MonoBehaviour
{
    public BlockBase Base { get; private set; }
    [Header("충돌 체크 오브젝트")]
    public GameObject ObjectBullet;
    [Header("루프 진행 시간")]
    float LoopTimer = 2.0f;
    [Header("안보이는 유지 시간")]
    float HideTimer = 2.0f;

	public tk2dSprite anim;

	public BoxCollider2D coll;

	private Dictionary<int, Vector2> sizeDic = new Dictionary<int, Vector2>();
	private Dictionary<int, Vector2> offsetDic = new Dictionary<int, Vector2>();

    void Awake()
    {
        Base = GetComponent<BlockBase>();
		sizeDic.Add( 93, new Vector2( 0.8f, 1.5f ) );
		sizeDic.Add( 88, new Vector2( 0.8f, 1.9f ) );
		sizeDic.Add( 87, new Vector2( 0.8f, 2.4f ) );
		sizeDic.Add( 86, new Vector2( 0.8f, 2.6f ) );
		sizeDic.Add( 92, new Vector2( 0.8f, 2.98f ) );
		sizeDic.Add( 90, new Vector2( 0.8f, 3.4f ) );
		sizeDic.Add( 89, new Vector2( 0.8f, 3.4f ) );
		sizeDic.Add( 91, new Vector2( 0.8f, 3.4f ) );

		offsetDic.Add( 93, new Vector2( 0f, -1.36f ) );
		offsetDic.Add( 88, new Vector2( 0f, -1.19f ) );
		offsetDic.Add( 87, new Vector2( 0f, -0.93f ) );
		offsetDic.Add( 86, new Vector2( 0f, -0.83f ) );
		offsetDic.Add( 92, new Vector2( 0f, -0.65f ) );
		offsetDic.Add( 90, new Vector2( 0f, -0.4f ) );
		offsetDic.Add( 89, new Vector2( 0f, -0.4f ) );
		offsetDic.Add( 91, new Vector2( 0f, -0.4f ) );

        // 블럭이 리셋되거나 새로운 게임을 시작될때 호출된다.
        Base.DelegateReset = () =>
        {
			m_action = false;
			m_spriteIndex = 1;
            ObjectBullet.SetActive(false);
            StopCoroutine("cShot");
            if (GameManager.Instance.IsPlaying)
            {
                Base.Animated.playAutomatically = false;
                StartCoroutine("cShot");
            }
			m_spriteIndex = anim.spriteId;
        };
    }

	bool m_action = false;

	int m_spriteIndex = 1;

	void Update()
	{
		if (GameManager.Instance.IsPlaying)
		{
			if( m_action && m_spriteIndex != anim.spriteId )
			{
				m_spriteIndex = anim.spriteId;
				coll.size = sizeDic[m_spriteIndex];
				coll.offset = offsetDic[m_spriteIndex];
			}
		}
	}

    IEnumerator cShot()
    {
        ObjectBullet.SetActive(false);

        yield return new WaitForSeconds(HideTimer);
		m_action = true;
        ObjectBullet.SetActive(true);
        Base.Animated.Play("flame_start");
        Base.Animated.AnimationCompleted = (animator, clip) =>
        {
            Base.Animated.AnimationCompleted = null;                
            StartCoroutine("cEndShot");
        };

    }

    IEnumerator cEndShot()
    {
        Base.Animated.Play("flame_loop");
        yield return new WaitForSeconds(LoopTimer);
        Base.Animated.Play("flame_end");
        Base.Animated.AnimationCompleted = (animator, clip) =>
        {
			m_action = false;
			m_spriteIndex = 1;
            Base.Animated.AnimationCompleted = null;
            StartCoroutine("cShot");
        };
    }

}
