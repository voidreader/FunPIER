using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Air movement which engages a jetpack on the second press of the jump button similar to a double jump.
	/// </summary>
	public class AirMovement_DoubleJumpJetpack : AirMovement_JetPack
	{
		/// <summary>
		/// Track if a normal jump has started.
		/// </summary>
		protected bool jumpStarted;

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Special/JetPack/JetpackAsDoubleJump";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Air movement which engages a jetpack on the second press of the jump button similar to a double jump (you will also need another " +
			" Air Movement to handle jump).";
		
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

		#endregion

		/// <summary>
		/// Unity update hook. Reset jump button started if character releases the button.
		/// </summary>
		void Update()
		{
			// Ready to start jetpack once button released
			if (jumpStarted && character.Input.JumpButton == ButtonState.NONE) jumpStarted = false;
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to intiate the jump. In this case its always false.
		/// </summary>
		/// <value>true</value>
		/// <c>false</c>
		/// <returns><c>true</c>, if jump was wanted, <c>false</c> otherwise.</returns>
		override public bool WantsJump()
		{
			if (!enabled) return false;
			// Avoid infinite loop in case user fogets to add a default jump movement.
#if UNITY_EDITOR
			if (character.DefaultAirMovement == this)
			{
				Debug.LogWarning("Jetpack as double jump CANNOT be the default AirMovement. Make sure you add another AirMovement as the default.");
				return false;
			}
#endif
			// We are about to start the default jump
			if (character.DefaultAirMovement.WantsJump()) jumpStarted = true;
			return false;
		}

		/// <summary>
		/// Is the jet pack engaged.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		override public bool IsJetPackEngaged()
		{
			// Jump down then don't take control
			if (jumpStarted) return false;
			// Else check button and fuel
			if (jetpackFuelConsumption <= 0.0f || itemManager.ItemCount(fuelItemType) > 0)
			{
				if (character.Input.JumpButton == ButtonState.HELD) return true;
			}
			return false;
		}
#if UNITY_EDITOR
		/// <summary>
		/// Draws the inspector.
		/// </summary>
		new public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			// Same as base class.
			return AirMovement_JetPack.DrawInspector (movementData, ref showDetails, target); 
		}
#endif
	}
}
