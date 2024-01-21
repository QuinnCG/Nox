using Game.AI.BehaviorTree;
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

		private BTTree _tree;

		protected virtual void Awake()
		{
			Movement = GetComponent<Movement>();
			Health = GetComponent<Health>();
			Collider = GetComponent<Collider2D>();

			_tree = new(this);
		}

		protected virtual void Start()
		{
			PlayerManager = PlayerManager.Instance;
		}

		protected virtual void Update()
		{
			_tree.Update();
		}

		protected void AddNode(params BTNode[] nodes)
		{
			_tree.Add(nodes);
		}
	}
}
