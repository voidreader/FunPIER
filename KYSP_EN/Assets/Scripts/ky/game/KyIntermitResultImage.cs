using UnityEngine;
using System.Collections;

public class KyIntermitResultImage : KyScriptObject {

	#region MonoBehaviour Methods

	protected override void Start() {
		base.Start();
		string path;
		path = string.Format("result/intermediateResult{0:D2}", ImageIndex);
		GameObject image = CommandManager.CreateKyObject("image", path);
		image.transform.localPosition = new Vector3(-80, 0, 3);

		if (GameMode == 0) {
			path = "result/result00";
		} else {
			path = "result/result00Boke";
		}
		GameObject mode = CommandManager.CreateKyObject("mode", path);
		mode.transform.localPosition = new Vector3(0, 0, 3);

		path = string.Format("result/result{0:D2}", ResultIndex + 1);
		GameObject text = CommandManager.CreateKyObject("text", path);
		text.transform.localPosition = new Vector3(20, 255, 1);
	}

	#endregion

	#region Fields

	public static int ImageIndex = 18;
	public static int ResultIndex = 1;
	public static int GameMode = 1;

	#endregion
}
