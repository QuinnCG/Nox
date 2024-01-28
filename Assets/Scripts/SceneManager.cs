using Game.Player;
using Game.RoomSystem;
using Game.UI;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using System.Linq;
using System;
using Cinemachine;

namespace Game
{
	public class SceneManager : MonoBehaviour
	{
		public static SceneManager Instance { get; private set; }

		[field: SerializeField, BoxGroup("Scene Names")]
		public string MainMenuSceneName { get; private set; } = "MainMenu";

		[field: SerializeField, BoxGroup("Scene Names")]
		public string RuntimeSceneName { get; private set; } = "RuntimeScene";

		[SerializeField]
		private float FadeOutDuration = 1.5f, FadeInDuration = 1f;

		public event Action<Scene> OnSceneLoaded;

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

			if (PossessionManager.Instance != null
				&& PossessionManager.Instance.PossessedCharacter != null
				&& HUD.Instance != null)
			{
				// Get reference to the fade-to-black UI.
				_blackout = HUD.Instance.Blackout;

				// Disable player input.
				input = PlayerManager.Instance.GetComponent<InputReader>();
				input.enabled = false;

				// Fade to black.
				yield return StartCoroutine(FadeOut());
			}

			// Enable loading screen.
			var scene = USceneManager.GetActiveScene();
			var root = scene.GetRootGameObjects();
			foreach (var go in root)
			{
				if (go.CompareTag("Loading"))
				{
					go.SetActive(true);
					break;
				}
			}

			// Unload additives scenes (excluding "PersistentScene").
			yield return StartCoroutine(UnloadAllAdditiveScenes());

			// Load/reload scene.
			var opHandle = USceneManager.LoadSceneAsync(RuntimeSceneName, LoadSceneMode.Additive);

			// Wait for scene to finish loading.
			yield return new WaitUntil(() => opHandle.isDone);
			OnSceneLoaded?.Invoke(USceneManager.GetSceneByName(RuntimeSceneName));

			// Enable "PlayerGroup".
			scene = USceneManager.GetActiveScene();
			GameObject[] gameObjects = scene.GetRootGameObjects();
			foreach (var gameObject in gameObjects)
			{
				if (gameObject.GetComponentInChildren<PlayerManager>() != null)
				{
					gameObject.transform.root.gameObject.SetActive(true);
					break;
				}
			}

			// Wait until player's possessed character is loaded.
			yield return new WaitUntil(() => PossessionManager.Instance.PossessedCharacter != null);

			// Get player's possessed character.
			Transform possessed = PossessionManager.Instance.PossessedCharacter.transform;

			// Set player position.
			possessed.position = Room.Current.PlayerSpawnPoint.position;

			// Update HUD.
			HUD.Instance.OnBossRoomStart(Room.Current);

			var vcam = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera as CinemachineVirtualCamera;
			vcam.transform.position = possessed.position;

			// Enable player input.
			input = PlayerManager.Instance.GetComponent<InputReader>();
			input.enabled = true;

			// Disable loading screen.
			foreach (var gameObject in gameObjects)
			{
				if (gameObject.CompareTag("Loading"))
				{
					gameObject.SetActive(false);
					break;
				}
			}

			// Fade from black.
			_blackout = HUD.Instance.Blackout;
			yield return StartCoroutine(FadeIn());
		}

		private IEnumerator UnloadAllAdditiveScenes()
		{
			var ops = new List<AsyncOperation>();

			// Starts from index '1' to avoid unloading "PersistentScene".
			for (int i = 1; i < USceneManager.sceneCount; i++)
			{
				var scene = USceneManager.GetSceneAt(i);
				var op = USceneManager.UnloadSceneAsync(scene);
				ops.Add(op);
			}

			// Wait until all scenes are unloaded.
			while (ops.Count > 0)
			{
				yield return new WaitUntil(() => ops[0].isDone);
				ops.RemoveAt(0);
			}
		}

		private IEnumerator FadeOut()
		{
			var color = _blackout.color;
			color.a = 0f;
			_blackout.color = color;

			while (_blackout.color.a < 1f)
			{
				color = _blackout.color;
				color.a += Time.deltaTime * FadeOutDuration;
				_blackout.color = color;

				yield return null;
			}
		}

		private IEnumerator FadeIn()
		{
			var color = _blackout.color;
			color.a = 1f;
			_blackout.color = color;

			while (_blackout.color.a > 0f)
			{
				color = _blackout.color;
				color.a -= Time.deltaTime * FadeInDuration;
				_blackout.color = color;

				yield return null;
			}
		}
	}
}
