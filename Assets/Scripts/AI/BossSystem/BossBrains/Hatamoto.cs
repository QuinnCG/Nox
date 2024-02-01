using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Game.AI.BossSystem.BossBrains
{
	public class Hatamoto : BossBrain
	{
		[SerializeField, BoxGroup("Settings"), Unit(Units.Second)]
		private float FleeThrowInterval = 3f;

		[SerializeField, BoxGroup("Settings"), Unit(Units.Meter)]
		private float PlayerDistanceBeforeTeleport = 4f;

		[SerializeField, BoxGroup("Settings"), Unit(Units.Second)]
		private float DurationToWall = 0.3f;

		[SerializeField, Required, BoxGroup("References")]
		private GameObject ShurikenPrefab;

		[SerializeField, BoxGroup("References")]
		private GameObject TeleportVFX;

		[SerializeField, BoxGroup("References")]
		private Transform[] TeleportPoints;

		private State _flee, _teleport, _teleport2;
		private Timer _fleeThrowTimer;

		private float _fleeDir = -1f;

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
			Vector2 start = Bounds.center;
			Vector2 end = start + (Movement.Velocity * DurationToWall);

			return !LineOfSight(start, end);
		}

		/* STATES */
		private void OnFlee()
		{
			Move(Vector2.right * _fleeDir);

			if (_fleeThrowTimer.IsDone)
			{
				ThrowShuriken();
				_fleeThrowTimer.Reset();
			}

			// Transitions.

			if (IsNearWall())
			{
				_fleeDir *= -1f;
				TransitionTo(_teleport);
				return;
			}
		}

		private IEnumerator OnTeleport()
		{
			yield return SpawnVFXOneShot(TeleportVFX, Bounds.center);

			Vector2 point = GetPointFarthestFrom(PlayerPosition, TeleportPoints);
			TeleportTo(point);

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
