#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace PlatformerPro
{
	/// <summary>
	/// Inspector for movements which uses reflection to create a pop up of available implementations from which the user can select.
	/// </summary>
	public abstract class BaseMovementInspector <T> : Editor where  T : BaseMovement <T>
	{
		#region members
		
		/// <summary>
		/// The available ground movement classes.
		/// </summary>
		protected System.Type[] types;
		
		/// <summary>
		/// The available ground movement class names.
		/// </summary>
		protected string[] typeNames;
		
		/// <summary>
		/// The available ground movement class description.
		/// </summary>
		protected string[] typeDescriptions;
		
		/// <summary>
		/// Index of the currently selected type.
		/// </summary>
		protected int selectedTypeIndex;
		
		/// <summary>
		/// Should the details view be expanded.
		/// </summary>
		protected bool showDetails;
		
		#endregion

		/// <summary>
		/// When the component is accessed update.
		/// </summary>
		protected void InitTypes()
		{
			types = typeof(T).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(T)) && !type.IsAbstract).ToArray();
			types = types.Where(t=>((MovementInfo)t.GetProperty("Info").GetValue(null, null)).Name != null).ToArray();
			typeNames = types.Select(t=>((MovementInfo)t.GetProperty("Info").GetValue(null, null)).Name).Where (st=>st != null).ToArray();
			typeDescriptions = types.Select(t=>((MovementInfo)t.GetProperty("Info").GetValue(null, null)).Description).Where (st=>st != null).ToArray();
		}

		/// <summary>
		/// Draw the inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			string originalType = ((T)target).MovementType;
			// Store copy of original data to track undo
			int movementVariableSize = ((T)target).MovementData == null ? 0 : ((T)target).MovementData.Length;
			MovementVariable[] originalMovementData = new MovementVariable[movementVariableSize];
			for (int i = 0; i < movementVariableSize; i++)
			{
				originalMovementData [i] = new MovementVariable (((T)target).MovementData [i]);
			}

			// Unity says we don't need to do this, but if we don't do this then the automatic serialised object updates get the same name as the last object recorded
			Undo.FlushUndoRecordObjects ();
			Undo.RecordObject (target, typeof(T).Name + " Update");


			if (target.GetType () == typeof(T))
			{

				// Ensure we have set types data
				if (types == null)
					InitTypes ();

				// Select type
				selectedTypeIndex = types.Select (t => t.Name).ToList ().IndexOf (((T)target).MovementType);
				int originalIndex = selectedTypeIndex;
				selectedTypeIndex = EditorGUILayout.Popup (selectedTypeIndex >= 0 ? selectedTypeIndex : 0, typeNames);
				EditorGUILayout.HelpBox (typeDescriptions [selectedTypeIndex], MessageType.None, true);
				
				// Make sure we reset movement data if the type was changed
				if (originalIndex != selectedTypeIndex)
					((T)target).MovementData = null;
			}
			else
			{
				if (target.GetType ().GetProperty ("Info") != null)
				{
					EditorGUILayout.HelpBox (((MovementInfo)target.GetType ().GetProperty ("Info").GetValue (null, null)).Description, MessageType.None, true);
				}
				else
				{
					DrawDefaultInspector ();
					return;
				}
			}

			// Buttons for saving and loading
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Load Settings"))
			{
				string path = EditorUtility.OpenFilePanel("Load Movement Data", "", "bin");
				if (path.Length > 0)
				{
					MovementVariable[] data = SavableMovementVariable.LoadFromFile (path);
					if (data != null)
					{
						((T)target).MovementData = data;
					}
				}
			}
			if (GUILayout.Button("Save Settings"))
			{
				string path = EditorUtility.SaveFilePanel("Save Movement Data", "", "MovementData", "bin");
				if (path.Length > 0)
				{
					MovementVariable[] data = ((T)target).MovementData;
					SavableMovementVariable.SaveToFile(path, data);
				}
			}
			GUILayout.EndHorizontal();

			// Check for flippable gravity
			if (((T)target).GetComponentInParent<FlippableGravity>() != null && 
			    types[selectedTypeIndex].GetInterface("IFlippableGravityMovement") == null)
			{
				EditorGUILayout.HelpBox("You are using a flippable gravity but this movement does not support it.",
				                        MessageType.Warning, true);
			}

			if (target.GetType () == typeof(T))
			{
				
				// Draw types custom inspector
				if (types [selectedTypeIndex].GetMethods ().Where (m => m.Name == "DrawInspector").Count () > 0)
				{
					object[] arguments = new object[] {
						((T)target).MovementData,
						showDetails,
						(Character)((Movement)target).Character
					};
					((T)target).MovementData = (MovementVariable[])types [selectedTypeIndex].GetMethod ("DrawInspector").Invoke (null, arguments);
					showDetails = (bool)arguments [1];
				}
				// Apply type selection
				((T)target).MovementType = types[selectedTypeIndex].Name;

				// Set dirty if changed
				if ((((T)target).MovementType != originalType)  ||
					((T)target).IsMovementDataDifferent(originalMovementData))
				{
					EditorUtility.SetDirty(target);
				}
			}
			else
			{
				// Draw types custom inspector
				if (target.GetType().GetMethods ().Where (m => m.Name == "DrawInspector").Count () > 0)
				{
					object[] arguments = new object[] {
						((T)target).MovementData,
						showDetails,
						(Character)((Movement)target).Character
					};
					((T)target).MovementData = (MovementVariable[])target.GetType().GetMethod ("DrawInspector").Invoke (null, arguments);
					showDetails = (bool)arguments [1];
				}
				// Set dirty if changed
				if (((T)target).IsMovementDataDifferent(originalMovementData))
				{
					EditorUtility.SetDirty(target);
				}
			}

		}
	}
}