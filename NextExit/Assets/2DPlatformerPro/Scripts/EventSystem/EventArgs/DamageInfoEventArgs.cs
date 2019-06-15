using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Damage info event arguments used for damage, death and game over events.
	/// </summary>
	public class DamageInfoEventArgs : System.EventArgs
	{
		/// <summary>
		/// Gets the damage info.
		/// </summary>
		public DamageInfo DamageInfo 
		{
			get;
			protected set;
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.DamageInfoEventArgs"/> class.
		/// </summary>
		public DamageInfoEventArgs()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.DamageInfoEventArgs"/> class.
		/// </summary>
		/// <param name="damageInfo">Damage info.</param>
		public DamageInfoEventArgs(DamageInfo damageInfo)
		{
			DamageInfo = damageInfo;
		}
		
		/// <summary>
		/// Updates the damage info event arguments.
		/// </summary>
		/// </summary>
		/// <param name="damageInfo">Damage info.</param>
		virtual public void UpdateDamageInfoEventArgs(DamageInfo damageInfo)
		{
			DamageInfo = damageInfo;
		}

		override public string ToString()
		{
			return string.Format("DamageInfo - {0} {1} by {2}", DamageInfo.Amount, DamageInfo.DamageType, DamageInfo.DamageCauser);
		}
	}
	
}