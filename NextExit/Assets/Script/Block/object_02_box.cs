using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BlockBase))]
public class object_02_box : MonoBehaviour
{
    public BlockBase Base { get; private set; }
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
                m_IsOverlap = false;
                StopCoroutine("cOverlap");
                gameObject.SetActive(true);
            }
        };
    }


    void Update()
    {
        if (GameManager.Instance.IsPlaying)
        {
            if (!m_IsOverlap && GameManager.Instance.player.groundCheck_Down_Left != null)
            {
                if (m_Collider.OverlapPoint(GameManager.Instance.player.groundCheck_Down_Left.transform.position) ||
                    m_Collider.OverlapPoint(GameManager.Instance.player.groundCheck_Down_Right.transform.position))
                {
                    m_IsOverlap = true;
                    //Debug.Log("Overlap");
                    StartCoroutine("cOverlap");
                }
            }
        }
    }

    IEnumerator cOverlap()
    {
        yield return new WaitForSeconds(1.0f);

        float totalTime = 3.0f;
        float t = 0.0f;
        Vector3 nowPosition = transform.localPosition;
        Vector3 movePosition = nowPosition + new Vector3(0, -32, 0);

        while (totalTime >= t)
        {
            t += Time.deltaTime;

            Vector3 position = Vector3.Lerp(nowPosition, movePosition, t);
            transform.localPosition = position;
            yield return null;
        }
        //DestroyImmediate(this);
        gameObject.SetActive(false);
        //transform.localPosition = nowPosition;
    }



}
