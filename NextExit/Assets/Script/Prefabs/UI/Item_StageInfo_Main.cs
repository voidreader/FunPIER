using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_StageInfo_Main : MonoBehaviour
{
	public RPGLayer tranBlock;

	public GameObject cleckToObject;

	public GameObject objStageSelect;

	public GameObject objClear;
	public RPGTextMesh textTitle;
	public RPGTextMesh textState;
	public RPGTextMesh textTime;
	public RPGTextMesh textDeath;

	public GameObject objLoked;

	public tk2dSprite spriteStage;
	public tk2dSprite spriteStageInfo;

	public tk2dUIItem button;

	public int stageNum = -1;

	public int m_index = 0;

	public void init( int _index )
	{
		m_index = _index;
		//Debug.Log( "_index : " + _index );
		//Debug.Log( "GameManager.Instance.STAGE_LIST.Count : " + GameManager.Instance.STAGE_LIST.Count );
		if ( _index >= GameManager.Instance.STAGE_LIST.Count )
			_index -= GameManager.Instance.STAGE_LIST.Count;
		//Debug.Log( "_index : " + _index );
		if ( _index < 0 )
		{
			if( _index == -1 )
			{
				_index = GameManager.Instance.STAGE_LIST.Count - 1;
			}
			if ( _index == -2 )
			{
				_index = GameManager.Instance.STAGE_LIST.Count - 2;
			}
			if ( _index == -3 )
			{
				_index = GameManager.Instance.STAGE_LIST.Count - 3;
			}
		}
		if ( stageNum == _index )
			return;
		tranBlock.removeAllChild();
		stageNum = _index;
		//Debug.Log( "stageNum: " + stageNum );
		ArrayList _info = GameManager.Instance.STAGE_LIST[stageNum];
		if ( stageNum == 0 )
			textTitle.Text = "PROLOG";
		else if ( stageNum == 1 )
			textTitle.Text = "0th EXIT";
		else
			textTitle.Text = ( stageNum - 1 ).ToString() + "th EXIT";
		Dictionary<string, object> dic = DataSaveManager.Instance.GetStageClearInfo( ( stageNum + 1 ) );
		
		objStageSelect.gameObject.SetActive( false );
		if ( dic == null )
		{
			if ( stageNum == 0 )
			{
				button.enabled = true;
				objClear.gameObject.SetActive( false );
				objLoked.gameObject.SetActive( false );
				textState.gameObject.SetActive( true );
				textState.Text = "CHALLENGE";
				spriteStage.SetSprite( "ui_frm_slot_stage_challenge_js" );
				spriteStageInfo.SetSprite( "ui_frm_slot_stageinfo_challenge_js" );
				BlockManager.Instance.loadCustom( _info, tranBlock );
			}
			else
			{
                objClear.gameObject.SetActive(false);
                objLoked.gameObject.SetActive(false);
                textState.gameObject.SetActive(true);
                textState.Text = "CHALLENGE";
                spriteStage.SetSprite("ui_frm_slot_stage_challenge_js");
                spriteStageInfo.SetSprite("ui_frm_slot_stageinfo_challenge_js");
                button.enabled = true;
                BlockManager.Instance.loadCustom(_info, tranBlock);

                /*
				dic = DataSaveManager.Instance.GetStageClearInfo( stageNum );
				if ( dic != null )
				{
					objClear.gameObject.SetActive( false );
					objLoked.gameObject.SetActive( false );
					textState.gameObject.SetActive( true );
					textState.Text = "CHALLENGE";
					spriteStage.SetSprite( "ui_frm_slot_stage_challenge_js" );
					spriteStageInfo.SetSprite( "ui_frm_slot_stageinfo_challenge_js" );
					button.enabled = true;
					BlockManager.Instance.loadCustom( _info, tranBlock );
				}
				else
				{
					button.enabled = false;
					objClear.gameObject.SetActive( false );
					objLoked.gameObject.SetActive( true );
					textState.gameObject.SetActive( true );
					textState.Text = "LOCKED";
					spriteStage.SetSprite( "ui_frm_slot_stage_locked_js" );
					spriteStageInfo.SetSprite( "ui_frm_slot_stageinfo_locked_js" );
				}
                */
            }
		}
		else
		{
			button.enabled = true;
			objLoked.gameObject.SetActive( false );
			objClear.gameObject.SetActive( true );
			textState.gameObject.SetActive( false );
			textTime.Text = PrintTime( int.Parse( dic["playTime"].ToString() ) );
			textDeath.Text = dic["death"].ToString();
			spriteStage.SetSprite( "ui_frm_slot_stage_clear_js" );
			spriteStageInfo.SetSprite( "ui_frm_slot_stageinfo_clear_js" );
			//BlockManager.Instance.loadCustom( _info, tranBlock );
		}
	}

	string PrintTime( int playTime )
	{
		int PlayTime = playTime;

		int hour = 0;
		int min = 0;
		int sec = PlayTime % 60;
		if ( PlayTime >= 60 )
			min = PlayTime / 60;
		if ( PlayTime >= 60 * 60 )
			hour = PlayTime / 60 / 60;
		return string.Format( "{0}:{1}:{2}", hour.ToString( "D2" ), min.ToString( "D2" ), sec.ToString( "D2" ) );
	}

	public void OnClick()
	{
		cleckToObject.SendMessage( "OnStageClick", this );
	}

	public void StageSelect( bool _selest )
	{
		objStageSelect.gameObject.SetActive( _selest );
	}

}
