/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

// Defines to toggle various features, comment or uncomment as desired
// Set some AOT compiler hints to help with AOT builds (e.g. PS Vita)
// #define AOT_COMPILER_HINTS			
// Disable named properties, disabling them slighlty improves startup peformance of characters.
// #define DISABLE_NAMED_PROPERTIES		

using UnityEngine;
#if !UNITY_4_6 && !UNITY_5_1 && !UNITY_5_2
using UnityEngine.SceneManagement;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro
{
	/// <summary>
	/// The root class for a character. This class ties together movement,
	/// animation, and other behaviours.
	/// </summary>
	[RequireComponent (typeof(Gravity))]
	[RequireComponent (typeof(BaseCollisions))]
	public class Character : MonoBehaviour, IMob
	{

#if UNITY_EDITOR
		const string DOC_URL = "https://jnamobile.zendesk.com/hc/en-gb/categories/200246030-Platformer-PRO-Documentation";

		[ContextMenu ("Character Documentation")]
		void CharacterDocumentation () {
			Application.OpenURL (DOC_URL);
		}
#endif

		#region public members
		
		/// <summary>
		/// How much extra we cast beyond the collider length when checking
		/// feet colliders. We do this so we can (for example) detect a
		/// landing state.
		/// </summary>
		public float feetLookAhead = DefaultFeetLookAhead;
		
		/// <summary>
		/// How much extra we cast beyond the collider length when checking
		/// side colliders. We do this so we can (for example) detect a
		/// wall.
		/// </summary>
		public float sideLookAhead = DefaultSideLookAhead;
		
		/// <summary>
		/// How much extra we cast beyond the collider length when checking
		/// feet colliders. We do this so we can (for example) detect a
		/// landing state.
		/// </summary>
		public float groundedLookAhead = DefaultGroundedLookAhead;
		
		/// <summary>
		/// All layers that we collide with. Note that just because something is in this group it doesn't mean
		/// we do anything about collisions with it. For example this mask includes layers like ladders and triggers 
		/// which are ignored when responding to collisions.
		/// </summary>
		/// <seealso cref="Character.geometryLayerMask"/>
		public LayerMask layerMask = 1;
		
		/// <summary>
		/// The layers that are considered when working out collisions. i.e. the things we can't move through. Layers in this mask must 
		/// appear in the layerMask.
		/// </summary>
		/// <seealso cref="Character.layerMask"/>
		public LayerMask geometryLayerMask = 1;
		
		/// <summary>
		/// The layers that the character can walk on top of but can passthrough in other directions.
		/// </summary>
		/// <seealso cref="Character.layerMask"/>
		public LayerMask passthroughLayerMask;
		
		/// <summary>
		/// Usually set this very low as it gives smoother movement while falling. Can be an issue
		/// if you have platforms that move faster downwards than this speed.
		/// </summary>
		public float fallSpeedForIgnoreHeadCollisions = DefaultFallSpeedForIgnoreHeadCollisions;
		
		/// <summary>
		/// How long do we need to wait before we change the state to falling. This stops minor ground
		/// preturbations causing us to trigger the fall animation.
		/// </summary>
		public float timeForFallState = 0.1f;
		
		/// <summary>
		/// Maximum fall speed (note that any individual movement may override this if it wants to).
		/// </summary>
		[Range (-35, -1)]
		public float terminalVelocity = DefaultTerminalVelocity;

		/// <summary>
		/// Should we Calcualte slope angles for use by movements?
		/// </summary>
		public bool calculateSlopes = true;

		/// <summary>
		/// Should we allow the character to walk on slopes?
		/// </summary>
		public bool rotateToSlopes = false;
		
		/// <summary>
		/// How far can the character rotate in degrees.
		/// </summary>
		[Range (0, 180)]
		public float maxSlopeRotation = DefaultMaxSlopeRotation;
		
		/// <summary>
		/// How fast the character rotates to match a slope. Measured in degrees per second.
		/// </summary>
		[Range (120, 1080)]
		public float rotationSpeed = DefaultRotationSpeed;
		
		/// <summary>
		/// The minimum angle at which a wall needs to be at for it to be considered a "side".
		/// </summary>
		[Range (30, 90)]
		public float sideAngle = DefaultSideAngle;
		
		/// <summary>
		/// How long after leaving the ground are we still considered grounded?
		/// </summary>
		[Range (0.01f, 1)]
		public float groundedLeeway = DefaultGroundedLeeway;

		/// <summary>
		/// At what point will a passthrough collider consider itself groudned. Too small and you fall through
		/// platforms. Too large and you snap oddly. Usually the default is okay unless you change gravity dramatically.
		/// </summary>
		[Range (-2.0f, 0.0f)]
		public float passthroughLeeway = DefaultPassthroughLeeway;

		/// <summary>
		/// Should we switch left and right colliders when the character changes direction (used
		/// for asymmetric characters).
		/// </summary>
		public bool switchCollidersOnDirectionChange;

		/// <summary>
		/// The type of slope rotation.
		/// </summary>
		public CharacterRotationType characterRotationType;

		#endregion
		
		#region protected members (serialized)
		
		/// <summary>
		/// The feet colliders.
		/// </summary>
		public BasicRaycast[] feetColliders;
		
		/// <summary>
		/// The left side colliders.
		/// </summary>
		public BasicRaycast[] leftColliders;
		
		/// <summary>
		/// The right side colliders.
		/// </summary>
		public BasicRaycast[] rightColliders;
		
		/// <summary>
		/// The head colliders.
		/// </summary>
		public BasicRaycast[] headColliders;
		
		/// <summary>
		/// Should we use a tag to detect ladders, default will use layer.
		/// </summary>
		[SerializeField]
		protected bool detectLaddersByTag;
		
		/// <summary>
		/// Layers or tag name used for ladder detection.
		/// </summary>
		/// <value>The transform.</value>
		[SerializeField]
		protected string ladderLayerOrTagName;
		
		/// <summary>
		/// Cached copy of the input.
		/// </summary>
		[SerializeField]
		protected Input input;

		/// <summary>
		/// Detect all walls for use by wall movements?
		/// </summary>
		[SerializeField]
		protected bool detectAllWalls;

		/// <summary>
		/// Detect walls by tag or layer?
		/// </summary>
		[SerializeField]
		protected bool detectWallsByTag;

		/// <summary>
		/// Layer or tag name for wall detection.
		/// </summary>
		[SerializeField]
		protected string wallLayerOrTagName;

		#endregion
		
		#region protected members 
		
		/// <summary>
		/// Cached copy of the characters transform.
		/// </summary>
		protected Transform myTransform;
		
		/// <summary>
		/// Cached reference to the base movement.
		/// </summary>
		protected BaseCollisions baseMovement;
		
		/// <summary>
		/// Cached references to all the movements.
		/// </summary>
		protected Movement[] movements;
		
		/// <summary>
		/// The index of the currently active movement in the movements
		/// array.
		/// </summary>
		protected int activeMovement;

		/// <summary>
		/// The active attack.
		/// </summary>
		protected int activeAttack;

		/// <summary>
		/// The index of the default ground movement in the movements
		/// array.
		/// </summary>
		protected int defaultGroundMovement;
		
		/// <summary>
		/// The index of the default air movement in the movements
		/// array.
		/// </summary>
		protected int defaultAirMovement;
		
		/// <summary>
		/// The index of the default ladder movement in the movements
		/// array. -1 for no ladder movement.
		/// </summary>
		protected int defaultLadderMovement;
		
		/// <summary>
		/// The index of the default wall movement in the movements
		/// array. -1 for no wall movement.
		/// </summary>
		protected int defaultWallMovement;
		
		/// <summary>
		/// The index of the default damage movement in the movements
		/// array. -1 for no damage movement.
		/// </summary>
		protected int defaultDamageMovement;
		
		/// <summary>
		/// The index of the default death movement in the movements
		/// array. -1 for no death movement.
		/// </summary>
		protected int defaultDeathMovement;
		
		/// <summary>
		/// The index of the default attack movement.
		/// </summary>
		protected int defaultAttackMovement;
		
		/// <summary>
		/// The class that does gravity movement.
		/// </summary>
		protected Gravity gravity;
		
		/// <summary>
		/// The current ladder collider or null if we don't have a ladder.
		/// </summary>
		protected Collider2D currentLadder;
		
		/// <summary>
		/// The current wall collider or null if we don't have a wall.
		/// </summary>
		protected Collider2D currentWall;
		
		/// <summary>
		/// The current wall collider direction. 1 for right, -1 for left, 0 for none.
		/// </summary>
		protected int currentWallCollider;
		
		/// <summary>
		/// The caaracters current animation state.
		/// </summary>
		protected AnimationState animationState;
		
		/// <summary>
		/// The parent platform.
		/// </summary>
		protected Platform parentPlatform;

		/// <summary>
		/// The transform of th parent platform.
		/// </summary>
		protected Transform parentTransform;

		/// <summary>
		/// Tracks the raycast type we are using for parent calculation.
		/// </summary>
		protected RaycastType parentRaycastType;

		/// <summary>
		/// Cached reference to all colliders. Some funciton expect this to be ordered with feet colliders left to right at the start.
		/// </summary>
		protected BasicRaycast[] colliders;
		
		/// <summary>
		/// Keeps track of the closest collisions.
		/// </summary>
		protected int[] closestColliders;
		
		/// <summary>
		/// If slopes are on this is the rotation we are rotating towards.
		/// </summary>
		protected float slope;
		
		/// <summary>
		/// Position of the feet collider in the feet colliders array that we should rotate around.
		/// </summary>
		protected int rotationPoint;
		
		/// <summary>
		/// Current animations animation priority.
		/// </summary>
		protected int animationPriority;
		
		/// <summary>
		/// Current animation override.
		/// </summary>
		protected string overrideState;
		
		/// <summary>
		/// The forced  animation state.
		/// </summary>
		protected AnimationState forcedAnimationState;
		
		/// <summary>
		/// The time remaining to play the forced animation.
		/// </summary>
		protected float forcedAnimationTime;
		
		/// <summary>
		/// A cached animation event args that we update so we don't need to allocate.
		/// </summary>
		protected AnimationEventArgs animationEventArgs;

		/// <summary>
		/// The cached attack event args.
		/// </summary>
		protected AttackEventArgs attackEventArgs;

		/// <summary>
		/// The angle of the ignored slope.
		/// </summary>
		protected float ignoredSlope;
		
		/// <summary>
		/// The direction currently faced. Used for switch collider calculations.
		/// </summary>
		protected int currentFacingDirection;
		
		/// <summary>
		/// Local copy of grounded state.
		/// </summary>
		protected bool grounded;
		
		/// <summary>
		/// A list of colliders that can be used for wall cling (i.e. only those high enough).
		/// </summary>
		protected List<BasicRaycast> collidersToUseForWallCling;
		
		/// <summary>
		/// Cached conversion of ladder layer to layer int.
		/// </summary>
		protected int ladderLayerNumber = -1;

		/// <summary>
		/// Cached conversion of wall layer to layer int.
		/// </summary>
		protected int wallLayerNumber = -1;

		/// <summary>
		/// The previous frames target rotation.
		/// </summary>
		protected float previousRotation;

		/// <summary>
		/// The minimum required colliders from the various Wall movements.
		/// </summary>
		protected int minimumWallColliders;

		/// <summary>
		/// Cached empty ray cast hit.
		/// </summary>
		protected RaycastHit2D EmptyRaycastHit = new RaycastHit2D();

		/// <summary>
		/// Dictionary of names properties to field info.
		/// </summary>
		protected Dictionary <string, List<System.Reflection.FieldInfo>> namedProperties;

		/// <summary>
		/// A list of colliders to ingore during collisions.
		/// </summary>
		protected HashSet<Collider2D> ignoredColliders;
	
		/// <summary>
		/// Reference to the disabled input. Used when InputEnabled is set to false.
		/// </summary>
		protected DisabledInput disabledInput;

		/// <summary>
		/// Cached input to the main input used when re-enabling input.
		/// </summary>
		protected Input cachedInput;

#endregion

		/// <summary>
		/// Delegate for setting a named property.
		/// </summary>
		public delegate void NamedPropertySetter(object value);
		
		#region defaults
		
		/// <summary>
		/// The default feet look ahead.
		/// </summary>
		public const float DefaultFeetLookAhead = 0.2f;
		
		/// <summary>
		/// The default grounded look ahead.
		/// </summary>
		public const float DefaultGroundedLookAhead = 0.05f;
		
		/// <summary>
		/// The default time after leaving ground for which the character is still conisdered to be on the ground.
		/// </summary>
		public const float DefaultGroundedLeeway = 0.1f;
		
		/// <summary>
		/// The default side look ahead.
		/// </summary>
		public const float DefaultSideLookAhead = 0.1f;

		/// <summary>
		/// Deafult passthrough leeway.
		/// </summary>
		public const float DefaultPassthroughLeeway = -0.25f;

		/// <summary>
		/// The default fall speed at which head collisions are ignored.
		/// </summary>
		public const float DefaultFallSpeedForIgnoreHeadCollisions = 0.2f;
		
		/// <summary>
		/// Defauly terminal velocity
		/// </summary>
		public const float DefaultTerminalVelocity = -15.0f;
		
		/// <summary>
		/// The default max slope rotation.
		/// </summary>
		public const float  DefaultMaxSlopeRotation = 50;
		
		/// <summary>
		/// The default rotation speed.
		/// </summary>
		public const float DefaultRotationSpeed = 180;
		
		/// <summary>
		/// The default side angle.
		/// </summary>
		public const float DefaultSideAngle = 50;
		
		
		#endregion
		
		#region properties
		
		/// <summary>
		/// Gets cache copy of the transform.
		/// </summary>
		virtual public Transform Transform
		{
			get
			{
				return myTransform;
			}
		}
		
		/// <summary>
		/// Gets or sets the colliders.
		/// </summary>
		/// <value>The colliders.</value>
		virtual public BasicRaycast[] Colliders
		{
			get
			{
				if (colliders == null || colliders.Length == 0)
				{
					colliders = new BasicRaycast[feetColliders.Length + leftColliders.Length + rightColliders.Length +  headColliders.Length];
					// Sort feet colliders left to right
					System.Array.Copy(feetColliders.OrderBy(f=>f.Extent.x).ToArray(), colliders, feetColliders.Length);
					System.Array.Copy(leftColliders, 0, colliders, feetColliders.Length, leftColliders.Length);
					System.Array.Copy(rightColliders, 0, colliders, feetColliders.Length + leftColliders.Length, rightColliders.Length);
					System.Array.Copy(headColliders, 0, colliders, feetColliders.Length + leftColliders.Length + rightColliders.Length, headColliders.Length);
				}
				return colliders;
			}
			set {
				colliders = value;
			}
		}
		
		/// <summary>
		/// Gets the current collisions.
		/// </summary>
		virtual public RaycastHit2D[][] CurrentCollisions
		{
			get;
			protected set;
		}
		
		/// <summary>
		/// Gets the assigned input.
		/// </summary>
		virtual public Input Input
		{
			get
			{
				return input;
			}
		}


		
		/// <summary>
		/// The characters velocity.
		/// For details of what this means see <see cref="Character.VelocityType"/>.
		/// </summary>
		virtual public Vector2 Velocity
		{
			get; protected set;
		}
		
		/// <summary>
		/// The characters velocity in the previous frame.
		/// </summary>
		virtual public Vector2 PreviousVelocity
		{
			get; protected set;
		}
		
		/// <summary>
		/// How is the characters velocity currently represented.
		/// 
		/// This is here because different classes of movements are often best implemented with different ways of 
		/// representing velocity (for example for most platform ground movements X velocity is best handled as 
		/// relative to character angle).
		/// </summary>
		virtual public VelocityType VelocityType
		{
			get
			{
				return movements[activeMovement].VelocityType; 
			}
		}
		
		/// <summary>
		/// Returns true if we are grounded or false otherwise.
		/// </summary>
		virtual public bool Grounded
		{
			get
			{
				if (grounded || TimeSinceGrounded < groundedLeeway) return true;
				return false;
			}
			protected set
			{
				grounded = value;
			}
		}
		
		/// <summary>
		/// If the character is currently controlled by a ladder the character is considered to be 
		/// on ladder.
		/// </summary>
		virtual public bool OnLadder
		{
			get;
			protected set;
		}
		
		/// <summary>
		/// Tracks how much time has passed since the character was last grounded.
		/// </summary>
		virtual public float TimeSinceGrounded
		{
			get; protected set;
		}
		
		/// <summary>
		/// Tracks how much time has passed since the character was last grounded OR on a ladder.
		/// Useful for movements that can happen when grounded or when a ladder (like jump).
		/// </summary>
		virtual public float TimeSinceGroundedOrOnLadder
		{
			get; protected set;
		}
		
		/// <summary>
		/// Tracks how much time has passed since the character started falling.
		/// </summary>
		virtual public float TimeFalling
		{
			get; protected set;
		}
		
		/// <summary>
		/// Tracks how much time has passed since the character was on a slope.
		/// </summary>
		virtual public float TimeSinceSlope
		{
			get; protected set;
		}
		
		/// <summary>
		/// Gets the default value of gravity.
		/// </summary>
		virtual public float DefaultGravity
		{
			get
			{
				return gravity.Value;
			}
		}
		
		/// <summary>
		/// Gets the current value of gravity.
		/// </summary>
		virtual public float Gravity
		{
			get
			{
				if (movements[activeMovement].ShouldApplyGravity) return gravity.Value;
				return movements[activeMovement].CurrentGravity;
			}
		}
		
		/// <summary>
		/// Gets the current ladder or null if no ladder in range.
		/// </summary>
		/// <value>The current ladder.</value>
		virtual public Collider2D CurrentLadder
		{
			get
			{
				return currentLadder;
			}
		}
		
		/// <summary>
		/// Gets the current wall or null if no wall in range.
		/// </summary>
		/// <value>The current wall.</value>
		virtual public Collider2D CurrentWall
		{
			get
			{
				return currentWall;
			}
		}
		
		/// <summary>
		/// Gets the current wall collider
		/// </summary>
		/// <value>The current wall.</value>
		virtual public int CurrentWallCollider
		{
			get
			{
				return currentWallCollider;
			}
		}
		
		/// <summary>
		/// Returns the y position (in world space) of the bottom of the characters feet. 
		/// </summary>
		public float BottomOfFeet
		{
			get
			{
				float offset = 0; 
				for (int i = 0; i < Colliders.Length; i++)
				{
					if (Colliders[i].RaycastType == RaycastType.FOOT) 
					{
						offset = Colliders[i].Extent.y;
						break;
					}
				}
				return myTransform.position.y + offset;
			}
		}

		/// <summary>
		/// Returns the x position (in world space) of the rightmost of the characters colliders. 
		/// </summary>
		public float RightExtent
		{
			get
			{
				float offset = 0;
				for (int i = 0; i < Colliders.Length; i++)
				{
					if (Colliders[i].RaycastType == RaycastType.SIDE_RIGHT) 
					{
						if (Colliders[i].Extent.x > offset) offset = Colliders[i].Extent.x;
					}
				}
				return myTransform.position.x + offset;
			}
		}
	
		/// <summary>
		/// Returns the x position (in world space) of the leftmost of the characters colliders. 
		/// </summary>
		public float LeftExtent
		{
			get
			{
				float offset = 0;
				for (int i = 0; i < Colliders.Length; i++)
				{
					if (Colliders[i].RaycastType == RaycastType.SIDE_LEFT) 
					{
						if (Colliders[i].Extent.x < offset) offset = Colliders[i].Extent.x;
					}
				}

				return myTransform.position.x + offset;
			}
		}

		/// <summary>
		/// The characters current animation state.
		/// </summary>
		virtual public AnimationState AnimationState
		{
			get
			{
				return animationState;
			}
			protected set
			{
				if (value != animationState)
				{
					// TODO Should we just have a generic mechanism to delay all states if desired?
					if ((value != AnimationState.FALL && value != AnimationState.AIRBORNE)|| TimeSinceGrounded > timeForFallState)
					{
						OnChangeAnimationState(value, animationState, animationPriority);
						animationState = value;
					}
				}
			}
		}
		
		/// <summary>
		/// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
		/// </summary>
		virtual public int FacingDirection
		{
			get 
			{
				return movements[activeMovement].FacingDirection;
			}
		}
		
		/// <summary>
		/// Returns the direction the character is facing, but if direction is currently 0 instead returns the direction last faced.
		/// </summary>
		virtual public int LastFacedDirection
		{
			get 
			{
				
				// TODO Add a way to set the default facing direction
				if (activeMovement == -1 || movements == null || movements.Length <= activeMovement || movements[activeMovement] == null) return 1;
				if (movements[activeMovement].FacingDirection == 0) return currentFacingDirection != 0 ? currentFacingDirection : 1;
				return movements[activeMovement].FacingDirection;
			}
		}

		/// <summary>
		/// Gets or sets the type of the raycast for the parent platform. Value has no meaning if not parented.
		/// </summary>
		virtual public RaycastType ParentRaycastType
		{
			get
			{
				return parentRaycastType;
			}
			set
			{
				parentRaycastType = value;
			}
		}

		/// <summary>
		/// Gets or sets the parent platform.
		/// </summary>
		virtual public Platform ParentPlatform
		{
			get
			{
				return parentPlatform;
			}
			set
			{
				if (value == null)
				{
					// Unparent
					if (parentPlatform != null) 
					{
						parentPlatform.UnParent();
						myTransform.parent = null;
						parentTransform = null;
						parentPlatform = null;
					}
					
				}
				// Parent
				else if (parentPlatform != value)
				{
					parentPlatform = value;
					parentTransform = parentPlatform.transform;
					myTransform.parent = value.transform;
					parentPlatform.Parent();
				}
			}
		}

		/// <summary>
		/// Gets or sets the platform the charater is currently standing on (even if they aren't parented to it).
		/// </summary>
		virtual public Platform StoodOnPlatform
		{
			get; set;
		}

		/// <summary>
		/// If slopes are on this is the rotation we are rotating towards.
		/// </summary>
		virtual public float SlopeTargetRotation
		{
			get
			{
				return slope;
			}
		}
		
		/// <summary>
		/// If slopes are on this is the rotation we are currently on.
		/// </summary>
		public float SlopeActualRotation
		{
			get
			{
				float result = myTransform.rotation.eulerAngles.z;
				if (result > 180.0f) result -= 360;
				return result;
			}
		}
		
		/// <summary>
		/// If slopes are on this is the rotation we were previously targetting.
		/// NOTE: This is actually target rotation from 2 frames ago, as if we use one
		/// we often get the delta angle between the real target and the new angle due to 
		/// the transition frame.
		/// </summary>
		public float PreviousRotation
		{
			get; protected set;
		}
		
		/// <summary>
		/// Gets the minimum angle at which geometry is considered a wall.
		/// </summary>
		virtual public float MinSideAngle
		{
			get
			{
				return sideAngle;
			}
		}
		
		/// <summary>
		/// Gets or sets the current friction value or -1 if the movement should use its own default.
		/// </summary>
		virtual public float Friction
		{
			get; set;
		}
		
		/// <summary>
		/// Gets or sets the ignored slope. The ignored slope is used primary for internal
		/// physics calculations and is the largest slope that was ignored by the side colliders last frame.
		/// </summary>
		/// <value>The ignored slope.</value>
		virtual public float IgnoredSlope
		{
			get
			{
				return ignoredSlope;
			}
			set
			{
				if (value > ignoredSlope) ignoredSlope = value;
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether this characters gravity has been flipped.
		/// </summary>
		virtual public bool IsGravityFlipped
		{
			get
			{
				if (gravity is FlippableGravity && ((FlippableGravity)gravity).IsGravityFlipped) return true;
				return false;
			}
		}
		
		/// <summary>
		/// Gets the animation override state.
		/// </summary>
		virtual public string OverrideState {
			get
			{
				return overrideState;
			}
			protected set 
			{
				if (value != overrideState)
				{
					overrideState = value;
					OnChangeAnimationState();
				}
			}
		}
		
		/// <summary>
		/// Gets the default ground movement.
		/// </summary>
		virtual public GroundMovement DefaultGroundMovement
		{
			get 
			{
				return (GroundMovement) movements[defaultGroundMovement];
			}
		}
		
		/// <summary>
		/// Gets the default air movement.
		/// </summary>
		virtual public AirMovement DefaultAirMovement
		{
			get 
			{
				return (AirMovement) movements[defaultAirMovement];
			}
		}
		
		/// <summary>
		/// Gets the currently active movement.
		/// </summary>
		virtual public Movement ActiveMovement
		{
			get 
			{
#if UNITY_EDITOR
				if (activeMovement == -1) return null;
#endif
				return movements[activeMovement];
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether this instance is attacking.
		/// </summary>
		/// <value><c>true</c> if this instance is attacking; otherwise, <c>false</c>.</value>
		virtual public bool IsAttacking
		{
			get
			{
				if (activeAttack != -1 && ((BasicAttacks)movements[activeAttack]).IsAttacking) return true;
				return false;
			}
		}

		/// <summary>
		/// Gets the normalised time of the current attack or -1 if not attacking.
		/// </summary>
		virtual public float AttackNormalisedTime
		{
			get
			{
				if (activeAttack == -1) return -1.0f;
				return ((BasicAttacks)movements[activeAttack]).ActiveAttackNormalisedTime;
			}
		}

		/// <summary>
		/// Gets the name of the current attack or null if not attacking.
		/// </summary>
		virtual public string AttackName
		{
			get
			{
				if (activeAttack == -1) return null;
				return ((BasicAttacks)movements[activeAttack]).ActiveAttackName;
			}
		}
		
		/// <summary>
		/// Gets the location of the current attack or ANY if not attacking
		/// </summary>
		virtual public AttackLocation AttackLocation
		{
			get
			{
				if (activeAttack == -1) return AttackLocation.ANY;
				return ((BasicAttacks)movements[activeAttack]).ActiveAttackLocation;
			}
		}

		/// <summary>
		/// Returns true if the current attacks hit box has hit an enemy.
		/// </summary>
		virtual public bool AttackHasHit
		{
			get
			{
				if (activeAttack == -1) return false;
				return ((BasicAttacks)movements[activeAttack]).ActiveAttackHasHit;
			}
		}

		/// <summary>
		/// Gets the characters current z layer.
		/// </summary>
		/// <value>The Z layer.</value>
		virtual public int ZLayer
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the feet layer: the layer the character is currently standing on if grounded.
		/// </summary>
		virtual public int GroundLayer
		{
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the feet collider: the collider the character is currently standing on if grounded.
		/// </summary>
		virtual public Collider2D GroundCollider
		{
			get;
			set;
		}

		/// <summary>
		/// How many feet colliders the character has;  handy for some calculations.
		/// </summary>
		virtual public int FootCount
		{
			get
			{
				return feetColliders.Length;
			}
		}

		/// <summary>
		/// How many wall colliders hit the current wall this frame.
		/// </summary>
		virtual public int CurrentWallColliderCount
		{
			get; protected set;
		}

		/// <summary>
		/// Did we hit our head this frame?
		/// </summary>
		virtual public bool WouldHitHeadThisFrame
		{
			get; set;
		}

		#endregion
		
		#region events
		
		/// <summary>
		/// Event for animation state changes.
		/// </summary>
		public event System.EventHandler <AnimationEventArgs> ChangeAnimationState;
		
		/// <summary>
		/// Event for scene exit.
		/// </summary>
		public event System.EventHandler <SceneEventArgs> WillExitScene;

		/// <summary>
		/// Event for respawning.
		/// </summary>
		public event System.EventHandler <CharacterEventArgs> Respawned;

		/// <summary>
		/// Event for attacking.
		/// </summary>
		public event System.EventHandler <AttackEventArgs> AttackStarted;

		/// <summary>
		/// kjh:: 케릭터의 좌우 상태값. 
		/// 별로로 하는 이유는 블럭중 좌우 강제 이동 시키는 블럭이 있어 
		/// 케릭터에서 별도로 조작. 
		/// </summary>
		public int HorizontalAxisDigital
		{
			get
			{
				//actionMoveBlock 기능이 실행 되면 케릭터의 좌우 이동값은 0이 된다.
				return actionMoveBlock ? 0 : input.HorizontalAxisDigital;
			}
		}


		/// <summary>
		/// Raises the change animation state event. This version is public and it 
		/// just triggers a resend not a change.
		/// </summary>
		virtual public void OnChangeAnimationState()
		{
			if (ChangeAnimationState != null)
			{
				animationEventArgs.UpdateAnimationOverrideState(OverrideState);
				ChangeAnimationState(this, animationEventArgs);
			}
		}
		
		/// <summary>
		/// Raises the change animation state event.
		/// </summary>
		/// <param name="state">Current state.</param>
		/// <param name="previousState">Previous state.</param>
		virtual protected void OnChangeAnimationState(AnimationState state, AnimationState previousState)
		{
			if (ChangeAnimationState != null)
			{
				animationEventArgs.UpdateAnimationEventArgs(state, previousState, OverrideState);
				ChangeAnimationState(this, animationEventArgs);
			}
		}
		
		/// <summary>
		/// Raises the change animation state event.
		/// </summary>
		/// <param name="state">Current state.</param>
		/// <param name="previousState">Previous state.</param>
		/// <param name="priority">Animaton state priority.</param>
		virtual protected void OnChangeAnimationState(AnimationState state, AnimationState previousState, int priority)
		{
			Debug.Log( ChangeAnimationState == null ? "null" : "!null" );
			if (ChangeAnimationState != null)
			{
				animationEventArgs.UpdateAnimationEventArgs(state, previousState, OverrideState, priority);
				ChangeAnimationState(this, animationEventArgs);
			}
		}
		/// <summary>
		/// Raises the scene will exit event.
		/// </summary>
		/// <param name="newSceneName">New scene name.</param>
		virtual protected void OnSceneWillExit(string newSceneName)
		{
			if (WillExitScene != null)
			{
				#if !UNITY_4_6 && !UNITY_5_1 && !UNITY_5_2
				WillExitScene(this, new SceneEventArgs(SceneManager.GetActiveScene().name, newSceneName));
				#else
				WillExitScene(this, new SceneEventArgs(Application.loadedLevelName, newSceneName));
				#endif
			}
		}

		/// <summary>
		/// Raises the respawned event.
		/// </summary>
		virtual protected void OnRespawned()
		{
			if (Respawned != null)
			{
				Respawned(this, new CharacterEventArgs(this));
			}
		}

		/// <summary>
		/// Raises the attack state event. This version is public and it 
		/// just triggers a resend not a change.
		/// </summary>
		virtual protected void OnAttackStarted()
		{
			if (AttackStarted != null)
			{
				if (activeAttack != -1)
				{
					attackEventArgs.UpdateAttackStartedArgs(AttackName);
					AttackStarted(this, attackEventArgs);
				}
			}
		}

		#endregion
		
		#region Unity hooks

		void Awake()
		{
			// We have to create new colliders of the right type due to Unity's poor serialization
			for (int i = 0; i < feetColliders.Length; i++)
			{
				BasicRaycast basic = feetColliders[i];
				feetColliders[i] = new NoAllocationSmartFeetcast();
				feetColliders[i].Extent = basic.Extent;
				feetColliders[i].Transform = basic.Transform;
				feetColliders[i].Length = basic.Length;
				feetColliders[i].LayerMask = layerMask;
				feetColliders[i].LookAhead = feetLookAhead;
				feetColliders[i].RaycastType = RaycastType.FOOT;
			}

			for (int i = 0; i < rightColliders.Length; i++)
			{
				BasicRaycast basic = rightColliders[i];
				rightColliders[i] = new NoAllocationSmartSidecast();
				rightColliders[i].Extent = basic.Extent;
				rightColliders[i].Transform = basic.Transform;
				rightColliders[i].Length = basic.Length;
				rightColliders[i].LayerMask = layerMask;
				rightColliders[i].LookAhead = sideLookAhead;
				rightColliders[i].RaycastType = RaycastType.SIDE_RIGHT;
			}

			for (int i = 0; i < leftColliders.Length; i++)
			{
				BasicRaycast basic = leftColliders[i];
				leftColliders[i] = new NoAllocationSmartSidecast();
				leftColliders[i].Extent = basic.Extent;
				leftColliders[i].Transform = basic.Transform;
				leftColliders[i].Length = basic.Length;
				leftColliders[i].LayerMask = layerMask;
				leftColliders[i].LookAhead = sideLookAhead;
				leftColliders[i].RaycastType = RaycastType.SIDE_LEFT;
			}

			for (int i = 0; i < headColliders.Length; i++)
			{
				BasicRaycast basic = headColliders[i];
				headColliders[i] = new NoAllocationSmartHeadcast();
				headColliders[i].Extent = basic.Extent;
				headColliders[i].Transform = basic.Transform;
				headColliders[i].Length = basic.Length;
				headColliders[i].LayerMask = layerMask;
				headColliders[i].LookAhead = 0;
				headColliders[i].RaycastType = RaycastType.HEAD;
			}

			// Set up colliders
			CurrentCollisions = new RaycastHit2D[Colliders.Length][];
			for (int i = 0; i < Colliders.Length; i++)
			{
				// Ensure character references are correct 
				if (Colliders[i] is IRaycastColliderWithIMob) ((IRaycastColliderWithIMob) Colliders[i]).Mob = this;
			}

			//			// NOTE - This is outdated, but I'm still hesitant to remove
			//			if (defaultWallMovement != -1)
			//			{
			//				// Initialise colliders to use for wall cling
			//				collidersToUseForWallCling = Colliders.Where (c=>c.RaycastType == RaycastType.SIDE_LEFT || c.RaycastType == RaycastType.SIDE_RIGHT).OrderByDescending(c=>c.WorldPosition.y).ToList();
			//				collidersToUseForWallCling.RemoveRange((2 * ((WallMovement)movements[defaultWallMovement]).RequiredColliders), 
			//				                                       collidersToUseForWallCling.Count - (2 *  ((WallMovement)movements[defaultWallMovement]).RequiredColliders));
			//			}
			//			
			
			// Init cached copy of closest colliders
			closestColliders = new int[Colliders.Length];
		}

		/// <summary>
		/// Get all related behaviours and initialise.
		/// </summary>
		void Start()
		{
			Init();
		}

		/// <summary>
		/// Do the movement.
		/// </summary>
		void Update()
		{
			if (enabled && !TimeManager.Instance.Paused)
			{
				Vector2 previousVelocity = Velocity;
				float previousRotationTmp = previousRotation;
				previousRotation = SlopeTargetRotation;

				// Do any rotations
				if ((rotateToSlopes || (transform.rotation.eulerAngles.z != 0 && !IsGravityFlipped)) && movements[activeMovement].ShouldDoRotations) DoRotation();
				// Transitions
				TransitionMovementControl();

				// Movement
				movements[activeMovement].DoMove();
				
				// Set default friction
				Friction = -1;

				// Haven't hit head yet this frame
				WouldHitHeadThisFrame = false;

				// Do base collisions
				RaycastType typeMask = movements [activeMovement].ShouldDoBaseCollisions;
				DoBaseCollisions(typeMask);

				// Gravity is usually handled in base collissions but if base collissions doesn't handle feet it wont be
				if (((typeMask & RaycastType.FOOT) != RaycastType.FOOT) && movements[activeMovement].ShouldApplyGravity) gravity.ApplyGravity();
				
				// If we have a ground movement and it supports sliding on slopes apply slope 
				if (movements[activeMovement] is GroundMovement && ((GroundMovement)movements[activeMovement]).SupportsSlidingOnSlopes) ((GroundMovement)movements[activeMovement]).ApplySlopeForce();
				
				// Check for facing direction changes
				if (switchCollidersOnDirectionChange)
				{
					if (FacingDirection != 0 && currentFacingDirection != 0 && currentFacingDirection != FacingDirection)
					{
						SwitchColliders();
					}
				}

				// Ignore zero direction
				if (FacingDirection != 0) currentFacingDirection = FacingDirection;
				
				CheckForLadder();
				
				// Only check for walls if we are not on a ladder
				if (currentLadder == null) CheckForWall();
				
				SetGrounded();
				
				// Get target rotation
				if (calculateSlopes && movements[activeMovement].ShouldDoRotations) 
				{
					CalculateTargetRotation();
				}

				// Ensure last ignored slope is updated to -1 (i.e. reset)
				ignoredSlope = -1;

				// PostCollissionDoMove
				movements[activeMovement].PostCollisionDoMove();

				// Set animation state
				if (forcedAnimationState != AnimationState.NONE && forcedAnimationTime > 0.0f)
				{
					// Overriden state
					forcedAnimationTime -= TimeManager.FrameTime;
					AnimationState = forcedAnimationState;
					if (forcedAnimationTime <= 0.0f) forcedAnimationState = AnimationState.NONE;
				}
				else if (activeAttack != -1 && ((BasicAttacks)movements[activeAttack]).WantsToSetAnimationState())
				{
					// Attack overriding animation state
					animationPriority = movements[activeAttack].AnimationPriority;
					AnimationState = movements[activeAttack].AnimationState;
				}
				else
				{
					// Normal state - note we need to get priority first
					animationPriority = movements[activeMovement].AnimationPriority;
					AnimationState = movements[activeMovement].AnimationState;
				}
				
				PreviousVelocity = previousVelocity;
				PreviousRotation = previousRotationTmp;
			}
		}
		
		#endregion
		
		#region public methods
		
		/// <summary>
		/// Translate the character by the supplied amount.
		/// </summary>
		/// <param name="x">The x amount.</param>
		/// <param name="y">The y amount.</param>
		/// <param name="applyYTransformsInWorldSpace">Should transforms be in world space instead of relative to the
		/// character position? Default is true.</param>
		virtual public void Translate (float x, float y, bool applyTransformsInWorldSpace)
		{

			// This is very handy for debugging where a translate is coming from
//			System.Diagnostics.StackFrame frame = new System.Diagnostics.StackFrame(1);
//			var method = frame.GetMethod();
//			var type = method.DeclaringType;
//			var name = method.Name;

			//Debug.Log( "x : " + x + "  y : " + y + "  applyTransformsInWorldSpace : " + applyTransformsInWorldSpace );
			if (applyTransformsInWorldSpace)
			{
				if (gravity.IsGravityFlipped) y = -y;
				myTransform.Translate(x, y, 0, Space.World);
			}
			else
			{
				myTransform.Translate(x, y, 0, Space.Self);
			}
		}
		
		/// <summary>
		/// Adds velocity.
		/// </summary>
		/// <param name="x">The velocity to add to x.</param>
		/// <param name="y">The velocity to add to y.</param>
		/// <param name="velocityIsInWorldSpace">If true the velocity will not added in world space, 
		/// otherwise the velocity will be added in character space.</param>
		virtual public void AddVelocity(float x, float y, bool velocityIsInWorldSpace)
		{
			if (velocityIsInWorldSpace)
			{
				Vector2 worldVelocity = new Vector2(x, y);
				worldVelocity = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * worldVelocity;
				x = worldVelocity.x;
				y = worldVelocity.y;
			}
			Velocity = new Vector2(Velocity.x + x, Velocity.y + y);
			if (Velocity.y < terminalVelocity) Velocity = new Vector2(Velocity.x, terminalVelocity);
		}
		
		/// <summary>
		/// Sets X velocity.
		/// </summary>
		/// <param name="x">The new x speed.</param>
		virtual public void SetVelocityX(float x)
		{
			Velocity = new Vector2(x, Velocity.y);
		}
		
		/// <summary>
		/// Sets Y velocity.
		/// </summary>
		/// <param name="y">The new y speed.</param>
		virtual public void SetVelocityY(float y)
		{
			Velocity = new Vector2(Velocity.x, y);
			if (Velocity.y < terminalVelocity) Velocity = new Vector2(Velocity.x, terminalVelocity);
		}
		
		/// <summary>
		/// Executes base collisions for a given raycast collider type. Used by movements that require collision data.
		/// </summary>
		/// <param name="type">Raycast type.</param>
		virtual public void DoBaseCollisionsForRaycastType(RaycastType type)
		{
			for (int i = 0; i < Colliders.Length; i++)
			{
				if (Colliders[i].RaycastType == type) 
				{
					CurrentCollisions[i] = Colliders[i].GetRaycastHits();
				}
			}
		}

		/// <summary>
		/// Gets the closest collision for the collider from Colliders at the index i.
		/// Note this is generally the collissions from the previous frame when called by a movement during a transition check.
		/// </summary>
		/// <param name="i">The index.</param>
		virtual public RaycastHit2D GetClosestCollision(int i)
		{
			if (i >= 0 && i < closestColliders.Length) 
			{
				if (CurrentCollisions[i] == null) return EmptyRaycastHit;
				if (closestColliders [i] != -1) return CurrentCollisions[i][closestColliders [i]];
				return EmptyRaycastHit;
			}
			Debug.LogWarning ("Invalid collider index passed to GetClosestCollision");
			return EmptyRaycastHit;
		}

		/// <summary>
		/// Applies the base collisions for the given raycast collider type.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <returns> A vector representing the amount of movement applied.</returns>
		virtual public Vector2 ApplyBaseCollisionsForRaycastType(RaycastType type)
		{
			DoBaseCollisionsForRaycastType(type);
			return baseMovement.DoCollisions(type, geometryLayerMask, passthroughLayerMask);
		}
		
		/// <summary>
		/// Overrides the animation for the given amount of time.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="minimumTime">Minimum time.</param>
		virtual public void ForceAnimation(AnimationState state, float minimumTime)
		{
			forcedAnimationState = state;
			forcedAnimationTime = Mathf.Max(0.0f, minimumTime);
		}
		
		/// <summary>
		/// Adds an animation override. NOTE: Currently only one override is supported at a time.
		/// Note overrides are used to refer to an animator or layer override (depending on animation type) not forcing 
		/// the override of a single animation which is done with ForceAnimation().
		/// </summary>
		/// <param name="overrideState">Override State, null values iwll be ignored.</param>
		virtual public void AddAnimationOverride(string overrideState)
		{
			if (overrideState != null) OverrideState = overrideState;
		}
		
		/// <summary>
		/// Removes an animation override. NOTE: Currently only one override is supported at a time.
		/// </summary>
		/// <param name="overrideState">Override State, will be removed if it matches current value. Use empty string to remove all overrides.</param>
		virtual public void RemoveAnimationOverride(string overrideState)
		{
			if (OverrideState == overrideState || overrideState == "") OverrideState = null;
		}
		
		/// <summary>
		/// Damages the character which typically triggers some kind of damage animation.
		/// </summary>
		virtual public void Damage(DamageInfo info)
		{
			// Cancel active attack
			if (activeAttack != -1) ((BasicAttacks)movements[activeAttack]).InterruptAttack();
			if (defaultDamageMovement != -1)
			{
				if (activeMovement != defaultDamageMovement && activeMovement != -1) movements[activeMovement].LosingControl();
				activeMovement = defaultDamageMovement;
				((DamageMovement)movements[defaultDamageMovement]).Damage(info, false);
			}
#if UNITY_EDITOR
			else
			{
				// Commented this out as its pretty reasonable to not need to animate damage (for example play particle effect using Event Listener).
				// Debug.Log ("Received damage but no damage movement was found.");
			}
#endif
		}
		
		/// <summary>
		/// Kill the character which typically triggers some kind of death animation.
		/// </summary>
		virtual public void Kill(DamageInfo info)
		{
			// Cancel active attack
			if (activeAttack != -1) ((BasicAttacks)movements[activeAttack]).InterruptAttack();

			// Death actions can be used to disable additional components, but its pretty safe to assume 
			// that if the character has a hurt box and is dead then we don't want the hurtbox to collide with 
			// things anymore
			CharacterHurtBox hurtBox = GetComponentInChildren<CharacterHurtBox> ();
			if (hurtBox != null) hurtBox.GetComponent<Collider2D>().enabled = false;
			
			if (defaultDeathMovement != -1)
			{
				if (activeMovement != defaultDeathMovement && activeMovement != -1) movements[activeMovement].LosingControl();
				activeMovement = defaultDeathMovement;
				((DamageMovement)movements[defaultDeathMovement]).Damage(info, true);
			}
			// Else see if there is a damage movement and try that
			else if (defaultDamageMovement != -1)
			{
				if (activeMovement != defaultDamageMovement && activeMovement != -1) movements[activeMovement].LosingControl();
				activeMovement = defaultDamageMovement;
				((DamageMovement)movements[defaultDamageMovement]).Damage(info, true);
			}
			else
			{
				Debug.LogWarning ("Received kill but no death movement was found. You should add a death movement!");
			}
		}
		
		/// <summary>
		/// Respawn the character. Called by LevelManager.
		/// </summary>
		virtual public void Respawn()
		{
			// Reset everything
			activeMovement = defaultGroundMovement;
			activeAttack = -1;
			Velocity = Vector2.zero;
			CharacterHurtBox hurtBox = GetComponentInChildren<CharacterHurtBox> ();
			if (hurtBox != null) hurtBox.GetComponent<Collider2D>().enabled = true;
			OnRespawned ();
		}
		
		/// <summary>
		/// Called by something that will load a new level (without killing character).
		/// </summary>
		virtual public void AboutToExitScene(string newSceneName)
		{
#if !UNITY_4_6 && !UNITY_5_1 && !UNITY_5_2
			LevelManager.PreviousLevel = SceneManager.GetActiveScene().name;
#else
			LevelManager.PreviousLevel = Application.loadedLevelName;
#endif
			OnSceneWillExit (newSceneName);
		}

		/// <summary>
		/// Indicate current attack is finished.
		/// </summary>
		virtual public void AttackFinished()
		{
			activeAttack = -1;
		}

		/// <summary>
		/// Sets the named property across all applicable movements.
		/// </summary>
		/// <param name="name">Property name.</param>
		/// <param name="value">Value to set.</param>
		virtual public void SetTaggedProperty(string name, object value)
		{
			SetTaggedProperty (name, value, null);
		}
		
		/// <summary>
		/// Sets the named property across all applicable movements.
		/// </summary>
		/// <param name="name">Property name.</param>
		/// <param name="value">Value to set.</param>
		/// <param name="excludedTypes">Movements of this type will not have their value changed.</param>
		virtual public void SetTaggedProperty(string name, object value, List<System.Type> excludedTypes)
		{
#if DISABLE_NAMED_PROPERTIES
			Debug.LogWarning("Tried to use named properties but named properties are disabled");
			return;
#endif
			if (namedProperties.ContainsKey(name))
			{
				foreach (System.Reflection.FieldInfo setter in namedProperties[name])
				{
					foreach (Movement movement in movements)
					{
						if ((excludedTypes == null || !excludedTypes.Contains(movement.GetType())) && setter.DeclaringType.IsAssignableFrom(movement.GetType()))
						{
							try
							{
								if (setter.FieldType == typeof(System.Boolean) )
								{
									bool valueAsBool = true;
									if (value is int && ((int)value) == 0) valueAsBool = false;
									if (value is bool && !(bool)value) valueAsBool = false;
									setter.SetValue(movement, valueAsBool);
								}
								else
								{
									setter.SetValue(movement, value);
								}
							}
							catch (System.ArgumentException)
							{
								Debug.LogWarning("Tried to set wrong value type for named attribute " + name + " this may cause crashes in builds");
							}
						}
					}
				}
			}
			#if UNITY_EDITOR
			else
			{
				Debug.LogWarning("Tried to set named property " + name + " but no movement properties matched");
			}
			#endif
		}
		
		
		/// <summary>
		/// Adds to the named property across all applicable movements.
		/// </summary>
		/// <param name="name">Property name.</param>
		/// <param name="value">Value to set.</param>
		virtual public void AddToTaggedProperty(string name, float value)
		{
			AddToTaggedProperty (name, value, null);
		}
		
		/// <summary>
		/// Adds to the named property across all applicable movements.
		/// </summary>
		/// <param name="name">Property name.</param>
		/// <param name="value">Value to set.</param>
		/// <param name="excludedTypes">Movements of this type will not have their value changed.</param>
		virtual public void AddToTaggedProperty(string name, float value, List<System.Type> excludedTypes)
		{
#if DISABLE_NAMED_PROPERTIES
			Debug.LogWarning("Tried to use named properties but named properties are disabled");
			return;
#endif
			if (namedProperties.ContainsKey(name))
			{
				foreach (System.Reflection.FieldInfo setter in namedProperties[name])
				{
					foreach (Movement movement in movements)
					{
						if ((excludedTypes == null || !excludedTypes.Contains(movement.GetType())) && setter.DeclaringType.IsAssignableFrom(movement.GetType()))
						{
							try
							{
								setter.SetValue(movement, (float)setter.GetValue(movement) + value);
							}
							catch (System.ArgumentException)
							{
								Debug.LogWarning("Tried to set wrong value type for named attribute " + name + " this may cause crashes in builds");
							}
						}
					}
				}
			}
			#if UNITY_EDITOR
			else
			{
				Debug.LogWarning("Tried to set named property " + name + " but no movement properties matched");
			}
			#endif
		}
		
		/// <summary>
		/// Multiplies the named property across all applicable movements.
		/// </summary>
		/// <param name="name">Property name.</param>
		/// <param name="value">Value to set.</param>
		virtual public void MultiplyTaggedProperty(string name, float value)
		{
			MultiplyTaggedProperty (name, value, null);
		}
		
		/// <summary>
		/// Multiplies the named property across all applicable movements.
		/// </summary>
		/// <param name="name">Property name.</param>
		/// <param name="value">Value to set.</param>
		/// <param name="excludedTypes">Movements of this type will not have their value changed.</param>
		virtual public void MultiplyTaggedProperty(string name, float value, List<System.Type> excludedTypes)
		{
#if DISABLE_NAMED_PROPERTIES
			Debug.LogWarning("Tried to use named properties but named properties are disabled");
			return;
#endif
			if (namedProperties.ContainsKey(name))
			{
				foreach (System.Reflection.FieldInfo setter in namedProperties[name])
				{
					foreach (Movement movement in movements)
					{
						if ((excludedTypes == null || !excludedTypes.Contains(movement.GetType())) && setter.DeclaringType.IsAssignableFrom(movement.GetType()))
						{
							try
							{
								setter.SetValue(movement, (float) setter.GetValue(movement) * value);
							}
							catch (System.ArgumentException)
							{
								Debug.LogWarning("Tried to set wrong value type for named attribute " + name + " this may cause crashes in builds");
							}
						}
					}
				}
			}
#if UNITY_EDITOR
			else
			{
				Debug.LogWarning("Tried to set named property " + name + " but no movement properties matched");
			}
#endif
		}

		/// <summary>
		/// Check if a given collider should be ignored in collisions.
		/// </summary>
		/// <returns><c>true</c> if this instance is ignoring he specified collider; otherwise, <c>false</c>.</returns>
		/// <param name="collider">Collider.</param>
		virtual public bool IsColliderIgnored(Collider2D collider) {
			if (ignoredColliders.Contains (collider)) return true;
			return false;
		}
		
		/// <summary>
		/// Adds a collider to the list of ignored colliders.
		/// </summary>
		/// <param name="collider">Collider.</param>
		virtual public void AddIgnoredCollider(Collider2D collider)
		{
			ignoredColliders.Add (collider);
		}
		
		/// <summary>
		/// Removes a collider from the list of ignored colliders.
		/// </summary>
		/// <param name="collider">Collider.</param>
		virtual public void RemoveIgnoredCollider(Collider2D collider)
		{
			ignoredColliders.Remove (collider);
		}

#if UNITY_EDITOR
		/// <summary>
		/// Used during validation in the editor to check if a movements has priority over default air movement.
		/// </summary>
		/// <returns><c>true</c>, if has priority over default air , <c>false</c> otherwise.</returns>
		public bool MovementHasPriorityOverDefaultAir(Movement movement)
		{
			for (int i = 0; i < movements.Length; i++)
			{
				if (movements[i] == movement) return true;
				if (i == defaultAirMovement) return false;
			}
			Debug.LogError ("No default movement set");
			return false;
		}

		/// <summary>
		/// Used during validation in the editor to check if a movements has priority over default ground movement.
		/// </summary>
		/// <returns><c>true</c>, if has priority over default ground, <c>false</c> otherwise.</returns>
		public bool MovementHasPriorityOverDefaultGround(Movement movement)
		{
			for (int i = 0; i < movements.Length; i++)
			{
				if (movements[i] == movement) return true;
				if (i == defaultGroundMovement) return false;
			}
			Debug.LogError ("No default movement set");
			return false;
		}

		/// <summary>
		/// Gets or sets a value indicating whether input is enabled for this Character.
		/// </summary>
		/// <value><c>true</c> if input enabled; otherwise, <c>false</c>.</value>
		public bool InputEnabled {
			get 
			{
				if (input is DisabledInput) return false;
				return true;
			}
			set
			{
				if (value) 
				{
					if (!(input is DisabledInput)) return;
					input = cachedInput;
				}
				else
				{
					if (input is DisabledInput) return;
					if (disabledInput == null) {
						disabledInput = gameObject.AddComponent<DisabledInput>();
						disabledInput.Init(input);
					}
					if (cachedInput == null) cachedInput = input;
					input = disabledInput;
				}
			}
		}

#endif
		#endregion
		
		#region protected methods
		
		/// <summary>
		/// Set up the character
		/// </summary>
		virtual protected void Init()
		{
#if AOT_COMPILER_HINTS || UNITY_WEBGL
			DoAotCompilerHints();
#endif

			// Cache transform
			myTransform = transform;
			
			// Get gravity
			gravity = GetComponent<Gravity>();
			if (gravity != null)
			{
				gravity.Init (this);
			}
			else
			{
				Debug.LogError ("A character must have a Gravity");
			}
			
			// Get base movement
			baseMovement = GetComponent<BaseCollisions>();
			if (baseMovement != null)
			{
				baseMovement.Init(this);
			}
			else
			{
				Debug.LogError ("A character must have a BaseCollisions");
			}
			
			// Get movements - this MUST be done after getting gravity
			defaultGroundMovement = -1; defaultAirMovement = -1; defaultLadderMovement = -1; defaultWallMovement = -1; 
			defaultAttackMovement = -1; defaultDamageMovement = -1; defaultDeathMovement = -1;
			minimumWallColliders = 999;
			movements = GetComponentsInChildren<Movement>();

			if (movements.Length > 1)
			{
				activeMovement = 0;
				for (int i = movements.Length - 1; i >= 0; i--)
				{
					// Set default ground movement
					if (movements[i] is GroundMovement && defaultGroundMovement == -1) defaultGroundMovement = i;
					// Set default air movement
					if (movements[i] is AirMovement && defaultAirMovement == -1) defaultAirMovement = i;
					// Set default ladder movement
					if (movements[i] is ClimbMovement && defaultLadderMovement == -1) defaultLadderMovement = i;
					// Set default wall movement
					if (movements[i] is WallMovement && defaultWallMovement == -1) defaultWallMovement = i;
					// Set default damage movement
					if (movements[i] is DamageMovement && defaultDamageMovement == -1) defaultDamageMovement = i;
					// Set default death movement
					if (movements[i] is DeathMovement && defaultDeathMovement == -1) defaultDeathMovement = i;
					// Set default death movement
					if (movements[i] is BasicAttacks && defaultAttackMovement == -1) defaultAttackMovement = i;


					// Initialise movement
					movements[i] = movements[i].Init(this);

					// Find minimum wall colliders
					if (movements[i] is WallMovement)
					{
						if (minimumWallColliders > ((WallMovement)movements[i]).RequiredColliders)
						{
							minimumWallColliders = ((WallMovement)movements[i]).RequiredColliders;
						}
					}
				}
			}
			else
			{
				Debug.LogError ("A character must have at least two Movements");
			}

#if UNITY_EDITOR
			for (int i = movements.Length - 1; i >= 0; i--)
			{
				string result = movements[i].PostInitValidation();
				if (result != null)
				{
					Debug.LogError("Major configuration error, aborting: " + result);
					this.enabled = false;
					return;
				}
			}
#endif

			if (defaultGroundMovement == -1)
			{
				Debug.LogError("Major configuration error, aborting: A character must have a default ground movement");
				this.enabled = false;
				return;
			}
			if (defaultAirMovement == -1)
			{
				Debug.LogError("Major configuration error, aborting: A character must have a default air movement");
				this.enabled = false;
				return;
			}
			
			ladderLayerNumber = LayerMask.NameToLayer(ladderLayerOrTagName);
			wallLayerNumber = LayerMask.NameToLayer(wallLayerOrTagName);

			// Get diabled input reference if it exists 
			disabledInput = GetComponentInChildren<DisabledInput>();

			// If we don't have an assigned input try to find one
			if (input == null)
			{

				Input[] inputs = FindObjectsOfType<Input>();
				if (inputs.Length == 1) 
				{
					if (inputs[0] is DisabledInput)
					{
						Debug.LogError ("Major configuration error, aborting: The character doesn't have an assigned Input and only a DisabledInput could be found in the scene.");
					}
					else 
					{
						input = inputs[0];
					}
				}
				else if (inputs.Length == 0)
				{
					Debug.LogError ("Major configuration error, aborting: The character doesn't have an assigned Input and no Inputs could be found in the scene.");
					this.enabled = false;
					return;
				}
				else 
				{
					// Check for a buffered input, these are preferred
					foreach (Input potentialInput in inputs)
					{
						if (potentialInput is BufferedInput)
						{
							input = potentialInput;
							break;
						}
					}
					// Check for MultiInput these are the next best
					foreach (Input potentialInput in inputs)
					{
						if (potentialInput is MultiInput)
						{
							input = potentialInput;
							break;
						}
					}
					// Otherwise raise a warning and use the first
					if (input == null)
					{
						// Okay so if they added two disabled inputs this would fail, but really?
						if (inputs[0] is DisabledInput) input = inputs[1];
						else input = inputs[0];
						Debug.LogWarning ("The character doesn't have an assigned Input and multiple Inputs were found in the scene, using the first.");
					}
				}

			}

			// Create variable for event arguments
			animationEventArgs = new AnimationEventArgs(AnimationState.NONE, AnimationState.NONE, null);
			attackEventArgs = new AttackEventArgs (this, null);

			// Assume we start out grounded (although its not a problem if we don't, we will switch movement immediately)
			activeMovement = defaultGroundMovement;

			// If we aren't calcualting slopes we can't use slopes
			if (!calculateSlopes) rotateToSlopes = false;

			// Set active attack to none (-1)
			activeAttack = -1;

#if !DISABLE_NAMED_PROPERTIES
			// Create named property dictionary
			CreateNamedPropertiesDictionary ();
#endif

			// Initialise ignored colliders list
			ignoredColliders = new HashSet<Collider2D> ();
		}
		
		/// <summary>
		/// Sets the grounded state by checking feet colliders.
		/// </summary>
		virtual protected void SetGrounded()
		{
			// If we use base collisions let them determine if grounded.
			if ( ( movements[activeMovement].ShouldDoBaseCollisions & RaycastType.FOOT ) == RaycastType.FOOT ) Grounded = baseMovement.IsGrounded();
			// Else let the active movement tell us
			else
				Grounded = movements[activeMovement].IsGrounded();

			// Update frames since grounded
			if (grounded) 
			{
				TimeSinceGrounded = 0.0f;
				TimeSinceGroundedOrOnLadder = 0.0f;
			}
			else 
			{
				TimeSinceGrounded += TimeManager.FrameTime;
				if (!OnLadder) TimeSinceGroundedOrOnLadder += TimeManager.FrameTime;
				else TimeSinceGroundedOrOnLadder = 0.0f;
			}
			// Update falling time
			if (Velocity.y < 0.0f)
			{
				TimeFalling += TimeManager.FrameTime;
			}
			else
			{
				TimeFalling = 0.0f;
			}
		}
		
		/// <summary>
		/// Pass control to the movement most suitable for the current state.
		/// </summary>
		virtual protected void TransitionMovementControl()
		{
			// Check attacks before anything else even if the other movement WantsControl()
			int newActiveAttack = -1;
			// Check attacks first
			for (int i = 0; i < movements.Length; i++)
			{
				if (movements[i].enabled && movements[i] is BasicAttacks && 
				    // Some platforms might disallow (skip) certain types of movements, lets check this
				    (parentPlatform == null || !parentPlatform.SkipMovement(this, movements[i])) &&
				    (StoodOnPlatform == null || StoodOnPlatform == parentPlatform || !StoodOnPlatform.SkipMovement(this, movements[i])) &&
			    	// Only combo attacks if attack is active
			    	(activeAttack == -1)
			    )
				{
					// Attacks
					if (newActiveAttack == -1 && movements[i] is BasicAttacks && ((BasicAttacks)movements[i]).WantsAttack())
					{
						bool wantsControl = ((BasicAttacks)movements[i]).Attack();
						// If this attack wants control then maintain it
						if (wantsControl) {
#if UNITY_EDITOR
							if (activeMovement != i && movements[activeMovement].WantsControl()) 
							{
// 								Debug.LogWarning("Attacks wants control at the same time as a movement, this may lead to unpredictable behaviour. Consider using an attack that does not override movement state.");
							}
#endif
							if (activeMovement != i)
							{
								movements[activeMovement].LosingControl();
								// We dont call gain control when gaining control through a "special" method
								// movements[i].GainControl();
							}
							activeMovement = i;
							activeAttack = i;
							return;
						}
						else
						{
							newActiveAttack = i;
							activeAttack = i;
							if (movements[i].OverrideState != null) AddAnimationOverride(movements[i].OverrideState);
						}
						OnAttackStarted();
					}
				}
			}

			// Current movement is not ready to relinquish control
			if (movements[activeMovement].enabled && movements[activeMovement].WantsControl()) return;

			// Get wall reference
			Platform currentWallPlatform = null;
			if (currentWall != null) currentWallPlatform = currentWall.GetComponent<Platform> ();

			for (int i = 0; i < movements.Length; i++)
			{
				if (movements[i].enabled &&
				    // Some platforms might disallow (skip) certain types of movements, lets check this
				    (currentWallPlatform == null || !currentWallPlatform.SkipMovement(this, movements[i])) &&
				    (parentPlatform == null || !parentPlatform.SkipMovement(this, movements[i])) &&
				 	(StoodOnPlatform == null || StoodOnPlatform == parentPlatform || !StoodOnPlatform.SkipMovement(this, movements[i])))
				{

					// Special moves
					if (movements[i] is SpecialMovement && (activeAttack == -1 || !((BasicAttacks)movements[activeAttack]).BlockSpecial()) &&  ((SpecialMovement)movements[i]).WantsSpecialMove())
					{
						if (activeMovement != i)
						{
							((SpecialMovement)movements[i]).DoSpecialMove();
							movements[activeMovement].LosingControl();
							// We dont call GainControl when gaining control through a "special" method
							// movements[i].GainControl();
						}
						activeMovement = i;
						return;
					}

					// Ladders
					if (movements[i] is ClimbMovement && movements[i].enabled && (activeAttack == -1 || !((BasicAttacks)movements[activeAttack]).BlockClimb()) && currentLadder != null)
					{
						// Ladder conditions are met
						if (((ClimbMovement)movements[i]).WantsClimb())
						{
							OnLadder = true;
							if (activeMovement != i)
							{
								movements[activeMovement].LosingControl();
								movements[i].GainControl();
							}
							activeMovement = i;
							return;
						}
					}
					
					// Walls
					if (movements[i] is WallMovement &&  movements[i].enabled && (activeAttack == -1 || !((BasicAttacks)movements[activeAttack]).BlockWall()))
					{
						// Wall conditions are met
						if (((WallMovement)movements[i]).WantsCling())
						{
							if (activeMovement != i) 
							{
								movements[activeMovement].LosingControl();
								((WallMovement)movements[i]).DoCling();
								// We dont call GainControl when gaining control through a "special" method
								// movements[i].GainControl();
							}
							activeMovement = i;
							return;
						}
					}
					// Air movement jump conditions are met
					if (movements[i] is AirMovement && movements[i].enabled && (activeAttack == -1 || !((BasicAttacks)movements[activeAttack]).BlockJump()) && ((AirMovement)movements[i]).WantsJump())
					{
						if (activeMovement != i)
						{
							movements[activeMovement].LosingControl();
							// We dont call GainControl when gaining control through a "special" method
							// movements[i].GainControl();
						}
						((AirMovement)movements[i]).DoJump();
						activeMovement = i;
						return;
					}
					// Or otherwise the air movement wants control without a jump
					else if (movements[i] is AirMovement && movements[i].enabled && ((AirMovement)movements[i]).WantsAirControl())
					{
						if (activeMovement != i)
						{
							movements[activeMovement].LosingControl();
							movements[i].GainControl();
						}
						activeMovement = i;
						return;
					}
					
					if (movements[i] is GroundMovement && movements[i].enabled && ((GroundMovement)movements[i]).WantsGroundControl())
					{
						if (activeMovement != i)
						{
							movements[activeMovement].LosingControl();
							movements[i].GainControl();
						}
						activeMovement = i;
						return;
					}
				}
			}
			// We didn't find a matching movement lets use defaults
			// Ground movement if grounded and not moving upwards
			if (grounded && Velocity.y <= 0)
			{
#if UNITY_EDITOR
				if (!movements[defaultGroundMovement].enabled) Debug.LogWarning("The default ground movement is disabled but it will be given control anyway");
#endif
				// If we are already on a ground movement leave it
				if (activeMovement == defaultGroundMovement) return;
				// Else give control to default grounded
				movements[activeMovement].LosingControl();

				movements[defaultGroundMovement].GainControl();
				activeMovement = defaultGroundMovement;
			}
			// Airborne
			else
			{
#if UNITY_EDITOR
				if (!movements[defaultAirMovement].enabled) Debug.LogWarning("The default air movement is disabled but it will be given control anyway");
#endif
				// If we are already on an air movement leave it
				if (activeMovement == defaultAirMovement) return;
				// Else give control back to default air
				movements[activeMovement].LosingControl();
				movements[defaultAirMovement].GainControl();
				activeMovement = defaultAirMovement;
			}
			
			// Set any override state
			if (movements[activeMovement].OverrideState != null) AddAnimationOverride(movements[activeMovement].OverrideState);
			
			// TODO Do we even need this?
			OnLadder = false;
			
		}
		
		/// <summary>
		/// Does the base collisions.
		/// </summary>
		virtual protected void DoBaseCollisions(RaycastType typeMask)
		{
			// Early out if none to be cast
			if (typeMask == RaycastType.NONE) return;
			
			// TODO Make this order configurable without coding
			
			// Cache velocity
			float y = Velocity.y; float x = Velocity.x; 
			
			if (((typeMask & RaycastType.SIDE_LEFT) == RaycastType.SIDE_LEFT) &&  x < 0) ApplyBaseCollisionsForRaycastType(RaycastType.SIDE_LEFT);
			if (((typeMask & RaycastType.SIDE_RIGHT) == RaycastType.SIDE_RIGHT) && x > 0) ApplyBaseCollisionsForRaycastType(RaycastType.SIDE_RIGHT);
			
			// Gravity
			if (movements[activeMovement].ShouldApplyGravity && ((typeMask & RaycastType.FOOT) == RaycastType.FOOT)) gravity.ApplyGravity();
			
			// Only apply head collisions if not falling too fast
			if (y >= fallSpeedForIgnoreHeadCollisions || (parentPlatform != null && parentRaycastType == RaycastType.HEAD))
			{
				if ((typeMask & RaycastType.HEAD) == RaycastType.HEAD) ApplyBaseCollisionsForRaycastType(RaycastType.HEAD);
			}
			// Else we still need to update these collisions just not apply them
			else if ((typeMask & RaycastType.HEAD) == RaycastType.HEAD) 
			{
				DoBaseCollisionsForRaycastType(RaycastType.HEAD);
				UpdateClosestColliders(RaycastType.HEAD);
			}
			if ((typeMask & RaycastType.FOOT) == RaycastType.FOOT) ApplyBaseCollisionsForRaycastType(RaycastType.FOOT);
			
			if (((typeMask & RaycastType.SIDE_LEFT) == RaycastType.SIDE_LEFT) && x >= 0) ApplyBaseCollisionsForRaycastType(RaycastType.SIDE_LEFT);
			if (((typeMask & RaycastType.SIDE_RIGHT) == RaycastType.SIDE_RIGHT) && x <= 0) ApplyBaseCollisionsForRaycastType(RaycastType.SIDE_RIGHT);
			
		}
		
		
		
		/// <summary>
		/// Checks if we collided with a ladder.
		/// </summary>
		virtual protected void CheckForLadder()
		{
			if (defaultLadderMovement != -1)
			{
				for (int i = 0; i < CurrentCollisions.Length; i++)
				{
					if (CurrentCollisions[i] != null)
					{
						for (int j = 0; j < CurrentCollisions[i].Length; j++)
						{
							if (CurrentCollisions[i][j].collider != null &&  CurrentCollisions[i][j].collider.gameObject != null )
							{
								if (detectLaddersByTag)
								{
									if (CurrentCollisions[i][j].collider.gameObject.tag == ladderLayerOrTagName) 
									{
										// TODO What if we touch multiple ladders do we need to disambiguate based on direction or distance?
										currentLadder = CurrentCollisions[i][j].collider;
										return;
									}
								}
								else
								{
									if (CurrentCollisions[i][j].collider.gameObject.layer == ladderLayerNumber) 
									{
										// TODO What if we touch multiple ladders do we need to disambiguate based on direction or distance?
										currentLadder = CurrentCollisions[i][j].collider;
										return;
									}
								}
							}
						}
					}
				}
				currentLadder = null;
			}
		}
		
		/// <summary>
		/// Checks if we collided with a wall.
		/// </summary>
		virtual protected void CheckForWall()
		{
			int leftCount = 0;
			int rightCount = 0;
			Collider2D tempWall = null;
			
			// TODO Handle non-default wall movement
			if (defaultWallMovement != -1)
			{
				for (int i = 0; i < CurrentCollisions.Length; i++)
				{
					if (CurrentCollisions[i] != null && (Colliders[i].RaycastType == RaycastType.SIDE_LEFT || Colliders[i].RaycastType == RaycastType.SIDE_RIGHT))
					{
						for (int j = 0; j < CurrentCollisions[i].Length; j++)
						{
							if (CurrentCollisions[i][j].collider != null && CurrentCollisions[i][j].collider.gameObject != null && 
							    // Only climb on geometry layers
							    (1 << CurrentCollisions[i][j].collider.gameObject.layer & geometryLayerMask) == 1 << CurrentCollisions[i][j].collider.gameObject.layer)
							{
								if (detectAllWalls)
								{
									if (Colliders[i].RaycastType == RaycastType.SIDE_RIGHT) rightCount++;
									else if (Colliders[i].RaycastType == RaycastType.SIDE_LEFT) leftCount++;
									if (tempWall == null) 
									{
										tempWall = CurrentCollisions[i][j].collider;
										currentWallCollider = i;
									}
									// We cling to the highest wall using the highest collider
									else if (Colliders[currentWallCollider].WorldExtent.y < Colliders[i].WorldExtent.y)
									{
										tempWall = CurrentCollisions[i][j].collider;
										currentWallCollider = i;
									} 
								}
								else if (detectWallsByTag)
								{
									if (CurrentCollisions[i][j].collider.gameObject.tag == wallLayerOrTagName) 
									{
										if (Colliders[i].RaycastType == RaycastType.SIDE_RIGHT) rightCount++;
										else if (Colliders[i].RaycastType == RaycastType.SIDE_LEFT) leftCount++;
										if (tempWall == null) 
										{
											tempWall = CurrentCollisions[i][j].collider;
											currentWallCollider = i;
										}
										// We cling to the highest wall using the highest collider
										else if (Colliders[currentWallCollider].WorldExtent.y < Colliders[i].WorldExtent.y)
										{
											tempWall = CurrentCollisions[i][j].collider;
											currentWallCollider = i;
										} 
									}
								}
								else
								{
									if (CurrentCollisions[i][j].collider.gameObject.layer == wallLayerNumber) 
									{
										if (Colliders[i].RaycastType == RaycastType.SIDE_RIGHT) rightCount++;
										else if (Colliders[i].RaycastType == RaycastType.SIDE_LEFT) leftCount++;
										if (tempWall == null) 
										{
											tempWall = CurrentCollisions[i][j].collider;
											currentWallCollider = i;
										}
										// We cling to the highest wall using the highest collider
										else if (Colliders[currentWallCollider].WorldExtent.y < Colliders[i].WorldExtent.y)
										{
											tempWall = CurrentCollisions[i][j].collider;
											currentWallCollider = i;
										} 
									}
								}
							}
						}
					}
				}
				// Set wall
				if (tempWall != null &&
				    ((Colliders[currentWallCollider].GetDirection().x == 1 && rightCount >= minimumWallColliders) ||
				  	 (Colliders[currentWallCollider].GetDirection().x == -1 && leftCount >= minimumWallColliders)))
				{
					if (Colliders[currentWallCollider].GetDirection().x == 1)
				    {
						CurrentWallColliderCount = rightCount;
					}
					else
					{
						CurrentWallColliderCount = leftCount;
					}
					currentWall = tempWall;
				}
				else
				{
					// No wall found
					currentWall = null;
					currentWallCollider = -1;
					CurrentWallColliderCount = 0;
				}
			}
		}
		
		/// <summary>
		/// Rotate towards the target rotation.
		/// </summary>
		virtual protected void DoRotation()
		{
			float difference  = -slope -myTransform.eulerAngles.z;

			// Special case keep upright on slopes if parented even when rotateToSlopes is off
			if (!rotateToSlopes) 
			{
				difference = -myTransform.rotation.eulerAngles.z;
			}

			// Shouldn't really happen but just in case
			if (difference > 180) difference = difference - 360;
			if (difference < -180) difference = difference + 360;
			Vector3 rotateAround = transform.position;

			// Determine the point we will rotate around
			switch (characterRotationType)
			{
				case CharacterRotationType.COLLIDING_TO_AVERAGE:
					if (rotationPoint != -1)
					{
						rotateAround = feetColliders[rotationPoint].WorldExtent;
					}
					else
					{
						rotateAround = (feetColliders[0].WorldExtent + feetColliders[feetColliders.Length - 1].WorldExtent) / 2.0f;
					}
					break;
				case CharacterRotationType.MIDDLE_TO_AVERAGE:
					rotateAround = (feetColliders[0].WorldExtent + feetColliders[feetColliders.Length - 1].WorldExtent) / 2.0f;
					break;
				case CharacterRotationType.TRANSFORM_TO_AVERAGE:
					// Do noting this is already set.
					break;
			}

			if (difference > rotationSpeed * TimeManager.FrameTime) difference = rotationSpeed * TimeManager.FrameTime;
			if (difference < -rotationSpeed * TimeManager.FrameTime) difference = -rotationSpeed * TimeManager.FrameTime;
			myTransform.RotateAround(rotateAround, new Vector3(0,0,1), difference);
		}
		
		/// <summary>
		/// Use the feet colliders to determine the target rotation.
		/// </summary>
		virtual protected void CalculateTargetRotation()
		{
			int layer = -1;
			
			float totalSlope = 0.0f;
			int slopeCount = 0;
			
			float rightFootWeight = 0.0f;
			float leftFootWeight = 0.0f;
			float rightFootSlope = 0.0f;
			float leftFootSlope = 0.0f;
			
			// If the active movement doesn't uses base collisions we need to execute the feet collisions.
			if ((movements[activeMovement].ShouldDoBaseCollisions & RaycastType.FOOT) != RaycastType.FOOT) 
			{
				DoBaseCollisionsForRaycastType(RaycastType.FOOT);
				UpdateClosestColliders(RaycastType.FOOT);
			}
			else
			{
				closestColliders = baseMovement.ClosestColliders;
			}
			
			// Left Foot hit something - assumes left foot is the first collider
			if (closestColliders[0] != -1)
			{
				// Right layer
				layer = CurrentCollisions[0][closestColliders[0]].collider.gameObject.layer ;
				if ((1 << layer & geometryLayerMask) == 1 << layer || (((1 << layer & passthroughLayerMask) == 1 <<layer) && Velocity.y <= 0))
				{
					float deg = Mathf.Rad2Deg * Mathf.Atan2(CurrentCollisions[0][closestColliders[0]].normal.x, CurrentCollisions[0][closestColliders[0]].normal.y);
					if (deg <= maxSlopeRotation && deg >= -maxSlopeRotation) 
					{
						leftFootSlope = deg;
						leftFootWeight = 1.0f;
					}
			
				}
			}
			
			// Right Foot hit something - assumes right foot is the collider at feetColliders.Length - 1
			if (closestColliders[feetColliders.Length - 1] != -1)
			{
				// Right layer
				layer = CurrentCollisions[feetColliders.Length - 1][closestColliders[feetColliders.Length - 1]].collider.gameObject.layer ;
				if ((1 << layer & geometryLayerMask) == 1 << layer || (((1 << layer & passthroughLayerMask) == 1 <<layer) && Velocity.y <= 0))
				{
					float deg = Mathf.Rad2Deg * Mathf.Atan2(CurrentCollisions[feetColliders.Length - 1][closestColliders[feetColliders.Length - 1]].normal.x, CurrentCollisions[feetColliders.Length - 1][closestColliders[feetColliders.Length - 1]].normal.y);
					if (deg <= maxSlopeRotation && deg >= -maxSlopeRotation) 
					{
						rightFootSlope = deg;
						rightFootWeight = 1.0f;
					}

				}
			}

			// If only one foot hit, just use that one
			if (rightFootWeight == 0.0f && leftFootWeight > 0.0f)
			{
				slope = leftFootSlope;
				TimeSinceSlope = 0.0f;
				rotationPoint = 0;
			}
			else if (leftFootWeight == 0.0f && rightFootWeight > 0.0f)
			{
				slope = rightFootSlope;
				TimeSinceSlope = 0.0f;
				rotationPoint = feetColliders.Length - 1;
			}
			// Else if both hit
			else if (leftFootWeight > 0.0f && rightFootWeight > 0.0f)
			{
				// Early out if they are the same
				if (Mathf.Approximately(leftFootSlope, rightFootSlope))
				{
					slope = leftFootSlope;
					TimeSinceSlope = 0.0f;
					rotationPoint = -1;
				}
				// Else cycle through other feet and get the average
				else
				{
					// Ensure we don't run in to issues with extremes
					if (rightFootSlope == 180.0f && leftFootSlope < 0) rightFootSlope = -180;
					if (rightFootSlope == -180.0f && leftFootSlope >= 0) rightFootSlope = 180;
					if (leftFootSlope == 180.0f && rightFootSlope < 0) leftFootSlope = -180;
					if (leftFootSlope == -180.0f && rightFootSlope >= 0) leftFootSlope = 180;

					totalSlope += rightFootSlope + leftFootSlope;
					slopeCount = 2;

					for (int i = 1; i < feetColliders.Length - 1; i++)
					{
						if (closestColliders[i] != -1)
						{
							layer = CurrentCollisions[i][closestColliders[i]].collider.gameObject.layer ;
							if ((1 << layer & geometryLayerMask) == 1 << layer || (1 << layer & passthroughLayerMask) == 1 << layer)
							{
								float deg = Mathf.Rad2Deg * Mathf.Atan2(CurrentCollisions[i][closestColliders[i]].normal.x, CurrentCollisions[i][closestColliders[i]].normal.y);
								if (deg < maxSlopeRotation && deg > -maxSlopeRotation) 
								{
									if (deg == 180.0f && (rightFootSlope < 0 || leftFootSlope < 0)) deg = -180;
									if (deg == -180.0f && (rightFootSlope >= 0 || leftFootSlope >= 0)) deg = 180;
									totalSlope += deg;
									slopeCount++;
								}
							}
						}
					}
					slope = totalSlope / slopeCount;
					rotationPoint = -1;
				}
			}
			// Else if none hit
			else
			{
				TimeSinceSlope += TimeManager.FrameTime;
				if (gravity.IsGravityFlipped)
				{
					slope = 180.0f;
				}
				else
				{
					slope = 0.0f;
				}
			}
		}
		
		/// <summary>
		/// Updates the closest colliders array for the given type mask, leaves other types alone.
		/// </summary>
		virtual protected void UpdateClosestColliders(RaycastType typeMask)
		{
			int layer;
			
			// Get the closest collision
			for (int i = 0; i < CurrentCollisions.Length; i++)
			{
				// If we are considering this type of collider
				if ((Colliders[i].RaycastType & typeMask) == Colliders[i].RaycastType)
				{
					int closest = -1;
					float smallestFraction = float.MaxValue;
					for (int j = 0; j < CurrentCollisions[i].Length; j++)
					{	
						// Make sure we have a collision to support raycast colliders which use statically allocated hits arrays 
						if (CurrentCollisions[i][j].collider != null)
						{
							// Correct layer?
							layer = CurrentCollisions[i][j].collider.gameObject.layer;
							if ((1 << layer & layerMask) == 1 << layer || (Colliders[i].RaycastType == RaycastType.FOOT && (1 << layer & passthroughLayerMask) == 1 << layer))
							{
								if (CurrentCollisions[i][j].fraction > 0 && CurrentCollisions[i][j].fraction < smallestFraction)
								{
									smallestFraction = CurrentCollisions[i][j].fraction;
									closest = j;
								}
							}
						}
					}
					// Store the closest collider
					closestColliders[i] = closest;

					if (Colliders[i].RaycastType == RaycastType.HEAD && closest !=-1) WouldHitHeadThisFrame = true;
				}
			}
		}
		
		/// <summary>
		/// Switches left and right collider direction and flips position of all colliders about y axis.
		/// </summary>
		virtual protected void SwitchColliders()
		{
			// Switch raycast colliders
			for (int i = 0; i < feetColliders.Length; i++)
			{
				feetColliders[i].Extent = new Vector2(-feetColliders[i].Extent.x, feetColliders[i].Extent.y);
			}
			for (int i = 0; i < headColliders.Length; i++)
			{
				headColliders[i].Extent = new Vector2(-headColliders[i].Extent.x, headColliders[i].Extent.y);
			}
			for (int i = 0; i < leftColliders.Length; i++)
			{
				leftColliders[i].Extent = new Vector2(-leftColliders[i].Extent.x, leftColliders[i].Extent.y);
				leftColliders[i].RaycastType = leftColliders[i].RaycastType == RaycastType.SIDE_LEFT ? RaycastType.SIDE_RIGHT : RaycastType.SIDE_LEFT;
			}
			for (int i = 0; i < rightColliders.Length; i++)
			{
				rightColliders[i].Extent = new Vector2(-rightColliders[i].Extent.x, rightColliders[i].Extent.y);
				rightColliders[i].RaycastType = rightColliders[i].RaycastType == RaycastType.SIDE_RIGHT ? RaycastType.SIDE_LEFT : RaycastType.SIDE_RIGHT;
			}
		}

		/// <summary>
		/// Creates the dictionary used for apply named proeprty changes.
		/// </summary>
		virtual protected void CreateNamedPropertiesDictionary()
		{
			namedProperties = new Dictionary<string, List<System.Reflection.FieldInfo>> ();
			foreach (Movement movement in movements)
			{
				System.Reflection.FieldInfo[] fields = movement.GetType().GetFields();
				foreach (System.Reflection.FieldInfo field in fields)
				{
					TaggedProperty[] attributes = (TaggedProperty[] )field.GetCustomAttributes(typeof(TaggedProperty), true);
					foreach (TaggedProperty attribute in attributes)
					{
						if (namedProperties.ContainsKey(attribute.name))
					    {
							namedProperties[attribute.name].Add (field);
						}
						else
						{
							namedProperties.Add (attribute.name, new List<System.Reflection.FieldInfo>() { field });
						}
					}
				}
			}
		}

		#endregion

		/// <summary>
		/// For some AOT builds (VITA WEBGL) we need to do extra hinting to get the right classes in our assembly.
		/// </summary>
		void DoAotCompilerHints()
		{
			Debug.Log ("Loading " + AirMovement_DelayedJump.Info.Name);
			Debug.Log ("Loading " + AirMovement_Digital.Info.Name);
			Debug.Log ("Loading " + AirMovement_DigitalNoDirChange.Info.Name);
			Debug.Log ("Loading " + AirMovement_JetPack.Info.Name);
			Debug.Log ("Loading " + AirMovement_Physics.Info.Name);
			Debug.Log ("Loading " + AirMovement_Variable.Info.Name);
			Debug.Log ("Loading " + AirMovement_VariableWithInertia.Info.Name);
			Debug.Log ("Loading " + GroundMovement_Crouch.Info.Name);
			Debug.Log ("Loading " + GroundMovement_CrouchNoCrawl.Info.Name);
			Debug.Log ("Loading " + GroundMovement_CrouchWithCrawl.Info.Name);
			Debug.Log ("Loading " + GroundMovement_CrouchWithSlide.Info.Name);
			Debug.Log ("Loading " + GroundMovement_Digital.Info.Name);
			Debug.Log ("Loading " + GroundMovement_DigitalWithRun.Info.Name);
			Debug.Log ("Loading " + GroundMovement_DigitalStopOnUpDown.Info.Name);
			Debug.Log ("Loading " + GroundMovement_Physics.Info.Name);
			Debug.Log ("Loading " + GroundMovement_PhysicsWithRun.Info.Name);
			Debug.Log ("Loading " + GroundMovement_MecanimAnimationDriven.Info.Name);
			Debug.Log ("Loading " + GroundMovement_RollOnLand.Info.Name);
			Debug.Log ("Loading " + DamageMovement_AnimationOnly.Info.Name);
			Debug.Log ("Loading " + DamageMovement_AnimationWithBobble.Info.Name);
			Debug.Log ("Loading " + SpecialMovement_PlayAnimation.Info.Name);
			Debug.Log ("Loading " + WallMovement_AutoStick.Info.Name);
			Debug.Log ("Loading " + WallMovement_WallCling.Info.Name);
			Debug.Log ("Loading " + WallMovement_WallJump.Info.Name);
			Debug.Log ("Loading " + LadderMovement_Digital.Info.Name);
			Debug.Log ("Loading " + BasicAttacks.Info.Name);
		}

		/// <summary>
		/// KJH:: 점프 블럭의 점프
		/// </summary>
		public void JumpBlock( int _jumpHeight )
		{
			RPGSoundManager.Instance.PlayEffectSound( 1 );
			( ( AirMovement )movements[0] ).DoOverridenJump( _jumpHeight, 1 );
		}

		/// <summary>
		/// MoveBlock 기능이 실행 되는지 체크 한다. 
		/// </summary>
		private bool actionMoveBlock = false;

		private float moveBlockTimer = 0f;

		private int HorizontalAxis = 0;

		public void ActionMoveBlock()
		{
			if( !actionMoveBlock )
			{
				HorizontalAxis = -HorizontalAxisDigital;
				actionMoveBlock = true;
				StartCoroutine( C_ActionMoveBlock() );
			}
			else
			{
				moveBlockTimer = 0;
				HorizontalAxis = -HorizontalAxis;
			}
		}

		public void C_ActionMoveBlockStop()
		{
			StopCoroutine( "C_ActionMoveBlock" );
		}

		private IEnumerator C_ActionMoveBlock()
		{
			moveBlockTimer = 0;
			float time = 0.2f;

			while( true )
			{
				moveBlockTimer += TimeManager.FrameTime;
				MoveBlock( 20 );
				if ( time <= moveBlockTimer || !GameManager.Instance.IsPlaying )
					break;
				yield return null;
			}
			actionMoveBlock = false;
			HorizontalAxis = 0;
		}

		/// <summary>
		/// 좌우 점프대 블럭. 
		/// </summary>
		/// <param name="_frameSpeed"></param>
		public void MoveBlock( float _frameSpeed )
		{
			float frameSpeed = _frameSpeed;
			if ( HorizontalAxis == 1 )
			{
				SetVelocityX( IsGravityFlipped ? -frameSpeed : frameSpeed );
				Translate( ( IsGravityFlipped ? -frameSpeed : frameSpeed ) * TimeManager.FrameTime, 0, false );
			}
			else if ( HorizontalAxis == -1 )
			{
				SetVelocityX( IsGravityFlipped ? frameSpeed : -frameSpeed );
				Translate( ( IsGravityFlipped ? frameSpeed : -frameSpeed ) * TimeManager.FrameTime, 0, false );
			}
		}

		public void TimePaused()
		{
			TimeManager.Instance.Paused = false;
		}
	}	
}