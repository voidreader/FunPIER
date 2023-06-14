using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public sealed class Field_Hall : Field
{
	//---------------------------------------
	[Header("POLTERGEIST - OBJECTS")]
	[SerializeField]
	private BossRG_Poltergeist[] _ptgst_Objects = null;
	[SerializeField]
	private BossRG_Poltergeist[] _ptgst_Objects_NormalOnly = null;
	[SerializeField]
	private BossRG_Poltergeist[] _ptgst_Objects_HardOnly = null;
	[SerializeField]
	private float[] _ptgst_Velocity = null;

	//---------------------------------------
	[Header("PICTURE DUMIES")]
    [SerializeField]
    private BossRG_Picture[] _pictureDummy_1F = null;
    [SerializeField]
    private BossRG_Picture[] _pictureDummy_2F = null;
	[SerializeField]
	private float[] _pictureSwapDelay = null;
	[SerializeField]
	private AudioClip _pictureAttackSound = null;

	public BossRG_Picture[] PictureDummy_1F { get { return _pictureDummy_1F; } }
    public BossRG_Picture[] PictureDummy_2F { get { return _pictureDummy_2F; } }

	private Coroutine _pictureProc = null;
	private bool _pictureAttackEnabled = false;
	private float _pictureSwapLeastTime = 0.0f;

	private int _currentPicture = 0;

    //---------------------------------------
    [Header("ORGAN DUMMY")]
    [SerializeField]
    private DummyPivot[] _organ_PipeDummy = null;

    public DummyPivot[] Organ_PipeDummy { get { return _organ_PipeDummy; } }


	//---------------------------------------
	public override void Init()
	{
		base.Init();

		//-----
		if (_ptgst_Objects_NormalOnly != null && _ptgst_Objects_NormalOnly.Length > 0)
		{
			eGameDifficulty eCurDiff = GameFramework._Instance.m_pPlayerData.m_eDifficulty;
			bool bPoltObj_Normal = (eCurDiff >= eGameDifficulty.eNormal) ? true : false;

			for (int nInd = 0; nInd < _ptgst_Objects_NormalOnly.Length; ++nInd)
				_ptgst_Objects_NormalOnly[nInd].gameObject.SetActive(bPoltObj_Normal);
		}

		if (_ptgst_Objects_HardOnly != null && _ptgst_Objects_HardOnly.Length > 0)
		{
			eGameDifficulty eCurDiff = GameFramework._Instance.m_pPlayerData.m_eDifficulty;
			bool bPoltObj_Hard = (eCurDiff >= eGameDifficulty.eHard) ? true : false;

			for (int nInd = 0; nInd < _ptgst_Objects_HardOnly.Length; ++nInd)
				_ptgst_Objects_HardOnly[nInd].gameObject.SetActive(bPoltObj_Hard);
		}
	}


	//---------------------------------------
	public void SetPoltergest()
	{
		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;

		for(int nInd = 0; nInd < _ptgst_Objects.Length; ++nInd)
			if (_ptgst_Objects[nInd].gameObject.activeInHierarchy)
				_ptgst_Objects[nInd].SetForce(_ptgst_Velocity[nDiff]);
	}

	//---------------------------------------
	public void InitPictureProc(bool bTo1F)
	{
		BossRG_Picture[] vPictures = (bTo1F) ? PictureDummy_1F : PictureDummy_2F;

		_currentPicture = HT.RandomUtils.Range(0, vPictures.Length);
		vPictures[_currentPicture].SetIsBossPicture();

		for (int nInd = 0; nInd < vPictures.Length; ++nInd)
			vPictures[nInd].UpdatePicture();
	}

	public void SetIntoPictureProc(bool bTo1F)
	{
		_pictureAttackEnabled = false;
		_pictureProc = StartCoroutine(IntoPicture_Internal(bTo1F));
	}

	public void StopPictureProc()
	{
		HT.Utils.SafeStopCorutine(this, ref _pictureProc);

        BossRG_Picture.BossPictureDisable();
    }

	public void SetPictureCanAttack(bool bEnable)
	{
		_pictureAttackEnabled = bEnable;
	}

	public BossRG_Picture GetCurPicture()
	{
		return BossRG_Picture._bossPicture;
	}

	private IEnumerator IntoPicture_Internal(bool bTo1F)
	{
		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;

		float fSwapDelay = _pictureSwapDelay[nDiff];
		_pictureSwapLeastTime = fSwapDelay;

		//-----
		BossRG_Picture[] vPictures = (bTo1F) ? PictureDummy_1F : PictureDummy_2F;

		//-----
		while (true)
		{
			_pictureSwapLeastTime -= HT.TimeUtils.GameTime;
			if (_pictureSwapLeastTime < 0.0f)
			{
				_currentPicture += (HT.RandomUtils.Range(0.0f, 1.0f) > 0.5f) ? 1 : -1;
				if (_currentPicture < 0)
					_currentPicture = vPictures.Length - 1;

				else if (_currentPicture >= vPictures.Length)
					_currentPicture = 0;

				vPictures[_currentPicture].SetIsBossPicture();

                for (int nInd = 0; nInd < vPictures.Length; ++nInd)
                    vPictures[nInd].UpdatePicture();

                //-----
                if (_pictureAttackEnabled)
				{
					if (_pictureAttackSound != null)
						HT.HTSoundManager.PlaySound(_pictureAttackSound);

					for (int nInd = 0; nInd < vPictures.Length; ++nInd)
						vPictures[nInd].PictureAttack();
				}

				//-----
				_pictureSwapLeastTime = fSwapDelay;
			}

			yield return new WaitForEndOfFrame();
		}
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------