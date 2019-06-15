using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A wrapper class for handling moving against a wall that proxies the movement function
	/// to the desired implementation.
	/// </summary>
	public class WallMovement : BaseMovement <WallMovement>
	{

		#region wall specific properties and methods
		
		/// <summary>
		/// Gets a value indicating whether this movement wants to intiate a wall clinging behaviour.
		/// </summary>
		virtual public bool WantsCling()
		{
			return false;
		}

		// <summary>
		/// Does the cling.
		/// </summary>
		virtual public void DoCling()
		{

		}

		/// <summary>
		/// If true this applies to all walls and there is no need to use tags or layers to detect walls.
		/// </summary>
		virtual public bool ClingToAllWalls
		{
			get; protected set;
		}

		/// <summary>
		/// Should we find walls by tag or by layer. If true we will use tags, if false we will use layers. Tags are easier to use
		/// but come with an allocation cost.
		/// </summary>
		virtual public bool DetectWallsByTag
		{
			get; protected set;
		}
		
		/// <summary>
		/// The name of the wall layer if we find by layers, or the tag name if we find by tags.
		/// </summary>
		virtual public string WallLayerOrTagName
		{
			get; protected set;
		}

		
		/// <summary>
		/// The number of colliders required for wall clinging to intiate. Note these colliders don't need to be hitting
		/// the same wall just a wall on the same side (so you can move up a wall made of tiled boxes).
		/// </summary>
		virtual public int RequiredColliders
		{
			get; protected set;
		}

		
		/// <summary>
		/// Gets a value indicating whether this <see cref="PlatformerPro.Movement"/> expects the
		/// rotations to be calculated and applied by the character. By default walls don't do this.
		/// </summary>
		override public bool ShouldDoRotations
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region movement info constants and properties
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Wall Movement";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "The base wall movement class, you shouldn't be seeing this did you forget to create a new MovementInfo?";

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
		/// The index of the property for the required colliders.
		/// /// </summary>
		protected const int RequiredCollidersIndex = 1;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 2;

		/// <summary>
		/// The index of the property for the required colliders.
		/// /// </summary>
		protected const int DefaultRequiredColliders = 2;

		#endregion
	}

}