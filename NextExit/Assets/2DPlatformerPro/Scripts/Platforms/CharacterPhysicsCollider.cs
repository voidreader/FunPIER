using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A special collider used to handle triggers and physics interaction.
	/// </summary>
	public class CharacterPhysicsCollider : MonoBehaviour
	{
	
		/// <summary>
		/// Reference to this character.
		/// </summary>
		protected Character myCharacter;

		/// <summary>
		/// Gets the character reference.
		/// </summary>
		virtual public Character Character
		{
			get
			{
				return myCharacter;
			}
		}

		/// <summary>
		/// Unity start hook
		/// </summary>
		void Start()
		{
			Collider2D myCollider = GetComponent<Collider2D>();
			if (myCollider == null) Debug.LogError("CharacterPhysicsCollider must have a 2D collider attached.");
			Rigidbody2D myRigidbody = GetComponent<Rigidbody2D>();
			if (myRigidbody == null) Debug.LogError("CharacterPhysicsCollider must have a 2D rigidbody attached.");

			if (myCollider.isTrigger) 
			{
				myCollider.isTrigger = false;
				Debug.LogError("Collider cannot be a trigger, removing.");
			}

			if (!myRigidbody.isKinematic)
			{
				myRigidbody.isKinematic = true;
				Debug.LogError("Rigidbody must be kinematic, setting.");
			}

			myCharacter = transform.parent.GetComponent<Character>();
			if (myCharacter == null) Debug.LogError("CharacterPhysicsCollider must have a parent with a Character attached.");

			if ((1 << gameObject.layer & Character.layerMask) == 1 << gameObject.layer)
			{
				Debug.LogError("CharacterPhysicsCollider cannot be in a layer that is used by the character for collisions. ( <b> LINK </b>)");
			}
		}
	}
}
