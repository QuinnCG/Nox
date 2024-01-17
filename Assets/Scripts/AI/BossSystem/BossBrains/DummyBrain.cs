using Game.AI.States;

namespace Game.AI.BossSystem.BossBrains
{
	public class DummyBrain : BossBrain
	{
		protected override void Awake()
		{
			base.Awake();

			var wander = new WanderState(3f, 2f, 0.5f);
			SetStartingState(wander);
		}
	}
}
