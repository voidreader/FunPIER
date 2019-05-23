using UnityEngine;
using System.Collections;

public class KyEscalatorStepMgr : MonoBehaviour {

	void Awake() {
		//mMeshFilter = gameObject.AddComponent<MeshFilter>();
		//mMeshFilter.mesh = new Mesh();
	}

	void Start () {
		for (int i = 25; i >= 0; --i) {
			GameObject instance = CreateStep();
			KyTweener tweener = instance.GetComponent<KyTweener>();
			tweener.ElapsedTime = i * StepTime;
		}
	}

	void Update() {
		if (KyScriptTime.DeltaTime == 0) { return; }
		mElapsedTime += KyScriptTime.DeltaTime;
		if (mElapsedTime >= StepTime) {
			mElapsedTime -= StepTime;
			CreateStep();
		}
	}

	private GameObject CreateStep() {
		GameObject instance = (GameObject)GameObject.Instantiate(StepPrefab);
		instance.transform.parent = transform;
		Vector3 pos = instance.transform.localPosition;
		pos.z = mStepCount * 0.01f;
		instance.transform.localPosition = pos;

		Transform mob = instance.transform.Find("mesenMob");
		if (mob != null) {
			bool setActive = false;
			if (UseMob) {
				if (mStepCount % 3 == 0) {
					setActive = true;
				} else {
					setActive = Random.Range(0.0f, 1.0f) < 0.2f;
				}
			}
			mob.gameObject.active = setActive;
		}

		/*CombineInstance combine = new CombineInstance();
		MeshFilter meshFilter = instance.GetComponent<MeshFilter>();
		if (meshFilter != null) {
			combine.mesh = meshFilter.sharedMesh;
			combine.transform = instance.transform.localToWorldMatrix;
			meshFilter.gameObject;
			mMeshFilter.mesh.CombineMeshes(
		}*/

		mStepCount++;
		return instance;
	}

	public void OnUseMob() {
		UseMob = true;
	}

	public GameObject StepPrefab = null;
	public float StepTime = 1.0f;
	public bool UseMob = false;

	private float mElapsedTime = 0.0f;
	private float mStepCount = 0;
	//private MeshFilter mMeshFilter;
}
