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

		[Space, SerializeField, Required]
		private Image Blackout;

		[SerializeField]
		private float FadeOutDuration = 1.5f, FadeInDuration = 1f;

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
			yield return StartCoroutine(FadeOut());

			var opHandle = USceneManager.LoadSceneAsync(RuntimeBuildIndex);
			yield return new WaitUntil(() => opHandle.isDone);

			yield return StartCoroutine(FadeIn());
		}

		private IEnumerator FadeOut()
		{
			while (Blackout.color.a > 0f)
			{
				var color = Blackout.color;
				color.a -= Time.deltaTime * FadeOutDuration;
				Blackout.color = color;

				yield return null;
			}
		}

		private IEnumerator FadeIn()
		{
			while (Blackout.color.a < 1f)
			{
				var color = Blackout.color;
				color.a += Time.deltaTime * FadeInDuration;
				Blackout.color = color;

				yield return null;
			}
		}
	}
}
