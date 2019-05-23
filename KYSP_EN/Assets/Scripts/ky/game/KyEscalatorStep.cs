using UnityEngine;
using System.Collections;

public class KyEscalatorStep : MonoBehaviour {

	void Awake () {
		transform.localPosition = new Vector3(200, -280, transform.localPosition.z);
		transform.localScale = new Vector3(1.0f, 1.0f, 1);

		mTweener = gameObject.AddComponent<KyTweener>();
		mTweener.UseFlag = KyTweener.UseFlags.UsePosition | KyTweener.UseFlags.UseScale | KyTweener.UseFlags.UsePers;
		mTweener.EndPosition = new Vector3(200, -320, 0);
		mTweener.EndScale = new Vector3(1.0f, 1.0f, 1);
		mTweener.Duration = Duration;
		mTweener.IgnoreZ = true;
		mTweener.AutoDestroy = true;
		mTweener.ElapsedTime = StartTime;
		mTweener.StartPersZ = 4.0f;
		mTweener.EndPersZ = 1.0f;
		mTweener.UseScriptTime = true;
		mTweener.VPoint = new Vector3(0, 460, 0);
	}
	
	void Update () {
		if (mTweener == null) {
			Destroy(gameObject);
		}
	}

	private KyTweener mTweener;

	public float Duration = 10.0f;
	public float StartTime = 0.0f;
}
