using UnityEngine;
using System.Collections;

public class GizmosUtil {

	public static void DrawRect(Rect rect) {
		Gizmos.DrawWireCube(
			new Vector3((rect.xMax + rect.xMin) / 2, (rect.yMax + rect.yMin) / 2, 0),
			new Vector3(rect.width, rect.height, 1));
	}
}
