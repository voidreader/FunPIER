using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Damage info event arguments which also includes an attakc name on top of the damage type.
	/// </summary>
	public class ExtraDamageInfoEventArgs : DamageInfoEventArgs
	{
		/// <summary>
		/// Extra info. The name of the attack.
		/// </summary>
		public string AttackName
		{
			get; 
			protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.ExtraDamageInfoEventArgs"/> class.
		/// </summary>
		public ExtraDamageInfoEventArgs() : base()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.ExtraDamageInfoEventArgs"/> class.
		/// </summary>
		/// <param name="damageInfo">Damage info.</param>
		public ExtraDamageInfoEventArgs(DamageInfo damageInfo) : base(damageInfo)
		{
			DamageInfo = damageInfo;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.ExtraDamageInfoEventArgs"/> class.
		/// </summary>
		/// <param name="damageInfo">Damage info.</param>
		public void UpdateDamageInfoEventArgs(DamageInfo damageInfo, string attackName)
		{
			base.UpdateDamageInfoEventArgs (damageInfo);
			AttackName = attackName;
		}

		override public string ToString()
		{
			return string.Format("DamageInfo - {0} {1} by {2} - {3}", DamageInfo.Amount, DamageInfo.DamageType, DamageInfo.DamageCauser, AttackName);
		}
	}
	
}