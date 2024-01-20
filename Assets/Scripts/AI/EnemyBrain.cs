using Game.DamageSystem;
using Game.MovementSystem;
using Game.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AI
{
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(Health))]
	public abstract class EnemyBrain : MonoBehaviour
	{
		public PlayerManager PlayerManager { get; private set; }

		public float HP => Health.Current;
		public float MaxHP => Health.Max;
		public bool IsDead => Health.IsDead;

		public Movement Movement { get; private set; }
		public Health Health { get; private set; }
		public Collider2D Collider { get; private set; }
		public Bounds Bounds => Collider.bounds;

		protected virtual void Awake()
		{
			Movement = GetComponent<Movement>();
			Health = GetComponent<Health>();
			Collider = GetComponent<Collider2D>();
		}

		protected virtual void Start()
		{
			PlayerManager = PlayerManager.Instance;
		}
	}
}
