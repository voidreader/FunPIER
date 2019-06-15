#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlatformerPro.Extras;

namespace PlatformerPro
{
	[CustomEditor(typeof(EventResponder), true)]
	public class EventResponderInspector : Editor
	{
		/// <summary>
		/// Cached and typed target reference.
		/// </summary>
		protected EventResponder myTarget;

		/// <summary>
		/// Cached types for target.
		/// </summary>
		protected string[] types;

		/// <summary>
		/// Cached events for type.
		/// </summary>
		protected string[] events;

		protected System.Type type;
		protected System.Reflection.EventInfo eventInfo;
		protected System.Type parameterType;

		/// <summary>
		/// Draw the GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			myTarget = (EventResponder)target;

			GameObject sender = (GameObject) EditorGUILayout.ObjectField(new GUIContent("Sender", "Add the Game Object that holds the target component."), myTarget.sender, typeof(GameObject), true);

			if (sender != myTarget.sender) 
			{
				myTarget.sender = sender;
				if (myTarget.sender != null) types = GetComponentsOnGameObject(myTarget.sender);
			}

			if (myTarget.sender == null) myTarget.sender = myTarget.gameObject;

			if (myTarget.sender != null)
			{
				string typeName = null;
				if (types == null) types = GetComponentsOnGameObject(myTarget.sender);
				int typeIndex = System.Array.IndexOf(types, myTarget.typeName);
				if (typeIndex == -1 || typeIndex >= types.Length) typeIndex = 0;
				if (types != null && types.Length > 0)
				{	
					typeName = types[EditorGUILayout.Popup("Component", typeIndex, types)];
				}
				else
				{
					EditorGUILayout.HelpBox("No components found on this GameObject.", MessageType.Info);
				}
				if (typeName != myTarget.typeName && typeName != null) 
				{
					myTarget.typeName = typeName;
					if (myTarget.typeName != null) events = GetEventNamesForType(myTarget.typeName);
				}
				if (myTarget.typeName != null && myTarget.typeName.Length > 0)
				{
					if (events == null) events = GetEventNamesForType(myTarget.typeName);
					int eventIndex = System.Array.IndexOf(events, myTarget.eventName);
					if (eventIndex == -1 || eventIndex >= events.Length) eventIndex = 0;
					if (events != null && events.Length > 0)
					{
						string name = events[EditorGUILayout.Popup("Event", eventIndex, events)];
						if (name != myTarget.eventName || parameterType == null)
						{
							myTarget.eventName = name;
							//try
							//{
								type = myTarget.GetType().Assembly.GetType(this.GetType().Namespace + "." + typeName);
								eventInfo = type.GetEvent(myTarget.eventName);
								parameterType = eventInfo.EventHandlerType.GetMethod("Invoke").GetParameters()[1].ParameterType;
							//}
							//catch (System.Exception ex) {
							//	Debug.Log (ex.Message);
							//}
						}

						// Animation event
						if (parameterType != null && parameterType.IsAssignableFrom(typeof(AnimationEventArgs)))
						{
							myTarget.animationStateFilter = (AnimationState) EditorGUILayout.EnumPopup(new GUIContent("Animation State", "The animation state which will trigger this event response, use NONE for any state"),
							                          myTarget.animationStateFilter);
						}
						// Damage event
						if (parameterType != null && parameterType.IsAssignableFrom(typeof(DamageInfoEventArgs)))
						{
							myTarget.damageTypeFilter = (DamageType) EditorGUILayout.EnumPopup(new GUIContent("Damage Type", "The damage type which will trigger this event response, use NONE for any type"),
							                                                                       myTarget.damageTypeFilter);
						}
						// Button event
						if (parameterType != null && parameterType.IsAssignableFrom(typeof(ButtonEventArgs)))
						{
							myTarget.buttonStateFilter = (ButtonState) EditorGUILayout.EnumPopup(new GUIContent("Button State", "The button state which triggers this response, use ANY for any type"),
							                                                                   myTarget.buttonStateFilter);
						}
						// Phase event
						if (parameterType != null && parameterType.IsAssignableFrom(typeof(PhaseEventArgs)))
						{
							myTarget.stringFilter = EditorGUILayout.TextField(new GUIContent("Phase", "Name of the phase or empty string for any phase."),
							                                                                myTarget.stringFilter);
						}
						// State event
						if (parameterType != null && parameterType.IsAssignableFrom(typeof(StateEventArgs)))
						{
							myTarget.stringFilter = EditorGUILayout.TextField(new GUIContent("State", "Name of the state or empty string for any state."),
							                                                                myTarget.stringFilter);
						}
					}
					else
					{
						EditorGUILayout.HelpBox("No events found on this component.", MessageType.Info);
					}
				}
			}

			if (myTarget.actions != null)
			{
				for (int i = 0; i < myTarget.actions.Length; i++)
				{
					RenderAction(myTarget, myTarget, myTarget.actions[i]);
				}
			}

			GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			// Add new actions
			if (GUILayout.Button("Add Action"))
			{
				if (myTarget.actions == null)
				{
					myTarget.actions = new EventResponse[1];
				}
				else
				{
					// Copy and grow array
					EventResponse[] tmpActions = myTarget.actions;
					myTarget.actions = new EventResponse[tmpActions.Length + 1];
					System.Array.Copy(tmpActions, myTarget.actions, tmpActions.Length);
				}
			}
			EditorGUILayout.EndHorizontal();

		}
	
		/// <summary>
		/// Draws an event response action in the inspector.
		/// </summary>
		/// <param name="action">Action.</param>
		public static void RenderAction(object target, object repsonder, EventResponse action)
		{
			if (!(target is EventResponder || target is PowerUpResponder)) 
			{
				Debug.LogWarning ("Unexpected type passed to RenderAction()");
				return;
			}

			GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
			if (action == null) action = new EventResponse();
			EventResponse originalAction = new EventResponse (action);
			action.responseType = (EventResponseType) EditorGUILayout.EnumPopup( new GUIContent("Action Type", "The type of action to do when this event occurs."), action.responseType);

			// Delay
			action.delay = EditorGUILayout.FloatField( new GUIContent("Action Delay", "how long to wait before doing the action."), action.delay);
			if (action.delay < 0.0f) action.delay = 0.0f;
			else if (action.delay > 0.0f) EditorGUILayout.HelpBox("If you use many events with delay you may notice some garbage collection issues on mobile devices", MessageType.Info);

			// Game Object
			if (action.responseType == EventResponseType.ACTIVATE_GAMEOBJECT ||
			    action.responseType == EventResponseType.DEACTIVATE_GAMEOBJECT ||
			    action.responseType == EventResponseType.SEND_MESSSAGE)
			{
				action.targetGameObject = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Game Object", "The game object that will be acted on"), action.targetGameObject , typeof(GameObject), true);
			}

			// Component
			if (action.responseType == EventResponseType.ENABLE_BEHAVIOUR ||
			    action.responseType == EventResponseType.DISABLE_BEHAVIOUR)
			{
				action.targetComponent = (Component)EditorGUILayout.ObjectField(new GUIContent("Behaviour", "The behaviour will be acted on"), action.targetComponent , typeof(Component), true);
			}

			// Particle system
			if (action.responseType == EventResponseType.PLAY_PARTICLES ||
			    action.responseType == EventResponseType.PAUSE_PARTICLES)
			{
				action.targetComponent = (Component)EditorGUILayout.ObjectField(new GUIContent("Particle System", "The particle system that will be acted on"), action.targetComponent , typeof(ParticleSystem), true);
			}

			// Send message
			if (action.responseType == EventResponseType.SEND_MESSSAGE) action.message = EditorGUILayout.TextField(new GUIContent("Message", "The message to send via send message"), action.message);

			// Animation Override
			if (action.responseType == EventResponseType.OVERRIDE_ANIMATON)
			{
				action.targetComponent = (Component)EditorGUILayout.ObjectField(new GUIContent("Character", "Character to update."), action.targetComponent , typeof(Character), true);
				action.overrideState = EditorGUILayout.TextField(new GUIContent("Override State", "The name of the override state."), action.overrideState);
			}

			// Clear Animation Override
			if (action.responseType == EventResponseType.CLEAR_ANIMATION_OVERRIDE)
			{
				action.targetComponent = (Component)EditorGUILayout.ObjectField(new GUIContent("Character", "Character to update."), action.targetComponent , typeof(Character), true);
				action.overrideState = EditorGUILayout.TextField(new GUIContent("Override State", "The name of the override state to clear."), action.overrideState);
			}

			// Force Animation
			if (action.responseType == EventResponseType.SPECIAL_MOVE_ANIMATION)			
			{
				EditorGUILayout.HelpBox ("This type has been deprectaed use PLAY_ANIMATION with a Character as your target instead.", MessageType.Warning);
				action.targetComponent = (Component)EditorGUILayout.ObjectField(new GUIContent("Character", "Character to update."), action.targetComponent , typeof(Character), true);
				action.animationState = (AnimationState) EditorGUILayout.EnumPopup(new GUIContent("Animation State", "The name of the override state."), action.animationState);
			}

			// Sprite
			if (action.responseType == EventResponseType.SWITCH_SPRITE)
			{
				action.targetComponent = (Component)EditorGUILayout.ObjectField(new GUIContent("Sprite Renderer", "SpriteRenderer to update."), action.targetComponent , typeof(SpriteRenderer), true);
				action.newSprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("New Sprite", "Sprite to switch in."), action.newSprite , typeof(Sprite), true);
			}

			// SFX
			if (action.responseType == EventResponseType.PLAY_SFX)
			{
				action.targetComponent = (Component)EditorGUILayout.ObjectField(new GUIContent("Sound Effect", "The sound effect to play."), action.targetComponent , typeof(SoundEffect), true);
			}

			// MUSIC PLAYER
			if (action.responseType == EventResponseType.PLAY_SONG ||
			    action.responseType == EventResponseType.STOP_SONG)
			{
				action.targetComponent = (Component)EditorGUILayout.ObjectField(new GUIContent("Music Player", "The music player."), action.targetComponent , typeof(MusicPlayer), true);
				if (action.responseType == EventResponseType.PLAY_SONG)
				{
					action.message = EditorGUILayout.TextField(new GUIContent("Song Name", "The name of the song to play."), action.message);
				}
			}

			
			// Vulnerable/Invulnerable
			if (action.responseType == EventResponseType.MAKE_VULNERABLE ||
			    action.responseType == EventResponseType.MAKE_INVULNERABLE)
			{
				if (action.targetComponent is CharacterHealth)
				{
					action.targetComponent = action.targetComponent.gameObject.GetComponentInParent(typeof(IMob));
				}
				action.targetComponent = (Component)EditorGUILayout.ObjectField(new GUIContent("Character", "The character or enemy that will be acted on"), action.targetComponent , typeof(IMob), true);
			}

			// Load level
			if (action.responseType == EventResponseType.LOAD_SCENE) {
				action.message = EditorGUILayout.TextField(new GUIContent("Scene Name", "The name of the scene to load (make sure its added to the build settings)."), action.message);
			}

			// Load level
			if (action.responseType == EventResponseType.LOCK ||
			    action.responseType == EventResponseType.UNLOCK) {
				action.message = EditorGUILayout.TextField(new GUIContent("Lock Name", "The name of the lock (often a level name but it doesn't have to be)."), action.message);
			}

			// Respawn
			if (action.responseType == EventResponseType.RESPAWN ||
			    action.responseType == EventResponseType.SET_ACTIVE_RESPAWN ) {
				action.message = EditorGUILayout.TextField(new GUIContent("Respawn Point Name", "The name of the respawn point to respawn at, leave blank for whatever is currently active."), action.message);
			}

			// Effects
			if (action.responseType == EventResponseType.START_EFFECT)
			{
				action.targetComponent = (Component)EditorGUILayout.ObjectField(new GUIContent("Effect", "The effect that will be played."), action.targetComponent , typeof(FX_Base), true);
				action.targetGameObject = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Callback Object", "The game object that will be called back when the effect is finished"), action.targetGameObject , typeof(GameObject), true);
				if (action.targetComponent != null && action.targetGameObject != null)
				{
					action.message = EditorGUILayout.TextField(new GUIContent("Callback Message", "The name message to send on call back."), action.message);
					EditorGUILayout.HelpBox("Note that many effects do not support call backs.", MessageType.Info);
				}
			}

			// Animations
			if (action.responseType == EventResponseType.PLAY_ANIMATION ||
			    action.responseType == EventResponseType.STOP_ANIMATION)
			{
				action.targetGameObject = (GameObject) EditorGUILayout.ObjectField(new GUIContent("Character/Animator", "GameObject holding a Character, Animation or Animator to play or stop."), action.targetGameObject, typeof(GameObject), true);
			}
			
			// Animation state
			if (action.responseType == EventResponseType.PLAY_ANIMATION && action.targetGameObject != null) {
				Character character = action.targetGameObject.GetComponent<Character>();
				SpecialMovement specialMovement = action.targetGameObject.GetComponentInChildren<SpecialMovement>();
				if (specialMovement != null)
				{
					action.animationState = (AnimationState)EditorGUILayout.EnumPopup (new GUIContent ("Animation State", "The state to play."), action.animationState);
				}
				else
				{
					if (character != null)
					{
						EditorGUILayout.HelpBox("If using a Character it is recommneded that you add a SpecialMovement_PlayAnimation to handle playing animations.", MessageType.Warning);
					}
					Animator animator = action.targetGameObject.GetComponent<Animator> ();
					if (animator != null)
					{
						action.message = EditorGUILayout.TextField (new GUIContent ("Animation State", "Name of the Animation to play."), action.message);
					}
				}
			}
			
			// Animation state
			if (action.responseType == EventResponseType.STOP_ANIMATION)
			{
				SpecialMovement specialMovement = action.targetGameObject == null ? null : action.targetGameObject.GetComponentInChildren<SpecialMovement>();
				Animator animator = action.targetGameObject == null ? null : action.targetGameObject.GetComponent<Animator>();
				if (specialMovement == null && animator != null)
				{
					EditorGUILayout.HelpBox("You cannot stop an Animator only an Animation or Character Special Movement. Instead use PLAY_ANIMATION and provide an IDLE or DEFAULT state", MessageType.Warning);
				}
			}

			if (!action.Equals(originalAction)) 
			{
				if (target is EventResponder) EditorUtility.SetDirty((EventResponder)target);
				if (target is PowerUpResponder) EditorUtility.SetDirty((PowerUpResponder)target);
			}


			// Scores
			if (action.responseType == EventResponseType.ADD_SCORE ||
			    action.responseType == EventResponseType.RESET_SCORE) 
			{
				action.message = EditorGUILayout.TextField(new GUIContent("Score Type", "ID string for the score type."), action.message);
			}
			if (action.responseType == EventResponseType.ADD_SCORE)
		    {
				action.intValue = EditorGUILayout.IntField(new GUIContent("Amount", "How much score to add."), action.intValue);
			}

			// Max health/lives
			if (action.responseType == EventResponseType.UPDATE_MAX_HEALTH ||
			    action.responseType == EventResponseType.SET_MAX_HEALTH ||
			    action.responseType == EventResponseType.UPDATE_MAX_LIVES ||
			    action.responseType == EventResponseType.SET_MAX_LIVES ||
			    action.responseType == EventResponseType.UPDATE_LIVES) {
				if (!(action.targetComponent is CharacterHealth)) action.targetComponent = null;
				action.targetComponent = (Component) EditorGUILayout.ObjectField(new GUIContent("CharacterHealth", "Reference to the CharacterHealth"), action.targetComponent, typeof(CharacterHealth), true);
			}

			
			// Item max
			if (action.responseType == EventResponseType.UPDATE_ITEM_MAX ||
			    action.responseType == EventResponseType.SET_ITEM_MAX) {
				if (!(action.targetComponent is ItemManager)) action.targetComponent = null;
				action.targetComponent = (Component) EditorGUILayout.ObjectField(new GUIContent("ItemManager", "Reference to the ItemManager"), action.targetComponent, typeof(ItemManager), true);
				action.message = EditorGUILayout.TextField(new GUIContent("Item Type", "Type of item to update max for"), action.message);
			}

			// Update Max health/lives/item max
			if (action.responseType == EventResponseType.UPDATE_MAX_HEALTH ||
			    action.responseType == EventResponseType.UPDATE_MAX_LIVES ||
			    action.responseType == EventResponseType.UPDATE_LIVES ||
			    action.responseType == EventResponseType.UPDATE_ITEM_MAX) {
				action.intValue = EditorGUILayout.IntField(new GUIContent("Amount", "How much to add or remove."), action.intValue);
			}

			// Set Max health/lives/item max
			if (action.responseType == EventResponseType.SET_MAX_HEALTH ||
			    action.responseType == EventResponseType.SET_MAX_LIVES ||
			    action.responseType == EventResponseType.SET_ITEM_MAX) {
				action.intValue = EditorGUILayout.IntField(new GUIContent("New Value", "The new value."), action.intValue);
			}

			// Named properties
			if (action.responseType == EventResponseType.SET_TAGGED_PROPERTY ||
			    action.responseType == EventResponseType.ADD_TO_TAGGED_PROPERTY ||
			    action.responseType == EventResponseType.MULTIPLY_TAGGED_PROPERTY)
			{
				action.targetComponent = (Component)EditorGUILayout.ObjectField(new GUIContent("Character", "Character to set property for."), action.targetComponent , typeof(Character), true);
				action.message = EditorGUILayout.TextField(new GUIContent("Property", "Named property type."), action.message);
				action.floatValue = EditorGUILayout.FloatField(new GUIContent("New Value", "The new value (float)."), action.floatValue);
			}

			if (action.responseType == EventResponseType.SET_TAGGED_PROPERTY)
			{
				EditorGUILayout.HelpBox("For booleans use 0 for false, anything else for true.", MessageType.Info);
			}

			// Spawn item
			if (action.responseType == EventResponseType.SPAWN_ITEM)
			{
				if (!(action.targetComponent is RandomItemSpawner)) action.targetComponent = null;
				action.targetComponent = (Component) EditorGUILayout.ObjectField(new GUIContent("Item Spawner", "Reference to the RandomItemSpawner"), action.targetComponent, typeof(RandomItemSpawner), true);
			}

			
			// Velocity
			if (action.responseType == EventResponseType.ADD_VELOCITY ||
			    action.responseType == EventResponseType.SET_VELOCITY) 
			{
				action.targetComponent = (Component)EditorGUILayout.ObjectField(new GUIContent("Reciever", "The behaviour that will have velocity added or set"), action.targetComponent , typeof(Component), true);
				if (!(action.targetComponent is IMob || action.targetComponent is Rigidbody2D))
				{
					EditorGUILayout.HelpBox("Component must be a Character, Enemy or Rigidbody 2D. To drag specific components try locking an inspector window.", MessageType.Warning);
				}
				action.vectorValue = EditorGUILayout.Vector2Field(new GUIContent("Velocity", "Velocity to add or set."), action.vectorValue);
				action.boolValue = EditorGUILayout.Toggle(new GUIContent("Velocity is Relative", "Is velocity relative to facing direction or rigidbody rotation?"), action.boolValue);
			}


			// Remove
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Remove Action"))
			{
				if (target is EventResponder) ((EventResponder)target).actions = ((EventResponder)target).actions.Where (a=>a != action).ToArray();
				if (repsonder is PowerUpResponse) ((PowerUpResponse)repsonder).actions = ((PowerUpResponse)repsonder).actions.Where (a=>a != action).ToArray();
			}
			EditorGUILayout.EndHorizontal();
		}

		/// <summary>
		/// Get the names of all events for a given type.
		/// </summary>
		/// <returns>The event names for type.</returns>
		/// <param name="type">Type.</param>
		protected string[] GetEventNamesForType(string typeName)
		{
			System.Type type = typeof(Character).Assembly.GetType(this.GetType().Namespace + "." + typeName);
			if (type == null) type = typeof(BoxCollider2D).Assembly.GetType(this.GetType().Namespace + "." + typeName);
			if (type == null) return new string[0];
			System.Reflection.EventInfo[] events = type.GetEvents (System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			if (events == null) return new string[0];
			return type.GetEvents(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Select(e=>e.Name).OrderBy(s=>s).ToArray();
		}

		/// <summary>
		/// Gets the type names of all components on a game object.
		/// </summary>
		/// <returns>The Components on game object.</returns>
		/// <param name="go">Go.</param>
		protected string[] GetComponentsOnGameObject(GameObject go)
		{
			return go.GetComponents(typeof(Component)).Select (c=>c.GetType().Name).OrderBy(s=>s).ToArray();
		}
	}

}