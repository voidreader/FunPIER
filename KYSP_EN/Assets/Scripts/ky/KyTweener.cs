using UnityEngine;
using System.Collections;

public class KyTweener : MonoBehaviour {
	
	public enum LoopProperty {
		None = 0x00,
		Loop = 0x01,
		Mirror = 0x02,
		MirrorLoop = Loop | Mirror,
	}
	
	public enum UseFlags {
		UsePosition = 1 << 0,
		UsePositionAdd = 1 << 1,
		UseScale = 1 << 2,
		UseRotation = 1 << 3,
		UseColor = 1 << 4,
		UsePers = 1 << 5,
		UseStart = 1 << 6,
	}

	#region MonoBehaviour Methods

	void Start() {
		//mIsPlaying = true;
		if ((UseFlag & UseFlags.UseStart) == 0) {
			StartPosition = transform.localPosition;
			StartScale = transform.localScale;
			StartRotation = transform.localRotation.eulerAngles;
			StartColor = SpriteUtil.GetVerticesColor(gameObject);
		}
		UpdateGeometry();
	}

	void Update() {
		if (!mIsPlaying) { return; }
		float deltaTime = 0;
		if (UseScriptTime) {
			deltaTime = KyScriptTime.DeltaTime;
		} else {
			deltaTime = Time.deltaTime;
		}
		if (deltaTime == 0) {
			return;
		}

		if ((LoopProp & LoopProperty.Loop) == 0) {
			if (mElapsedTime >= Duration) {
				mIsPlaying = false;
				if (Successor != null) {
					Successor.enabled = true;
				}
				if (AutoDestroy) {
					Destroy(this);
				}
			}
		}

		UpdateGeometry();

		mElapsedTime += deltaTime;
	}

	public void UpdateGeometry() {
		float rate = GetTweenRate();
		float persZ = 1.0f;
		if ((UseFlag & UseFlags.UsePosition) != 0) {
			Vector3 vec3 = Vector3.Lerp(StartPosition, EndPosition, rate);
			if (IgnoreZ) {
				vec3.z = transform.localPosition.z;
			}
			transform.localPosition = vec3;
		}
		if ((UseFlag & UseFlags.UseScale) != 0) {
			transform.localScale = Vector3.Lerp(StartScale, EndScale, rate);
		}
		if ((UseFlag & UseFlags.UseRotation) != 0) {
			transform.localRotation = Quaternion.Euler(Vector3.Lerp(StartRotation, EndRotation, rate));
		}
		if ((UseFlag & UseFlags.UseColor) != 0) {
			Color color = Color.Lerp(StartColor, EndColor, rate);
			SpriteUtil.SetVerticesColor(gameObject, color);
		}
		if ((UseFlag & UseFlags.UsePers) != 0) {
			persZ = Mathf.Lerp(StartPersZ, EndPersZ, rate);
			Vector3 pos = transform.localPosition;
			pos.x = (pos.x - VPoint.x) / persZ + VPoint.x;
			pos.y = (pos.y - VPoint.y) / persZ + VPoint.y;
			transform.localPosition = pos;
			Vector3 scale = transform.localScale;
			scale.x = scale.x / persZ;
			scale.y = scale.y / persZ;
			transform.localScale = scale;
		}
	}

	protected float GetTweenRate() {
		float time = mElapsedTime;
		if (time < 0) { time = 0; }
		if ((LoopProp & LoopProperty.Loop) != 0) {
			time = time % Duration;
		}
		if ((LoopProp & LoopProperty.Mirror) != 0) {
			if (time > Duration / 2) {
				time = Duration - time;
			}
			time *= 2;
		}
		float rate = Mathf.Clamp01(time / Duration);
		if (ScaleFactor < 1.0f) {
			rate = (1 - Mathf.Pow(ScaleFactor, rate)) / (1 - ScaleFactor);
		}
		return rate;
	}

	public void Play(bool playing) {
		mIsPlaying = playing;
	}

	public void StartTween(float fromTime) {
		Start();
		mIsPlaying = true;
		mElapsedTime = fromTime;
	}

	public void TweenPosition(Vector3 start, Vector3 end) {
		UseFlag |= UseFlags.UsePosition;
		StartPosition = start;
		EndPosition = end;
	}

	public void TweenColor(Color start, Color end) {
		UseFlag |= UseFlags.UseColor;
		StartColor = start;
		EndColor = end;
	}

	#endregion

	#region Properties

	public bool IsPlaying {
		get { return mIsPlaying; }
	}

	public float ElapsedTime {
		get { return mElapsedTime; }
		set { mElapsedTime = value; }
	}

	#endregion

	#region Fields

	public float Duration = 1.0f;
	public LoopProperty LoopProp = LoopProperty.None;
	public bool AutoDestroy = true;
	public bool IgnoreZ = false;
	public float ScaleFactor = 1.0f;
	public bool UseScriptTime = false;

	public UseFlags UseFlag;
	public Vector3 StartPosition = Vector3.zero;
	public Vector3 EndPosition = Vector3.zero;
	public Vector3 StartScale = Vector3.one;
	public Vector3 EndScale = Vector3.one;
	public Vector3 StartRotation = Vector3.zero;
	public Vector3 EndRotation = Vector3.zero;
	public Color StartColor;
	public Color EndColor;
	public Vector3 VPoint = Vector3.zero;
	public float StartPersZ = 1.0f;
	public float EndPersZ = 1.0f;
	public KyTweener Successor = null;

	protected bool mIsPlaying = true;
	protected float mElapsedTime = 0.0f;

	#endregion
}
