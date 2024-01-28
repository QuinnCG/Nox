using System.Collections;
using UnityEngine;

namespace Game.Experimental
{
	public class DummyAI : StateMachine
	{
		public bool CanSeePlayer;
		public bool InAttackRange;

		private void Update()
		{
			if (CanSeePlayer)
			{
				TransitionTo(Engage());
			}
			else
			{
				TransitionTo(Patrol());
			}
		}

		private IEnumerator Patrol()
		{
			MoveTo();
			yield return null;
		}

		private IEnumerator Engage()
		{
			while (true)
			{
				if (InAttackRange)
				{
					Attack();
					yield return new WaitForSeconds(2f);
				}
				else
				{
					MoveTo();
				}

				yield return null;
			}
		}

		public void MoveTo() { }

		public void Attack() { }
	}
}
