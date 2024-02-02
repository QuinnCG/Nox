using UnityEngine;

namespace Game
{
	public class DelayedDestroy : MonoBehaviour
	{
		[SerializeField]
		private float Delay = 2f;

		private void Start()
		{
			Destroy(gameObject, Delay);
		}
	}
}
