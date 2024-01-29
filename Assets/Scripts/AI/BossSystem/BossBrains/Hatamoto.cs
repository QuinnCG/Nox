using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.AI.BossSystem.BossBrains
{
	public class Hatamoto : BossBrain
	{
		[SerializeField, BoxGroup("Settings")]
		private float FleeThrowInterval = 3f;

		[SerializeField, BoxGroup("Settings")]
		private float PlayerDistanceBeforeTeleport = 4f;

		[SerializeField, Required, BoxGroup("References")]
		private GameObject ShurikenPrefab;

		private State _flee, _teleport, _teleport2;
		private Timer _fleeThrowTimer;

		protected override void Start()
		{
			base.Start();

			_flee = CreateState(OnFlee, "On Flee");
			_teleport = CreateState(OnTeleport, "On Teleport");
			_teleport2 = CreateState(OnTeleport2, "On Teleport 2");

			_fleeThrowTimer = new Timer(FleeThrowInterval);

			TransitionTo(_flee);
		}

		/* BLACKBOARD */
		// Will collide with wall in 1 second, given current trajectory.
		private bool IsNearWall()
		{
			Vector2 start = transform.position;
			Vector2 end = start + Movement.Velocity;

			return !LineOfSight(start, end);
		}

		/* STATES */
		private void OnFlee()
		{
			Move(-DirectionToPlayer);

			if (_fleeThrowTimer.IsDone)
			{
				_fleeThrowTimer.Reset();
				ThrowShuriken();
			}

			// Transitions.
			if (DistanceToPlayer < PlayerDistanceBeforeTeleport)
			{
				TransitionTo(_teleport);
				return;
			}

			if (IsNearWall())
			{
				OnTeleport();
				return;
			}
		}

		private void OnTeleport()
		{
			ThrowShuriken();
			TransitionTo(_flee);
		}

		private void OnTeleport2()
		{
			TransitionTo(_flee);
		}

		/* UTILITY */
		public void ThrowShuriken()
		{
			Shoot(ShurikenPrefab, Bounds.center, PlayerPosition, new ProjectileSystem.ShootSpawnInfo()
			{
				Count = 3,
				Method = ProjectileSystem.ShootMethod.EvenSpread,
				SpreadAngle = 45f
			});
		}

		public void SummonMinions()
		{
			Debug.Log("Summoned minions!");
		}
	}
}
