using UnityEngine;
using System.Collections;
using HT;

/////////////////////////////////////////
//---------------------------------------
public class LevelSettings : MonoBehaviour
{
    /////////////////////////////////////////
    //---------------------------------------
    [Header("LEVEL INFO")]
    public bool _isLastBoss = false;
	public bool _isPackToDemo = false;

	//---------------------------------------
	[Header("FIELD OBJECT INFO")]
	public string _fieldName = null;
	public Field m_pField;
	public IActorBase m_pEnemyActor;
	public IActorBase m_pPlayerActor;

	//---------------------------------------
	[Header("LOBBY INFO")]
	public string _playerLobbyAnimName = null;

	//---------------------------------------
	[Header("BGM INFO")]
	public AudioClip _lobbyBGM = null;
	[Range(0.0f, 1.0f)]
	public float _lobbyBGM_Volume = 1.0f;
	public AudioClip _inGameBGM = null;
	[Range(0.0f, 1.0f)]
	public float _inGameBGM_Volume = 1.0f;

	//---------------------------------------
	[Header("ILLUST INFO")]
	public Sprite m_pLevelIllust;
	public Sprite m_pFieldIllust;
	public Color m_pLevelColor;

	//---------------------------------------
	[Header("TEXT INFO")]
	public string _levelName = null;
	public string _levelDescription = null;

	//---------------------------------------
	[Header("BOSS SKILL INFO")]
	[Range(0.0f, 1.0f)]
	public float _skillInfo_Vitality = 0.5f;
	[Range(0.0f, 1.0f)]
	public float _skillInfo_Speed = 0.5f;
	[Range(0.0f, 1.0f)]
	public float _skillInfo_Complexity = 0.5f;

	public eBossType _bossInfo_BossType = eBossType.eCommon;

	//---------------------------------------
	[Header("LEADER BOARD SETTING")]
	[SerializeField]
	private string[] _leaderBoardIDs_Google = null;
	[SerializeField]
	private string[] _leaderBoardIDs_iOS = null;

	public string LeaderBoardID
	{
		get
		{
#if ENABLE_GOOGLEPLAY || ENABLE_GAMECENTER
			int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;

	#if ENABLE_GOOGLEPLAY
			if (_leaderBoardIDs_Google != null && _leaderBoardIDs_Google.Length > nDiff)
				return _leaderBoardIDs_Google[nDiff];
	#elif ENABLE_GAMECENTER
			if (_leaderBoardIDs_iOS != null && _leaderBoardIDs_iOS.Length > nDiff)
				return _leaderBoardIDs_iOS[nDiff];
	#endif // ENABLE_GAMECENTER
#endif // ENABLE_GOOGLEPLAY || ENABLE_GAMECENTER

			return string.Empty;
		}
	}

	public string[] LeaderBoardIDs_iOS
	{
		get { return _leaderBoardIDs_iOS; }
		set { _leaderBoardIDs_iOS = value; }
	}

	public string[] LeaderBoardIDs_Google
	{
		get { return _leaderBoardIDs_Google; }
		set { _leaderBoardIDs_Google = value; }
	}


	/////////////////////////////////////////
	//---------------------------------------
	public string GetLevelName()
	{
		//if (HTAfxPref.IsMobilePlatform)
			return string.Format("{0}_{1}", GameDefine.szLobbySceneName, _fieldName);

		//return GameDefine.szLobbySceneName;
	}
}
