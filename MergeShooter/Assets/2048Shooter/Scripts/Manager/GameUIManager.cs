using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Toolkit;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoSingleton<GameUIManager>
{
	private sealed class _ShowGameOverMoneyAnim_c__AnonStorey1
	{
		internal Vector3 startPos;
	}

	private sealed class _ShowGameOverMoneyAnim_c__AnonStorey0
	{
		internal Transform child;

		internal GameUIManager._ShowGameOverMoneyAnim_c__AnonStorey1 __f__ref_1;

		internal void __m__0()
		{
			MonoSingleton<AudioManager>.Instance.PlayMoneyAudio();
			this.child.gameObject.SetActive(false);
			this.child.localPosition = this.__f__ref_1.startPos;
		}
	}

	private AudioSource audioSrc;

	[SerializeField]
	private Course m_Course;

	[SerializeField]
	private HomePanel m_Home;

	[SerializeField]
	private ContinuePanel m_Continue;

	[SerializeField]
	private PausePanel m_Pause;

	[SerializeField]
	private GameOverPanel m_GameOver;

	[SerializeField]
	private StorePanel m_Store;

	[SerializeField]
	private ItemStorePanel m_ItemStore;

	[SerializeField]
	private GradePanel m_Grade;

	[SerializeField]
	private LoadingPanel m_Loading;

	[SerializeField]
	private LoadPanel m_Load;

	[SerializeField]
	private PopPanel m_Pop;

	[SerializeField]
	private AchievementPanel m_Achievement;

	[SerializeField]
	private NoticePanel m_Notice;

	[SerializeField]
	private SelectPopPanel m_SelectPop;

	[SerializeField]
	private GameOverWatch m_gameOverWatch;

	[SerializeField]
	private GameObject m_Comingtips;

	[SerializeField]
	private GameObject m_CoinImage;

	[SerializeField]
	private List<Text> m_textList;

	[SerializeField]
	private List<Image> m_imageList;

	private Dictionary<string, ColorData> colorMap;

	public override void Init()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		this.colorMap = new Dictionary<string, ColorData>();
		ColorDataJson t = ReadJson.GetT<ColorDataJson>("ColorData");
		foreach (ColorData current in t.Data)
		{
			this.colorMap.Add(current.Key, current);
		}
		this.m_Course.Init();
		this.m_Home.Init();
		this.m_Continue.Init();
		this.m_Pause.Init();
		this.m_GameOver.Init();
		this.m_Store.Init();
		this.m_ItemStore.Init();
		this.m_Grade.Init();
		this.m_Pop.Init();
		this.m_Achievement.Init();
		this.m_gameOverWatch.Init();

		audioSrc = GetComponent<AudioSource>();
	}

	public void ShowCourse()
	{
		this.m_Course.Show();
		this.m_Course.Play();
	}

	public void StopCourse()
	{
		this.m_Course.Stop();
	}

	public void HideCourse()
	{
		this.m_Course.Stop();
		this.m_Course.Hide();
	}

	public void ShowLoad(Action openCallBack = null, Action closeCallBack = null)
	{
		this.m_Load.Show(openCallBack, closeCallBack);
	}

	public void OpenHomePanel(Action callback = null, float dealy = 0f)
	{
		this.m_Home.Open(callback, dealy);
		this.m_gameOverWatch.Close(callback, dealy);

		PlayBGM();
	}

	public void PlayBGM()
	{
		if (MonoSingleton<GameDataManager>.Instance.Audio)
		{
			audioSrc.Play();
		}
	}

	public void StopBGM()
	{
		if (audioSrc.isPlaying)
		{
			audioSrc.Stop();
		}
	}

	public void OpenContinuePanel(Action callback = null, float dealy = 0f)
	{
		this.m_Continue.Open(callback, dealy);
	}

	public void OpenPasuePanel(Action callback = null, float dealy = 0f)
	{
		this.m_Pause.Open(callback, dealy);
	}

	public void OpenGameOverPanel(Action callback = null, float dealy = 0f)
	{
		this.m_GameOver.Open(callback, dealy);
		this.m_gameOverWatch.Close(callback, dealy);
	}

	public void SetAchievementText(string txt)
	{
		this.m_Achievement.SetText(txt);
	}

	public void OpenAchievementPanel(Action callback = null, float dealy = 0f)
	{
		this.m_Achievement.Open(callback, dealy);
	}

	public void CloseGameOverWatch(Action callback = null, float dealy = 0f)
	{
		this.m_gameOverWatch.Close(callback, dealy);
	}

	public void OpenGameOverWatch(Action callback = null, float dealy = 0f)
	{
		this.m_gameOverWatch.Open(callback, dealy);
	}

	public void CloseCurrentPanel(Action callback = null, float dealy = 0f)
	{
		if (this.m_Home.UIState && !IsStoreState())
		{
			this.m_Home.Close(callback, dealy);
		}
		if (this.m_Continue.UIState)
		{
			this.m_Continue.Close(callback, dealy);
		}
		if (this.m_Pause.UIState)
		{
			this.m_Pause.Close(callback, dealy);
		}
		if (this.m_GameOver.UIState)
		{
			this.m_GameOver.Close(callback, dealy);
		}
		if (this.m_Store.UIState)
		{
			this.m_Store.Close(callback, dealy);
		}
		if (this.m_ItemStore.UIState)
		{
			this.m_ItemStore.Close(callback, dealy);
		}
		if (this.m_Load.UIState)
		{
			this.m_Load.Close(callback, dealy);
		}
		if (this.m_Loading.UIState)
		{
			this.m_Loading.Close(callback, dealy);
		}
		if (this.m_Pop.UIState)
		{
			this.m_Pop.Close(callback, dealy);
		}
		if (this.m_Grade.UIState)
		{
			this.m_Grade.Close(callback, dealy);
		}
		if (this.m_Achievement.UIState)
		{
			this.m_Achievement.Close(callback, dealy);
		}
		if (this.m_Notice.UIState)
		{
			this.m_Notice.Close(callback, dealy);
		}
		if (this.m_SelectPop.UIState)
		{
			this.m_SelectPop.Close(callback, dealy);
		}
	}

	public void RefreshCurrentPanel()
	{
		if (this.m_Home.UIState)
		{
			this.m_Home.Refresh();
		}
		else if (this.m_Pause.UIState)
		{
			this.m_Pause.Refresh();
		}
		else if (this.m_GameOver.UIState)
		{
			this.m_GameOver.Refresh();
		}
	}

	public void UnlockSkin(SkinType type)
	{
		this.m_Store.UnlockSkin(type);
		this.m_Home.UnlockSkin(type);
		this.m_GameOver.UnlockSkin(type);
	}

	public void UseSkin(SkinType type)
	{
		this.changeColor(type);
	}

	public void OpenStorePanel(Action callback = null, float dealy = 0f)
	{
		this.m_Store.Open(callback, dealy);
	}

	public void CloseStorePanel(Action callback = null, float dealy = 0f)
	{
		this.m_Store.Close(callback, dealy);
	}

	public void OpenItemStorePanel(Action callback = null, float dealy = 0f)
	{
		this.m_ItemStore.Open(callback, dealy);
	}

	public void CloseItemStorePanel(Action callback = null, float dealy = 0f)
	{
		this.m_ItemStore.Close(callback, dealy);
	}

	public void OpenNoticePanel(string notice, Action callback = null, float dealy = 0f)
	{
		this.m_Notice.Open(notice, callback, dealy);
	}

	public void CloseNoticePanel(Action callback = null, float dealy = 0f)
	{
		this.m_Notice.Close(callback, dealy);
	}

	public void OpenSelectPopPanel(string notice, Action confirmCallback, Action callback = null, float dealy = 0f)
	{
		this.m_SelectPop.Open(notice, confirmCallback, callback, dealy);
	}

	public void CloseSelectPopPanel(Action callback = null, float dealy = 0f)
	{
		this.m_SelectPop.Close(callback, dealy);
	}

	public void OpenPopPanel(Action callback = null, float dealy = 0f)
	{
		this.m_Pop.Open(callback, dealy);
	}

	public void OpenGradlePanel(Action callback = null, float dealy = 0f)
	{
		this.m_Grade.Open(callback, dealy);
	}

	public void CloseGradlePanel(Action callback = null, float dealy = 0f)
	{
		this.m_Grade.Close(callback, dealy);
	}

	public void ShowLoading(Action callback = null, float dealy = 0f)
	{
		this.m_Loading.Show(callback, dealy);
	}

	public void TimerState(bool state = true)
	{
		this.m_Continue.TimerState(state);
	}

	public bool UIState()
	{
		return this.m_Home.UIState || this.m_Continue.UIState || this.m_Pause.UIState || this.m_GameOver.UIState || this.m_Grade.UIState || this.m_Load.UIState || this.m_Pause.UIState || this.m_Pop.UIState || this.m_Store.UIState || this.m_ItemStore.UIState|| this.m_Achievement.UIState;
	}

	public bool ISPopOpen()
	{
		return this.m_Pop.UIState;
	}

	public bool ISNoticeOpen()
	{
		return this.m_Notice.UIState;
	}

	public bool IsHomeOrGmaeOver()
	{
		return this.m_GameOver.UIState || this.m_Home.UIState;
	}

	public bool IsHomeState()
	{
		return this.m_Home.UIState;
	}

	public bool IsStoreState()
	{
		if(this.m_Store.UIState || this.m_ItemStore.UIState)
			return true;
		else
			return false;
	}

	public void StoreHit()
	{
		if (this.m_Store.UIState)
		{
			//this.m_Store.ShowHit();
		}
	}

	public void ShowGameOverMoneyAnim(bool isDouble, int num)
	{
		Vector3 startPos;
		if (this.IsHomeState())
		{
			this.m_Home.NumAnim(num);
			startPos = new Vector3(100f, 56.9f);
		}
		else
		{
			this.m_GameOver.NumAnim(isDouble, num);
			startPos = new Vector3(100f, 31.8f);
		}
		for (int i = 0; i < this.m_CoinImage.transform.childCount; i++)
		{
			Transform child = this.m_CoinImage.transform.GetChild(i);
			child.gameObject.SetActive(true);
			child.localPosition = startPos;
			child.localScale = Vector3.one;
			child.DOScale(Vector3.one * 0.5f, 1f).SetEase(Ease.InOutCubic).SetDelay((float)i * 0.1f);
			child.DOMove(new Vector3(40, 596, 0), 1f, false).SetEase(Ease.InOutCubic).OnComplete(delegate
			{
				MonoSingleton<AudioManager>.Instance.PlayMoneyAudio();
				child.gameObject.SetActive(false);
				child.localPosition = startPos;
			}).SetDelay((float)i * 0.1f);
		}
	}

	public void CloseWatchBtn()
	{
		this.m_GameOver.CloseWatchBtn();
		this.m_Home.CloseWatchBtn();
	}

	public void OnClickComingTips()
	{
		this.m_Comingtips.gameObject.SetActive(true);
		this.m_Comingtips.transform.DOKill(false);
		this.m_Comingtips.transform.localScale = Vector3.zero;
		this.m_Comingtips.transform.DOScale(Vector3.one, 0.4f).OnComplete(delegate
		{
			this.m_Comingtips.transform.DOScale(Vector3.zero, 0.4f).OnComplete(delegate
			{
				this.m_Comingtips.gameObject.SetActive(false);
			}).SetDelay(1f);
		});
	}

	public void OnClickContinueHeart()
	{
		// MonoSingleton<GAEvent>.Instance.ContinuePIC();
	}

	public void OnClickUnlockLevelMode()
	{
	}

	private void changeColor(SkinType type)
	{
		/*if (!this.colorMap.ContainsKey(type.ToString()))
		{
			SingleInstance<DebugManager>.Instance.LogError("changeTextColor colorMap hav not Key" + type.ToString());
			return;
		}
		ColorData colorData = this.colorMap[type.ToString()];
		if (this.m_textList.Count < 27 || this.m_imageList.Count == 0)
		{
			SingleInstance<DebugManager>.Instance.LogError("changeTextColor list");
			return;
		}
		this.m_textList[0].color = tool.HexcolorTofloat(colorData.AchieveText);
		this.m_textList[1].color = tool.HexcolorTofloat(colorData.Continue);
		this.m_textList[2].color = tool.HexcolorTofloat(colorData.GamingCurScore);
		this.m_textList[3].color = tool.HexcolorTofloat(colorData.GamingHit);
		this.m_textList[4].color = tool.HexcolorTofloat(colorData.GamingHitScore);
		this.m_textList[5].color = tool.HexcolorTofloat(colorData.GamingScore);
		this.m_textList[6].color = tool.HexcolorTofloat(colorData.GradeText);
		this.m_textList[7].color = tool.HexcolorTofloat(colorData.HomeGetMoney);
		this.m_textList[8].color = tool.HexcolorTofloat(colorData.HomeMoney);
		this.m_textList[9].color = tool.HexcolorTofloat(colorData.Loading);
		this.m_textList[10].color = tool.HexcolorTofloat(colorData.OverCurScore);
		this.m_textList[11].color = tool.HexcolorTofloat(colorData.OverGetMoney);
		this.m_textList[12].color = tool.HexcolorTofloat(colorData.OverMoney);
		this.m_textList[13].color = tool.HexcolorTofloat(colorData.OverScore);
		this.m_textList[14].color = tool.HexcolorTofloat(colorData.OverShare);
		this.m_textList[15].color = tool.HexcolorTofloat(colorData.PauseContinue);
		this.m_textList[16].color = tool.HexcolorTofloat(colorData.PauseHead);
		this.m_textList[17].color = tool.HexcolorTofloat(colorData.PauseText);
		this.m_textList[18].color = tool.HexcolorTofloat(colorData.PauseTry);
		this.m_textList[19].color = tool.HexcolorTofloat(colorData.PopContinue);
		this.m_textList[20].color = tool.HexcolorTofloat(colorData.PopNew);
		this.m_textList[21].color = tool.HexcolorTofloat(colorData.Tips);
		this.m_textList[22].color = tool.HexcolorTofloat(colorData.Watch);
		this.m_textList[23].color = tool.HexcolorTofloat(colorData.HomeLevelBtn);
		this.m_textList[24].color = tool.HexcolorTofloat(colorData.OverLevelBtn);
		this.m_textList[25].color = tool.HexcolorTofloat(colorData.PauseScore);
		this.m_textList[26].color = tool.HexcolorTofloat(colorData.PauseText);
		this.m_textList[27].color = tool.HexcolorTofloat(colorData.PauseText);
		this.m_textList[28].color = tool.HexcolorTofloat(colorData.OverWatchMoney);
		this.m_textList[29].color = tool.HexcolorTofloat(colorData.WatchAdFree);
		this.m_textList[30].color = tool.HexcolorTofloat(colorData.WatchAdFree);
		this.m_textList[31].color = tool.HexcolorTofloat(colorData.WatchAdTip);
		this.m_textList[32].color = tool.HexcolorTofloat(colorData.WatchAdTip);
		this.m_imageList[0].color = tool.HexcolorTofloat(colorData.Loading);*/
	}

	private void LateUpdate() 
	{
#if UNITY_ANDROID
		if(Input.GetKeyUp(KeyCode.Clear) || Input.GetKeyUp(KeyCode.Escape))
		{	
			if(UIState() == false)
			{
				GameManager.Instance.GamePause();
				return;
			}

			if(ISNoticeOpen())
			{
				this.m_Notice.Close();
				return;
			}
			else if(this.m_ItemStore.UIState && this.m_ItemStore.itemInfoWindow.activeSelf)
			{
				this.m_ItemStore.CloseItemInfoWindow();
				return;
			}
			else if(this.m_Continue.UIState)
			{
				GameManager.Instance.ContinueToGameOver();
				return;
			}
			else if(this.m_GameOver.UIState)
			{
				GameManager.Instance.GameHome();
				return;
			}

			// 게임 종료 팝업
			if (this.m_Home.UIState && !IsStoreState())
			{
				if(!this.m_SelectPop.UIState)
				{
					OpenSelectPopPanel(Localization.Instance.GetLable("ExitGame"), ExitGame);
				}
				else
					this.m_SelectPop.Close();

				return;
			}
			if(this.m_Pause.UIState)
			{
				GameManager.Instance.PauseContinue();
			}
			CloseCurrentPanel(null, 0f);
		}
#endif	
	}

	private void ExitGame()
	{
		Debug.Log("_____ ExitGame !!!!! _____");

		Application.Quit();
	}
}
