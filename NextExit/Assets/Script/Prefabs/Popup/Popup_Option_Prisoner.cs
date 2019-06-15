using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_Option_Prisoner : RPGLayer
{
	public static Popup_Option_Prisoner show()
	{
		return RPGSceneManager.Instance.pushPopup<Popup_Option_Prisoner>( "Prefabs/Popup/Popup_Option_Prisoner" );
	}

	public tk2dUIItem m_ToggleBGM;
	public tk2dUIItem m_ToggleSE;

	public tk2dUIItem m_ToggleTest;

	public tk2dSprite sptieBGM;
	public tk2dSprite sptieSE;

	public tk2dSprite spriteTest;

	public RPGTextMesh textBgm;
	public RPGTextMesh textSe;

	public RPGTextMesh textTest;

	public tk2dBaseSprite bg_black;
	public Transform tranInfo;
	public List<tk2dUIItem> buttomList = new List<tk2dUIItem>();

	private bool m_action = false;

	public override void init()
	{
		base.init();
		StartCoroutine( C_StartAction() );

		if ( RPGSoundManager.Instance.soundEffect )
		{
			sptieSE.SetSprite( "ui_popup_btn_sel" );
			textSe.Text = "SE OFF";
		}
		else
		{
			sptieSE.SetSprite( "ui_popup_btn_idle" );
			textSe.Text = "SE ON";
		}

		if ( RPGSoundManager.Instance.soundBgm )
		{
			sptieBGM.SetSprite( "ui_popup_btn_sel" );
			textBgm.Text = "BGM OFF";
		}
		else
		{
			sptieBGM.SetSprite( "ui_popup_btn_idle" );
			textBgm.Text = "BGM ON";
		}

		if ( GameManager.Instance.is_Test )
		{
			spriteTest.SetSprite( "ui_popup_btn_sel" );
			textTest.Text = "OFF";
		}
		else
		{
			spriteTest.SetSprite( "ui_popup_btn_idle" );
			textTest.Text = "ON";
		}
	}

	private IEnumerator C_StartAction()
	{
		m_action = true;
		float time = 0.1f;
		float timer = 0f;
		for ( int index = 0; index < buttomList.Count; ++index )
			buttomList[index].enabled = false;
		Color startColor = new Color( 0f, 57f / 255f, 1f, 0f );
		Color endColor = new Color( 0f, 57f / 255f, 1f, 104f / 255f );

		while( true )
		{
			timer += Time.deltaTime;
			bg_black.color = Color.Lerp( startColor, endColor, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		timer = 0;
		while ( true )
		{
			timer += Time.deltaTime;
			tranInfo.localScale = Vector3.Lerp( Vector3.zero, Vector3.one, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		for ( int index = 0; index < buttomList.Count; ++index )
			buttomList[index].enabled = true;
		m_action = false;
	}


	private IEnumerator C_EndAction()
	{
		m_action = true;
		float time = 0.1f;
		float timer = 0f;
		for ( int index = 0; index < buttomList.Count; ++index )
			buttomList[index].enabled = false;
		Color startColor = new Color( 0f, 57f / 255f, 1f, 0f );
		Color endColor = new Color( 0f, 57f / 255f, 1f, 104f / 255f );

		while ( true )
		{
			timer += Time.deltaTime;
			tranInfo.localScale = Vector3.Lerp( Vector3.one, Vector3.zero, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		timer = 0;
		while ( true )
		{
			timer += Time.deltaTime;
			bg_black.color = Color.Lerp( endColor, startColor, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}

		removeFromParent();
	}

	void OnBtnClose()
	{
		RPGSoundManager.Instance.PlayUISound( 3 );
		OnPopupClose();
	}

	public override void OnPopupClose()
	{
		if ( m_action )
			return;
		m_action = true;
		StartCoroutine( C_EndAction() );
	}

	void OnBgmClick()
	{
		RPGSoundManager.Instance.PlayUISound( 3 );
		RPGSoundManager.Instance.SetSoundBgm( !RPGSoundManager.Instance.soundBgm );
		if ( RPGSoundManager.Instance.soundBgm )
		{
			sptieBGM.SetSprite( "ui_popup_btn_sel" );
			textBgm.Text = "BGM OFF";
		}
		else
		{
			sptieBGM.SetSprite( "ui_popup_btn_idle" );
			textBgm.Text = "BGM ON";
		}
	}

	void OnSeClick()
	{
		RPGSoundManager.Instance.PlayUISound( 3 );
		RPGSoundManager.Instance.SetSoundEffect( !RPGSoundManager.Instance.soundEffect );
		if ( RPGSoundManager.Instance.soundEffect )
		{
			sptieSE.SetSprite( "ui_popup_btn_sel" );
			textSe.Text = "SE OFF";
		}
		else
		{
			sptieSE.SetSprite( "ui_popup_btn_idle" );
			textSe.Text = "SE ON";
		}
	}

	void OnTestClick()
	{
		RPGSoundManager.Instance.PlayUISound( 3 );
		GameManager.Instance.is_Test = !GameManager.Instance.is_Test;
		if ( GameManager.Instance.is_Test )
		{
			spriteTest.SetSprite( "ui_popup_btn_sel" );
			textTest.Text = "OFF";
		}
		else
		{
			spriteTest.SetSprite( "ui_popup_btn_idle" );
			textTest.Text = "ON";
		}
	}
}
