using System.Collections;
using UnityEngine;

namespace Game.Experimental
{
	public class StateMachine : MonoBehaviour
	{
		private IEnumerator _activeState;

		public void TransitionTo(IEnumerator state)
		{
			if (_activeState != null)
			{
				StopCoroutine(_activeState);
			}

			StartCoroutine(state);
			_activeState = state;
		}
	}
}