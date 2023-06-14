using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class BossVT_StretchRoot : MonoBehaviour
{
	//---------------------------------------
	public Action<BossVT_FlowerBud> onStretchComplete = null;

    //---------------------------------------
    [Header("DEFAULT INFOS")]
    [SerializeField]
    AIActor_Extend_VT _extendVT = null;
    [SerializeField]
	private Transform[] _roots = null;
	[SerializeField]
	private Transform[] _boxes = null;
	[SerializeField]
	private float[] _stretchRatio = null;
	[SerializeField]
	private float _decreasePerDamage = 0.0009f;
	[SerializeField]
	private Animation _rootAnimation = null;
	[SerializeField]
	private string _rootAnim_Remove = null;

	//---------------------------------------
	public Field_Garden.eGardenBud _targetBud = Field_Garden.eGardenBud.E;

	//---------------------------------------
	private float[] _rootsOrigScale = null;
	private float[] _boxesOrigScale = null;

	//---------------------------------------
	private bool _stretchStart = false;
	private bool _stretchProcessEnd = false;
	private float _currentProcess = 0.0f;

	//---------------------------------------
	private void Awake()
	{
		_rootsOrigScale = new float[_roots.Length];
		for (int nInd = 0; nInd < _roots.Length; ++nInd)
			_rootsOrigScale[nInd] = _roots[nInd].localScale.x;

		_boxesOrigScale = new float[_boxes.Length];
		for (int nInd = 0; nInd < _roots.Length; ++nInd)
		{
			_boxesOrigScale[nInd] = _boxes[nInd].localScale.x;

			GameObj_DamageReciver pReciver = _boxes[nInd].GetComponent<GameObj_DamageReciver>();
			if (pReciver != null)
				pReciver.onDamaged += OnDecreaseStretch;
		}

		SetProcess(0.0f);

		//-----
	}

	private void Update()
	{
		if (_stretchProcessEnd)
			return;

		if (_stretchStart)
		{
			int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;
            int nStretchCompleteCount = _extendVT.StretchRootCompleteCount();

			float fDecreaseRatio = 1.0f;
			if (nStretchCompleteCount > 0)
			{
				float[] vStretchRatioDec = _extendVT.GetStrRoots_StretchRatioDec();
				for(int nInd = 0; nInd < nStretchCompleteCount; ++nInd)
				{
					if (vStretchRatioDec.Length <= nInd)
						break;

					fDecreaseRatio -= vStretchRatioDec[nInd];
				}
			}

			_currentProcess += _stretchRatio[nDiff] * fDecreaseRatio * Time.deltaTime;
			if (_currentProcess >= 1.0f)
			{
				_currentProcess = 1.0f;

				_stretchStart = false;
				_stretchProcessEnd = true;

				Field_Garden pGarden = BattleFramework._Instance.m_pField as Field_Garden;

				BossVT_FlowerBud pTargetBud = pGarden.GetBud(_targetBud);

				_rootAnimation.Play(_rootAnim_Remove);
				pTargetBud.Remove();

				HT.Utils.SafeInvoke(onStretchComplete, pTargetBud);
			}

			SetProcess(_currentProcess);
		}
	}

	//---------------------------------------
	public void StretchStart()
	{
		_stretchStart = true;
	}

	public bool StretchProcessing()
	{
		return _stretchStart || _stretchProcessEnd;
	}

	public void PauseStretch()
	{
		_stretchStart = false;
	}

    public bool IsProcessEnd()
    {
        return _stretchProcessEnd;
    }

	//---------------------------------------
	public void OnDecreaseStretch(int nDamage)
	{
		if (_stretchProcessEnd)
			return;

		if (_stretchStart == false)
			return;
		
		float fDecreaseRatio = nDamage * _decreasePerDamage;
		StartCoroutine(DecreaseStretch_Internal(fDecreaseRatio));
	}

	public void OnDecreaseStretch(float fRatio, bool bRelative)
	{
		if (_stretchProcessEnd)
			return;

		if (_stretchStart == false)
			return;

		float fDecreaseRatio = ((bRelative) ? fRatio * _currentProcess : fRatio);
		StartCoroutine(DecreaseStretch_Internal(fDecreaseRatio));
	}

	private IEnumerator DecreaseStretch_Internal(float fRatio)
	{
		float fLeastRatio = fRatio;
		while(fLeastRatio > 0.0f)
		{
			float fDecrease = fRatio * Time.deltaTime;
			fLeastRatio -= fDecrease;
			_currentProcess -= fDecrease;

			//SetProcess(fRatio);

			yield return new WaitForEndOfFrame();
		}
	}

	//---------------------------------------
	public void SetProcess(float fRatio)
	{
		_currentProcess = fRatio;
		float fPerRoot = 1.0f / _roots.Length;

		//-----
		int nPfcCount = 0;
		float fCurRate = fPerRoot;
		Vector3 vScale;
		while (fCurRate <= fRatio)
		{
			if (Mathf.Abs(_roots[nPfcCount].transform.localScale.x - _rootsOrigScale[nPfcCount]) > float.Epsilon)
			{
				vScale = _roots[nPfcCount].transform.localScale;
				vScale.x = _rootsOrigScale[nPfcCount];
				_roots[nPfcCount].transform.localScale = vScale;

				_boxes[nPfcCount].gameObject.SetActive(true);
				vScale = _boxes[nPfcCount].transform.localScale;
				vScale.x = _boxesOrigScale[nPfcCount];
				_boxes[nPfcCount].transform.localScale = vScale;
			}

			fCurRate += fPerRoot;
			++nPfcCount;
		}

		if (nPfcCount >= _roots.Length)
			return;

		//-----
		float fLeastRatio = fRatio - (nPfcCount * fPerRoot);
		if (fLeastRatio > 0.0f)
		{
			float fScaleRate = fLeastRatio / fPerRoot;

			vScale = _roots[nPfcCount].transform.localScale;
			vScale.x = _rootsOrigScale[nPfcCount] * fScaleRate;
			_roots[nPfcCount].transform.localScale = vScale;

			_boxes[nPfcCount].gameObject.SetActive(true);
			vScale = _boxes[nPfcCount].transform.localScale;
			vScale.x = _boxesOrigScale[nPfcCount] * fScaleRate;
			_boxes[nPfcCount].transform.localScale = vScale;

			++nPfcCount;

			if (nPfcCount >= _roots.Length)
				return;
		}

		//-----
		for (int nInd = nPfcCount; nInd < _roots.Length; ++nInd)
		{
			vScale = _roots[nInd].transform.localScale;
			vScale.x = 0.0f;
			_roots[nInd].transform.localScale = vScale;

			_boxes[nPfcCount].gameObject.SetActive(false);
			vScale = _boxes[nInd].transform.localScale;
			vScale.x = 0.0f;
			_boxes[nInd].transform.localScale = vScale;
		}
	}
}


/////////////////////////////////////////
//---------------------------------------