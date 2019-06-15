using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Different types of damage that can be dealt. It includes some common types and some CUSTOM_X types
	/// which you can use for your own. There's also a sample name generator to show how you could use an
	/// extension method to easily name your own damage types.
	/// </summary>
	public enum DamageType
	{
		NONE,
		PHYSICAL,
		FIRE,
		COLD,
		ELECTRICAL,
		POISON,
		MAGIC,
		HOLY, 
		DEMONIC,
		HEAD_STOMP,
		PLATFORM_BOBBLE,
		CUSTOM_1,
		CUSTOM_2,
		CUSTOM_3,
		AUTO_KILL,
		TIME_EXPIRED
	}

	/// <summary>
	/// Just a sample showing you how you could create your own names for custom damage types.
	/// </summary>
	public static class DamageTypeExtension
	{
		/// <summary>
		/// Sample extension method.
		/// </summary>
		/// <returns>The type as string.</returns>
		/// <param name="type">Type.</param>
		public static string AsString(this DamageType type)
		{
			switch(type)
			{
			case DamageType.CUSTOM_1 : return "VENGFUL";
			case DamageType.CUSTOM_2 : return "MALOVENT";
			default					 : return type.ToString();
			}
		}
	}
}
