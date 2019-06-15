using UnityEditor;
using UnityEngine;
using System.Collections;

namespace PlatformerPro.Validation
{
	/// <summary>
	/// Validation result.
	/// </summary>
	public class ValidationResult
	{
		/// <summary>
		/// The type of the message.
		/// </summary>
		public MessageType messageType;

		/// <summary>
		/// The message.
		/// </summary>
		public string message;

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.Validation.ValidationResult"/> class.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="messageType">Message type.</param>
		public ValidationResult(string message, MessageType messageType)
		{
			this.message = message;
			this.messageType = messageType;
		}
	}
}
