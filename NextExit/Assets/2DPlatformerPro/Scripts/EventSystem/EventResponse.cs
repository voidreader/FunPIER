using UnityEngine;

namespace PlatformerPro
{
	
	/// <summary>
	/// Defines a repsonse to an event.
	/// </summary>
	[System.Serializable]
	public class EventResponse
	{
		/// <summary>
		/// Type of response.
		/// </summary>
		public EventResponseType responseType;

		/// <summary>
		/// How long to wait before handling the action.
		/// </summary>
		public float delay;

		/// <summary>
		/// The target game object.
		/// </summary>
		public GameObject targetGameObject;

		/// <summary>
		/// The target component.
		/// </summary>
		public Component targetComponent;

		/// <summary>
		/// The message for send message, or the song name, or the score type.
		/// </summary>
		public string message;

		/// <summary>
		/// The animation override state.
		/// </summary>
		public string overrideState;

		/// <summary>
		/// The new sprite to use.
		/// </summary>
		public Sprite newSprite;

		/// <summary>
		/// The forced animation state.
		/// </summary>
		public AnimationState animationState;

		/// <summary>
		/// Generic int value attached to the response.
		/// </summary>
		public int intValue;

		/// <summary>
		/// Generic Vector value attached to the response.
		/// </summary>
		public Vector2 vectorValue;

		/// <summary>
		/// Generic bool value.
		/// </summary>
		public bool boolValue;

		/// <summary>
		/// Generic float value attached to the response.
		/// </summary>
		public float floatValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.EventResponse"/> class.
		/// </summary>
		public EventResponse()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.EventResponse"/> class by cloning another instance.
		/// </summary>
		/// <param name="original">Original.</param>
		public EventResponse(EventResponse original)
		{
			this.responseType = original.responseType;	
			this.delay = original.delay;
			this.targetGameObject = original.targetGameObject;
			this.targetComponent = original.targetComponent;
			this.overrideState = original.overrideState;
			this.message = original.message;
			this.newSprite = original.newSprite;
			this.animationState = original.animationState;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="PlatformerPro.EventResponse"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="PlatformerPro.EventResponse"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="PlatformerPro.EventResponse"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals (object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals (this, obj))
				return true;
			if (obj.GetType () != typeof(EventResponse))
				return false;
			EventResponse other = (EventResponse)obj;
			return responseType == other.responseType && delay == other.delay && targetGameObject == other.targetGameObject && targetComponent == other.targetComponent && message == other.message && overrideState == other.overrideState && newSprite == other.newSprite && animationState == other.animationState && intValue == other.intValue && vectorValue == other.vectorValue && boolValue == other.boolValue && floatValue == other.floatValue;
		}
		
		/// <summary>
		/// Serves as a hash function for a <see cref="PlatformerPro.EventResponse"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode ()
		{
			unchecked {
				return responseType.GetHashCode () ^ delay.GetHashCode () ^ (targetGameObject != null ? targetGameObject.GetHashCode () : 0) ^ (targetComponent != null ? targetComponent.GetHashCode () : 0) ^ (message != null ? message.GetHashCode () : 0) ^ (overrideState != null ? overrideState.GetHashCode () : 0) ^ (newSprite != null ? newSprite.GetHashCode () : 0) ^ animationState.GetHashCode () ^ intValue.GetHashCode () ^ vectorValue.GetHashCode () ^ boolValue.GetHashCode () ^ floatValue.GetHashCode ();
			}
		}
		

	}
}
