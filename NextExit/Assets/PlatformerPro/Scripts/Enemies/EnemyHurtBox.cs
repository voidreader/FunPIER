using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PlatformerPro
{
	/// <summary>
	/// Enemy hurt box.
	/// </summary>
	public class EnemyHurtBox : PlatformerProMonoBehaviour, IHurtable
	{
		/// <summary>
		/// Gets the header string used to describe the component.
		/// </summary>
		/// <value>The header.</value>
		override public string Header
		{
			get
			{
				return "Add this to an enemy to allow the enemy to take damage";
			}
		}
		
		//// <summary>
		/// Cached reference to the enemy script.
		/// </summary>
		protected Enemy enemy;
		
		#region properties
		
		/// <summary>
		/// Gets the enemy reference.
		/// </summary>
		/// <value>The enemy.</value>
		virtual public Enemy Enemy
		{
			get
			{
				return enemy;
			}
		}
		
		/// <summary>
		/// Is the enemy currently invulnerable?
		/// </summary>
		/// <value>The character.</value>
		virtual public bool IsInvulnerable
		{
			get
			{
				return enemy.IsInvulnerable;
			}
		}
		
		#endregion
		
		/// <summary>
		/// Start this instance.
		/// </summary>
		void Start()
		{
			// Try looking for enemy
			if (enemy == null) enemy = gameObject.GetComponentInParent<Enemy>();
			if (enemy == null) Debug.LogError ("Unable to find Enemy for EnemyHurtBox");
		}
		
		/// <summary>
		/// Pass damage from the given hazard to the Enemy script.
		/// </summary>
		/// <param name="info">Information about the damage such as the hazard causing the damage.</param>
		virtual public void Damage(DamageInfo info)
		{
			enemy.Damage(info);
		}

		/// <summary>
		/// Get the mobile (charater) that this hurt box belongs too. Can return null.
		/// </summary>
		virtual public IMob Mob
		{
			get
			{
				return enemy;
			}
		}

		override public void Validate(PlatformerProMonoBehaviour myTarget)
		{
#if UNITY_EDITOR
			Rigidbody2D r = myTarget.gameObject.GetComponent<Rigidbody2D>();
			Collider2D c = myTarget.gameObject.GetComponent<Collider2D>();
			if (c == null) {
				EditorGUILayout.HelpBox("EnemyHurtBox should be on the same GameObject as a Collider2D", MessageType.Warning);
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Add BoxCollider2D", EditorStyles.miniButton))
				{
					c = myTarget.gameObject.AddComponent<BoxCollider2D>();
				}
				GUILayout.EndHorizontal();
			} 
			if (r == null) {
				EditorGUILayout.HelpBox("EnemyHurtBox should be on the same GameObject as a Rigidbody2D", MessageType.Warning);
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
				EditorGUILayout.HelpBox("EnemyHurtBox rigidbodies are usually kinematic", MessageType.Info);
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