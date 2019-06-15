using UnityEngine;
using System.Collections;

public class cannon_bullet : MonoBehaviour {

    /// <summary>
    /// 초당 이동되는 Vector.
    /// </summary>
    Vector3 m_MovePos;
    /// <summary>
    /// 발사체가 발사중인지?
    /// </summary>
    //bool m_IsShot = false;

    float m_removeTime = 0.0f;

    Transform m_trans;

    /// <summary>
    /// xy 방향 좌표와 speed(초당 이동되는 픽셀)로 발사한다.
    /// </summary>
    /// <param name="vec2"></param>
    /// <param name="speed"></param>
    public void shot(Vector2 vec2, float speed)
    {
        m_trans = transform;
        m_MovePos = new Vector3(vec2.x * speed, vec2.y * speed);
        m_removeTime = (1136.0f / GameConfig.PixelsPerMeter) / speed;
        //m_IsShot = true;
        StartCoroutine("cShot");
    }
    /*
    void Update()
    {
        if (m_IsShot)
        {
            m_trans.localPosition += m_MovePos * Time.deltaTime;
            m_removeTime -= Time.deltaTime;
            if (m_removeTime <= 0.0f)
                remove();
        }
    }
    */
    IEnumerator cShot()
    {
        while (true)
        {
            m_trans.localPosition += m_MovePos * Time.deltaTime;
            m_removeTime -= Time.deltaTime;
            if (m_removeTime <= 0.0f)
                break;
            yield return null;
        }
        remove();
    }

    void remove()
    {
        //DestroyImmediate(gameObject);
        Destroy(gameObject);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("OnTriggerEnter2D cannon_bullet = " + other.tag);
        // 벽에 닿으면 제거됨.
        if (other.CompareTag("out_ground"))
        {
            object_03_cannon cannon = GetComponentInParent<object_03_cannon>();
            cannon.ShowEffect(transform.position);
            remove();
        }
    }
    

}
