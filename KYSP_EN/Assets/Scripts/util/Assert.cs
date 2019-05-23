using UnityEngine;
using System.Collections;
using System.Diagnostics;

public static class Assert {

	[Conditional("UNITY_EDITOR")]
	public static void AssertTrue(bool condition, string message) {
		AssertCore(condition, message, 2);
	}

	[Conditional("UNITY_EDITOR")]
	public static void AssertTrue(bool condition) {
		AssertCore(condition, "the condition is not true.", 2);
	}

	[Conditional("UNITY_EDITOR")]
	public static void AssertNotNull(Object obj, string message) {
		AssertCore(obj != null, message, 2);
	}

	[Conditional("UNITY_EDITOR")]
	public static void AssertNotNull(Object obj) {
		AssertCore(obj != null, "the object is null.", 2);
	}

	[Conditional("UNITY_EDITOR")]
	private static void AssertCore(bool condition, string message, int depth) {
#if UNITY_EDITOR
		if (!condition) {
			StackTrace trace = new StackTrace(true);
			StackFrame frame = trace.GetFrame(depth);
			string assertInfo =
				"Filename: " + frame.GetFileName() +
				"\nMethod: " + frame.GetMethod() +
				"\nLine: " + frame.GetFileLineNumber();

			UnityEngine.Debug.Break();

			if (UnityEditor.EditorUtility.DisplayDialog("Assert", message + "\n" + assertInfo, "OK")) {
				UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(
					frame.GetFileName(),
					frame.GetFileLineNumber());
				UnityEngine.Debug.Log(assertInfo);
			}
		}
#endif
	}
}
