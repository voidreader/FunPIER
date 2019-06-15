
namespace PlatformerPro
{
	/// <summary>
	/// Button event arguments.
	/// </summary>
	public class ButtonEventArgs : System.EventArgs
	{
		/// <summary>
		/// Gets the button state.
		/// </summary>
		public ButtonState State 
		{
			get;
			protected set;
		}

		/// <summary>
		/// Updates the button event arguments.
		/// </summary>
		/// <param name="state">New state.</param>
		virtual public void UpdateButtonState(ButtonState state)
		{
			State = state;
		}
		
	}
	
}
