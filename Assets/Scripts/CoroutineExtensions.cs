using System.Collections;
using UnityEngine;

namespace Game
{
	public static class CoroutineExtensions
	{
		public static void Run(this IEnumerator coroutine, Transform parent = default)
		{
			CoroutineBuilder.Create(coroutine, parent);
		}
	}
}
