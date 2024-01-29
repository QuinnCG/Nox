using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using USceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Game
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance { get; private set; }

		public bool InGameOver { get; set; }

		// Unity will call this method before the very first scene of the game is loaded.
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Bootstrap()
		{
			var instance = Addressables.InstantiateAsync("GameManager.prefab")
				.WaitForCompletion();

			DontDestroyOnLoad(instance);
		}

		private async void Start()
		{
			Instance = this;
			await USceneManager.LoadSceneAsync(SceneManager.Instance.MainMenuSceneName, LoadSceneMode.Additive);

			// Disable loading screen.
			var scene = USceneManager.GetActiveScene();
			GameObject[] gameObjects = scene.GetRootGameObjects();
			foreach (var gameObject in gameObjects)
			{
				if (gameObject.CompareTag("Loading"))
				{
					gameObject.SetActive(false);
					break;
				}
			}
		}
	}
}
