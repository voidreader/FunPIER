using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;

public class KyScriptReader {

	#region Methods

	public KyScriptReader() {
		mXmlReader = null;
	}

	/// <summary>
	/// リソース名の指定してスクリプトを読み込みます。
	/// </summary>
	/// <param name="name">リソース名</param>
	public void ReadFromResName(string name) {
		TextAsset asset = (TextAsset)Resources.Load(name, typeof(TextAsset));
		if (asset == null) {
			Debug.LogError("script file not found.");
		}
		if (asset != null) {
			ReadFromText(asset.text);
		}
	}

	/// <summary>
	/// ロード済みのテキストデータからスクリプトを読み込みます。
	/// </summary>
	/// <param name="text">テキストデータ</param>
	public void ReadFromText(string text) {
		StringReader sreader = new StringReader(text);
		mXmlReader = new XmlTextReader(sreader);
		ReadCore();
	}

	protected void ReadCore() {
		mCommandMgr = new KyCommandManager();
		mCommandMgr.BeginCommand();
		while (mXmlReader.Read()) {
			if (mXmlReader.NodeType == XmlNodeType.Element) {
				switch (mXmlReader.Name) {
				case "kyscript": OnKyScript(); break;
				case "use": OnTagUse(); break;
				case "unuse": OnTagUnuse(); break;
				case "show": OnTagShow(); break;
				case "hide": OnTagHide(); break;
				case "useAndShow": OnTagUseAndShow(); break;
				case "useEmpty": OnTagUseEmpty(); break;
				case "drawRect": OnTagDrawRect(); break;
				case "drawText": OnTagDrawText(); break;
				case "setPos": OnTagSetPos(); break;
				case "beginMove": OnTagBeginMove(); break;
				case "beginMoveEx": OnTagBeginMoveEx(); break;
				case "endMove": OnTagEndMove(); break;
				case "beginAnim": OnTagBeginAnim(); break;
				case "beginTimer": OnTagBeginTimer(); break;
				case "function": OnTagFunction(); break;
				case "frame": OnTagFrame(); break;
				case "beginDrag": OnTagBeginDrag(); break;
				case "endDrag": OnTagEndDrag(); break;
				case "beginIf": OnTagBeginIf(); break;
				case "beginIfVar": OnTagBeginIfVar(); break;
				case "endIf": OnTagEndIf(); break;
				case "beginSound": OnTagBeginSound(); break;
				case "endSound": OnTagEndSound(); break;
				case "fadeScreen": OnTagFadeScreen(); break;
				case "var": OnTagVar(); break;
				case "wait": OnTagWait(); break;
				case "halt": OnTagHalt(); break;
				default:
					DebugUtil.Log("unknown tag : " + mXmlReader.Name);
					break;
				}
			}
		}
		mCommandMgr.EndCommand();
	}

	protected void OnKyScript() {
		bool notrim = GetAttrAsBoolean("notrim", false);
		mCommandMgr.UseTrimWindow = !notrim;
		mCommandMgr.UseYuragi = GetAttrAsBoolean("yuragi", true);
	}

	protected void OnTagUse() {
		KyCommandUse cmd = new KyCommandUse();
		cmd.Id = GetAttrAsString("id");
		cmd.ResName = GetAttrAsString("res");
		cmd.Parent = GetAttrAsString("parent");
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagUseEmpty() {
		KyCommandUseEmpty cmd = new KyCommandUseEmpty();
		cmd.Id = GetAttrAsString("id");
		cmd.Parent = GetAttrAsString("parent");
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagUnuse() {
		KyCommandUnuse cmd = new KyCommandUnuse();
		cmd.Id = GetAttrAsString("id");
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagShow() {
		KyCommandShow cmd = new KyCommandShow();
		cmd.Id = GetAttrAsString("id");
		cmd.UsePosition = HasAttr("pos");
		cmd.Position = GetAttrAsVector3("pos");
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagHide() {
		KyCommandHide cmd = new KyCommandHide();
		cmd.Id = GetAttrAsString("id");
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagUseAndShow() {
		OnTagUse();
		OnTagShow();
	}

	protected void OnTagDrawRect() {
		KyCommandDrawRect cmd = new KyCommandDrawRect();
		cmd.Id = GetAttrAsString("id");
		cmd.Position = GetAttrAsVector3("pos");
		cmd.Size = GetAttrAsVector2("size");
		cmd.Color = GetAttrAsColor("color");
		cmd.AnchorX = GetAttrAsAnchor("anchorx");
		cmd.AnchorY = GetAttrAsAnchor("anchory");
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagDrawText() {
		KyCommandDrawText cmd = new KyCommandDrawText();
		cmd.Id = GetAttrAsString("id");
		cmd.Parent = GetAttrAsString("parent");
		cmd.Position = GetAttrAsVector3("pos");
		cmd.Size = GetAttrAsInt("size", 12);
		if (HasAttr("color")) {
			cmd.Color = GetAttrAsColor("color");
		} else {
			cmd.Color = new Color(0, 0, 0, 1);
		}
		cmd.Text = GetAttrAsString("text");
		cmd.TextId = GetAttrAsInt("textId", 0);
		cmd.Font = GetAttrAsInt("font", 0);
		cmd.Vertical = GetAttrAsBoolean("vertical", false);
		cmd.AnchorX = GetAttrAsAnchor("anchorx");
		cmd.AnchorY = GetAttrAsAnchor("anchory");
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagSetPos() {
		KyCommandSetPos cmd = new KyCommandSetPos();
		cmd.Id = GetAttrAsString("id");
		cmd.Position = GetAttrAsVector3("pos");
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagBeginMove() {
		KyCommandBeginMoveEx cmd = new KyCommandBeginMoveEx();
		cmd.Id = GetAttrAsString("id");
		if (HasAttr("to")) {
			cmd.Attr |= KyCommandBeginMoveEx.UseFlags.UsePosition;
			cmd.Position = GetAttrAsVector3("to");
		} else if (HasAttr("pos")) {
			cmd.Attr |= KyCommandBeginMoveEx.UseFlags.UsePosition;
			cmd.Position = GetAttrAsVector3("pos");
		}
		cmd.Duration = GetAttrAsInt("duration", 0);
		/*cmd.ToPos = GetAttrAsVector3("to");
		if (HasAttr("from")) {
			cmd.FromPos = GetAttrAsVector3("from");
			cmd.Attr |= KyCommandBeginMove.AttrFlag.UseFromPos;
		}*/
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagBeginMoveEx() {
		KyCommandBeginMoveEx cmd = new KyCommandBeginMoveEx();
		cmd.Id = GetAttrAsString("id");
		if (HasAttr("pos")) {
			cmd.Attr |= KyCommandBeginMoveEx.UseFlags.UsePosition;
			cmd.Position = GetAttrAsVector3("pos");
		} else if (HasAttr("posAdd")) {
			cmd.Attr |= KyCommandBeginMoveEx.UseFlags.UsePositionAdd;
			cmd.Position = GetAttrAsVector3("posAdd");
		}
		if (HasAttr("rot")) {
			cmd.Attr |= KyCommandBeginMoveEx.UseFlags.UseRotation;
			cmd.Rotation = GetAttrAsVector3("rot");
		}
		if (HasAttr("scale")) {
			cmd.Attr |= KyCommandBeginMoveEx.UseFlags.UseScale;
			cmd.Scale = GetAttrAsVector3("scale");
		}
		if (HasAttr("color")) {
			cmd.Attr |= KyCommandBeginMoveEx.UseFlags.UseColor;
			cmd.Color = GetAttrAsColor("color");
		}
		cmd.Duration = GetAttrAsInt("duration", 0);
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagEndMove() {
		KyCommandEndMove cmd = new KyCommandEndMove();
		cmd.Id = GetAttrAsString("id");
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagBeginAnim() {
		KyCommandBeginAnim cmd = new KyCommandBeginAnim();
		cmd.Id = GetAttrAsString("id");
		cmd.AnimIndex = GetAttrAsVar("anim", -1);
		cmd.FrameIndex = GetAttrAsVar("frame", -1);
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagBeginTimer() {
		KyCommandBeginTimer cmd = new KyCommandBeginTimer();
		cmd.Id = GetAttrAsString("id");
		cmd.Duration = GetAttrAsInt("duration", 0);
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagFunction() {
		KyCommandFunction cmd = new KyCommandFunction();
		cmd.Id = GetAttrAsString("id");
		cmd.FuncName = GetAttrAsString("func");
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagFrame() {
		KyCommandFrame cmd = new KyCommandFrame();
		cmd.Count = GetAttrAsInt("count", 0);
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagBeginDrag() {
		KyCommandBeginDrag cmd = new KyCommandBeginDrag();
		cmd.Id = GetAttrAsString("id");
		if (HasAttr("range")) {
			cmd.UseRange = true;
			cmd.Range = GetAttrAsRect("range");
		}
		else {
			cmd.UseRange = false;
		}
		cmd.Rewind = GetAttrAsBoolean("rewind", false);
		cmd.Factor = GetAttrAsFloat("factor", 1.0f);
		cmd.Grab = GetAttrAsBoolean("grab", false);
		cmd.GrabRange = GetAttrAsRect("grabRange");
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagEndDrag() {
		KyCommandEndDrag cmd = new KyCommandEndDrag();
		cmd.Id = GetAttrAsString("id");
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagBeginIf() {
		KyCommandBeginIf cmd = new KyCommandBeginIf();
		cmd.Id = GetAttrAsString("id");
		cmd.Frame = GetAttrAsInt("goto", 0);
		switch (GetAttrAsString("target")) {
		case "object":
			cmd.Target = KyCommandBeginIf.TargetType.Object;
			break;
		case "touch":
			cmd.Target = KyCommandBeginIf.TargetType.Touch;
			break;
		case "touchaction":
			cmd.Target = KyCommandBeginIf.TargetType.TouchAction;
			break;
		case "animend":
			cmd.Target = KyCommandBeginIf.TargetType.AnimEnd;
			break;
		case "always":
			cmd.Target = KyCommandBeginIf.TargetType.Always;
			break;
		}
		switch (GetAttrAsString("trigger")) {
		case "down":
			cmd.Trigger = KyCommandBeginIf.TriggerType.Down;
			break;
		case "up":
			cmd.Trigger = KyCommandBeginIf.TriggerType.Up;
			break;
		}
		cmd.TargetName = GetAttrAsString("name");
		if (HasAttr("range")) {
			cmd.Range = GetAttrAsRect("range");
		} else if (HasAttr("crange")) {
			cmd.Range = GetAttrAsCenterRect("crange");
		}
		cmd.Direction = GetAttrAsVector3("direction");
		cmd.AutoEnd = GetAttrAsBoolean("autoEnd", true);
		cmd.Priority = GetAttrAsInt("priority", 0);
		cmd.Interval = GetAttrAsInt("interval", 0);
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagBeginIfVar() {
		KyCommandBeginIfVar cmd = new KyCommandBeginIfVar();
		cmd.Id = GetAttrAsString("id");
		cmd.Frame = GetAttrAsInt("goto", 0);
		cmd.Priority = GetAttrAsInt("priority", 0);
		cmd.Lhs = GetAttrAsVar("lhs", 0);
		cmd.Rhs = GetAttrAsVar("rhs", 0);
		switch (GetAttrAsString("op")) {
		case "eq":
			cmd.Operator = KyCommandBeginIfVar.OperatorType.Equal;
			break;
		case "neq":
			cmd.Operator = KyCommandBeginIfVar.OperatorType.NotEqual;
			break;
		case "gt":
			cmd.Operator = KyCommandBeginIfVar.OperatorType.Greater;
			break;
		case "gte":
			cmd.Operator = KyCommandBeginIfVar.OperatorType.GreaterEqual;
			break;
		}
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagEndIf() {
		KyCommandEndIf cmd = new KyCommandEndIf();
		cmd.Id = GetAttrAsString("id");
		cmd.Priority = GetAttrAsInt("priority", -1);
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagBeginSound() {
		KyCommandBeginSound cmd = new KyCommandBeginSound();
		cmd.Track = GetAttrAsInt("track", 0);
		cmd.SoundName = GetAttrAsString("name");
		cmd.Looping = GetAttrAsBoolean("loop", false);
		cmd.Volume = GetAttrAsFloat("volume", 1.0f);
		cmd.Pitch = GetAttrAsFloat("pitch", 1.0f);
		cmd.FadeTime = GetAttrAsFloat("fade", 0.0f);
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagEndSound() {
		KyCommandEndSound cmd = new KyCommandEndSound();
		cmd.Track = GetAttrAsInt("track", -1);
		cmd.FadeTime = GetAttrAsFloat("fade", 1.0f);
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagFadeScreen() {
		KyCommandFadeScreen cmd = new KyCommandFadeScreen();
		switch (GetAttrAsString("kind")) {
		case "fadein": 
			cmd.Kind = KyCommandFadeScreen.FadeKind.FadeIn; 
			break;
		case "fadeout": 
			cmd.Kind = KyCommandFadeScreen.FadeKind.FadeOut; 
			break;
		case "fadeend":
			cmd.Kind = KyCommandFadeScreen.FadeKind.FadeEnd;
			break;
		}
		cmd.Duration = GetAttrAsInt("duration", 0);
		if (cmd.Duration == 0) {
			cmd.Duration = 1;
		}
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagVar() {
		KyCommandVar cmd = new KyCommandVar();
		cmd.Name = GetAttrAsString("name");
		cmd.Value = GetAttrAsFloat("value", 0);
		switch (GetAttrAsString("op")) {
		case "set":
			cmd.Operator = KyCommandVar.OperatorType.Set;
			break;
		case "inc":
			cmd.Operator = KyCommandVar.OperatorType.Inc;
			break;
		case "dec":
			cmd.Operator = KyCommandVar.OperatorType.Dec;
			break;
		default:
			cmd.Operator = KyCommandVar.OperatorType.Set;
			break;
		}
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagWait() {
		KyCommandWait cmd = new KyCommandWait();
		cmd.Count = GetAttrAsInt("count", 0);
		mCommandMgr.AddCommand(cmd);
	}

	protected void OnTagHalt() {
		KyCommandHalt cmd = new KyCommandHalt();
		cmd.Result = GetAttrAsInt("result", -1);
		cmd.ContinueSound = GetAttrAsBoolean("continueSound", false);
		mCommandMgr.AddCommand(cmd);
	}

	protected bool HasAttr(string name) {
		return (mXmlReader.GetAttribute(name) != null);
	}

	protected string GetAttrAsString(string name) {
		return mXmlReader.GetAttribute(name);
	}

	protected int GetAttrAsInt(string name, int defaultValue) {
		int result;
		string value = mXmlReader.GetAttribute(name);
		if (string.IsNullOrEmpty(value)) {
			return defaultValue;
		}
		if (int.TryParse(value, out result)) {
			return result;
		} else {
			return defaultValue;
		}
	}

	protected float GetAttrAsFloat(string name, float defaultValue) {
		float result;
		string value = mXmlReader.GetAttribute(name);
		if (string.IsNullOrEmpty(value)) {
			return defaultValue;
		}
		if (float.TryParse(value, out result)) {
			return result;
		}
		else {
			return defaultValue;
		}
	}

	protected KyVariable GetAttrAsVar(string name, float defaultValue) {
		float result;
		string value = mXmlReader.GetAttribute(name);
		if (string.IsNullOrEmpty(value)) {
			return new KyVariable(defaultValue);
		}
		if (float.TryParse(value, out result)) {
			return new KyVariable(result);
		} else {
			if (value.Length > 0 && value[0] == '$') {
				return new KyVariable(value.Substring(1));
			} else {
				return new KyVariable(defaultValue);
			}
		}
	}

	protected bool GetAttrAsBoolean(string name, bool defaultValue) {
		bool result;
		string value = mXmlReader.GetAttribute(name);
		if (string.IsNullOrEmpty(value)) {
			return defaultValue;
		}
		if (bool.TryParse(value, out result)) {
			return result;
		}
		else {
			return defaultValue;
		}
	}

	protected SpriteAnchor GetAttrAsAnchor(string name) {
		SpriteAnchor result = SpriteAnchor.Middle;
		string value = mXmlReader.GetAttribute(name);
		if (string.IsNullOrEmpty(value)) {
			return result;
		}
		switch (value) {
		case "min": return SpriteAnchor.Minimum;
		case "max": return SpriteAnchor.Maximum;
		default: return SpriteAnchor.Middle;
		}
	}

	protected Vector2 GetAttrAsVector2(string name) {
		Vector2 result = new Vector2();
		string value = mXmlReader.GetAttribute(name);
		if (string.IsNullOrEmpty(value)) {
			return result;
		}
		string[] nums = value.Split(new char[] { ',' }, 3);
		if (nums.Length >= 1) { float.TryParse(nums[0], out result.x); }
		if (nums.Length >= 2) { float.TryParse(nums[1], out result.y); }
		return result;
	}

	protected Vector3 GetAttrAsVector3(string name) {
		Vector3 result = new Vector3();
		string value = mXmlReader.GetAttribute(name);
		if (string.IsNullOrEmpty(value)) {
			return result;
		}
		string[] nums = value.Split(new char[] { ',' }, 3);
		if (nums.Length >= 1) { float.TryParse(nums[0], out result.x); }
		if (nums.Length >= 2) { float.TryParse(nums[1], out result.y); }
		if (nums.Length >= 3) { float.TryParse(nums[2], out result.z); }
		return result;
	}

	protected Color GetAttrAsColor(string name) {
		Color result = new Color(1, 1, 1, 1);
		string value = mXmlReader.GetAttribute(name);
		if (string.IsNullOrEmpty(value)) {
			return result;
		}
		string[] nums = value.Split(new char[] { ',' }, 4);
		if (nums.Length >= 1) { float.TryParse(nums[0], out result.r); }
		if (nums.Length >= 2) { float.TryParse(nums[1], out result.g); }
		if (nums.Length >= 3) { float.TryParse(nums[2], out result.b); }
		if (nums.Length >= 4) { float.TryParse(nums[3], out result.a); }
		return result;
	}

	protected Rect GetAttrAsRect(string name) {
		Rect result = new Rect();
		float xMin, yMin, xMax, yMax;
		string value = mXmlReader.GetAttribute(name);
		if (string.IsNullOrEmpty(value)) {
			return result;
		}
		string[] nums = value.Split(new char[] { ',' }, 4);
		if (nums.Length != 4) {
			return result;
		}
		float.TryParse(nums[0], out xMin); 
		float.TryParse(nums[1], out yMin); 
		float.TryParse(nums[2], out xMax); 
		float.TryParse(nums[3], out yMax); 
		result.xMin = xMin;
		result.yMin = yMin;
		result.xMax = xMax;
		result.yMax = yMax;
		return result;
	}

	protected Rect GetAttrAsCenterRect(string name) {
		Rect result = new Rect();
		float x = 0, y = 0, w = 0, h = 0;
		string value = mXmlReader.GetAttribute(name);
		if (string.IsNullOrEmpty(value)) {
			return result;
		}
		string[] nums = value.Split(new char[] { ',' }, 4);
		if (nums.Length != 4) {
			return result;
		}
		float.TryParse(nums[0], out x);
		float.TryParse(nums[1], out y);
		float.TryParse(nums[2], out w);
		float.TryParse(nums[3], out h);
		result.xMin = x - w / 2;
		result.xMax = x + w / 2;
		result.yMin = y - h / 2;
		result.yMax = y + h / 2;
		return result;
	}

	protected bool TryParseVec3(out Vector3 vec3, string str) {
		vec3 = new Vector3();
		if (string.IsNullOrEmpty(str)) {
			return false;
		}
		string[] nums = str.Split(new char[] { ',' }, 3);
		if (nums.Length == 0) {
			return false;
		}
		vec3.x = float.Parse(nums[0]);
		vec3.y = float.Parse(nums[1]);
		if (nums.Length >= 3) {
			vec3.z = float.Parse(nums[2]);
		}
		return true;
	}

	protected bool TryParseRect(out Rect rect, string str) {
		rect = new Rect();
		if (string.IsNullOrEmpty(str)) {
			return false;
		}
		string[] nums = str.Split(new char[] { ',' }, 4);
		if (nums.Length == 0) {
			return false;
		}
		rect.xMin = float.Parse(nums[0]);
		rect.yMin = float.Parse(nums[1]);
		rect.xMax = float.Parse(nums[2]);
		rect.yMax = float.Parse(nums[3]);
		return true;
	}

	#endregion

	#region Properties

	public KyCommandManager CommandManager {
		get { return mCommandMgr; }
	}

	#endregion

	#region Fields

	private XmlTextReader mXmlReader;
	private KyCommandManager mCommandMgr;

	#endregion
}
