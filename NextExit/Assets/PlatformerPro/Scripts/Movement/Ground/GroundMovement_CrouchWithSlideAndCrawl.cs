#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Movement for crouching that will slide if moving at speed.
	/// </summary>
	public class GroundMovement_CrouchWithSlideAndCrawl : GroundMovement_CrouchWithSlide
	{
		
		#region members
		/// <summary>
		/// Speed we crawl at.
		/// </summary>
		public float crawlSpeed;

		/// <summary>
		/// True if we have started crawling
		/// </summary>
		protected bool isCrawling;

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Crouch/With Slide and Crawl";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Crouch movement that will slide if you crouch while running, and allows you to crawl a long once moving too slow to slide.";
		
		/// <summary>
		/// Static movement info used by the editor.
		/// </summary>
		new public static MovementInfo Info
		{
			get
			{
				return new MovementInfo(Name, Description);
			}
		}

		/// <summary>
		/// The index of the max speed.
		/// </summary>
		protected const int CrawlSpeedIndex = 12;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 13;

		#endregion
		
		#region public methods
		
		/// <summary>
		/// If this is true then the movement wants to control the object on thre ground.
		/// </summary>
		/// <returns><c>true</c> if control is wanted, <c>false</c> otherwise.</returns>
		override public bool WantsGroundControl()
		{
			if (!enabled) return false;
			if (character.Grounded && CheckInput ())
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			float actualFriction = character.Friction;
			if (actualFriction == -1) actualFriction = friction;

			float actualSpeed = GetSpeed (crawlSpeed);

			ApplySlopeForce();

			// Apply drag

			if (character.Velocity.x > 0) 
			{
				character.AddVelocity(-character.Velocity.x * actualFriction * TimeManager.FrameTime, 0, false);
				if (character.Velocity.x < 0) character.SetVelocityX(0);
			}
			else if (character.Velocity.x < 0) 
			{
				character.AddVelocity(-character.Velocity.x * actualFriction * TimeManager.FrameTime, 0, false);
				if (character.Velocity.x > 0) character.SetVelocityX(0);
			}


			// Limit to max speed
			if (character.Velocity.x > maxSpeed) 
			{
				character.SetVelocityX(maxSpeed);
			}
			else if (character.Velocity.x < -maxSpeed) 
			{
				character.SetVelocityX(-maxSpeed);
			}
			// Quiesce if moving very slowly and no force applied
			if (character.Velocity.x > 0 && character.Velocity.x < quiesceSpeed ) 
			{
				character.SetVelocityX(0);
			}
			else if (character.Velocity.x  < 0 && character.Velocity.x > -quiesceSpeed)
			{
				character.SetVelocityX(0);
			}

			// If we were moving faster than crawl we are sliding not crawling no matter what
			if (Mathf.Abs(character.Velocity.x) > actualSpeed) isCrawling = false;

			// Stop instantly if we were crawling
			if (isCrawling && character.Input.HorizontalAxisDigital == 0) character.SetVelocityX(0);

			// Check if we should crawl
			if (character.Input.HorizontalAxisDigital != 0)
			{
				if (character.Velocity.x < actualSpeed && character.Input.HorizontalAxisDigital == 1)
				{
					character.SetVelocityX(actualSpeed);
					isCrawling = true;
				}
				else if (character.Velocity.x > -actualSpeed && character.Input.HorizontalAxisDigital == -1)
				{
					character.SetVelocityX(-actualSpeed);
					isCrawling = true;
				}
			}

			// Translate
			character.Translate(character.Velocity.x * TimeManager.FrameTime, 0, false);

		}

		
		/// <summary>
		/// Initialise the mvoement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			base.Init (character, movementData);
			
			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				crawlSpeed = movementData [CrawlSpeedIndex].FloatValue;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}
			return this;
		}
		
		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (character.Velocity.x == 0) return AnimationState.CROUCH;
				if (isCrawling)	return AnimationState.CROUCH_WALK;
				return AnimationState.CROUCH_SLIDE;
			}
		}

		/// <summary>
		/// Called when the movement gets control. Typically used to do initialisation of velocity and the like.
		/// </summary>
		override public void GainControl()
		{
			base.GainControl ();
			isCrawling = false;
		}
		
		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions. You should
		/// ensure that character velocity is reset back to world-relative velocity here.
		/// </summary>
		override public void LosingControl()
		{
			base.LosingControl ();
			isCrawling = false;
		}

		#endregion
		
		#if UNITY_EDITOR
		
		#region draw inspector
		
		/// <summary>
		/// Draws the inspector.
		/// </summary>
		new public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData == null || movementData.Length < MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}

			// Draw base inspector and copy values
			MovementVariable[] baseMovementData = GroundMovement_CrouchWithSlide.DrawInspector (movementData, ref showDetails, target);
			System.Array.Copy (baseMovementData, movementData, baseMovementData.Length);

			// Crawl Speed
			if (movementData[CrawlSpeedIndex] == null) movementData[CrawlSpeedIndex] = new MovementVariable();
			movementData[CrawlSpeedIndex].FloatValue = EditorGUILayout.Slider(new GUIContent("Crawl Speed", "The speed the character crawls at."), movementData[CrawlSpeedIndex].FloatValue,  0, Mathf.Max(1, movementData[MaxSpeedIndex].FloatValue - 1.0f));

			return movementData;
		}
		
		#endregion
		
		#endif
	}
	
}

