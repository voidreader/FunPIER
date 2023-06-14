using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class GameDefine
{
	/////////////////////////////////////////
	//---------------------------------------
	static public readonly int nSaveFile_Version_Game = 11;
	static public readonly int nSaveFile_Version_System = 3;

	static public readonly uint nSteamID = 683730;

	static public readonly string szSaveFile_System = "_System.bin";
	static public readonly string szSaveFile_Game = "_Data.bin";


	/////////////////////////////////////////
	//---------------------------------------
	static public readonly float fEssentialTime = 2.0f;

	//---------------------------------------
	static public readonly int nPlayer_Base_HP = 4;
	static public readonly float fPlayer_Arrow_SpreadDeg = 40.0f;

	static public readonly float fAttackPowerIncreaseRatio = 0.2f;
	static public readonly float fMoveSpeedIncreaseRatio = 0.1f;

	static public readonly float fChargeTimeDecreaseRatio = 0.1f; // 1 / 60 * 5 = 0.0833
	static public readonly float fDashDelayDecreaseRatio = 0.1f;


	/////////////////////////////////////////
	//---------------------------------------
	static public readonly float fBoss_Base_HP_IncreaseRatio = 0.0f;
	static public readonly float fBoss_Base_HP_IncreaseRatio_Difficult = 0.2f;

	static public readonly int nBoss_AddPattern_ByStage = 3;

	//---------------------------------------
	static public readonly float fBoss_PG_PoisonDebuffRatio = 15.0f;

	static public readonly float fBoss_PG_CenterPoison_IncRatio = 0.045f;
	static public readonly float fBoss_PG_CenterPoison_DecRatio = 0.5f;

	static public readonly int fBoss_PG_PipeCloseNeedIntCnt = 1;

    //---------------------------------------
    static public readonly int nTimeScaleLayer_Tutorial = 10;
	static public readonly int nTimeScaleLayer_CounterAtk = 11;
	static public readonly int nTimeScaleLayer_FullChargeAtk = 12;


	/////////////////////////////////////////
	//---------------------------------------
	public const string szKeyName_Fire1 = "Fire1";
	public const string szKeyName_Fire2 = "Fire2";
	public const string szKeyName_SpcAtk = "SpcAtk";
	public const string szKeyName_Interact = "Interact";

	public const string szKeyName_Submit = "Submit";
	public const string szKeyName_Cancel = "Cancel";


	/////////////////////////////////////////
	//---------------------------------------
	static public readonly string szMainTitleSceneName = "B_MainTitle";

	static public readonly string szLoadSceneName = "C_LoadScene";
	static public readonly string szLobbySceneName = "D_GameScene";

	static public readonly string szInGameSceneName = "E_InGame";
	static public readonly string szEndGameSceneName = "F_EndGame";

	static public readonly string szSelectFieldSceneName = "C_StageSelect";


	/////////////////////////////////////////
	//---------------------------------------
	public const int STEAM_APPID = 683730;


	/////////////////////////////////////////
	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------
public enum eGameDifficulty
{
	eEasy = 0,
	eNormal,
	eHard,
	
	eMax,
}

public enum eBossType
{
	eCommon,

	eSoulBringer,
	eMeltenMaw,
	eSchenabel,
	eArchitecture,
	eStoneGolem,
	eVenusTrap,
	eGladiator,
    eRoyalGuard,
	eBlack,
}

public enum ePadButton
{
	A,
	B,
	X,
	Y,

	Max,
}

/////////////////////////////////////////
//---------------------------------------
public enum eAlertRingType
{
	Angle360,
	Angle180,
	Angle130,
	Angle90,

	Angle360_Simple,
}

/////////////////////////////////////////
//---------------------------------------
public enum eArchiveType
{
	EveryEvent,
	BattleEnd,

	Win,
	Win_TryCount,
	Defeat,

	HealthCheck,
	HealthCheck_Equal,
	HealthCheck_Lost,

	HasBuff_Enemy,
	HasBuff_Player,

	Max,
}

public enum eArchiveCountType
{
	MoreThan,
	SmallThan,
	SmallThan_MoreZero,
}