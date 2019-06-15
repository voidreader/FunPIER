using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Attach this to the character sprite (or 3D model) to stop it rotating in the Z axis. This
	/// is behaviour found in many retro games. This script needs to have its execution order set
	/// to be after the main character script (Character.cs).
	/// </summary>
	public class DontRotateSprite : MonoBehaviour {

		/// <summary>
		/// The initial z rotation.
		/// </summary>
		protected float initialRotation;

		/// <summary>
		/// Cached transform reference.
		/// </summary>
		protected Transform myTransform;

		/// <summary>
		/// Cached character reference.
		/// </summary>
		protected Character character;

		/// <summary>
		/// Store the relative offset.
		/// </summary>
		protected Vector3 relativeOffset;

		/// <summary>
		/// Unity start hook, get the starting z position.
		/// </summary>
		void Start () {
			initialRotation = transform.rotation.eulerAngles.z;
			myTransform = transform;
			character = GetComponent<Character> ();
			relativeOffset = transform.localPosition;
			if (character == null) character = GetComponentInParent<Character> ();
			if (character == null) 
			{
				Debug.LogError ("Don't rotate sprite script couldn't find cahracter reference");
			}

		}


	
		/// <summary>
		/// Reset rotation in late update. This script needs to have its execution order set
		/// to be after the main character script (Character.cs).
		/// </summary>
		void LateUpdate () {
			if (character.IsGravityFlipped)
			{
				// TODO We need to apply an additional offset to the rotation before calcualting transform
				myTransform.transform.localPosition = Quaternion.Inverse(character.transform.localRotation) * relativeOffset;
				myTransform.rotation = Quaternion.Euler(myTransform.rotation.eulerAngles.x,myTransform.rotation.eulerAngles.y, initialRotation + 180.0f); 
			}
			else
			{
				myTransform.transform.localPosition = Quaternion.Inverse(character.transform.localRotation) * relativeOffset;
				myTransform.rotation = Quaternion.Euler(myTransform.rotation.eulerAngles.x,myTransform.rotation.eulerAngles.y, initialRotation); 
			}


		}
	}

}