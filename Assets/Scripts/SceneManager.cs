using Game.Player;
using Game.RoomSystem;
using Game.UI;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using USceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Game
{
	public class SceneManager : MonoBehaviour
	{
		public static SceneManager Instance { get; private set; }

		[SerializeField]
		private int RuntimeBuildIndex;

		[SerializeField]
		private float FadeOutDuration = 1.5f, FadeInDuration = 1f;

		private Image _blackout;

		private void Awake()
		{
			Instance = this;
		}

		public void LoadRuntimeScene()
		{
			StartCoroutine(LoadSequence());
		}

		private IEnumerator LoadSequence()
		{
			InputReader input = null;
			GameObject playerGroup = null;

			if (PossessionManager.Instance != null
				&& PossessionManager.Instance.PossessedCharacter != null)
			{
				// Prevent the player, camera, and UI from being unloaded.
				playerGroup = PossessionManager.Instance.PossessedCharacter.transform.root.gameObject;
				DontDestroyOnLoad(playerGroup);

				// Get reference to the fade-to-black UI.
				_blackout = HUD.Instance.Blackout;

				// Disable player input.
				input = PlayerManager.Instance.GetComponent<InputReader>();
				input.enabled = false;

				// Fade to black.
				yield return StartCoroutine(FadeOut());
			}

			// Load/reload scene.
			var opHandle = USceneManager.LoadSceneAsync(RuntimeBuildIndex);
			yield return new WaitUntil(() => 
			opHandle.isDone
			&& PossessionManager.Instance != null
			&& PossessionManager.Instance.PossessedCharacter != null);

			// Move player, camera, and UI back to normal scene.
			USceneManager.MoveGameObjectToScene(playerGroup, USceneManager.GetActiveScene());

			// Position player to correct location.
			Transform possessed = PossessionManager.Instance.PossessedCharacter.transform;
			possessed.position = Room.Current.PlayerSpawnPoint.position;

			// Enable player input.
			input.enabled = true;

			// Fade from black.
			yield return StartCoroutine(FadeIn());
		}

		private IEnumerator FadeOut()
		{
			while (_blackout.color.a > 0f)
			{
				var color = _blackout.color;
				color.a -= Time.deltaTime * FadeOutDuration;
				_blackout.color = color;

				yield return null;
			}
		}

		private IEnumerator FadeIn()
		{
			while (_blackout.color.a < 1f)
			{
				var color = _blackout.color;
				color.a += Time.deltaTime * FadeInDuration;
				_blackout.color = color;

				yield return null;
			}
		}
	}
}
