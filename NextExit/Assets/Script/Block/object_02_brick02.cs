using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BlockBase))]
public class object_02_brick02 : MonoBehaviour
{
    [Header("블럭이 표시되는 거리(플레이어와의 간격)")]
    public float ShowDistance = 4.0f;

    public BlockBase Base { get; private set; }
    bool IsShow = false;

    void Awake()
    {
        Base = GetComponent<BlockBase>();
        if (GameManager.Instance.IsPlaying)
            Base.ImageTransform.gameObject.SetActive(IsShow);
    }

    void Update()
    {
        if (GameManager.Instance.IsPlaying)
        {
            float distance = Vector2.Distance(transform.position, GameManager.Instance.player.transform.position);
            //Debug.Log("distance = " + distance);
            if (IsShow && distance > ShowDistance)
            {
                IsShow = false;
                Base.ImageTransform.gameObject.SetActive(IsShow);
            }
            else if (!IsShow && distance < ShowDistance)
            {
                IsShow = true;
                Base.ImageTransform.gameObject.SetActive(IsShow);
            }
        }
    }




}
