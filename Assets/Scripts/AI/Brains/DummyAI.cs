using Game.AI.States;

namespace Game.AI.Brains
{
	public class DummyAI : EnemyBrain
	{
		protected override void Awake()
		{
			base.Awake();

			var wander = new WanderState(3f, 2f, 0.5f);
			SetStartingState(wander);
		}
	}
}
