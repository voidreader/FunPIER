using System;
using System.Collections.Generic;
using Toolkit;

public class GameLevelManger : MonoSingleton<GameLevelManger>
{
	private static readonly string LogDebug = "GameLevelManger.";

	private Dictionary<int, ShootLevel> m_levelData;

	public int m_CurLevel;

	public int m_CurSpeedLevel;

	public List<ValuePer> m_CurValuePercent;

	private Dictionary<int, List<int>> m_CurBricksDic;

	public float m_CurOverTime;

	private Missiontype m_CurMission;

	private Dictionary<int, int> m_MissionData;

	private int m_CurReward;

	public int LevelCount
	{
		get
		{
			return this.m_levelData.Count;
		}
	}

	public Dictionary<int, List<int>> CurBricksDic
	{
		get
		{
			return this.m_CurBricksDic;
		}
	}

	public Missiontype CurMission
	{
		get
		{
			return this.m_CurMission;
		}
	}

	public int CurReward
	{
		get
		{
			return this.m_CurReward;
		}
	}

	public override void Init()
	{
		base.Init();
		this.LoadJsonData();
		this.initGameLevelData();
	}

	public void RefreshLevelData(int levelNum)
	{
		if (levelNum <= 0 || levelNum > this.LevelCount)
		{
			SingleInstance<DebugManager>.Instance.LogError(GameLevelManger.LogDebug + "GetLevelData args is error, value = " + levelNum);
			return;
		}
		if (!this.m_levelData.ContainsKey(levelNum))
		{
			SingleInstance<DebugManager>.Instance.LogError(GameLevelManger.LogDebug + "GetLevelData m_levelData has not key, value = " + levelNum);
			return;
		}
		ShootLevel shootLevel = this.m_levelData[levelNum];
		this.m_CurLevel = levelNum;
		this.m_CurSpeedLevel = shootLevel.Speedlevel;
		this.m_CurValuePercent.Clear();
		this.m_CurValuePercent.Add(new ValuePer(2, shootLevel.N2));
		this.m_CurValuePercent.Add(new ValuePer(4, shootLevel.N4));
		this.m_CurValuePercent.Add(new ValuePer(8, shootLevel.N8));
		this.m_CurValuePercent.Add(new ValuePer(16, shootLevel.N16));
		this.m_CurValuePercent.Add(new ValuePer(32, shootLevel.N32));
		this.m_CurValuePercent.Add(new ValuePer(64, shootLevel.N64));
		this.m_CurValuePercent.Add(new ValuePer(128, shootLevel.N128));
		this.m_CurValuePercent.Add(new ValuePer(256, shootLevel.N256));
		this.m_CurValuePercent.Add(new ValuePer(512, shootLevel.N512));
		this.m_CurValuePercent.Add(new ValuePer(1024, shootLevel.N1024));
		this.m_CurValuePercent.Add(new ValuePer(2048, shootLevel.N2048));
		this.m_CurValuePercent.Add(new ValuePer(4096, shootLevel.N4096));
		this.m_CurValuePercent.Add(new ValuePer(8192, shootLevel.N8192));
		this.m_CurBricksDic.Clear();
		if (string.IsNullOrEmpty(shootLevel.Picture))
		{
			SingleInstance<DebugManager>.Instance.LogError(GameLevelManger.LogDebug + "RefreshLevelData json data Picture error,value =" + shootLevel.Picture);
			return;
		}
		string[] array = shootLevel.Picture.Split(new char[]
		{
			','
		});
		for (int i = 0; i < array.Length; i++)
		{
			int key = i % 5;
			if (!this.m_CurBricksDic.ContainsKey(key))
			{
				this.m_CurBricksDic.Add(key, new List<int>());
			}
			if (array[i] == string.Empty)
			{
				this.m_CurBricksDic[key].Add(0);
			}
			else
			{
				int item = int.Parse(array[i]);
				this.m_CurBricksDic[key].Add(item);
			}
		}
		this.m_CurOverTime = shootLevel.Time;
		this.m_CurMission = shootLevel.type;
		if (!string.IsNullOrEmpty(shootLevel.detail))
		{
			string[] array2 = shootLevel.detail.Split(new char[]
			{
				','
			});
			this.m_MissionData.Clear();
			switch (this.m_CurMission)
			{
			case Missiontype.Position:
				for (int j = 0; j < array2.Length; j++)
				{
					if (array2[j] == string.Empty)
					{
						this.m_MissionData.Add(j, 0);
					}
					else
					{
						int value = int.Parse(array2[j]);
						this.m_MissionData.Add(j, value);
					}
				}
				if (this.m_MissionData.Count > 35)
				{
					SingleInstance<DebugManager>.Instance.LogError(GameLevelManger.LogDebug + "RefreshLevelData level.detail 数据过多");
				}
				break;
			case Missiontype.Mergenum:
			case Missiontype.Existnum:
				if (array2.Length % 2 != 0)
				{
					SingleInstance<DebugManager>.Instance.LogError(GameLevelManger.LogDebug + "RefreshLevelData level.detail error 不是偶数");
				}
				for (int k = 0; k < array2.Length; k += 2)
				{
					int key2 = int.Parse(array2[k]);
					int value2 = int.Parse(array2[k + 1]);
					this.m_MissionData.Add(key2, value2);
				}
				break;
			}
			this.m_CurReward = shootLevel.Reward;
			return;
		}
		SingleInstance<DebugManager>.Instance.LogError(GameLevelManger.LogDebug + "RefreshLevelData level.detail IsNullOrEmpty");
	}

	public bool CheckGameOver()
	{
		return MonoSingleton<GameDataManager>.Instance.GameMode != 2 && false;
	}

	public bool CheckGameWin()
	{
		if (MonoSingleton<GameDataManager>.Instance.GameMode != 2)
		{
			return false;
		}
		Dictionary<int, List<Brick>> bricksDic = MonoSingleton<GamePlayManager>.Instance.BricksDic;
		Dictionary<int, int> brickNumDic = MonoSingleton<GamePlayManager>.Instance.BrickNumDic;
		bool result = true;
		switch (this.m_CurMission)
		{
		case Missiontype.Position:
			for (int i = 0; i < this.m_MissionData.Count; i++)
			{
				if (this.m_MissionData[i] != 0)
				{
					int key = i % 5;
					int num = i / 5;
					if (!bricksDic.ContainsKey(key) || bricksDic[key].Count <= num)
					{
						return false;
					}
					if (bricksDic[key][num].m_Number != this.m_MissionData[i])
					{
						return false;
					}
				}
			}
			break;
		case Missiontype.Mergenum:
			for (int j = 2; j <= 8192; j *= 2)
			{
				if (this.m_MissionData.ContainsKey(j) && brickNumDic[j] < this.m_MissionData[j])
				{
					result = false;
					break;
				}
			}
			break;
		case Missiontype.Existnum:
			for (int k = 2; k <= 8192; k *= 2)
			{
				if (this.m_MissionData.ContainsKey(k))
				{
					int num2 = 0;
					for (int l = 0; l < bricksDic.Count; l++)
					{
						foreach (Brick current in bricksDic[l])
						{
							if (current.m_Number == k)
							{
								num2++;
							}
						}
					}
					if (num2 < this.m_MissionData[k])
					{
						result = false;
						break;
					}
				}
			}
			break;
		}
		return result;
	}

	private void LoadJsonData()
	{
		this.m_levelData = new Dictionary<int, ShootLevel>();
		ShootLevelJson t = ReadJson.GetT<ShootLevelJson>("Level");
		foreach (ShootLevel current in t.Data)
		{
			this.m_levelData.Add(current.Level, current);
		}
	}

	private void initGameLevelData()
	{
		this.m_CurValuePercent = new List<ValuePer>();
		this.m_CurBricksDic = new Dictionary<int, List<int>>();
		this.m_MissionData = new Dictionary<int, int>();

		//==if (MonoSingleton<GameDataManager>.Instance.GameMode == 2 && MonoSingleton<GameDataManager>.Instance.GameLevel != 0)
		{
			this.RefreshLevelData(MonoSingleton<GameDataManager>.Instance.GameLevel);
		}
	}
}
