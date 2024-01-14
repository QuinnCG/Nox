using DG.Tweening;
using Game.DamageSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
	public class HUD : MonoBehaviour
	{
		[SerializeField, Required]
		private UIDocument Document;

		[SerializeField, BoxGroup("Animation Settings")]
		private float PlayerHealthFadeIn = 1f;

		[SerializeField, BoxGroup("Animation Settings")]
		private float PlayerHealthFadeOut = 2f;

		private VisualElement _root;

		private ProgressBar _playerHealth;

		private VisualElement _bossTitleContainer;
		private ProgressBar _bossHealth;

		private Health _health;

		private void Awake()
		{
			_root = Document.rootVisualElement;

			_playerHealth = _root.Q<ProgressBar>("player-health");
			_bossTitleContainer = _root.Q<VisualElement>("boss-title-container");
			_bossHealth = _root.Q<ProgressBar>("boss-health");

			HideHealth();
			HideBoss();
		}

		private void Start()
		{
			PlayerManager.Instance.OnCharacterPossessed += character =>
			{
				var health = character.GetComponent<Health>();
				_health = health;

				health.OnDamaged += _ => ShowHealth();
				health.OnReachMaxHealth += () => HideHealth();

				if (health.IsCritical)
				{
					ShowHealth();
				}
			};
		}

		private void Update()
		{
			_playerHealth.value = _health.Current / _health.Max;
		}

		private void ShowHealth()
		{
			DOTween.To(
				() => _playerHealth.style.opacity.value,
				x => _playerHealth.style.opacity = x,
				1f, PlayerHealthFadeIn).SetEase(Ease.Linear);
		}

		private void HideHealth()
		{
			DOTween.To(
				() => _playerHealth.style.opacity.value,
				x => _playerHealth.style.opacity = x,
				0f, PlayerHealthFadeOut).SetEase(Ease.Linear);
		}

		private void ShowBoss()
		{
			_bossTitleContainer.style.opacity = 1f;
		}

		private void HideBoss()
		{
			_bossTitleContainer.style.opacity = 0f;
		}
	}
}
