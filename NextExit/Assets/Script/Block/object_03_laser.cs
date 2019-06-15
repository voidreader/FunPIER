using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BlockBase))]
public class object_03_laser : RPGLayer
{
    public BlockBase Base { get; private set; }

    [Header("최대 회전 값 +- 적용")]
    public float MaxRotate = 45.0f;

    [Header("회전값이 들어갈 Transform")]
    public Transform RotateTransform;
    [Header("발사체 Sprite")]
    public tk2dSlicedSprite SpriteBullet;

	public BoxCollider2D boxColliderBullet;

    [Header("플레이어와 거리가 멀때 이동 속도 초당 픽셀수")]
    public float BulletSpeedFar = 150.0f;
    [Header("플레이어와 거리가 가까울때 이동 속도 초당 픽셀수")]
    public float BulletSpeedNear = 40.0f;
    [Header("레이저 첫 발사 위치 플레이어와의 간격")]
    [Tooltip("위아래는 X축으로 이동하고 좌우는 Y축으로 이동합니다")]
    public float FirstPosition = -100.0f;
    [Header("레이저의 속도가 느려지는 플레이어와의 각도 차이")]
    //public float SlowDistance = 80.0f;
    public float SlowAngle = 10.0f;
    [Header("대상이 시야에서 벗어났을때 취소될때까지의 시간")]
    public float CancelTimer = 1.0f;
    [Header("발사가 끝난 이후의 대기 시간")]
    public float WaitTimer = 2.0f;

    /// <summary>
    /// 현재 발사 방향.
    /// </summary>
    float m_Angle = 0.0f;
    /// <summary>
    /// 발사체를 발사할 준비가 되어 있는지?
    /// </summary>
    bool m_IsReady = false;

    void Awake()
    {
        Base = GetComponent<BlockBase>();
        // 블럭이 리셋되거나 새로운 게임을 시작될때 호출된다.
        Base.DelegateReset = () =>
        {
            m_IsReady = false;
            SpriteBullet.dimensions = new Vector2(0, 0);
            RotateTransform.localEulerAngles = new Vector3(0, 0, 0);
            StopCoroutine("cShot");
            if (GameManager.Instance.IsPlaying)
                StartCoroutine("cShot");
        };
    }

    void Update()
    {
        if (GameManager.Instance.IsPlaying && !m_IsReady)
        {
            Vector2 BulletPosition = RotateTransform.position;
            Vector2 playerPosition = GameManager.Instance.player.transform.position;

            m_Angle = RPGDefine.getAngle(BulletPosition, playerPosition);
            float distanceAngle = Base.BlockData.DistanceRotate(m_Angle);
            // 최대 각도를 벗어나지 않았을때만 회전한다.
            if ((distanceAngle >= (360.0f - MaxRotate) || distanceAngle <= MaxRotate))
            {
                // 각도로 좌표를 구합니다.
                Vector2 vec2 = RPGDefine.getAngleDistancePosition(m_Angle, 1);
                // 플레이어가 레이저의 사거리 내로 들어왔는지 체크합니다.
                RaycastHit2D rayHit = Physics2D.Raycast(BulletPosition, -vec2, 1200, 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Ground"));
                if (rayHit.transform && rayHit.transform.CompareTag("Player"))
                {
                    m_IsReady = true;
                    StartCoroutine("cShot");
                }
            }
            //RotateObject.localEulerAngles = new Vector3(0, 0, distanceAngle);

            //Debug.Log("angle = " + angle);
        }
    }

    /// <summary>
    /// 해당 각도에서 해당 거리만큼 떨어진 좌표를 구합니다.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    Vector2 getAnglePosition(Vector2 from, Vector2 to, float distance=1)
    {
        float angle = RPGDefine.getAngle(from, to);
        return -RPGDefine.getAngleDistancePosition(angle, distance);
    }

    /// <summary>
    /// 레이저 발사 시작.
    /// </summary>
    /// <returns></returns>
    IEnumerator cShot()
    {
        Vector2 BulletPosition = RotateTransform.position;
        Vector2 playerPosition = GameManager.Instance.player.transform.position;
        RaycastHit2D rayHit;
        // 플레이어 위치를 찾아서 첫 레이저 발사위치를 잡아줍니다.
        // 플레이어의 왼쪽을 기준으로 찾아내고 왼쪽의 위치가 충돌되면 위쪽 위치를 찾습니다.
        Vector2 movePosition;
        bool isMoveX =  true;
        switch (Base.BlockData.RotateWay)
        {
            case BlockBase.eBlockRotateWay.Left:
            case BlockBase.eBlockRotateWay.Right:
                isMoveX = false;
                break;
        }
		if ( isMoveX )
		{
			movePosition = playerPosition + new Vector2( FirstPosition / GameConfig.PixelsPerMeter, 0 );

		}
		else
		{
			movePosition = playerPosition + new Vector2( 0, -FirstPosition / GameConfig.PixelsPerMeter );
		}
        float cancelTimer = CancelTimer;
        while (m_IsReady)
        {
            playerPosition = GameManager.Instance.player.transform.position;
            // 플레이어가 사거리에 있는지 확인.
            //bool isSightPlayer = false;
            // 플레이어가 CancelTimer동안 시야에서 벗어나면 발사를 취소합니다.
            rayHit = Physics2D.Raycast(BulletPosition, getAnglePosition(BulletPosition, playerPosition), 1200,
                1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Ground"));
            if (rayHit.transform && rayHit.transform.CompareTag("Player"))
            {
                cancelTimer = CancelTimer;
                //isSightPlayer = true;
            }
            else
            {
                cancelTimer -= Time.deltaTime;
                if (cancelTimer <= 0.0f)
                    break;
            }
            m_Angle = RPGDefine.getAngle(BulletPosition, movePosition);
            float distanceAngle = Base.BlockData.DistanceRotate(m_Angle);
            RotateTransform.localEulerAngles = new Vector3(0, 0, distanceAngle);
            // 각도로 좌표를 구합니다.
            Vector2 vec2 = RPGDefine.getAngleDistancePosition(m_Angle, 1);
            // 레이저를 벽의 위치에 맞춰서 길이를 변경합니다.
            rayHit = Physics2D.Raycast(BulletPosition, -vec2, 1200, 1 << LayerMask.NameToLayer("Ground"));
			if ( rayHit.transform )
			{
				boxColliderBullet.enabled = true;
				SpriteBullet.dimensions = new Vector2( 24, rayHit.distance * GameConfig.PixelsPerMeter );
				boxColliderBullet.size = new Vector2( 1.2f, ( SpriteBullet.dimensions.y * 0.1f ) /2f );
				boxColliderBullet.offset = new Vector2( 0f, boxColliderBullet.size.y / 2f );
			}
			else
			{
				SpriteBullet.dimensions = new Vector2( 0, 0 );
				boxColliderBullet.size = new Vector2( 0f, 0f );
				boxColliderBullet.offset = new Vector2( 0f, 0f );
				boxColliderBullet.enabled = false;
			}
            float playerAngle = RPGDefine.getAngle(BulletPosition, playerPosition);
            float bulletSpeed = BulletSpeedNear;
            if (Mathf.Abs(m_Angle - playerAngle) > SlowAngle)
                bulletSpeed = BulletSpeedFar;
            if (isMoveX)
            {
                if (movePosition.x > playerPosition.x)
                    bulletSpeed = -bulletSpeed;
                movePosition += new Vector2(bulletSpeed / GameConfig.PixelsPerMeter * Time.deltaTime, 0);
            }
            else
            {
                if (movePosition.y < playerPosition.y)
                    bulletSpeed = -bulletSpeed;
                movePosition += new Vector2(0, -bulletSpeed / GameConfig.PixelsPerMeter * Time.deltaTime);
            }
            yield return null;
        }
        SpriteBullet.dimensions = new Vector2(0, 0);
		boxColliderBullet.size = new Vector2( 0f, 0f );
		boxColliderBullet.offset = new Vector2( 0f, 0f );
		boxColliderBullet.enabled = false;
        // 다음 발사까지의 대기시간까지 대기합니다.
        yield return new WaitForSeconds(WaitTimer);
        m_IsReady = false;
    }




}
