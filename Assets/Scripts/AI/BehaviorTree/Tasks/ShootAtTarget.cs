using Game.ProjectileSystem;
using UnityEngine;

namespace Game.AI.BehaviorTree.Tasks
{
	public class ShootAtTarget : BTTask
	{
		private readonly GameObject _projectile;

		private readonly Transform _origin;
		private readonly BTProperty<Vector2> _target;

		private readonly ShootSpawnInfo _spawnInfo;

		public ShootAtTarget(GameObject projectile, Transform origin, BTProperty<Vector2> target, ShootSpawnInfo spawnInfo = default)
		{
			_projectile = projectile;

			_origin = origin;
			_target = target;

			_spawnInfo = spawnInfo;
		}

		protected override BTStatus OnUpdate()
		{
			Projectile.Spawn(_projectile, _origin.position, _target.Value, Agent.gameObject, _spawnInfo);
			return BTStatus.Success;
		}
	}
}
