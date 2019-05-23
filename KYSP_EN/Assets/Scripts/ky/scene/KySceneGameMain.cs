using UnityEngine;
using System.Collections;

/// <summary>
/// 메인게임 모드 [공기읽기.] [읽지 않는다]의 게임 진행관리 씬.
/// </summary>
public class KySceneGameMain : KyScene {

	#region MonoBehaviour Methods

	public void Start() {

        


		mGameEngine = KyGameEngine.Create();
		mGameEngine.transform.parent = transform;
		if (GameMode == KyConst.GameMode.KukiYomi) {
			mStageList = KyApplication.StageInfo.StageList1;
			mIntermit = KySaveData.IntermitData1;
			mStageCount = Mathf.Min(KyConst.StageCountYomu, KyDebugPrefs.MainStageCountCap);
		} else if (GameMode == KyConst.GameMode.Yomanai) {
			mStageList = KyApplication.StageInfo.StageList2;
			mIntermit = KySaveData.IntermitData2;
			mStageCount = Mathf.Min(KyConst.StageCountYomanai, KyDebugPrefs.MainStageCountCap);
		}
		mState.ChangeState(StateIntroduction);
		//	ここで中断データの問題番号をロード。

		if (!DemoMode) {
            Debug.Log(" >>> Not Demo Mode  <<< :: " + mIntermit.StageIndex);
			mStageIndex = mIntermit.StageIndex;
		}
	}

	public override void Update() {
		if (mState != null) {
			mState.Execute();
		}
	}

    #endregion

    #region State Methods

    /// <summary>
    /// 처음부터 시작했을 때의 설명 화면을 실행하는 스테이트.
    /// </summary>
    private int StateIntroduction() {
		if (mState.Sequence == 0) {
			if (mStageIndex == 0) {
				KySoftKeys.Instance.SetLabels(KySoftKeys.Label.None, KySoftKeys.Label.None);
				if (GameMode == KyConst.GameMode.KukiYomi) {
					mGameEngine.StartScript("s003");
				} else if (GameMode == KyConst.GameMode.Yomanai) {
					mGameEngine.StartScript("s006");
				} else {
					mGameEngine.StartScript("s003");
				}
				mStageIndex++;
			}
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (!mGameEngine.Running) {
				mState.ChangeState(StateStageMain);
			}
		}
		return 0;
	}

    /// <summary>
    /// 문제를 실행하는 스테이트
    /// </summary>
    private int StateStageMain() {
		if (mState.Sequence == 0) {
			if (KyDebugPrefs.MainSkipDaimon) {
				mState.Sequence++;
			} else {
                //	제○장 신을 로딩해 실행(종료 후에 다음의 시퀀스에).
                KySceneDaimon scene = (KySceneDaimon)PushScene("KySceneDaimon");
				scene.Number = mStageIndex;
				mState.Sequence++;
			}
		} else if (mState.Sequence == 1) {
            //	문제 리스트에서 문제 ID를 취득해 스크립트 실행.
            KySoftKeys.Instance.SetLabels(KySoftKeys.Label.Intermit, KySoftKeys.Label.None);
			int stageId = mStageList[mStageIndex-1].MainID;
			string name = string.Format("{0:D4}", stageId);
			mGameEngine.StartScript(name);
			mState.Sequence++;
		} else if (mState.Sequence == 2) {
			mGameEngine.Pause(false);
			mState.Sequence++;
		} else if (mState.Sequence == 3) {
            //	스크립트 실행중의 폴링.
            if (KySoftKeys.Instance.SelectedButtonIndex == 0) {
                //	중단확인 대화상표
                KySoftKeys.Instance.ResetAllButtons();
				KySoftKeys.Instance.SetGuiEnabled(false);
				mGameEngine.Pause(true);
				KySceneDialog scene = (KySceneDialog)PushScene("KySceneDialog");
				scene.Message = KyText.GetText(22011);  //	게임을 중단합니까?
                mState.Sequence++;
			} else if (!mGameEngine.Running) {
                //	스크립트 종료
                KySoftKeys.Instance.EnableSoftKeys(false);
				mState.Sequence += 2;
				if (DemoMode) {
					mStageIndex = mStageIndex == mStageCount ? 1 : mStageIndex + 1;
					mState.Sequence = 0;
				}
			}
		} else if (mState.Sequence == 4) {
            //	중단확인 대화 결과 처리.
            KySceneDialog scene = GetChildScene<KySceneDialog>();
			if (scene.DialogResult == KySceneDialog.Result.Yes) {
				mState.ChangeState(StateLeave);
			} else if (scene.DialogResult == KySceneDialog.Result.No) {
				KySoftKeys.Instance.SetGuiEnabled(true);
				mState.ChangeState(StateStageMain);
				mState.Sequence = 2;
			}
			Destroy(scene.gameObject);
		} else if (mState.Sequence == 5) {
            //	중간 상태를 세이브
            mIntermit.StageIndex = mStageIndex + 1;
			bool scoreUp = GameMode == KyConst.GameMode.KukiYomi ?
				(mGameEngine.Result == 1) : (mGameEngine.Result == 2);
			mIntermit.StageResults[mStageIndex - 1] = scoreUp;

            //	secret 달성인가 어떤지를 조사해서,
            // 처음으로 달성했을 경우는 플래그를 설정해 세이브.

            int stageId = mStageList[mStageIndex - 1].MainID;
			bool secret = mGameEngine.CommandManager.GetVariable("secret") == 1.0f;
			if (secret) {
				int stageNo = KyApplication.StageInfo.Stages[stageId].StageNo - 1;
				if (!KySaveData.RecordData.SecretFlags[stageNo]) {
					KySaveData.RecordData.SecretFlags[stageNo] = true;
					KySaveData.Instance.Save(KySaveData.DataKind.Record);
				}
			}
			
			mStageIndex++;
			if (mStageIndex > mStageCount) {
                //	전문제가 종료했을 경우
                JudgeResult();
			} else {

                // Debug.Log("Check Pos #1");

                //	아직 계속되는 경우
                if (GameMode == KyConst.GameMode.KukiYomi) {
					KySaveData.Instance.Save(KySaveData.DataKind.Intermit1);
				} else if (GameMode == KyConst.GameMode.Yomanai) {
					KySaveData.Instance.Save(KySaveData.DataKind.Intermit2);
				}
			}
			mState.ChangeState(StateInterimResult);
		}
		return 0;
	}

    /// <summary>
    ///중간 결과를 표시하는(일지도 모르는) 스테이트.
    /// </summary>
    private int StateInterimResult() {
		if (mState.Sequence == 0) {
			if (KyDebugPrefs.MainSkipIntermit) {
				mState.Sequence++;
			} else {
                //	규정 수의 문제를 종료했다면 중간결과를 표시한다.
                if (mStageIndex % 5 == 1) {
					int chapterScore = 0;
					for (int i = 1; i <= 5; ++i) {
						if (mIntermit.StageResults[mStageIndex - 1 - i]) { chapterScore++; }
					}
					int chapter = (mStageIndex - 1) / 5 - 1;
					DebugUtil.Log("chapter:" + chapter);
					DebugUtil.Log("chapter score:" + chapterScore);
					int paramId = GameMode == KyConst.GameMode.KukiYomi ? 
						300 + chapter :
						400 + chapter;
					int imageIndex = KyDesignParams.GetParam(paramId);
					KySoftKeys.Instance.SetLabels(KySoftKeys.Label.None, KySoftKeys.Label.None);
					KyIntermitResultImage.GameMode = (int)GameMode;
					KyIntermitResultImage.ResultIndex = chapterScore;
					KyIntermitResultImage.ImageIndex = imageIndex;
					mGameEngine.StartScript("s004");
				}
				mState.Sequence++;
			}
		} else if (mState.Sequence == 1) {
            //	스크립트 종료 대기
            if (!mGameEngine.Running) {
				mState.ChangeState(StateFinalResult);
			}
		}
		return 0;
	}



	/// <summary>
	/// 最終結果を表示する(かもしれない)ステート。
	/// </summary>
	private int StateFinalResult() {
		if (mState.Sequence == 0) {

            Debug.Log(">>> Stage [" + mStageIndex + "] End! <<< ");

            // 10 스테이지마다 광고 오픈 
            if (mStageIndex > 1 && mStageIndex % 10 == 0) {

                Debug.Log(">>> Internal ad start <<< ");

                if (mStageIndex < 95) {

                    GoogleAdmobMgr.Instance.OpenFrontAD();

                }
            }

            //	모든 문제를 종료했다면 최종 결과를 보여준다.
            //if (mStageIndex >= mStageList.Length) {
            if (mStageIndex > mStageCount) {
				KySoftKeys.Instance.SetLabels(KySoftKeys.Label.None, KySoftKeys.Label.None);
				KySceneFinalResult scene = ChangeScene("KySceneFinalResult") as KySceneFinalResult;
				scene.GameMode = GameMode;
				mState.Sequence+=2;
			} else {
				mState.ChangeState(StateStageMain);
			}
		} else if (mState.Sequence == 1) {
            //스크립트 종료 대기
            mState.ChangeState(StateStageMain);
		} else if (mState.Sequence == 2) {
			mState.ChangeState(StateLeave);
			mState.Sequence = 1;
		}
		return 0;
	}

    /// <summary>
    ///	이 장면을 종료하기 위한 스테이트.
    /// </summary>
    private int StateLeave() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.SetGuiEnabled(false);
			ScreenFader.Main.FadeOut();
			KyAudioManager.Instance.StopAll();
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				ChangeScene("KySceneMainMenu");
			}
		}
		return 0;
	}

    #endregion

    #region Methods

    /// <summary>
    /// 여기서 종합 결과의 산출이나 갱신이나 세이브나 실시한다.
    /// </summary>
    private void JudgeResult() {
		KySaveData.Record record = KySaveData.RecordData;
		int gameMode = (int)GameMode;
		int lastScoreSum = record.GetScoreSum(gameMode, (int)KyConst.RecordType.Recent);
        //	문제 정오 정보에 따라 종합 득점을 산출.
        record.SetScoresByResults(gameMode, (int)KyConst.RecordType.Recent, mIntermit.StageResults);
        //	디버깅용 랜덤 득점
        if (KyDebugPrefs.MainRandomScore) {
			byte[] scores = GameMode == KyConst.GameMode.KukiYomi ? record.Scores[0] : record.Scores[3];
			int[] category = GameMode == KyConst.GameMode.KukiYomi ? KyApplication.StageInfo.StagesCategory1 : KyApplication.StageInfo.StagesCategory2;
			for (int i = 0; i < KyConst.ScoreCategoryCount; ++i) {
				scores[i] = (byte)Random.Range(0, category[i] + 1);
			}
		}

        //	최고 최저 스코어의 갱신
        int scoreMax = record.GetScoreSum(gameMode, (int)KyConst.RecordType.Maximum);
		int scoreMin = record.GetScoreSum(gameMode, (int)KyConst.RecordType.Minimum);
		int scoreSum = record.GetScoreSum(gameMode, (int)KyConst.RecordType.Recent);
		int clearCount = record.GameClearCount[(int)GameMode];

		byte[] recent = record.GetScores(gameMode, (int)KyConst.RecordType.Recent);
		if (clearCount == 0 || scoreSum >= scoreMax) {
			byte[] max = record.GetScores(gameMode, (int)KyConst.RecordType.Maximum);
			System.Array.Copy(recent, max, recent.Length);
		}
		if (clearCount == 0 || scoreSum <= scoreMin) {
			byte[] min = record.GetScores(gameMode, (int)KyConst.RecordType.Minimum);
			System.Array.Copy(recent, min, recent.Length);
		}
		KySaveData.ContextData.RecordBetterThanLast = (scoreSum > lastScoreSum);
		KySaveData.ContextData.RecordPerfect = record.GetScoreSumInPercent(gameMode, (int)KyConst.RecordType.Recent) == 100;

        //	클리어 횟수를 인크리먼트
        record.GameClearCount[gameMode] = (byte)Mathf.Clamp(clearCount + 1, 0, 100);

		mIntermit.StageIndex = 0;
		if (GameMode == KyConst.GameMode.KukiYomi) {
			KySaveData.Instance.Save(KySaveData.DataKind.Intermit1);
		} else if (GameMode == KyConst.GameMode.Yomanai) {
			KySaveData.Instance.Save(KySaveData.DataKind.Intermit2);
		}
		KySaveData.Instance.Save(KySaveData.DataKind.Record);
	}

	#endregion

	#region Fields

	public KyConst.GameMode GameMode = 0;   //	게임 모드。
    public bool DemoMode = false;
	private KyStateManager mState = new KyStateManager();   //	스테이트 관리
    private KyGameEngine mGameEngine;   //	스크립트 엔진
    private KyStageInfo.Stage[] mStageList; //	문제 리스트의 참조
    private KySaveData.Intermit mIntermit;  //	중간 데이터에 대한 참조.
    private int mStageIndex = 0;    //	다음 문제 번호。
    private int mStageCount = 0;    //	전체 문제수。

    #endregion
}
