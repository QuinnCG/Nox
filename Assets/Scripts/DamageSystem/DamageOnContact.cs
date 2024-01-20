using Game.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DamageSystem
{
	[RequireComponent(typeof(Health))]
	public class DamageOnContact : MonoBehaviour
	{
		[SerializeField]
		private float ContactDamage = 10f;

		private CapsuleCollider2D _collider;
		private Health _health;

		private void Awake()
		{
			_collider = GetComponent<CapsuleCollider2D>();
			_health = GetComponent<Health>();
		}

		private void FixedUpdate()
		{
			Collider2D[] colliders = Physics2D.OverlapCapsuleAll(
				_collider.bounds.center, _collider.bounds.size, 
				_collider.direction, 0f, 
				LayerMask.GetMask("Character"));

			foreach (var collider in colliders)
			{
				if (collider.gameObject == gameObject)
					continue;

				if (!collider.TryGetComponent(out Character _))
					continue;

				var player = PossessionManager.Instance.PossessedCharacter.gameObject;
				if (gameObject == player && collider.gameObject != player)
				{
					_health.TakeDamage(ContactDamage);
				}
			}
		}
	}
}
