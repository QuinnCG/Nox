using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.AI
{
	public class StateMachine
	{
		public State Active { get; private set; }

		// Required to cache the coroutine callback due to weird null ref when accessing directly from Active.
		private Func<IEnumerator> _coroutineCallback;

		public void Update()
		{
			if (Active != null)
			{
				if (Active.IsCoroutine)
				{
					var current = Active.Coroutine.Current;
					bool moveNext = ShouldMoveNext(current);

					if (moveNext)
					{
						// Reached end. Time to reset.
						if (!Active.Coroutine.MoveNext())
						{
							Active.Coroutine = _coroutineCallback();
							Active.Coroutine.MoveNext();
						}
					}
				}
				else
				{
					Active.Callback();
				}
			}
		}

		public void TransitionTo(State state)
		{
			Debug.Log(state.Name);

			if (state != Active)
			{
				Active = state;

				if (state.IsCoroutine)
				{
					state.Coroutine = state.CoroutineCallback();
					state.Coroutine.MoveNext();

					_coroutineCallback = state.CoroutineCallback;
				}
			}
		}

		public State CreateState(Action callback, string name = "No Name")
		{
			return new State(callback, name);
		}
		public State CreateState(Func<IEnumerator> coroutineCallback, string name = "No Name")
		{
			return new State(coroutineCallback, name);
		}

		private bool ShouldMoveNext(object current)
		{
			if (current is YieldSeconds yieldSeconds)
			{
				if (Time.time < yieldSeconds.End)
				{
					return false;
				}
			}
			else if (current is YieldNextFrame)
			{
				return true;
			}
			else if (current is YieldUntil yieldUntil)
			{
				if (!yieldUntil.Predicate())
				{
					return false;
				}
			}
			else if (current is YieldTween yieldTween)
			{
				if (!yieldTween.Tween.IsActive())
				{
					if (!yieldTween.Tween.IsComplete())
					{
						return false;
					}
				}
			}
			else if (current is YieldEnumerator yieldEnumerator)
			{
				if (!yieldEnumerator.Enumerator.MoveNext())
				{
					bool value = ShouldMoveNext(yieldEnumerator.Enumerator.Current);
                    if (value)
					{
						yieldEnumerator.Enumerator.MoveNext();
					}

                    return value;
				}
			}

#if UNITY_EDITOR
			else if (current is not null)
			{
				Debug.LogError("StateMachine has been given an invalid yield!\n" +
					$"Yield given: '{current.GetType().Name}'.");
				return false;
			}
#endif

			return true;
		}
	}
}
