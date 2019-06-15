using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BlockBase))]
public class object_03_cannon : RPGLayer
{
    public BlockBase Base { get; private set; }

    [Header("최대 회전 값 +- 적용")]
    public float MaxRotate = 45.0f;

    [Header("회전값이 들어갈 Transform")]
    public Transform RotateTransform;
    [Header("원본 발사체 오브젝트")]
    public GameObject BulletObject;
    [Header("발사체가 올라갈 상위 Transform")]
    public Transform BulletParent;
    [Header("발사체가 벽에 맞았을때 나오는 이펙트")]
    public GameObject BulletEffectBomb;

    [Header("발사체의 이동 속도 초당 픽셀수")]
    public float BulletSpeed = 300.0f;
    [Header("인게임이 시작되고 첫 발사까지의 시간.")]
    public float BulletFirstTimer = 2.0f;
    [Header("발사체가 발사될 시간 주기(초)")]
    public float BulletTimer = 3.0f;

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
            StopCoroutine("cShot");
            RotateTransform.localEulerAngles = new Vector3(0, 0, 0);
            // 현재 생성된 모든 발사체를 제거합니다.
            removeAllChild(BulletParent, false);
            if (GameManager.Instance.IsPlaying)
                StartCoroutine("cShot");
        };
        //StartCoroutine("cTest");
    }

    void Update()
    {
        if (GameManager.Instance.IsPlaying)
        {
            m_Angle = RPGDefine.getAngle(transform.position, GameManager.Instance.player.transform.position);
            float distanceAngle = Base.BlockData.DistanceRotate(m_Angle);
            
            // 각도로 좌표를 구합니다.
            Vector2 vec2 = RPGDefine.getAngleDistancePosition(m_Angle, 1);

            // 플레이어와 대포 사이에 벽이 있는지 체크합니다.
            // 벽이 있으면 발사하지 않도록 합니다.
            bool IsBlocked = false;
            RaycastHit2D rayHit = Physics2D.Raycast(transform.position, -vec2, 1000, LayerMask.GetMask("Ground","Player"));
            if (rayHit.transform && rayHit.transform.CompareTag("ground"))
            {                
                //Debug.Log(Base.BlockData.RotateWay + " rayHit name = " + rayHit.transform.name);
                IsBlocked = true;
            }

            // 최대 각도를 벗어나지 않았을때만 회전한다.
            if ((distanceAngle >= (360.0f - MaxRotate) || distanceAngle <= MaxRotate) && !IsBlocked)
            {
                RotateTransform.localEulerAngles = new Vector3(0, 0, distanceAngle);
                m_IsReady = true;
            }
            else
            {
                RotateTransform.localEulerAngles = new Vector3(0, 0, 0);
                m_IsReady = false;
            }
            //RotateObject.localEulerAngles = new Vector3(0, 0, distanceAngle);

            //Debug.Log("angle = " + angle);
        }
    }

    IEnumerator cShot()
    {
        // 게임이 시작되고 첫 대기시간.
        yield return new WaitForSeconds(BulletFirstTimer);
        while (true)
        {
            if (m_IsReady)
            {
                float bulletAngle = Base.BlockData.BulletRotate(m_Angle);

                // 각도로 좌표를 구합니다.
                Vector2 vec2 = RPGDefine.getAngleDistancePosition(bulletAngle, 1);

                GameObject copyObject = addChild(BulletObject, BulletParent);
                copyObject.GetComponent<cannon_bullet>().shot(vec2, BulletSpeed / GameConfig.PixelsPerMeter);
            }
            // 미사일 발사 후 다음 미사일이 발사될 시간.
            yield return new WaitForSeconds(BulletTimer);
        }
    }

    public void ShowEffect(Vector3 position)
    {
        GameObject obj = addChild(BulletEffectBomb);
		obj.gameObject.SetActive( true );
        obj.transform.position = position;

        StartCoroutine(cShowEffect(obj, 1.0f));
    }

    IEnumerator cShowEffect(GameObject obj, float showTime)
    {
        obj.SetActive(true);
        yield return new WaitForSeconds(showTime);
        removeChild(obj);
    }
    
    IEnumerator cTest()
    {
        yield return new WaitForSeconds(2.0f);

        while (true)
        {
            float angle = RPGDefine.getAngle(transform.position, GameManager.Instance.player.transform.position);
            float bulletAngle = Base.BlockData.BulletRotate(angle);
            //float bulletAngle = angle - Base.BlockData.Rotate();


            float radian = bulletAngle * Mathf.Deg2Rad;
            float tx = Mathf.Sin(radian) * 10.0f;
            float ty = Mathf.Cos(radian) * 10.0f;
            Debug.Log(Base.BlockData.RotateWay + " tx = " + tx + " ty = " + ty + " angle = " + angle + " bulletAngle = " + bulletAngle);

            BulletObject.transform.localPosition = new Vector3(tx, ty, 0);

            yield return new WaitForSeconds(1.0f);
        }
    }
    



}
