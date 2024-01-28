using Game.AI.BehaviorTree;
using Game.DamageSystem;
using Game.MovementSystem;
using Game.Player;
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

		/// <summary>
		/// The movement component on this enemy.
		/// </summary>
		public Movement Movement { get; private set; }
		/// <summary>
		/// The health component on this enemy.
		/// </summary>
		public Health Health { get; private set; }
		/// <summary>
		/// The hitbox for this enemy.
		/// </summary>
		public Collider2D Collider { get; private set; }
		/// <summary>
		/// The bounds of the hitbox for this enemy.
		/// </summary>
		public Bounds Bounds => Collider.bounds;

		/// <summary>
		/// The tree running this enemy.
		/// </summary>
		public BTTree Tree { get; private set; }
		/// <summary>
		/// The position of this enemy.
		/// </summary>
		[Expose]
		public BTProperty<Vector2> Position { get; private set; } = new();
		/// <summary>
		/// The percent (0 - 1) of health remaining on this enemy.
		/// </summary>
		[Expose]
		public BTProperty<float> HealthPercent { get; private set; } = new();
		/// <summary>
		/// The position of the currently possessed character (e.g. the player).
		/// </summary>
		[Expose]
		public BTProperty<Vector2> PlayerPos { get; private set; } = new();

		private bool _treeStarted;

		protected virtual void Awake()
		{
			Movement = GetComponent<Movement>();
			Health = GetComponent<Health>();
			Collider = GetComponent<Collider2D>();

			Tree = new(this);
		}

		protected virtual void Start()
		{
			PlayerManager = PlayerManager.Instance;
		}

		protected virtual void Update()
		{
			if (!_treeStarted)
			{
				_treeStarted = true;
				Tree.Start();
			}

			Position.Value = (Vector2)transform.position;
			HealthPercent.Value = Health.Percent;
			PlayerPos.Value = PossessionManager.Instance.Position;

			Tree.Update();
		}

		protected void AddNode(params BTNode[] nodes)
		{
			Tree.Add(nodes);
		}
	}
}
