using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KyVariable {

	public enum VarType {
		Immediate = 0,
		Variable,
	}

	public KyVariable(float value) {
		Type = VarType.Immediate;
		Value = value;
	}

	public KyVariable(string name) {
		Type = VarType.Variable;
		VarName = name;
	}

	public VarType Type = VarType.Immediate;
	public float Value = 0.0f;
	public string VarName = "";

}

public class KyCommandManager {

	#region Methods

	public KyCommandManager() {
		mCommandList = new List<KyCommand>();
		mFrameMap = new Dictionary<int, int>();
		mVarTable = new Dictionary<string, float>();
		Clear();
	}

	public void Clear() {
		mCommandPos = 0;
		mFramePos = 0;
		mNextFramePos = 0;
		mMoveFramePos = -1;
		mMovePriority = -1;
		mFinished = false;
		mFinishing = false;
		mWaiting = false;
		mUseTrimWindow = false;
		mUpdateTime = 1.0f / 60.0f;

		mCommandList.Clear();
		mFrameMap.Clear();
		mVarTable.Clear();
	}

	public KyCommand GetNext() {
		if (mCommandPos < mCommandList.Count) {
			KyCommand cmd = mCommandList[mCommandPos];
			mCommandPos++;
			return cmd;
		}
		else {
			return null;
		}
	}

	public void MoveFrame(int frame, int priority) {
		if (mMovePriority <= priority) {
			mMoveFramePos = frame;
			mMovePriority = priority;
			//mWaiting = false;
		}
	}

	public void HaltCommand() {
		mFinishing = true;
	}

	public GameObject Find(string name) {
		return KyUtil.FindChildRecursively(RootObject, name);
	}

	public GameObject CreateKyObject(string id, string resName) {
		if (resName == null) {
			resName = id;
		}

		GameObject instance = null;
		GameObject prefab = (GameObject)Resources.Load("GameTextures/" + resName, typeof(GameObject));
		if (prefab == null) {
			prefab = (GameObject)Resources.Load("GamePrefabs/" + resName, typeof(GameObject));
		}
		if (prefab != null) {
			instance = (GameObject)GameObject.Instantiate(prefab);
		} else {
			Texture texture = (Texture)Resources.Load("GameTextures/" + resName, typeof(Texture));
			if (texture == null) {
				Debug.LogWarning(string.Format("Texture {0} not found.", resName));
				return null;
			}
			instance = new GameObject(id);
			SpriteUtil.CreateSpriteFromTexture(instance, texture);
		}
		instance.tag = "KyObject";
		if (mUseTrimWindow) {
			instance.layer = 8;
			KyUtil.ForEachChildIn(instance, delegate(GameObject obj) { obj.layer = 8; }, true);
		}
		instance.name = id;
		instance.transform.parent = RootObject.transform;
		//	まずは非表示
		//KyUtil.SetVisibleWithChildren(instance, false);

		return instance;
	}

	public GameObject CreateKyObject(string id) {
		GameObject instance = new GameObject(id);
		instance.tag = "KyObject";
		instance.name = id;
		instance.transform.parent = RootObject.transform;
		if (mUseTrimWindow) {
			instance.layer = 8;
			KyUtil.ForEachChildIn(instance, delegate(GameObject obj) { obj.layer = 8; }, true);
		} 
		return instance;
	}

	public void DestroyAllKyObjects() {
		GameObject[] objects;
		objects = GameObject.FindGameObjectsWithTag("KyObject");
		foreach (GameObject obj in objects) {
			GameObject.Destroy(obj);
		}
		objects = GameObject.FindGameObjectsWithTag("KyCondition");
		foreach (GameObject obj in objects) {
			GameObject.Destroy(obj);
		}
	}

	public void BeginCommand() {
		Clear();
	}

	public void EndCommand() {
		mFrameMap.Clear();
		for (int i = 0; i < mCommandList.Count; ++i) {
			if (mCommandList[i].GetType() == typeof(KyCommandFrame)) {
			//if (mCommandList[i].Type == KyCommandType.Frame) {
				KyCommandFrame cmdFrame = (KyCommandFrame)mCommandList[i];
				mFrameMap[cmdFrame.Count] = i;
			}
		}
	}

	public void AddCommand(KyCommand cmd) {
		mCommandList.Add(cmd);
	}

	public void Prepare() {
		if (mUseTrimWindow) {
			//GameObject obj = CreateKyObject("KyEngineWindow", null);
			//Vector3 pos = obj.transform.localPosition;
			//pos.z = -5;
			//obj.transform.localPosition = pos;
		}
	}

	public KyResult ExecOneFrame() {
		KyResult result = KyResult.Success;
		KyScriptTime.DeltaTime = 0;
		if (mFinishing) {
			mFinished = true;
			mFinishing = false;
		}
		if (mPausing) { return result; }
		if (mFinished) { return result; }

		KyScriptTime.DeltaTime = Time.deltaTime;
		//mMovePriority = -1;
		ExecMoveFrame();
		if (mWaiting) { return result; }
		while (mFramePos == mNextFramePos) {
			KyCommand cmd = GetNext();
			if (cmd == null) {
				mFinished = true;
				return result;
			}
			result = (KyResult)cmd.Execute(this);
			mLastResult = result;
			mLastCommandTag = cmd.GetType().Name;
			mLastCommandIndex = mCommandPos;
			if (result != KyResult.Success) {
				DebugUtil.Log("Error : " + result.ToString());
				DebugUtil.Log("Last Command Tag : " + LastCommandTag);
				DebugUtil.Log("Last Command Index : " + LastCommandIndex);
				break;
			}
			if (mMoveFramePos >= 0) {
				break;
			}
		}
		if (mMoveFramePos < 0) {
			++mFramePos;
		}

		return result;
	}

	public void SetVariable(string name, float value) {
		mVarTable[name] = value;
	}

	public float GetVariable(string name) {
		if (mVarTable.ContainsKey(name)) {
			return mVarTable[name];
		} else {
			return 0.0f;
		}
	}

	public float GetEvalValue(KyVariable variable) {
		if (variable.Type == KyVariable.VarType.Variable) {
			return GetVariable(variable.VarName);
		} else {
			return variable.Value;
		}
	}

	public void ClearVariables() {
		mVarTable.Clear();
	}

	public void Pause(bool pause) {
		if (mPausing != pause) {
			mPausing = pause;
			KyAudioManager.Instance.Pause(pause);
		}
	}

	public void Update() {
		mElapsedTime += Time.deltaTime;
		if (mElapsedTime > mUpdateTime * 5) {
			mElapsedTime = mUpdateTime * 5;
		}
		while (mElapsedTime >= mUpdateTime) {
			ExecOneFrame();
			mElapsedTime -= mUpdateTime;
		}
	}

	private void ExecMoveFrame() {
		if (mMoveFramePos >= 0) {
			DebugUtil.Log("MoveFrame : " + mMoveFramePos);
			bool moved = false;
			int lastKeyFrame = 0;
			mFramePos = mMoveFramePos;
			foreach (int keyFrame in mFrameMap.Keys) {
				lastKeyFrame = keyFrame;
				if (keyFrame >= mMoveFramePos) {
					mCommandPos = mFrameMap[keyFrame];
					mNextFramePos = keyFrame;
					moved = true;
					break;
				}
			}
			if (!moved) {
				mCommandPos = mCommandList.Count;
				mNextFramePos = lastKeyFrame;
				moved = true;
			}
			mMoveFramePos = -1;
			mMovePriority = -1;
			mWaiting = false;
			//DebugUtil.Log("CommandPos : " + mCommandPos);
		}/* else {
			++mFramePos;
		}*/
	}

	#endregion

	#region Properties

	/// <summary>
	/// コマンドリストを取得します。
	/// コマンドを追加するにはAddCommandを使用します。
	/// </summary>
	public List<KyCommand> CommandList {
		get { return mCommandList; }
	}

	public Dictionary<int, int> FrameMap {
		get { return mFrameMap; }
	}

	/// <summary>
	/// 現在処理しているコマンドを取得します。
	/// </summary>
	public KyCommand CurrentCommand {
		get {
			if (mCommandPos < mCommandList.Count) {
				return mCommandList[mCommandPos];
			}
			else {
				return null;
			}
		}
	}

	/// <summary>
	/// 現在のフレーム位置を取得します。
	/// </summary>
	public int FramePosition {
		get { return mFramePos; }
	}

	public int NextFramePosition {
		get { return mNextFramePos; }
		set { mNextFramePos = value; }
	}

	/// <summary>
	/// スクリプトコマンドの実行が完全に終了したかどうかを取得します。
	/// </summary>
	public bool IsFinished {
		get { return mFinished; }
	}

	public KyResult LastResult {
		get { return mLastResult; }
	}

	public string LastCommandTag {
		get { return mLastCommandTag; }
	}

	public int LastCommandIndex {
		get { return mLastCommandIndex; }
	}

	public int ScriptResult {
		get { return (int)GetVariable("result"); }
		set { SetVariable("result", value); }
	}

	public bool Waiting {
		get { return mWaiting; }
		set { mWaiting = value; }
	}

	public float UpdateTime {
		get { return mUpdateTime; }
		set { mUpdateTime = value; }
	}

	public bool UseTrimWindow {
		get { return mUseTrimWindow; }
		set { mUseTrimWindow = value; }
	}

	public bool UseYuragi {
		get { return mUseYuragi; }
		set { mUseYuragi = value; }
	}

	public bool PreviewMode {
		get { return mPreviewMode; }
		set { mPreviewMode = value; }
	}

	public GameObject RootObject {
		get { return mRootObject; }
		set { mRootObject = value; }
	}

	#endregion

	#region Fields

	private const string PrefabsPath = "GamePrefabs/";
	private const string TexturesPath = "GameTextures/";

	private int mCommandPos;
	private int mFramePos;
	private int mNextFramePos;
	private int mMoveFramePos;
	private int mMovePriority;
	private bool mFinished;
	private bool mFinishing;
	private bool mWaiting;
	private bool mPausing;
	private int mWateFrame;
	private float mUpdateTime = 1.0f / 60.0f;
	private float mElapsedTime;

	private List<KyCommand> mCommandList;
	private Dictionary<int, int> mFrameMap;
	private Dictionary<string, float> mVarTable;

	private KyResult mLastResult;
	private string mLastCommandTag;
	private int mLastCommandIndex;
	private bool mUseTrimWindow;
	private bool mUseYuragi;
	private bool mPreviewMode = false;

	private GameObject mRootObject;

	#endregion
}
