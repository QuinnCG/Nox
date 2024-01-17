using UnityEngine;

namespace Game.AI.BossSystem
{
	public abstract class Boss : EnemyBrain
	{
		[SerializeField]
		private int PhaseCount = 2;

		// Most data required by a boss already exists in the parent class.
		protected int CurrentPhase { get; private set; } = 1;

		private float _phaseSegment;


		protected override void Start()
		{
			base.Start();

			_phaseSegment = Health.Max / PhaseCount;
			Health.OnDamaged += _ => OnDamaged();
		}

		private void OnDamaged()
		{
			float current = Health.Current;
			int phases = Mathf.FloorToInt(current / _phaseSegment);

			// Start counting from 1 instead of 0.
			CurrentPhase = phases + 1;
		}
	}
}
