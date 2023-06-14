using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/////////////////////////////////////////
//---------------------------------------
public class Archivement : MonoBehaviour
{
	//---------------------------------------
	[Header("ARCHIVEMENT INFO")]
	[SerializeField]
	private string _archiveID = null;
	[SerializeField]
	private string _archiveID_Google = null;
	[SerializeField]
	private string _archiveName = null;
	[SerializeField]
	private string _archiveDesc = null;
	[SerializeField]
	private Sprite _archiveIcon = null;
	[SerializeField]
	private int _archivePoint = 10;

	[Header("ARCHIVEMET SETTING")]
	[SerializeField]
	private int _requireCount = 1;
	[SerializeField]
	private eBossType _bossType = eBossType.eCommon;
	[SerializeField]
	private eArchiveType _archiveType = eArchiveType.Max;
	[SerializeField]
	private eArchiveCountType _archiveCountType = eArchiveCountType.SmallThan;
	[SerializeField]
	private eGameDifficulty _difficulty = eGameDifficulty.eMax;
	[SerializeField]
	private bool _isHardcoreOnly = false;

	[Header("TECHNICAL SETTING")]
	[SerializeField]
	private ActorBuff _playerBuffWhenBattle = null;
	[SerializeField]
	private ActorBuff _enemyBuffWhenBattle = null;


	//---------------------------------------
	public string ArchiveID { get { return _archiveID; } }
	public string ArchiveID_Google { get { return _archiveID_Google; } }
	public string CorrectID
	{
		get
		{
#if ENABLE_GOOGLEPLAY
			return _archiveID_Google;
#endif // ENABLE_GOOGLEPLAY
			return _archiveID;
		}
	}

	public string ArchiveName { get { return _archiveName; } }
	public string ArchiveDesc{ get { return _archiveDesc; } }

	public Sprite ArchiveIcon { get { return _archiveIcon; } }
	public int ArchivePoint { get { return _archivePoint; } }

	//---------------------------------------
	public int RequireCount { get { return _requireCount; } }
	public eBossType BossType { get { return _bossType; } }
	public eArchiveType ArchiveType { get { return _archiveType; } }
	public eArchiveCountType ArchiveCountType { get { return _archiveCountType; } }

	public bool IsHardcoreOnly { get { return _isHardcoreOnly; } }

	//---------------------------------------
	private Archives _linkArchive = null;

	private bool _isInBattle = false;
	public bool IsInBattle { get { return _isInBattle; } }

	private bool _recordingArchive = false;


	//---------------------------------------
	public void OnBattleStart()
	{
		if (_linkArchive == null)
			_linkArchive = ArchivementManager.Instance.FindArchive(_archiveID);

		PlayerData pData = GameFramework._Instance.m_pPlayerData;
		if (_isHardcoreOnly && pData._isHardCoreMode == false)
			return;

		if (_difficulty > pData.m_eDifficulty)
			return;

		//-----
		if (_linkArchive != null && _linkArchive.IsComplete() == false)
		{
			_isInBattle = true;
			_linkArchive.Counted = GetDefaultCount();
		}

		if (_isInBattle)
			_recordingArchive = true;

		//-----
		if (_playerBuffWhenBattle != null)
			BattleFramework._Instance.m_pPlayerActor.AddActorBuff(_playerBuffWhenBattle);

		if (_enemyBuffWhenBattle != null)
			BattleFramework._Instance.m_pEnemyActor.AddActorBuff(_enemyBuffWhenBattle);
	}

	public void OnBattleEnd(bool bPlayerWin)
	{
		if (_linkArchive == null)
			_linkArchive = ArchivementManager.Instance.FindArchive(_archiveID);

		if (_isInBattle == false)
			return;

		_isInBattle = false;

		if (_linkArchive != null)
		{
			//-----
			PlayerData pData = GameFramework._Instance.m_pPlayerData;

			ControllableActor pPlayer = BattleFramework._Instance.m_pPlayerActor as ControllableActor;
			AIActor pEnemy = BattleFramework._Instance.m_pEnemyActor as AIActor;

			bool bShowArchiveShower = false;
			switch (ArchiveType)
			{
				case eArchiveType.Win:
					if (bPlayerWin)
					{
						OnArchiveCount(RequireCount);
						bShowArchiveShower = true;
					}
					break;

				case eArchiveType.Defeat:
					if (bPlayerWin == false)
					{
						OnArchiveCount(RequireCount);
						bShowArchiveShower = true;
					}
					break;

				case eArchiveType.Win_TryCount:
					if (bPlayerWin && pData.m_vTryCount[pData.m_nActivatedLevel] <= RequireCount)
					{
						OnArchiveCount(RequireCount);
						bShowArchiveShower = true;
					}
					break;

				case eArchiveType.HealthCheck:
					if (bPlayerWin)
					{
						OnArchiveCount(pPlayer.GetCurrHP());
						bShowArchiveShower = true;
					}
					break;

				case eArchiveType.HealthCheck_Equal:
					if (bPlayerWin && pPlayer.GetCurrHP() == RequireCount)
					{
						OnArchiveCount(RequireCount);
						bShowArchiveShower = true;
					}
					break;

				case eArchiveType.HealthCheck_Lost:
					if (bPlayerWin)
					{
						int nLostHP = pPlayer.GetMaxHP() - pPlayer.GetCurrHP();
						if (ArchiveCountType == eArchiveCountType.MoreThan && nLostHP > RequireCount)
						{
							OnArchiveCount(RequireCount);
							bShowArchiveShower = true;
						}
						else if (ArchiveCountType == eArchiveCountType.SmallThan)
						{
							if (nLostHP >= RequireCount)
								_linkArchive.Counted -= RequireCount;

							else
								bShowArchiveShower = true;
						}
					}
					break;

				case eArchiveType.HasBuff_Enemy:
					if (bPlayerWin && pEnemy.FindEnabledActorBuff(_enemyBuffWhenBattle) != null)
					{
						OnArchiveCount(RequireCount);
						bShowArchiveShower = true;
					}
					break;

				case eArchiveType.HasBuff_Player:
					if (bPlayerWin && pPlayer.FindEnabledActorBuff(_playerBuffWhenBattle) != null)
					{
						OnArchiveCount(RequireCount);
						bShowArchiveShower = true;
					}
					break;

				default:
					if (bPlayerWin == false)
						_linkArchive.Counted = 0;

					break;
			}

			//-----
			if (_linkArchive.IsComplete())
			{
				if (ArchiveType == eArchiveType.BattleEnd || bShowArchiveShower)
				{
					HT.Utils.SafeInvoke(ArchivementManager.Instance.onArchiveComplete, _linkArchive);

					//-----
					if (HT.HTPlatform.Instance.IsAchievementUseInt())
						HT.HTPlatform.Instance.SubmitAchievement(CorrectID);
					else
						HT.HTPlatform.Instance.SubmitAchievementPoint(CorrectID, 100.0f);
				}
			}
			else
				_linkArchive.Counted = 0;
		}

		_recordingArchive = false;
	}

	//---------------------------------------
	public void OnArchiveCount(int nCount)
	{
		if (_linkArchive == null)
			_linkArchive = ArchivementManager.Instance.FindArchive(_archiveID);

		if (_recordingArchive && _linkArchive != null && _linkArchive.IsComplete() == false)
		{
			PlayerData pData = GameFramework._Instance.m_pPlayerData;

			if (_isHardcoreOnly && pData._isHardCoreMode == false)
				return;

			if (_difficulty > pData.m_eDifficulty)
				return;

			switch(ArchiveCountType)
			{
				case eArchiveCountType.MoreThan:
					_linkArchive.Counted += nCount;
					break;

				case eArchiveCountType.SmallThan:
				case eArchiveCountType.SmallThan_MoreZero:
					_linkArchive.Counted -= nCount;
					break;
			}

			eArchiveType pType = _linkArchive.Archive._archiveType;
			if (_linkArchive.IsComplete() && (pType == eArchiveType.EveryEvent))
			{
				HT.Utils.SafeInvoke(ArchivementManager.Instance.onArchiveComplete, _linkArchive);

				//-----
				if (HT.HTPlatform.Instance.IsAchievementUseInt())
					HT.HTPlatform.Instance.SubmitAchievement(CorrectID);
				else
					HT.HTPlatform.Instance.SubmitAchievementPoint(CorrectID, 100.0f);
			}
		}
	}

	//---------------------------------------
	public int GetDefaultCount()
	{
		switch (ArchiveCountType)
		{
			case eArchiveCountType.MoreThan:
				return 0;

			case eArchiveCountType.SmallThan:
			case eArchiveCountType.SmallThan_MoreZero:
				return RequireCount;
		}

		return 0;
	}
}


/////////////////////////////////////////
//---------------------------------------
public class Archives
{
	//---------------------------------------
	private Archivement _archive = null;
	public Archivement Archive { get { return _archive; } }
	
	//---------------------------------------
	private int _counted = 0;
	public int Counted
	{
		get { return _counted; }
		set { _counted = value; }
	}


	//---------------------------------------
	public void Init(Archivement pArchive)
	{
		_archive = pArchive;
		_counted = 0;
	}

	//---------------------------------------
	public bool IsComplete()
	{
		if (_archive.IsInBattle && _archive.ArchiveType != eArchiveType.EveryEvent)
			return false;

		if (_counted >= _archive.RequireCount)
			return true;

		if (_archive.ArchiveCountType == eArchiveCountType.SmallThan_MoreZero && _counted > 0)
			return true;

		return false;
	}
}


/////////////////////////////////////////
//---------------------------------------