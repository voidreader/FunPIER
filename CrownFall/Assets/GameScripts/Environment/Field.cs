using UnityEngine;
using System.Collections;


/////////////////////////////////////////
//---------------------------------------
public class Field : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("FIELD SETTINGS")]
	public FieldEvent[] m_vFieldEvents;
	public LevelLighting m_pLevelLighting;


	/////////////////////////////////////////
	//---------------------------------------
	[Header("FIELD OBJECTS")]
	public GameObject m_pMainFieldGround;
	[SerializeField]
	private GameObject m_pLobbyOnlyObjects = null;
	[SerializeField]
	private DummyPivot[] _dummyPivots = null;


	/////////////////////////////////////////
	//---------------------------------------
	[Header("INSTANCE INFO")]
	public bool m_bIsLobbyField;
	public Animation _fieldAnimator = null;
	public string _battleStartSeqAnimName = null;

	//---------------------------------------
	[Header("OBJECTS")]
	public GameObject m_pPlayerStartPos;
	public GameObject m_pPlayerLobbyPos;
	public GameObject m_pBossStartPos;

	//---------------------------------------
	public Camera m_pLobbyCamera;
	public Camera m_pGameCamera;

	//---------------------------------------
	public LightManager[] m_pControllableDyanmicLights;


	/////////////////////////////////////////
	//---------------------------------------
	private void Awake()
	{
		OnAwake();
	}

	protected virtual void OnAwake()
	{
	}

	public virtual void Init()
	{
		for (int nInd = 0; nInd < m_pControllableDyanmicLights.Length; ++nInd)
			m_pControllableDyanmicLights[nInd].Init();

		//-----
		m_pLobbyCamera.gameObject.SetActive((m_bIsLobbyField) ? true : false);
		m_pGameCamera.gameObject.SetActive((m_bIsLobbyField) ? false : true);

		//-----
		if (m_pLobbyOnlyObjects != null)
			m_pLobbyOnlyObjects.SetActive(m_bIsLobbyField);

		m_pLevelLighting.EnableLighting(m_bIsLobbyField);

		//-----
		if (m_bIsLobbyField == false && string.IsNullOrEmpty(_battleStartSeqAnimName) == false)
			_fieldAnimator.Play(_battleStartSeqAnimName);
	}

	//---------------------------------------
	private void Update()
	{
		Frame();
	}

	protected virtual void Frame()
	{

	}

	//---------------------------------------
	private void FixedUpdate()
	{
		FixedFrame();
	}

	protected virtual void FixedFrame()
	{

	}

	/////////////////////////////////////////
	//---------------------------------------
	public virtual void OnBattleStart()
	{
	}

	public virtual void OnBattleEnd(bool bIsPlayerWin)
	{

	}

	/////////////////////////////////////////
	//---------------------------------------
	public GameObject FindDummyPivot(string szName)
	{
		if (_dummyPivots == null)
			return null;

		for (int nInd = 0; nInd < _dummyPivots.Length; ++nInd)
			if (_dummyPivots[nInd].name == szName)
				return _dummyPivots[nInd].gameObject;

		return null;
	}

	public DummyPivot[] GetDummyPivots()
	{
		return _dummyPivots;
	}
}
