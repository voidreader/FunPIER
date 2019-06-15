using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerPro
{

    /// <summary>
    /// A trigger which checks a condition and enters when condition is met, then exits when condition is not met.
    /// </summary>
    public class ConditionTrigger : Trigger
    {
        [Header ("Player")]
        /// <summary>
        /// The player identifier.
        /// </summary>
        public int playerId = -1;

        /// <summary>
        /// If a new character gets loaded in the same player id should state stay as it is or be reset to false.
        /// </summary>
        public bool resetOnCharacterLoad;

        /// <summary>
        /// Map of characters to player ids.
        /// </summary>
        protected Dictionary<int, Character> characters;

        /// <summary>
        /// Map of states to player id.
        /// </summary>
        protected Dictionary<int, bool> stateMap;

        /// <summary>
        /// Gets the header.
        /// </summary>
        override public string Header
        {
            get
            {
                return "A trigger which checks a condition and enters when condition is met, then exits when condition is not met";
            }
        }

        /// <summary>
        /// Init this instance.
        /// </summary>
        override protected void PostInit()
        {
            base.PostInit();
            characters = new Dictionary<int, Character>();
            stateMap = new Dictionary<int, bool>();
            PlatformerProGameManager.Instance.CharacterLoaded += HandleCharacterLoaded;
        }


        /// <summary>
        /// Handles the character loaded event.
        /// </summary>
        void HandleCharacterLoaded(object sender, CharacterEventArgs e)
        {
            if (playerId == -1 || e.PlayerId == playerId)
            { 
                if (characters.ContainsKey(e.PlayerId))
                {
                    characters[e.PlayerId] = e.Character;
                    if (resetOnCharacterLoad) stateMap[e.PlayerId] = false;
                }
                else
                {
                    characters.Add(e.PlayerId, e.Character);
                    stateMap.Add(e.PlayerId, false);
                }
            }
        }

        /// <summary>
        /// Unity update hook. Check for activation.
        /// </summary>
        void Update()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                foreach (var c in characters)
                {
                    CheckCharacter(c);
                }
            }
#else
            foreach (var c in characters)
            {
                CheckCharacter(c);
            }
#endif
        }

        /// <summary>
        /// Checks the character (with associated id) is triggering conditions.
        /// </summary>
        /// <param name="kvp">Kvp.</param>
        virtual protected void CheckCharacter(KeyValuePair<int, Character> kvp)
        { 
            if (stateMap[kvp.Key])
            {
                if (!ConditionsMet(kvp.Value))
                {
                    LeaveTrigger(kvp.Value);
                    stateMap[kvp.Key] = false;
                }
            }
            else
            {
                if (ConditionsMet(kvp.Value))
                {
                    EnterTrigger(kvp.Value);
                    stateMap[kvp.Key] = true;
                }
            }
        }

        /// <summary>
        /// Enters the trigger.
        /// </summary>
        /// <returns><c>true</c>, if trigger was entered, <c>false</c> otherwise.</returns>
        /// <param name="character">Character.</param>
        override protected bool EnterTrigger(Character character)
        {
            DoEnterTrigger(character);
            // Let each condition know it was activated
            foreach (AdditionalCondition condition in conditions)
            {
                condition.Activated(character, this);
            }
            return true;
        }

        /// <summary>
        /// Character leaves the trigger.
        /// </summary>
        /// <param name="character">Character. NOTE: This can be null if triggered by something that is not a character.</param>
        override protected bool LeaveTrigger(Character character)
        {
            DoLeaveTrigger(character);
            return true;
        }
    }
}