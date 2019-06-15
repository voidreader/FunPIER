using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Animation event arguments.
	/// </summary>
	public class AnimationEventArgs : System.EventArgs
	{
		/// <summary>
		/// Gets the animation state.
		/// </summary>
		public AnimationState State 
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the previous animation state.
		/// </summary>
		/// <value>The state of the previous.</value>
		public AnimationState PreviousState 
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the animation override state which can be maintained separately from base state and can be used
		/// for animation overrides like shooting or carrying.
		/// </summary>
		public string OverrideState 
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the animation priority. An animation with a higher priority will not be interrupted
		/// mid-loop by an animation with a lower priority.
		/// </summary>
		public int Priority 
		{
			get;
			protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AnimationEventArgs"/> class.
		/// </summary>
		/// <param name="state">Current state.</param>
		/// <param name="previousState">Previous state.</param>
		/// <param name="overrideState">Override state.</param>
		public AnimationEventArgs(AnimationState state, AnimationState previousState, string overrideState)
		{
			State = state;
			PreviousState = previousState;
			OverrideState = overrideState;
			Priority = 0;
		}

		/// <summary>
		/// Updates the animation event arguments.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="previousState">Previous state.</param>
		/// <param name="overrideState">Override State.</param>
		virtual public void UpdateAnimationEventArgs(AnimationState state, AnimationState previousState, string overrideState)
		{
			State = state;
			PreviousState = previousState;
			OverrideState = overrideState;
			Priority = 0;
		}

		/// <summary>
		/// Updates the animation event arguments.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="previousState">Previous state.</param>
		/// <param name="overrideState">Override State.</param>
		/// <param name="priority">Priority of the animation.</param>
		virtual public void UpdateAnimationEventArgs(AnimationState state, AnimationState previousState, string overrideState, int priority)
		{
			State = state;
			PreviousState = previousState;
			OverrideState = overrideState;
			Priority = priority;
		}

		/// <summary>
		/// Updates the animation override state.
		/// </summary>
		/// <param name="overrideState">Override State</param>
		virtual public void UpdateAnimationOverrideState(string overrideState)
		{
			OverrideState = overrideState;
		}

	}

}
