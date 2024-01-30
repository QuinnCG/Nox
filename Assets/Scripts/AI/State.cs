using System;
using System.Collections;
using UnityEngine;

namespace Game.AI
{
	public class State
	{
		public string Name { get; }
		public Action Callback { get; }
		public Func<IEnumerator> CoroutineCallback { get; }
		public IEnumerator Coroutine { get; set; }
		public bool IsCoroutine { get; }

		public State(Action callback, string name)
		{
			Callback = callback;
			Name = name;
		}
		public State(Func<IEnumerator> coroutineCallback, string name)
		{
			IsCoroutine = true;
			CoroutineCallback = coroutineCallback;

			Name = name;
		}
	}
}
