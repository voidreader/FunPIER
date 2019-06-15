using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PlatformerPro
{
	/// <summary>
	/// Enumeration of all supported animation states.
	/// </summary>
	public enum AnimationState
	{
		NONE			=		-1,

		IDLE			=		000,
		IDLE_ARMED		=		001,
		IDLE_ALT0		=		010,
		IDLE_ALT1		=		011,
		IDLE_ALT2		=		012,

		WALK			=		100,
		SLIDE			=		105,
		SLIDE_DIR_CHANGE=		106,
		RUN				=		110,
		CROUCH			=		120,
		CROUCH_WALK		=		130,
		ROLL			=		131,
		CROUCH_SLIDE	=		132,

		PUSH			=		140,
		PULL			=		150,

		STAIRS_UP		=		160,
		STAIRS_UP_RIGHT	=		161,
		STAIRS_UP_RIGHT_IDLE =	162,
		STAIRS_UP_LEFT	=		163,
		STAIRS_UP_LEFT_IDLE	=	164,
		STAIRS_DOWN		=		165,
		STAIRS_DOWN_RIGHT =		166,
		STAIRS_DOWN_RIGHT_IDLE=	167,
		STAIRS_DOWN_LEFT	=	168,
		STAIRS_DOWN_LEFT_IDLE =	169,
		STAIRS_DOWN_TO_GROUND =	170,
		STAIRS_UP_TO_GROUND_RIGHT =	171,
		STAIRS_UP_TO_GROUND_LEFT = 172,

		JUMP			=		200,
		JUMP_CROUCH		=		201,
		DOUBLE_JUMP		=		205,
		AIRBORNE		=		210,
		AIRBORNE_CROUCH	=		211,
		FALL			=		220,
		FALL_CROUCH		=		221,
		JETPACK			=		230,
		FLY				=		240,
		FLOAT			=		250,
		GLIDE			=		260,

		CLIMB_HOLD		=		300,
		CLIMB_UP		=		310,
		CLIMB_DOWN		=		320,
		ROPE_CLIMB_HOLD =		330,
		ROPE_CLIMB_UP	=		340,
		ROPE_CLIMB_DOWN	=		350,
		ROPE_SWING		=		360,
		CLIMB_LEFT		=		370,
		CLIMB_RIGHT		=		380,

		WALL_CLING		=		400,
		WALL_SLIDE		=		410,
		WALL_CLIMB_UP	=		420,
		WALL_CLIMB_DOWN	=		430,
		WALL_JUMP		=		440,

		LEDGE_REACH		=		500,
		LEDGE_GRASP 	= 		510,
		LEDGE_HANG		=		520,
		LEDGE_FALL		=		530,
		LEDGE_CLIMB		=		540,
		LEDGE_DONE		=		550,
		LEDGE_DISMOUNT  =		560,
		LEDGE_DISMOUNT_DONE  =	570,

		SWIM			=		600,
		SWIM_STROKE		=		610,
		SWIM_ALT_0		=		620,
		SWIM_WALK		=		630,
		SWIM_ENTER		=		640,
		SWIM_EXIT		=		650,

		GRAPPLE_THROW	=		700,
		GRAPPLE_THROW_45=		701,
		GRAPPLE_THROW_90=		702,
		GRAPPLE_SWING	=		710,
		GRAPPLE_HANG	=		720,

		CEILING_HANG	=		800,
		CEILING_CLIMB	=		810,
		CEILING_CLIMB_LAUNCH =	820,

		STUNNED			=		1000,
		HURT_NORMAL		=		1001,
		HURT_HIGH		=		1002,
		HURT_LOW		=		1003,
		HURT_BACK		=		1004,
		HURT_OTHER_1	=		1005,
		HURT_OTHER_2	=		1006,
		HURT_OTHER_3	=		1007,

		ATTACK			=		1500,
		ATTACK_PUNCH	=		1501,
		ATTACK_KICK		=		1502,
		ATTACK_SLASH	=		1503,
		ATTACK_STAB		=		1504,
		ATTACK_AIR		=		1505,
		ATTACK_SHOOT	=		1506,
		ATTACK_SHOOT_UP	=		1507,
		ATTACK_THROW	=		1508,

		ATTACK_CUSTOM0	=		1550,
		ATTACK_CUSTOM1	=		1551,
		ATTACK_CUSTOM2	=		1552,
		ATTACK_CUSTOM3	=		1553,
		ATTACK_CUSTOM4	=		1554,

		ATTACK_WEILD0	=		1560,
		ATTACK_WEILD1	=		1561,
		ATTACK_WEILD2	=		1562,
		ATTACK_WEILD3	=		1563,
		ATTACK_WEILD4	=		1564,

		POWER_BOMB		=		1570,
		POWER_BOMB_CHARGE=		1571,
		POWER_BOMB_LAND	=		1572,

		DEPLOY			=		1600,
		UNDEPLOY		=		1601,
		HIDING			=		1602,

		DEATH			=		2000,

		GAME_OVER		=		2100,

		ACRO_HURDLE		=		3001,
		ACRO_CAT_GRAB	=		3002,
		ACRO_LEDGE_CLIMB=		3003,
		ACRO_DIVE		=		3004,
		ACRO_FRONT_FLIP =		3005,
		ACRO_BACK_FLIP	=		3006,
		ACRO_SIDE_FLIP	=		3007,
		ACRO_GAINER		=		3008,
		ACRO_VAULT		=		3009,
		ACRO_SPIN		=		3010,

		ACRO_CUSTOM_0	=		3100,
		ACRO_CUSTOM_1	=		3101,
		ACRO_CUSTOM_2	=		3102,
		ACRO_CUSTOM_3	=		3103,
		ACRO_CUSTOM_4	=		3104,

		GRAB			=		4000,
		GRAB_HIGH		=		4001,
		GRAB_LOW		=		4002,
		PICKUP			=		4003,
		PICKUP_ALT		=		4004,
		PRESS			=		4005,

		INTERACTION_0	=		4100,
		INTERACTION_1	=		4101,
		INTERACTION_2	=		4102,
		INTERACTION_3	=		4103,
		INTERACTION_4	=		4104,

		CUSTOM_0		=		10000,
		CUSTOM_1		=		10001,
		CUSTOM_2		=		10002,
		CUSTOM_3		=		10003,
		CUSTOM_4		=		10004,
		CUSTOM_5		=		10005,
		CUSTOM_6		=		10006,
		CUSTOM_7		=		10007,
		CUSTOM_8		=		10008,
		CUSTOM_9		=		10009

	}

	/// <summary>
	/// Static extensions for Animation State
	/// </summary>
	public static class AnimationStateExtensions
	{
		/// <summary>
		/// Dictionary of the states as strings.
		/// </summary>
		static Dictionary<int, string> animationStatesAsStrings;

		/// <summary>
		/// Initializes the dictionary.
		/// </summary>
		static AnimationStateExtensions()
		{
			animationStatesAsStrings = new Dictionary<int, string>();
			foreach (AnimationState value in System.Enum.GetValues(typeof(AnimationState)))
			{
				animationStatesAsStrings.Add ((int)value, System.Enum.GetName(typeof(AnimationState), value));
			}
		}

		///<summary
		/// AsString() is a ToString() like method that doesn't allocate in heap space.
		/// </summary>
		public static string AsString(this AnimationState state)
		{
			return animationStatesAsStrings[(int)state];
		}
	}
}

