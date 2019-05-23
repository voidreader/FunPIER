using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

/// <summary>
/// XML 관리자로 보인다.
/// </summary>
public class KyGameEngine : KyScene {

	#region MonoBehaviour Methods

	void Awake () {
		mElapsedTime = 0.0f;
		mFrameCount = 0;
		mNextFrame = 0;
		mMoveFrame = -1;
		mNextState = null;
		mScriptReader = new KyScriptReader();
	}

	void Start () {
		mGameCamera = KyUtil.GetComponentInChild<Camera>(gameObject, "GameCamera");
		Assert.AssertNotNull(mGameCamera);
		mTargetScreen = KyUtil.GetComponentInChild<MeshRenderer>(gameObject, "TargetScreen");
		Assert.AssertNotNull(mTargetScreen);
		mGameCamera.enabled = false;
		mTargetScreen.enabled = false;
	}

	private void StateScriptOneFrame() {
		mScriptReader.ReadFromResName("GameScripts/" + ScriptNames[ScriptIndex]);
		mCommandManager = mScriptReader.CommandManager;
		Assert.AssertTrue(mCommandManager != null);
		mCommandManager.RootObject = gameObject;

		mCommandManager.Prepare();
		mCommandManager.Update();

		mNextState = null;
	}

	private void StateScriptBegin() {
		System.GC.Collect();
		Resources.UnloadUnusedAssets();

		mScriptReader.ReadFromResName("GameScripts/" + ScriptNames[ScriptIndex]);
		mCommandManager = mScriptReader.CommandManager;
		Assert.AssertTrue(mCommandManager != null);
		mCommandManager.RootObject = gameObject;
		mNextState = StateScriptMain;

		DebugUtil.Log("ScriptBegin :: " + ScriptNames[ScriptIndex]);
	}

	private void StateScriptMain() {
		if (mSequence == 0) {
			if (mCommandManager.UseTrimWindow) {
				mGameCamera.enabled = true;
				mTargetScreen.enabled = true;
			}
			bool yuragiEnabled = KySaveData.PrefsData.YuragiEnabled && mCommandManager.UseYuragi;
			KyYuragiPlane yuragi = mTargetScreen.GetComponent<KyYuragiPlane>();
			Assert.AssertNotNull(yuragi);
			yuragi.SetYuragiEnabled(yuragiEnabled);
			mCommandManager.Prepare();

			ScreenFader.Main.FadeIn();
			mSequence++;
		} else if (mSequence == 1) {
			mCommandManager.Update();
			if (!ScreenFader.Main.FadeRunning) {
				KySoftKeys.Instance.EnableSoftKeys(true);
				mSequence++;
			} else if (mCommandManager.IsFinished) {
				KySoftKeys.Instance.EnableSoftKeys(false);
				ScreenFader.Main.FadeOut();
				mSequence += 2;
			}
		} else if (mSequence == 2) {
			mCommandManager.Update();
			if (mCommandManager.IsFinished) {
				KySoftKeys.Instance.EnableSoftKeys(false);
				ScreenFader.Main.FadeOut();
				mSequence++;
			}
		} else if (mSequence == 3) {
			if (!ScreenFader.Main.FadeRunning) {
				mNextState = StateScriptEnd;
			}
		}
	}

	private void StateScriptMainPreview() {
		if (mSequence == 0) {
			System.GC.Collect();
			Resources.UnloadUnusedAssets();
			DebugUtil.Log("ScriptBegin");
			mSequence++;
		} else if (mSequence == 1) {
			if (mCommandManager != null) {
				mCommandManager.DestroyAllKyObjects();
			}
			mSequence++;
		} else if (mSequence == 2) {
			mScriptReader.ReadFromResName("GameScripts/" + ScriptNames[ScriptIndex]);
			mCommandManager = mScriptReader.CommandManager;
			mCommandManager.RootObject = gameObject;
			mCommandManager.PreviewMode = true;
			if (mCommandManager.UseTrimWindow) {
				mGameCamera.enabled = true;
				mTargetScreen.enabled = true;
			}
			bool yuragiEnabled = KySaveData.PrefsData.YuragiEnabled && mCommandManager.UseYuragi;
			KyYuragiPlane yuragi = mTargetScreen.GetComponent<KyYuragiPlane>();
			yuragi.SetYuragiEnabled(yuragiEnabled);
			yuragi.SetupPreview();
			mCommandManager.Prepare();

			KyCommandFadeScreen cmd = new KyCommandFadeScreen();
			cmd.Duration = 60;
			cmd.Kind = KyCommandFadeScreen.FadeKind.FadeIn;
			cmd.Execute(mCommandManager);

			//ScreenFader.Main.FadeIn();
			mSequence++;
		} else if (mSequence == 3) {
			mCommandManager.Update();
			if (mCommandManager.IsFinished) {
				//mSequence = 1;
				//KyCommandFadeScreen cmd = new KyCommandFadeScreen();
				//cmd.Duration = 60;
				//cmd.Kind = KyCommandFadeScreen.FadeKind.FadeOut;
				//cmd.Execute(mCommandManager);
				//ScreenFader.Main.FadeOut();
				mSequence++;
			}
		} else if (mSequence == 4) {
			//if (!ScreenFader.Main.FadeRunning) {
			//	mCommandManager.DestroyAllKyObjects();
			//	mSequence++;
			//}
		}
	}

	private void StateScriptEnd() {
		if (mSequence == 0) {
			mCommandManager.DestroyAllKyObjects();
			mGameCamera.enabled = false;
			mTargetScreen.enabled = false;
			mFrameCount = 0;
			DebugUtil.Log("ScriptEnd");
			DebugUtil.Log("ScriptResult = " + this.Result);
			//DEBUG:結果通知用のサウンド
			if (KyDebugPrefs.StageResultSound) {
				int result = (int)mCommandManager.GetVariable("result");
				if (result == 1) {
					KyAudioManager.Instance.PlayOneShot("se_bell", 1.0f, 1.0f);
				} else if (result == 2) {
					KyAudioManager.Instance.PlayOneShot("se_cancel", 1.0f, 1.0f);
				}
			}
			mSequence++;
		} else if (mSequence == 1) {
			if (mFrameCount == 30) {
				int secret = (int)mCommandManager.GetVariable("secret");
				if (secret == 1) {
					DebugUtil.Log("Secret!!");
					//DEBUG:シークレット達成通知用のサウンド
					if (KyDebugPrefs.StageResultSound) {
						KyAudioManager.Instance.PlayOneShot("se_tennis", 1.0f, 1.0f);
					}
				}
				if (Looping) {
					mNextState = StateScriptBegin;
					ScriptIndex = (ScriptIndex + 1) % ScriptNames.Length;
				} else {
					mNextState = null;
				}
			} else {
				mFrameCount++;
			}
		}
	}

	#endregion

	#region Methods

	public GameObject FindPrefab(string name) {
		return (GameObject)Resources.Load(ResourcePath + name, typeof(GameObject));
	}

	public void MoveFrame(int frame) {
		DebugUtil.Log("MoveFrame = " + frame);
		mMoveFrame = frame;
	}

	public void StartScript(string name) {
		if (mState == null) {
			ScriptNames = new string[] { name };
			ScriptIndex = 0;
			mNextState = StateScriptBegin;
		}
	}

	public void StartScriptAsPreview(string name) {
		PreviewMode = true;
		ScriptNames = new string[] { name };
		ScriptIndex = 0;
		mNextState = StateScriptMainPreview;
		mSequence = 0;
	}

	public void StartScriptOneFrame(string name) {
		if (mState == null) {
			ScriptNames = new string[] { name };
			ScriptIndex = 0;
			mNextState = StateScriptOneFrame;
		}
	}

	public void EndScript() {
		mCommandManager.HaltCommand();
	}

	public void Pause(bool pause) {
		if (mCommandManager != null) {
			mCommandManager.Pause(pause);
		}
	}

	public static KyGameEngine Create() {
		GameObject prefab = (GameObject)Resources.Load("GamePrefabs/KyGameEngine");
		Assert.AssertNotNull(prefab);
		GameObject obj = (GameObject)Instantiate(prefab);
		Assert.AssertNotNull(obj);
		//GameObject obj = new GameObject("KyEngineRoot");
		KyGameEngine engine = obj.GetComponent<KyGameEngine>();
		obj.layer = 0;
		return engine;
	}

	#endregion

	#region Properties

	public int FrameCount {
		get { return mFrameCount; }
	}

	public int NextFrame {
		get { return mNextFrame; }
		set { mNextFrame = value; }
	}

	public bool Running {
		get { return (mState != null) && (mNextState != null); }
	}

	public int Result {
		get { return mCommandManager.ScriptResult; }
		set { mCommandManager.ScriptResult = value; }
	}

	public KyCommandManager CommandManager {
		get { return mCommandManager; }
	}

	public Transform TargetScreenTransform {
		get {
			return KyUtil.GetComponentInChild<Transform>(gameObject, "TargetScreen");
		}
	}

	#endregion

	#region Fields

	public string[] ScriptNames;
	public int ScriptIndex = 0;
	public bool Looping = false;
	public bool EndlessMode = false;
	public bool PreviewMode = false;
	public GameObject EngineRoot;

	private const string ResourcePath = "GamePrefabs/";
	private const float FramePerSec = 60.0f;
	private const float SecPerFrame = 1.0f / FramePerSec;

	protected float mElapsedTime;
	protected int mFrameCount;
	protected int mNextFrame;
	protected int mMoveFrame;
	protected int mFrame;
	protected KyScriptReader mScriptReader;
	protected KyCommandManager mCommandManager;
	protected Camera mGameCamera;
	protected MeshRenderer mTargetScreen;

	#endregion
}

