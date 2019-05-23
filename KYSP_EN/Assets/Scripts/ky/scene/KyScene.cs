using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KyScene : MonoBehaviour {


	#region MonoBehaviour Methods

	public virtual void Update() {
		UpdateState();
		if (mState != null) {
			mState();
		}
		if (mWaitTime > 0) {
			mWaitTime -= Time.deltaTime;
			if (mWaitTime < 0) { 
				mWaitTime = 0; 
			}
		}
	}

	#endregion

	#region Methods

	/// <summary>
	/// 이 씬을 종료하고 새로운 씬을 시작한다.
	/// </summary>
	/// <param name="sceneName">새로운 장면 이름 </param>
	public KyScene ChangeScene(string sceneName) {
		GameObject scenePrefab = LoadScenePrefab(sceneName);
		return ChangeScene(scenePrefab);
	}

	public KyScene ChangeScene(GameObject scenePrefab) {
		if (scenePrefab == null) { return null; }
		GameObject instance = (GameObject)Instantiate(scenePrefab);
		KyScene scene = instance.GetComponent<KyScene>();
		if (scene != null) {
			scene.ParentScene = ParentScene;
			instance.transform.parent = transform.parent;
			OnLeaveScene();
			Destroy(gameObject);
			return scene;
		} else {
			return null;
		}
	}

    /// <summary>
    /// 이 장면을 스택에 푸시하고 새로운 장면을 시작합니다.
    /// 푸시 된 장면 개체는 저장되지만, Update가 호출되지 않습니다.
    /// </summary>
    /// <param name="sceneName"></param>
    public KyScene PushScene(string sceneName) {
		return PushScene(sceneName, false);
	}

	public KyScene PushScene(string sceneName, bool deactivate) {
		GameObject scenePrefab = LoadScenePrefab(sceneName);
		if (scenePrefab != null) {
			return PushScene(scenePrefab, deactivate);
		} else {
			return null;
		}
	}

	public KyScene PushScene(GameObject scenePrefab, bool deactivate) {
		if (scenePrefab == null) { return null; }
		GameObject instance = (GameObject)Instantiate(scenePrefab);
		KyScene scene = instance.GetComponent<KyScene>();
		if (scene != null) {
			scene.ParentScene = gameObject;
			instance.transform.parent = transform;
			//if (deactivate) {
			//	gameObject.active = false;
			//} else {
				enabled = false;
			//}
		}
		return scene;
	}

    /// <summary>
    /// 이 장면을 종료하고 스택에서 장면을 팝합니다.
    /// 팝 된 장면은 Update가 재개됩니다.
    /// </summary>
    /// <param name="sceneName"></param>
    public void PopScene(bool destroy) {
		enabled = false;
		if (mParentScene != null) {
			KyScene scene = mParentScene.GetComponent<KyScene>();
			if (scene != null) {
				//scene.gameObject.active = true;
				scene.enabled = true;
				OnLeaveScene();
				if (destroy) {
					Destroy(gameObject);
				}
			}
		}
	}

	public void PopScene() {
		PopScene(true);
	}

	public T GetChildScene<T>() where T : KyScene {
		return GetComponentInChildren<T>();
	}

	protected GameObject LoadScene(string sceneName) {
		GameObject prefab = (GameObject)Resources.Load("GameScenes/" + sceneName, typeof(GameObject));
		if (prefab != null) { return null; }
		GameObject instance = (GameObject)Instantiate(prefab);
		if (instance != null) { return null; }
		return instance;
	}

	protected GameObject LoadScenePrefab(string sceneName) {
		return (GameObject)Resources.Load("GameScenes/" + sceneName, typeof(GameObject));
	}

	protected void BeginWait(float duration) {
		mWaitTime = duration;
	}

	protected void UpdateState() {
		if (mNextState != mState) {
			mState = mNextState;
			mSequence = mInitialSequence;
			mInitialSequence = 0;
		}
	}

	protected virtual void OnLeaveScene() {
	}

	#endregion

	#region Properties

	public GameObject ParentScene {
		get { return mParentScene; }
		set { mParentScene = value; }
	}

	public bool Waiting {
		get { return mWaitTime > 0; }
	}

	#endregion

	#region Fields

	protected delegate void State();
	protected GameObject mParentScene = null;
	protected State mState = null;
	protected State mNextState = null;
	protected int mSequence;
	protected int mInitialSequence;
	protected float mWaitTime;

	#endregion
}
