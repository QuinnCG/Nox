using System.Collections;

namespace Game.AI
{
	public class YieldEnumerator
	{
		public IEnumerator Enumerator { get; }

		public YieldEnumerator(IEnumerator enumerator)
		{
			Enumerator = enumerator;
		}
	}
}
