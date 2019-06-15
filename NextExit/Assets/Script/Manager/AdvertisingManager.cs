using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using RPG.AntiVariable;

using UnityEngine.Advertisements;

public class AdvertisingManager : MonoBehaviour
{
	private static AdvertisingManager s_instance = null;
	static public AdvertisingManager Instance
	{
		get
		{
			if ( !s_instance )
			{
				s_instance = GameObject.FindObjectOfType( typeof( AdvertisingManager ) ) as AdvertisingManager;
				if ( !s_instance )
					s_instance = new GameObject( "AdvertisingManager" ).AddComponent<AdvertisingManager>();
			}
			return s_instance;
		}
	}

	public enum eAdvertisingType
	{
		None,
		UnityAds,
		AdColony,
	}

    const string _KEY_SHOW_REBUY_AD = "_KEY_SHOW_REBUY_AD";

	public eAdvertisingType AdvertisingType = eAdvertisingType.None;

	private int unityAdsMaxCount = 20;
	private int unityAdsCount = 0;
	private void Awake()
	{
		//AdInit();
	}
	public void init( bool _isTest )
	{
		AdInit();
	}

	private DateTime m_dateTime;

	/// <summary>
	/// 광고 횟수 초기화. 
	/// </summary>
	public void AdInit()
	{
		string ads = HPlayerPrefs.GetString( "adsOn", "" );


        // 최초에는 adsOnCheck 가 true다. (true = 광고 플레이)
		if ( ads == null || ads.Length <= 0 )
		{
			adsOnCheck = true;
			HPlayerPrefs.SetString( "adsOn", adsOnCheck.ToString() );
		}
		else
		{
			adsOnCheck = bool.Parse( ads );
		}
		ads = HPlayerPrefs.GetString( "adsTime", "" );
		if ( ads == null || ads.Length <= 0 )
		{
			adsTime = 0;
			HPlayerPrefs.SetString( "adsTime", adsTime.ToString() );
		}
		else
		{
			adsTime = float.Parse( ads );
		}

		ads = HPlayerPrefs.GetString( "adsTimer", "" );
		if ( ads == null || ads.Length <= 0 )
		{
			adsTimer = 0;
			HPlayerPrefs.SetString( "adsTimer", adsTimer.ToString() );
		}
		else
		{
			adsTimer = float.Parse( ads );
		}
		ads = HPlayerPrefs.GetString( "timeAds", "" );

		if ( ads == null || ads.Length <= 0 )
		{
			timeAds = true;
			HPlayerPrefs.SetString( "timeAds", timeAds.ToString() );
		}
		else
		{
			timeAds = bool.Parse( ads );
		}

        Advertisement.Initialize("1620806", false);


	}


    /// <summary>
    /// 유니티 애즈 동영상 광고 플레이 
    /// </summary>
	public void ShowRewardedAd()
	{
		Debug.Log( "rewardedVideo" );
		Debug.Log( "Advertisement.IsReady( rewardedVideo ) : " + Advertisement.IsReady() );
		//*

        
		if ( !AdvertisingManager.Instance.adsOnCheck )
			return;

		if ( Advertisement.IsReady() )
		{
			var options = new ShowOptions
			{
				resultCallback = HandleShowResult
			};
			Advertisement.Show( options );
		}
	}



	/// <summary>
    /// 유니티 애즈 플레이 콜백 
    /// </summary>
    /// <param name="result"></param>
	private void HandleShowResult( ShowResult result )
	{
		Debug.Log( "result : " + result );
		switch ( result )
		{
		case ShowResult.Finished:	//유저가 동영상을 끝까지 본경우
		break;
		case ShowResult.Skipped:	//유저가 중간에 동영상을 스킵한경우
		break;
		case ShowResult.Failed:		//동영상 보여주기 실패한경우
		break;
		}
	}

	//###########################################################################
	//구글 전면 광고
	private int m_countMin = 15;
	private int m_gameStartCount = 20;
	private int m_gameStartCounting = 0;

	public bool adsOnCheck = true;

	public bool timeAds = true;

	private float adsTime = 0f;
	private float adsTimer = 0f;


	private DateTime m_gameTime;

	public bool AdsOpen
	{
		get
		{
			if ( !adsOnCheck )
				return false;
			return timeAds;
		}
	}

    /// <summary>
    /// YSY::재구매 팝업을 띄울지 여부.
    /// (AdsOpen && IsReBuyAD) = 재구매 팝업을 띄워야함.
    /// 띄운 이후에는 IsReBuyAD = false로 변경.
    /// </summary>
    public bool IsReBuyAD
    {
        get
        {
            return HPlayerPrefs.GetInt(_KEY_SHOW_REBUY_AD, 0) == 1?true:false;
        }
        set
        {
            HPlayerPrefs.SetInt(_KEY_SHOW_REBUY_AD, value?1:0);
            HPlayerPrefs.Save();
        }
    }

	public void SetAdsOn( bool _adsOn )
	{
		adsOnCheck = _adsOn;
		HPlayerPrefs.SetString( "adsOn", adsOnCheck.ToString() );

		//AndroidManager.GetInstance.OpenSavedGame( true );
        //YSY::재구매 가능한 상태로 추가.
        IsReBuyAD = true;
	}

	public void SetAdsTime( float time )
	{
		adsTime += time;
		if ( adsTime > 0 )
		{
			if ( timeAds )
			{
				m_gameTime = UnbiasedTime.Instance.Now();
				WriteTimestamp( "AdsTimeData", m_gameTime );

			}
			timeAds = false;

		}
		HPlayerPrefs.SetString( "adsTime", adsTime.ToString() );
		HPlayerPrefs.SetString( "adsTimer", adsTimer.ToString() );
		HPlayerPrefs.SetString( "timeAds", timeAds.ToString() );

        //YSY::재구매 가능한 상태로 추가.
        IsReBuyAD = true;
	}

	private DateTime ReadTimestamp( string key, DateTime defaultValue )
	{
		long tmp = Convert.ToInt64( HPlayerPrefs.GetString( key, "0" ) );
		if ( tmp == 0 )
		{
			return defaultValue;
		}
		return DateTime.FromBinary( tmp );
	}

	private void WriteTimestamp( string key, DateTime _time )
	{
		HPlayerPrefs.SetString( key, _time.ToBinary().ToString() );
	}


	void Update()
	{
		if( !timeAds )
		{
			DateTime loadTime = ReadTimestamp( "AdsTimeData", UnbiasedTime.Instance.Now() );
			m_gameTime = UnbiasedTime.Instance.Now();
			TimeSpan span = m_gameTime - loadTime;

			adsTimer += Time.deltaTime;
			if ( adsTime <= span.TotalSeconds )
			{
				adsTime = 0f;
				adsTimer = 0f;
				timeAds = true;
				HPlayerPrefs.SetString( "adsTime", adsTime.ToString() );
				HPlayerPrefs.SetString( "adsTimer", adsTimer.ToString() );
				HPlayerPrefs.SetString( "timeAds", timeAds.ToString() );
			}
		}
	}

	public void SetGameOverCount()
	{
		m_gameStartCounting++;
		//HPlayerPrefs.SetString( "m_gameStartCounting", m_gameStartCounting.ToString() );
	}






}
