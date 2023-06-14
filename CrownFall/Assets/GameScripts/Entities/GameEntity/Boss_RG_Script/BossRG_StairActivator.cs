using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class BossRG_StairActivator : MonoBehaviour
{
	//---------------------------------------
	private static bool _secondFloorActivated = false;
	public static bool SecondFloorActivated { get { return _secondFloorActivated; } }

	private static bool _thirdCamWallActivated = false;
	public static bool ThirdCamWallActivated { get { return _thirdCamWallActivated; } }

	//---------------------------------------
	[SerializeField]
	private HT.PhysicEventer _physEventer_1F = null;
	[SerializeField]
	private HT.PhysicEventer _physEventer_2F = null;
	[SerializeField]
	private HT.PhysicEventer _stairEventer_1 = null;
	[SerializeField]
	private HT.PhysicEventer _stairEventer_2 = null;
	[SerializeField]
	private Camera _thirdCamera = null;
	[SerializeField]
	private Camera _gameCamera = null;
	[SerializeField]
	private GameObject _thirdCamWallObj = null;
	[SerializeField]
	private GameObject _activateTarget = null;
	[SerializeField]
	private GameObject _secondFloor = null;

	//---------------------------------------
	private IActorBase _playerActor = null;
	private bool _enteredStair = false;


	//---------------------------------------
	public void Awake()
	{
		_secondFloorActivated = false;
		_thirdCamera.gameObject.SetActive(false);

		_physEventer_1F.Init(null, OnExitFloor1);
		_physEventer_2F.Init(null, OnExitFloor2);

		_stairEventer_1.Init(OnEnterStair, OnExisStair);
		_stairEventer_2.Init(OnEnterStair, OnExisStair);
	}

	public void Update()
	{
		if (_playerActor == null && BattleFramework._Instance != null)
			_playerActor = BattleFramework._Instance.m_pPlayerActor;

		if (_thirdCamera.gameObject.activeInHierarchy)
		{
			Vector3 vView = (_playerActor.transform.position - _thirdCamera.transform.position).normalized;
			_thirdCamera.transform.forward = vView;
		}
	}

	//---------------------------------------
	public void OnExitFloor1(GameObject pObj)
	{
		if (BattleFramework._Instance.m_pPlayerActor.gameObject != pObj)
			return;

		if (_enteredStair)
		{
			_gameCamera.gameObject.SetActive(false);

			_thirdCamera.gameObject.SetActive(true);
			_activateTarget.gameObject.SetActive(true);

			_thirdCamWallObj.SetActive(true);
			_secondFloor.SetActive(true);
		}
		else
		{
			_gameCamera.gameObject.SetActive(true);

			_thirdCamera.gameObject.SetActive(false);
			_activateTarget.gameObject.SetActive(false);

			_thirdCamWallObj.SetActive(false);
			_secondFloor.SetActive(false);

			//-----
			_secondFloorActivated = false;
		}

		_thirdCamWallActivated = _thirdCamWallObj.activeInHierarchy;
	}

	public void OnExitFloor2(GameObject pObj)
	{
		if (BattleFramework._Instance.m_pPlayerActor.gameObject != pObj)
			return;

		if (_enteredStair)
		{
			_gameCamera.gameObject.SetActive(false);

			_thirdCamera.gameObject.SetActive(true);
			_activateTarget.gameObject.SetActive(true);

			_thirdCamWallObj.SetActive(true);
			_secondFloor.SetActive(true);
		}
		else
		{
			_gameCamera.gameObject.SetActive(true);

			_thirdCamera.gameObject.SetActive(false);
			_activateTarget.gameObject.SetActive(false);

			_thirdCamWallObj.SetActive(true);
			_secondFloor.SetActive(true);

			//-----
			_secondFloorActivated = true;
		}

		_thirdCamWallActivated = _thirdCamWallObj.activeInHierarchy;
	}

	//---------------------------------------
	private void OnEnterStair(GameObject pObj)
	{
		if (BattleFramework._Instance.m_pPlayerActor.gameObject == pObj)
			_enteredStair = true;
	}

	private void OnExisStair(GameObject pObj)
	{
		if (BattleFramework._Instance.m_pPlayerActor.gameObject == pObj)
			_enteredStair = false;
	}
}


/////////////////////////////////////////
//---------------------------------------