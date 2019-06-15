using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Activation event arguments.
	/// </summary>
	public class ActivationEventArgs : System.EventArgs
	{

		/// <summary>
		/// Gets the group name
		/// </summary>
		public string GroupName
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the item id.
		/// </summary>
		virtual public string ItemId 
		{
			get;
			protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.ActivationEventArgs"/> class.
		/// </summary>
		/// <param name="groupName">Group name.</param>
		/// <param name="itemId">Item identifier.</param>
		public ActivationEventArgs(string groupName, string itemId)
		{
			GroupName = groupName;
			ItemId = itemId;
		}

	}
	
}
