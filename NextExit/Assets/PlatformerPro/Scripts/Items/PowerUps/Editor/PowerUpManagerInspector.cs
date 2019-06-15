#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro
{
	[CustomEditor(typeof(PowerUpManager), false)]
	public class PowerUpManagerInspector : Editor
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
		protected PowerUpManager myTarget;

		/// <summary>
		/// Draw the GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			if (ItemTypeManager.Instance == null)
			{
				EditorGUILayout.HelpBox("You need to add an ItemTypeManager to your scene and add some item data.", MessageType.Warning);
				if (GUILayout.Button("Do It Now")) {
					ItemTypeEditor.ShowMainWindow ();
				}
				return;
			}

			EditorGUILayout.HelpBox ("Use the PowerUpManager to add extra actions to your power-ups. " +
				"Note that many movement based power-ups can now be handled using the PowerUpActiveCondition, " +
				"PowerUpManager is mainly intended for things like particle effects and sprite swaps", MessageType.Info);
			
			// Unity says we don't need to do this, but if we don't do this then serialised object updates get the same name as the last object recorded
			Undo.FlushUndoRecordObjects ();
			Undo.RecordObject (target, "PowerUp Update");
			myTarget = (PowerUpManager)target;
			if (responseVisibility == null) {
				responseVisibility = new Dictionary<PowerUpResponse, bool> ();
			}
			if (myTarget.responses == null)
			{
				myTarget.responses = new List<PowerUpResponse>();
			}

			// Get responses from ItemTypeManager
			List<ItemTypeData> itemTypes = ItemTypeManager.Instance.ItemData;
			itemTypes = itemTypes.Where (i => i.itemBehaviour == ItemBehaviour.POWER_UP).ToList ();
			List<PowerUpResponse> newResponses = new List<PowerUpResponse>();
			List<PowerUpResponse> responsesToRemove = new List<PowerUpResponse>();
			foreach (ItemTypeData type in itemTypes)
			{
				bool matchFound = false;
				foreach (PowerUpResponse response in myTarget.responses) 
				{
					bool reverseMatchFound = false;
					if (response.typeId == type.typeId)
					{
						matchFound = true;
					}
					foreach (ItemTypeData innerType in itemTypes)
					{
						if (response.typeId == innerType.typeId)
						{
							reverseMatchFound = true;
						}
					}
					foreach (PowerUpResponse innerResponse in myTarget.responses) 
					{
						if (response != innerResponse && response.typeId == innerResponse.typeId)
						{
							if (!responsesToRemove.Contains (innerResponse) && !responsesToRemove.Contains (response))
							{
								if (innerResponse.actions != null && innerResponse.actions.Length > 0)
								{
									Debug.LogWarning ("Duplicate power-up definitions found, but they both have actions. To clean up manually remove the actions from one entry");
								} 
								else
								{
									responsesToRemove.Add (innerResponse);
								}
							}
						}
					}
					if (!reverseMatchFound)
					{
						responsesToRemove.Add (response);
					}
				}
				if (!matchFound)
				{
					PowerUpResponse newResponse = new PowerUpResponse ();
					newResponse.typeId = type.typeId;
					newResponses.Add (newResponse);
				}
			}
			myTarget.responses.RemoveAll (responsesToRemove.Contains);
			myTarget.responses.AddRange (newResponses);

			// Cache response names
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(10);
			EditorGUILayout.BeginVertical();
			RenderResetResponse();
		
			if (myTarget.responses != null && myTarget.responses.Count > 0)
			{
				for (int i = 0; i < myTarget.responses.Count; i++)
				{
					if (!responseVisibility.ContainsKey(myTarget.responses[i])) responseVisibility.Add (myTarget.responses[i], false);
					RenderResponse(myTarget.responses[i]);
				}
			
			}

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}

		virtual protected void RenderResponse (PowerUpResponse response) {
			responseVisibility[response] = EditorGUILayout.Foldout(responseVisibility[response], response.typeId);
			if (responseVisibility[response])
			{
				ItemTypeData itemData = ItemTypeManager.Instance.GetTypeData (response.typeId);
				GUILayout.BeginVertical(EditorStyles.textArea);

				GUILayout.Space(5);
				GUILayout.Label("PowerUp Actions");
				GUILayout.Space(5);

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
				}
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(5);

				if (response.actions != null)
				{
					for (int i = 0; i < response.actions.Length; i++)
					{
						EditorGUILayout.BeginVertical ("HelpBox");
						GUILayout.BeginHorizontal ();
						GUILayout.FlexibleSpace ();
						if (i == 0) GUI.enabled = false;
						if (GUILayout.Button ("Move Up", EditorStyles.miniButtonLeft))
						{
							EventResponse tmp = response.actions[i-1];
							response.actions[i-1] = response.actions[i];
							response.actions[i] = tmp;
							break;
						}
						GUI.enabled = true;
						if (i == response.actions.Length - 1) GUI.enabled = false;
						if (GUILayout.Button ("Move Down", EditorStyles.miniButtonRight))
						{
							EventResponse tmp = response.actions[i+1];
							response.actions[i+1] = response.actions[i];
							response.actions[i] = tmp;
							break;
						}
						GUI.enabled = true;
						// Remove
						GUILayout.Space(4);
						bool removed = false;
						if (GUILayout.Button("Remove", EditorStyles.miniButton))
						{
							response.actions = response.actions.Where (a=>a != response.actions[i]).ToArray();
							removed = true;
						}
						GUILayout.EndHorizontal ();
						if (!removed) EventResponderInspector.RenderAction(target, response.actions[i]);
						EditorGUILayout.EndVertical();
					}

				}
				GUILayout.EndVertical();


				// Show resets if timer > 0
				if (itemData != null && (itemData.effectDuration > 0 || itemData.resetEffectOnDamage))
				{
					GUILayout.BeginVertical(EditorStyles.textArea);

					GUILayout.Space(5);
					GUILayout.Label("PowerUp Reset Actions");
					GUILayout.Space(5);

					EditorGUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					// Add new actions
					if (GUILayout.Button("Add Action"))
					{
						if (response.resetActions == null)
						{
							response.resetActions = new EventResponse[1];
						}
						else
						{
							// Copy and grow array
							EventResponse[] tmpActions = response.resetActions;
							response.resetActions = new EventResponse[tmpActions.Length + 1];
							System.Array.Copy(tmpActions, response.resetActions, tmpActions.Length);
						}
					}
					EditorGUILayout.EndHorizontal();
					GUILayout.Space(5);

					if (response.resetActions != null)
					{
						for (int i = 0; i < response.resetActions.Length; i++)
						{
							EditorGUILayout.BeginVertical ("HelpBox");
							GUILayout.BeginHorizontal ();
							GUILayout.FlexibleSpace ();
							if (i == 0) GUI.enabled = false;
							if (GUILayout.Button ("Move Up", EditorStyles.miniButtonLeft))
							{
								EventResponse tmp = response.resetActions[i-1];
								response.resetActions[i-1] = response.resetActions[i];
								response.resetActions[i] = tmp;
								break;
							}
							GUI.enabled = true;
							if (i == response.resetActions.Length - 1) GUI.enabled = false;
							if (GUILayout.Button ("Move Down", EditorStyles.miniButtonRight))
							{
								EventResponse tmp = response.resetActions[i+1];
								response.resetActions[i+1] = response.actions[i];
								response.resetActions[i] = tmp;
								break;
							}
							GUI.enabled = true;
							// Remove
							GUILayout.Space(4);
							bool removed = false;
							if (GUILayout.Button("Remove", EditorStyles.miniButton))
							{
								response.resetActions = response.resetActions.Where (a=>a != response.resetActions[i]).ToArray();
								removed = true;
							}
							GUILayout.EndHorizontal ();
							if (!removed) EventResponderInspector.RenderAction(target, response.resetActions[i]);
							EditorGUILayout.EndVertical();
						}

					}
					GUILayout.EndVertical ();
				}
			}
		}

		virtual protected void RenderResetResponse () {

			resetResponseVisibility = EditorGUILayout.Foldout(resetResponseVisibility, "Reset Actions");
			if(resetResponseVisibility)
			{
				
				GUILayout.BeginVertical(EditorStyles.textArea);
				GUILayout.Space(5);
				EditorGUILayout.HelpBox ("The Reset Actions are applied called when a character respawns and is primarily used to clear any old power-up behaviour when you do not reload scene on death.", MessageType.Info);
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
				}

				EditorGUILayout.EndHorizontal();
				
				if (myTarget.resetResponse.actions != null)
				{
					for (int i = 0; i < myTarget.resetResponse.actions.Length; i++)
					{
						EditorGUILayout.BeginVertical ("HelpBox");
						
						GUILayout.BeginHorizontal ();
						GUILayout.FlexibleSpace ();
						if (i == 0) GUI.enabled = false;
						if (GUILayout.Button ("Move Up", EditorStyles.miniButtonLeft))
						{
							EventResponse tmp = myTarget.resetResponse.actions[i-1];
							myTarget.resetResponse.actions[i-1] = myTarget.resetResponse.actions[i];
							myTarget.resetResponse.actions[i] = tmp;
							break;
						}
						GUI.enabled = true;
						if (i == myTarget.resetResponse.actions.Length - 1) GUI.enabled = false;
						if (GUILayout.Button ("Move Down", EditorStyles.miniButtonRight))
						{
							EventResponse tmp = myTarget.resetResponse.actions[i+1];
							myTarget.resetResponse.actions[i+1] = myTarget.resetResponse.actions[i];
							myTarget.resetResponse.actions[i] = tmp;
							break;
						}
						GUI.enabled = true;
						// Remove
						GUILayout.Space(4);
						bool removed = false;
						if (GUILayout.Button("Remove", EditorStyles.miniButton))
						{
							myTarget.resetResponse.actions = myTarget.resetResponse.actions.Where (a=>a != myTarget.resetResponse.actions[i]).ToArray();
							removed = true;
						}
						GUILayout.EndHorizontal ();
						if (!removed) EventResponderInspector.RenderAction(target, myTarget.resetResponse.actions[i]);
						EditorGUILayout.EndVertical();
					}
					
				}
				EditorGUILayout.EndVertical();
			}

		}

	
	}
}