using UnityEngine;

namespace Game.UI
{
	public class QuitApplication : MonoBehaviour
	{
		private void Start()
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}

		public void Quit()
		{
			Application.Quit();
		}
	}
}
