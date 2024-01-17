namespace Game.AI
{
	public class State
	{
		public virtual void OnStart(EnemyBrain enemy) { }
		public virtual void OnUpdate(EnemyBrain enemy) { }
		public virtual void OnFinish(EnemyBrain enemy) { }
	}
}
