using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public sealed class Field_PoisonFactory : Field
{
	//---------------------------------------
	[Header("FIELD OBJECTS")]
	[SerializeField]
	private PoisonFactory_Pipe[] _pipes = null;
	[SerializeField]
	private PoisonFactory_OutterDoor[] _doors = null;

	//---------------------------------------
	[Header("CENTER POISON INFO")]
	[SerializeField]
	private GameObject _centerPoison = null;
	[SerializeField]
	private float _centerPoison_DefScale = 1.0f;
	[SerializeField]
	private float _centerPoison_MaxScale = 1.0f;
	[SerializeField]
	private SpawnActor _instance_SpawnSlime = null;

	[Header("POISON SPRINKLER INFO")]
	[SerializeField]
	private float[] _sprinkler_OnTime = null;
	[SerializeField]
	private float[] _sprinkler_RepeatTime = null;

	[Header("POISON PIPE INFO")]
	[SerializeField]
	private AudioClip _pipeOpenAlertSounds = null;
	[SerializeField]
	private Animation _pipeOpenAlertAnimator = null;

	//---------------------------------------
	private float _poisonFillRatio = 0.0f;
	public float PoisonFillRatio
	{
		get { return _poisonFillRatio; }
		set
		{
			_poisonFillRatio = value;
			_poisonFillRatio = Mathf.Clamp(_poisonFillRatio, 0.0f, 1.0f);
		}
	}

	//---------------------------------------
	public override void Init()
	{
		base.Init();
	}

	protected override void Frame()
	{
		base.Frame();

		//-----
		if (m_bIsLobbyField)
			return;

		//-----
		float fFillRatio = _centerPoison_DefScale;
		float fTerm = (_centerPoison_MaxScale - _centerPoison_DefScale) * _poisonFillRatio;
		_centerPoison.transform.localScale = Vector3.one * (fFillRatio + fTerm);
	}

	//---------------------------------------
	public override void OnBattleStart()
	{
		base.OnBattleStart();

		eGameDifficulty eDiff = GameFramework._Instance.m_pPlayerData.m_eDifficulty;
		Invoke("SetSprinkerActive", _sprinkler_RepeatTime[(int)eDiff]);
	}

	public override void OnBattleEnd(bool bPlayerWin)
	{
		base.OnBattleEnd(bPlayerWin);

		CancelInvoke();
	}

	//---------------------------------------
	private void SetSprinkerActive()
	{
		eGameDifficulty eDiff = GameFramework._Instance.m_pPlayerData.m_eDifficulty;

		int nFoundIndex = -1;
		for (int nInd = 0; nInd < 1000; ++nInd)
		{
			int nIndex = UnityEngine.Random.Range(0, _doors.Length);
			if (_doors[nIndex].IsOpened == false)
			{
				nFoundIndex = nIndex;
				break;
			}
		}

		_doors[nFoundIndex].DoorOpen(_sprinkler_OnTime[(int)eDiff]);
		Invoke("SetSprinkerActive", _sprinkler_RepeatTime[(int)eDiff]);
	}

	//---------------------------------------
	public void SetOnPipesOpen(int nPipeIndex)
	{
		//IActorBase pBoss = BattleFramework._Instance.m_pEnemyActor;
		//pBoss.AddActorBuff(_summonSlimeBuff);

		HT.HTSoundManager.PlaySound(_pipeOpenAlertSounds);
		_pipeOpenAlertAnimator.Play();

		_pipes[nPipeIndex].PipeOpen();
	}

	public int GetPipeCount()
	{
		return _pipes.Length;
	}

	public void SetClosePipe(int nPipeIndex)
	{
		_pipes[nPipeIndex].PipeClose();
	}

	public void CloseAllPipe()
	{
		for (int nInd = 0; nInd < _pipes.Length; ++nInd)
			_pipes[nInd].PipeClose();
	}

	public bool IsPipeOpened(int nPipeIndex)
	{
		if (_pipes[nPipeIndex].IsPipeOpened && _pipes[nPipeIndex].PipeProcessing == false && _pipes[nPipeIndex].WaitForPoisonProc == false)
			return true;

		if (_pipes[nPipeIndex].IsPipeOpened == false && _pipes[nPipeIndex].PipeProcessing)
			return true;

		return false;
	}

	//---------------------------------------
	public float GetCenterPoisonFillRatio()
	{
		float fMax = _centerPoison_MaxScale - _centerPoison_DefScale;
		return (_centerPoison.transform.localScale.x - _centerPoison_DefScale) / fMax;
	}

	//---------------------------------------
	public SpawnActor SpawnNewSlime(Vector3 vPos)
	{
		SpawnActor pSpawn = HT.Utils.Instantiate(_instance_SpawnSlime);
		pSpawn.transform.position = GameFramework._Instance.GetPositionByPhysic(vPos);

		pSpawn.SetSpawnActor(_instance_SpawnSlime);

		SpawnActor_Extend_Slime pExtend = pSpawn.GetComponent<SpawnActor_Extend_Slime>();
		pExtend.SetSlimeInfo(GetCenterPoisonFillRatio());

		return pSpawn;
	}

	public SpawnActor SpawnNewSlime(Vector3 vPos, int nLevel, float fSpeedRate)
	{
		SpawnActor pSpawn = HT.Utils.Instantiate(_instance_SpawnSlime);
		pSpawn.transform.position = GameFramework._Instance.GetPositionByPhysic(vPos);
		
		pSpawn.SetSpawnActor(_instance_SpawnSlime);

		SpawnActor_Extend_Slime pExtend = pSpawn.GetComponent<SpawnActor_Extend_Slime>();
		pExtend.SetSlimeInfo(fSpeedRate, nLevel);

		return pSpawn;
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------