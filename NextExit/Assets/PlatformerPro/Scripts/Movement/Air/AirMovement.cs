#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A wrapper class for handling moving on the ground that proxies the movement function
	/// to the desired implementation.
	/// </summary>
	public class AirMovement : BaseMovement <AirMovement>
	{
		/// <summary>
		/// Cached reference to a flippable gravity.
		/// </summary>
		protected FlippableGravity flippableGravity;

		#region air specific properties and methods

		/// <summary>
		/// Gets a value indicating whether this movement wants to intiate the jump.
		/// </summary>
		/// <value><c>true</c> if this instance can jump; otherwise, <c>false</c>.</value>
		virtual public bool WantsJump()
		{
			return false;
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to control the movement in the air.
		/// Default is false with movement falling back to default air. Override if you want control.
		/// </summary>
		/// <value><c>true</c> if this instance wants control; otherwise, <c>false</c>.</value>
		virtual public bool WantsAirControl()
		{
			return false;
		}

		/// <summary>
		/// Do the jump.
		/// </summary>
		virtual public void DoJump()
		{
			Debug.LogError ("DoJump is not supported by this air movement");
		}

		/// <summary>
		/// Does a jump with overriden values for the key variables. Primarily used to allow
		/// platforms and wall jumps to affect jump height in non-physics based jumps.
		/// </summary>
		virtual public void DoOverridenJump(float newHeight, int jumpCount, bool skipPowerUps = false)
		{
			Debug.LogError ("DoJumpOverride is not supported by this air movement");
		}
		
		/// <summary>
		/// Partially moves the character. A lot of movements (e.g. wall movements) require partial air movement.
		/// This allows them to use the air movement instead of re-implementing the  movement.
		/// </summary>
		/// <param name="moveInX">If set to <c>true</c> move in x.</param>
		/// <param name="moveInY">If set to <c>true</c> move in y.</param>
		/// <param name="xInput">Simulated X input.</param>
		/// <param name="runButton">Simulated Run button.</param>
		/// <summary>
		/// Partially moves the character. A lot of movements (e.g. wall movements) require partial air movement.
		/// This allows them to use the air movement instead of re-implementing the  movement.
		/// </summary>
		virtual public void DoOverridenMove(bool moveInX, bool moveInY, float xInput, ButtonState runButton)
		{
			int xInputDigital = (xInput > 0) ? 1 : ((xInput < 0) ? -1 : 0);
			if (moveInX) MoveInX(xInput, xInputDigital, runButton);
			if (moveInY) MoveInY();
		}

		/// <summary>
		/// Moves in x.
		/// </summary>
		/// <param name="horizontalAxis">Horizontal axis.</param>
		/// <param name="horizontalAxisDigital">Horizontal axis digital.</param>
		/// <param name="runButton">Run button.</param>
		virtual protected void MoveInX (float horizontalAxis, int horizontalAxisDigital, ButtonState runButton)
		{
		}
		
		/// <summary>
		/// Moves in y.
		/// </summary>
		virtual protected void MoveInY ()
		{
		}

		/// <summary>
		/// Add handler for the gravity flip event
		/// </summary>
		virtual protected void AddGravityFlipHandler()
		{
			flippableGravity = character.GetComponent<FlippableGravity> ();
			if (flippableGravity != null) flippableGravity.GravityFlipped += HandleGravityFlipped;
		}

		/// <summary>
		/// Handles the gravity being flipped.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		virtual protected void HandleGravityFlipped (object sender, System.EventArgs e)
		{
		}

		#endregion

		#region Unity hooks

		/// <summary>
		/// Unity on destroy hook. Clean up event handlers.
		/// </summary>
		void OnDestroy()
		{
			if (flippableGravity != null) flippableGravity.GravityFlipped -= HandleGravityFlipped;
		}

		#endregion
		#region movement info constants and properties
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Air Movement";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "The base air movement class, you shouldn't be seeing this did you forget to create a new MovementInfo?";
		
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
		/// Gets the ground speed.
		/// </summary>
		/// <returns>The ground speed.</returns>
		/// <param name="baseSpeed">Base speed.</param>
		virtual public float GetSpeed(float baseSpeed)
		{
			float multiplier = 1.0f;
			// Apply weilded items
			// Apply upgrades
			// Apply power-ups
			if (character.PowerUpManager != null)
			{
				foreach (string itemId in character.PowerUpManager.ActivePowerUps)
				{
					ItemTypeData typeData = ItemTypeManager.Instance.GetTypeData (itemId);
					multiplier *= typeData.moveSpeedMultiplier;
				}
			}
			return baseSpeed * multiplier;
		}

		/// <summary>
		/// Gets the run speed.
		/// </summary>
		/// <returns>The run speed.</returns>
		/// <param name="baseSpeed">Base run speed.</param>
		virtual public float GetRunSpeed(float baseRunSpeed)
		{
			// TODO
			float multiplier = 1.0f;
			// Apply weilded items
			// Apply upgrades
			// Apply power-ups
			if (character.PowerUpManager != null)
			{
				foreach (string itemId in character.PowerUpManager.ActivePowerUps)
				{
					ItemTypeData typeData = ItemTypeManager.Instance.GetTypeData (itemId);
					multiplier *= typeData.runSpeedMultiplier;
				}
			}
			return baseRunSpeed * multiplier;

		}

		/// <summary>
		/// Gets the acceleration.
		/// </summary>
		/// <returns>The acceleration.</returns>
		/// <param name="baseSpeed">Base acceleration.</param>
		virtual public float GetAcceleration(float baseAcceleration)
		{
			// TODO
			float multiplier = 1.0f;
			// Apply weilded items
			// Apply upgrades
			// Apply power-ups
			if (character.PowerUpManager != null)
			{
				foreach (string itemId in character.PowerUpManager.ActivePowerUps)
				{
					ItemTypeData typeData = ItemTypeManager.Instance.GetTypeData (itemId);
					multiplier *= typeData.accelerationMultiplier;
				}
			}
			return baseAcceleration * multiplier;
		}

		/// <summary>
		/// Gets the jump height or the jump force.
		/// </summary>
		/// <returns>The acceleration.</returns>
		/// <param name="baseSpeed">Base acceleration.</param>
		virtual public float GetJumpHeightOrForce(float baseHeight)
		{
			// TODO
			float multiplier = 1.0f;
			// Apply weilded items
			// Apply upgrades
			// Apply power-ups
			if (character.PowerUpManager != null)
			{
				foreach (string itemId in character.PowerUpManager.ActivePowerUps)
				{
					ItemTypeData typeData = ItemTypeManager.Instance.GetTypeData (itemId);
					multiplier *= typeData.jumpHeightMultiplier;
				}
			}
			return baseHeight * multiplier;
		}

		/// <summary>
		/// Gets the double jump height or the double jump force.
		/// </summary>
		/// <returns>The acceleration.</returns>
		/// <param name="baseSpeed">Base acceleration.</param>
		virtual public float GetDoubleJumpHeightOrForce(float baseHeight)
		{
			// TODO
			float multiplier = 1.0f;
			// Apply weilded items
			// Apply upgrades
			// Apply power-ups
			if (character.PowerUpManager != null)
			{
				foreach (string itemId in character.PowerUpManager.ActivePowerUps)
				{
					ItemTypeData typeData = ItemTypeManager.Instance.GetTypeData (itemId);
					multiplier *= typeData.jumpHeightMultiplier;
				}
			}
			return baseHeight * multiplier;
		}

#if UNITY_EDITOR

		/// <summary>
		/// Draws an inspector with the standard jump details settings.
		/// </summary>
		/// <param name="movementData">Movement data.</param>
		/// <param name="jumpGravityIndex">Jump gravity index.</param>
		/// <param name="groundedLeewayIndex">Grounded leeway index.</param>
		/// <param name="jumpWhenButtonHeldIndex">Jump when button held index.</param>
		public static MovementVariable[] DrawStandardJumpDetails(MovementVariable[] movementData, int jumpRelativeGravityIndex, int groundedLeewayIndex, int jumpWhenButtonHeldIndex)
		{
			// Relative jump gravity
			if (movementData[jumpRelativeGravityIndex] == null || movementData[jumpRelativeGravityIndex].FloatValue < 0.01f || movementData[jumpRelativeGravityIndex].FloatValue > 2.0f)
			{
				movementData[jumpRelativeGravityIndex] = new MovementVariable(1.0f);
			}
			movementData[jumpRelativeGravityIndex].FloatValue = EditorGUILayout.Slider(new GUIContent("Jump Gravity", "Set a value lower than 1 to make the jump floatier as seen in games like Super Mario Bros."), movementData[jumpRelativeGravityIndex].FloatValue, 0.01f, 2.0f);
			
			// Grounded Leeway
			if (movementData[groundedLeewayIndex] == null)
			{
				movementData[groundedLeewayIndex] = new MovementVariable(0.1f);
			}
			movementData[groundedLeewayIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Grounded Leeway", "After the character leaves the ground it will still be able to jump if you press jump within this timeframe."), movementData[groundedLeewayIndex].FloatValue);
			
			// Jump When Button Held Index
			if (movementData[jumpWhenButtonHeldIndex] == null) movementData[jumpWhenButtonHeldIndex] = new MovementVariable();
			movementData[jumpWhenButtonHeldIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Jump When Button Held", "Does holding the jump button jump automatically or does the user need to press it each time."), movementData[jumpWhenButtonHeldIndex].BoolValue);

			return movementData;
		}

#endif

		#endregion
	}
	
}