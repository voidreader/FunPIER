using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class PowerPlant_Wall : MonoBehaviour
{
	//---------------------------------------
	[Header("DOOR")]
	[SerializeField]
	private Animation _doorAnim_Entrance = null;
	[SerializeField]
	private Animation _doorAnim_Exit = null;
	[SerializeField]
	private string _doorAnimName_Open = null;
	[SerializeField]
	private string _doorAnimName_Close = null;

	//---------------------------------------
	[Header("CONVEYOR BELTS")]
	[SerializeField]
	private float _beltSpeed = 3.0f;
	[SerializeField]
	private GameObject[] _beltArrow = null;
	[SerializeField]
	private GameObject[] _beltSpawnPos = null;
	[SerializeField]
	private GameObject _beltThing_Instance = null;
	[SerializeField]
	private float _beltThing_SpawnTime = 4.0f;
	[SerializeField]
	private float _beltArrow_ViewTime = 2.0f;
	[SerializeField]
	private float _beltThing_LifeTime = 20.0f;
	[SerializeField]
	private int _beltThing_FirstCount = 4;
	[SerializeField]
	private PowerPlant_Press _belt_Press = null;
	public PowerPlant_Press Belt_Press { get { return _belt_Press; } }

	//---------------------------------------
	private List<GameObject> _beltUpObjects = new List<GameObject>();
	
	//---------------------------------------
	private void Start()
	{
		for (int nInd = 0; nInd < _beltThing_FirstCount; ++nInd)
		{
			int nIndex = Random.Range(0, _beltSpawnPos.Length);

			GameObject pNewThing = HT.Utils.Instantiate(_beltThing_Instance);
			HT.Utils.SafeDestroy(pNewThing, _beltThing_LifeTime);

			float fDistance = (_beltSpeed * _beltThing_SpawnTime) * nInd;
			Vector3 vStartPos = _beltSpawnPos[nIndex].transform.position;
			pNewThing.transform.position = vStartPos + (Vector3.back * fDistance);

			PowerPlant_Thing pThing = pNewThing.GetComponent<PowerPlant_Thing>();
			pThing._parent = this;
			pThing.Init();
		}

		DoorOpen(true, false);
		DoorOpen(false, false);

		StartCoroutine(CreateBeltThing_Internal());
	}

	private void Update()
	{
		for (int nInd = 0; nInd < _beltUpObjects.Count; ++nInd)
		{
			Vector3 vCurPos = _beltUpObjects[nInd].transform.position;
			vCurPos += Vector3.back * (_beltSpeed * HT.TimeUtils.GameTime);

			_beltUpObjects[nInd].transform.position = vCurPos;
		}
	}

	//---------------------------------------
	public void DoorOpen(bool bEntrance, bool bOpen)
	{
		Animation pAnim = (bEntrance) ? _doorAnim_Entrance : _doorAnim_Exit;
		string pAnimName = (bOpen) ? _doorAnimName_Open : _doorAnimName_Close;
		pAnim.Play(pAnimName);
	}

	public Vector3 GetDoorPosition(bool bEntrance)
	{
		return (bEntrance) ? _doorAnim_Entrance.gameObject.transform.position : _doorAnim_Exit.gameObject.transform.position;
	}

	//---------------------------------------
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<IActorBase>() != null)
			AddBeltUpObject(other.gameObject);
	}

	private void OnTriggerExit(Collider other)
	{
		if (_beltUpObjects.Contains(other.gameObject))
			RemoveBeltUpObject(other.gameObject);
	}

	//---------------------------------------
	public void AddBeltUpObject(GameObject pObj)
	{
		_beltUpObjects.Add(pObj);
	}

	public void RemoveBeltUpObject(GameObject pObj)
	{
		_beltUpObjects.Remove(pObj);
	}

	//---------------------------------------
	IEnumerator CreateBeltThing_Internal()
	{
		yield return new WaitForSeconds(_beltThing_SpawnTime);

		while (true)
		{
			int nIndex = Random.Range(0, _beltSpawnPos.Length);

			GameObject pNewThing = HT.Utils.Instantiate(_beltThing_Instance);
			HT.Utils.SafeDestroy(pNewThing, _beltThing_LifeTime);

			pNewThing.transform.position = _beltSpawnPos[nIndex].transform.position;

			PowerPlant_Thing pThing = pNewThing.GetComponent<PowerPlant_Thing>();
			pThing._parent = this;
			pThing.Init();

			_beltArrow[nIndex].gameObject.SetActive(true);

			yield return new WaitForSeconds(_beltArrow_ViewTime);

			_beltArrow[nIndex].gameObject.SetActive(false);

			yield return new WaitForSeconds(_beltThing_SpawnTime - _beltArrow_ViewTime);
		}
	}

	//---------------------------------------
	public bool IsOnBelt(GameObject pObj)
	{
		return _beltUpObjects.Contains(pObj);
	}
}


/////////////////////////////////////////
//---------------------------------------