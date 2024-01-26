using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.RoomSystem
{
	public class RoomManager : MonoBehaviour
	{
		public static RoomManager Instance { get; private set; }

		[SerializeField, RequiredListLength(MinLength = 1)]
		private GameObject[] Rooms;

		public int CurrentRoom { get; private set; }

		[Button("Load Room"), BoxGroup("Debug")]
		public void LoadRoom(int index)
		{
			if (!Application.isPlaying)
			{
				Debug.LogWarning("How do you expect me to load a level when the game isn't running?! Load your own damn level.");
				return;
			}

			CurrentRoom = index - 1;
			Next();
		}

		private void Awake()
		{
			Instance = this;
		}

		public void Reload()
		{
			CurrentRoom--;
			Next();
		}

		public void Next()
		{
			CurrentRoom++;
			SceneManager.Instance.LoadRuntimeScene();
		}
	}
}
