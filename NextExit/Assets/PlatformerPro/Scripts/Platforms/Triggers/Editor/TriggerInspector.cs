using UnityEditor;
using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Custom editor for triggers. Draws a default inspector and then some warnings if settings look wrong.
	/// </summary>
	[CustomEditor (typeof(Trigger), true)]
	public class TriggerInspector : PersistableObjectInspector 
	{
        protected SerializedProperty useDefaultPersistence;

        override protected void OnEnable()
        {
            base.OnEnable();
            useDefaultPersistence = serializedObject.FindProperty("useDefaultPersistence");
        }

        /// <summary>
        /// Draw the GUI
        /// </summary>
        override public void OnInspectorGUI ()
		{

            DrawDefaultInspector();

            serializedObject.Update();

            if (target != null && ((Trigger)target).receivers != null)
			{
				foreach (TriggerTarget t in ((Trigger)target).receivers)
				{
					// Exit actions
					if (t.receiver == null) EditorGUILayout.HelpBox("No receiver set", MessageType.Warning);
					else if (t.enterAction == TriggerActionType.ACTIVATE_PLATFORM || t.enterAction == TriggerActionType.DEACTIVATE_PLATFORM)
					{
						if (t.receiver.GetComponent<Platform>() == null) EditorGUILayout.HelpBox("Expected a Platform receiver for the enter action but Platform was not found.", MessageType.Warning);
					}
					else if (t.enterAction == TriggerActionType.FORWARD)
					{
						if (t.receiver.GetComponent<TriggerCombiner>() == null) EditorGUILayout.HelpBox("Expected a TriggerCombiner for the enter action but TriggerCombiner was not found.", MessageType.Warning);
					}

					// Leave actions
					if (t.receiver == null) { }
					else if (t.leaveAction == TriggerActionType.ACTIVATE_PLATFORM || t.leaveAction == TriggerActionType.DEACTIVATE_PLATFORM)
					{
						if (t.receiver.GetComponent<Platform>() == null) EditorGUILayout.HelpBox("Expected a Platform receiver for the leave action but Platform was not found.", MessageType.Warning);
					}
					else if (t.enterAction == TriggerActionType.FORWARD)
					{
						if (t.receiver.GetComponent<TriggerCombiner>() == null) EditorGUILayout.HelpBox("Expected a TriggerCombiner for the leave action but TriggerCombiner was not found.", MessageType.Warning);
					}

					// TODO More warnings
					if (t.newSprite != null && (t.enterAction == TriggerActionType.SWITCH_SPRITE || t.leaveAction == TriggerActionType.SWITCH_SPRITE))
					{
						EditorGUILayout.HelpBox("Changing sprites via trigger is deprecated. Use an EventResponder instead.", MessageType.Info);
					}
				}
			}

            serializedObject.Update();

            // Persistence
            PersistableObject.UpdateGuid((PersistableObject)target);
            EditorGUILayout.PropertyField(useDefaultPersistence);
            serializedObject.ApplyModifiedProperties();
            if (!useDefaultPersistence.boolValue)
            {
                base.DrawInspector();
            }
            serializedObject.ApplyModifiedProperties();
        }
	}
}
