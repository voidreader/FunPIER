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
	/// Inspector for death movement classes. This is just a damage movement inspector with a different name and type.
	/// </summary>
	[CustomEditor(typeof(DeathMovement), false)]
	public class DeathMovementInspector : Editor
	{
		#region properties
		
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
		
		#region Unity hooks
		
		/// <summary>
		/// When the component is accessed update.
		/// </summary>
		void OnEnable()
		{
			// Note we are looking for damage movements here, death movement is not a type that should be inherited from
			types = typeof(DamageMovement).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(DamageMovement))).ToArray();
			typeNames = types.Select(t=>((MovementInfo)t.GetProperty("Info").GetValue(null, null)).Name).ToArray();
			typeDescriptions = types.Select(t=>((MovementInfo)t.GetProperty("Info").GetValue(null, null)).Description).ToArray();
			selectedTypeIndex = types.Select(t=>t.Name).ToList().IndexOf(((DeathMovement)target).MovementType);
		}
		
		/// <summary>
		/// Draw the inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			// Select type
			int originalIndex = selectedTypeIndex;
			selectedTypeIndex = EditorGUILayout.Popup(selectedTypeIndex, typeNames);
			if (selectedTypeIndex >= 0)
			{
				EditorGUILayout.HelpBox(typeDescriptions[selectedTypeIndex], MessageType.None, true);
				
				// Make sure we reset movement data if the type was changed
				if (originalIndex != selectedTypeIndex) ((DeathMovement)target).MovementData = null;
				
				// Draw types custom inspector
				if (types[selectedTypeIndex].GetMethods().Where (m=>m.Name == "DrawInspector").Count() > 0)
				{
					object[] arguments = new object[]{((DeathMovement)target).MovementData, showDetails, (Character)((Movement)target).Character};
					((DeathMovement)target).MovementData = (MovementVariable[]) types[selectedTypeIndex].GetMethod("DrawInspector").Invoke(null, arguments);
					showDetails = (bool) arguments[1];
				}
				
				// Apply type selection
				((DeathMovement)target).MovementType = types[selectedTypeIndex].Name;
			}
		}
		
		#endregion
		
	}
	
}