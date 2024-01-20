using Game.AI.BehaviorTree;
using Game.AI.BehaviorTree.Composites;
using Game.AI.BehaviorTree.Conditionals;
using Game.AI.BehaviorTree.Tasks;

namespace Game.AI.BossSystem.BossBrains
{
	public class DummyBoss : BossBrain
	{
		private readonly BTTree _tree = new();

		private bool _first;

		protected override void Awake()
		{
			base.Awake();

			var log1 = new Log("1", true);
			var chance = new CustomCondition(() => _first);
			log1.Condition(chance);

			var sequence = new Sequence();
			var custom = new CustomTask(() => { _first = true; return BTStatus.Success; });
			var log2 = new Log("2", true);
			sequence.Add(custom);
			sequence.Add(log2);

			_tree.Add(log1, sequence);
		}

		private void Update()
		{
			_tree.Update();
		}
	}
}
