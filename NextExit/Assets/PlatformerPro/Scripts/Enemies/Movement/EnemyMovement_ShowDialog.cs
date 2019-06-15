using UnityEngine;
using System.Collections;
using PlatformerPro.Extras;

namespace PlatformerPro
{
	public class EnemyMovement_ShowDialog : EnemyMovement, ICompletableMovement
	{
		/// <summary>
		/// The dialog to show.
		/// </summary>
		public UIDialog dialog;

		/// <summary>
		/// Are we showing?
		/// </summary>
		protected bool showing;

		/// <summary>
		/// Are we done showing and hiding?
		/// </summary>
		protected bool done;

		/// <summary>
		/// Shows the dialog.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		override public bool DoMove()
		{
			if (showing)
			{
				if (!done && !dialog.Visible)
				{
					enemy.MovementComplete();
					dialog.HideDialog();
					done = true;
				}
			}
			else if (!done)
			{
				dialog.ShowDialog();
				showing = true;
			}
			return true;
		}

		/// <summary>
		/// Called when this movement is losing control.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		override public bool LosingControl()
		{
			if (dialog.Visible) dialog.HideDialog ();
			showing = false;
			done = false;
			return false;
		}
	}
}