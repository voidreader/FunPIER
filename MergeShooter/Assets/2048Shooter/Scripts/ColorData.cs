using System;

[Serializable]
public class ColorData : ICloneable
{
	public string Key;

	public string HomeGetMoney;

	public string HomeLevelBtn;

	public string HomeMoney;

	public string OverMoney;

	public string OverCurScore;

	public string OverScore;

	public string OverShare;

	public string OverLevelBtn;

	public string OverWatchMoney;

	public string OverGetMoney;

	public string GamingCurScore;

	public string GamingScore;

	public string GamingHitScore;

	public string GamingHit;

	public string PauseHead;

	public string PauseText;

	public string PauseScore;

	public string PauseTry;

	public string PauseContinue;

	public string GradeText;

	public string Loading;

	public string AchieveText;

	public string Watch;

	public string Tips;

	public string PopContinue;

	public string PopNew;

	public string Continue;

	public string WatchAdTip;

	public string WatchAdFree;

	public object Clone()
	{
		return base.MemberwiseClone();
	}
}
