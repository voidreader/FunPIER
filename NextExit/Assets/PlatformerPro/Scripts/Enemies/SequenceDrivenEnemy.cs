using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PlatformerPro
{
	/// <summary>
	/// An enemy that has a sequence of moves which happen in order (can loop). 
	/// </summary>
	public class SequenceDrivenEnemy : Enemy, IMob
	{
		#region public members
		
		/// <summary>
		/// How far can the enemy see?
		/// </summary>
		[HideInInspector]
		public float sightDistance = 5.0f;
		
		/// <summary>
		/// Y position of the characters 'eyes'.
		/// </summary>
		[HideInInspector]
		public float sightYOffset;
		
		/// <summary>
		/// Layers to check for obstacle and characters.
		/// </summary>
		[HideInInspector]
		public LayerMask sightLayers;
		
		/// <summary>
		/// Range of the proximity sense.
		/// </summary>
		[HideInInspector]
		[Tooltip ("Range of the hearing/proximity sense. Set to 0 to ingore.")]
		public float hearingRadius = 1.0f;
		
		/// <summary>
		/// Centre point of the proximity sense.
		/// </summary>
		[HideInInspector]
		[Tooltip ("Centre point of the hearing/proximity sense.")]
		public Vector2 hearingOffset;

		/// <summary>
		/// Layers to check for proximity sense.
		/// </summary>
		[HideInInspector]
		public LayerMask hearingLayers;

		/// <summary>
		/// LDistance at which the target is "lost".
		/// </summary>
		[HideInInspector]
		public float maxTargetDistance;

		/// <summary>
		/// List of the states in the sequence.
		/// </summary>
		[HideInInspector]
		public List<EnemyPhase> phaseInfo;

		/// <summary>
		/// List of GOTO conditions that are evaluated every loop.
		/// </summary>
		public List<EnemyStateInfo> globalGotos;

		#endregion
		
		
		#region protected members
		
		/// <summary>
		/// The currently active phase.
		/// </summary>
		protected int currentPhase;
		
		/// <summary>
		/// The currently active state.
		/// </summary>
		protected int currentState;
		
		/// <summary>
		/// Stores how long we have been in current phase. If the same state is played multiple times this tracks the time 
		/// in the CURRENT iteration.
		/// </summary>
		protected float phaseTimer;
		
		/// <summary>
		/// Tracks how many times we have played the current phase.
		/// </summary>
		protected int phaseLoopCount;
		
		/// <summary>
		/// Stores the number of times we have been damaged while in current phase.
		/// </summary>
		protected int phaseHitCount;
		
		/// <summary>
		/// Stores how long we have been in current state. If the same state is played multiple times this tracks the time 
		/// in the CURRENT iteration.
		/// </summary>
		protected float stateTimer;
		
		/// <summary>
		/// Stores the completion state of current move.
		/// </summary>
		protected bool stateMovementComplete;
		
		/// <summary>
		/// Stores the number of times we have been damaged while in current state.
		/// </summary>
		protected int stateHitCount;
		
		/// <summary>
		/// Tracks how many times we have changed state in a frame, if its too many this is likely a loop.
		/// </summary>
		protected int infiniteLoopCheck;

		/// <summary>
		/// Used to store results of hearing overlap call.
		/// </summary>
		protected Collider2D[] proximityColliders;

		/// <summary>
		/// Tracks if exit condition for state has been met.
		/// </summary>
		protected bool willExitState;

		/// <summary>
		/// Tracks if exit condition for phase has been met.
		/// </summary>
		protected bool willExitPhase;

		/// <summary>
		/// Track if we have run the check for player call so we don't eat resources by running it more than once a frame.
		/// </summary>
		protected bool hasRunCheckForCharacter;

		/// <summary>
		/// Result of the last check for character.
		/// </summary>
		protected bool checkForCharacterResult;

		/// <summary>
		/// Cached character health for current target.
		/// </summary>
		protected CharacterHealth currentTargetHealth;

		/// <summary>
		/// Capture starting health in case we need to calcualte based on health percentage.
		/// </summary>
		protected float startingHealth;

		#endregion
		
		
		#region constants

		/// <summary>
		/// Maximum number of state change sin a frame.
		/// </summary>
		protected const int MaxStateChanges = 20;

		/// <summary>
		/// The max colliders for statically allocated colliders.
		/// </summary>
		private const int MaxColliders = 8;

		#endregion
		
		
		#region events
		
		/// <summary>
		/// Event for anmation state changes.
		/// </summary>
		public event System.EventHandler <SequenceEnemyPhaseEventArgs> PhaseEnter;
		
		/// <summary>
		/// Event for damage.
		/// </summary>
		public event System.EventHandler <SequenceEnemyPhaseEventArgs> PhaseExit;

		
		/// <summary>
		/// Event for anmation state changes.
		/// </summary>
		public event System.EventHandler <StateEventArgs> StateEnter;
		
		/// <summary>
		/// Event for state exit.
		/// </summary>
		public event System.EventHandler <StateEventArgs> StateExit;

		/// <summary>
		/// Raises the phase enter event.
		/// </summary>
		virtual protected void OnPhaseEnter()
		{
			if (PhaseEnter != null) PhaseEnter (this, new SequenceEnemyPhaseEventArgs (phaseInfo[currentPhase].name));
		}

		/// <summary>
		/// Raises the phase exit event.
		/// </summary>
		virtual protected void OnPhaseExit()
		{
			if (PhaseExit != null) PhaseExit (this, new SequenceEnemyPhaseEventArgs (phaseInfo[currentPhase].name));
		}

		/// <summary>
		/// Raises the state enter event.
		/// </summary>
		virtual protected void OnStateEnter()
		{
			if (StateEnter != null) StateEnter (this, new StateEventArgs (phaseInfo[currentPhase].stateInfo[currentState].stateName));
		}

		/// <summary>
		/// Raises the state exit event.
		/// </summary>
		virtual protected void OnStateExit()
		{
			if (StateExit != null) StateExit (this, new StateEventArgs (phaseInfo[currentPhase].stateInfo[currentState].stateName));
		}

		#endregion
		
		
		#region properties
		
		/// <summary>
		/// Gets the index of the current phase.
		/// </summary>
		public int CurrentPhase
		{
			get { return currentPhase; }
		}
		
		/// <summary>
		/// Gets the index of the current state.
		/// </summary>
		public int CurrentState
		{
			get { return currentState; }
		}

		#endregion
		
		
		#region unity hooks

#if UNITY_EDITOR
		/// <summary>
		/// Draw gizmos for showing sight range.
		/// </summary>
		virtual public void OnDrawGizmos()
		{
			Vector3 offset = new Vector3 (0, sightYOffset, 0);
			float arrowOffset = 0.25f;
			float actualSightDistance = sightDistance;
			int offsetModifier = 1;
			if (gameObject.GetComponentInParent<Enemy>().FacingDirection < 0)
			{
				arrowOffset  *= -1;
				actualSightDistance *= -1;
				offsetModifier = -1;
			}
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position + offset,  transform.position + offset + new Vector3(actualSightDistance, 0, 0));
			Gizmos.DrawLine(transform.position + offset + new Vector3(actualSightDistance, 0.0f, 0), transform.position + offset + new Vector3(actualSightDistance - arrowOffset, 0.25f, 0));
			Gizmos.DrawLine(transform.position + offset + new Vector3(actualSightDistance, 0.0f, 0), transform.position + offset + new Vector3(actualSightDistance  - arrowOffset, -0.25f, 0));

			Handles.color = new Color (1, 0, 0, 0.25f);
			Handles.DrawSolidDisc (transform.position + new Vector3 (hearingOffset.x * offsetModifier, hearingOffset.y, 0), new Vector3 (0, 0, 1), hearingRadius);
			
			Gizmos.color = Color.white;
			Handles.color = Color.white;
		}
#endif

		#endregion
		
		
		#region protected methods
		
		/// <summary>
		/// Set up the character
		/// </summary>
		override protected void PostInit()
		{
			base.PostInit ();

			// Cache health
			startingHealth = health;

			// Initialise hearing colliders
			proximityColliders = new Collider2D[MaxColliders];

			// Check for errors
			if (phaseInfo == null || phaseInfo.Count < 1)
			{
				Debug.LogWarning("A sequence driven enemy must have at least one phase.");
				gameObject.SetActive(false);
				return;
			}
			for (int i = 0; i < phaseInfo.Count; i++)
			{
				// Check for errors
				if (phaseInfo[i].stateInfo == null || phaseInfo[i].stateInfo.Count < 1)
				{
					Debug.LogWarning("A phase must have at least one state.");
					gameObject.SetActive(false);
					return;
				}
				for (int j = 0; j < phaseInfo[i].stateInfo.Count; j++)
				{
					// No movement
					if (phaseInfo[i].stateInfo[j].assignedMovement == null && phaseInfo[i].stateInfo[j].gotoState < 0)
					{
						Debug.LogWarning("The state at position " + j + " in phase " + i + " has no movement assigned");
						gameObject.SetActive(false);
						return;
					}
					else if (phaseInfo[i].stateInfo[j].gotoPhase == i && phaseInfo[i].stateInfo[j].gotoState == j)
					{
						Debug.LogWarning("A GOTO state is pointing to itself!");
						gameObject.SetActive(false);
						return;
					}
				}
			}
			
			// Init all movements
			for (int i = 0; i < phaseInfo.Count; i++)
			{
				for (int j = 0; j < phaseInfo[i].stateInfo.Count; j++)
				{
					if (phaseInfo[i].stateInfo[j].assignedMovement != null)phaseInfo[i].stateInfo[j].assignedMovement.Init (this);
				}
			}
			
			// Set current movement
			currentPhase = 0;
			currentState = 0;
			phaseInfo[currentPhase].stateInfo [currentState].assignedMovement.GainingControl();
			movement = phaseInfo[currentPhase].stateInfo [currentState].assignedMovement;
			OnPhaseEnter ();
			OnStateEnter ();
		}
		
		/// <summary>
		/// Run each frame to determine and execute move.
		/// </summary>
		override protected void DoUpdate()
		{
			hasRunCheckForCharacter = false;
			base.DoUpdate ();
			infiniteLoopCheck = 0;
			stateTimer += TimeManager.FrameTime;
			phaseTimer += TimeManager.FrameTime;
		}
		
		/// <summary>
		/// Decides on the next movement. Here we use the state sequence.
		/// </summary>
		override protected void DecideOnNextMovement()
		{
			// If DEAD do nothing
			if (State == EnemyState.DEAD) return;
			// If DAMAGED do nothing
			if (State == EnemyState.DAMAGED) return;
		
			// Check for phase exit
			if (willExitPhase || CheckPhaseExitCondition (phaseInfo [currentPhase]))
			{
				willExitPhase = true;
				if (!CurrentMovement.LosingControl())
				{
					ChangePhase(currentPhase + 1, 0);
				}
				return;
			}
			
			// Should we exit this state
			if (willExitState || CheckStateExitCondition(phaseInfo[currentPhase].stateInfo[currentState]))
			{
				willExitState = true;
				if (!CurrentMovement.LosingControl())
				{
					// Advance to next state
					ChangeState(currentState + 1);
				}
			}

			// Check for phase exit again 
			if (willExitPhase || CheckPhaseExitCondition (phaseInfo [currentPhase]))
			{
				willExitPhase = true;
				if (!CurrentMovement.LosingControl())
				{
					ChangePhase(currentPhase + 1, 0);
				}
				return;
			}
			
		}
		
		/// <summary>
		/// Checks if the states exit condition has been met.
		/// </summary>
		/// <returns><c>true</c>, if exit condition was checked, <c>false</c> otherwise.</returns>
		/// <param name="enemy">Enemy.</param>
		/// <param name="stateTImer">Time we have been in this state.</param>
		virtual protected bool CheckStateExitCondition(EnemyStateInfo currentState)
		{
			switch (currentState.exitType)
			{
			case EnemyStateExitType.TIMER:
				if (stateTimer >= currentState.exitSupportingData) return true;
				break;
			case EnemyStateExitType.MOVE_COMPLETE:
				if (stateMovementComplete)
				{
					return true;
				}
				break;
			case EnemyStateExitType.TIMER_PLUS_RANDOM:
				if (stateTimer >= currentState.exitSupportingData)
				{
					stateTimer = 0;
					if (Random.Range (0, 100) < (int)currentState.exitSupportingDataAlt) return true;
				}
				break;
			case EnemyStateExitType.NUMBER_OF_HITS:
				if (stateHitCount >= (int)currentState.exitSupportingData)
				{
					return true;
				}
				break;
			case EnemyStateExitType.HEALTH_PERCENTAGE:
			 	if ((health / startingHealth) <= currentState.exitSupportingData) 
				{
					return true;
				}
				break;
			case EnemyStateExitType.SENSE_PLAYER:
				if (CheckForCharacter()) return true;
				break;
			case EnemyStateExitType.LOST_PLAYER_TARGET:
				CheckForCharacter();
				if (CurrentTarget == null) return true;
				break;
			case EnemyStateExitType.TARGET_WITHIN_RANGE:
				if ((currentState.exitSupportingDataAlt == 0 || See()) &&
				    CurrentTarget != null && 
				    ((currentState.exitSupportingData > 0 && Vector2.Distance(CurrentTargetTransform.position, transform.position) <= currentState.exitSupportingData)
				 	||
				 	 (currentState.exitSupportingData < 0 && Vector2.Distance(CurrentTargetTransform.position, transform.position) >= -currentState.exitSupportingData)))
				{
					return true;
				}
				break;
			case EnemyStateExitType.ALWAYS:
				return true;
			case EnemyStateExitType.NONE:
				break;
			default:
				Debug.Log ("Exit type not yet implemented: " + currentState.exitType);
				break;
			}
			return false;
		}
		
		/// <summary>
		/// Checks if the phase exit condition has been met.
		/// </summary>
		/// <returns><c>true</c>, if exit condition was checked, <c>false</c> otherwise.</returns>
		virtual protected bool CheckPhaseExitCondition(EnemyPhase currentPhase)
		{
			switch (currentPhase.exitType)
			{
			case EnemyPhaseExitType.TIMER:
				if (phaseTimer >= currentPhase.exitSupportingData) return true;
				break;
			case EnemyPhaseExitType.NUMBER_OF_LOOPS:
				if (phaseLoopCount >= (int)currentPhase.exitSupportingData)
				{
					return true;
				}
				break;
			case EnemyPhaseExitType.NUMBER_OF_HITS:
				if (phaseHitCount >= (int)currentPhase.exitSupportingData)
				{
					return true;
				}
				break;
			case EnemyPhaseExitType.HEALTH_PERCENTAGE:
				if ((health / startingHealth) <= currentPhase.exitSupportingData) 
				{
					return true;
				}
				break;
			case EnemyPhaseExitType.TIMER_PLUS_RANDOM:
				if (phaseTimer >= currentPhase.exitSupportingData)
				{
					phaseTimer = 0;
					if (Random.Range (0, 100) < (int)currentPhase.exitSupportingDataAlt) return true;
				}
				break;
			case EnemyPhaseExitType.SENSE_PLAYER:
				if (CheckForCharacter()) return true;
				break;
			case EnemyPhaseExitType.LOST_PLAYER_TARGET:
				if (CurrentTarget == null) return true;
				break;
			case EnemyPhaseExitType.TARGET_WITHIN_RANGE:
				if ((currentPhase.exitSupportingDataAlt == 0 || See()) &&
				    CurrentTarget != null && 
				    ((currentPhase.exitSupportingData > 0 && Vector2.Distance(CurrentTargetTransform.position, transform.position) <= currentPhase.exitSupportingData)
				    ||
				     (currentPhase.exitSupportingData < 0 && Vector2.Distance(CurrentTargetTransform.position, transform.position) >= -currentPhase.exitSupportingData)))

				{
					return true;
				}
				break;
			case EnemyPhaseExitType.NONE:
				break;
			default:
				Debug.Log ("Exit type not yet implemented: " + currentPhase.exitType);
				break;
			}
			return false;
		}
		
		/// <summary>
		/// Changes the state.
		/// </summary>
		/// <param name="newState">New state index.</param>
		virtual protected void ChangeState(int newState)
		{
			// Check for infinite loops
			infiniteLoopCheck++;
			if (infiniteLoopCheck > MaxStateChanges)
			{
				if (gameObject.activeSelf) Debug.LogWarning("Infinite loop detected in sequence. Aborting");
				gameObject.SetActive(false);
				return;
			}
			
			if (newState < 0)
			{
				Debug.LogWarning("Tried to advance to a state that did not exist");
				willExitState = false;
				return;
			}
			else if (newState >= phaseInfo[currentPhase].stateInfo.Count)
			{
				newState = 0;
				phaseLoopCount++;
			}

			OnStateExit ();
			stateHitCount = 0;
			currentState = newState;
			stateTimer = 0;
			stateMovementComplete = false;
			;
			willExitState = false;

			// If this is a GOTO do the GOTO
			if (phaseInfo[currentPhase].stateInfo[currentState].gotoState >= 0)
			{
				if (CheckStateExitCondition(phaseInfo[currentPhase].stateInfo[currentState]))
				{
					ChangePhase(phaseInfo[currentPhase].stateInfo[currentState].gotoPhase, phaseInfo[currentPhase].stateInfo[currentState].gotoState);
				}
				else
				{
					ChangeState(currentState + 1);
				}
			}
			// Else set the movement
			else
			{
				phaseInfo[currentPhase].stateInfo[currentState].assignedMovement.GainingControl();
				movement = phaseInfo[currentPhase].stateInfo[currentState].assignedMovement;

				OnStateEnter ();
			}


		}
		
		/// <summary>
		/// Changes the phase.
		/// </summary>
		/// <param name="newPhase">New phase.</param>
		/// <param name="newState">New state.</param>
		virtual protected void ChangePhase(int newPhase, int newState)
		{
			if (newPhase < 0 || newPhase >= phaseInfo.Count)
			{
				Debug.LogWarning("Tried to advance to a phase that did not exist");
				return;
			}

			OnPhaseExit ();

			currentPhase = newPhase;
			phaseTimer = 0;
			phaseHitCount = 0;
			stateHitCount = 0;
			phaseLoopCount = 0;
			willExitPhase = false;
			willExitState = false;

			// We set this in case below calls fails at least we wont completely bomb
			currentState = 0;
			
			ChangeState (newState);

			OnPhaseEnter ();
		}

		/// <summary>
		/// Try to find a character by looking and listening.
		/// </summary>
		/// <returns><c>true</c>, if for character was checked, <c>false</c> otherwise.</returns>
		virtual protected bool CheckForCharacter()
		{
			if (hasRunCheckForCharacter) return checkForCharacterResult;
			hasRunCheckForCharacter = true;

			// If we have a target check if we have lost it
			if (currentTarget != null)
			{
				if (Vector2.Distance(currentTarget.transform.position, transform.position) > maxTargetDistance)
				{
					currentTarget = null;
					currentTargetHealth = null;
					checkForCharacterResult = false;
					return false;
				}
				checkForCharacterResult = true;
				return true;
			}
			if (See ()) 
			{
				checkForCharacterResult = true;
				return true;
			}
			if (Hear ())
			{
				checkForCharacterResult = true;
				return true;
			}
			checkForCharacterResult = false;
			return false;
		}

		/// <summary>
		/// Look for a character.
		/// </summary>
		virtual protected bool See()
		{
			Character character = null;
			ICharacterReference characterRef = null;
			if (sightDistance > 0.0f)
			{
				Vector3 offset = new Vector3 (0, sightYOffset, 0);
				RaycastHit2D hit = Physics2D.Raycast(myTransform.position + offset, new Vector3(LastFacedDirection, 0, 0), sightDistance, sightLayers);
				if (hit.collider != null)
				{
					characterRef = (ICharacterReference) hit.collider.gameObject.GetComponent(typeof(ICharacterReference));
					if (characterRef == null)
					{
						character = hit.collider.gameObject.GetComponent<Character>();
					} 
					else
					{
						character = characterRef.Character;
					}
					if (character != null)
					{
						currentTarget = character;
						currentTargetHealth = character.GetComponent<CharacterHealth>();
						return true;
					}
				}
			} 
			return false;
		}

		/// <summary>
		/// Listen for a character.
		/// </summary>
		virtual protected bool Hear()
		{
			Character character = null;
			ICharacterReference characterRef = null;
			if (hearingRadius > 0.0f)
			{
				Vector2 offset = (Vector2)transform.position + (FacingDirection == -1 ? new Vector2(-hearingOffset.x, hearingOffset.y) : hearingOffset);
				int hits = Physics2D.OverlapCircleNonAlloc (offset, hearingRadius, proximityColliders, hearingLayers);
				if (hits > 0)
				{
					// Always pick first target in list
					characterRef = (ICharacterReference) proximityColliders[0].gameObject.GetComponent(typeof(ICharacterReference));
					if (characterRef == null)
					{
						character = proximityColliders[0].gameObject.GetComponent<Character>();
					} 
					else
					{
						character = characterRef.Character;
					}
					currentTarget = character;
					currentTargetHealth = character.GetComponent<CharacterHealth>();
					return true;
				}
			}
			return false;
		}

		#endregion
		
		#region public methods
		
		/// <summary>
		/// Damage the enemy with the specified damage information.
		/// </summary>
		/// <param name="info">The damage info.</param>
		override public void Damage(DamageInfo info)
		{
			base.Damage (info);
			if (phaseInfo [currentPhase].exitSupportingDamageType == DamageType.NONE ||
				phaseInfo [currentPhase].exitSupportingDamageType == info.DamageType)
			{
				phaseHitCount++;
			}
			if (phaseInfo [currentPhase].stateInfo[currentState].exitSupportingDamageType == DamageType.NONE ||
			    phaseInfo [currentPhase].stateInfo[currentState].exitSupportingDamageType == info.DamageType)
			{
				stateHitCount++;
			}

			// Consider all movements complete when damaged
			stateMovementComplete = true;
		}

		/// <summary>
		/// Called when the enemies current attack finished
		/// </summary>
		override public void MovementComplete()
		{
			stateMovementComplete = true;
		}

		#endregion
	}
	
	[System.Serializable]
	public class EnemyPhase
	{
		/// <summary>
		/// Name of this phase.
		/// </summary>
		public string name;
		
		/// <summary>
		/// List of states in this phase.
		/// </summary>
		public List<EnemyStateInfo> stateInfo;
		
		/// <summary>
		/// How we exit this phase.
		/// </summary>
		public EnemyPhaseExitType exitType;
		
		/// <summary>
		/// The exit supporting data (for example timer).
		/// </summary>
		public float exitSupportingData;
		
		/// <summary>
		/// The exit supporting data (for example random percentage).
		/// </summary>
		public float exitSupportingDataAlt;

		/// <summary>
		/// The exit supporting damage type (for number of hits filter).
		/// </summary>
		public DamageType exitSupportingDamageType;

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.EnemyPhase"/> class.
		/// </summary>
		public EnemyPhase()
		{
			stateInfo = new List<EnemyStateInfo> ();
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.EnemyPhase"/> class.
		/// </summary>
		public EnemyPhase(string name): this()
		{
			this.name = name;
		}
		
	}
	
	[System.Serializable]
	public class EnemyStateInfo
	{
		/// <summary>
		/// If this is non null instead of being a state this is a goto statement which goes to the phase
		/// at the indicated position. The GOTO command is executed until the EXIT condition is true.
		/// </summary>
		public int gotoPhase;
		
		/// <summary>
		/// If this is non null instead of being a state this is a goto statement which goes to the state
		/// at the indicated position. The GOTO command is executed until the EXIT condition is true.
		/// </summary>
		public int gotoState;
		
		/// <summary>
		/// The name of the state shown in editor.
		/// </summary>
		public string stateName;
		
		/// <summary>
		/// The movement to use for the state.
		/// </summary>
		public EnemyMovement assignedMovement;
		
		/// <summary>
		/// How we exit this state.
		/// </summary>
		public EnemyStateExitType exitType;
		
		/// <summary>
		/// The exit supporting data (for example timer).
		/// </summary>
		public float exitSupportingData;
		
		/// <summary>
		/// The exit supporting data (for example random percentage).
		/// </summary>
		public float exitSupportingDataAlt;

		/// <summary>
		/// The exit supporting damage type (for number of hits filter).
		/// </summary>
		public DamageType exitSupportingDamageType;

	}
	
	/// <summary>
	/// Ways in which an enemy state can be exited.
	/// </summary>
	public enum EnemyStateExitType
	{
		TIMER,
		MOVE_COMPLETE,
		TIMER_PLUS_RANDOM,
		NUMBER_OF_HITS,
		HEALTH_PERCENTAGE,
		SENSE_PLAYER,
		LOST_PLAYER_TARGET,
		TARGET_WITHIN_RANGE,
		NONE,
		ALWAYS
	}
	
	/// <summary>
	/// Ways in which an enemy phase can be exited.
	/// </summary>
	public enum EnemyPhaseExitType
	{
		TIMER,
		NUMBER_OF_LOOPS,
		TIMER_PLUS_RANDOM,
		NUMBER_OF_HITS,
		HEALTH_PERCENTAGE,
		SENSE_PLAYER,
		LOST_PLAYER_TARGET,
		TARGET_WITHIN_RANGE,
		NONE
	}
	
}