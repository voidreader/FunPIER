using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerPro
{

    /// <summary>
    /// Changes collider sizes based on animation state. Use this component if you want to change the shape of the characters colliders for specific aniamtion states/movements.
    /// </summary>
    public class DynamicColliderResizer : PlatformerProMonoBehaviour
    {
        /// <summary>
        /// The different collider morphs to apply.
        /// </summary>
        public List<ResizeMap> morphs;

        /// <summary>
        /// Maps individual animation states to a morph.
        /// </summary>
        protected Dictionary<AnimationState, List<ColliderResizeInfo>> stateToMorphMap;

        /// <summary>
        /// Maps individual animation states to Unity collider size changes.
        /// </summary>
        protected Dictionary<AnimationState, List<UnityColliderResizeInfo>> stateToUnityColliderMap;

        /// <summary>
        /// Extents of the default colliders.
        /// </summary>
        protected Vector2[] defaultExtents;

        /// <summary>
        /// Lengths of the default colliders.
        /// </summary>
        protected float[] defaultLengths;

        /// <summary>
        /// Maps colliders to their original settings.
        /// </summary>
        protected Dictionary<Collider2D, UnityColliderDetails> defaultUnityColliders;

        /// <summary>
        /// Character reference
        /// </summary>
        /// <value>The character.</value>
        protected Character character;

        /// <summary>
        /// Tracks if we are currently in a morph state.
        /// </summary>
        bool isMorphed;

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>The header.</value>
        override public string Header
        {
            get
            {
                return "Use this component if you want to change the shape of the characters colliders for specific animation states/movements";
            }
        }

        /// <summary>
        /// Unity Awake hook.
        /// </summary>
        void Awake()
        {
            Init();
        }

        /// <summary>
        /// Unity start hook.
        /// </summary>
        void Start()
        {
            PostInit();
        }

        /// <summary>
        /// Init this instance.
        /// </summary>
        virtual protected void Init()
        {
            // Get character
            character = GetComponentInParent<Character>();
            // Set up dictionary
            stateToMorphMap = new Dictionary<AnimationState, List<ColliderResizeInfo>>();
            stateToUnityColliderMap = new Dictionary<AnimationState, List<UnityColliderResizeInfo>>();
            foreach (var m in morphs)
            {
                foreach (var s in m.states)
                {
                    stateToMorphMap.Add(s, m.morph);
                }
                foreach (var s in m.states)
                {
                    stateToUnityColliderMap.Add(s, m.unityColliders);
                }
            }
        }

        /// <summary>
        /// Additional Init performed during start.
        /// </summary>
        virtual protected void PostInit()
        {
            // Get raycast defaults
            defaultExtents = new Vector2[character.Colliders.Length];
            defaultLengths = new float[character.Colliders.Length];
            for (int i = 0;  i < character.Colliders.Length; i++)
            {
                defaultExtents[i] = character.Colliders[i].Extent;
                defaultLengths[i] = character.Colliders[i].RawLength;
            }
            // Get Unity Collider Defaults
            defaultUnityColliders = new Dictionary<Collider2D, UnityColliderDetails>();
            foreach (var m in morphs)
            {
                if (m.unityColliders != null) 
                {
                    foreach (var c in m.unityColliders)
                    {
                        UnityColliderDetails details = new UnityColliderDetails();
                        details.enabled = c.collider.enabled;
                        details.offset = c.collider.offset;
                        if (c.collider is BoxCollider2D)
                        {
                            details.size = ((BoxCollider2D)c.collider).size;
                        } 
                        else if ( c.collider is CircleCollider2D)
                        {
                            details.size = new Vector2(((CircleCollider2D)c.collider).radius, 0);
                        }
                        else if (c.collider is CapsuleCollider2D)
                        {
                            details.size = ((CapsuleCollider2D)c.collider).size;
                        }
                        defaultUnityColliders.Add(c.collider, details);
                    }
                }
            }
            // Listen to events
            character.ChangeAnimationState += HandleChangeAnimationState;

        }

        /// <summary>
        /// Handles the state of the change animation.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void HandleChangeAnimationState(object sender, AnimationEventArgs e)
        {
            if (stateToMorphMap.ContainsKey(e.State))
            {
                for (int i = 0; i < stateToMorphMap[e.State].Count; i++)
                {
                    ApplyMorph(stateToMorphMap[e.State][i]);
                }
                for (int i = 0; i < stateToUnityColliderMap[e.State].Count; i++)
                {
                    ApplyUnityColliderMorph(stateToUnityColliderMap[e.State][i]);
                }
                isMorphed = true;
            }
            else
            {
                if (isMorphed) RestoreToDefaults();
                isMorphed = false;
            }
        }

        /// <summary>
        /// Apply the given morph
        /// </summary>
        /// <param name="morph">Morph.</param>
        virtual protected void ApplyMorph(ColliderResizeInfo morph)
        {
            for (int i = 0; i < character.Colliders.Length; i++)
            {
                if ((character.Colliders[i].RaycastType & morph.type) == character.Colliders[i].RaycastType) {
                    if (morph.disableCollider)
                    {
                        if (morph.type == RaycastType.HEAD || morph.type == RaycastType.FOOT ||
                            morph.type == RaycastType.ALL || morph.type == RaycastType.ANY)
                        {
                            if ((morph.disableBeyondExtent > 0 && character.Colliders[i].Extent.x > morph.disableBeyondExtent) ||
                                (morph.disableBeyondExtent < 0 && character.Colliders[i].Extent.x < morph.disableBeyondExtent) ||
                                morph.disableBeyondExtent == 0)
                            {
                                character.Colliders[i].Disabled = true;
                            }
                        }
                        else if (morph.type == RaycastType.SIDE_LEFT || morph.type == RaycastType.SIDE_RIGHT || morph.type == RaycastType.SIDES ||
                            morph.type == RaycastType.ALL || morph.type == RaycastType.ANY)
                        {
                            if ((morph.disableBeyondExtent > 0 && character.Colliders[i].Extent.y > morph.disableBeyondExtent) ||
                                (morph.disableBeyondExtent < 0 && character.Colliders[i].Extent.y < morph.disableBeyondExtent) ||
                                morph.disableBeyondExtent == 0)
                            {
                                character.Colliders[i].Disabled = true;
                            }
                        }
                    }
                    else
                    {
                        character.Colliders[i].Extent = defaultExtents[i] + morph.extentChange;
                        character.Colliders[i].Length = defaultLengths[i] + morph.lengthChange;
                    }
                }
            }
        }

        /// <summary>
        /// Applies a morph to a unity collider.
        /// </summary>
        /// <param name="morph">Morph.</param>
        virtual protected void ApplyUnityColliderMorph(UnityColliderResizeInfo morph)
        {
            if (morph.action == UnityColliderAction.DISABLE)
            {
                morph.collider.enabled = false;
                // No need to resize if disabled
                return;
            }
            else if (morph.action == UnityColliderAction.ENABLE)
            {
                morph.collider.enabled = true;
            }
            morph.collider.offset += morph.offsetChange;
            if (morph.collider is BoxCollider2D)
            {
                ((BoxCollider2D)morph.collider).size += morph.sizeChange;
            }
            else if (morph.collider is CircleCollider2D)
            {
                ((CircleCollider2D)morph.collider).radius += morph.sizeChange.x;
            }
            else if (morph.collider is CapsuleCollider2D)
            {
                ((CapsuleCollider2D)morph.collider).size += morph.sizeChange;
            }
        }

        /// <summary>
        /// Reset colliders back to defaults.
        /// </summary>
        virtual protected void RestoreToDefaults()
        {
            // Raycasts
            for (int i = 0; i < character.Colliders.Length; i++)
            {
                character.Colliders[i].Disabled = false;
                character.Colliders[i].Extent = defaultExtents[i];
                character.Colliders[i].Length = defaultLengths[i];
            }
            // Unity Colliders
            foreach (var item in defaultUnityColliders)
            {
                item.Key.enabled = item.Value.enabled;
                item.Key.offset = item.Value.offset;
                if (item.Key is BoxCollider2D)
                {
                    ((BoxCollider2D)item.Key).size = item.Value.size;
                }
                else if (item.Key is CircleCollider2D)
                {
                    ((CircleCollider2D)item.Key).radius = item.Value.size.x;
                }
                else if (item.Key is CapsuleCollider2D)
                {
                    ((CapsuleCollider2D)item.Key).size = item.Value.size;
                }
            }
        }
    }

    [System.Serializable]
    public class ResizeMap
    {
        /// <summary>
        /// States to apply this morph to.
        /// </summary>
        [Tooltip ("States to apply this morph to.")]
        public List<AnimationState> states;

        /// <summary>
        /// Details of the morph.
        /// </summary>
        [Tooltip ("Details of the morph. These change the Platformer PRO colliders.")]
        public List<ColliderResizeInfo> morph;

        /// <summary>
        /// Details of the morph.
        /// </summary>
        [Tooltip("Details of the unity collider changes.")]
        public List<UnityColliderResizeInfo> unityColliders;
    }

    [System.Serializable]
    public class ColliderResizeInfo
    {
        /// <summary>
        /// Type of collider to change.
        /// </summary>
        [Tooltip ("Type of collider to change.")]
        public RaycastType type;

        /// <summary>
        /// Resize or disable the colldier. Set to true to disable (resize will be ignored).
        /// </summary>
        [Tooltip ("Resize or disable the colldier. Set to true to disable (resize will be ignored).")]
        public bool disableCollider;

        /// <summary>
        /// Only colliders whoose extent in the axis orthogonal to the collider is larger than this will be disabled.
        /// If you use a negative value it disables colliders with a lower value.
        /// i.e. If you set this to 2 for SIDES, then only side colliders that have y extent > 2 will be disabled.
        /// </summary>
        [Tooltip("Only colliders whoose extent in the axis orthogonal to the collider is larger than this will be disabled. " +
        	"If you use a negative value it disables colliders with a lower value.\n" +
        	"i.e. If you set this to 2 for SIDES, then only side colliders that have y extent > 2 will be disabled.")]
        [DontShowWhen("disableCollider", showWhenTrue = true)]
        public float disableBeyondExtent;

        /// <summary>
        /// Difference between the normal extent and the new extent.
        /// </summary>
        [Tooltip ("Difference between the normal extent and the new extent.")]
        [DontShowWhen("disableCollider", showWhenTrue = false)]
        public Vector2 extentChange;

        /// <summary>
        /// Amount to alter the length, usually zero is fine.
        /// </summary>
        [Tooltip("Amount to alter the length, usually zero is fine.")]
        [DontShowWhen("disableCollider", showWhenTrue = false)]
        public float lengthChange;
    }

    [System.Serializable]
    public class UnityColliderResizeInfo
    {
        [Tooltip("Collider to change.")]
        public Collider2D collider;
        [Tooltip("Disable or enable the collider?")]
        public UnityColliderAction action;
        [Tooltip("Amount to alter the offset.")]
        public Vector2 offsetChange;
        [Tooltip("Amount to alter the size. For radius the x value is used.")]
        public Vector2 sizeChange;
    }

    /// <summary>
    /// Will we dis
    /// </summary>
    public enum UnityColliderAction
    {
        ENABLE,
        DISABLE,
        NONE
    }

    /// <summary>
    /// Stores defaults for unity colliders so we can reset them back to exact values without potenetial floating point drift.
    /// </summary>
    public class UnityColliderDetails
    {
        public bool enabled;
        public Vector2 offset;
        public Vector2 size;
    }
}