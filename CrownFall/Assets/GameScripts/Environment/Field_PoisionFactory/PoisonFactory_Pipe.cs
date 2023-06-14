using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class PoisonFactory_Pipe : MonoBehaviour
{
	//---------------------------------------
	[Header("PIPE SETTING")]
	[SerializeField]
	private Animation _pipeAnimator = null;
	[SerializeField]
	private string _pipeAnim_Open = null;
	[SerializeField]
	private string _pipeAnim_Close = null;
	[SerializeField]
	private string _pipeAnim_IDLE_Open = null;
	[SerializeField]
	private string _pipeAnim_IDLE_Close = null;
	[SerializeField]
	private float _pipeAnim_PoisonWaitTime = 2.0f;

	[Header("SUB OBJECTS")]
	[SerializeField]
	private PoisonFactory_PipeHandle _pipeHandle = null;

	[Header("STATUS")]
	[SerializeField]
	private bool _isPipeOpened = false;
	[SerializeField]
	private bool _pipeProcessing = false;
	[SerializeField]
	private bool _waitForPoisonProc = false;

	//---------------------------------------
	public bool IsPipeOpened
	{
		get { return _isPipeOpened; }
		set { _isPipeOpened = value; }
	}

	public bool PipeProcessing
	{
		get { return _pipeProcessing; }
		set { _pipeProcessing = value; }
	}

	public bool WaitForPoisonProc
	{
		get { return _waitForPoisonProc; }
		set { _waitForPoisonProc = value; }
	}


	//---------------------------------------
	private int _pipeOpenValue = 0;
	public int PipeOpenValue
	{
		get { return _pipeOpenValue; }
		set { _pipeOpenValue = value; }
	}

	//---------------------------------------
	private void Start()
	{
		_pipeHandle.gameObject.SetActive(true);
	}

	private void Update()
	{
	}
	
	//---------------------------------------
	public void PipeOpen()
	{
		if (IsPipeOpened)
			return;

		if (_pipeProcessing)
			return;
		
		StartCoroutine(PipeProc_Internal(true));
	}

	public void PipeClose()
	{
		if (IsPipeOpened == false)
			return;

		if (_pipeProcessing)
			return;
		
		StartCoroutine(PipeProc_Internal(false));
	}

	//---------------------------------------
	public void PipeOpenRatioDecrease()
	{
		--_pipeOpenValue;
		if (_pipeOpenValue <= 0)
		{
			_pipeOpenValue = 0;
			PipeClose();
		}
	}

	//---------------------------------------
	private IEnumerator PipeProc_Internal(bool bIsOpen)
	{
		_isPipeOpened = bIsOpen;
		_pipeProcessing = true;
		_waitForPoisonProc = true;
		_pipeOpenValue = (bIsOpen) ? GameDefine.fBoss_PG_PipeCloseNeedIntCnt : 0;

		string szProcAnimName = (bIsOpen) ? _pipeAnim_Open : _pipeAnim_Close;
		string szIdleAnimName = (bIsOpen) ? _pipeAnim_IDLE_Open : _pipeAnim_IDLE_Close;

		AnimationClip pProcClip = _pipeAnimator.GetClip(szProcAnimName);
		_pipeAnimator.Play(szProcAnimName);

		yield return new WaitForSeconds(pProcClip.length);
		
		_pipeAnimator.Play(szIdleAnimName);
		_pipeProcessing = false;

		yield return new WaitForSeconds(_pipeAnim_PoisonWaitTime);

		_waitForPoisonProc = false;
	}
}


/////////////////////////////////////////
//---------------------------------------
