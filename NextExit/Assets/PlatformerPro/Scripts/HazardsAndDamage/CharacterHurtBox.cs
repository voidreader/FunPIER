using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PlatformerPro
{
	/// <summary>
	/// A collider that can register damage agaisnt the character.
	/// </summary>
	public class CharacterHurtBox : PlatformerProMonoBehaviour, IHurtable, ICharacterReference
	{

		/// <summary>
		/// Gets the header string used to describe the component.
		/// </summary>
		/// <value>The header.</value>
		override public string Header
		{
			get
			{
				return "Add this to a character to allow the charater to take damage";
			}
		}
		
		//// <summary>
		/// Cached reference to the characters health script.
		/// </summary>
		protected CharacterHealth health;

		#region properties
		
		/// <summary>
		/// Gets the character reference.
		/// </summary>
		/// <value>The character.</value>
		virtual public Character Character
		{
			get
			{
                if (health == null || health.Character == null) return GetComponentInParent<Character>();
				return health.Character;
			}
            set
            {
                Debug.LogWarning("CharacterHurtBox doesn't allow character to be changed");
            }
        }

		/// <summary>
		/// Is the character currently invulnerable?
		/// </summary>
		/// <value>The character.</value>
		virtual public bool IsInvulnerable
		{
			get
			{
				return health.IsInvulnerable;
			}
		}

		#endregion

		/// <summary>
		/// Unity OnEnable hook.
		/// </summary>
		void Start()
		{
			// Try looking for character health in self then parents
			if (health == null) health = gameObject.GetComponentInParent<CharacterHealth>();
			if (health == null) Debug.LogError ("Unable to find CharacterHealth for CharacterHurtBox");
		}

		/// <summary>
		/// Pass damage of the given amount to the CharacterHealth script.
		/// </summary>
		/// <param name="amount">Amount.</param>
		virtual public void Damage(int amount)
		{
			if (enabled) health.Damage(amount);
		}

		/// <summary>
		/// Pass damage from the given hazard to the CharacterHealth script.
		/// </summary>
		/// <param name="info">Information about the damage such as the hazard causing the damage.</param>
		virtual public void Damage(DamageInfo info)
		{
			if (enabled)
			{
				if (info.DamageType == DamageType.AUTO_KILL)
				{
					health.Kill();
				}
				else
				{
					health.Damage(info);
				}
			}
		}

		/// <summary>
		/// Get the mobile (charater) that this hurt box belongs too. Can return null.
		/// </summary>
		virtual public IMob Mob
		{
			get
			{
				return Character;
			}
		}

		/// <summary>
		/// Show validation details.
		/// </summary>
		/// <param name="myTarget"></param>
		override public void Validate(PlatformerProMonoBehaviour myTarget)
		{
			base.Validate(myTarget);
			#if UNITY_EDITOR
			Rigidbody2D r = myTarget.gameObject.GetComponent<Rigidbody2D>();
			Collider2D c = myTarget.gameObject.GetComponent<Collider2D>();
			if (c == null)
			{
				ShowValidationHeader();
				EditorGUILayout.HelpBox("CharacterHurtBox should be on the same GameObject as a Collider2D", MessageType.Warning);
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Add BoxCollider2D", EditorStyles.miniButton))
				{
					c = myTarget.gameObject.AddComponent<BoxCollider2D>();
				}
				GUILayout.EndHorizontal();
			} 
			if (r == null) {
				ShowValidationHeader();
				EditorGUILayout.HelpBox("CharacterHurtBox should be on the same GameObject as a Rigidbody2D", MessageType.Warning);
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Add Rigidbody2D", EditorStyles.miniButton))
				{
					r = myTarget.gameObject.AddComponent<Rigidbody2D>();
					r.bodyType = RigidbodyType2D.Kinematic;
				}
				GUILayout.EndHorizontal();
			}  
			else if (r.bodyType != RigidbodyType2D.Kinematic)
			{
				ShowValidationHeader();
				EditorGUILayout.HelpBox("CharacterHurtBox rigidbodies are usually kinematic", MessageType.Info);
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Fix", EditorStyles.miniButton))
				{
					r.bodyType = RigidbodyType2D.Kinematic;
				}
				GUILayout.EndHorizontal();
			}
			#endif
		}
	}

}