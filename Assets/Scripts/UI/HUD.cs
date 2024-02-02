using DG.Tweening;
using Game.DamageSystem;
using Game.Player;
using Game.RoomSystem;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class HUD : MonoBehaviour
	{
		public static HUD Instance { get; private set; }

		[SerializeField, BoxGroup("Animation Settings")]
		private float PlayerHealthFadeIn = 1f;

		[SerializeField, BoxGroup("Animation Settings")]
		private float PlayerHealthFadeOut = 2f;

		[SerializeField, BoxGroup("Animation Settings")]
		private float PlayerHealthCriticalPulseDuration = 0.5f;

		[SerializeField, BoxGroup("Animation Settings")]
		private float PlayerHealthCriticalPulseScale = 1.2f;

		[SerializeField, BoxGroup("References"), Required]
		private Slider PlayerHealth;

		[SerializeField, BoxGroup("References"), Required]
		private Slider PossessionMeter;

		[SerializeField, BoxGroup("References"), Required]
		private GameObject BossContainer;

		[SerializeField, BoxGroup("References"), Required]
		private Slider BossHealth;

		[SerializeField, BoxGroup("References"), Required]
		private TextMeshProUGUI BossTitle;

		[SerializeField, BoxGroup("References"), Required]
		private CriticalHealthOverlay criticalHealthOverlay;

		[field: SerializeField, BoxGroup("References"), Required]
		public Image Blackout { get; private set; }

		[field: SerializeField, BoxGroup("References"), Required]
		public GameObject GameOverRoot { get; private set; }

		private Character _lastCharacter;
		private Health _health;

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			PossessionManager.Instance.OnCharacterPossessed += OnCharacterPossessed;
			OnCharacterPossessed(PossessionManager.Instance.PossessedCharacter);

			HidePlayerHealth();
			HideBoss();
		}

		private void Update()
		{
			if (_health)
			{
				PlayerHealth.value = _health.Current / _health.Max;

				// Check for critical health to show the overlay
				if (_health.IsCritical && criticalHealthOverlay != null)
				{
					criticalHealthOverlay.ShowCriticalOverlay();
				}
				else
				{
					criticalHealthOverlay.HideCriticalOverlay();
				}
			}

			var p = PossessionManager.Instance;
			PossessionMeter.value = p.CurrentPossessionMeter / p.MaxPossessionMeter;

			if (Room.Current != null && Room.Current.Boss != null)
			{
				var hp = Room.Current.Boss.Health;
				BossHealth.value = hp.Current / hp.Max;
			}
		}

		public void InitiateGameOver()
		{
			if (GameManager.Instance.InGameOver) return;

			Time.timeScale = 0f;
			GameManager.Instance.InGameOver = true;

			CrosshairManager.Instance.enabled = false;
			GameOverRoot.SetActive(true);

			if (Room.Current != null)
			{
				Room.Current.StopBossMusic();
			}
		}

		public void RetryButton()
		{
			GameManager.Instance.InGameOver = false;
			GameOverRoot.SetActive(false);
			Time.timeScale = 1f;

			CrosshairManager.Instance.enabled = true;
			RoomManager.Instance.Reload();
		}

		public void OnBossRoomStart(Room room)
		{
			BossTitle.text = room.Boss.Title;
			ShowBoss();

			room.OnBossDeath += () =>
			{
				HideBoss();
			};
		}

		private void OnCharacterPossessed(Character character)
		{
			if (character == null)
			{
				// Handle the case when the character is null.
				return;
			}

			// Clean up old character.
			if (_lastCharacter != null)
			{
				var lastHealth = _lastCharacter.GetComponent<Health>();
				if (lastHealth != null)
				{
					lastHealth.OnDamaged -= OnDamaged;
					lastHealth.OnReachMaxHealth -= FadeOutPlayerHealth;
				}
			}

			// Update new character.
			if (character.TryGetComponent(out _health))
			{
				_health.OnDamaged += OnDamaged;
				_health.OnReachMaxHealth += FadeOutPlayerHealth;

				if (_health.IsCritical)
				{
					FadeInPlayerHealth();
				}
				else
				{
					FadeOutPlayerHealth();
				}
			}

			// Update _lastCharacter for use in next possession.
			_lastCharacter = character;
		}

		private void OnDamaged(float damage)
		{
			FadeInPlayerHealth();
		}

		private void FadeInPlayerHealth()
		{
			// This is not really needed as you never wander around and pretty much always need to see your heatlh bar.

			//var renderers = PlayerHealth.GetComponentsInChildren<Image>();

			//renderers[0].DOKill();
			//renderers[1].DOKill();

			//renderers[0].DOFade(1f, PlayerHealthFadeIn).SetEase(Ease.Linear);
			//renderers[1].DOFade(1f, PlayerHealthFadeIn).SetEase(Ease.Linear);

			//// Optional: Pulse animation for critical health
			//if (_health.IsCritical)
			//{
			//	var handleRectTransform = PlayerHealth.GetComponentInChildren<RectTransform>();

			//	handleRectTransform.DOScale(PlayerHealthCriticalPulseScale, PlayerHealthCriticalPulseDuration / 2)
			//			.SetEase(Ease.InOutSine)
			//			.OnComplete(() =>
			//			{
			//				handleRectTransform.DOScale(1f, PlayerHealthCriticalPulseDuration / 2)
			//				.SetEase(Ease.InOutSine);
			//			});
			//}

		}

		private void FadeOutPlayerHealth()
		{
			// This is not really needed as you never wander around and pretty much always need to see your heatlh bar.

			//var renderers = PlayerHealth.GetComponentsInChildren<Image>();

			//renderers[0].DOKill();
			//renderers[1].DOKill();

			//renderers[0].DOFade(0f, PlayerHealthFadeOut).SetEase(Ease.Linear);
			//renderers[1].DOFade(0f, PlayerHealthFadeOut).SetEase(Ease.Linear);
		}

		private void HidePlayerHealth()
		{
			// This is not really needed as you never wander around and pretty much always need to see your heatlh bar.

			//var renderers = PlayerHealth.GetComponentsInChildren<Image>();

			//var color = renderers[0].color;
			//color.a = 0f;
			//renderers[0].color = color;

			//color = renderers[1].color;
			//color.a = 0f;
			//renderers[1].color = color;
		}

		private void ShowBoss()
		{
			BossContainer.SetActive(true);
		}

		private void HideBoss()
		{
			BossContainer.SetActive(false);
		}
	}
}
