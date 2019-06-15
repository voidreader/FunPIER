using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RPGLayer : MonoBehaviour
{
    Transform m_Transform;

    void Awake()
    {
        getTransform();
        init();
    }

    /// <summary>
    /// transform의 속도 개선을 위한 용도입니다.
    /// .transform을 사용하지 말고 이것을 사용하세요.
    /// </summary>
    /// <returns></returns>
    public Transform getTransform()
    {
        if (m_Transform == null)
            m_Transform = transform;
        return m_Transform;
    }
    
    /// <summary>
    /// 화면에 붙을때 Awake에서 호출됩니다.
    /// </summary>
	public virtual void init()
	{
        // 데이터 파싱등 초기화할때 override해서 사용합니다.
	}

    /// <summary>
    /// 데이터가 전달될때 호출됩니다. 데이터 전달이 없으면 호출되지 않습니다.
    /// init()이 먼저 호출되고 init(dictionary)는 이후에 호출됩니다.
    /// </summary>
    /// <param name="_info"></param>
	public virtual void init( Dictionary<string, object> _info )
	{

	}
	
	public void setParent( Transform parent, GameObject _child )
	{
		Vector3 localPos = _child.transform.localPosition;
		Vector3 localScale = _child.transform.localScale;
		
		_child.transform.SetParent(parent);
		
		_child.transform.localPosition = localPos;
		_child.transform.localScale = localScale;
	}
	
	/// <summary>
	/// Adds the child object.
	/// </summary>
	/// <param name="_child">_child.</param>
	public void addChildObject( GameObject _child )
	{
        setParent(getTransform(), _child);
	}
	
    public GameObject addChild( string res )
    {
        return addChild(Resources.Load(res));
    }

	/// <summary>
	/// Adds the child.
	/// </summary>
	/// <returns>The child.</returns>
	/// <param name="child">Child.</param>
	public GameObject addChild( Object child )
	{
		GameObject obj = GameObject.Instantiate(child) as GameObject;
		addChildObject (obj);
		return obj;
	}

    public GameObject addChild(string res, Transform parent)
    {
        return addChild(Resources.Load(res), parent);
    }

	/// <summary>
	/// Adds the child.
	/// </summary>
	/// <returns>The child.</returns>
	/// <param name="child">Child.</param>
	/// <param name="parent">Parent.</param>
	public GameObject addChild( Object child, Transform parent )
	{
		GameObject obj = GameObject.Instantiate(child) as GameObject;
		setParent (parent, obj);
		return obj;
	}

    public GameObject addChild(string res, float x, float y)
    {
        return addChild(Resources.Load(res), x, y);
    }

	/// <summary>
	/// Adds the child position.
	/// </summary>
	/// <returns>The child.</returns>
	/// <param name="child">Child.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public GameObject addChild ( Object child, float x, float y )
	{
        GameObject obj = GameObject.Instantiate(child, getTransform().position + new Vector3(x, y, 0), getTransform().rotation) as GameObject;
		addChildObject (obj);
		return obj;
	}
	
	/// <summary>
	/// 해당 오브젝트가 존재하는지 찾아서 리턴합니다.
	/// 존재하면 true.
	/// </summary>
	/// <returns><c>true</c>, if object was found, <c>false</c> otherwise.</returns>
	/// <param name="obj">Object.</param>
	public bool findObject(GameObject obj, Transform parant_trans = null)
	{
		if (parant_trans == null)
            parant_trans = getTransform();
		if (gameObject.Equals(obj))
			return true;
		for (int i=0; i<parant_trans.childCount; i++)
		{
			GameObject child_object = parant_trans.GetChild(i).gameObject;
			if (child_object.Equals(obj))
				return true;
			if (findObject(obj, child_object.transform))
				return true;
		}
		return false;
	}
	
	/// <summary>
	/// 모든 하위 자식을 지운다.
	/// </summary>
	public void removeAllChild()
	{
        removeAllChild(getTransform());
	}
	
	public void removeAllChild(Transform trans)
	{
        
		while (trans.childCount > 0) {
            int childcount = trans.childCount;
			Transform child = trans.GetChild(0);
			DestroyImmediate(child.gameObject);
            //Destroy(child.gameObject);
            // 제거가 되지 않고 오류 발생.
            if (childcount == trans.childCount)
            {
                Debug.LogError("childcount == trans.childCount");
                break;
            }
		}       
        
        //StartCoroutine("cRemoveAllChild", trans);
	}

    public void removeAllChild(Transform trans, bool Immediate)
    {
        if (Immediate)
            removeAllChild(trans);
        else
            StartCoroutine("cRemoveAllChild", trans);
    }

    IEnumerator cRemoveAllChild(Transform trans)
    {
        yield return new WaitForEndOfFrame();
        while (trans.childCount > 0)
        {
            Transform child = trans.GetChild(0);
            DestroyImmediate(child.gameObject);
        }
    }
	
	/// <summary>
	/// 선택된 자식을 지운다.
	/// </summary>
	/// <param name="child">Child.</param>
	public void removeChild(GameObject child)
	{
		DestroyImmediate(child);
	}
	
	public void removeChildDelay(float delay, GameObject child)
	{
		// 마이너스값이 들어오면 실행하지 않음.
		if (delay < 0.0f)
			return;
		if (delay == 0.0f)
			removeChild(child);
		else
			StartCoroutine(C_removeObject(delay, child));
	}
	
	IEnumerator C_removeObject(float remove_delay, GameObject effect)
	{
		yield return new WaitForSeconds(remove_delay);
		removeChild(effect);
	}
	
	/// <summary>
	/// 나 자신을 지운다.
	/// </summary>
	public void removeFromParent()
	{
		DestroyImmediate(gameObject);
	}
	
	public void btnHide()
	{
		this.gameObject.SetActive (false);
	}
	
	public void btnShow()
	{
		this.gameObject.SetActive (true);
	}
	
	/// <summary>
	/// kjh:: 페이지 안에서 뒤로 이동. 
	/// </summary>
	public virtual bool OnHeaderBtnClick()
	{
		return true;
	}

	public virtual void OnPopupClose()
	{

	}
}
