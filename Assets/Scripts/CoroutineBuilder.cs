using System;
using System.Collections;
using UnityEngine;

namespace Game
{
	public class CoroutineBuilder : MonoBehaviour
	{
		public event Action OnFinish;

		public static CoroutineBuilder Create(IEnumerator coroutine, Transform parent = default)
		{
			var instance = new GameObject("Coroutine Builder");
			var builder = instance.GetComponent<CoroutineBuilder>();
			builder.Run(coroutine);

			if (parent != null)
			{
				instance.transform.parent = parent;
			}

			return builder;
		}

		private void Run(IEnumerator coroutine)
		{
			StartCoroutine(Sequence(coroutine));
		}

		private IEnumerator Sequence(IEnumerator coroutine)
		{
			yield return StartCoroutine(coroutine);
			OnFinish?.Invoke();
			Destroy(gameObject);
		}
	}
}
