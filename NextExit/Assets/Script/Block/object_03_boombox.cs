using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BlockBase))]
public class object_03_boombox : MonoBehaviour
{
    public BlockBase Base { get; private set; }
    [Header("발사체 Transform")]
    public Transform BulletTrans;
    [Header("오브첵트가 떨어지는 속도")]
    public float MoveSpeed = 300.0f;

	public GameObject bgItem;

    BoxCollider2D m_Collider;

    bool m_IsOverlap = false;

    void Awake()
    {
        Base = GetComponent<BlockBase>();
        m_Collider = GetComponentInChildren<BoxCollider2D>(true);

        Base.DelegateReset = () =>
        {
            if (m_IsOverlap)
            {
				bgItem.gameObject.SetActive( true );
                m_IsOverlap = false;
                StopCoroutine("cOverlap");
                BulletTrans.gameObject.SetActive(true);
            }
        };
    }


    void Update()
    {
        if (GameManager.Instance.IsPlaying)
        {
            if (!m_IsOverlap && GameManager.Instance.player.groundCheck_Down_Left != null)
            {
                Vector3 overlapPointLeft = new Vector3(GameManager.Instance.player.groundCheck_Down_Left.transform.position.x, BulletTrans.position.y);
                Vector3 overlapPointRight = new Vector3(GameManager.Instance.player.groundCheck_Down_Right.transform.position.x, BulletTrans.position.y);
                if ((m_Collider.OverlapPoint(overlapPointLeft) ||
                    m_Collider.OverlapPoint(overlapPointRight) ) && GameManager.Instance.player.transform.localPosition.y < transform.localPosition.y )
                {
                    m_IsOverlap = true;
					bgItem.gameObject.SetActive( false );
                    //Debug.Log("Overlap");
                    StartCoroutine("cOverlap");
                }
            }
        }
    }

    IEnumerator cOverlap()
    {
        while(true)
        {
            BulletTrans.localPosition -= new Vector3(0, MoveSpeed / GameConfig.PixelsPerMeter * Time.deltaTime, 0);
            yield return null;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnTriggerEnter2D object_03_boombox = " + other.tag);
        // 벽에 닿으면 제거됨.
        if (other.CompareTag("out_ground"))
        {
            StopCoroutine("cOverlap");
            BulletTrans.gameObject.SetActive(false);
        }
    }


}
