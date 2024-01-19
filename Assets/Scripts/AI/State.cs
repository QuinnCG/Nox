namespace Game.AI
{
	public class State
	{
		public virtual string Name => GetType().Name;

		public virtual void OnStart(EnemyBrain agent) { }
		public virtual void OnUpdate(EnemyBrain agent) { }
		public virtual void OnFinish(EnemyBrain agent) { }
	}
}
