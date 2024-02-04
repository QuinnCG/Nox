using Game.Player;
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

		//[MenuItem("Tools/Rooms/Disgraced Hatamoto")]
		//public static void LoadHatamoto()
		//{
		//	LoadRoom(0);
		//}

		//[MenuItem("Tools/Rooms/Shugo The Drunkard")]
		//public static void LoadShugo()
		//{
		//	LoadRoom(1);
		//}

		//public static void LoadRoom(int index)
		//{
		//	if (!Application.isPlaying)
		//	{
		//		EditorApplication.EnterPlaymode();
		//	}

		//	// TODO: Task.Run won't execute in-editor...
		//	Task.Run(() =>
		//	{
		//		Task.Delay(3000);
		//		Instance.CurrentRoom = index - 1;
		//		Instance.Next(true);
		//	});
		//}

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
				if (scene.name == SceneManager.Instance.MainMenuSceneName)
				{
					return;
				}

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
			PossessionManager.Instance.Respawn();

			CurrentRoom--;
			Next(true);
		}

		public void Next(bool skipFadeOut = false)
		{
			CurrentRoom++;

			if (CurrentRoom >= Rooms.Length)
			{
				Debug.LogWarning($"Cannot load next room ({CurrentRoom}) as it doesn't exist!\n" +
					$"Go to 'RoomManager', attached to the 'GameManager' prefab, to configure available rooms.\n" +
					$"Current room: {CurrentRoom - 1}. Next room: {CurrentRoom}.");

				CurrentRoom--;
				return;
			}

			SceneManager.Instance.LoadRuntimeScene(skipFadeOut);
		}
	}
}
