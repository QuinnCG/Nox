using System.Collections;
using UnityEngine;

namespace Game.Experimental
{
	public class StateMachine : MonoBehaviour
	{
		public IEnumerator Current { get; private set; }

		public void TransitionTo(IEnumerator state)
		{
			if (state == Current) return;

			if (Current != null)
			{
				StopCoroutine(Current);
			}

			StartCoroutine(state);
			Current = state;
		}
	}
}
