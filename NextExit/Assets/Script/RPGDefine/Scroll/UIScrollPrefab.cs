using UnityEngine;
using System.Collections;

public class UIScrollPrefab : MonoBehaviour {


    /// <summary>
    /// 오브젝트 할당플래그
    /// </summary>
    [Tooltip("오브젝트 할당플래그")]
    [System.NonSerialized]
    public bool m_FindObjFlag = false;

    /// <summary>
    /// 스크롤아이템인덱스
    /// </summary>
    /// <param name="data_"></param>
    public int m_Index;

    public RPGLayer m_ParentLayer;

    public virtual void Parsing(object data_)
    {
        if (false == m_FindObjFlag) ObjFind();
    }

    public virtual void ObjFind()
    {
        m_FindObjFlag = true;
    }
}
