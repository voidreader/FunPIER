using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RPGScroll : tk2dUIBaseDemoController
{
    /// <summary>
    /// 프리팹아이템
    /// </summary>
    [Tooltip("프리팹 아이템")]
    public tk2dUILayout m_prefabitem;

    [Tooltip("최상단에 노출될 프리팹 아이템")]
    public tk2dUILayout m_prefabItemFirst;
    GameObject m_FirstItem;

    /// <summary>
    /// 객체사이 간격 
    /// </summary>
    [Tooltip("객체 사이 간격")]
    float m_itemStride = 0;
    [Tooltip("첫 아이템 객체 사이 간격")]
    float m_itemStrideFirst = 0;
    /// <summary>
    /// 스크롤 에어리어 객체
    /// </summary>
    [Tooltip("스크롤 에어리어 객체")]
    public tk2dUIScrollableArea m_scrollableArea;
    /// <summary>
    /// 영역내에 보여질 객체 리스트
    /// </summary>
    [Tooltip("영역내에 보여질 객체 리스트")]
    List<Transform> m_cachedContentList = new List<Transform>();
    /// <summary>
    /// 영역밖에 숨겨둘 객체 리스트
    /// </summary>
    [Tooltip("영역밖에 숨겨둘 객체 리스트")]
    List<Transform> m_unusedContentList = new List<Transform>();
    /// <summary>
    /// 제일 위에 보여질 데이터 인덱스
    /// </summary>
    [Tooltip("제일 위에 보여질 데이터 인덱스")]
    int m_firstCachedItem = -1;
    /// <summary>
    /// 데이터 갯수
    /// </summary>
    [Tooltip("데이터 갯수")]
    int m_maxVisibleItems = 0;
    /// <summary>
    /// 스크롤데이터 리스트
    /// </summary>
    [Tooltip("스크롤데이터 리스트")]
    public List<object> m_allItems = new List<object>();

    void OnEnable()
    {
        DoSetActive(m_prefabitem.transform, false);
        m_scrollableArea.OnScroll += OnScroll;
    }
    void OnDisable()
    {
        m_scrollableArea.OnScroll -= OnScroll;
    }

    /////////////DJ 수정 : 프리팹아이템 변경시 리스트 클리어/////////
    public void ListClear(bool reArrange = true)//reArrange false 면 아이템 정렬 안한다. 
    {
        //Debug.Log("리스트 클리어"); 
        for (int i = 0; i < m_cachedContentList.Count; i++)
        {
            GameObject.Destroy(m_cachedContentList[i].gameObject);
        }
        m_cachedContentList.Clear();
        for (int i = 0; i < m_unusedContentList.Count; i++)
        {
            GameObject.Destroy(m_unusedContentList[i].gameObject);
        }
        m_unusedContentList.Clear();

        m_allItems.Clear();
        if (reArrange)
        {
            m_scrollableArea.Value = 0;
        }
        //m_scrollableArea.contentContainer.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// 스크롤 아이템 정렬
    /// </summary>
    public void ArrangeScrollItems()
    {
        m_scrollableArea.Value = 0;
    }
    ///////////////////////////////////////////////////////////////


    /// <summary>
    /// 스크롤 이닛 부분
    /// </summary>
    /// <param name="dic_"></param>
    public void Init(IList list_)
    {
        //Debug.Log("리스트 이닛 : " + m_allItems.Count); 
        for (int i = 0; i < list_.Count; i++) m_allItems.Add(list_[i]);
        Create(m_allItems.Count);
    }

    /// <summary>
    /// first item 갱신 추가.
    /// </summary>
    /// <param name="obj"></param>
    public void setFirstItem(object obj)
    {
        if (m_FirstItem != null)
            m_FirstItem.GetComponent<UIScrollPrefab>().Parsing(obj);
    }

    /// <summary>
    /// 객체 생성 및 데이터 갯수 셋팅 
    /// </summary>
    /// <param name="datacount_"></param>
    void Create(int datacount_)    
    {        
        //Y axis  __ X Axis 시 수정할 부분 
        float stride = 0;
        if (m_prefabItemFirst != null && m_FirstItem == null)
        {
            // YSY::첫아이템이 있는 경우에 추가. 임시.
            DoSetActive(m_prefabItemFirst.transform, false);
            m_itemStrideFirst = (m_prefabItemFirst.GetMaxBounds() - m_prefabItemFirst.GetMinBounds()).y;
            tk2dUILayout layout = Instantiate(m_prefabItemFirst) as tk2dUILayout;
            m_FirstItem = layout.gameObject;
            layout.transform.parent = m_scrollableArea.contentContainer.transform;
            layout.transform.localPosition = new Vector3(0, stride, 0);
            DoSetActive(layout.transform, true);
            stride -= m_itemStrideFirst;
        }
        // YSY::밖으로 빠져나가서 삭제가 되지 않은 문제점이 발생하므로 주석처리 합니다.
        //m_prefabitem.transform.parent = null;
        DoSetActive(m_prefabitem.transform, false);
        if (m_scrollableArea.scrollAxes == tk2dUIScrollableArea.Axes.XAxis)
            m_itemStride = (m_prefabitem.GetMaxBounds() - m_prefabitem.GetMinBounds()).x;
        else
            m_itemStride = (m_prefabitem.GetMaxBounds() - m_prefabitem.GetMinBounds()).y;
        m_maxVisibleItems = Mathf.CeilToInt(m_scrollableArea.VisibleAreaLength / m_itemStride) + 2;//기본 넓이에 생성될 객체  + 추가로 생성할 객체 

        for (int i = 0; i < m_maxVisibleItems; ++i)
        {
            tk2dUILayout layout = Instantiate(m_prefabitem) as tk2dUILayout;
            //layout.gameObject.name = i.ToString();  //Test
            layout.transform.parent = m_scrollableArea.contentContainer.transform;
            //YSY::이름 수정
            layout.transform.name = i + "_" + layout.transform.name;
            DoSetActive(layout.transform, false);
            m_unusedContentList.Add(layout.transform);
            if (m_scrollableArea.scrollAxes == tk2dUIScrollableArea.Axes.XAxis)
            {
                layout.transform.localPosition = new Vector3(-stride, 0, 0);
                stride -= m_itemStride;
            }
            else
            {
                layout.transform.localPosition = new Vector3(0, stride, 0);
                stride -= m_itemStride;
            }
        }
        SetItemCount(datacount_);
    }




    /// <summary>
    /// 오브젝트 내용변경 및 위치값 수정 
    /// </summary>
    /// <param name="contentRoot_"></param>
    /// <param name="itemId_"></param>
    void CustomizeListObject(Transform contentRoot_, int itemId_)
    {
        //오브젝트 셋팅 및 오브젝트 포지션 설정 
        UIScrollPrefab ScrollPrefab = contentRoot_.GetComponent<UIScrollPrefab>();
        if (ScrollPrefab != null)
        {
            ScrollPrefab.Parsing(m_allItems[itemId_]);
            ScrollPrefab.m_Index = itemId_;
        }
        //YSY::첫아이템 셋팅 추가.
        float stride = 0;
        if (m_prefabItemFirst != null)
        {
            //if (itemId_ > 0)
            stride = -((itemId_) * m_itemStride) - m_itemStrideFirst;
        }
        else
            stride = -(itemId_ * m_itemStride);
        if (m_scrollableArea.scrollAxes == tk2dUIScrollableArea.Axes.XAxis)
            contentRoot_.localPosition = new Vector3(-stride, 0, 0);
        else
            contentRoot_.localPosition = new Vector3(0, stride, 0);
    }

    /// <summary>
    /// 중간 리스트에 데이터 추가될 경우 사용할 함수 
    /// </summary>
    /// <param name="numItems"></param>
    void SetItemCount(int numItems)
    {
        if (numItems < m_allItems.Count)
        {
            m_allItems.RemoveRange(numItems, m_allItems.Count - numItems);
        }
        //else
        //{
        //    for (int j = allItems.Count; j < numItems; ++j)
        //    {

        //    }
        //}   //---------------- 스크롤중 데이터가 추가될때 ! 
        UpdateListGraphics();
    }

    void OnScroll(tk2dUIScrollableArea scrollableArea)
    {
        UpdateListGraphics();
    }

    /// <summary>
    /// 재사용 오브젝트 위치 변경 , 활성화 관리
    /// </summary>
    public void UpdateListGraphics()
    {
        float previousOffset = m_scrollableArea.Value * (m_scrollableArea.ContentLength - m_scrollableArea.VisibleAreaLength);
        float newContentLength = m_allItems.Count * m_itemStride;
        //YSY::첫아이템 셋팅 추가.
        previousOffset -= m_itemStrideFirst;
        if (previousOffset < 0)
            previousOffset = 0;
        newContentLength += m_itemStrideFirst;
        int firstVisibleItem = Mathf.FloorToInt(previousOffset / m_itemStride);

        //YSY::스크롤 스프링 효과 추가.
        //if (!Mathf.Approximately(newContentLength, m_scrollableArea.VisibleAreaLength))
        if (!Mathf.Approximately(newContentLength, m_scrollableArea.ContentLength))
        {
            if (newContentLength < m_scrollableArea.VisibleAreaLength)
            {
                m_scrollableArea.Value = 0;
                for (int i = 0; i < m_cachedContentList.Count; ++i)
                {
                    DoSetActive(m_cachedContentList[i], false);
                    m_unusedContentList.Add(m_cachedContentList[i]);
                }
                m_cachedContentList.Clear();
                m_firstCachedItem = -1;
                firstVisibleItem = 0;
            }
            m_scrollableArea.ContentLength = newContentLength;

            if (m_scrollableArea.ContentLength > 0)
            {
                m_scrollableArea.Value = previousOffset / (m_scrollableArea.ContentLength - m_scrollableArea.VisibleAreaLength);
            }
        }
        int lastVisibleItem = Mathf.Min(firstVisibleItem + m_maxVisibleItems, m_allItems.Count);

        while (m_firstCachedItem >= 0 && m_firstCachedItem < firstVisibleItem)
        {
            m_firstCachedItem++;
            DoSetActive(m_cachedContentList[0], false);
            m_unusedContentList.Add(m_cachedContentList[0]);
            m_cachedContentList.RemoveAt(0);
            if (m_cachedContentList.Count == 0)
            {
                m_firstCachedItem = -1;
            }
        }

        while (m_firstCachedItem >= 0 && (m_firstCachedItem + m_cachedContentList.Count) > lastVisibleItem)
        {
            DoSetActive(m_cachedContentList[m_cachedContentList.Count - 1], false);
            m_unusedContentList.Add(m_cachedContentList[m_cachedContentList.Count - 1]);
            m_cachedContentList.RemoveAt(m_cachedContentList.Count - 1);
            if (m_cachedContentList.Count == 0)
            {
                m_firstCachedItem = -1;
            }
        }

        if (m_firstCachedItem < 0)
        {
            m_firstCachedItem = firstVisibleItem;
            int maxToAdd = Mathf.Min(m_firstCachedItem + m_maxVisibleItems, m_allItems.Count);
            for (int i = m_firstCachedItem; i < maxToAdd; ++i)
            {
                Transform t = m_unusedContentList[0];
                m_cachedContentList.Add(t);
                m_unusedContentList.RemoveAt(0);
                CustomizeListObject(t, i);
                DoSetActive(t, true);
            }
        }
        else
        {
            // Fill in items that should be visible but aren't
            while (m_firstCachedItem > firstVisibleItem)
            {
                --m_firstCachedItem;
                Transform t = m_unusedContentList[0];
                m_unusedContentList.RemoveAt(0);
                m_cachedContentList.Insert(0, t);
                CustomizeListObject(t, m_firstCachedItem);
                DoSetActive(t, true);
            }
            while (m_firstCachedItem + m_cachedContentList.Count < lastVisibleItem)
            {
               
                Transform t = m_unusedContentList[0];
                m_unusedContentList.RemoveAt(0);
                CustomizeListObject(t, m_firstCachedItem + m_cachedContentList.Count);
                m_cachedContentList.Add(t);
                DoSetActive(t, true);
            }
        }
    }

    /// <summary>
    /// YSY::해당 오브젝트의 인덱스를 찾아서 리턴합니다.
    /// -1이 반환되면 해당 오브젝트가 없는 경우 입니다.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int FindIndex(object obj)
    {
        return m_allItems.IndexOf(obj);
    }

    /// <summary>
    /// YSY::해당 인덱스의 오브젝트를 갱신합니다.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="obj"></param>
    public void setData(int index, object obj)
    {
        m_allItems[index] = obj;
        // 해당 인덱스의 아이템을 찾아서 갱신합니다.
        Transform Item = m_cachedContentList.Find(x => x.GetComponent<UIScrollPrefab>().m_Index == index);
        if (Item != null)
            Item.GetComponent<UIScrollPrefab>().Parsing(obj);
    }


    /// <summary>
    /// 스크롤 데이터가 추가될때
    /// </summary>
    /// <param name="obj_"></param>
    public void Add(object obj_)
    {
        m_allItems.Add(obj_);
        UpdateListGraphics();
    }
    /// <summary>
    /// 스크롤 데이터 리무브 
    /// </summary>
    /// <param name="count_"></param>
    public void Remove(int count_)
    {
        if (m_allItems.Count > count_)
        {
            m_allItems.Remove(m_allItems[count_]);
            UpdateListGraphics();
        }
    }
    /// <summary>
    /// 오브젝트 배열주소 찾을때
    /// </summary>
    /// <param name="obj_"></param>
    /// <returns></returns>
    public int FindObjCount(object obj_)
    {
        int count = -1;
        for(int i=0; i < m_allItems.Count; i++)
        {
            if(obj_ == m_allItems[i])
            {
                count = i;
                break;
            }
        }
        return count;
    }
    

    public void ListUpdate(IList updatelist_)
    {
        ListClear(false);
        for (int i = 0; i < updatelist_.Count; i++) m_allItems.Add(updatelist_[i]);
        Create(m_allItems.Count);
    }

    public void ListUpdate(IList updatelist_, bool tf_)
    {
        ListClear(tf_);
        for (int i = 0; i < updatelist_.Count; i++) m_allItems.Add(updatelist_[i]);
        Create(m_allItems.Count);
    }


    public Transform GetSlotItem(int num_)
    {
        if (m_cachedContentList.Count > num_) return m_cachedContentList[num_];
        return null;
    }

    public int GetItemCount()
    {
        return m_allItems.Count;
    }

    public int GetCachedItemCount()
    {
        return m_cachedContentList.Count;
    }

    /// <summary>
    /// 스크롤 위치 코드로 조정 value = 0 ~ 1f;
    /// </summary>
    /// <param name="value_">0~1f</param>
    public void SetAreaValue(float value_)
    {
        m_scrollableArea.Value = value_;
        UpdateListGraphics();
    }
}
