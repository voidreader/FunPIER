/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// The base collision behaviour stops the character from passing through obstacles.
	/// It is called by most <c>Movement<c> calls after they have finished movement to ensure
	/// that the character doesn't penetrate level geometry.
	/// <see cref="Movement"/> 
	/// </summary>
	public class BaseCollisions : MonoBehaviour
	{
		#region members
		
		/// <summary>
		/// Cached reference to the character.
		/// </summary>
		protected Character character;
		
		/// <summary>
		/// Keeps track of the grounded state ccording to the last time collisions were executed.
		/// </summary>
		protected bool grounded;
		
		/// <summary>
		/// Calcualte how long it would take to fall feet distance with the current gravity. We use this to 
		/// control passthrough platform application.
		/// </summary>
		protected float timeToFallFeetDistance;
		
		/// <summary>
		/// If gravity and feet length is constant we can do less calculations for passthrough platforms.
		/// </summary>
		protected bool assumeConstantGravityAndFeetDistance;
		
		/// <summary>
		/// Keeps track of the closest collisions.
		/// </summary>
		protected int[] closestColliders;
		
		/// <summary>
		/// The platform collision arguments. Avoid creating a new one each frame.
		/// </summary>
		protected PlatformCollisionArgs platformCollisionArgs;
		
		/// <summary>
		/// An array of colldiers which received an action message this frame.
		/// </summary>
		protected Collider2D[] collidersAlreadySentMessages;

		/// <summary>
		/// When we fall fast we need to consider passthroughs that we fell past but should have snapped to.
		/// However at the very peak of a jump this can cause us to snap oddly. This constant  stops that happening by
		/// ignoring fall times smaller than this. 
		/// </summary>
		private const float MinFastFallPassthroughTime = 0.5f;
		
		#endregion
		
		#region properties
		
		/// <summary>
		/// Make closest collisions available so we don't need to calculate twice.
		/// </summary>
		public int[] ClosestColliders
		{
			get
			{
				return closestColliders;
			}
		}
		
		#endregion
		
		#region public methods
		
		/// <summary>
		/// Initialise this instance. Called by the cahracter on start.
		/// </summary>
		/// <param name="character">Character.</param>
		virtual public void Init(Character character)
		{
			this.character = character;
			if (assumeConstantGravityAndFeetDistance)
			{
				for (int i = 0; i < character.Colliders.Length; i++)
				{
					if (character.Colliders[i].RaycastType == RaycastType.FOOT) 
					{
						if (character.Gravity < 0)
						{
							timeToFallFeetDistance = Mathf.Sqrt((-2 * character.Colliders[i].Length) / character.Gravity);
							break;
						}
					}
				}
			}
			closestColliders = new int[character.Colliders.Length];
			platformCollisionArgs = new PlatformCollisionArgs();
			collidersAlreadySentMessages = new Collider2D[character.Colliders.Length];
		}
		
		/// <summary>
		/// Check for collider penetration of the given types on the given layers and move the character if found.
		/// </summary>
		/// <param name="typeMask">The types to consider.</param>
		/// <param name="layerMask">The layers to consider.</param>
		/// <returns>A vector representing the amount of movement applied.</returns>
		virtual public Vector2 DoCollisions(RaycastType typeMask, int layerMask, int passthroughLayerMask)
		{
			Vector2 result = Vector2.zero;
			
			float deepestSidePenetration = 0.0f;
			float deepestHeadPenetration = 0.0f;
			float deepestFootPenetration = 0.0f;
			float deepestNonPassthroughFootPenetration = 0.0f;
			
			int sideIndex = -1;
			int headIndex = -1;
			int footIndex = -1;
			int nonPassthroughFootIndex = -1;
			
			int groundedFeetCount = 0;
			int passthroughFeetCount = 0;
			
			int layer;
			bool parentFound = false;
			
			// TODO could we just do this every frame only sending a message once even if multiple collider types hit the collider?
			ResetCollisions();

			// If we are doing feet collisions update grounded
			if ((RaycastType.FOOT & typeMask) == RaycastType.FOOT )
			{
				grounded = false;
				if (character is Character)
				{
					((Character)character).StoodOnPlatform = null;
					((Character)character).GroundLayer = -1;
					((Character)character).GroundCollider = null;
				}
			}
			
			// If we are moving in Y assume we aren't grounded
			// if (character.Velocity.y != 0 ) grounded = false;
			
			// Get the closest collision - TODO: we have similar code in character too, its (very slightly) more efficient this way but maybe easier to maintain if we reuse?
			for (int i = 0; i < character.CurrentCollisions.Length; i++)
			{
				// If we are considering this type of collider
				if ((character.Colliders[i].RaycastType & typeMask) == character.Colliders[i].RaycastType)
				{
					int closest = -1;
					float smallestFraction = float.MaxValue;
					Platform platform = null;
					for (int j = 0; j < character.CurrentCollisions[i].Length; j++)
					{	
						// Make sure we have a collision to support raycast colliders which use statically allocated hits arrays 
						if (character.CurrentCollisions[i][j].collider != null)
						{
							// Correct layer?
							layer = character.CurrentCollisions[i][j].collider.gameObject.layer;
							if ((1 << layer & layerMask) == 1 << layer || (character.Colliders[i].RaycastType == RaycastType.FOOT && (1 << layer & passthroughLayerMask) == 1 << layer))
							{
								if (character.CurrentCollisions[i][j].fraction > 0 && character.CurrentCollisions[i][j].fraction < smallestFraction)
								{
									// The memory allocation only occurs in the editor, its not the GC trap that is seems
									Platform tmpPlatform = character.CurrentCollisions[i][j].collider.GetComponent<Platform>();
                                    if (tmpPlatform == null || !tmpPlatform.IgnoreCollision(character, character.Colliders[i]))
                                    {
                                        platform = tmpPlatform;
                                        smallestFraction = character.CurrentCollisions[i][j].fraction;
                                        closest = j;
                                    }
								}
							}
						}
					}
					
					// Store the closest collider
					closestColliders[i] = closest;
					// Found a collision
					if (closest != -1)
					{
						layer = character.CurrentCollisions[i][closest].collider.gameObject.layer;
						float penetration = (character.CurrentCollisions[i][closest].fraction * (character.Colliders[i].Length + character.Colliders[i].LookAhead)) - character.Colliders[i].Length;
						switch (character.Colliders[i].RaycastType)
						{
							
						case RaycastType.SIDE_LEFT :
							if (penetration < deepestSidePenetration)
							{
								deepestSidePenetration = penetration;
								sideIndex = i;
							}
							// Check for platforms
							// TODO We may need a new lookahead if condition here. If the grounded look ahead
							// was large we may be triggering platforms too early.
							if (!HasReceivedCollideMessage(character.CurrentCollisions[i][closest].collider))
							{
								platformCollisionArgs.Character = character;
								platformCollisionArgs.RaycastCollider = character.Colliders[i];
								platformCollisionArgs.Penetration = penetration;
								if (platform != null) 
								{
									bool parent = platform.Collide(platformCollisionArgs);
									if (parent) 
									{
										parentFound = true;
										character.ParentPlatform = platform;
										character.ParentRaycastType = RaycastType.SIDE_LEFT;
									}
									AddColliderToSentMessageReceivers(character.CurrentCollisions[i][closest].collider);
								}
							}
							break;
						case RaycastType.SIDE_RIGHT :
							if (penetration < deepestSidePenetration)
							{
								deepestSidePenetration = penetration;
								sideIndex = i;
							}
							// Check for platforms
							// TODO We may need a new lookahead if condition here. If the grounded look ahead
							// was large we may be triggering platforms too early.
							if (!HasReceivedCollideMessage(character.CurrentCollisions[i][closest].collider))
							{
								platformCollisionArgs.Character = character;
								platformCollisionArgs.RaycastCollider = character.Colliders[i];
								platformCollisionArgs.Penetration = penetration;
								if (platform != null) 
								{
									bool parent = platform.Collide(platformCollisionArgs);
									if (parent) 
									{
										parentFound = true;
										character.ParentPlatform = platform;
										character.ParentRaycastType = RaycastType.SIDE_RIGHT;
									}
									AddColliderToSentMessageReceivers(character.CurrentCollisions[i][closest].collider);
								}
							}
							break;
						case RaycastType.HEAD :
							if (penetration < deepestHeadPenetration)
							{
								deepestHeadPenetration = penetration;
								headIndex = i;
							}
							// Check for platforms
							// TODO We may need a new lookahead if condition here. If the head look ahead
							// was large we may be triggering platforms too early.
							if (!HasReceivedCollideMessage(character.CurrentCollisions[i][closest].collider))
							{
								platformCollisionArgs.Character = character;
								platformCollisionArgs.RaycastCollider = character.Colliders[i];
								platformCollisionArgs.Penetration = penetration;
								if (platform != null) 
								{
									bool parent = platform.Collide(platformCollisionArgs);
									if (parent) 
									{
										parentFound = true;
										character.ParentPlatform = platform;
										character.ParentRaycastType = RaycastType.HEAD;
									}
									AddColliderToSentMessageReceivers(character.CurrentCollisions[i][closest].collider);
								}
							}
							character.WouldHitHeadThisFrame = true;
							break;
						case RaycastType.FOOT :
							if (penetration < deepestFootPenetration)
							{
								deepestFootPenetration = penetration;
								footIndex = i;
							}
							// Keep track of non passthrough feet collisions as these should be applied even if passthrough is deeper
							if (((1 << layer & passthroughLayerMask) != 1 << layer) && penetration < deepestNonPassthroughFootPenetration)
							{
								deepestNonPassthroughFootPenetration = penetration;
								nonPassthroughFootIndex = i;
							}
							// Check for platforms
							// TODO We may need a new lookahead if condition here. If the grounded look ahead
							// was large we may be triggering platforms too early.
							if (!HasReceivedCollideMessage(character.CurrentCollisions[i][closest].collider))
							{
								platformCollisionArgs.Character = character;
								platformCollisionArgs.RaycastCollider = character.Colliders[i];
								platformCollisionArgs.Penetration = penetration;
								if (platform != null) 
								{
									bool parent = platform.Collide(platformCollisionArgs);
									if (parent) 
									{
										parentFound = true;
										character.ParentPlatform = platform;
										character.ParentRaycastType = RaycastType.FOOT;
									}
									AddColliderToSentMessageReceivers(character.CurrentCollisions[i][closest].collider);
									// TODO: Currently we just take whichever is last in the feet collider array
									// we could update this to handle multiple platforms.
									character.StoodOnPlatform = platform;
								}
							}
							
							if (penetration <= character.groundedLookAhead)
							{
								
								// Keep track of how many passthrough feet we have
								if ((1 << layer & passthroughLayerMask) == 1 << layer)
								{
									passthroughFeetCount++;
									// TODO consider if this should be user configurable?
									if (character.Velocity.y <= 0 && 
									    (character.Colliders[i].WorldExtent.y - character.CurrentCollisions[i][closest].point.y) > character.passthroughLeeway)
									{
										// Set grounded only if we are moving downwards
										grounded = true;
										// Keep track of how many feet we have grounded
										groundedFeetCount++;
										// Set layer if its not set or if this is the middle foot collider
										if (character.GroundLayer == -1 || i == character.FootCount / 2) 
										{
											character.GroundLayer = layer;
											character.GroundCollider = character.CurrentCollisions[i][closest].collider;
										}
									}
								}
								else
								{
									// Set grounded
									grounded = true;
									// Keep track of how many feet we have grounded
									groundedFeetCount++;
									// Set layer if its not set or if this is the middle foot collider
									if (character.GroundLayer == -1 || i == character.FootCount / 2)
									{
										character.GroundLayer = layer;
										character.GroundCollider = character.CurrentCollisions[i][closest].collider;
									}
								}
							}
							break;
						default:
							Debug.LogError("Unexpected collider type");
							break;
						}
					}
				}
			}
			
			// If we had penetration apply movement
			if (sideIndex != -1 && deepestSidePenetration < 0)
			{
				result.x = deepestSidePenetration * (character.Colliders[sideIndex].RaycastType == RaycastType.SIDE_LEFT ? -1 : 1);
				character.Translate(result.x, 0, false);
				if (!character.ActiveMovement.PreventXVelocityReset)
				{
					if ((character.Velocity.x > 0 && character.Colliders[sideIndex].RaycastType == RaycastType.SIDE_RIGHT) ||
					    (character.Velocity.x < 0 && character.Colliders[sideIndex].RaycastType == RaycastType.SIDE_LEFT))
					{
						character.SetVelocityX(0);
					}
				}
				// character.SetVelocityY(character.Velocity.y);
			}
			// Always apply geometry layer feet collisions
			if (nonPassthroughFootIndex != -1 && deepestNonPassthroughFootPenetration < 0)
			{
				result.y = -deepestNonPassthroughFootPenetration;
				character.Translate(0, -deepestNonPassthroughFootPenetration, false);
				// If we were moving down reset Y velocity to 0
				if (character.Velocity.y < 0) character.SetVelocityY(0);
			}
			// Check for passthrough feet too
			if (footIndex != -1 && footIndex != nonPassthroughFootIndex && deepestFootPenetration < deepestNonPassthroughFootPenetration)
			{
				// Ensure we calculate the time to move feet distance
				if (!assumeConstantGravityAndFeetDistance && character.Colliders[footIndex].RaycastType == RaycastType.FOOT)
				{
					if (character.Gravity < 0)
					{
						timeToFallFeetDistance = Mathf.Sqrt((-2 * character.Colliders[footIndex].Length) / character.Gravity);
					}
				}
				// Additional conditions for passthrough feet
				if (character.Velocity.y <= 0 && (grounded || (character.TimeFalling > character.minFastFallPassthroughTime && character.TimeFalling > timeToFallFeetDistance)))
				{
					result.y = -deepestFootPenetration;
					character.Translate(0, -deepestFootPenetration, false);
					
					// If we were moving down reset Y velocity to 0
					if (character.Velocity.y < 0) character.SetVelocityY(0);
				}
			}
			if (headIndex != -1 & deepestHeadPenetration < 0)
			{
				result.y = deepestHeadPenetration;
				character.Translate(0, deepestHeadPenetration, false);
				// If we were moving up reset Y velocity to 0
				if (character.Velocity.y > 0) character.SetVelocityY(0);
			}
			
			// If we are doing feet collisions update parent state
			if (((character.ParentRaycastType & typeMask) == character.ParentRaycastType ) && !parentFound)
			{
				character.ParentPlatform = null;
			}

			// If we are doing feet collisions update grounded
			if ((RaycastType.FOOT & typeMask) == RaycastType.FOOT )
			{
				character.GroundedFootCount = groundedFeetCount;
			}
			return result;
		}
		
		/// <summary>
		/// Is the character grounded based on the last time collisions were calculated?
		/// </summary>
		virtual public bool IsGrounded()
		{
			return grounded;
		}
		
		#endregion
		
		#region protected methods
		
		/// <summary>
		/// Clear the array of colliders that have received Collide() messages.
		/// </summary>
		virtual protected void ResetCollisions()
		{
			for (int i = 0; i < collidersAlreadySentMessages.Length; i++) collidersAlreadySentMessages[i] = null;
		}
		
		/// <summary>
		/// Determines whether the provided collider has received Collide() message this frame.
		/// </summary>
		/// <returns><c>true</c> if the collider has received Collide() message; otherwise, <c>false</c>.</returns>
		/// <param name="collider">Collider.</param>
		virtual protected bool HasReceivedCollideMessage(Collider2D collider)
		{
			for (int i = 0; i < collidersAlreadySentMessages.Length; i++) 
			{
				if (collidersAlreadySentMessages[i] == collider) return true;
			}
			return false;
		}
		
		/// <summary>
		/// Adds a colldier to the list of colliders that received the Collide() message.
		/// </summary>
		/// <param name="collider">Collider to add.</param>
		virtual protected void AddColliderToSentMessageReceivers(Collider2D collider)
		{
			for (int i = 0; i < collidersAlreadySentMessages.Length; i++) 
			{
				if (collidersAlreadySentMessages[i] == null)
				{
					collidersAlreadySentMessages[i] = collider;
					break;
				}
			}
		}
		
		#endregion
		
	}
}