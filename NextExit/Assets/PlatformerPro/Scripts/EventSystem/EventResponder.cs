using UnityEngine;
using System.Collections;
using System.Linq;

namespace PlatformerPro
{
	/// <summary>
	/// Use this component to do something when an event occurs. For example play a sound or particle effect.
	/// </summary>
	public class EventResponder : GenericResponder
	{
		/// <summary>
		/// Listen to only this game object. If its null listen to all.
		/// </summary>
		public GameObject sender;

		/// <summary>
		/// The name of the type to listen to (for example Enemy or Character).
		/// </summary>
		public string typeName;

		/// <summary>
		/// The name of the event to listen to.
		/// </summary>
		public string eventName;

		/// <summary>
		/// What to do when the event is received.
		/// </summary>
		public EventResponse[] actions;

		/// <summary>
		/// If the event is an animation change only trigger the event if this filter criteria is met.
		/// </summary>
		public AnimationState animationStateFilter;

		/// <summary>
		/// If the event is a damage or death  only trigger the event if this filter criteria is met.
		/// </summary>
		public DamageType damageTypeFilter;

		/// <summary>
		/// If the event is an button change only trigger the event if this filter criteria is met.
		/// </summary>
		public ButtonState buttonStateFilter;

		/// <summary>
		/// If the event is an attack only trigger event if attack name matches this.
		/// </summary>
		public string stringFilter;

		/// <summary>
		/// If the event is an integer event, or an item event with an amount.
		/// </summary>
		public int intFilter;

		/// <summary>
		/// If the event is an game phase change only trigger event if phase matches this. If it is any other event
		/// only apply if this == NONE or the game amanger GamePhase is equal to this.
		/// </summary>
		public GamePhase gamePhaseFilter = GamePhase.NONE;

		/// <summary>
		/// Cached event info.
		/// </summary>
		protected System.Reflection.EventInfo eventInfo;

		/// <summary>
		/// Cached handler .
		/// </summary>
		protected System.Delegate handler;

		/// <summary>
		/// Cached sending component.
		/// </summary>
		protected Component sendingComponent;

		/// <summary>
		/// When this gets to zero do the action.
		/// </summary>
		protected float handleTimer;

		protected CustomEventFilter[] customFilters;

		#region Unity hooks

		/// <summary>
		/// Unity enable hook, add event listeners.
		/// </summary>
		void OnEnable()
		{
			AddHandler();
		}

		/// <summary>
		/// Unity disable hook, remove event listeners.
		/// </summary>
		void OnDisable()
		{
			RemoveHandler();
		}

		void Start() {
			Init();
		}
		#endregion

		/// <summary>
		/// Initialise this responder.
		/// </summary>
		virtual protected void Init() {
			customFilters = GetComponents<CustomEventFilter> ();
		}

		/// <summary>
		/// Adds the event handler.
		/// </summary>
		virtual protected void AddHandler()
		{
			// We can't use AddEventHandler on AOT based platforms
#if UNITY_IPHONE || UNITY_XBOX360 || UNITY_PS3 || UNITY_PSP2 || UNITY_XBOXONE || UNITY_PS4 || UNITY_WIIU || UNITY_WEBGL
			// Used cached info
			if (eventInfo != null && handler != null && sendingComponent != null)
			{
				System.Reflection.MethodInfo handleMethod = this.GetType().GetMethod("HandleEvent", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				handler = System.Delegate.CreateDelegate(eventInfo.EventHandlerType, this, handleMethod);
				System.Reflection.MethodInfo addMethod = eventInfo.GetAddMethod();
				addMethod.Invoke(sendingComponent, new object[]{handler});
			}
			// Dynamically add event listener
			else
			{
				sendingComponent = sender.GetComponent(typeName);
				System.Type type = this.GetType().Assembly.GetType(this.GetType().Namespace + "." + typeName);
				
				if (type != null && sendingComponent != null)
				{
					eventInfo = type.GetEvent(eventName);
					
					if (eventInfo != null)
					{
						System.Reflection.MethodInfo handleMethod = this.GetType().GetMethod("HandleEvent", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
						handler = System.Delegate.CreateDelegate(eventInfo.EventHandlerType, this, handleMethod);
						System.Reflection.MethodInfo addMethod = eventInfo.GetAddMethod();
						addMethod.Invoke(sendingComponent, new object[]{handler});
					}
				}
			}
#else

			// Used cached info
			if (eventInfo != null && handler != null && sendingComponent != null)
			{
				eventInfo.AddEventHandler(sendingComponent, handler);
			}
			else
			{
				// Dynamically add event listener
				sendingComponent = sender.GetComponent(typeName);
				System.Type type = typeof(Character).Assembly.GetType ("PlatformerPro." + typeName);
				if (type == null) type = typeof(Character).Assembly.GetTypes().Where(t=>t.Name == typeName).FirstOrDefault();

				if (type != null && sendingComponent != null)
				{
					eventInfo = type.GetEvent(eventName);

					if (eventInfo != null)
					{
						System.Reflection.MethodInfo handleMethod = this.GetType().GetMethod("HandleEvent", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
						handler = System.Delegate.CreateDelegate(eventInfo.EventHandlerType, this, handleMethod);
						eventInfo.AddEventHandler(sendingComponent, handler);
					}
				}
			}
#endif
		}

		/// <summary>
		/// Removes the event handler.
		/// </summary>
		virtual protected void RemoveHandler()
		{
			// We can't use RemoveEventHandler on AOT based platforms
#if UNITY_IPHONE || UNITY_XBOX360 || UNITY_PS3 || UNITY_PSP2 || UNITY_XBOXONE || UNITY_PS4 || UNITY_WIIU|| UNITY_WEBGL
			if (eventInfo != null && handler != null && sendingComponent != null)
			{
				System.Reflection.MethodInfo handleMethod = this.GetType().GetMethod("HandleEvent", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				handler = System.Delegate.CreateDelegate(eventInfo.EventHandlerType, this, handleMethod);
				System.Reflection.MethodInfo removeMethod = eventInfo.GetRemoveMethod();
				removeMethod.Invoke(sendingComponent, new object[]{handler});
			}
#else
			// Remove listeners
			if (eventInfo != null && handler != null && sendingComponent != null)
			{
				eventInfo.RemoveEventHandler(sendingComponent, handler);
			}
#endif
		}

		/// <summary>
		/// Handles the event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		virtual protected void HandleEvent( object sender, System.EventArgs args)
		{
			for (int i = 0 ; i < actions.Length; i++)
			{
				if (ApplyFilters(actions[i], args))
				{
					if (actions[i].delay == 0.0f) DoImmediateAction (actions[i], args);
					else StartCoroutine(DoDelayedAction(actions[i], args));
				}
			}
		}

		/// <summary>
		/// Applies the filters.
		/// </summary>
		/// <returns><c>true</c>, if filtering was passed, <c>false</c> otherwise.</returns>
		/// <param name="action">Action.</param>
		/// <param name="args">Arguments.</param>
		virtual protected bool ApplyFilters(EventResponse action, System.EventArgs args)
		{
			// Apply game phase filter
			if (gamePhaseFilter != GamePhase.NONE)
			{
				if (PlatformerProGameManager.Instance.GamePhase != gamePhaseFilter) return false;
			}
			// Apply custom filters if we have them
			if (customFilters != null && customFilters.Length > 0)
			{
				// Try to get a character reference
				Character character = null;
				if (sendingComponent is Character)
				{
					character = (Character)sendingComponent;
				} else if (sendingComponent is ICharacterReference)
				{
					character = ((ICharacterReference)sendingComponent).Character;
				} else if (args is CharacterEventArgs)
				{
					character = ((CharacterEventArgs)args).Character;
				} 
				else
				{
					character = sender.GetComponent<Character> ();
				}
				for (int i = 0; i < customFilters.Length; i++)
				{
					if (!customFilters [i].Filter (character, action, args))
					{
						return false;
					}
				}
				return true;
			}
			// If there were no custom filters apply the defaults
			if (args is AnimationEventArgs)
			{
				if (animationStateFilter == AnimationState.NONE || animationStateFilter == ((AnimationEventArgs)args).State) return true;
				return false;
			}
			if (args is ExtraDamageInfoEventArgs)
			{
				if ((stringFilter == null || stringFilter == "" || stringFilter == ((ExtraDamageInfoEventArgs)args).AttackName) &&
					(damageTypeFilter == DamageType.NONE || damageTypeFilter == ((DamageInfoEventArgs)args).DamageInfo.DamageType)) return true;
				return false;
			}
			if (args is DamageInfoEventArgs)
			{
				if (damageTypeFilter == DamageType.NONE || damageTypeFilter == ((DamageInfoEventArgs)args).DamageInfo.DamageType) return true;
				return false;
			}
			if (args is ButtonEventArgs)
			{
				if (buttonStateFilter == ButtonState.ANY || buttonStateFilter == ((ButtonEventArgs)args).State) return true;
				return false;
			}
			if (args is AttackEventArgs)
			{
				if (stringFilter == null || stringFilter == "" || stringFilter == ((AttackEventArgs)args).Name) return true;
				return false;
			}
			if (args is SequenceEnemyPhaseEventArgs)
			{
				if (stringFilter == null || stringFilter == "" || stringFilter == ((SequenceEnemyPhaseEventArgs)args).PhaseName) return true;
				return false;
			}
			if (args is StateEventArgs)
			{
				if (stringFilter == null || stringFilter == "" || stringFilter == ((StateEventArgs)args).StateName) return true;
				return false;
			}
            if (args is ItemEventArgs)
			{
				if (stringFilter == null || stringFilter == "" || stringFilter == ((ItemEventArgs)args).Type)
				{
					if (intFilter == 0) return true;
					if (((ItemEventArgs)args).Amount >= intFilter) return true;
				}
				return false;
			}
			if (args is ChargeEventArgs)
			{
				if (intFilter <= 0) return true;
				if (intFilter == (Mathf.RoundToInt(((ChargeEventArgs)args).Amount))) return true;
				return false;
			}
			if (args is ActivationEventArgs)
			{
				if (stringFilter == null || stringFilter == "" || stringFilter == ((ActivationEventArgs)args).ItemId) return true;
				return false;
			}
			if (args is GamePhaseEventArgs)
			{
				if (gamePhaseFilter == ((GamePhaseEventArgs)args).Phase) return true;
				return false;
			}
			return true;
		}

		/// <summary>
		/// Do the action
		/// </summary>
		/// <param name="args">Event arguments.</param>
		/// <param name="action">Action.</param>
		override protected void DoImmediateAction(EventResponse action, System.EventArgs args)
		{
			// Override debug to add extra information
			if (action.responseType == EventResponseType.DEBUG_LOG)
			{
				string argumentsAsString = args == null ? "null" : args.ToString();
				Debug.Log (string.Format("Got event from sender {0}, arguments: {1}", sender, argumentsAsString));
			}
			else
			{
				base.DoImmediateAction(action, args);
			}
		}

	}
}