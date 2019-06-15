using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Encapsulates the various states of a button.
	/// </summary>
	public enum ButtonState 
	{
		NONE	 	=	0,		// State of the button is not pressed or unknown.
		DOWN		=	1,		// The button was pressed in this frame.
		HELD		=	2,		// The button is being held down.
		UP			=	4,		// The button was released this frame.
		ANY			=	DOWN | HELD | UP
	}

}