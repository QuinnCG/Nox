using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game
{
	/// <summary>
	/// <para>This is a general manager class.
	/// Really all it does is instantiate itself at the start of the game.</para>
	/// <para>You can add other "manager" classes to the prefab called "GameManager.prefab",
	/// and they will exist for the whole lifecycle of the game.</para>
	/// </summary>
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance { get; private set; }

		// The attribute here is provided by Unity.
		// It makes it so this static function will be called before the first scene is loaded.
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Bootstrap()
		{
			// This is how you instantiate a prefab using the "Addressables" package used in this project.
			var instance = Addressables.InstantiateAsync("GameManager.prefab").WaitForCompletion();
			DontDestroyOnLoad(instance);
		}

		private void Awake()
		{
			// Look up "singleton pattern".
			Instance = this;
		}
	}
}
