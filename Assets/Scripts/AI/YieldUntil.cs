using System;

namespace Game.AI
{
	public class YieldUntil
	{
		public Func<bool> Predicate { get; }

		public YieldUntil(Func<bool> predicate)
		{
			Predicate = predicate;
		}
	}
}
