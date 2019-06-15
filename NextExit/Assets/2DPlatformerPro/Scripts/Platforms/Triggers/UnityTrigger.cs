using UnityEngine;
using System.Collections;

namespace PlatformerPro
{

	/// <summary>
	/// Trigger class that uses Unity triggers. Requires a collider on the character.
	/// </summary>
	[ExecuteInEditMode]
	public class UnityTrigger : Trigger {

		/// <summary>
		/// Unity enable hook
		/// </summary>

		void OnEnable()
		{
			if (!Application.isPlaying)
			{
				if (GetComponent<Collider2D>() == null)
				{
					BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
					boxCollider.isTrigger = true;
				}
			}
		}

		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start () {
			Init();
		}

		/// <summary>
		/// Unity 2D trigger hook
		/// </summary>
		/// <param name="other">Other.</param>
		void OnTriggerEnter2D(Collider2D other)
		{
			Character character = null;
			CharacterReference characterReference = other.GetComponent<CharacterReference> ();
			if (characterReference != null) character = characterReference.Character;
			if (character == null) character = other.GetComponentInParent<Character> ();
			EnterTrigger(character);
		}

		/// <summary>
		/// Unity 2D trigger hook
		/// </summary>
		/// <param name="other">Other.</param>
		void OnTriggerExit2D(Collider2D other)
		{
			Character character = null;
			CharacterReference characterReference = other.GetComponent<CharacterReference> ();
			if (characterReference != null) character = characterReference.Character;
			if (character == null) character = other.GetComponentInParent<Character> ();
			LeaveTrigger(character);
		}

		/// <summary>
		/// Unity gizmo hook, draw the connection.
		/// </summary>
		void OnDrawGizmos()
		{
			if (receivers != null)
			{
				Gizmos.color = Trigger.GizmoColor;

				foreach (TriggerTarget receiver in receivers) 
				{
					if (receiver != null && receiver.receiver != null) Gizmos.DrawLine(transform.position, receiver.receiver.transform.position);
				}
			}
		}

	}
}