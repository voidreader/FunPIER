using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class PoisonFactory_PipeHandle : MonoBehaviour
{
	//---------------------------------------
	[Header("OBJECTS")]
	[SerializeField]
	private PoisonFactory_Pipe _parent_Pipe = null;
	[SerializeField]
	private Sprite _interactIconImage = null;
	[SerializeField]
	private Animation _animator = null;
	[SerializeField]
	private string _handleAnim_Rotate = null;

	[Header("ARCHIVES")]
	[SerializeField]
	private string _effectiveAcv_OnClick = null;

	//---------------------------------------
	private ControllableActor _enteredActor = null;
	private bool _handleAnimated = false;
	private Object_AreaAlert _createdInteractNotice = null;

	//---------------------------------------
	private HT.HTKey _interactKey = null;

	//---------------------------------------
	private void Awake()
	{
		_interactKey = HT.HTInputManager.Instance.GetKey(GameDefine.szKeyName_Interact);
	}

	private void Update()
	{
		bool bShowNotice = false;
		do
		{
			if (_parent_Pipe.IsPipeOpened == false)
				break;

			bShowNotice = true;
		}
		while (false);
		
		if (bShowNotice)
		{
			if (_createdInteractNotice == null)
				_createdInteractNotice = BattleFramework._Instance.CreateInteractionNotice(gameObject.transform.position);
		}
		else
		{
			if (_createdInteractNotice != null)
			{
				BattleFramework._Instance.RemoveInteractionNotice(_createdInteractNotice);
				_createdInteractNotice = null;
			}
		}

		//-----
		if (InteractionEnable())
		{
			if (_interactKey.IsDown)
			{
				_handleAnimated = true;

				AnimationClip pClip = _animator.GetClip(_handleAnim_Rotate);
				Invoke("OnHandleRotateEnd", pClip.length + 0.25f);

				_animator.Play(_handleAnim_Rotate);
				_enteredActor.DoInteraction(pClip.length, InteractionCanceled);
				BattleFramework._Instance._ui_InteractBtnView.SetCoolTime(pClip.length);
			}
		}
	}

	private void FixedUpdate()
	{
		if (_enteredActor != null)
		{
			bool bEnable = false;
			if (_parent_Pipe.IsPipeOpened && _parent_Pipe.PipeProcessing == false)
				bEnable = true;

			BattleFramework._Instance._ui_InteractBtnView.SetEnabled(bEnable);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		ControllableActor pCtrlActor = other.gameObject.GetComponent<ControllableActor>();
		if (pCtrlActor != null)
		{
			_enteredActor = pCtrlActor;
			BattleFramework._Instance._ui_InteractBtnView.ShowInteract(true, _interactIconImage);

			FixedUpdate();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (_enteredActor != null && _enteredActor.gameObject == other.gameObject)
		{
			_enteredActor = null;
			BattleFramework._Instance._ui_InteractBtnView.ShowInteract(false);
		}
	}

	//---------------------------------------
	public bool InteractionEnable()
	{
		if (_enteredActor == null)
			return false;

		if (_parent_Pipe.IsPipeOpened == false)
			return false;

		if (_parent_Pipe.PipeProcessing)
			return false;

		if (_handleAnimated)
			return false;

		return true;
	}

	private void OnHandleRotateEnd()
	{
		_handleAnimated = false;

		_parent_Pipe.PipeOpenRatioDecrease();

		if (string.IsNullOrEmpty(_effectiveAcv_OnClick) == false)
		{
			Archives pArchives = ArchivementManager.Instance.FindArchive(_effectiveAcv_OnClick);
			pArchives.Archive.OnArchiveCount(1);
		}
	}

	//---------------------------------------
	public void InteractionCanceled()
	{
		_handleAnimated = false;
		CancelInvoke();

		BattleFramework._Instance._ui_InteractBtnView.StopCoolTime();
	}
}


/////////////////////////////////////////
//---------------------------------------