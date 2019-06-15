#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro
{
	[CustomEditor(typeof(PowerUpResponder), false)]
	public class PowerUpResponderInspector : Editor
	{

		/// <summary>
		/// Stores visibility for each repsonse type.
		/// </summary>
		protected Dictionary<PowerUpResponse, bool> responseVisibility;

		/// <summary>
		/// Stores visibility of the reset response item.
		/// </summary>
		protected bool resetResponseVisibility;

		/// <summary>
		/// Cached and typed target reference.
		/// </summary>
		protected PowerUpResponder myTarget;

		/// <summary>
		/// Cached list of response names.
		/// </summary>
		protected List<string> responseNames;

		/// <summary>
		/// Draw the GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			myTarget = (PowerUpResponder)target;
			if (responseVisibility == null) {
				responseVisibility = new Dictionary<PowerUpResponse, bool> ();
			}
			if (myTarget.responses == null)
			{
				myTarget.responses = new List<PowerUpResponse>();
			}

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			// Add new actions
			if (GUILayout.Button("Add PowerUp Type"))
			{
				if (myTarget.responses == null)
				{
					myTarget.responses = new List<PowerUpResponse>();
					myTarget.responses.Add (new PowerUpResponse());
				}
				else
				{
					myTarget.responses.Add (new PowerUpResponse());
				}
			}
			EditorGUILayout.EndHorizontal();

			// Cache response names
			if (responseNames == null) responseNames = new List<string>();
			if (myTarget.responses.Count != responseNames.Count) 
			{
				responseNames = new List<string>();
				foreach (PowerUpResponse response in myTarget.responses)
				{
					responseNames.Add (response.type);
				}
			}

			if (myTarget.responses != null)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10);
				EditorGUILayout.BeginVertical();
				RenderResetResponse();
				for (int i = 0; i < myTarget.responses.Count; i++)
				{
					if (!responseVisibility.ContainsKey(myTarget.responses[i])) responseVisibility.Add (myTarget.responses[i], false);
					RenderResponse(myTarget.responses[i]);
				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}



		}

		virtual protected void RenderResponse (PowerUpResponse response) {
			responseVisibility[response] = EditorGUILayout.Foldout(responseVisibility[response], response.type);
			if(responseVisibility[response])
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10);
				GUILayout.Box("", GUILayout.Width(1), GUILayout.ExpandHeight(true));
				EditorGUILayout.BeginVertical();

				string type = EditorGUILayout.TextField(new GUIContent("PowerUp Type", "Type of the PowerUp."), response.type);
				if (type != response.type)
				{
					response.type = type;
					EditorUtility.SetDirty(target);
					responseNames = null;
				}

				bool resetOnDamage = EditorGUILayout.Toggle(new GUIContent("Reset on Damage", "Should the power up be removed if the character is damaged)."), response.resetOnDamage);
				if (resetOnDamage != response.resetOnDamage)
				{
					response.resetOnDamage = resetOnDamage;
					EditorUtility.SetDirty(target);
				}

				int timer = EditorGUILayout.IntField(new GUIContent("PowerUp Timer", "Time the PowerUp is active for (use 0 for unlimited)."), response.time);
				if (timer < 0) timer = 0;
				if (timer != response.time)
				{
					response.time = timer;
					EditorUtility.SetDirty(target);
				}
				// Show resets if timer > 0
				if (timer > 0.0f)
				{
					int originalResetIndex = responseNames.IndexOf(response.powerUpReset);
					int selectedResetIndex = originalResetIndex;
					selectedResetIndex = EditorGUILayout.Popup("Reset Response", originalResetIndex, responseNames.ToArray());
					if (originalResetIndex != selectedResetIndex)
					{
						response.powerUpReset = responseNames[selectedResetIndex];
					}
				}

				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				// Add new actions
				if (GUILayout.Button("Add Action"))
				{
					if (response.actions == null)
					{
						response.actions = new EventResponse[1];
					}
					else
					{
						// Copy and grow array
						EventResponse[] tmpActions = response.actions;
						response.actions = new EventResponse[tmpActions.Length + 1];
						System.Array.Copy(tmpActions, response.actions, tmpActions.Length);
					}
					EditorUtility.SetDirty(target);
				}
				if (GUILayout.Button("Remove PowerUp Type"))
				{
					myTarget.responses.Remove (response);
					EditorUtility.SetDirty(target);
				}

				EditorGUILayout.EndHorizontal();

				if (response.actions != null)
				{

					for (int i = 0; i < response.actions.Length; i++)
					{
						EventResponderInspector.RenderAction(target, response, response.actions[i]);
					}

				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}
		}

		virtual protected void RenderResetResponse () {
			resetResponseVisibility = EditorGUILayout.Foldout(resetResponseVisibility, "RESET");
			if(resetResponseVisibility)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10);
				GUILayout.Box("", GUILayout.Width(1), GUILayout.ExpandHeight(true));
				EditorGUILayout.BeginVertical();

				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				// Add new actions
				if (GUILayout.Button("Add Action"))
				{
					if (myTarget.resetResponse.actions == null)
					{
						myTarget.resetResponse.actions = new EventResponse[1];
					}
					else
					{
						// Copy and grow array
						EventResponse[] tmpActions = myTarget.resetResponse.actions;
						myTarget.resetResponse.actions = new EventResponse[tmpActions.Length + 1];
						System.Array.Copy(tmpActions, myTarget.resetResponse.actions, tmpActions.Length);
					}
					EditorUtility.SetDirty(target);
				}

				EditorGUILayout.EndHorizontal();
				
				if (myTarget.resetResponse.actions != null)
				{
					
					for (int i = 0; i < myTarget.resetResponse.actions.Length; i++)
					{
						EventResponderInspector.RenderAction(target, myTarget.resetResponse, myTarget.resetResponse.actions[i]);
					}
					
				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}
		}

	
	}
}