namespace PlatformerPro
{
	/// <summary>
	/// The types of event responses.
	/// </summary>
	public enum EventResponseType
	{
		DEBUG_LOG,
		SEND_MESSSAGE,
		ACTIVATE_GAMEOBJECT,
		DEACTIVATE_GAMEOBJECT,
		ENABLE_BEHAVIOUR,
		DISABLE_BEHAVIOUR,
		OVERRIDE_ANIMATON,
		CLEAR_ANIMATION_OVERRIDE,
		PLAY_PARTICLES,
		PAUSE_PARTICLES,
		SWITCH_SPRITE,
		PLAY_SFX,
		PLAY_SONG,
		STOP_SONG,
		MAKE_INVULNERABLE,
		MAKE_VULNERABLE,
		LEVEL_COMPLETE,
		LOAD_SCENE,
		LOCK,
		UNLOCK,
		RESPAWN,
		SET_ACTIVE_RESPAWN,
		START_EFFECT,
		FLIP_GRAVITY,
		SPECIAL_MOVE_ANIMATION,
		START_SWIM,
		STOP_SWIM,
		ADD_SCORE,
		RESET_SCORE,
		PLAY_ANIMATION,
		STOP_ANIMATION,
		ADD_LIVES,
		UPDATE_MAX_HEALTH,
		SET_MAX_HEALTH,
		UPDATE_MAX_LIVES,
		SET_MAX_LIVES,
		SET_TAGGED_PROPERTY,
		ADD_TO_TAGGED_PROPERTY,
		MULTIPLY_TAGGED_PROPERTY,
		SPAWN_ITEM,
		ADD_VELOCITY,
		SET_VELOCITY,
		SET_DEPTH,
		POWER_UP,
		RESET_POWER_UP,
		COLLECT_ITEM,
		CONSUME_ITEM,
		HEAL,
		DAMAGE,
		KILL,
		SWIM_SURFACE,
		ACTIVATE_ITEM,
		DEACTIVATE_ITEM,
        SET_CHARACTER,
        CLEAR_RESPAWN_POINTS,
        END_SWIM_SURFACE
	}

	public static class EventResponseExtensions {
		public static string GetName(this EventResponseType me) {
			switch (me)
			{
			case EventResponseType.DEBUG_LOG: return "General/Debug Log";
			case EventResponseType.SEND_MESSSAGE: return "General/Send Message";

			case EventResponseType.ACTIVATE_GAMEOBJECT: return "Activation/Activate GameObject";
			case EventResponseType.DEACTIVATE_GAMEOBJECT: return "Activation/Deactivate GameObject";
			case EventResponseType.ENABLE_BEHAVIOUR: return "Activation/Enable Behaviour";
			case EventResponseType.DISABLE_BEHAVIOUR: return "Activation/DisableBehaviour";

			case EventResponseType.OVERRIDE_ANIMATON: return "Animation/Animation Override";
			case EventResponseType.CLEAR_ANIMATION_OVERRIDE: return "Animation/Clear Animation Override";
			case EventResponseType.PLAY_ANIMATION: return "Animation/Play Animation";
			case EventResponseType.STOP_ANIMATION: return "Animation/Stop Animation";
			case EventResponseType.SWITCH_SPRITE: return "Animation/Switch Sprite";

			case EventResponseType.PLAY_PARTICLES: return "Effects/Particles/Play Particles";
			case EventResponseType.PAUSE_PARTICLES: return "Effects/Particles/Pause Particles";

			case EventResponseType.START_EFFECT: return "Effects/FX System/Start Effect";

			case EventResponseType.PLAY_SFX: return "Effects/Sound/Play SFX";
			case EventResponseType.PLAY_SONG: return "Effects/Sound/Play Song";
			case EventResponseType.STOP_SONG: return "Effects/Sound/Stop Song";

			case EventResponseType.MAKE_INVULNERABLE: return "Character/Health/Make Invulnerable";
			case EventResponseType.MAKE_VULNERABLE: return "Character/Health/Make Vulnerable";
			case EventResponseType.HEAL: return "Character/Health/Heal";
			case EventResponseType.DAMAGE: return "Character/Health/Damage";
			case EventResponseType.KILL: return "Character/Health/Kill";

			case EventResponseType.ADD_LIVES: return "Character/Health/Add (or Remove) Lives";
			case EventResponseType.UPDATE_MAX_HEALTH: return "Character/Health/Add (or Remove) Max Health";
			case EventResponseType.SET_MAX_HEALTH: return "Character/Health/Set Max Health";
			case EventResponseType.UPDATE_MAX_LIVES: return "Character/Health/Add (or Remove) Max Lives";
			case EventResponseType.SET_MAX_LIVES: return "Character/Health/Set Max Lives";
            case EventResponseType.SET_CHARACTER: return "Character/Set Character";

			case EventResponseType.START_SWIM: return "Character/Movement/Start Swim";
			case EventResponseType.STOP_SWIM: return "Character/Movement/Stop Swim";
			case EventResponseType.SWIM_SURFACE: return "Character/Movement/Swim On Surface";
            case EventResponseType.END_SWIM_SURFACE: return "Character/Movement/End Swim On Surface";
            case EventResponseType.ADD_VELOCITY: return "Character/Movement/Add Velocity";
			case EventResponseType.SET_VELOCITY: return "Character/Movement/Set Velocity";

			case EventResponseType.SET_TAGGED_PROPERTY: return "Character/Tagged Properties/Set Tagged property";
			case EventResponseType.ADD_TO_TAGGED_PROPERTY: return "Character/Tagged Properties/Add to Tagged Properery";
			case EventResponseType.MULTIPLY_TAGGED_PROPERTY: return "Character/Tagged Properties/Multiply Tagged Property";

			case EventResponseType.SPECIAL_MOVE_ANIMATION: return "Animation/Special Move Animation";

			case EventResponseType.COLLECT_ITEM: return "Character/Items/Collect Item";
			case EventResponseType.CONSUME_ITEM: return "Character/Items/Consume Item";
				
			case EventResponseType.POWER_UP: return "Character/Power-Up/Grant Power-Up";
			case EventResponseType.RESET_POWER_UP: return "Character/Power-Up/Reset Power-Up";

			case EventResponseType.SPAWN_ITEM: return "Spawner/Spawn Item";

			case EventResponseType.SET_DEPTH: return "Character/Special/Set Depth";
			case EventResponseType.FLIP_GRAVITY: return "Character/Special/Flip Gravity";
			case EventResponseType.RESPAWN: return "Character/Respawn/Respawn";

			case EventResponseType.LEVEL_COMPLETE: return "Level/Level Complete";
			case EventResponseType.LOAD_SCENE: return "Level/Load Scene";
			case EventResponseType.LOCK: return "Level/Lock Level";
			case EventResponseType.UNLOCK: return "Level/Unlock Level";	
			case EventResponseType.SET_ACTIVE_RESPAWN: return "Level/Set Active Respawn";
            case EventResponseType.CLEAR_RESPAWN_POINTS: return "Level/Clear Respawn Points";

            case EventResponseType.ADD_SCORE: return "Score/Add Score";
			case EventResponseType.RESET_SCORE: return "Score/Reset Score";

			case EventResponseType.ACTIVATE_ITEM: return "Activation Group/Activate";
			case EventResponseType.DEACTIVATE_ITEM: return "Activation Group/Deactivate";
			}			
			return "Other/" + me;
		}
		public static string GetDescription(this EventResponseType me) {
			return "";
		}
	}
}