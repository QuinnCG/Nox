using Game.AI.States;
using UnityEngine;

namespace Game.AI.BossSystem.BossBrains
{
	public class DummyBoss : BossBrain
	{
		protected override void Awake()
		{
			base.Awake();

			var firstPhase = new SubSystemState(new FirstPhase(this), "First Phase");
			var secondPhase = new SubSystemState(new SecondPhase(this), "Second Phase");

			SetStartingState(firstPhase);
			Connect(firstPhase, secondPhase, () => Health.Percent <= 0.5f);
		}

		protected override void Update()
		{
			base.Update();
		}

		private class FirstPhase : StateMachine
		{
			public FirstPhase(EnemyBrain agent) 
				: base(agent)
			{
				SetStartingState(new WanderState(5f, 2.5f));
			}
		}

		private class SecondPhase : StateMachine
		{
			public SecondPhase(EnemyBrain agent)
				: base(agent)
			{
				SetStartingState(new CustomState(update: agent =>
				{
					Debug.Log("Second Phase!".Bold().Color(StringColor.Red));
				}));
			}
		}
	}
}
