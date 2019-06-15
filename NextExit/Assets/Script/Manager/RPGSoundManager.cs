using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 사운드 메니서. 
/// </summary>
public class RPGSoundManager : MonoBehaviour
{
	public enum eSoundBGM
	{
		None	= -1,
		MainBGM,// = 0,
		GameBGM,
	}
	
	private static RPGSoundManager s_instance = null;
	private static GameObject gameObject_ = null;
	
	public static RPGSoundManager Instance
	{
		get
		{
			if( null == s_instance )
			{
				s_instance = FindObjectOfType( typeof( RPGSoundManager ) ) as RPGSoundManager;
				if( null == s_instance )
				{
					GameObject KSoundManager = new GameObject( "KSoundManager" );
					KSoundManager.transform.localScale = Vector3.one;
					KSoundManager.transform.localPosition = Vector3.zero;
					s_instance = KSoundManager.AddComponent<RPGSoundManager>();
					
					GameObject audioBgm = new GameObject( "audioBgm" );
					audioBgm.transform.parent = s_instance.transform;
					audioBgm.transform.localScale = Vector3.one;
					audioBgm.transform.localPosition = Vector3.zero;
					s_instance.audioBgm = KSoundManager.AddComponent<AudioSource>();
					
					GameObject audioEffect = new GameObject( "audioEffect" );
					audioEffect.transform.parent = s_instance.transform;
					audioEffect.transform.localScale = Vector3.one;
					audioEffect.transform.localPosition = Vector3.zero;
					
					s_instance.audioEffect = KSoundManager.AddComponent<AudioSource>();
				}
			}
			return s_instance;
		}
	}

	[Tooltip(" soundUI == true ? On : Off \n 효과음. ")]
	public bool soundUI = true;
	[Tooltip(" soundEffect == true ? On : Off \n 효과음. ")]
	public bool soundEffect = true;
	[Tooltip(" soundBgm == true ? On : Off \n 배경음. ")]
	public bool soundBgm = true;
	/// <summary>
	/// 사운드 볼름 설정 ( volume == 1.0 ? 최대  )
	/// </summary>
	public float volumeBgm = 1.0f;
	public float volumeEffect = 1.0f;
	/// <summary>
	/// 재생 되고 있는 배경음. 
	/// </summary>
	private eSoundBGM reserveBgmSoundIndex = eSoundBGM.None;
	
	public eSoundBGM getBgmSoundIndex{ get{ return reserveBgmSoundIndex; } }

	private void Awake()
	{
		init();
	}

	public void init()
	{
		RPGSoundManager.s_instance = this;
		RPGSoundManager.gameObject_ = gameObject;
		//신 전환 하여서 제거 되지 않는다. 
		//GameObject.DontDestroyOnLoad( this );
		
		GetSoundSetting();

		audioBgm.volume = volumeBgm;
		audioEffect.volume = 0.7f;
		audioUI.volume = volumeEffect;
	}
	
	/// <summary>
	/// 내부 세팅 값 확인. 
	/// </summary>
	private void GetSoundSetting()
	{
		if( PlayerPrefs.GetString( "SoundBgm" ).Length == 0 )
			PlayerPrefs.SetString( "SoundBgm", "Y" );
		
		if( PlayerPrefs.GetString( "SoundEffect" ).Length == 0 )
			PlayerPrefs.SetString( "SoundEffect", "Y" );

		if( PlayerPrefs.GetString( "VolumeBgm" ).Length == 0 )
			PlayerPrefs.SetString( "VolumeBgm", "1" );
		
		if( PlayerPrefs.GetString( "VolumeEffect" ).Length == 0 )
			PlayerPrefs.SetString( "VolumeEffect", "1" );


		SetSoundBgm( PlayerPrefs.GetString( "SoundBgm" ) =="Y" ? true : false );
		soundEffect = ( PlayerPrefs.GetString( "SoundEffect" ) == "Y" ? true : false );

		volumeBgm = float.Parse( PlayerPrefs.GetString( "VolumeBgm" ) );
		volumeEffect = float.Parse( PlayerPrefs.GetString( "VolumeEffect" ) );
	}
	
	public void Terminate()
	{
		GameObject.DestroyImmediate( gameObject_ );
	}
	
	//#######################################################################
	
	public void SoundPause()
	{
		AudioListener.pause = true;
	}
	
	public void SoundResume()
	{
		AudioListener.pause = false;
	}
	
	//#######################################################################
	[Tooltip("UI 사운드 재생 래스트. ")]
	public List<AudioClip> uiAudioList = new List<AudioClip>();
	[Tooltip("이펙트 사운드 리스트. ")]
	public List<AudioClip> effectAudioList = new List<AudioClip>();
	[Tooltip("배경음 사운드.  ")]
	public List<AudioClip> bgmAudioList = new List<AudioClip>();
	[Tooltip("배경음 재생 오브젝트.")]
	public AudioSource audioBgm;
	[Tooltip("효과음 재생 오브젝트.")]
	public AudioSource audioEffect;
	[Tooltip("UI 재생 오브젝트.")]
	public AudioSource audioUI;
	
	public void PlayUISound( int _index )
	{
		if( uiAudioList.Count == 0 )
			return ;
		if( !soundEffect )
			return;
		if( _index == 0 )
			return;
		if( _index-1 > uiAudioList.Count )
			return;

		audioUI.PlayOneShot( uiAudioList[_index-1] );
	}

	public void StopPlaySound()
	{
		audioUI.Stop();
	}

	/**/
	/// <summary>
	/// 사운드 리스트 기준의 index 사용. 
	/// </summary>
	/// <param name="_index">_index.</param>
	public void PlayEffectSound( int _index )
	{
		//if ( !soundEffect || audioEffect.isPlaying )
		if ( !soundEffect  )
			return;
		
		if( _index-1 > effectAudioList.Count )
			return;

		audioEffect.PlayOneShot( effectAudioList[_index-1] );
	}
	
	/// <summary>
	/// _loop ? 사운드 루프 : 루프 중단. 
	/// </summary>
	/// <returns><c>true</c>, if effect sound loop was played, <c>false</c> otherwise.</returns>
	/// <param name="_index">_index.</param>
	/// <param name="_loop">If set to <c>true</c> _loop.</param>
	public bool PlayEffectSoundLoop( int _index, bool _loop )
	{
		if( !soundEffect )
			return false;
		
		if( _index-1 > effectAudioList.Count )
			return false;
		
		audioEffect.loop = _loop;
		if( !_loop  )
		{
			audioEffect.Stop();
			audioEffect.clip = null;
		}
		else
		{
			if( audioEffect.isPlaying )
				return true;
			
			AudioClip clip = effectAudioList[_index-1];
			audioEffect.clip = clip;
			audioEffect.loop = true;
			//YSY::20141125::유니티의 버그인듯? play를 두번하지 않으면 정상적으로 루프되지 않는 버그가 있음.
			//bgm에서는 이상이 없는거보니.. PlayOneShot 이후에 생기는 버그인것 같음.
			audioEffect.Play();
		}
		return true;
	}
	
	
	public void PlayBgm()
	{
		int bgmPlayerIndex = (int)reserveBgmSoundIndex;
		//Debug.Log( bgmPlayerIndex-1  );
		if( bgmPlayerIndex > bgmAudioList.Count || bgmPlayerIndex < 0 )
			return;
		
		audioBgm.clip = bgmAudioList[bgmPlayerIndex];
		audioBgm.loop = true;
		audioBgm.Play();
	}
	/// <summary>
	/// 해당 index의 사운드 재생. 
	/// </summary>
	/// <param name="_index">_index.</param>
	public void PlayBgm( eSoundBGM _index )
	{
		if( reserveBgmSoundIndex == _index )
			return;
		reserveBgmSoundIndex = _index;
		int bgmPlayerIndex = (int)reserveBgmSoundIndex;
		if( !soundBgm )
		{
			//reserveBgmSoundIndex = eSoundBGM.None;
			return;
		}
		
		//Debug.Log( bgmPlayerIndex-1  );
		if( bgmPlayerIndex > bgmAudioList.Count || bgmPlayerIndex < 0 )
			return;
		
		audioBgm.clip = bgmAudioList[bgmPlayerIndex];
		audioBgm.loop = true;
		audioBgm.Play();
	}
	
	//	public void PlayOneShotBgm( int _index )
	//	{
	//		if( !soundBgm )
	//			return;
	//		
	//		if( _index-1 > bgmAudioList.Count )
	//			return;
	//		
	//		audioBgm.clip = bgmAudioList[_index-1];
	//		audioBgm.loop = false;
	//		audioBgm.Play();
	//	}
	
	/// <summary>
	/// 배경음 사운드 정지. 
	/// </summary>
	public void StopBgm()
	{
		audioBgm.Stop();
		reserveBgmSoundIndex = eSoundBGM.None;
	}
	
	//#######################################################################	
	/// <summary>
	/// _soundEffect ? 사운드 on : 사운드 off;
	/// </summary>
	/// <param name="_soundEffect">If set to <c>true</c> _sound effect.</param>
	public void SetSoundEffect( bool _soundEffect )
	{
		soundEffect = _soundEffect;
		PlayerPrefs.SetString( "SoundEffect", soundEffect ? "Y" : "N" );
		PlayerPrefs.Save();
	}
	
	/// <summary>
	/// _soundBgm ? 사운드 on : 사운드 off;
	/// </summary>
	/// <param name="_soundBgm">If set to <c>true</c> _sound bgm.</param>
	public void SetSoundBgm( bool _soundBgm )
	{
		soundBgm = _soundBgm;
		if( soundBgm )
			PlayBgm();
		else
			audioBgm.Stop();
		PlayerPrefs.SetString( "SoundBgm", soundBgm ? "Y" : "N" );
		PlayerPrefs.Save();
	}

	/// <summary>
	/// 효과음. Gets the sound effect.
	/// </summary>
	/// <returns><c>true</c>, if sound effect was gotten, <c>false</c> otherwise.</returns>
	public bool GetSoundEffect()
	{
		return soundEffect;
	}
	/// <summary>
	/// 배경음. true : 사운드 on : 사운드 off;
	/// </summary>
	/// <returns><c>true</c>, if sound bgm was gotten, <c>false</c> otherwise.</returns>
	public bool GetSoundBgm()
	{
		return soundBgm;
	}
	/// <summary>
	/// 효과음 : _soundEffect ? 사운드 on : 사운드 off;
	/// 배경음 : _soundBgm ? 사운드 on : 사운드 off;
	/// </summary>
	/// <param name="_effect">If set to <c>true</c> _effect.</param>
	/// <param name="_bgm">If set to <c>true</c> _bgm.</param>
	public void SoundSetting( bool _effect, bool _bgm )
	{
		soundEffect = _effect;
		
		soundBgm = _bgm;
		if( soundBgm )
			PlayBgm( reserveBgmSoundIndex );
		else
			audioBgm.Stop();
	}
	public void SetBgmVolume( float _volume )
	{
		volumeBgm = _volume;
		audioBgm.volume = volumeBgm;
		PlayerPrefs.SetString( "VolumeBgm", volumeBgm.ToString() );
	}
	public void SetEffectVolume( float _volume )
	{
		volumeEffect = _volume;
		audioEffect.volume = volumeEffect;
		audioUI.volume = volumeEffect;

		PlayerPrefs.SetString( "VolumeEffect", volumeEffect.ToString() );
	}
}
