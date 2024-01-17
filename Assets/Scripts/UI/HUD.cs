using DG.Tweening;
using Game.DamageSystem;
using Game.Player;
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

		private Character _lastCharacter;
		private Health _health;
		private Tween _healthFadeInTween, _healthFadeOutTween;

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
			PlayerManager.Instance.OnCharacterPossessed += OnCharacterPossessed;
			OnCharacterPossessed(PlayerManager.Instance.PossessedCharacter);
		}

		private void Update()
		{
			_playerHealth.value = _health != null ? _health.Current / _health.Max : 0f;
		}

		private void OnCharacterPossessed(Character character)
		{
			if (character == null)
			{
				// Handle the case when the character is null
				return;
			}

			if (_lastCharacter != null)
			{
				var lastHealth = _lastCharacter.GetComponent<Health>();
				if (lastHealth != null)
				{
					lastHealth.OnDamaged -= OnDamaged;
					lastHealth.OnReachMaxHealth -= HideHealth;
				}
			}

			_health = character.GetComponent<Health>();
			if (_health != null)
			{
				_health.OnDamaged += OnDamaged;
				_health.OnReachMaxHealth += HideHealth;

				if (_health.IsCritical)
				{
					ShowHealth();
				}
			}

			_lastCharacter = character;
		}


		private void OnDamaged(float damage)
		{
			ShowHealth();
		}

		private void ShowHealth()
		{
			_healthFadeInTween?.Kill();

			_healthFadeInTween = DOTween.To(
					() => _playerHealth.style.opacity.value,
					x => _playerHealth.style.opacity = x,
					1f, PlayerHealthFadeIn).SetEase(Ease.Linear);
		}

		private void HideHealth()
		{
			_healthFadeOutTween?.Kill();

			_healthFadeOutTween = DOTween.To(
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
