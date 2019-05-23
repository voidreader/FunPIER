using UnityEngine;
using System.Collections;

public class KyIntermitResult : MonoBehaviour {

	void Start () {
		KySpriteAnimation anim = GetComponent<KySpriteAnimation>();
		if (anim != null) {
			//int index = Mathf.Clamp(Score, 0, ScoreTable.Length - 1);
			anim.Frame = Mathf.Clamp(Comment, 0, 6);
		}
	}

	/*public static readonly int[] ScoreTable = new int[11] {
		0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 6
	};*/

	//public static int Score = 0;
	public static int Comment = 0;
}
