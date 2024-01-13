using System;
using UnityEngine;

namespace Game
{
	public class InputReader : MonoBehaviour
	{
		// Expose some of the input keys we may want to change later.
		[SerializeField]
		private KeyCode DashKey = KeyCode.LeftAlt;

		public event Action<Vector2> OnMove;
		public event Action OnDash;
		public event Action OnAttack;

		public void Update()
		{
			// Create a vector2 where
			// x = horizontal input (a = -1 && d = 1)
			// y = vertical input (w = 1 && s = -1)
			// then normalize the result to avoid diagonal movements from being faster (see https://unitycodemonkey.com/video.php?v=YMwwYO1naCg)
			var dirInput = new Vector2()
			{
				x = Input.GetAxisRaw("Horizontal"),
				y = Input.GetAxisRaw("Vertical")
			}.normalized;

			// Triggers anything subscribed to the event.
			// Notice the '?' at the end of move,
			// That does a check to make sure that something is subscribed before invoking
			// to avoid a null reference exception.
			OnMove?.Invoke(dirInput);

			// Every frame test if 'DashKey' has JUST been pressed.
			if (Input.GetKeyDown(DashKey))
			{
				OnDash?.Invoke();
			}

			// 0 = left mouse, 1 = right mouse, 2 = middle mouse.
			// You find this stuff from Unity's docs.
			if (Input.GetMouseButtonDown(0))
			{
				OnAttack?.Invoke();
			}
		}
	}
}
