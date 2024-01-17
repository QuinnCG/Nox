using UnityEngine;

namespace Game.AI.BossSystem
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
