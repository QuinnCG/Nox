using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Quinn
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance { get; private set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Bootstrap()
		{
			var instance = Addressables.InstantiateAsync("GameManager.prefab").WaitForCompletion();
			DontDestroyOnLoad(instance);
		}

		private void Awake()
		{
			Instance = this;
		}
	}
}
