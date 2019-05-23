using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public enum KyResult {
	Success,
	GeneralError,
	ArgumentError,
	ObjectNotFoundError,
	AlreadyDefinedError,
}

public class KyCommand {
	public virtual KyResult Execute(KyCommandManager manager) {
		return KyResult.Success;
	}
}

public class KyCommandUse : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		GameObject instance = manager.CreateKyObject(Id, ResName);
		if (instance == null) {
			return KyResult.ObjectNotFoundError;
		}
		if (!string.IsNullOrEmpty(Parent)) {
			GameObject parent = manager.Find(Parent);
			if (parent == null) {
				return KyResult.ObjectNotFoundError;
			}
			instance.transform.parent = parent.transform;
		}
		return KyResult.Success;
	}

	public string Id;
	public string ResName;
	public string Parent;
}

public class KyCommandUseEmpty : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		GameObject instance = manager.CreateKyObject(Id);
		if (instance == null) {
			return KyResult.ObjectNotFoundError;
		}
		if (!string.IsNullOrEmpty(Parent)) {
			GameObject parent = manager.Find(Parent);
			if (parent == null) {
				return KyResult.ObjectNotFoundError;
			}
			instance.transform.parent = parent.transform;
		}
		return KyResult.Success;
	}

	public string Id;
	public string Parent;
}

public class KyCommandUnuse : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		if (Id == "all") {
			GameObject[] instances = GameObject.FindGameObjectsWithTag("KyObject");
			foreach (GameObject instance in instances) {
				if (!instance.name.StartsWith("_")) {
					GameObject.Destroy(instance);
				}
			}
		} else {
			GameObject instance = manager.Find(Id);
			if (instance != null) {
				GameObject.Destroy(instance);
			}
		}
		return KyResult.Success;
	}

	public string Id;
}

public class KyCommandShow : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		GameObject instance = manager.Find(Id);
		if (instance == null) {
			return KyResult.ObjectNotFoundError;
		}
		KyUtil.SetVisibleWithChildren(instance, true);

		if (UsePosition) {
			instance.transform.position = Position;
		}
		return KyResult.Success;
	}

	public string Id;
	public bool UsePosition;
	public Vector3 Position;
}

public class KyCommandHide : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		GameObject instance = manager.Find(Id);
		if (instance != null) {
			KyUtil.SetVisibleWithChildren(instance, false);
		}
		return KyResult.Success;
	}

	public string Id;
}

/// <summary>
/// 矩形を描画するコマンド。
/// </summary>
public class KyCommandDrawRect : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		GameObject instance = manager.Find(Id);
		if (instance == null) {
			instance = manager.CreateKyObject(Id);
			instance.AddComponent<SpriteSimple>();
		}
		SpriteSimple sprite = instance.GetComponent<SpriteSimple>();
		if (sprite != null) {
			sprite.Size = Size;
			sprite.MainColor = Color;
			sprite.AnchorX = AnchorX;
			sprite.AnchorY = AnchorY;
			sprite.PixelFitting = false;
			sprite.UpdateAll();
		}
		instance.transform.localPosition = Position;
		return KyResult.Success;
	}

	public string Id;		///	KyオブジェクトID
	public Vector3 Position;	///	表示位置
	public Vector2 Size;	///	矩形サイズ
	public Color Color;		///	矩形色
	public SpriteAnchor AnchorX;	///	X軸アンカー
	public SpriteAnchor AnchorY;	///	Y軸アンカー
}

/// <summary>
/// テキストを描画するコマンド。
/// </summary>
public class KyCommandDrawText : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		GameObject obj = manager.CreateKyObject(Id);
		SpriteTextCustom sprite = obj.AddComponent<SpriteTextCustom>();
		if (!string.IsNullOrEmpty(Parent)) {
			GameObject parent = manager.Find(Parent);
			if (parent == null) {
				return KyResult.ObjectNotFoundError;
			}
			obj.transform.parent = parent.transform;
		}
		if (Font == 0) {
			sprite.FontPrefab = (GameObject)KySharedRes.Instance.GetResource("fontKurita");
		} else {
			sprite.FontPrefab = (GameObject)KySharedRes.Instance.GetResource("fontGrecoStd-DB");
		}
		sprite.Vertical = Vertical;
		sprite.CharacterSize = new Vector2(Size, Size);
		sprite.FontColor = Color;
		sprite.AnchorX = AnchorX;
		sprite.AnchorY = AnchorY;
		if (string.IsNullOrEmpty(Text)) {
			sprite.Text = KyText.GetText(TextId);
		} else {
			sprite.Text = Text;
		}
		sprite.UpdateAll();
		obj.transform.localPosition = Position;
		return KyResult.Success;
	}

	public string Id;	///	KyオブジェクトID
	public string Parent;	///	親オブジェクトID
	public string Text;	/// 表示テキスト
	public int TextId;	///	テキストID
	public Vector3 Position;	///	表示位置
	public int Font;	///	フォントフェイスタイプ
	public int Size;	///	フォントサイズ
	public Color Color;	///	フォント色
	public bool Vertical;	///	垂直書きか
	public SpriteAnchor AnchorX;
	public SpriteAnchor AnchorY;
}

/// <summary>
/// オブジェクトの位置を設定するコマンド(非推奨)。
/// </summary>
public class KyCommandSetPos : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		GameObject instance = manager.Find(Id);
		if (instance != null) {
			instance.transform.position = Position;
		}
		return KyResult.Success;
	}

	public string Id;
	public Vector3 Position;
}

public class KyCommandBeginMove : KyCommand {
	public enum AttrFlag {
		UseFromPos = 1 << 0,
	}

	public override KyResult Execute(KyCommandManager manager) {
		GameObject instance = manager.Find(Id);
		if (instance == null) {
			return KyResult.ObjectNotFoundError;
		}
		if ((Attr & KyCommandBeginMove.AttrFlag.UseFromPos) == 0) {
			FromPos = instance.transform.position;
		}
		KyTweener cmp = instance.AddComponent<KyTweener>();
		cmp.IgnoreZ = true;
		cmp.AutoDestroy = true;
		cmp.StartPosition = FromPos;
		cmp.EndPosition = ToPos;
		cmp.Duration = (float)Duration / 60;
		cmp.UseScriptTime = true;
		return KyResult.Success;
	}

	public string Id;
	public AttrFlag Attr;
	public Vector3 FromPos;
	public Vector3 ToPos;
	public int Duration;
}

public class KyCommandBeginMoveEx : KyCommand {

	public enum UseFlags {
		UsePosition = 1 << 0,
		UsePositionAdd = 1 << 1,
		UseScale = 1 << 2,
		UseRotation = 1 << 3,
		UseColor = 1 << 4,
	}

	public override KyResult Execute(KyCommandManager manager) {
		GameObject instance = manager.Find(Id);
		if (instance == null) {
			return KyResult.ObjectNotFoundError;
		}

		if (Duration > 0) {
			KyTweener cmp = instance.AddComponent<KyTweener>();
			cmp.IgnoreZ = true;
			cmp.AutoDestroy = true;
			cmp.UseFlag = (KyTweener.UseFlags)Attr;
			cmp.StartPosition = instance.transform.localPosition;
			if ((Attr & UseFlags.UsePositionAdd) != 0) {
				cmp.UseFlag &= ~(KyTweener.UseFlags)UseFlags.UsePositionAdd;
				cmp.UseFlag |= (KyTweener.UseFlags)UseFlags.UsePosition;
				cmp.EndPosition = instance.transform.localPosition + Position;
			} else {
				cmp.EndPosition = Position;
			}
			cmp.StartScale = instance.transform.localScale;
			cmp.EndScale = Scale;
			cmp.StartRotation = instance.transform.localRotation.eulerAngles;
			cmp.EndRotation = Rotation;
			cmp.EndColor = Color;
			cmp.Duration = (float)Duration / 60;
			cmp.UseScriptTime = true;
		} else {
			if ((Attr & UseFlags.UsePosition) != 0) {
				Vector3 pos = instance.transform.localPosition;
				pos.x = Position.x;
				pos.y = Position.y;
				pos.z = Position.z;
				instance.transform.localPosition = pos;
			} else if ((Attr & UseFlags.UsePositionAdd) != 0) {
				Vector3 pos = instance.transform.localPosition;
				pos.x += Position.x;
				pos.y += Position.y;
				pos.z += Position.z;
				instance.transform.localPosition = pos;
			}
			if ((Attr & UseFlags.UseScale) != 0) {
				instance.transform.localScale = Scale;
			}
			if ((Attr & UseFlags.UseRotation) != 0) {
				instance.transform.localRotation = Quaternion.Euler(Rotation);
			}
			if ((Attr & UseFlags.UseColor) != 0) {
				SpriteUtil.SetVerticesColor(instance, Color);
			}
		}
		return KyResult.Success;
	}

	public string Id;
	public UseFlags Attr;
	public Vector3 Position;
	public Vector3 Scale;
	public Vector3 Rotation;
	public Color Color;
	public int Duration;

}

public class KyCommandEndMove : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		GameObject instance = manager.Find(Id);
		if (instance != null) {
			KyTweener cmp = instance.GetComponent<KyTweener>();
			if (cmp != null) {
				GameObject.Destroy(cmp);
			}
		}
		return KyResult.Success;
	}

	public string Id;
}

public class KyCommandBeginAnim : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		GameObject instance = manager.Find(Id);
		if (instance == null) {
			return KyResult.ObjectNotFoundError;
		}
		int anim = (int)manager.GetEvalValue(AnimIndex);
		int frame = (int)manager.GetEvalValue(FrameIndex);
		{
			KySpriteAnimation sprite = instance.GetComponent<KySpriteAnimation>();
			if (sprite != null) {
				sprite.AnimationIndex = anim;
				return KyResult.Success;
			}
		}
		{
			Sprite sprite = instance.GetComponent<Sprite>();
			if (sprite != null) {
				if (anim >= 0) {
					sprite.AnimationIndex = anim;
					sprite.UpdateAll();
				} else if (frame >= 0) {
					sprite.FrameIndex = frame;
					sprite.StopAnimation();
				}
				return KyResult.Success;
			}
		}
		return KyResult.Success;
	}

	public string Id;
	//public int AnimIndex;
	//public int FrameIndex;
	public KyVariable AnimIndex;
	public KyVariable FrameIndex;
}

public class KyCommandBeginTimer : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		GameObject instance = manager.Find(Id);
		if (instance != null) {
			//return KyResult.AlreadyDefinedError;
		} else {
			instance = new GameObject(Id);
			instance.tag = "KyObject";
		}

		instance.transform.parent = manager.RootObject.transform;
		KyTweener cmp = instance.AddComponent<KyTweener>();
		cmp.Duration = (float)Duration / 60;
		cmp.AutoDestroy = true;
		cmp.UseScriptTime = true;
		return KyResult.Success;
	}

	public string Id;
	public int Duration;
}

public class KyCommandFunction : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		GameObject instance = manager.Find(Id);
		if (instance == null) {
			return KyResult.ObjectNotFoundError;
		}
		instance.SendMessage(FuncName);
		return KyResult.Success;
	}

	public string Id;
	public string FuncName;
}

public class KyCommandFrame : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		if (Count < manager.FramePosition) {
			return KyResult.ArgumentError;
		}
		manager.NextFramePosition = Count;
		return KyResult.Success;
	}

	public int Count;
}

public class KyCommandBeginDrag : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		if (manager.PreviewMode) { return KyResult.Success; }

		GameObject instance = manager.Find(Id);
		if (instance == null) {
			return KyResult.ObjectNotFoundError;
		}
		KyDragControl cmp = instance.AddComponent<KyDragControl>();
		if (cmp == null) {
			return KyResult.GeneralError;
		}
		cmp.UseRange = UseRange;
		cmp.Range = Range;
		cmp.Rewind = Rewind;
		cmp.ScaleFactor = Factor;
		cmp.OnlyGrab = Grab;
		cmp.GrabRange = GrabRange;
		return KyResult.Success;
	}

	public string Id;
	public bool UseRange;
	public Rect Range;
	public bool Rewind;
	public float Factor;
	public bool Grab;
	public Rect GrabRange;
}

public class KyCommandEndDrag : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		GameObject instance = manager.Find(Id);
		if (instance == null) {
			return KyResult.ObjectNotFoundError;
		}
		KyDragControl cmp = instance.GetComponent<KyDragControl>();
		if (cmp != null) {
			GameObject.Destroy(cmp);
		}
		return KyResult.Success;
	}
	public string Id;
}

public class KyCommandBeginIf : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		if (Target == TargetType.Touch) {
			if (manager.PreviewMode) { return KyResult.Success; }
		} else if (Target == TargetType.TouchAction) {
			if (manager.PreviewMode) { return KyResult.Success; }
		}
		if (Target == TargetType.Always) {
			manager.MoveFrame(Frame, Priority);
			return KyResult.Success;
		}

		GameObject instance = manager.Find(Id);
		if (instance != null) {
			KyTrigger trig = instance.GetComponent<KyTrigger>();
			if (trig == null) {
				return KyResult.AlreadyDefinedError;
			} else {
				Object.Destroy(instance);
			}
		}
		instance = new GameObject(Id);
		if (Target == TargetType.Object) {
			KyTriggerObjectPosition trig = instance.AddComponent<KyTriggerObjectPosition>();
			trig.TargetName = TargetName;
			trig.Range = Range;
			trig.AutoDestroy = AutoEnd;
			trig.Trigger += this.OnTrigger;
			trig.WhenReleased = (Trigger == TriggerType.Up);
			trig.Priority = Priority;
		} else if (Target == TargetType.Touch) {
			KyTriggerTouchPosition trig = instance.AddComponent<KyTriggerTouchPosition>();
			trig.Range = Range;
			trig.AutoDestroy = AutoEnd;
			trig.Trigger += this.OnTrigger;
			trig.Priority = Priority;
		} else if (Target == TargetType.TouchAction) {
			KyTriggerTouchAction trig = instance.AddComponent<KyTriggerTouchAction>();
			trig.Action = KyTriggerTouchAction.ActionType.Slide;
			trig.Direction = Direction;
			trig.AutoDestroy = AutoEnd;
			trig.Trigger += this.OnTrigger;
			trig.Priority = Priority;
			trig.IntervalTime = (float)Interval / 60.0f;
		} else if (Target == TargetType.AnimEnd) {
			KyTriggerAnimEnd trig = instance.AddComponent<KyTriggerAnimEnd>();
			trig.TargetName = TargetName;
			trig.AutoDestroy = AutoEnd;
			trig.Trigger += this.OnTrigger;
			trig.Priority = Priority;
		}
		instance.tag = "KyCondition";
		instance.transform.parent = manager.RootObject.transform;
		mManager = manager;
		return KyResult.Success;
	}

	private void OnTrigger(object sender, System.EventArgs args) {
		//DebugUtil.Log("OnTrigger");
		mManager.MoveFrame(Frame, Priority);
	}

	public enum TargetType {
		Object,
		Touch,
		TouchAction,
		AnimEnd,
		Always,
	}

	public enum TriggerType {
		None,
		Down,
		Up
	}

	public string Id;
	public TargetType Target;
	public TriggerType Trigger;
	public string TargetName;
	public int Frame;
	public Rect Range;
	public Vector3 Direction;
	public bool AutoEnd;
	public int Priority;
	public int Interval;

	private KyCommandManager mManager;
}

public class KyCommandBeginIfVar : KyCommand {

	public enum OperatorType {
		Equal = 0,
		NotEqual,
		Greater,
		GreaterEqual,
	}

	public override KyResult Execute(KyCommandManager manager) {
		KyTriggerVar.Condition cond = null;
		switch (Operator) {
		case OperatorType.Equal:
			cond = delegate(float lhs, float rhs) { return lhs == rhs; };
			break;
		case OperatorType.NotEqual:
			cond = delegate(float lhs, float rhs) { return lhs != rhs; };
			break;
		case OperatorType.Greater:
			cond = delegate(float lhs, float rhs) { return lhs > rhs; };
			break;
		case OperatorType.GreaterEqual:
			cond = delegate(float lhs, float rhs) { return lhs >= rhs; };
			break;
		}

		if (string.IsNullOrEmpty(Id) && cond != null) {
			if (cond(manager.GetEvalValue(Lhs), manager.GetEvalValue(Rhs))) {
				manager.MoveFrame(Frame, Priority);
			}
			return KyResult.Success;
		}

		GameObject instance = manager.Find(Id);
		if (instance != null) {
			return KyResult.AlreadyDefinedError;
		}
		instance = new GameObject(Id);

		KyTriggerVar trig = instance.AddComponent<KyTriggerVar>();
		trig.Lhs = Lhs;
		trig.Rhs = Rhs;
		trig.CommandManager = manager;
		trig.Priority = Priority;
		trig.Cond = cond;
		trig.Trigger += OnTrigger;
		instance.tag = "KyCondition";
		instance.transform.parent = manager.RootObject.transform;
		mManager = manager;
		return KyResult.Success;
	}

	private void OnTrigger(object sender, System.EventArgs args) {
		mManager.MoveFrame(Frame, Priority);
	}

	public string Id;
	public KyVariable Lhs;
	public KyVariable Rhs;
	public OperatorType Operator;
	public int Frame;
	public int Priority;
	private KyCommandManager mManager;
}

public class KyCommandEndIf : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		if (string.IsNullOrEmpty(Id) || Id == "all") {
			GameObject[] instances = GameObject.FindGameObjectsWithTag("KyCondition");
			foreach (GameObject instance in instances) {
				if (Priority >= 0) {
					KyTrigger trig = instance.GetComponent<KyTrigger>();
					if (trig != null && trig.Priority == Priority) {
						GameObject.Destroy(instance);
					}
				} else {
					GameObject.Destroy(instance);
				}
			}
		} else {
			GameObject instance = manager.Find(Id);
			if (instance != null) {
				GameObject.Destroy(instance);
			}
		}
		return KyResult.Success;
	}

	public string Id;
	public int Priority;
}

public class KyCommandMoveFrame : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		manager.MoveFrame(Frame, 0);
		return KyResult.Success;
	}

	public int Frame;
}

public class KyCommandBeginSound : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		if (Looping) {
			if (0 <= Track && Track <= 1) {
				KyAudioElement elem = KyAudioManager.Instance.AudioElements[Track];
				elem.Play(SoundName, true, Volume, Pitch, FadeTime);
			}
		} else {
			KyAudioManager.Instance.PlayOneShot(SoundName, Volume, Pitch);
		}
		return KyResult.Success;
	}

	public int Track;
	public string SoundName;
	public bool Looping;
	public float Volume;
	public float Pitch;
	public float FadeTime;
}

public class KyCommandEndSound : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		if (Track < 0) {
			foreach (KyAudioElement elem in KyAudioManager.Instance.AudioElements) {
				elem.Stop(FadeTime);
			}
		} else {
			if (0 <= Track && Track <= 1) {
				KyAudioElement elem = KyAudioManager.Instance.AudioElements[Track];
				elem.Stop(FadeTime);
			}
		}
		//KyAudioManager.Instance.Stop(FadeTime);
		return KyResult.Success;
	}

	public int Track;
	public float FadeTime;
}

public class KyCommandFadeScreen : KyCommand
{
	public enum FadeKind {
		FadeIn,
		FadeOut,
		FadeEnd,
	}

	public override KyResult Execute(KyCommandManager manager) {
		GameObject instance = manager.Find("_fader");
		if (instance == null) {
			instance = manager.CreateKyObject("_fader", "system/systemFader");
		}
		KyTweener tweener = instance.GetComponent<KyTweener>();
		tweener.UseFlag = KyTweener.UseFlags.UseStart | KyTweener.UseFlags.UseColor;

		if (Kind == FadeKind.FadeIn) {
			tweener.StartColor = new Color(1,1,1,1);
			tweener.EndColor = new Color(1,1,1,0);
			tweener.Duration = Duration/60.0f;
			tweener.StartTween(0);
		} else if (Kind == FadeKind.FadeOut) {
			tweener.StartColor = new Color(1,1,1,0);
			tweener.EndColor = new Color(1,1,1,1);
			tweener.Duration = Duration/60.0f;
			tweener.StartTween(0);
		}
		return KyResult.Success;
	}

	public FadeKind Kind;
	public int Duration;
}

/// <summary>
/// 変数を設定するためのコマンド。
/// </summary>
public class KyCommandVar : KyCommand {

	public enum OperatorType {
		Set = 0,
		Inc,
		Dec,
	}

	public override KyResult Execute(KyCommandManager manager) {
		switch (Operator) {
		case OperatorType.Set: {
			manager.SetVariable(Name, Value);
			DebugUtil.Log("" + Name + "=" + Value);
		} break;
		case OperatorType.Inc: {
			float variable = manager.GetVariable(Name);
			manager.SetVariable(Name, variable + Value);
			DebugUtil.Log("" + Name + "=" + (variable + Value));
		} break;
		case OperatorType.Dec: {
			float variable = manager.GetVariable(Name);
			manager.SetVariable(Name, variable - Value);
			DebugUtil.Log("" + Name + "=" + (variable - Value));
		} break;
		}
		return KyResult.Success;
	}

	public string Name;
	public OperatorType Operator;
	public float Value;
}

public class KyCommandWait : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		manager.Waiting = true;
		return KyResult.Success;
	}

	public int Count;
}

public class KyCommandHalt : KyCommand {

	public override KyResult Execute(KyCommandManager manager) {
		GameObject[] instances;
		instances = GameObject.FindGameObjectsWithTag("KyCondition");
		foreach (GameObject instance in instances) {
			GameObject.Destroy(instance);
		}

		instances = GameObject.FindGameObjectsWithTag("KyObject");
		foreach (GameObject instance in instances) {
			KyDragControl cmp = instance.GetComponent<KyDragControl>();
			if (cmp != null) {
				GameObject.Destroy(cmp);
			}
		}
		if (!ContinueSound) {
			KyAudioManager.Instance.Stop(1.0f);
		}
		manager.HaltCommand();
		if (Result >= 0) {
			manager.SetVariable("result", Result);
		}
		return KyResult.Success;
	}

	public int Result;
	public bool ContinueSound;
}
