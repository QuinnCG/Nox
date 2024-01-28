using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.RoomSystem
{
	public class RoomManager : MonoBehaviour
	{
		public static RoomManager Instance { get; private set; }

		[SerializeField, RequiredListLength(MinLength = 1)]
		private GameObject[] Rooms;

		[field: SerializeField, ReadOnly]
		public int CurrentRoom { get; private set; }

		[Button("Load Room"), BoxGroup("Tools")]
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

		public event Action<Room> OnBossRoomStart;

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			CurrentRoom = 0;

			SceneManager.Instance.OnSceneLoaded += scene =>
			{
				if (CurrentRoom < Rooms.Length)
				{
					var room = Instantiate(Rooms[CurrentRoom]);
					UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(room, scene);

					OnBossRoomStart?.Invoke(room.GetComponentInChildren<Room>());
				}
			};
		}

		public void Reload()
		{
			CurrentRoom--;
			Next();
		}

		public void Next()
		{
			if (CurrentRoom + 1 >= Rooms.Length)
			{
				Debug.LogWarning($"Cannot load next room ({CurrentRoom + 1}) as it doesn't exist!\n" +
					$"Go to 'RoomManager', attached to the 'GameManager' prefab, to configure available rooms.\n" +
					$"Current room: {CurrentRoom}. Next room: {CurrentRoom + 1}.");
				return;
			}

			//CurrentRoom++;
			SceneManager.Instance.LoadRuntimeScene();
		}
	}
}
