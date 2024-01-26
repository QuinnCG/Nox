using UnityEngine;

namespace Game.RoomSystem
{
	public class Door : MonoBehaviour
	{
		public void Open()
		{
			gameObject.SetActive(false);
		}

		public void Close()
		{
			gameObject.SetActive(true);
		}
	}
}
